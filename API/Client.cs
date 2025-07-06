using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Data;
namespace API
{
    public class Client<T> : IClient<T>
    {
        UriBuilder uriBuilder;
        HttpRequestMessage request;
        HttpResponseMessage response;

        public string Schema
        {
            set
            {
                this.uriBuilder.Scheme = "https";
            }
        }

        public string Host
        {
            set
            {
                this.uriBuilder.Host = "localhost";
            }
        }
        public int Port
        {
            set
            {
                this.uriBuilder.Port = 7189;
            }
        }
        public string Path
        {
            set
            {
                this.uriBuilder.Path = value;
            }
        }

        public Client()
        {
            this.uriBuilder = new UriBuilder();
            this.uriBuilder.Query = string.Empty;
            this.request = new HttpRequestMessage();
        }

        public void AddParams(string name, string Value)
        {
            if (this.uriBuilder.Query == string.Empty)
            {
                this.uriBuilder.Query = "?";
            }
            else
            {
                this.uriBuilder.Query += "&";
            }
            this.uriBuilder.Query += $"{name}={Value}";
        }

        public async Task<string> GetQR()
        {
            using (HttpClient client = new HttpClient())
            {
                // ❗ Create a new request each time
                var request = new HttpRequestMessage(HttpMethod.Get, this.uriBuilder.Uri);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    string base64 = Convert.ToBase64String(imageBytes);
                    return $"data:image/png;base64,{base64}";
                }
            }

            return string.Empty;
        }



        public async Task<T> GetAsync()
        {
            this.request.Method = HttpMethod.Get;
            this.request.RequestUri = this.uriBuilder.Uri;
            using (HttpClient Client = new HttpClient())
            {
                this.response = await Client.SendAsync(this.request);
                Console.WriteLine("Status Code: " + this.response.StatusCode);
                if (this.response.IsSuccessStatusCode)
                {
                    string json = await this.response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    T viewModel = await this.response.Content.ReadAsAsync<T>();
                    return viewModel;
                }
            }
            return default(T);
        }

        public async Task<int> RegPost(T model)
        {
            this.request.Method = HttpMethod.Post;
            this.request.RequestUri = this.uriBuilder.Uri;
            ObjectContent<T> objectContent = new ObjectContent<T>(model, new JsonMediaTypeFormatter());
            this.request.Content = objectContent;
            using (HttpClient Client = new HttpClient())
            {
                this.response = await Client.SendAsync(this.request);
                Console.WriteLine("Status Code: " + this.response.StatusCode);
                if (this.response.IsSuccessStatusCode)
                {
                    return await this.response.Content.ReadAsAsync<int>();
                }
            }
            return 0;
        }

        public async Task<bool> PostAsync(T model)
        {
            this.request.Method = HttpMethod.Post;
            this.request.RequestUri = this.uriBuilder.Uri;
            ObjectContent<T> objectContent = new ObjectContent<T>(model, new JsonMediaTypeFormatter());
            this.request.Content = objectContent;
            using (HttpClient Client = new HttpClient())
            {
                this.response = await Client.SendAsync(this.request);
                Console.WriteLine("Status Code: " + this.response.StatusCode);
                if (this.response.IsSuccessStatusCode)
                {
                    return await this.response.Content.ReadAsAsync<bool>();
                }
            }
            return false;
        }
    }
}
