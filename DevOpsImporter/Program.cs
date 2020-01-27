using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using DevOpsImporter.REST;
using DevOpsImporter.Entities;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using DevOpsImporter.SQL;
namespace DevOpsImporter
{
    class Program
    {
        public static Dictionary<string, string> ConfigDict = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            var start ="Operation Started : " + DateTime.Now.ToString();
            InitializeConfiguration();
            var Cases = REST_Caller.GetAllFogBugItems(ConfigDict["FogBugzAPI"], GetFogBugzToken());
           // var Cases = ReadDataFromFile();
            var totalCases = Cases.Count;
            var TotalActiveCase = Cases.Where(x => x.Status == "Active").ToList<WorkItem>().Count;
            var TotalActiveBugs = Cases.Where(b => b.Category == "Bug" && b.Status == "Active").ToList<WorkItem>().Count;
            var TotalResolved = Cases.Where(b => b.Status.ToLower().StartsWith("closed") || b.Status.ToLower().StartsWith("resolved")).ToList<WorkItem>().Count;

            Console.WriteLine("Total Cases:" + totalCases);
            Console.WriteLine("Total Active:" + TotalActiveCase);
            Console.WriteLine("Total Bugs:" + TotalActiveBugs);
            Console.WriteLine("Total Resolved:" + TotalResolved);

            //string JsonString = JsonConvert.SerializeObject(Cases);
            //StreamWriter wr = new StreamWriter(Directory.GetCurrentDirectory() + @"\AllCases.json");
            //wr.Write(JsonString);
            Console.WriteLine(start);
            Console.WriteLine("Operation Ended : " + DateTime.Now.ToString());
            // var Cases = ReadDataFromFile();

            var repo = FogBugzSQLRepository.GetInstance;
            repo.connectionString = GetSQLConnectionString();
            foreach (WorkItem item in Cases)

            {
                foreach(Event eventItem in item.EventList)
                {
                    foreach(Attachments a in eventItem.CaseAttachments)
                    {

                        repo.InsertAttachments(a);
                    }
                    repo.InsertEvent(eventItem);
                }
                repo.InsertWorkItem(item);
            }
        }

        public static void InitializeConfiguration()
        {
            var config = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("AppSettings.json", false)
                           .Build();

            ConfigDict.Add("DevOpsAPI", config.GetSection("DevOpsAPI").Value);
            ConfigDict.Add("FogBugzAPI", config.GetSection("FogBugzAPI").Value);
            ConfigDict.Add("DevOpsAPI_Version", config.GetSection("DevOpsAPI_Version").Value);
            ConfigDict.Add("FogBugzUserName", config.GetSection("FogBugzUserName").Value);
            ConfigDict.Add("FogBugzEncryptedPassword", config.GetSection("FogBugzEncryptedPassword").Value);
            ConfigDict.Add("EncryptDecryptAPI", config.GetSection("EncryptDecryptAPI").Value);
            ConfigDict.Add("IdentityServer", config.GetSection("IdentityServer").Value);
            ConfigDict.Add("IdentityServerClientCredentials", config.GetSection("IdentityServerClientCredentials").Value);
            ConfigDict.Add("DevOpsToken", config.GetSection("DevOpsToken").Value);
            ConfigDict.Add("ConnectionString", config.GetSection("FogBugzSQL_ConnectionString").Value);
        }

        public static string GetFogBugzToken()
        {
            var token = REST_Caller.GetTokenFromIdentityServer(ConfigDict["IdentityServerClientCredentials"], ConfigDict["IdentityServer"]);
            var DecryptedText = REST_Caller.GetDecryptedText(ConfigDict["FogBugzEncryptedPassword"], ConfigDict["EncryptDecryptAPI"], token);
            var fogbugz_token = REST_Caller.FogBugzLogon(ConfigDict["FogBugzUserName"], DecryptedText, ConfigDict["FogBugzAPI"]);
            return fogbugz_token;
        }

        public static List<WorkItem> ReadDataFromFile()
        {
            List<WorkItem> lst = new List<WorkItem>();
            StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + @"\AllCases.json");
            var AllData=reader.ReadToEnd();
            lst= JsonConvert.DeserializeObject<List<WorkItem>>(AllData);
            return lst;
        }
       public static string GetSQLConnectionString()
        {
            return ConfigDict["ConnectionString"];
        }
    }
}
