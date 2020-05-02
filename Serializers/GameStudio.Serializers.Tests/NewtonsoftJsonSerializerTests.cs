using Xunit;

namespace GameStudio.Serializers.Tests
{
	public class NewtonsoftJsonSerializerTests : JsonSerializerTests
	{
		public override ITextSerializer GetSerializer()
		{
			return new NewtonsoftJsonSerializer();
		}
        
		[Theory,
		InlineData(TestEnum.NegativeOne, "NegativeOne"), 
		InlineData(TestEnum.One, "One"), 
		InlineData(TestEnum.Zero, "Zero")]
		public void Serializer_Enum_Should_Be_String_Value(TestEnum value, string valueString)
		{
			var serializer = GetSerializer();

			var obj = new ClassWithTestEnum()
			{
				TestEnum = value
			};

			var json = serializer.Serialize(obj);

			Assert.Contains(valueString,json);
		}
	}

	public class NewtonsoftJsonDeserializerTests : JsonDeserializerTests
	{
		public override ITextDeserializer GetSerializer()
		{
			return new NewtonsoftJsonSerializer();
		}
	}
}
