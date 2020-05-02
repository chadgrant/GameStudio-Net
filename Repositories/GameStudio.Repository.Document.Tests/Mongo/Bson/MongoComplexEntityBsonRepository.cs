using Microsoft.Extensions.Options;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests.Mongo
{
    public class MongoComplexEntityBsonRepository : MongoRepository<string,ComplexEntity>
    {
        public MongoComplexEntityBsonRepository(IOptions<MongoOptions> options, string collection)
            : base(options, new ComplexEntityBsonMapper(options), collection)
        {
        }
    }
}
