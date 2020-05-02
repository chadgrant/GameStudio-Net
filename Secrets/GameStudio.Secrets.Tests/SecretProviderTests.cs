using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using GameStudio.Cloud.Aws;
using Xunit;

namespace GameStudio.Secrets.Tests
{
    //Used to enable tests for developing, since these wouldn't run well in ci/cd
    public static class TestsConfig
    {
        public static bool AWS = false;

        //don't ever need these if machine setup properly
        public static string AwsAccessKey = "";
        public static string AwsSecretKey = "";
        public static string AwsRegion = "us-east-1";

        public static bool Azure = false;

        public static string AzureVaultUrl = "";
        public static string AzureClientId = "";
        public static string AzureClientSecret = "";
    }

    public abstract class SecretProviderTests
    {
        readonly SecretsProvider _provider;

        protected abstract bool Enabled { get; }

        protected SecretProviderTests(SecretsProvider provider)
        {
            _provider = provider;
        }
        
        [SkippableFact]
        public async Task Get_Secret()
        {
            Skip.IfNot(Enabled);

            var secret = _provider.CreateSecret("test", "GetSecret");
            try
            {
                Assert.True(await _provider.PutAsync(secret, "test_value", "secret for testing"));
                Assert.Equal("test_value", await _provider.GetAsync(secret));
            }
            finally
            {
                await _provider.DeleteAsync(secret);
            }
        }

        [SkippableFact]
        public async Task Get_NonExistant_Secret_Fails()
        {
            Skip.IfNot(Enabled);

            var secret = _provider.CreateSecret("test", "DontExist");

            await Assert.ThrowsAsync<SecretNotFoundException>(async ()=>  await _provider.GetAsync(secret) );
        }

        [SkippableFact]
        public async Task Put_Secret()
        {
            Skip.IfNot(Enabled);

            var secret = _provider.CreateSecret("test", "Put", "Secret");

            try
            {
                Assert.True(await _provider.PutAsync(secret, "testValue", "secret for testing"));

                Assert.Equal("testValue", await _provider.GetAsync(secret));
            }
            finally
            {
                await _provider.DeleteAsync(secret);
            }
        }

        [SkippableFact]
        public async Task Update_Secret()
        {
            Skip.IfNot(Enabled);

            var secret = _provider.CreateSecret("test", "Update", "Secret");

            try
            {
                Assert.True(await _provider.PutAsync(secret, "test_value", "secret for testing"));

                Assert.True(await _provider.PutAsync(secret, "test_value2", "secret for testing"));

                Assert.Equal("test_value2",await _provider.GetAsync(secret));
            }
            finally
            {
                await _provider.DeleteAsync(secret);
            }
        }

        [SkippableFact]
        public async Task Delete_Secret()
        {
            Skip.IfNot(Enabled);

            var secret = _provider.CreateSecret("test", "Delete", "Secret");

            Assert.True( await _provider.PutAsync(secret,"value","test"));
            Assert.True( await _provider.DeleteAsync(secret) );
        }
    }


    public class AwsSecretProviderTests : SecretProviderTests
    {

        public AwsSecretProviderTests() : base(new AwsSecretsProvider( new AwsAuthentication(TestsConfig.AwsRegion, TestsConfig.AwsAccessKey,TestsConfig.AwsSecretKey), "units"))
        {
        }

        protected override bool Enabled => TestsConfig.AWS;
    }

    public class AzureSecretProviderTests : SecretProviderTests
    {
        public AzureSecretProviderTests() : base(new AzureSecretsProvider(
            TestsConfig.AzureVaultUrl,
            new KeyVaultClient(async (authority, resource, scope) =>
            {
                var adCredential = new ClientCredential(TestsConfig.AzureClientId, TestsConfig.AzureClientSecret);
                var authenticationContext = new AuthenticationContext(authority, null);
                return (await authenticationContext.AcquireTokenAsync(resource, adCredential)).AccessToken;
            })))
        {
        }

        protected override bool Enabled => TestsConfig.Azure;
    }
}
