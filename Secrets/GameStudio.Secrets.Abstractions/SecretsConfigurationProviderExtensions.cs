using GameStudio.Secrets;

namespace Microsoft.Extensions.Configuration
{
	public static class EnvironmentVariablesExtensions
	{
		public static IConfigurationBuilder AddSecrets(this IConfigurationBuilder configurationBuilder, SecretsProvider provider, params Secret[] secrets)
		{
			configurationBuilder.Add(new SecretsConfigurationSource(provider, secrets));
			return configurationBuilder;
		}
	}
}
