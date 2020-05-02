using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
	//Shamelessly stolen from Scrutor https://github.com/khellang/Scrutor
	public static class DecoratorExtensions
	{
		/// <summary>
		/// Decorates all registered services of type <typeparamref name="TService"/>
		/// using the specified type <typeparamref name="TDecorator"/>.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <exception cref="MissingTypeRegistrationException">If no service of the type <typeparamref name="TService"/> has been registered.</exception>
		/// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
		public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
			where TDecorator : TService
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			return services.DecorateDescriptors(typeof(TService), x => x.Decorate(typeof(TDecorator)));
		}

		/// <summary>
		/// Decorates all registered services of type <typeparamref name="TService"/>
		/// using the specified type <typeparamref name="TDecorator"/>.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
		public static bool TryDecorate<TService, TDecorator>(this IServiceCollection services)
			where TDecorator : TService
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			return services.TryDecorateDescriptors(typeof(TService), x => x.Decorate(typeof(TDecorator)));
		}

		/// <summary>
		/// Decorates all registered services of the specified <paramref name="serviceType"/>
		/// using the specified <paramref name="decoratorType"/>.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <param name="serviceType">The type of services to decorate.</param>
		/// <param name="decoratorType">The type to decorate existing services with.</param>
		/// <exception cref="MissingTypeRegistrationException">If no service of the specified <paramref name="serviceType"/> has been registered.</exception>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
		/// <paramref name="serviceType"/> or <paramref name="decoratorType"/> arguments are <c>null</c>.</exception>
		public static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, Type decoratorType)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			if (decoratorType == null)
				throw new ArgumentNullException(nameof(decoratorType));

			if (IsGeneric(serviceType) && IsGeneric(decoratorType))
			{
				return services.DecorateOpenGeneric(serviceType, decoratorType);
			}

			return services.DecorateDescriptors(serviceType, x => x.Decorate(decoratorType));
		}

		/// <summary>
		/// Decorates all registered services of the specified <paramref name="serviceType"/>
		/// using the specified <paramref name="decoratorType"/>.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <param name="serviceType">The type of services to decorate.</param>
		/// <param name="decoratorType">The type to decorate existing services with.</param>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
		/// <paramref name="serviceType"/> or <paramref name="decoratorType"/> arguments are <c>null</c>.</exception>
		public static bool TryDecorate(this IServiceCollection services, Type serviceType, Type decoratorType)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			if (decoratorType == null)
				throw new ArgumentNullException(nameof(decoratorType));

			if (IsGeneric(serviceType) && IsGeneric(decoratorType))
			{
				return services.TryDecorateOpenGeneric(serviceType, decoratorType);
			}

			return services.TryDecorateDescriptors(serviceType, x => x.Decorate(decoratorType));
		}

		/// <summary>
		/// Decorates all registered services of type <typeparamref name="TService"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <typeparam name="TService">The type of services to decorate.</typeparam>
		/// <param name="services">The services to add to.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="MissingTypeRegistrationException">If no service of <typeparamref name="TService"/> has been registered.</exception>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
		/// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static IServiceCollection Decorate<TService>(this IServiceCollection services, Func<TService, IServiceProvider, TService> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.DecorateDescriptors(typeof(TService), x => x.Decorate(decorator));
		}

		/// <summary>
		/// Decorates all registered services of type <typeparamref name="TService"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <typeparam name="TService">The type of services to decorate.</typeparam>
		/// <param name="services">The services to add to.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
		/// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static bool TryDecorate<TService>(this IServiceCollection services, Func<TService, IServiceProvider, TService> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.TryDecorateDescriptors(typeof(TService), x => x.Decorate(decorator));
		}

		/// <summary>
		/// Decorates all registered services of type <typeparamref name="TService"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <typeparam name="TService">The type of services to decorate.</typeparam>
		/// <param name="services">The services to add to.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="MissingTypeRegistrationException">If no service of <typeparamref name="TService"/> has been registered.</exception>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
		/// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static IServiceCollection Decorate<TService>(this IServiceCollection services, Func<TService, TService> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.DecorateDescriptors(typeof(TService), x => x.Decorate(decorator));
		}

		/// <summary>
		/// Decorates all registered services of type <typeparamref name="TService"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <typeparam name="TService">The type of services to decorate.</typeparam>
		/// <param name="services">The services to add to.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
		/// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static bool TryDecorate<TService>(this IServiceCollection services, Func<TService, TService> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.TryDecorateDescriptors(typeof(TService), x => x.Decorate(decorator));
		}

		/// <summary>
		/// Decorates all registered services of the specified <paramref name="serviceType"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <param name="serviceType">The type of services to decorate.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="MissingTypeRegistrationException">If no service of the specified <paramref name="serviceType"/> has been registered.</exception>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
		/// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, Func<object, IServiceProvider, object> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.DecorateDescriptors(serviceType, x => x.Decorate(decorator));
		}

		/// <summary>
		/// Decorates all registered services of the specified <paramref name="serviceType"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <param name="serviceType">The type of services to decorate.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
		/// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static bool TryDecorate(this IServiceCollection services, Type serviceType, Func<object, IServiceProvider, object> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.TryDecorateDescriptors(serviceType, x => x.Decorate(decorator));
		}

		/// <summary>
		/// Decorates all registered services of the specified <paramref name="serviceType"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <param name="serviceType">The type of services to decorate.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="MissingTypeRegistrationException">If no service of the specified <paramref name="serviceType"/> has been registered.</exception>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
		/// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, Func<object, object> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.DecorateDescriptors(serviceType, x => x.Decorate(decorator));
		}

		/// <summary>
		/// Decorates all registered services of the specified <paramref name="serviceType"/>
		/// using the <paramref name="decorator"/> function.
		/// </summary>
		/// <param name="services">The services to add to.</param>
		/// <param name="serviceType">The type of services to decorate.</param>
		/// <param name="decorator">The decorator function.</param>
		/// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
		/// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
		public static bool TryDecorate(this IServiceCollection services, Type serviceType, Func<object, object> decorator)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));
			if (decorator == null)
				throw new ArgumentNullException(nameof(decorator));

			return services.TryDecorateDescriptors(serviceType, x => x.Decorate(decorator));
		}

		static IServiceCollection DecorateOpenGeneric(this IServiceCollection services, Type serviceType, Type decoratorType)
		{
			if (services.TryDecorateOpenGeneric(serviceType, decoratorType))
			{
				return services;
			}

			throw new MissingTypeRegistrationException(serviceType);
		}

		static bool TryDecorateOpenGeneric(this IServiceCollection services, Type serviceType, Type decoratorType)
		{
			bool TryDecorate(Type[] typeArguments)
			{
				var closedServiceType = serviceType.MakeGenericType(typeArguments);
				var closedDecoratorType = decoratorType.MakeGenericType(typeArguments);

				return services.TryDecorateDescriptors(closedServiceType, x => x.Decorate(closedDecoratorType));
			}

			var arguments = services
				.Where(descriptor => serviceType.IsAssignableFrom(descriptor.ServiceType))
				.Select(descriptor => descriptor.ServiceType.GenericTypeArguments)
				.ToArray();

			if (arguments.Length == 0)
			{
				return false;
			}

			return arguments.Aggregate(true, (result, args) => result && TryDecorate(args));
		}

		static IServiceCollection DecorateDescriptors(this IServiceCollection services, Type serviceType, Func<ServiceDescriptor, ServiceDescriptor> decorator)
		{
			if (services.TryDecorateDescriptors(serviceType, decorator))
			{
				return services;
			}

			throw new MissingTypeRegistrationException(serviceType);
		}

		static bool TryDecorateDescriptors(this IServiceCollection services, Type serviceType, Func<ServiceDescriptor, ServiceDescriptor> decorator)
		{
			if (!services.TryGetDescriptors(serviceType, out var descriptors))
				return false;

			foreach (var descriptor in descriptors)
			{
				var index = services.IndexOf(descriptor);

				// To avoid reordering descriptors, in case a specific order is expected.
				services.Insert(index, decorator(descriptor));

				services.Remove(descriptor);
			}

			return true;
		}

		static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out ICollection<ServiceDescriptor> descriptors)
		{
			return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Any();
		}

		static ServiceDescriptor Decorate<TService>(this ServiceDescriptor descriptor, Func<TService, IServiceProvider, TService> decorator)
		{
			return descriptor.WithFactory(provider => decorator((TService)provider.GetInstance(descriptor), provider));
		}

		static ServiceDescriptor Decorate<TService>(this ServiceDescriptor descriptor, Func<TService, TService> decorator)
		{
			return descriptor.WithFactory(provider => decorator((TService)provider.GetInstance(descriptor)));
		}

		static ServiceDescriptor Decorate(this ServiceDescriptor descriptor, Type decoratorType)
		{
			return descriptor.WithFactory(provider => provider.CreateInstance(decoratorType, provider.GetInstance(descriptor)));
		}

		static ServiceDescriptor WithFactory(this ServiceDescriptor descriptor, Func<IServiceProvider, object> factory)
		{
			return ServiceDescriptor.Describe(descriptor.ServiceType, factory, descriptor.Lifetime);
		}

		static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
		{
			if (descriptor.ImplementationInstance != null)
			{
				return descriptor.ImplementationInstance;
			}

			if (descriptor.ImplementationType != null)
			{
				return provider.GetServiceOrCreateInstance(descriptor.ImplementationType);
			}

			return descriptor.ImplementationFactory(provider);
		}

		static object GetServiceOrCreateInstance(this IServiceProvider provider, Type type)
		{
			return ActivatorUtilities.GetServiceOrCreateInstance(provider, type);
		}

		static object CreateInstance(this IServiceProvider provider, Type type, params object[] arguments)
		{
			return ActivatorUtilities.CreateInstance(provider, type, arguments);
		}

		static bool IsGeneric(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			return type.GetTypeInfo().IsGenericTypeDefinition;
		}
	}

	class MissingTypeRegistrationException : ApplicationException
	{
		public Type Type { get; }

		public MissingTypeRegistrationException(Type type)
		{
			Type = type;
		}
	}
}