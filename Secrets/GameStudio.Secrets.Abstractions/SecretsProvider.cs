using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameStudio.Secrets
{
	public abstract class SecretsProvider
    {
        public abstract Secret CreateSecret(params string[] parts);

		public abstract Task<string> GetAsync(Secret secret, CancellationToken cancellationToken = default(CancellationToken));

		public abstract Task<bool> PutAsync(Secret secret, string value, string description, CancellationToken cancellationToken = default(CancellationToken));

		public abstract Task<bool> DeleteAsync(Secret secret, CancellationToken cancellationToken = default(CancellationToken));

		public async Task<bool> DeleteAsync(IEnumerable<Secret> secrets, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (await Task.WhenAll(secrets.Select(k => DeleteAsync(k, cancellationToken)))).All(b=>b);
		}

		public async Task<IReadOnlyDictionary<Secret, string>> GetAsync(IEnumerable<Secret> secrets, CancellationToken cancellationToken = default(CancellationToken))
		{
			var dic = new Dictionary<Secret,string>();
			foreach (var s in secrets)
				dic.Add(s, await GetAsync(s, cancellationToken));
			return dic;
		}

		public async Task<bool> PutAsync(IDictionary<Secret, string> secrets, string description=null, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var kv in secrets)
				if (!await PutAsync(kv.Key, kv.Value, description, cancellationToken))
					return false;

			return true;
		}

		public async Task<(bool,string)> TryGetAsync(Secret key, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return (true, await GetAsync(key, cancellationToken));
			}
			catch
			{
				return (false, null);
			}
		}

		public async Task<(bool, IReadOnlyDictionary<Secret,string>)> TryGetAsync(IEnumerable<Secret> secrets, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return (true, await GetAsync(secrets, cancellationToken));
			}
			catch
			{
				return (false, null);
			}
		}
	}
}
