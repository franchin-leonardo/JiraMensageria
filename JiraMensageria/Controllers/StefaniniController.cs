using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using JiraMensageria.Models;

namespace JiraMensageria.Controller
{
    public class StefaniniController 
    {
        #region Váriaveis Declaradas
        private readonly string username;
        private readonly string password;
        private readonly RestClient stf;
        private readonly RestClient tkt;
        #endregion

        #region Construtor da Classe
        public StefaniniController()
        {
            stf = new RestClient(new Uri("https://jiracorp.stefanini.com"))
            {
                Authenticator = new HttpBasicAuthenticator("e_integracaoJira", "@7px&hBc")
            };

            tkt = new RestClient(new Uri("http://172.29.95.124"))
            {
                Authenticator = new HttpBasicAuthenticator("appjirastefanini", "J!r@edenred")
            };
        }
        #endregion

        #region Atualização de issue
       
        public void IssueUpdated(JObject obj)
        {
            JArray changLogs = JsonConvert.DeserializeObject<JArray>(obj["changelog"]["items"].ToString());

            foreach (var log in changLogs)
            {
                switch (log["field"].ToString())
                {
                    case "Attachment": UpdateAttachments(obj); break;
                    case "assignee": UpdateAssignee(log["toString"].ToString(), obj); break;
                    case "status": UpdateStatus(log["toString"].ToString(), obj); break;
                    default: break;
                }
            }
        }

        public void UpdateStatus(JObject obj)
        {
            bool isSubTask = Convert.ToBoolean(obj["issue"]["fields"]["issuetype"]["subtask"].ToString());
            object updateIssue = new object();

            if (!isSubTask)
            {
                try
                {
                    string codigoExterno = obj["fields"]["customfield_10707"].ToString();

                    string statusNameFormat = obj["fields"]["status"]["name"].ToString()
                        .ToUpper()
                        .Trim()
                        .Replace(" ", "")
                        .Replace("-", "TRAÇO")
                        .Replace("/", "BARRA")
                        .Replace("+", "MAIS");

                    int statusId = (Int32)Enum.Parse(typeof(ExpandStatustTicket), statusNameFormat);

                    updateIssue = new
                    {
                        transition = new
                        {
                            id = statusId
                        }
                    };

                    IRestResponse response = tkt.Execute(new RestRequest($"rest/api/2/issue/{codigoExterno}/transitions", Method.POST)
                    {
                        RequestFormat = DataFormat.Json
                    }.AddBody(updateIssue));


                    NewLog("event", obj, JObject.Parse(JsonConvert.SerializeObject(updateIssue)), JObject.Parse(JsonConvert.SerializeObject(response)));
                }
                catch (Exception e)
                {
                    NewLog("error", obj, JObject.Parse(JsonConvert.SerializeObject(updateIssue)), JObject.Parse(JsonConvert.SerializeObject(e)));
                }
            }
        }

        private void UpdateAssignee(string assigneeName, JObject obj)
        {
            bool isSubTask = Convert.ToBoolean(obj["issue"]["fields"]["issuetype"]["subtask"].ToString());
            object updateIssue = new object();

            if (!isSubTask)
            {
                try
                {
                    string issueKey = obj["issue"]["key"].ToString();
                    string codigoExterno = obj["issue"]["fields"]["customfield_10707"].ToString();

                    IRestResponse r = SearchIssueStf(issueKey);

                    if (r.IsSuccessful)
                    {
                        updateIssue = new
                        {
                            fields = new
                            {
                                customfield_10219 = new
                                {
                                    name = assigneeName
                                }
                            }
                        };

                        IRestResponse response = tkt.Execute(new RestRequest($"/rest/api/2/issue/{codigoExterno}", Method.PUT)
                        {
                            RequestFormat = DataFormat.Json
                        }.AddBody(updateIssue));

                        NewLog("event", obj, JObject.Parse(JsonConvert.SerializeObject(updateIssue)), JObject.Parse(JsonConvert.SerializeObject(response)));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    NewLog("error", obj, JObject.Parse(JsonConvert.SerializeObject(updateIssue)), JObject.Parse(JsonConvert.SerializeObject(e)));
                }
            }
        }

        private void UpdateAttachments(JObject obj)
        {
            try
            {
                string url = "https://hammerbr.atlassian.net/secure/attachment";
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

                IRestResponse r = SearchIssueStf(issueKey);

                if (r.IsSuccessful)
                {
                    JObject stfIssue = JObject.Parse(r.Content.ToString());
                    string key = stfIssue["issues"][0]["fields"]["customfield_10024"].ToString();
                    IRestResponse response = tkt.Execute(new RestRequest($"/rest/api/2/issue/{key}/attachments", Method.POST)
                    {
                        RequestFormat = DataFormat.Json
                    }.AddFile(nome.Substring(0, nome.IndexOf('.')), GetFile($"{url}/{id}/{nome}"), nome));

                    NewLog("event", obj, new JObject(), JObject.Parse(JsonConvert.SerializeObject(response)));
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                NewLog("error", obj, new JObject(), JObject.Parse(JsonConvert.SerializeObject(e)));
            }
        }
        #endregion
       

        #region Atualização de worklog
       
        public void WorklogCreated(JObject obj)
        {
            object newWorklog = new object();

            try
            {
                //Aqui eu recupero as informações que o evento me disponibiliza.
                string issueId = obj["worklog"]["issueId"].ToString();
                double timeSpentSeconds = double.Parse(obj["worklog"]["timeSpentSeconds"].ToString()) / 3600;

                //Aqui eu busco a issue que o evento se refere.
                JObject issue = JsonConvert.DeserializeObject<JObject>(SearchIssueStf(issueId).Content.ToString());

                if (issue["fields"].ToString().Contains("customfield_10706"))
                {
                    //Aqui eu recupero as informações da issue sendo:
                    //projectStage = Estágio do projeto que recebera o worklog.
                    //parentKey = Chave da issue pai, para que eu possa recuperar o "código externo".
                    //projectKey = Chave do projeto, para que eu possa executar uma instrução JQL.
                    string projectStage = issue["fields"]["customfield_10706"]["value"].ToString();
                    string parentKey = issue["fields"]["parent"]["key"].ToString();
                    string projectKey = issue["fields"]["project"]["key"].ToString();

                    //Aqui eu busco a issue pai e verifico se possui o código externo
                    JObject parentIssue = JsonConvert.DeserializeObject<JObject>(SearchIssueStf(parentKey).Content.ToString());

                    if (parentIssue["fields"].ToString().Contains("customfield_11220"))
                    {
                        projectStage += " - Change Request";
                    }

                    if (parentIssue["fields"].ToString().Contains("customfield_10707"))
                    {
                        string codigoExterno = parentIssue["fields"]["customfield_10707"].ToString();

                        //Aqui eu busco a issue referênciada pelo código externo
                        IRestResponse searchIssueReferency = SearchIssueTkt(codigoExterno);

                        if (searchIssueReferency.IsSuccessful)
                        {
                            string jql = "";

                            //Aqui eu realizo uma instrução jql
                            //Para buscar a issue cujo o estagio do projeto esta sendo referenciado
                            jql = $"jql=parent=\"{codigoExterno}\"and\"summary\"~\"{projectStage}\"";
                            IRestResponse searchIssueStage = SearchIssuesTktWithJql(jql);

                            if (searchIssueStage.IsSuccessful)
                            {
                                JObject issuesReturn = JsonConvert.DeserializeObject<JObject>(searchIssueStage.Content.ToString());
                                JArray arrayIssues = JsonConvert.DeserializeObject<JArray>(issuesReturn["issues"].ToString());

                                string keySubTask = "";
                                int storyPoint = 0;

                                if (arrayIssues.Count.Equals(0))
                                {
                                    JObject issueReferency = JsonConvert.DeserializeObject<JObject>(searchIssueReferency.Content.ToString());

                                    object newSubTask = new
                                    {
                                        fields = new
                                        {
                                            project = new
                                            {
                                                key = issueReferency["fields"]["project"]["key"].ToString()
                                            },
                                            parent = new
                                            {
                                                key = issueReferency["key"].ToString()
                                            },
                                            summary = projectStage,
                                            issuetype = new
                                            {
                                                id = "10502"
                                            }
                                        }
                                    };

                                    IRestResponse restResponse = tkt.Execute(new RestRequest("rest/api/2/issue", Method.POST)
                                    {
                                        RequestFormat = DataFormat.Json
                                    }.AddBody(newSubTask));

                                    keySubTask = JsonConvert.DeserializeObject<JObject>(restResponse.Content.ToString())["key"].ToString();
                                }
                                else
                                {
                                    foreach (var item in arrayIssues)
                                    {
                                        if (item["fields"]["summary"].ToString().Trim().Equals(projectStage))
                                        {
                                            if (item["fields"].ToString().Contains("customfield_10006"))
                                            {
                                                storyPoint = Convert.ToInt16(item["fields"]["customfield_10006"]);
                                            }

                                            keySubTask = item["key"].ToString();
                                        }
                                    }
                                }

                                //Aqui eu executo uma instrução JQL, 
                                //para que eu possa recuperar os "timeTraking" de todas as subtask que possuem a mesma referência para o estágio do projeto.
                                jql = $"jql=project=\"{projectKey}\"and\"parent\"=\"{parentKey}\"and\"cf[10706]\"=\"{projectStage}\"&fields=timetracking&expand=issues.renderedFields";
                                JObject issuesForProjectStage = JsonConvert.DeserializeObject<JObject>(SearchIssuesStfWithJql(jql).Content.ToString());

                                //Aqui eu converto as issues retornadas para um array
                                JArray issues = JsonConvert.DeserializeObject<JArray>(issuesForProjectStage["issues"].ToString());

                                int originalEstimate = 0;
                                int remainingEstimate = 0;

                                foreach (var item in issues)
                                {
                                    originalEstimate += item["fields"]["timetracking"]["originalEstimateSeconds"].ToString() != "0" ?
                                        Convert.ToInt32(item["fields"]["timetracking"]["originalEstimateSeconds"]) / 3600 : 0;

                                    remainingEstimate += item["fields"]["timetracking"]["remainingEstimateSeconds"].ToString() != "0" ?
                                        Convert.ToInt32(item["fields"]["timetracking"]["remainingEstimateSeconds"]) / 3600 : 0;
                                }

                                Int32 spent = Convert.ToInt32(((timeSpentSeconds / (originalEstimate + remainingEstimate)) * (storyPoint * 10)) * 3600);

                                newWorklog = new
                                {
                                    timeSpentSeconds = spent
                                };

                                IRestResponse response = tkt.Execute(new RestRequest($"rest/api/2/issue/{keySubTask}/worklog", Method.POST)
                                {
                                    RequestFormat = DataFormat.Json
                                }.AddBody(newWorklog));


                                NewLog("event", obj, JObject.Parse(JsonConvert.SerializeObject(newWorklog)), JsonConvert.DeserializeObject<JObject>(response.Content.ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                NewLog("error", obj, JObject.Parse(JsonConvert.SerializeObject(newWorklog)), JObject.Parse(JsonConvert.SerializeObject(e)));
            }
        }
        #endregion

        #region Métodos de pesquisa
        public IRestResponse SearchIssueStf(string issueKey)
        {
            return stf.Execute(new RestRequest($"rest/api/2/issue/{issueKey}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            });
        }

        public IRestResponse SearchIssueTkt(string issueKey)
        {
            return tkt.Execute(new RestRequest($"rest/api/2/issue/{issueKey}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            });
        }

        public IRestResponse SearchIssuesStfWithJql(string jql)
        {
            return stf.Execute(new RestRequest($"rest/api/2/search?{jql}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            });
        }

        public IRestResponse SearchIssuesTktWithJql(string jql)
        {
            return tkt.Execute(new RestRequest($"rest/api/2/search?{jql}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            });
        }
        #endregion

        #region Geral
        public byte[] GetFile(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);

            String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
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
        public void NewLog(string type, JObject objReceived, JObject objResponse, JObject responseServer)
        {
            var array = new JArray();
            string path = "";

            if (type.Equals("event"))
            {
                path = Path.Combine($@"{HttpRuntime.AppDomainAppPath}\Files\", "logEvent.json");
            }
            else
            {
                path = Path.Combine($@"{HttpRuntime.AppDomainAppPath}\Files\", "logError.json");
            }

            using (StreamReader read = new StreamReader(path))
            {
                array = JArray.Parse(read.ReadToEnd());
            }

            object newLog = new
            {
                data = DateTime.Now.ToLongDateString(),
                evento = objReceived["webhookEvent"].ToString(),
                objetoRecebido = objReceived,
                objetoRetornado = objResponse,
                responseServer
            };

            array.Add(JObject.Parse(JsonConvert.SerializeObject(newLog)));

            using (StreamWriter write = File.CreateText(path))
            {
                write.WriteLine(JsonConvert.SerializeObject(array, Formatting.Indented));
            }
        }
        #endregion
    }
}
