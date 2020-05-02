using Microsoft.Extensions.Configuration;

namespace GameStudio.Secrets
{
	public class SecretsConfigurationSource : IConfigurationSource
	{
		readonly SecretsProvider _provider;
		readonly Secret[] _secrets;

		public SecretsConfigurationSource(SecretsProvider provider, params Secret[] secrets)
		{
			_provider = provider;
			_secrets = secrets;
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new SecretsConfigurationProvider(_provider, _secrets);
		}
	}

	public class SecretsConfigurationProvider: ConfigurationProvider, IConfigurationProvider
	{
		readonly SecretsProvider _provider;
		readonly Secret[] _secrets;

		public SecretsConfigurationProvider(SecretsProvider provider, params Secret[] secrets)
		{
			_provider = provider;
            _secrets = secrets;
		}

		static string NormalizeKey(Secret secret)
		{
			return secret.ToString().Replace("__", ConfigurationPath.KeyDelimiter);
		}

		public override void Load()
		{
			var (success,dic) = _provider.TryGetAsync(_secrets).Result;
			if (!success) return;

			foreach (var kv in dic)
				Data[NormalizeKey( kv.Key )] = kv.Value;
		}
	}
}
