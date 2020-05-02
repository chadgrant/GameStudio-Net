namespace GameStudio.Repository.Document.Mongo
{
	public class MongoOptions
	{
		public string ConnectionString { get; set; }

        /// <summary>
        /// Cosmos DB will add a generated _id field this is here to set both id/_id to the same value 
        /// </summary>
        public string AdditionalIdField { get; set; } = "id";
    }
}
