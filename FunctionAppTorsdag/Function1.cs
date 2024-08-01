using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FunctionAppTorsdag
{
    public class Function1
    {

        public class Person
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string Name { get; set; }
        }

        static List<Person> personList = new();

        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("GetPeople")]
        public async Task<IActionResult> GetPeople([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "person")] HttpRequest req)
        {
            _logger.LogInformation("Getting people.");

            return new OkObjectResult(personList);

        }

        [Function("CreatePerson")]
        public async Task<IActionResult> CreatePerson([HttpTrigger(AuthorizationLevel.Function, "post", Route = "person")] HttpRequest req)
        {

            var requestData = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Person>(requestData);

            _logger.LogInformation("Creating a person.");


            var Person = new Person
            {
                Id = Guid.NewGuid().ToString(),
                Name = data.Name
            };

            personList.Add(Person);
            return new OkObjectResult(Person);

        }

        [Function("GetPersonById")]
        public async Task<IActionResult> GetPersonById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/{id}")]
        HttpRequest req, string id)
        {
            _logger.LogInformation($"Getting a person by id: {id}");

            var person = personList.FirstOrDefault(x => x.Id == id);
            if (person == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(person);

        }
    }
}