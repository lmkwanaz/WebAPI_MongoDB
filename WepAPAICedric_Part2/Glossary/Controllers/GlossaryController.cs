using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MongoDB.Driver;
using System;
using System.IO;

namespace Glossary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlossaryController: ControllerBase
    {
          private IMongoDatabase mongoDatabase;  
  
        public IMongoDatabase GetMongoDatabase()  
        {  
            var mongoClient = new MongoClient("mongodb://localhost:27017");  
            return mongoClient.GetDatabase("TryIt");  
        }
        

         [HttpGet]  
        public ActionResult<List<GlossaryItem>> Get()  
        {
            mongoDatabase = GetMongoDatabase();  
           
            var result = mongoDatabase.GetCollection<GlossaryItem>("GItems").Find(FilterDefinition<GlossaryItem>.Empty).ToList();  
            return Ok(result);  
        }  

        [HttpGet]
        [Route("{Id}")]
        public ActionResult<GlossaryItem> Get(string term)
        {
            if (term == null)  
            {  
                return NotFound();  
            }  
           
            mongoDatabase = GetMongoDatabase();   
            GlossaryItem customer = mongoDatabase.GetCollection<GlossaryItem>("GItems").Find<GlossaryItem>
            (k => k.Term == term).FirstOrDefault();  
            if (customer == null)  
            {  
                return NotFound();  
            }  
            return customer; 
        }

       [HttpPost]
        public ActionResult Post(GlossaryItem glossaryItem)
        {
             try  
            {  
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();  
                mongoDatabase.GetCollection<GlossaryItem>("GItems").InsertOne(glossaryItem);  
            }  
            catch (Exception)  
            {  
                throw;  
            }  
            var resourceUrl = Path.Combine(Request.Path.ToString(), Uri.EscapeUriString(glossaryItem.Term));
            return Created(resourceUrl,glossaryItem); 
        }

        [HttpPut]
        public ActionResult Update(GlossaryItem items)
        {
            try  
            {  
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();  
                //Build the where condition  
                var filter = Builders<GlossaryItem>.Filter.Eq("Id", items.Id);  
                //Build the update statement   
                var updatestatement = Builders<GlossaryItem>.Update.Set("Id", items.Id);  
                updatestatement = updatestatement.Set("Term", items.Term);  
                updatestatement = updatestatement.Set("Definition", items.Definition);  
                //fetch the details from CustomerDB based on id and pass into view  
                var result = mongoDatabase.GetCollection<GlossaryItem>("GItems").UpdateOne(filter, updatestatement);  
                if (result.IsAcknowledged == false)  
                {  
                    return BadRequest("Unable to update Customer  " + items.Term);  
                }  
            }  
            catch (Exception)  
            {  
                throw;  
            }  
  
            return Ok(); 
        }

         [HttpDelete]
            public ActionResult Delete(GlossaryItem glossaryItem){
                try  
            {  
                //Get the database connection  
                mongoDatabase = GetMongoDatabase();  
                //Delete the customer record  
                var result = mongoDatabase.GetCollection<GlossaryItem>("GItems")
                .DeleteOne<GlossaryItem>(k => k.Id == glossaryItem.Id);  
                if (result.IsAcknowledged == false)  
                {  
                    return BadRequest("Unable to Delete Customer " + glossaryItem.Id);  
                }  
            }  
            catch (Exception)  
            {  
                throw;  
            }  
            return Ok();  
            }
    }
}