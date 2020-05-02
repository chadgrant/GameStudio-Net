using System;
using Microsoft.Extensions.Options;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests.Mongo
{
    public class MongoSimpleEntityReflectionRepository
        : MongoRepository<Guid,SimpleEntity>
    {
        public MongoSimpleEntityReflectionRepository(IOptions<MongoOptions> options, string collection)
            : base(options, new SimpleEntityReflectionBsonMapper(options), collection)
        {
        }    
    }
}

