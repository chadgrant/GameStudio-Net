using System;
using Xunit;

namespace GameStudio.Serializers.Tests
{
	public abstract class JsonDeserializerTests : TextDeserializerTests
	{
		[Fact]
		public void Serializer_Should_Deserialize_Json_Generic()
		{
			var serializer = GetSerializer();

			var deserialized = serializer.Deserialize<TestSerializedObject>("{ \"Name\" : \"Unit Tests\" }");

			Assert.NotNull(deserialized);
			Assert.Equal(typeof(TestSerializedObject), deserialized.GetType());
			Assert.False(string.IsNullOrWhiteSpace(deserialized.Name));
		}

		[Fact]
		public void Serializer_Date_Should_Deserialize_ISO_8601_UTC()
		{
			var serializer = GetSerializer();

			var json = "{ \"Date\" :\"1974-01-06T10:31:00Z\" }";

			var deserialized = serializer.Deserialize<ClassWithDate>(json);

			Assert.Equal(1974, deserialized.Date.Year);
			Assert.Equal(1, deserialized.Date.Month);
			Assert.Equal(6, deserialized.Date.Day);
			Assert.Equal(10, deserialized.Date.Hour);
			Assert.Equal(31,deserialized.Date.Minute);
			Assert.True(deserialized.Date.Kind == DateTimeKind.Utc);
		}

		[Fact]
		public void Serializer_NullableDate_With_Value_Should_Deserialize_ISO_8601_UTC()
		{
			var serializer = GetSerializer();

			var obj = new ClassWithNullableDate(new DateTime(1974, 1, 6, 10, 31, 0, DateTimeKind.Utc));

			var json = "{ \"Date\" :\"1974-01-06T10:31:00Z\" }";

			var deserialized = serializer.Deserialize<ClassWithDate>(json);

			Assert.Equal(obj.Date.GetValueOrDefault(DateTime.MinValue), deserialized.Date);
			Assert.Equal(obj.Date.GetValueOrDefault(DateTime.MinValue), deserialized.Date);
		}

		[Fact]
		public void Serializer_NullableDate_With_NullValue_Should_Deserialize_ISO_8601_UTC()
		{
			var serializer = GetSerializer();

			var json = "{ \"Date\" : null }";

			var deserialized = serializer.Deserialize<ClassWithNullableDate>(json);

			Assert.Null(deserialized.Date);

			json = "{ }";

			deserialized = serializer.Deserialize<ClassWithNullableDate>(json);

			Assert.Null(deserialized.Date);
		}

		[Fact]
		public void Should_Deserialize_Json()
		{
			var serializer = GetSerializer();

			var deserialized = serializer.Deserialize("{ \"Name\" : \"Unit Tests\" }", typeof(TestSerializedObject));

			Assert.NotNull(deserialized);
			Assert.Equal(typeof(TestSerializedObject), deserialized.GetType());
			Assert.False(string.IsNullOrWhiteSpace(((TestSerializedObject)deserialized).Name));
		}

		[Fact(Skip = "Manual Run")]
		public void Deserialize_SpeedTest()
		{
			var max = 1000000;
			var serializer = GetSerializer();
			var str = GetTestText();

			for (var i = 0; i < max; i++)
			{
				serializer.Deserialize<TestSerializedObject>(str);
			}
		}

		public override string GetTestText()
		{
			return "{ \"Name\" : \"Unit Tests\" }";
		}
	}
}
