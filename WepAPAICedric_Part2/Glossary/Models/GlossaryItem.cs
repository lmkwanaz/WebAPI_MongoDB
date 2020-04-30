using MongoDB.Bson;  
using MongoDB.Bson.Serialization.Attributes;

namespace Glossary
{
    [BsonIgnoreExtraElements]
   public class GlossaryItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set;}
        public string Term {get; set;}
      
        public string Definition {get; set;}
    }
}