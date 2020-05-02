using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using GameStudio.Serializers.Jil;

namespace GameStudio.WebApi
{
	public partial class StartupBase
	{
		public virtual void ConfigureMvcServices(IServiceCollection services)
		{
			services.AddVersionedApiExplorer(
					opt =>
					{
						opt.GroupNameFormat = "'v'VVV";
						opt.SubstituteApiVersionInUrl = true;
					})
				.AddOptions()
				.AddRouting(r => r.LowercaseUrls = r.LowercaseQueryStrings = true)
				.AddMvcCore(ConfigureMvcOptions)
                .AddJsonOptions(JsonOptions)
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			// Api Version routing is not compatible with 2_2 yet
			// https://github.com/Microsoft/aspnet-api-versioning/issues/363
			// .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

        void JsonOptions(JsonOptions jsonOptions)
        {
            
        }

        protected virtual void ConfigureMvc(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseMvc();
		}

		public virtual void ConfigureMvcOptions(MvcOptions options)
		{
			// options.InputFormatters.RemoveType<JsonInputFormatter>();
			// options.InputFormatters.Add(new SerializerInputFormatter(new JilSerializer(), "application/json", "text/json"));
			//
			// options.OutputFormatters.RemoveType<JsonOutputFormatter>();
			// options.OutputFormatters.Add(new SerializerOutputFormatter(new JilSerializer(), "application/json","text/json"));

            foreach (var t in GetFilters())
                options.Filters.Add(t);
        }

        public virtual IEnumerable<Type> GetFilters()
        {
            return Type.EmptyTypes;
        }
    }
}
