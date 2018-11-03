﻿using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testinvi.Helpers;
using Testinvi.SetupHelpers;
using Tweetinvi.Controllers.Messages;
using Tweetinvi.Core.Web;
using Tweetinvi.Models.DTO;
using Tweetinvi.Parameters;

namespace Testinvi.TweetinviControllers.MessageTests
{
    [TestClass]
    public class MessageQueryExecutorTests
    {
        private FakeClassBuilder<MessageQueryExecutor> _fakeBuilder;

        private IMessageQueryGenerator _messageQueryGenerator;
        private ITwitterAccessor _twitterAccessor;

        [TestInitialize]
        public void TestInitialize()
        {
            _fakeBuilder = new FakeClassBuilder<MessageQueryExecutor>();
            _messageQueryGenerator = _fakeBuilder.GetFake<IMessageQueryGenerator>().FakedObject;
            _twitterAccessor = _fakeBuilder.GetFake<ITwitterAccessor>().FakedObject;
        }

        #region GetLatestMessages
        
        private void ArrangeQueryGeneratorGetLatestMessagesReceived(int count, string query)
        {
            A.CallTo(() =>
                    _messageQueryGenerator.GetLatestMessagesQuery(
                        A<IGetMessagesParameters>.That.Matches(p => p.Count == count)))
                .Returns(query);
        }

        #endregion

        #region Publish Message

        [TestMethod]
        public void PublishMessage_WithCreateMessageDTO_ReturnsTwitterAccessor()
        {
            // Arrange
            var queryExecutor = CreateMessageQueryExecutor();
            var parameters = A.Fake<IPublishMessageParameters>();
            var reqDTO = A.Fake<ICreateMessageDTO>();
            var resDTO = A.Fake<ICreateMessageDTO>();
            var query = TestHelper.GenerateString();

            ArrangeQueryGeneratorPublishMessage(parameters, query, reqDTO);
            _twitterAccessor.ArrangeExecutePostQueryJsonBody(query, reqDTO, resDTO);

            // Act
            var result = queryExecutor.PublishMessage(parameters);

            // Assert
            Assert.AreEqual(result, resDTO);
        }

        private void ArrangeQueryGeneratorPublishMessage(IPublishMessageParameters parameters, string query,
            ICreateMessageDTO createMessageDTO)
        {
            A.CallTo(() => _messageQueryGenerator.GetPublishMessageQuery(parameters)).Returns(query);
            A.CallTo(() => _messageQueryGenerator.GetPublishMessageBody(parameters)).Returns(createMessageDTO);
        }

        #endregion

        #region Destroy Message

        [TestMethod]
        public void DestroyMessage_WithMessageDTO_ReturnsTwitterAccessorResult()
        {
            // Arrange - Act
            var result1 = DestroyMessage_WithMessageDTO_Returns(true);
            var result2 = DestroyMessage_WithMessageDTO_Returns(false);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        public bool DestroyMessage_WithMessageDTO_Returns(bool expectedResult)
        {
            // Arrange
            var queryExecutor = CreateMessageQueryExecutor();
            var eventDTO = A.Fake<IEventDTO>();
            var query = Guid.NewGuid().ToString();

            ArrangeQueryGeneratorDestroyMessage(eventDTO, query);
            _twitterAccessor.ArrangeTryExecuteDELETEQuery(query, expectedResult);

            // Act
            return queryExecutor.DestroyMessage(eventDTO);
        }

        private void ArrangeQueryGeneratorDestroyMessage(IEventDTO eventDTO, string query)
        {
            A.CallTo(() => _messageQueryGenerator.GetDestroyMessageQuery(eventDTO)).Returns(query);
        }

        [TestMethod]
        public void DestroyMessage_WithMessageId_ReturnsTwitterAccessorResult()
        {
            // Arrange - Act
            var result1 = DestroyMessage_WithMessageId_Returns(true);
            var result2 = DestroyMessage_WithMessageId_Returns(false);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        public bool DestroyMessage_WithMessageId_Returns(bool expectedResult)
        {
            // Arrange
            var queryExecutor = CreateMessageQueryExecutor();
            var messageId = new Random().Next();
            var query = Guid.NewGuid().ToString();

            ArrangeQueryGeneratorDestroyMessage(messageId, query);
            _twitterAccessor.ArrangeTryExecuteDELETEQuery(query, expectedResult);

            // Act
            return queryExecutor.DestroyMessage(messageId);
        }

        private void ArrangeQueryGeneratorDestroyMessage(long userId, string query)
        {
            A.CallTo(() => _messageQueryGenerator.GetDestroyMessageQuery(userId)).Returns(query);
        }

        #endregion

        public MessageQueryExecutor CreateMessageQueryExecutor()
        {
            return _fakeBuilder.GenerateClass();
        }
    }
}