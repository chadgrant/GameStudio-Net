using Xunit;

namespace GameStudio.Secrets.Tests
{
    public class SecretsTests
    {
        [Fact]
        public void Secrets_Are_Equal()
        {
            var s1 = new Secret("_", "one");
            var s2 = new Secret("_", "one");
            var s3 = new Secret("-", "three");

            Assert.True(s1.Equals(s2));
            Assert.Equal(s1.GetHashCode(), s2.GetHashCode());

            Assert.False(s1.Equals(s3));
            Assert.NotEqual(s1, s3);
            Assert.NotEqual(s1.GetHashCode(), s3.GetHashCode());
        }
    }
}
