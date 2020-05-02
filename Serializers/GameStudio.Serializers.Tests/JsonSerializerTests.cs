using System;
using GameStudio.Extensions;
using Xunit;

namespace GameStudio.Serializers.Tests
{
	public abstract class JsonSerializerTests : TextSerializerTests
	{
		[Fact]
		public void Serializer_Date_Should_Serialize_ISO_8601_UTC()
		{
			var serializer = GetSerializer();

			var obj = new ClassWithDate(new DateTime(1974, 1, 6, 10, 31, 0, DateTimeKind.Utc));

			var json = serializer.Serialize(obj);

			Assert.True(json.Contains("1974-01-06T10:31:00Z"), "Incorrect DateTime Format");
		}
        
        [Fact]
        public void Serializes_Null()
        {
            var ser = new NewtonsoftJsonSerializer();
            var x = ser.Serialize((object)null, typeof(ClassWithTestEnum));
            Assert.Equal("null", x);
        }

        [Fact]
		public void Serializer_Should_Serialize_Json_Generic()
		{
			var serializer = GetSerializer();

			var serialized = serializer.Serialize<TestSerializedObject>(GetTestObject());

			Assert.False(string.IsNullOrWhiteSpace(serialized));
			Assert.True(serialized.IsJson());
		}

		[Fact]
		public void Serializer_Should_Serialize_Json()
		{
			var serializer = GetSerializer();

			object test = GetTestObject();
			var serialized = serializer.Serialize(test);

			Assert.False(string.IsNullOrWhiteSpace(serialized));
			Assert.True(serialized.IsJson());
		}


		[Fact(Skip = "Manual Run")]
		public void Serializer_SpeedTest()
		{
			var max = 1000000;
			var serializer = GetSerializer();
			var obj = GetTestObject();

			for (var i = 0; i < max; i++)
			{
				serializer.Serialize<TestSerializedObject>(obj);
			}
		}
	}
}
