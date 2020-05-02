using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameStudio.Serializers
{
	public class NewtonsoftJsonSerializer : TextBase, ITextDeserializer, ITextSerializer
	{
		public static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
		{
            //would be preffered
			//ContractResolver = new CamelCasePropertyNamesContractResolver(),
			NullValueHandling = NullValueHandling.Ignore,
			Converters = new JsonConverter[] { new StringEnumConverter()},
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,
			CheckAdditionalContent = false
		};

		readonly JsonSerializer _serializer;

		public NewtonsoftJsonSerializer() : this(DefaultSettings)
		{
		}

		public NewtonsoftJsonSerializer(JsonSerializerSettings settings)
		{
			_serializer = JsonSerializer.CreateDefault(settings);
		}

		public override T Deserialize<T>(string str)
		{
			return (T)Deserialize(str, typeof(T));
		}

		public override T Deserialize<T>(TextReader reader)
		{
			using (var jreader = new JsonTextReader(reader))
			{
				return (T)_serializer.Deserialize(jreader, typeof(T));
			}
		}
		
		public override object Deserialize(string str, Type type)
		{
			using (var reader = new JsonTextReader(new StringReader(str)))
			{
				return _serializer.Deserialize(reader, type);
			}
		}

		public override object Deserialize(TextReader reader, Type type)
		{
			using (var jreader = new JsonTextReader(reader))
			{
				return _serializer.Deserialize(jreader, type);
			}
		}

		public override string Serialize<T>(T obj)
		{
			return Serialize(obj, typeof(T));
		}

		public string Serialize(object obj)
		{
            if (obj == null)
                return "null";

            return Serialize(obj, obj.GetType());
		}

		public override string Serialize(object obj, Type type)
		{
			using (var sw = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture))
			{
				using (var jsonWriter = new JsonTextWriter(sw))
				{
					_serializer.Serialize(jsonWriter, obj, type);
				}
				return sw.ToString();
			}
		}

		public override void Serialize<T>(TextWriter writer, T obj)
		{
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				_serializer.Serialize(jsonWriter, obj);
			}
		}

		public override void Serialize(TextWriter writer, object obj)
		{
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				_serializer.Serialize(jsonWriter, obj);
			}
		}

		public override void Serialize(TextWriter writer, object obj, Type type)
		{
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				_serializer.Serialize(jsonWriter, obj, type);
			}
		}
	}
}
