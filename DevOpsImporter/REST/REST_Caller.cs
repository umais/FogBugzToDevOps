using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DevOpsImporter.Entities;
namespace DevOpsImporter.REST
{
   public static class REST_Caller
    {
        public static string GetTokenFromIdentityServer(string clientCredentials,string IdentityServerURL)
        {
            var Client = new RestClient();
            Client.BaseUrl = new Uri(IdentityServerURL);
            var request = new RestRequest("", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", "Basic "+clientCredentials);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "read");
            var response = Client.Execute(request);
            JObject res = JsonConvert.DeserializeObject<JObject>(response.Content);
            var token = (string)res.SelectToken("access_token");
            return token;
        }

        public static string GetDecryptedText(string EncryptedText,string DecryptURL,string access_token)
        {
            var Client = new RestClient();
            Client.BaseUrl = new Uri(DecryptURL);
            var request = new RestRequest("Decrypt", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer "+access_token);
            request.AddJsonBody(new { EncryptedText = EncryptedText });
            var response = Client.Execute(request);
            JObject res = JsonConvert.DeserializeObject<JObject>(response.Content);
            var DecryptedText = (string)res.SelectToken("Value");
            return DecryptedText;
        }

        public static string FogBugzLogon(string username,string password,string FogBugzURL)
        {
            var Client = new RestClient();
            Client.BaseUrl = new Uri(FogBugzURL);
            var request = new RestRequest("logon", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { email = username, password = password });
            var response = Client.Execute(request);
            JObject res = JsonConvert.DeserializeObject<JObject>(response.Content);
            var token = (string)res.SelectToken("data.token");
            return token;
        }

        public static List<WorkItem> GetAllFogBugItems(string FogBugzURL,string token)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            
                 var Client = new RestClient();
            Client.BaseUrl = new Uri(FogBugzURL);
            var request = new RestRequest("listCases", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { token = token, sFilter = "121",cols=new List<String>() {"events" } });
            var response = Client.Execute(request);
            JObject res = JsonConvert.DeserializeObject<JObject>(response.Content);
            var AllCases = res.SelectToken("data.cases");
            int i = 1;
            foreach (JObject o in AllCases)
            {
                var Id=o.SelectToken("ixBug").ToString();
            
                request= new RestRequest("search", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new { token = token, q = Id, cols = "sTitle,sStatus,sLatestTextSummary,sPriority,sArea,sProject,sFixFor,sCategory,ixPersonAssignedTo" });
                var response2 = Client.Execute(request);
                JObject res2 = JsonConvert.DeserializeObject<JObject>(response2.Content);
                var Case = res2.SelectToken("data.cases[0]");
                var CaseTitle = Case.SelectToken("sTitle").ToString();
                var CaseStatus= Case.SelectToken("sStatus").ToString();
                var CaseLatestSummary= Case.SelectToken("sLatestTextSummary").ToString();
                var CasePriority= Case.SelectToken("sPriority").ToString();
                var CaseArea= Case.SelectToken("sArea").ToString();
                var CaseProject = Case.SelectToken("sProject").ToString();
                var CaseMilestone= Case.SelectToken("sFixFor").ToString();
                var CaseAssignedTo = "";
                try
                {
                    CaseAssignedTo= WorkItem.EmployeeList[Case.SelectToken("ixPersonAssignedTo").ToString()];
                }
                catch
                {
                    CaseAssignedTo = Case.SelectToken("ixPersonAssignedTo").ToString();
                }
                var CaseCategory= Case.SelectToken("sCategory").ToString();
                //Getting All the Events Including Attachments.
                var events = o.SelectToken("events");
                List<Event> caseEvents = new List<Event>();
                foreach (JObject e in events)
                {
                    var eventID = e.SelectToken("ixBugEvent");
                    var eventType = e.SelectToken("sVerb");
                    var eventName = e.SelectToken("evtDescription");
                    var eventDate = e.SelectToken("dt");
                    var eventText = e.SelectToken("s");

                    var attachments = e.SelectToken("rgAttachments");
                    List<Attachments> eventAttachments = new List<Attachments>();
                    foreach(JToken attachment in attachments)
                    {
                        var fileName = attachment.SelectToken("sFileName");
                        var fileURL = attachment.SelectToken("sURL");
                        eventAttachments.Add(new Attachments() { 
                                                                 AttachmentName = fileName.ToString(), 
                                                                 AttachmentURL=fileURL.ToString() ,
                                                                 EventId= Convert.ToInt32(eventID)
                                                                });
                    }

                    caseEvents.Add(new Event() { EventId = Convert.ToInt32(eventID),
                                                 ItemId= Convert.ToInt32(Id),
                                                 EventType=eventType.ToString(),
                                                 EventName= eventName.ToString(),
                                                 EventDate=eventDate.ToString(),
                                                 EventText=eventText.ToString(),
                                                 CaseAttachments=eventAttachments
                                               }
                                  );
                }
                //End Of Events and Attachments
                workItems.Add(new WorkItem() {
                    
                    Id = Id, 
                    Title = CaseTitle ,
                    ProjectName=CaseProject,
                    LatestTextSummary=CaseLatestSummary,
                    Status=CaseStatus,
                    Category=CaseCategory,
                    Priority=CasePriority,
                    Milestone=CaseMilestone,
                    Area=CaseArea,
                    AssignedTo=CaseAssignedTo,
                    EventList=caseEvents
                });
               
                Console.WriteLine(i);
                i++;
            }
                return workItems;
        }
    }
}
