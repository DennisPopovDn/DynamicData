using System;
using System.Linq;
using DynamicData.Tests.Domain;
using FluentAssertions;
using Xunit;

namespace DynamicData.Tests.Cache
{
    
    public class DeferAnsdSkipFixture
    {
        [Fact]
        public void DeferUntilLoadedDoesNothingUntilDataHasBeenReceived()
        {
            bool updateReceived = false;
            IChangeSet<Person, string> result = null;

            var cache = new SourceCache<Person, string>(p => p.Name);

            var deferStream = cache.Connect().DeferUntilLoaded()
                                   .Subscribe(changes =>
                                   {
                                       updateReceived = true;
                                       result = changes;
                                   });

            updateReceived.Should().BeFalse();
            cache.AddOrUpdate(new Person("Test", 1));

            updateReceived.Should().BeTrue();
            result.Adds.Should().Be(1);
            result.First().Current.Should().Be(new Person("Test", 1));
            deferStream.Dispose();
        }

        [Fact]
        public void SkipInitialDoesNotReturnTheFirstBatchOfData()
        {
            bool updateReceived = false;

            var cache = new SourceCache<Person, string>(p => p.Name);

            var deferStream = cache.Connect().SkipInitial()
                                   .Subscribe(changes => updateReceived = true);

            updateReceived.Should().BeFalse();

            cache.AddOrUpdate(new Person("P1", 1));

            updateReceived.Should().BeFalse();

            cache.AddOrUpdate(new Person("P2", 2));
            updateReceived.Should().BeTrue();
            deferStream.Dispose();
        }
    }
}
