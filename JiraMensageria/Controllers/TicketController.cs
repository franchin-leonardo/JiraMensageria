using JiraMensageria.Helpers;
using JiraMensageria.Models;
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



namespace JiraMensageria.Controller
{
    public class TicketController
    {
        #region Váriaveis Declaradas
        private readonly JiraClient client = new JiraClient();
        #endregion

        #region Endpoints para eventos de alterações de comentários
        public bool CommentCreated(JObject obj)
        {
            Logger.Now.LogEvent("CommentCreated", "TICKET", "Entrou no método");
            bool _r = false;
            try
            {
                string user = obj["user"]["name"].ToString();
                if (user != "appjirastefanini")
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
                    IRestResponse r = client.Stefanini.BuscarPorCustomField(issueKey, "10707");
                    
                    if (r.IsSuccessful)
                    {
                        Logger.Now.LogEvent("CommentCreated", "TICKET", "Finalizou", $"SUCESSO");
                        JObject stfIssue = JObject.Parse(r.Content.ToString());

                        if (int.Parse(stfIssue["total"].ToString()) > 0)
                        {
                            IRestResponse rr = client.Stefanini.CreateComment(stfIssue["issues"][0]["id"].ToString(), obj["comment"]["body"].ToString());
                            Logger.Now.LogEvent("CommentCreated", "TICKET", "Finalizou", rr.IsSuccessful ? "SUCESSO" : $"FALHA: {rr.Content}", obj.ToString());
                            _r = rr.IsSuccessful;
                        }

                    }
                    Logger.Now.LogEvent("CommentCreated", "TICKET", "Finalizou", $"FALHA: {r.Content}");
                }
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("CommentCreated", "TICKET", "Exceção gerada", e.Message);
            }

            return _r;
        }
        #endregion

        #region Endpoints para os eventos de criações de Issues

        public bool NewIssue(JObject obj)
        {
            Logger.Now.LogEvent("NewIssue", "TICKET", "Entrou no método");
            bool _r = false;
            try
            {
                if (obj["user"]["name"].ToString() != "appjirastefanini")
                {
                    JObject fields = JsonConvert.DeserializeObject<JObject>(obj["issue"]["fields"].ToString());

                    bool isSubTask = Convert.ToBoolean(fields["issuetype"]["subtask"].ToString());

                    if (!isSubTask)
                    {
                        if (fields.ContainsKey("customfield_10400"))
                        {
                            JArray affectedCountries = JsonConvert.DeserializeObject<JArray>(fields["customfield_10400"].ToString());

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

                                _r = IssueCreate(obj, projectKey, origem);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("NewIssue", "TICKET", "Exceção gerada", e.Message);
            }

            return _r;
        }

        public bool IssueCreate(JObject obj, string projectKey, string origem)
        {
            Logger.Now.LogEvent("IssueCreate", "TICKET", "Entrou no método");
            bool _r = false;
            try
            {
                JObject fields = JsonConvert.DeserializeObject<JObject>(obj["issue"]["fields"].ToString());

                string issueType = fields["issuetype"]["name"]
                    .ToString()
                    .Trim()
                    .Replace(" ", "")
                    .Replace("-", "TRAÇO")
                    .Replace("&", "ECOMERCIAL")
                    .ToUpper();

                Int32 issueTypeId = (Int32)Enum.Parse(typeof(ExpandIssues), issueType);
                String description = fields.ContainsKey("description") ? fields["description"].ToString() : "";

                IRestResponse response = client.Stefanini.CreateIssue(projectKey, fields["summary"].ToString(),
                                                                      issueTypeId, fields["priority"]["id"].ToString(),
                                                                      description, obj["issue"]["key"].ToString(),
                                                                      fields["reporter"]["displayName"].ToString(),
                                                                      origem);
                _r = response.IsSuccessful;
   
                if (response.IsSuccessful)
                {
                    
                    Logger.Now.LogEvent("IssueCreate", "TICKET", "Finalizou", "SUCESSO", response.Content);
                    JObject issue = JsonConvert.DeserializeObject<JObject>(response.Content.ToString());
                    JArray getAttachment = JsonConvert.DeserializeObject<JArray>(obj["issue"]["fields"]["attachment"].ToString());

                    if (getAttachment.Count != 0)
                    {
                        foreach (var file in getAttachment)
                        {
                            string url = $"/secure/attachment/{file["id"].ToString()}/{file["filename"].ToString()}";
                            string nome = file["filename"].ToString();

                            var r = client.Ticket.Buscar(url);

                            IRestResponse rr = client.Stefanini.CreateAttachment(issue["Key"].ToString(), r.RawBytes, nome, r.ContentType);
                            Logger.Now.LogEvent("IssueCreate", "TICKET", "Finalizou", rr.IsSuccessful ? "SUCESSO" : $"FALHA: {rr.Content}", obj.ToString());
                        }
                    }
                }
                Logger.Now.LogEvent("IssueCreate", "TICKET", "Finalizou", $"FALHA: {response.Content}");
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("IssueCreate", "TICKET", "Exceção gerada", e.Message);
            }
            return _r;
        }
    
        public bool IssueUpdated(JObject obj)
        {
            Logger.Now.LogEvent("IssueUpdated", "TICKET", "Entrou no método");
            bool _r = false;
            string user = obj["user"]["name"].ToString();
            if (user != "appjirastefanini")
            {
                JArray changLogs = JsonConvert.DeserializeObject<JArray>(obj["changelog"]["items"].ToString());
                foreach (var log in changLogs)
                {
                    switch (log["field"].ToString())
                    {
                        case "Attachment":  _r =UpdateAttachments(obj); break;
                        //case "assignee":  _r = UpdateAssignee(obj); break;
                        case "status":      _r = UpdateStatus(obj); break;
                        case "description": _r = UpdateDescription(obj); break;
                    }
                }
            }

            return _r;
        }

        private bool UpdateDescription(JObject obj)
        {
            string key = GetStfKey(obj);
            bool _r = false;
            JArray changLogs = JsonConvert.DeserializeObject<JArray>(obj["changelog"]["items"].ToString());
            foreach (var log in changLogs)
            {
                if (log["field"].ToString() == "description")
                {
                    IRestResponse r = client.Stefanini.UpdateDescription(key, log["toString"].ToString());
                    _r = r.IsSuccessful;
                }
            }
            return _r;
        }

        private void UpdateAssignee(JObject obj)
        {
            Logger.Now.LogEvent("UpdateAssignee", "TICKET", "Entrou no método");
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

                IRestResponse r = client.Stefanini.BuscarPorCustomField(issueKey, "10707");

                if (r.IsSuccessful)
                {
                    Logger.Now.LogEvent("UpdateAssignee", "TICKET", "Finalizou", "SUCESSO", obj.ToString());
                    JObject stfIssue = JObject.Parse(r.Content.ToString());

                    IRestResponse rr = client.Stefanini.UpdateAssignee(stfIssue["issues"][0]["id"].ToString(), nome);

                    Logger.Now.LogEvent("UpdateAssignee", "TICKET", "Finalizou", rr.IsSuccessful ? "SUCESSO" : $"FALHA: {rr.Content}", obj.ToString());
                }
                Logger.Now.LogEvent("UpdateAttachments", "TICKET", "Finalizou", $"FALHA: {r.Content}");

            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("UpdateAssignee", "TICKET", "Exceção gerada", e.Message);
            }
        }

        private bool UpdateAttachments(JObject obj)
        {
            Logger.Now.LogEvent("UpdateAttachments", "TICKET", "Entrou no método");
            bool _r = false;
            try
            {
                string url = $"/secure/attachment/{obj["changelog"]["items"][0]["to"].ToString()}/{obj["changelog"]["items"][0]["toString"].ToString()}";
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

                IRestResponse r = client.Stefanini.BuscarPorCustomField(issueKey, "10707");
                

                JObject stfIssue = JObject.Parse(r.Content.ToString());

                string id = stfIssue["issues"][0]["id"].ToString();

                if (r.IsSuccessful)
                {
                    Logger.Now.LogEvent("UpdateAttachments", "TICKET", "Finalizou", "SUCESSO");
                    var req = client.Ticket.Buscar(url);

                    IRestResponse rr = client.Stefanini.CreateAttachment(id, req.RawBytes, nome, req.ContentType);
                    _r = rr.IsSuccessful;

                    Logger.Now.LogEvent("UpdateAttachments", "TICKET", "Finalizou", rr.IsSuccessful ? "SUCESSO" : $"FALHA: {rr.Content}", obj.ToString());
               
                }
                Logger.Now.LogEvent("UpdateAttachments", "TICKET", "Finalizou", $"FALHA: {r.Content}");
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("UpdateAttachments", "TICKET", "Exceção gerada", e.Message);
            }

            return _r;
        }

        private bool UpdateStatus(JObject obj)
        {
            Logger.Now.LogEvent("UpdateStatus", "TICKET", "Entrou no método");
            bool _r = false;
            Dictionary<string, string> statuses = new Dictionary<string, string>()
            {
                {"10301", "10404"},
                {"10822", "10600"},
                {"10820", "10501"},
                {"10823", "10504"},
                {"10302", "10504"},
                {"10828", "10506"},
                {"10827", "10521"},
                {"10818", "10505"},
                {"10821", "10510"},
                {"10824", "10511"},
                {"10825", "10510"},
                {"10819", "10514"},
                {"10817", "10207"}
            };
            try
            {
                bool isSubTask = Convert.ToBoolean(obj["issue"]["fields"]["issuetype"]["subtask"].ToString());

                if (!isSubTask)
                {
                    string key = GetStfKey(obj);

                    string statusId = statuses[obj["issue"]["fields"]["status"]["id"].ToString()];

                    IRestResponse irr = client.Stefanini.BuscarTransitions(key);
                    _r = irr.IsSuccessful;
                    Logger.Now.LogEvent("UpdateStatus", "TICKET", "BuscarTransitions", irr.IsSuccessful ? "SUCESSO" : $"FALHA: {irr.Content}");

                    JObject t = JObject.Parse(irr.Content.ToString());
                    JArray transitions = JsonConvert.DeserializeObject<JArray>(t["transitions"].ToString());

                    foreach (var item in transitions)
                    {
                        if (item["to"]["id"].ToString().Equals(statusId))
                        {
                            int id = int.Parse(item["id"].ToString());
                            IRestResponse r = client.Stefanini.UpdateStatus(key, id);
                            
                            Logger.Now.LogEvent("UpdateStatus", "TICKET", "UpdateStatus", r.IsSuccessful ? "SUCESSO" : $"FALHA: {r.Content}");
                            break;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("UpdateStatus", "TICKET", "Exceção gerada", e.Message);
            }

            return _r;
        }

        public string GetStfKey(JObject obj)
        {
            Logger.Now.LogEvent("UpdateStatus", "TICKET", "Entrou no método");

            string issueKey;

            if (bool.Parse(obj["issue"]["fields"]["issuetype"]["subtask"].ToString()))
            {
                issueKey = obj["issue"]["fields"]["parent"]["key"].ToString();
            }
            else
            {
                issueKey = obj["issue"]["key"].ToString();
            }

            IRestResponse r = client.Stefanini.BuscarPorCustomField(issueKey, "10707");
            Logger.Now.LogEvent("UpdateStatus", "TICKET", "UpdateStatus", r.IsSuccessful ? "SUCESSO" : $"FALHA: {r.Content}");
            JObject stfIssue = JObject.Parse(r.Content.ToString());

            string key = stfIssue["issues"][0]["key"].ToString();

            return key;
        }

        #endregion

        #region Classe não utilizada lógica possívelmente útil (guardando)
        //[HttpPost]
        //[Route("ticket/project/created")]
        //public object ProjectCreatedAsync(JObject obj)
        //{
        //    var objForPost = new
        //    {
        //        key = obj["project"]["key"].ToString(),
        //        name = obj["project"]["name"].ToString(),
        //        projectTypeKey = "business",
        //        projectTemplateKey = "com.atlassian.jira-core-project-templates:jira-core-simplified-project-management",
        //        description = "",
        //        assigneeType = "UNASSIGNED",
        //        lead = "admin"
        //    };

        //    var request = new RestRequest("rest/api/2/project", Method.POST)
        //    {
        //        RequestFormat = DataFormat.Json,
        //    };
        //    request.AddBody(objForPost);
        //    var response = JsonConvert.DeserializeObject(stf.Execute(request).Content);
        //    return response;
        //}
        #endregion
    }
}
