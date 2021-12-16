using FundAppsScripts.DTOs;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;

namespace FundAppsScripts.Scripts
{
    public partial class AdapptrScripts
    {
        public string UploadPositions()
        {
            var baseUrl = _adapptrConfig.BaseUrl;
            var username = _adapptrConfig.Username;
            var password = _adapptrConfig.Password;
            var clientEnvironmentSubDomain = "demo-melon";
            var pathToFile = "Data/Adapptr_Import.csv";
            var snapshotDate = DateTime.Today.ToString("yyyy-MM-dd");
            var services = 2;
            var primaryIdentifier = 1;
            var excludeErroredAssets = false;

            //Example using RestSharp (https://github.com/restsharp/RestSharp)

            //Create a client which will connect to the HTTPS endpoint with the API credentials you have been provided
            var client = new RestClient(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };

            // make the HTTP POST request
            var request = new RestRequest($"/rest/api/v1/task/positions", Method.POST);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("X-Client-Environment", clientEnvironmentSubDomain);

            // add body params to the request
            request.AddFile("positions", pathToFile, "text/csv");
            request.AddParameter("snapshotDate", snapshotDate);
            request.AddParameter("services", services);
            request.AddParameter("primaryIdentifier", primaryIdentifier);
            request.AddParameter("excludeErroredAssets", excludeErroredAssets);

            var response = client.Execute<TaskProfileResponse>(request);

            // if response comes back with a 200 status, then a task for the positions file was created successfully
            if ((response.StatusCode != HttpStatusCode.OK) && (response.StatusCode != HttpStatusCode.Accepted))
            {
                throw new Exception("Failed to send file. Received a HTTP " + (int)response.StatusCode + " " + response.StatusCode + " instead of HTTP 200 OK");
            }

            var taskId = response.Data.Id;

            return taskId;
            // success
        }
    }
}
