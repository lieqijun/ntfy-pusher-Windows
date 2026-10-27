using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace ntfysh_client.Notifications
{
    public class NotificationListener
    {
        public readonly Dictionary<string, SubscribedTopic?> SubscribedTopicsByUnique = new();
        public delegate void NotificationReceiveHandler(NotificationListener sender, NotificationReceiveEventArgs e);
        public event NotificationReceiveHandler? OnNotificationReceive;

        public delegate void ConnectionErrorHandler(NotificationListener sender, SubscribedTopic topic);
        public event ConnectionErrorHandler? OnConnectionMultiAttemptFailure;
        public event ConnectionErrorHandler? OnConnectionCredentialsFailure;

        public NotificationListener()
        {
            ServicePointManager.DefaultConnectionLimit = 100;
        }

        private async Task ListenToTopicWithHttpLongJsonAsync(HttpRequestMessage message, SubscribedTopic topic, int reconnectAttempts = 10, int reconnectAttemptDelay = 3, CancellationToken cancellationToken = default)
        {
            int connectionAttempts = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (reconnectAttempts != 0 && connectionAttempts >= reconnectAttempts)
                {
                    OnConnectionMultiAttemptFailure?.Invoke(this, topic);
                    return;
                }
                try
                {
                    using HttpClient client = new();
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    using HttpResponseMessage response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    await using Stream body = await response.Content.ReadAsStreamAsync(cancellationToken);

                    response.EnsureSuccessStatusCode();
                    connectionAttempts = 0;

                    StringBuilder mainBuffer = new();
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        byte[] buffer = new byte[8192];
                        int readBytes = await body.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        mainBuffer.Append(Encoding.UTF8.GetString(buffer, 0, readBytes));
                        List<string> lines = mainBuffer.ToString().Split('\n').ToList();
                        if (lines.Count <= 1) continue;

                        int partialLineIndex = lines.Count - 1;
                        string partialLine = lines[partialLineIndex];
                        lines.RemoveAt(partialLineIndex);

                        foreach (string line in lines) ProcessMessage(topic, line);

                        mainBuffer.Clear();
                        mainBuffer.Append(partialLine);
                    }
                }
                catch (HttpRequestException hre)
                {
                    if (hre.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                    {
                        OnConnectionCredentialsFailure?.Invoke(this, topic);
                        return;
                    }
#if DEBUG
                    Debug.WriteLine(hre);
#endif
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        if (reconnectAttempts == 0 || connectionAttempts != 0)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(reconnectAttemptDelay), cancellationToken);
                        }
                        connectionAttempts++;
                    }
                }
            }
        }

        private async Task ListenToTopicWithWebsocketAsync(Uri uri, string? bearerHeader, SubscribedTopic topic, int reconnectAttempts = 10, int reconnectAttemptDelay = 3, CancellationToken cancellationToken = default)
        {
            int connectionAttempts = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (reconnectAttempts != 0 && connectionAttempts >= reconnectAttempts)
                {
                    OnConnectionMultiAttemptFailure?.Invoke(this, topic);
                    return;
                }
                try
                {
                    using ClientWebSocket socket = new();
                    if (!string.IsNullOrWhiteSpace(bearerHeader))
                    {
                        socket.Options.SetRequestHeader("Authorization", bearerHeader);
                    }
                    await socket.ConnectAsync(uri, cancellationToken);
                    connectionAttempts = 0;

                    StringBuilder mainBuffer = new();
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        byte[] buffer = new byte[8192];
                        WebSocketReceiveResult? result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                        mainBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        List<string> lines = mainBuffer.ToString().Split('\n').ToList();
                        if (lines.Count <= 1) continue;

                        int partialLineIndex = lines.Count - 1;
                        string partialLine = lines[partialLineIndex];
                        lines.RemoveAt(partialLineIndex);

                        foreach (string line in lines) ProcessMessage(topic, line);

                        mainBuffer.Clear();
                        mainBuffer.Append(partialLine);
                    }
                }
                catch (WebSocketException)
                {
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        if (reconnectAttempts == 0 || connectionAttempts != 0)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(reconnectAttemptDelay), cancellationToken);
                        }
                        connectionAttempts++;
                    }
                }
            }
        }

        private void ProcessMessage(SubscribedTopic topic, string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
            NtfyEvent? evt = JsonConvert.DeserializeObject<NtfyEvent>(message);
            if (evt is null) return;
            if (evt.Event == "message")
            {
                OnNotificationReceive?.Invoke(this, new NotificationReceiveEventArgs(topic, evt.Title ?? "", evt.Message, evt.Priority ?? NotificationPriority.Default));
            }
        }

        public void SubscribeToTopicUsingLongHttpJson(string unique, string topicId, string serverUrl, string? bearerToken, int reconnectAttempts, int reconnectAttemptDelay)
        {
            if (SubscribedTopicsByUnique.ContainsKey(unique))
                throw new ArgumentException("A topic with this unique already exists", nameof(unique));

            if (reconnectAttempts < 0) throw new ArgumentException("Reconnect attempts must be 0 or more", nameof(reconnectAttempts));
            if (reconnectAttemptDelay < 0) throw new ArgumentException("Reconnect attempt delay; must be 0 or more", nameof(reconnectAttemptDelay));

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"{serverUrl}/{HttpUtility.UrlEncode(topicId)}/json");

            if (!string.IsNullOrEmpty(bearerToken))
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            SubscribedTopic newTopic = new SubscribedTopic(topicId, serverUrl, bearerToken);
            CancellationTokenSource listenCanceller = new();
            Task listenTask = ListenToTopicWithHttpLongJsonAsync(message, newTopic, reconnectAttempts, reconnectAttemptDelay, listenCanceller.Token);

            newTopic.SetAssociatedRunner(listenTask, listenCanceller);
            SubscribedTopicsByUnique.Add(unique, newTopic);
        }

        public void SubscribeToTopicUsingWebsocket(string unique, string topicId, string serverUrl, string? bearerToken, int reconnectAttempts, int reconnectAttemptDelay)
        {
            if (SubscribedTopicsByUnique.ContainsKey(unique))
                throw new ArgumentException("A topic with this unique already exists", nameof(unique));

            if (reconnectAttempts < 0) throw new ArgumentException("Reconnect attempts must be 0 or more", nameof(reconnectAttempts));
            if (reconnectAttemptDelay < 0) throw new ArgumentException("Reconnect attempt delay; must be 0 or more", nameof(reconnectAttemptDelay));

            SubscribedTopic newTopic = new SubscribedTopic(topicId, serverUrl, bearerToken);
            string? bearerHeader = !string.IsNullOrEmpty(bearerToken) ? $"Bearer {bearerToken}" : null;

            CancellationTokenSource listenCanceller = new();
            Task listenTask = ListenToTopicWithWebsocketAsync(new Uri($"{serverUrl}/{HttpUtility.UrlEncode(topicId)}/ws"), bearerHeader, newTopic, reconnectAttempts, reconnectAttemptDelay, listenCanceller.Token);

            newTopic.SetAssociatedRunner(listenTask, listenCanceller);
            SubscribedTopicsByUnique.Add(unique, newTopic);
        }

        public async Task UnsubscribeFromTopicAsync(string topicUniqueString)
        {
#if DEBUG
            Debug.WriteLine($"Removing topic {topicUniqueString}");
#endif
            SubscribedTopic topic;
            if (!SubscribedTopicsByUnique.TryGetValue(topicUniqueString, out topic!)) return;

            topic.RunnerCanceller?.Cancel();
            try
            {
                if (topic.Runner is not null) await topic.Runner;
            }
            catch (Exception)
            {
            }
            topic.Runner?.Dispose();
            SubscribedTopicsByUnique.Remove(topicUniqueString);
        }
    }
}
