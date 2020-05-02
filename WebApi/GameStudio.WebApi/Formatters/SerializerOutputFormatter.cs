using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using GameStudio.Serializers;

namespace GameStudio.WebApi
{
	public class SerializerOutputFormatter : TextOutputFormatter
	{
		readonly ITextSerializer _serializer;

		public SerializerOutputFormatter(ITextSerializer serializer, params string[] supportedMediaTypes)
		{
			_serializer = serializer;
			SupportedEncodings.Add(Encoding.UTF8);
			SupportedEncodings.Add(Encoding.Unicode);

			foreach(var mt in supportedMediaTypes)
				SupportedMediaTypes.Add(mt);
		}

		public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding encoding)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			using (var writer = context.WriterFactory(context.HttpContext.Response.Body, encoding ?? Encoding.UTF8))
			{
				_serializer.Serialize(writer, context.Object);
				await writer.FlushAsync();
			}
		}

	}
}
