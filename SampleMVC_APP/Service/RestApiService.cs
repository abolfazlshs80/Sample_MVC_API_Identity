using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NuGet.Common;
using RestSharp;

using SampleDomain.Dtos;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RestSharpSample.Service
{
    public class RestApiService
    {
        private readonly HttpClient _httpClient;
        public string baseUrl { get; set; }
        public string ControllerName { get; set; }
        public string AccessToken { get; set; }


        public bool PostValidAccessToken(string token)
        {
            var client = new RestClient(baseUrl);

            // ایجاد یک درخواست POST
            var request = new RestRequest($"api/Account/valid-AccessToken", Method.Post);
            request.AddJsonBody(new
            {
                AccessToken = token
            });

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Response received:");
                Console.WriteLine(response.Content);

                var authResponse = JsonConvert.DeserializeObject<bool>(response.Content);


                return authResponse;
            }
            else
            {
                Console.WriteLine("Error occurred:");
                Console.WriteLine(response.ErrorMessage);
                return false;
            }
        }
        public AuthResponseDto PostRefleshToken(string token)
        {
            var client = new RestClient(baseUrl);

            // ایجاد یک درخواست POST
            var request = new RestRequest($"api/Account/refresh-token", Method.Post);
            request.AddJsonBody(new
            {
                RefreshToken=token
            });

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Response received:");
                Console.WriteLine(response.Content);

                var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(response.Content);


                return authResponse;
            }
            else
            {
                Console.WriteLine("Error occurred:");
                Console.WriteLine(response.ErrorMessage);
                return new AuthResponseDto();
            }
        }
        public AuthResponseDto PostLogin(LoginDto model)
        {
            var client = new RestClient(baseUrl);

            // ایجاد یک درخواست POST
            var request = new RestRequest($"api/Account/login", Method.Post);
            request.AddJsonBody(model);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Response received:");
                Console.WriteLine(response.Content);

                var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(response.Content);
        
                return authResponse;
            }
            else
            {
                Console.WriteLine("Error occurred:");
                Console.WriteLine(response.ErrorMessage);
                return new AuthResponseDto();
            }
        }

        public string GetCategories()
        {
            var client = new RestClient(baseUrl);

            // ایجاد یک درخواست POST
            var request = new RestRequest($"api/Category/Get", Method.Get);
            //request.AddJsonBody(model);
            request.AddHeader("Authorization", $"Bearer {AccessToken}");
            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Response received:");
                Console.WriteLine(response.Content);

                var authResponse = JsonConvert.DeserializeObject<string>(response.Content);

                return authResponse;
            }
            else
            {
                Console.WriteLine("Error occurred:");
                Console.WriteLine(response.ErrorMessage);
                return string.Empty;
            }
        }
        public bool PostLogout(string Token)
        {
            var client = new RestClient(baseUrl);

            // ایجاد یک درخواست POST
            var request = new RestRequest($"api/Account/logout", Method.Post);
            request.AddJsonBody(new 
            {
                AccessToken= Token,
                RefreshToken="s"
            });

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Response received:");
                Console.WriteLine(response.Content);

                var authResponse = JsonConvert.DeserializeObject<bool>(response.Content);

                return authResponse;
            }
            else
            {
                Console.WriteLine("Error occurred:");
                Console.WriteLine(response.ErrorMessage);
                return false;
            }
        }
        public AuthResponseDto PostRegister(RegisterDto model)
        {
            var client = new RestClient(baseUrl);

            // ایجاد یک درخواست POST
            var request = new RestRequest($"api/Account/register", Method.Post);
            request.AddJsonBody(model);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Response received:");
                Console.WriteLine(response.Content);

                var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(response.Content);
                return authResponse;
            }
            else
            {
                Console.WriteLine("Error occurred:");
                Console.WriteLine(response.ErrorMessage);
                return new AuthResponseDto();
            }
        }

    }
}
