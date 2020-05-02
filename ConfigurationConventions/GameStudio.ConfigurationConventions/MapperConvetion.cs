using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStudio.ConfigurationConventions
{
    public class MapperConvention : IConfigureServices
    {
        readonly IEnumerable<Type> _types;

        static readonly Func<Type, bool> Convention =
            type => !type.IsAbstract &&
                    type.Name.EndsWith("Mapper") &&
                    type.GetConstructor(Type.EmptyTypes) != null;

        public MapperConvention(IEnumerable<Type> types)
        {
            _types = types;
        }

        public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            var mappers = _types.Where(Convention);

            foreach (var mapper in mappers)
                services.AddSingleton(mapper);
        }
    }
}
