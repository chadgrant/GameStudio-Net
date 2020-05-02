using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace GameStudio.WebApi
{
	public partial class StartupBase
	{
		protected readonly Container _container = new Container();
        protected bool _verifyContainer = true;

		protected virtual void ConfigureContainerServices(IServiceCollection services)
		{
			ConfigureConventionServices(Configuration, services);
			ConfigureApplicationServices(services);
            
			_container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
			//services.EnableSimpleInjectorCrossWiring(_container);
			services.UseSimpleInjectorAspNetRequestScoping(_container);

            _container.RegisterInstance(typeof(IServiceCollection), services);
        }

		protected virtual void ConfigureContainer(IApplicationBuilder app, IHostingEnvironment env)
		{
			ConfigureConventions(Configuration, app, env);
			//_container.RegisterMvcControllers(app);
			//_container.AutoCrossWireAspNetComponents(app);
			ConfigureApplication(_container, app, env);

            if (!env.IsProduction() && _verifyContainer)
			    _container.Verify();
		}

		public virtual void ConfigureApplication(Container container, IApplicationBuilder app, IHostingEnvironment env)
		{
		}

		public virtual void ConfigureApplicationServices(IServiceCollection services)
		{
		}
	}
}
