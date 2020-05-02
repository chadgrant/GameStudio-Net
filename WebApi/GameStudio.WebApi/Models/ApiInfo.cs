using System;
using System.Collections.Generic;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;

namespace GameStudio.WebApi
{
	public class ApiInfo
	{
		public ApiInfo()
		{
			Extensions = new Dictionary<string, object>();
		}

		public string Version { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string TermsOfService { get; set; }

		public ApiContact Contact { get; set; }

		public ApiLicense License { get; set; }

		public Dictionary<string, object> Extensions { get; private set; }
	}

	public class ApiContact
	{
		public string Name { get; set; }

		public string Url { get; set; }

		public string Email { get; set; }
	}

	public class ApiLicense
	{
		public string Name { get; set; }

		public string Url { get; set; }
	}
}
