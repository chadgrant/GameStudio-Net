using System;

namespace GameStudio
{
    public class EuidV1
    {
        public static Guid Create(Studio studio, Application app, uint playerId)
        {
            var pd = BitConverter.GetBytes(playerId);

            var bytes = new byte[]
            {
                (byte)studio,
                (byte)app,
                0,0,
                pd[0], pd[1], pd[2], pd[3],
                0,0,0,0,
                0,0,0,0
            };

            return new Guid(bytes);
        }

        public static Guid Create(EuidInfoV1 info)
        {
            return Create(info.Studio, info.Application, info.PlayerId);
        }

        public static EuidInfoV1 Parse(string str)
        {
            return Parse(Guid.Parse(str));
        }

        public static EuidInfoV1 Parse(Guid guid)
        {
            return Parse(guid.ToByteArray());
        }

        public static EuidInfoV1 Parse(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length != 16)
                throw new ArgumentException("must be 16 bytes", nameof(bytes));

            return new EuidInfoV1
            {
                Studio  = (Studio)bytes[0],
                Application = (Application)bytes[1],
                PlayerId = BitConverter.ToUInt32(new[] { bytes[4], bytes[5], bytes[6], bytes[7] }, 0)
            };
        }
    }

    /// <summary>
    /// Entity ID - Unique Entity ID
    /// 1 byte  = Studio
    /// 1 byte  = App ID        - 256 Apps
    /// 2 bytes = Entity Type   - (65,535 possible values) types of documents
    /// 4 bytes = Player ID     - (4,294,967,295 possible values) player id 
    /// 4 bytes = Random        - (4,294,967,295 possible values)
    /// 4 bytes = Created       - Unix Timestamp 32bit
    /// </summary>
    public class Euid
    {
        static readonly Random Random = new Random((int)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());

        public static Guid Create(Studio studio, Application app, ushort type, uint playerId, DateTime created, byte[] random = null)
        {
            if (created.Kind != DateTimeKind.Utc)
                created = created.ToUniversalTime();

            var et = BitConverter.GetBytes(type);
            var pd = BitConverter.GetBytes(playerId);
            var cd = BitConverter.GetBytes((uint)((DateTimeOffset)created).ToUnixTimeSeconds());

            var rd = new byte[3];

            if (random != null)
            {
                if (random.Length != 4)
                    throw new ArgumentException("random must be 4 bytes");

                rd = random;
            }
            else
                Random.NextBytes(rd);

            var bytes = new byte[16]
            {
                (byte)studio,
                (byte)app,
                et[0], et[1],
                pd[0], pd[1], pd[2], pd[3],
                rd[0], rd[1], rd[2], rd[3],
                cd[0], cd[1], cd[2], cd[3]
            };

            return new Guid(bytes);
        }

        public static Guid Create(EuidInfo info)
        {
            return Create(info.Studio, info.Application, info.Type, info.PlayerId, info.Created);
        }

        public static EuidInfo Parse(string str)
        {
            return Parse(Guid.Parse(str));
        }

        public static EuidInfo Parse(Guid guid)
        {
            return Parse(guid.ToByteArray());
        }

        public static EuidInfo Parse(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length != 16)
                throw new ArgumentException("must be 16 bytes", nameof(bytes));

            return new EuidInfo
            {
                Studio = (Studio)bytes[0],
                Application = (Application)bytes[1],
                Type = BitConverter.ToUInt16(new[] { bytes[2], bytes[3] }, 0),
                PlayerId = BitConverter.ToUInt32(new[] { bytes[4], bytes[5], bytes[6], bytes[7] }, 0),
                Random = BitConverter.ToUInt32(new[] { bytes[8], bytes[9], bytes[10], bytes[11] }, 0),
                Created = DateTimeOffset.FromUnixTimeSeconds(BitConverter.ToUInt32(new[] { bytes[12], bytes[13], bytes[14], bytes[15] }, 0)).UtcDateTime
            };
        }
    }
}
