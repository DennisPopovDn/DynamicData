﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData.Tests.Domain;
using NUnit.Framework;

namespace DynamicData.Tests.ListFixtures
{
    [TestFixture]
    public class CloneFixture
    {
        private ICollection<Person> _collection = new Collection<Person>();
        private ISourceCache<Person, string> _source;

        private IDisposable _cloner;
        private readonly RandomPersonGenerator _generator = new RandomPersonGenerator();

        [SetUp]
        public void SetUp()
        {
            _collection = new Collection<Person>();
            _source = new SourceCache<Person, string>(p => p.Name);
            _cloner = _source.Connect()
                             .Clone(_collection)
                             .Subscribe();
        }

        [TearDown]
        public void CleanUp()
        {
            _cloner.Dispose();
            _source.Dispose();
        }

        [Test]
        public void AddToSourceAddsToDestination()
        {
            var person = new Person("Adult1", 50);
            _source.AddOrUpdate(person);

            Assert.AreEqual(1, _collection.Count, "Should be 1 item in the collection");
            Assert.AreEqual(person, _collection.First(), "Should be same person");
        }

        [Test]
        public void UpdateToSourceUpdatesTheDestination()
        {
            var person = new Person("Adult1", 50);
            var personUpdated = new Person("Adult1", 51);
            _source.AddOrUpdate(person);
            _source.AddOrUpdate(personUpdated);

            Assert.AreEqual(1, _collection.Count, "Should be 1 item in the collection");
            Assert.AreEqual(personUpdated, _collection.First(), "Should be updated person");
        }

        [Test]
        public void RemoveSourceRemovesFromTheDestination()
        {
            var person = new Person("Adult1", 50);
            _source.AddOrUpdate(person);
            _source.Remove(person);

            Assert.AreEqual(0, _collection.Count, "Should be 1 item in the collection");
        }

        [Test]
        public void BatchAdd()
        {
            var people = _generator.Take(100).ToList();
            _source.AddOrUpdate(people);

            Assert.AreEqual(100, _collection.Count, "Should be 100 items in the collection");
            CollectionAssert.AreEquivalent(people, _collection, "Collections should be equivalent");
        }

        [Test]
        public void BatchRemove()
        {
            var people = _generator.Take(100).ToList();
            _source.AddOrUpdate(people);
            _source.Clear();
            Assert.AreEqual(0, _collection.Count, "Should be 100 items in the collection");
        }
    }
}
