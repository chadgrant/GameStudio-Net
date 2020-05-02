using System;
using System.IO;

namespace GameStudio.Serializers
{
	public interface ITextDeserializer
	{ 
		T Deserialize<T>(string str);
		T Deserialize<T>(TextReader reader);
		bool TryDeserialize<T>(string str, out T result);
		bool TryDeserialize<T>(TextReader reader, out T result);

		object Deserialize(string str, Type type);
		object Deserialize(TextReader reader, Type type);
		bool TryDeserialize(string str, Type type, out object result);
		bool TryDeserialize(TextReader reader, Type type, out object result);
	}
}