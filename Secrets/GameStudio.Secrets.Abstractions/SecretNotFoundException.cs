using System;

namespace GameStudio.Secrets
{
	public class SecretNotFoundException : ApplicationException
	{
        public string Secret { get; }

		public SecretNotFoundException(string secret, Exception inner) : base(inner.Message, inner)
        {
            Secret = secret;
        }
	}
}
