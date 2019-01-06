using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
namespace New_Folder.DiscordClasses
{
    
    public class Webhook
    {
        
        public string username{get;set;}
        public string avatar_url{get;set;}
        public string content{get;set;}
        //public file file{get;set;}
        public List<Embed> embeds{get;set;}
        private static readonly HttpClient client = new HttpClient();
        public async Task<string> SendWebhook(string url)
        {
            Console.WriteLine(JsonConvert.SerializeObject(this));
            var SerializedWebhookToSend = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
            SerializedWebhookToSend.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, SerializedWebhookToSend);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}