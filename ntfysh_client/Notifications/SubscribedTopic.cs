using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ntfysh_client.Notifications
{
    public class SubscribedTopic
    {
        [JsonConstructor]
        public SubscribedTopic(string topicId, string serverUrl, string? bearerToken = null)
        {
            TopicId = topicId;
            ServerUrl = serverUrl;
            BearerToken = bearerToken;
        }

        public void SetAssociatedRunner(Task runner, CancellationTokenSource runnerCanceller)
        {
            if (Runner is not null || RunnerCanceller is not null)
                throw new InvalidOperationException("Runner already associated");
            Runner = runner;
            RunnerCanceller = runnerCanceller;
        }

        public string TopicId { get; }
        public string ServerUrl { get; }
        public string? BearerToken { get; }

        [JsonIgnore]
        public Task? Runner { get; private set; }
        [JsonIgnore]
        public CancellationTokenSource? RunnerCanceller { get; private set; }
    }
}
