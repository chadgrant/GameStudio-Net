using System;
using System.Collections.Generic;
using System.IO;

namespace GameStudio.WebApi
{
	public sealed class Metadata
	{
		static readonly Dictionary<string, string> Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		static Metadata()
        {
            if (!File.Exists("./metadata.txt"))
                return;

			var metadata = File.ReadAllText("./metadata.txt");
			var items = metadata.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			foreach (var i in items)
			{
				var idx = i.IndexOf('=');
				if (idx == -1) continue;

				Values.Add(i.Substring(0, idx), i.Substring(idx + 1));
			}
		}

		/// <summary>
		/// Current build number deployed
		/// </summary>
		/// <example>1.2.3</example>
		public string BuildNumber => GetValue("build_number");

		/// <summary>
		/// User that built the artifacts
		/// Should be the system that built and the commit user that triggered the build
		/// </summary>
		/// <example>Jenkins / cgrant</example>
		public string BuiltBy => GetValue("build_user");

		/// <summary>
		/// When was this built
		/// </summary>
		/// <example>2015-03-12T19:40:18.877Z</example>
		public string BuiltWhen => GetValue("build_date");

		/// <summary>
		/// Git commit sha of the current build
		/// </summary>
		/// <example>d567d2650318f704747204815adedd2396a203f5</example>
		public string GitSha1 => GetValue("build_hash");

		/// <summary>
		/// Version of the compiler used to build
		/// </summary>
		/// <example>dotnet 2.2</example>
		public string CompilerVersion => GetValue("build_compiler");


		/// <summary>
		/// Name of the current machine / container
		/// </summary>
		/// <example>Machine123</example>
		public string MachineName { get; set; } = Environment.MachineName;

		/// <summary>
		/// Time the service was started
		/// </summary>
		/// <example>2015-03-12T19:40:18.877Z</example>
		public DateTime UpSince { get; set; }

		/// <summary>
		/// Current time of the server
		/// </summary>
		/// <example>2015-03-12T19:40:18.877Z</example>
		public DateTime CurrentTime { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// The group that owns the artifact. 
		/// </summary>
		/// <example>Burlingame Backend</example>
		public string GroupId => GetValue("build_group");

		/// <summary>
		/// OS architecture amd64 or x86
		/// </summary>
		/// <example>amd64</example>
		public string OsArch { get; set; } = Environment.Is64BitOperatingSystem ? "amd64" : "x86";

		/// <summary>
		/// OS Name
		/// </summary>
		/// <example>Linux</example>
		public string OsName { get; set; } = Environment.OSVersion.VersionString;

		/// <summary>
		/// Number of processors on the machine
		/// </summary>
		/// <example>4</example>
		public int OsNumProcessors { get; set; } = Environment.ProcessorCount;

		/// <summary>
		/// Current version of the runtime if applicable
		/// </summary>
		/// <example>dotnet 2.2</example>
		public string RuntimeVersion { get; set; } = $"dotnet {Environment.Version}";

        /// <summary>
        /// What environment is this
        /// </summary>
        public string EnvironmentName { get; set; }

		/// <summary>
		/// Version of this schema
		/// </summary>
		/// <example>1</example>
		public int Version { get; set; } = 1;

		public string GetValue(string key)
		{
			if (Values.TryGetValue(key, out var value))
				return value;

			return string.Empty;
		}

        public void SetValue(string key, string value)
        {
            Values[key] = value;
        }
    }
}
