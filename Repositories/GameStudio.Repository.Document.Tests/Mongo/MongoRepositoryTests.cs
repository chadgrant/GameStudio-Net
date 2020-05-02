using System;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests.Mongo
{
    public abstract class MongoRepositoryTests<TId,TDocument> : DocumentRepositoryTests<TId,TDocument> where TDocument : new()
    {
        readonly MongoRepository<TId,TDocument> _repo;

        protected MongoRepositoryTests(bool enabled, MongoRepository<TId,TDocument> repo, TestDocumentProvider<TId, TDocument> docProvider) : base(docProvider)
        {
            Enabled = enabled;
            _repo = repo;
        }

        public override bool Enabled { get; }

        public override MongoRepository<TId, TDocument> GetRepository()
        {
            return _repo;
        }
    }

    public abstract class MongoSimpleEntityRepositoryTests : MongoRepositoryTests<Guid, SimpleEntity>
    {
        protected MongoSimpleEntityRepositoryTests(bool enabled, MongoRepository<Guid,SimpleEntity> repo)
            : base(enabled,repo,new SimpleTestDocumentProvider())
        {
        }
    }

    public abstract class MongoComplexEntityRepositoryTests : MongoRepositoryTests<string, ComplexEntity>
    {
        protected MongoComplexEntityRepositoryTests(bool enabled, MongoRepository<string,ComplexEntity> repo)
            : base(enabled,repo,new ComplexTestDocumentProvider())
        {
        }
    }
}

namespace GameStudio.Repository.Document.Tests.Mongo.CosmosDB.Reflection
{
    public class CosmosDbComplexEntityRepositoryTests : MongoComplexEntityRepositoryTests
    {
        public CosmosDbComplexEntityRepositoryTests()
            : base(TestConfig.CosmosDbEnabled,
                new MongoComplexEntityReflectionRepository(
                    TestConfig.GetOptions(TestConfig.CosmosConnectionString, "id"),
                    "complex-reflection")
            )
        {
        }
    }

    public class CosmosDbSimpleEntityRepositoryTests : MongoSimpleEntityRepositoryTests
    {
        public CosmosDbSimpleEntityRepositoryTests() :
            base(TestConfig.CosmosDbEnabled,
                new MongoSimpleEntityReflectionRepository(
                    TestConfig.GetOptions(TestConfig.CosmosConnectionString, "id"),
                    "simple-reflection")
            )
        {
        }
    }
}

namespace GameStudio.Repository.Document.Tests.Mongo.CosmosDB.Bson
{
    public class CosmosDbComplexEntityRepositoryTests : MongoComplexEntityRepositoryTests
    {
        public CosmosDbComplexEntityRepositoryTests()
            : base(TestConfig.CosmosDbEnabled,
                new MongoComplexEntityBsonRepository(
                    TestConfig.GetOptions(TestConfig.CosmosConnectionString,"id"),
                    "complex")
            )
        {
        }
    }

    public class CosmosDbSimpleEntityRepositoryTests : MongoSimpleEntityRepositoryTests
    {
        public CosmosDbSimpleEntityRepositoryTests() :
            base(TestConfig.CosmosDbEnabled,
                new MongoSimpleEntityBsonRepository(
                    TestConfig.GetOptions(TestConfig.CosmosConnectionString,"id"),
                    "simple")
            )
        {
        }
    }
}

namespace GameStudio.Repository.Document.Tests.Mongo.AWS.Reflection
{
    public class AwsDbComplexEntityRepositoryTests : MongoComplexEntityRepositoryTests
    {
        public AwsDbComplexEntityRepositoryTests()
            : base(TestConfig.AwsEnabled,
                new MongoComplexEntityReflectionRepository(
                    TestConfig.GetOptions(TestConfig.AwsDbConnectionString, null),
                    "complex-reflection")
            )
        {
        }
    }

    public class AwssDbSimpleEntityRepositoryTests : MongoSimpleEntityRepositoryTests
    {
        public AwssDbSimpleEntityRepositoryTests() :
            base(TestConfig.AwsEnabled,
                new MongoSimpleEntityReflectionRepository(
                    TestConfig.GetOptions(TestConfig.AwsDbConnectionString, null),
                    "simple-reflection")
            )
        {
        }
    }
}

namespace GameStudio.Repository.Document.Tests.Mongo.AWS.Bson
{
    public class AwsDbComplexEntityRepositoryTests : MongoComplexEntityRepositoryTests
    {
        public AwsDbComplexEntityRepositoryTests()
            : base(TestConfig.AwsEnabled,
                new MongoComplexEntityBsonRepository(
                    TestConfig.GetOptions(TestConfig.AwsDbConnectionString, null),
                    "complex")
            )
        {
        }
    }

    public class AwsDbSimpleEntityRepositoryTests : MongoSimpleEntityRepositoryTests
    {
        public AwsDbSimpleEntityRepositoryTests() :
            base(TestConfig.AwsEnabled,
                new MongoSimpleEntityBsonRepository(
                    TestConfig.GetOptions(TestConfig.AwsDbConnectionString,null),
                    "simple")
            )
        {
        }
    }
}

namespace GameStudio.Repository.Document.Tests.Mongo.Vanilla.Reflection
{
    public class VanillaComplexEntityRepositoryTests : MongoComplexEntityRepositoryTests
    {
        public VanillaComplexEntityRepositoryTests()
            : base(TestConfig.VanillaEnabled,
                new MongoComplexEntityReflectionRepository(
                    TestConfig.GetOptions(TestConfig.VanillaConnectionString, null),
                    "complex-reflection")
            )
        {
        }
    }

    public class VanillaSimpleEntityRepositoryTests : MongoSimpleEntityRepositoryTests
    {
        public VanillaSimpleEntityRepositoryTests()
            : base(TestConfig.VanillaEnabled,
                new MongoSimpleEntityReflectionRepository(
                    TestConfig.GetOptions(TestConfig.VanillaConnectionString, null),
                    "simple-reflection")
            )
        {
        }
    }
}

namespace GameStudio.Repository.Document.Tests.Mongo.Vanilla.Bson
{
    public class VanillaComplexEntityRepositoryTests : MongoComplexEntityRepositoryTests
    {
        public VanillaComplexEntityRepositoryTests()
            : base(TestConfig.VanillaEnabled,
                new MongoComplexEntityBsonRepository(
                    TestConfig.GetOptions(TestConfig.VanillaConnectionString,null),
                    "complex")
            )
        {
        }
    }

    public class VanillaSimpleEntityRepositoryTests : MongoSimpleEntityRepositoryTests
    {
        public VanillaSimpleEntityRepositoryTests()
            : base(TestConfig.VanillaEnabled,
                new MongoSimpleEntityBsonRepository(
                    TestConfig.GetOptions(TestConfig.VanillaConnectionString,null),
                    "simple")
            )
        {
        }
    }
}
