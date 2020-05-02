using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace GameStudio.Secrets
{
	public class AzureSecretsProvider : SecretsProvider
	{
		readonly string _vaultUrl;
		readonly KeyVaultClient _keyVaultClient;

		public AzureSecretsProvider(string vaultUrl)
		{
            _vaultUrl = vaultUrl;
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            _keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }
        
        internal AzureSecretsProvider(string vaultUrl, KeyVaultClient client)
        {
            _vaultUrl = vaultUrl;
            _keyVaultClient = client;
        }

        public override Secret CreateSecret(params string[] parts)
        {
            return new Secret("-",parts);
        }

        public override async Task<string> GetAsync(Secret secret, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var result = await _keyVaultClient.GetSecretAsync(_vaultUrl, secret.ToString(), cancellationToken);

				return result.Value;
			}
            catch (Exception ex)
            {
                if (IsNotFound(ex))
                    throw new SecretNotFoundException(secret.ToString(), ex);

                throw;
            }
        }

		public override async Task<bool> PutAsync(Secret secret, string value, string description, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _keyVaultClient.SetSecretAsync(_vaultUrl, secret.ToString(), value, cancellationToken:cancellationToken);
			return true;
		}

		public override async Task<bool> DeleteAsync(Secret secret, CancellationToken cancellationToken = default(CancellationToken))
		{
            try
            {
                await _keyVaultClient.DeleteSecretAsync(_vaultUrl, secret.ToString(), cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                if (IsNotFound(ex))
                    throw new SecretNotFoundException(secret.ToString(),ex);

                throw;
            }
        }

        static bool IsNotFound(Exception ex)
        {
            return ex.Message.Contains("NotFound");
        }
    }
}
