using Google.Cloud.PubSub.V1;

namespace SGT.Riachuelo.Service
{
    public class PubSub
    {
        #region Constructor
        public PubSub() : base() { }
        #endregion

        #region Metodos Publicos

        public async Task<int> PublicarMensagemAsyn(string projectId, string topicId, List<string> messageTexts)
        {
            TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            int publishedMessageCount = 0;
            var publishTasks = messageTexts.Select(async text =>
            {
                string message = await publisher.PublishAsync(text);
                Interlocked.Increment(ref publishedMessageCount);

            });
            await Task.WhenAll(publishTasks);
            return publishedMessageCount;
        }

        #endregion
    }
}
