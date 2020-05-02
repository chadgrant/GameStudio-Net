using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Xunit;

namespace GameStudio.Serializers.Tests
{
	public interface ITestSerializedObject
	{
		string Name { get; set; }
	}

	[DataContract(Name = "TestSerializedObject", Namespace = "")]
	public class TestSerializedObject : ITestSerializedObject
	{
		[DataMember]
		public string Name { get; set; }
	}

	public abstract class TextSerializerTests
	{
		public abstract ITextSerializer GetSerializer();

		protected TestSerializedObject GetTestObject()
		{
			return new TestSerializedObject() { Name = "Unit Tests" };
		}

		[Fact]
		public void Serializer_Should_Serialize()
		{
			var serializer = GetSerializer();

			var obj = GetTestObject();

			var str = serializer.Serialize(obj);

			Assert.False(String.IsNullOrWhiteSpace(str));
		}

	}
}