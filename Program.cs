using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RESTful.Client
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Get");
            await Get(1);
            Console.WriteLine("GetAll");
            await GetAll();
            Console.WriteLine("Post");
            await Post();
            Console.WriteLine("GetAll");
            await GetAll();
            Console.WriteLine("Search by start text: 'Trzecia'");
            await GetByText("Trzecia");
            Console.WriteLine("Search by object");

            var searchParam = new SearchParam
            {
                Author = "Jacek",
                Content = "Trzecia"
            };

            await GetByObject(searchParam);
        }

        async static Task Get(long id)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://localhost:5001/messages/{id}")
            };

            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);

            var response = JsonConvert.DeserializeObject<JObject>(await httpResponseMessage.Content.ReadAsStringAsync());
            var message = JsonConvert.DeserializeObject<Message>(response.ToString());

            Console.WriteLine(message.ToString());
        }

        async static Task GetAll()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://localhost:5001/messages")
            };

            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);

            var response = JsonConvert.DeserializeObject<JArray>(await httpResponseMessage.Content.ReadAsStringAsync());
            var messages = JsonConvert.DeserializeObject<IEnumerable<Message>>(response.ToString());

            foreach (var message in messages)
            {
                Console.WriteLine(message.ToString());
            }
        }

        async static Task Post()
        {
            var mess = new Message
            {
                Id = 10,
                Author = "Przemek",
                Content = "Trzecia nowa testowa wiadomosc",
                Created = DateTime.UtcNow
            };

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://localhost:5001/messages"),
                Content = new StringContent(JsonConvert.SerializeObject(mess, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            };

            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage responseMessage = await httpClient.SendAsync(request);

            string response = await responseMessage.Content.ReadAsStringAsync();

            var message = JsonConvert.DeserializeObject<Message>(response);

            Console.WriteLine(message.ToString());
        }

        async static Task GetByText(string startAt)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://localhost:5001/messages/search/{startAt}")
            };

            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);

            var response = JsonConvert.DeserializeObject<JArray>(await httpResponseMessage.Content.ReadAsStringAsync());
            var messages = JsonConvert.DeserializeObject<IEnumerable<Message>>(response.ToString());

            foreach (var message in messages)
            {
                Console.WriteLine(message.ToString());
            }
        }

        async static Task GetByObject(SearchParam searchParam)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://localhost:5001/messages/searchbyobject"),
                Content = new StringContent(JsonConvert.SerializeObject(searchParam, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            };

            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);

            var response = JsonConvert.DeserializeObject<JArray>(await httpResponseMessage.Content.ReadAsStringAsync());
            var messages = JsonConvert.DeserializeObject<IEnumerable<Message>>(response.ToString());

            foreach (var message in messages)
            {
                Console.WriteLine(message.ToString());
            }
        }
    }

    public class Message
    {
        public long Id { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Author: {Author}, Created: {Created}, Content: {Content}";
        }
    }
    public class SearchParam
    {
        public string Author { get; set; }
        public string Content { get; set; }
    }
}