using System;
using System.IO;
using Jil;

namespace GameStudio.Serializers.Jil
{
	public class JilSerializer : TextBase, ITextDeserializer, ITextSerializer
	{
		public static Options DefaultOptions = new Options(
			excludeNulls: true, 
			includeInherited: true, 
			dateFormat:DateTimeFormat.ISO8601,
			unspecifiedDateTimeKindBehavior: UnspecifiedDateTimeKindBehavior.IsUTC
			//serializationNameFormat:SerializationNameFormat.CamelCase (would be preferred)
		);

        readonly Options _options;

		public JilSerializer(Options options)
		{
			_options = options;
		}

		public JilSerializer() : this(DefaultOptions)
		{
		}

		public override T Deserialize<T>(string str)
		{
			return JSON.Deserialize<T>(str, _options);
		}

		public override T Deserialize<T>(TextReader reader)
		{
			return (T)Deserialize(reader, typeof(T));
		}

		public override object Deserialize(TextReader reader, Type type)
		{
			return JSON.Deserialize(reader, type, _options);
		}

		public override object Deserialize(string str, Type type)
		{
			return JSON.Deserialize(str, type, _options);
		}

		public override string Serialize<T>(T obj)
		{
			return Serialize(obj, typeof(T));
		}

		public override void Serialize<T>(TextWriter writer, T obj)
		{
			JSON.Serialize(obj, writer, _options);
		}

		public override void Serialize(TextWriter writer, object obj)
		{
			JSON.Serialize(obj, writer, _options);
		}

		public override void Serialize(TextWriter writer, object obj, Type type)
        {
            JSON.Serialize(obj, writer, _options);
        }

		public string Serialize(object obj)
        {
            if (obj == null)
                return "null";

			return Serialize(obj, obj.GetType());
		}

		public override string Serialize(object obj, Type type)
		{
			return JSON.Serialize(obj, _options);
		}
	}
}
