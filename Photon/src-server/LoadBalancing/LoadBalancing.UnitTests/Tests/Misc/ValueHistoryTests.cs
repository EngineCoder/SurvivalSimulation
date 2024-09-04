using NUnit.Framework;
using Photon.Common.LoadBalancer.LoadShedding;

namespace Photon.LoadBalancing.UnitTests.Tests.Misc
{
    [TestFixture]
    public class ValueHistoryTests
    {
        [Test]
        public void CapacityUpdateTest()
        {
            const int Capacity = 100;
            var h = new ValueHistory(Capacity);

            for (int i = 0; i < Capacity + 10; ++i)
            {
                h.Add(i);
            }

            Assert.That(h.Count, Is.EqualTo(Capacity));

            const int BiggerCapacity = 110;
            h.UpdateCapacity(BiggerCapacity);

            Assert.That(h.Count, Is.EqualTo(Capacity));

            for (int i = 0; i < BiggerCapacity; ++i)
            {
                h.Add(i);
            }
            Assert.That(h.Count, Is.EqualTo(BiggerCapacity));

            const int SmallerCapacity = 10;

            h.UpdateCapacity(SmallerCapacity);
            Assert.That(h.Count, Is.EqualTo(SmallerCapacity));
        }
    }
}
