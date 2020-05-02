using System;
using Xunit;

namespace GameStudio.Tests
{
    public class EuidTests
    {
        [Fact]
        public void Can_Create_Euidv1()
        {
            var id = EuidV1.Create(Studio.Burlingame, Application.MyVegasWeb, 1123453657);

            Assert.Equal("00000101-8ad9-42f6-0000-000000000000", id.ToString());
        }

        [Fact]
        public void Can_Parse_EuidV1()
        {
            var euid = EuidV1.Parse("00000101-8ad9-42f6-0000-000000000000");

            Assert.Equal(Studio.Burlingame, euid.Studio);
            Assert.Equal(Application.MyVegasWeb, euid.Application);
            Assert.Equal((uint)1123453657, euid.PlayerId);
        }

        [Fact]
        public void Can_Create_Euid()
        {
            var rand = new byte[] { 0,0,0,0 };

            var dt = new DateTime(2019,1,1,1,1,1, DateTimeKind.Utc);

            var id = Euid.Create(Studio.Burlingame,Application.MyVegasWeb, 11234, 1123453657,  dt, rand);

            Assert.Equal("2be20101-8ad9-42f6-0000-0000cdbb2a5c", id.ToString());
        }

        [Fact]
        public void Can_Parse_Euid()
        {
            var euid = Euid.Parse("2be20101-8ad9-42f6-0000-0000cdbb2a5c");

            Assert.Equal(new DateTime(2019, 1, 1, 1, 1, 1, DateTimeKind.Utc), euid.Created);
            Assert.Equal(Studio.Burlingame, euid.Studio);
            Assert.Equal(Application.MyVegasWeb,euid.Application);
            Assert.Equal(11234, euid.Type);
            Assert.Equal((uint)1123453657,euid.PlayerId);
        }
    }
}
