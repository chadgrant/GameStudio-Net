using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace GameStudio.WebApi
{
	public abstract partial class StartupBase
	{
        /// <summary>
        /// Requires basic information about the service
        /// new ApiInfo
        ///{
        ///		Title = "Re-Tars",
        ///		Version = 1
        ///		Description = "This is a a great service",
        ///		Contact = new ApiContact { Name = "Mike Hunt", Email = "mikeh@gamestudio.com" },
        ///	};
        /// </summary>
        protected abstract ApiInfo CreateInfoForApiVersion(ApiVersionDescription version);

		public virtual void ConfigureSwaggerServices(IServiceCollection services)
		{
			services.AddApiVersioning();
			services.AddSwaggerGen(opt =>
			{
				// using (var serviceProvider = services.BuildServiceProvider())
				// {
				// 	var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
				// 	foreach (var description in provider.ApiVersionDescriptions)
				// 		opt.SwaggerDoc(description.GroupName, ToInfo(CreateInfoForApiVersion(description)));
				// }

				//opt.OperationFilter<SwaggerDefaultValues>();

				//Load the xml documentation for assemblies
				foreach (var assm in Assemblies)
				{
					var xmlDocs = Path.Combine(AppContext.BaseDirectory, $"{assm.GetName().Name}.xml");
					if (File.Exists(xmlDocs))
						opt.IncludeXmlComments(xmlDocs);
				}
			});
		}

		public virtual void ConfigureSwagger(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseSwagger().UseSwaggerUI(opt =>
			{
				var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
				foreach (var description in provider.ApiVersionDescriptions)
					opt.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
			});
		}
	}
}
