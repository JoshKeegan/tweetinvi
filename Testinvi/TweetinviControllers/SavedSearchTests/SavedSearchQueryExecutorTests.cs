﻿using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testinvi.Helpers;
using Testinvi.SetupHelpers;
using Tweetinvi.Controllers.SavedSearch;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;

namespace Testinvi.TweetinviControllers.SavedSearchTests
{
    [TestClass]
    public class SavedSearchQueryExecutorTests
    {
        private FakeClassBuilder<SavedSearchQueryExecutor> _fakeBuilder;

        private ISavedSearchQueryGenerator _savedSearchQueryGenerator;
        private ITwitterAccessor _twitterAccessor;

        [TestInitialize]
        public void TestInitialize()
        {
            _fakeBuilder = new FakeClassBuilder<SavedSearchQueryExecutor>();
            _savedSearchQueryGenerator = _fakeBuilder.GetFake<ISavedSearchQueryGenerator>().FakedObject;
            _twitterAccessor = _fakeBuilder.GetFake<ITwitterAccessor>().FakedObject;
        }

        [TestMethod]
        public void GetSavedSearches_ReturnsTwitterAccessorResult()
        {
            // Arrange
            var controller = CreateSavedSearchQueryExecutor();
            string query = TestHelper.GenerateString();
            IEnumerable<ISavedSearchDTO> expectedResult = new List<ISavedSearchDTO>();

            A.CallTo(() => _savedSearchQueryGenerator.GetSavedSearchesQuery()).Returns(query);
            _twitterAccessor.ArrangeExecuteGETQuery(query, expectedResult);

            // Act
            var result = controller.GetSavedSearches();

            // Assert
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void DestroySavedSearch_WithSavedSearchObject_ReturnsTwitterAccessorResult()
        {
            // Arrange - Act
            var result1 = DestroySavedSearch_WithSavedSearchObject(true);
            var result2 = DestroySavedSearch_WithSavedSearchObject(false);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        private bool DestroySavedSearch_WithSavedSearchObject(bool expectedResult)
        {
            // Arrange
            var controller = CreateSavedSearchQueryExecutor();
            string query = TestHelper.GenerateString();
            var savedSearch = A.Fake<ISavedSearch>();

            A.CallTo(() => _savedSearchQueryGenerator.GetDestroySavedSearchQuery(savedSearch)).Returns(query);
            _twitterAccessor.ArrangeTryExecutePOSTQuery(query, expectedResult);

            // Act
            return controller.DestroySavedSearch(savedSearch);
        }

        [TestMethod]
        public void DestroySavedSearch_WithSavedSearchId_ReturnsTwitterAccessorResult()
        {
            // Arrange - Act
            var result1 = DestroySavedSearch_WithSavedSearchId(true);
            var result2 = DestroySavedSearch_WithSavedSearchId(false);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        private bool DestroySavedSearch_WithSavedSearchId(bool expectedResult)
        {
            // Arrange
            var controller = CreateSavedSearchQueryExecutor();
            string query = TestHelper.GenerateString();
            var searchId = TestHelper.GenerateRandomLong();

            A.CallTo(() => _savedSearchQueryGenerator.GetDestroySavedSearchQuery(searchId)).Returns(query);
            _twitterAccessor.ArrangeTryExecutePOSTQuery(query, expectedResult);

            // Act
            return controller.DestroySavedSearch(searchId);
        }

        public SavedSearchQueryExecutor CreateSavedSearchQueryExecutor()
        {
            return _fakeBuilder.GenerateClass();
        }
    }
}