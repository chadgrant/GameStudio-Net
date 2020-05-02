using GameStudio.Serializers.Jil;

namespace GameStudio.Serializers.Tests
{
	public class JilJsonSerializerTests : JsonSerializerTests
	{
		public override ITextSerializer GetSerializer()
		{
			return new JilSerializer();
		}
	}

	public class JilJsonDeserializerTests : JsonDeserializerTests
	{
		public override ITextDeserializer GetSerializer()
		{
			return new JilSerializer();
		}
	}
}