using System;
using Xunit;

namespace GameStudio.Serializers.Tests
{
	/// <summary>
	/// Simple tests Test Text should always deserialize an object.
	/// With a Property Name that Equals "Unit Tests"
	/// </summary>
	public abstract class TextDeserializerTests
	{
		public abstract ITextDeserializer GetSerializer();

		public abstract string GetTestText();

		[Fact]
		public void Serializer_Should_Deserialize()
		{
			var serializer = GetSerializer();

			var str = GetTestText();

			Assert.False(string.IsNullOrWhiteSpace(str));

			var deserialized = serializer.Deserialize<TestSerializedObject>(str);

			Assert.NotNull(deserialized);
			Assert.Equal("Unit Tests", deserialized.Name);
			Assert.False(string.IsNullOrWhiteSpace(deserialized.Name));
		}
	}
}