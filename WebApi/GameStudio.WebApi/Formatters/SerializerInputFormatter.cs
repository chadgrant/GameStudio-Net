using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using GameStudio.Serializers;

namespace GameStudio.WebApi
{
	public class SerializerInputFormatter : TextInputFormatter
	{
		readonly ITextDeserializer _serializer;

		public SerializerInputFormatter(ITextDeserializer serializer, params string[] suportedMesdiaTypes)
		{
			_serializer = serializer;

			SupportedEncodings.Add(Encoding.UTF8);
			SupportedEncodings.Add(Encoding.Unicode);

			foreach(var mt in suportedMesdiaTypes)
				SupportedMediaTypes.Add(mt);
		}

		public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (encoding == null)
				throw new ArgumentNullException(nameof(encoding));

			
			using (var reader = context.ReaderFactory(context.HttpContext.Request.Body, encoding))
			{
				var type = context.ModelType;

				try
				{
					var model = _serializer.Deserialize(reader, type);
					return InputFormatterResult.SuccessAsync(model);
				}
				catch (Exception)
				{
					return InputFormatterResult.FailureAsync();
				}
			}
		}

	}
}
