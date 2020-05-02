using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using GameStudio.Cloud.Aws;

namespace GameStudio.Secrets
{
	public class AwsSecretsProvider: SecretsProvider
	{
        readonly AwsAuthentication _auth;
        readonly string _prefix;
		AmazonSecretsManagerClient _client;
        
		public AwsSecretsProvider(AwsAuthentication auth, string prefix)
		{
			_prefix = prefix;
            _auth = auth;
        }

        public AwsSecretsProvider(AwsAuthentication auth) : this(auth,string.Empty)
        {
        }

        public override Secret CreateSecret(params string[] parts)
        {
            return new Secret("_", parts);
        }

        public AmazonSecretsManagerClient Client => _client ?? (_client = new AmazonSecretsManagerClient(_auth.GetCredentials(), _auth.GetRegion()));

		public override async Task<string> GetAsync(Secret secret, CancellationToken cancellationToken = default(CancellationToken))
		{
            var sec = Prefix(secret);

            try
            {
				var resp = await Client.GetSecretValueAsync(new GetSecretValueRequest {SecretId = sec}, cancellationToken);

				return resp.SecretString;
			}
			catch (ResourceNotFoundException ex)
			{
				throw new SecretNotFoundException(sec,ex);
			}
		}
		
		public override async Task<bool> PutAsync(Secret secret, string value, string description, CancellationToken cancellationToken = default(CancellationToken))
		{
            var sec = Prefix(secret);
            try
            {
                var resp = await Client.PutSecretValueAsync(new PutSecretValueRequest { SecretId = sec, SecretString = value}, cancellationToken);

				return resp.Name == sec;
			}
			catch (ResourceNotFoundException)
			{
				var resp = await Client.CreateSecretAsync(new CreateSecretRequest {Name = sec, SecretString = value, Description = description}, cancellationToken);

				return resp.Name == sec;
			}
		}

		public override async Task<bool> DeleteAsync(Secret secret, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sec = Prefix(secret);
            try
			{
				var resp = await Client.DeleteSecretAsync(new DeleteSecretRequest {SecretId = sec, ForceDeleteWithoutRecovery = true}, cancellationToken);
				return resp.HttpStatusCode == HttpStatusCode.OK;
			}
			catch (ResourceNotFoundException ex)
			{
				throw new SecretNotFoundException(sec,ex);
			}
		}

		string Prefix(Secret secret)
		{
			if (string.IsNullOrWhiteSpace(_prefix))
				return secret.ToString();

			return $"{_prefix}_{secret}";
		}
	}
}
