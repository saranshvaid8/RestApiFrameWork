using System;
using System.IO;
using System.Linq;
using System.Net;
using IniParser;
using IniParser.Model;
using NUnit.Framework;
using RestSharp;

namespace RestAPIHelpers
{
    public class RestAPIClient : RestClient
    {

        #region Private Members

        private const string FileName = "testConfig.ini";
        private IniData configuration;
        private RestAPIClient restAPIClient;
        private string server ;
        #endregion Private Members

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="baseUrl"></param>

        public RestAPIClient(string baseUrl) : base(baseUrl)
        {
        }

        public IniData Configuration
        {
            get 
            {
                if (configuration == null)
                    configuration = LoadConfiguration();
                return configuration;
            }
            set
            {
                Configuration = value;
            }
        }
        /// <summary>
        /// Gets Client based on the url passed 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public RestAPIClient GetClient(string baseUrl)
        {
            if (restAPIClient == null)
            {
                restAPIClient = new RestAPIClient(baseUrl);
                if (baseUrl.ToString().ToLower().StartsWith("https"))
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                

                var acceptValue = restAPIClient.DefaultParameters.First(p => p.Name == "Accept").Value;
                restAPIClient.DefaultParameters.First(
                    p => p.Name == "Accept").Value = $"{acceptValue},                  application/hal+json";
            }

            return restAPIClient;
        }


        /// <summary>
        /// Gets Client based on the Server url defined in the .ini file
        /// </summary>
        /// <returns></returns>
        public RestAPIClient GetClient()
        {
             server = Configuration["API"]["Server"];
            return GetClient(server);
        }

        /// <summary>
        /// Executes the request and returns the response 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override IRestResponse Execute(IRestRequest request)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH.mmm.ss")} Performing Request to <{request.Resource}>");

            foreach (var parameter in request.Parameters)
            {
                Console.WriteLine($"\t\t Parameters Type <{parameter.Type}> Name :<{parameter.Name}> ContentType : <{parameter.ContentType}>): <{parameter.Value}>");

            }

            IRestResponse restResponse= base.Execute(request);
            Console.WriteLine($"{DateTime.Now.ToString("HH.mmm.ss")} Response Status : <{restResponse.ResponseStatus}> Status Code : <{restResponse.StatusCode}>  Status Description : <{restResponse.StatusDescription}>");
            Console.WriteLine($"\t\t Content<{ restResponse.Content}>");

            return restResponse;
        }

        /// <summary>
        /// Load Configuration 
        /// 1) Checks for DIR (DIR = )
        /// 2) Combine's the TestDir + .ini file to get path
        /// 3) if path found write's DateTime for Ini Merging and Read's the .IniFile
        /// 4) Set's the dir to Parent Dir 
        /// 5) Returns Config of the File
        /// </summary>
        /// <returns></returns>
        private IniData LoadConfiguration()
        {
            var parser = new FileIniDataParser();
            var config = new IniData();
            var directory = TestContext.CurrentContext.TestDirectory;

            while (Directory.Exists(directory))
            {
                var path = Path.Combine(directory, FileName);
                if (File.Exists(path))
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm.SS.FFFFF")}: Mergeing INI file <{path}>");
                    config.Merge(parser.ReadFile(path));
                }
                directory = Directory.GetParent(directory)?.FullName;
            }
            return config;
        }
    }
}
