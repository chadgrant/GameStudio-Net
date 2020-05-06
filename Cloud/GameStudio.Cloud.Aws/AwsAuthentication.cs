using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace GameStudio.Cloud.Aws
{
    public class AwsAuthentication
    {
        static readonly RegionEndpoint DefaultRegion = RegionEndpoint.USEast1;
        static readonly ConcurrentBag<string> Environments = new ConcurrentBag<string>(new []{ "GS_ENVIRONMENT","ENVIRONMENT", "ASPNETCORE_ENVIRONMENT" });
        static readonly ConcurrentBag<string> Profiles = new ConcurrentBag<string>(new[] { "default", "development", "staging", "production" });

        readonly RegionEndpoint _region;

        static AwsAuthentication()
        {
            FallbackCredentialsFactory.CredentialsGenerators = new List<FallbackCredentialsFactory.CredentialsGenerator>
            {
                () => new InstanceProfileAWSCredentials(),
                GetCredentialsFromProfile,
                () => new EnvironmentVariablesAWSCredentials()
            };
        }

        public AwsAuthentication(string region, string accessKey, string secretKey) : this(accessKey,secretKey)
        {
            _region = RegionEndpoint.GetBySystemName(region);
        }

        public AwsAuthentication(string accessKey, string secretKey)
        {
            if (!string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey))
                FallbackCredentialsFactory.CredentialsGenerators.Add(()=> new BasicAWSCredentials(accessKey, secretKey));
        }

        public AwsAuthentication()
        {

        }

        public AWSCredentials GetCredentials()
        {
            return FallbackCredentialsFactory.GetCredentials();
        }

        public void AddProfile(string profile)
        {
            Profiles.Add(profile);
        }

        public void AddEnvironment(string environment)
        {
            Environments.Add(environment);
        }

        static AWSCredentials GetCredentialsFromProfile()
        {
            var provider = new CredentialProfileStoreChain();

            var env = GetEnvironment();
            if (!string.IsNullOrWhiteSpace(env))
            {
                if (provider.TryGetAWSCredentials(env, out var envcreds))
                    return envcreds;
            }

            foreach (var profile in Profiles.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                if (provider.TryGetAWSCredentials(profile, out var creds))
                    return creds;
            }

            return null;
        }

        static string GetEnvironment()
        {
            return Environments.Select(Environment.GetEnvironmentVariable).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
        }

        public RegionEndpoint GetRegion()
        {
            if (_region != null)
                return _region;

            return FallbackRegionFactory.GetRegionEndpoint(true) ?? DefaultRegion;
        }
    }
}
