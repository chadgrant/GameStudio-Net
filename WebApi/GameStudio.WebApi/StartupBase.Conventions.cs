using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GameStudio.ConfigurationConventions;
using GameStudio.Extensions;

namespace GameStudio.WebApi
{
    public partial class StartupBase
    {
	    protected static readonly Assembly[] Assemblies =
		    Directory.GetFiles(AppContext.BaseDirectory, "GameStudio*.dll")
			    .Select(p => AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(p, AppContext.BaseDirectory)))
			    .ToArray();
        
        protected Lazy<Type[]> ExportedTypes = new Lazy<Type[]>(() => { return Assemblies.SelectMany(asm => asm.ExportedTypes).ToArray(); });

	    static Func<Type, bool> ConfigureServicesConvention => type =>
		    !type.IsAbstract &&
		    type.GetConstructor(Type.EmptyTypes) != null &&
		    type.IsAssignableTo<IConfigureServices>();

		static Func<Type, bool> ConfigureConvention => type =>
		    !type.IsAbstract &&
		    type.IsAssignableTo<IConfigure>();

		public virtual void ConfigureConventionServices(IConfiguration configuration, IServiceCollection services)
		{
			GetConfigureServicesConventions(services)
				.ForEach(i=> i.ConfigureServices(configuration,services));
		}

	    public virtual void ConfigureConventions(IConfiguration configuration, IApplicationBuilder app, IHostingEnvironment env)
	    {
		    GetConfigureConventions(app.ApplicationServices)
				.ForEach(i => i.Configure(configuration,app,env));
		}

	    protected virtual IEnumerable<IConfigureServices> GetConfigureServicesConventions(IServiceCollection services)
	    {
		    return Assemblies.SelectMany(a => a.ExportedTypes.Where(ConfigureServicesConvention))
			    .Select(t=> (IConfigureServices) Activator.CreateInstance(t));
	    }

	    protected virtual IEnumerable<IConfigure> GetConfigureConventions(IServiceProvider provider)
	    {
		    return Assemblies.SelectMany(a => a.ExportedTypes.Where(ConfigureConvention))
			    .Select(t => (IConfigure)ActivatorUtilities.GetServiceOrCreateInstance(provider, t));
		}
	}
}
