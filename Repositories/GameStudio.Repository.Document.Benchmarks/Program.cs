using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Options;
using GameStudio.Repository.Document.Mongo;
using GameStudio.Repository.Document.Tests;

namespace GameStudio.Repository.Document.Benchmarks
{
    [CoreJob, RPlotExporter, RankColumn]
    public class BsonMapperBenchmarks
    {
        ComplexEntityBsonMapper _complex;
        ComplexEntityReflectionBsonMapper _complexReflectionBson;
        Document<string,ComplexEntity> _complexEntity;

        [GlobalSetup]
        public void Setup()
        {
            var opts = Options.Create(new MongoOptions());

            _complex = new ComplexEntityBsonMapper(opts);
            _complexReflectionBson = new ComplexEntityReflectionBsonMapper(opts);
            _complexEntity = new ComplexTestDocumentProvider().CreateTestDocument();
        }

        [Benchmark]
        public void Complex()
        {
            _complex.Map(_complexEntity);
        }

        [Benchmark]
        public void ComplexReflection()
        {
            _complexReflectionBson.Map(_complexEntity);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BsonMapperBenchmarks>();
            Console.ReadLine();
        }
    }
}
