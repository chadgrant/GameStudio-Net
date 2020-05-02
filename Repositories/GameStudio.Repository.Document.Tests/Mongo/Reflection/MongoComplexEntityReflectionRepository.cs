using Microsoft.Extensions.Options;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests.Mongo
{
    public class MongoComplexEntityReflectionRepository : MongoRepository<string,ComplexEntity>
    {
        public MongoComplexEntityReflectionRepository(IOptions<MongoOptions> options, string collection)
            : base(options, new ComplexEntityReflectionBsonMapper(options), collection)
        {
        }    
    }
}
