using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using GameStudio.Cloud.Aws;
using Xunit;

namespace GameStudio.Secrets.Tests
{
	public abstract class SecretConfigurationProviderTests
	{
		protected abstract SecretsProvider Provider { get; }
        protected abstract bool Enabled { get; }

        [SkippableFact]
		public async Task GetKeysAsync()
        {
            Skip.IfNot(Enabled);

            var k1 = Provider.CreateSecret("key", "one");
            var k2 = Provider.CreateSecret("key", "two");

            var dic = new Dictionary<Secret, string>
			{
				{ k1,"one.value" },
				{ k2,"two.value" }
			};

			try
			{
				Assert.True(await Provider.PutAsync(dic));

				var configProvider = new SecretsConfigurationProvider(Provider, k1, k2);
				configProvider.Load();

				Assert.True(configProvider.TryGet(k1.ToString(), out var value));
				Assert.Equal("one.value", value);
			}
			finally
			{
				await Provider.DeleteAsync(dic.Keys);
			}
		}
	}

	public class AwsConfigurationProviderTests : SecretConfigurationProviderTests {
		protected override SecretsProvider Provider => new AwsSecretsProvider(new AwsAuthentication(TestsConfig.AwsRegion, TestsConfig.AwsAccessKey, TestsConfig.AwsSecretKey),"units");
        protected override bool Enabled => TestsConfig.AWS;
    }

    public class AzureConfigurationProviderTests : SecretConfigurationProviderTests
    {
        protected override SecretsProvider Provider => new AzureSecretsProvider(TestsConfig.AzureVaultUrl,
            new KeyVaultClient(async (authority, resource, scope) =>
            {
                var adCredential = new ClientCredential(TestsConfig.AzureClientId, TestsConfig.AzureClientSecret);
                var authenticationContext = new AuthenticationContext(authority, null);
                return (await authenticationContext.AcquireTokenAsync(resource, adCredential)).AccessToken;
            }));
        protected override bool Enabled => TestsConfig.Azure;
    }
}
