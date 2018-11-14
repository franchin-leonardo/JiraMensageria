using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using JiraMensageria.Models;


namespace JiraMensageria.Controller
{
    public class TicketController 
    {
        #region Váriaveis Declaradas
        private readonly RestClient stf;
        private readonly RestClient tkt;
        #endregion

        #region Construtor da Classe
        public TicketController()
        {
            stf = new RestClient(new Uri("https://hammerbr.atlassian.net"))
            {
                Authenticator = new HttpBasicAuthenticator("e_integracaoJira", "@7px&hBc")
            };

            tkt = new RestClient(new Uri("http://jirahomol.lanet.accorservices.net"))
            {
                Authenticator = new HttpBasicAuthenticator("appjirastefanini", "J!r@edenred")
            };
        }
        #endregion

        public string CommentCreated(JObject obj)
        {
            string log = "comment_created";
            try
            {
                string issueKey;
                if (bool.Parse(obj["issue"]["fields"]["issuetype"]["subtask"].ToString()))
                {
                    issueKey = obj["issue"]["fields"]["parent"]["key"].ToString();
                }
                else
                {
                    issueKey = obj["issue"]["key"].ToString();
                }
                IRestResponse r = SearchIssue(issueKey);

                if (r.IsSuccessful)
                {
                    JObject stfIssue = JObject.Parse(r.Content.ToString());

                    if (int.Parse(stfIssue["total"].ToString()) > 0)
                    {
                        stf.Execute(
                            new RestRequest($@"/rest/api/2/issue/{stfIssue["issues"][0]["id"].ToString()}/comment", Method.POST)
                            {
                                RequestFormat = DataFormat.Json
                            }.AddBody(new
                            {
                                body = obj["comment"]["body"].ToString(),
                                visibility = new { }
                            }));
                    }
                }
                else
                {
                    Log($"{log}-SearchError", r.Content, true);
                }
                return r.Content;
            }
            catch (Exception ex)
            {
                return "errou";
            }
        }
        
        public void NewIssue(JObject obj)
        {
            try
            {
                if (obj["user"]["name"].Value<string>() != "appjirastefanini")
                {
                    bool isSubTask = Convert.ToBoolean(obj["issue"]["fields"]["issuetype"]["subtask"].ToString());

                    if (!isSubTask)
                    {
                        JObject newIssue = JObject.Parse(JsonConvert.SerializeObject(IssueCreate(obj)));
                        JArray affectedCountries = JsonConvert.DeserializeObject<JArray>(obj["issue"]["fields"]["customfield_10400"].ToString());

                        foreach (var item in affectedCountries)
                        {
                            string projectKey = "";
                            string origem = "";

                            switch (item["value"].ToString().Trim().ToUpper())
                            {
                                case "BRAZIL":
                                    projectKey = GroupTypes.BRASIL;
                                    origem = "Brasil";
                                    break;
                                case "BRASIL":
                                    projectKey = GroupTypes.BRASIL;
                                    origem = "Brasil";
                                    break;
                                case "CHILE":
                                    projectKey = GroupTypes.CHILE;
                                    origem = "Chile";
                                    break;
                                case "MÉXICO":
                                    projectKey = GroupTypes.MÉXICO;
                                    origem = "México";
                                    break;
                                case "MEXICO":
                                    projectKey = GroupTypes.MÉXICO;
                                    origem = "México";
                                    break;
                            }

                            object project = new
                            {
                                key = projectKey
                            };

                            newIssue["fields"]["project"] = JObject.Parse(JsonConvert.SerializeObject(project));
                            newIssue["fields"]["customfield_10705"] = origem;

                            stf.Execute(new RestRequest("rest/api/2/issue", Method.POST).AddBody(newIssue));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var array = new JArray();

                using (StreamReader read = new StreamReader(Path.Combine($@"{HttpRuntime.AppDomainAppPath}\Files\", "logError.json")))
                {
                    array = JArray.Parse(read.ReadToEnd());
                    array.Add(JObject.Parse(JsonConvert.SerializeObject(e)));
                }

                using (StreamWriter write = File.CreateText(Path.Combine($@"{HttpRuntime.AppDomainAppPath}\Files\", "logError.json")))
                {
                    write.WriteLine(JsonConvert.SerializeObject(array, Formatting.Indented));
                }
            }
        }

        public object IssueCreate(JObject obj)
        {
            JObject fields = JsonConvert.DeserializeObject<JObject>(obj["issue"]["fields"].ToString());
            string issueType = fields["issuetype"]["name"]
                .ToString()
                .Trim()
                .Replace(" ", "")
                .Replace("-", "TRAÇO")
                .Replace("&", "ECOMERCIAL")
                .ToUpper();

            object newIssue = new
            {
                fields = new
                {
                    project = new
                    {

                    },
                    summary = fields["summary"].ToString(),
                    issuetype = new
                    {
                        id = (Int32)Enum.Parse(typeof(ExpandIssues), issueType),
                    },
                    assignee = new
                    {
                        name = fields["customfield_10219"]["name"].ToString() ?? null
                    },
                    reporter = new
                    {
                        name = fields["customfield_10902"][0]["name"].ToString() ?? null
                    },
                    priority = new
                    {
                        id = fields["priority"]["id"].ToString()
                    },
                    environment = fields["environment"].ToString() ?? "Environment",
                    description = fields["description"].ToString() ?? "Description",
                    customfield_10707 = obj["issue"]["key"].ToString(),
                    customfield_10705 = "",
                }
            };

            return newIssue;
        }

        public void IssueUpdated(JObject obj)
        {           
                switch (obj["changelog"]["items"][0]["field"].ToString())
                {
                    case "Attachment": UpdateAttachments(obj); break;
                    case "assignee": UpdateAssignee(obj); break;
                    default: break;
                }            
        }

        private void UpdateAssignee(JObject obj)
        {
            string log = "issue_updated_assignee";
            try
            {
                string nome = obj["changelog"]["items"][0]["to"].ToString();
                string issueKey;
                if (bool.Parse(obj["issue"]["fields"]["issuetype"]["subtask"].ToString()))
                {
                    issueKey = obj["issue"]["fields"]["parent"]["key"].ToString();
                }
                else
                {
                    issueKey = obj["issue"]["key"].ToString();
                }

                IRestResponse r = SearchIssue(issueKey);

                if (r.IsSuccessful)
                {
                    JObject stfIssue = JObject.Parse(r.Content.ToString());

                    stf.Execute(new RestRequest($"/rest/api/2/issue/{stfIssue["issues"][0]["id"].ToString()}/assignee", Method.PUT)
                    {
                        RequestFormat = DataFormat.Json
                    }.AddBody(new { name = nome }));
                }
                else
                {
                    Log($"{log}-SearchError", r.Content, true);
                }
            }
            catch (Exception ex)
            {
                Log($"{log}-exception", $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}", true);
            }
        }

        private void UpdateAttachments(JObject obj)
        {
            string log = "issue_updated_attachments";
            try
            {
                string url = "https://jirahomol.lanet.accorservices.com/secure/attachment";
                string id = obj["changelog"]["items"][0]["to"].ToString();
                string nome = obj["changelog"]["items"][0]["toString"].ToString();
                string issueKey;
                if (bool.Parse(obj["issue"]["fields"]["issuetype"]["subtask"].ToString()))
                {
                    issueKey = obj["issue"]["fields"]["parent"]["key"].ToString();
                }
                else
                {
                    issueKey = obj["issue"]["key"].ToString();
                }

                IRestResponse r = SearchIssue(issueKey);

                if (r.IsSuccessful)
                {
                    JObject stfIssue = JObject.Parse(r.Content.ToString());

                    IRestResponse rr = stf.Execute(new RestRequest($"/rest/api/2/issue/{stfIssue["issues"][0]["id"]}/attachments", Method.POST)
                    {
                        RequestFormat = DataFormat.Json
                    }.AddFile(nome.Substring(0, nome.IndexOf('.')), GetFile($"{url}/{id}/{nome}"), nome));
                }
                else
                {
                    Log($"{log}-SearchError", r.Content, true);
                }
            }
            catch (Exception ex)
            {
                Log($"{log}-exception", $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}", true);
            }
        }
        #region Geral
        public IRestResponse SearchIssue(string issueKey)
        {
            return stf.Execute(new RestRequest($@"/rest/api/2/search?jql='cf[10707]'~'{issueKey}'", Method.GET)
            {
                RequestFormat = DataFormat.Json
            });
        }

        public byte[] GetFile(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);

            String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("appjirastefanini:J!r@edenred"));
            request.Headers.Add("Authorization", "Basic " + encoded);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
            }

            return buffer;
        }

        private void Log(string filename, string data, bool isError)
        {
            string mydocpath = $"{HttpRuntime.AppDomainAppPath}/Files/{(isError ? "errors" : "actions")}/{filename}";

            if (!Directory.Exists(mydocpath))
            {
                Directory.CreateDirectory(mydocpath);
            }

            filename = $"{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.log";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath, filename), true))
            {
                outputFile.WriteLine(data);
            }
        }
        #endregion
    }
}
