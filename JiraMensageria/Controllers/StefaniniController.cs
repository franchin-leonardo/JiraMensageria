using JiraMensageria.Helpers;
using JiraMensageria.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace JiraMensageria.Controller
{
    public class StefaniniController 
    {
        #region Váriaveis Declaradas
        private readonly JiraClient client = new JiraClient();
        #endregion
       
        public void IssueCreated(JObject obj)
        {
            Logger.Now.LogEvent("IssueCreated", "STEFANINI", "Entrou no método");
            try
            {
                string user = obj["user"]["name"].ToString();
                if (user != "e_integracaoJira")
                {
                    //verificar se é subtask
                    bool isSubTask = Convert.ToBoolean(obj["issue"]["fields"]["issuetype"]["subtask"].ToString());
                    if (isSubTask)
                    {
                        //verificar se existe uma subtask com a phase customfield_10706 na ticket
                        string parentKey = obj["issue"]["fields"]["parent"]["key"].ToString();

                        IRestResponse r = client.Stefanini.BuscarIssuePorChave(parentKey);
                        Logger.Now.LogEvent("IssueCreated", "STEFANINI", "BuscarIssuePorChave", r.IsSuccessful ? "SUCESSO" : $"FALHA: {r.Content}");

                        JObject parentIssue = JObject.Parse(r.Content.ToString());

                        string codigoExterno = parentIssue["fields"]["customfield_10707"].ToString();
                        string phase = obj["issue"]["fields"]["customfield_10706"]["value"].ToString();
                        string search = phase.Replace(" - ", " ");

                        string jql = $"jql='parent'='{codigoExterno}'AND'summary'~'{search}'";

                        IRestResponse response = client.Ticket.BuscarIssuesComJql(jql);
                        Logger.Now.LogEvent("IssueCreated", "STEFANINI", "BuscarIssuesComJql", response.IsSuccessful ? "SUCESSO" : $"FALHA: {response.Content}");

                        JObject tktIssue = JObject.Parse(response.Content.ToString());

                        JArray issues = JsonConvert.DeserializeObject<JArray>(tktIssue["issues"].ToString());

                        int total = 0;
                        foreach (var issue in issues)
                        {
                            if (issue["fields"]["summary"].ToString().Equals(phase))
                            {
                                total++;
                            }
                        }
                        //se não existir criar
                        if (total == 0)
                        {
                            JObject issue = JObject.Parse(client.Ticket.BuscarIssuePorChave(codigoExterno).Content.ToString());
                            string projectKey = issue["fields"]["project"]["key"].ToString();

                            string startDate = "";
                            string endDate = "";
                            int originEstimate = 0;

                            switch (phase)
                            {
                                case "Inception":
                                    startDate = parentIssue["fields"]["customfield_11208"].ToString();
                                    endDate = parentIssue["fields"]["customfield_11209"].ToString();
                                    break;
                                case "Planning":
                                    endDate = parentIssue["fields"]["customfield_11203"].ToString();
                                    break;
                                case "Grooming":
                                    endDate = parentIssue["fields"]["customfield_11204"].ToString();
                                    break;
                                case "Desenvolvimento":
                                    startDate = parentIssue["fields"]["customfield_11101"].ToString();
                                    endDate = parentIssue["fields"]["customfield_11102"].ToString();
                                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11110"].ToString());
                                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11111"].ToString());
                                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11112"].ToString());
                                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11202"].ToString());
                                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11121"].ToString());
                                    break;
                                case "Review":
                                    endDate = parentIssue["fields"]["customfield_11100"].ToString();
                                    break;
                                case "Review Técnica":
                                    endDate = parentIssue["fields"]["customfield_11212"].ToString();
                                    break;
                                case "Validação em Homologação - Fábrica":
                                    startDate = parentIssue["fields"]["customfield_11105"].ToString();
                                    endDate = parentIssue["fields"]["customfield_11106"].ToString();
                                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11123"].ToString());
                                    break;
                                case "Validação em Pre-Produção - Fábrica":
                                    startDate = parentIssue["fields"]["customfield_11107"].ToString();
                                    endDate = parentIssue["fields"]["customfield_11108"].ToString();
                                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11124"].ToString());
                                    break;
                                case "Implantação em Produção":
                                    endDate = parentIssue["fields"]["customfield_11219"].ToString();
                                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11215"].ToString());
                                    break;
                                case "Validação em QA - Fábrica":
                                    startDate = parentIssue["fields"]["customfield_11103"].ToString();
                                    endDate = parentIssue["fields"]["customfield_11104"].ToString();
                                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11122"].ToString());
                                    break;
                                case "Refinamento":
                                    startDate = parentIssue["fields"]["customfield_11206"].ToString();
                                    endDate = parentIssue["fields"]["customfield_11207"].ToString();
                                    break;
                            }

                            originEstimate *= 10;

                            if (parentIssue["fields"].ToString().Contains("customfield_11220") &&
                                parentIssue["fields"]["customfield_11220"].ToString() != "")
                            {
                                phase += " - Change Request";
                            }

                            IRestResponse resp = client.Ticket.CreateSubTask(projectKey, codigoExterno, phase, originEstimate);
                            Logger.Now.LogEvent("IssueCreated", "STEFANINI", "CreateSubTask", resp.IsSuccessful ? "SUCESSO" : $"FALHA: {resp.Content}");

                            JObject respIssue = JObject.Parse(resp.Content.ToString());
                            IRestResponse rr = client.Ticket.UpdateSubtaskDate(respIssue["key"].ToString(), startDate, endDate);

                            Logger.Now.LogEvent("IssueCreated", "STEFANINI", "UpdateSubtaskDate", rr.IsSuccessful ? "SUCESSO" : $"FALHA: {rr.Content}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("IssueCreated", "STEFANINI", "Exceção gerada", e.Message);
            }

        }

        #region Atualização de issue
        public bool IssueUpdated(JObject obj)
        {
            Logger.Now.LogEvent("IssueUpdated", "STEFANINI", "Entrou no método");
            bool _r = false;
            try
            {
                Dictionary<string, string> phases = new Dictionary<string, string>
                {
                    { "Estimated Planning Date", "Planning" },
                    { "Estimated Start Date - Homologation", "Validação em Homologação - Fábrica" },
                    { "Estimated End Date - Homologation", "Validação em Homologação - Fábrica" },
                    { "Estimated Start Date - PreProd", "Validação em Pre-Produção - Fábrica" },
                    { "Estimated End Date - PreProd", "Validação em Pre-Produção - Fábrica" },
                    { "Estimated Review Date", "Review" },
                    { "Estimated Technical Review Date", "Review Técnico" },
                    { "Estimated Start Date", "Desenvolvimento" },
                    { "Estimated End Date", "Desenvolvimento" },
                    { "Estimated Start Date - QA", "Validação em QA - Fábrica" },
                    { "Estimated End Date - QA", "Validação em QA - Fábrica" },
                    { "Estimated Production Date", "Implantação em Produção" },
                    { "Estimated Grooming Date", "Grooming" },
                    { "Estimated Start Date - Refinement", "Refinamento" },
                    { "Estimated End Date - Refinement", "Refinamento" },
                };
                Dictionary<string, string> storyPoints = new Dictionary<string, string>
                {
                    { "Story Point for PL/SQL", "Desenvolvimento" },
                    { "Story Point for .Net", "Desenvolvimento" },
                    { "Story Point for QA", "Desenvolvimento" },
                    { "Story Point for Authorization", "Desenvolvimento" },
                    { "Story Point (Others)", "Desenvolvimento" },
                    { "Ticket Story Points QA Tests", "Validação em QA - Fábrica" },
                    { "Ticket Story Points Homologation Tests", "Validação em Homologação - Fábrica" },
                    { "Ticket Story Points PreProd Tests", "Validação em Pre-Produção - Fábrica" },
                    { "Ticket Story Points Prod Tests", "Implantação em Produção" },
                };
                string user = obj["user"]["name"].ToString();
                object response = new object();

                if (user != "e_integracaoJira")
                {
                    JArray changLogs = JsonConvert.DeserializeObject<JArray>(obj["changelog"]["items"].ToString());
                    foreach (var log in changLogs)
                    {
                        if (log["fieldtype"].ToString() == "custom")
                        {
                            string codigoExterno = obj["issue"]["fields"]["customfield_10707"].ToString();
                            if (log["field"].ToString() == "Story Point")
                            {
                                client.Ticket.UpdateStoryPoint(codigoExterno, log["toString"].ToString());
                            }
                            else if (phases.ContainsKey(log["field"].ToString()))
                            {
                                string search = phases[log["field"].ToString()];
                                search = search.Replace(" - ", " ");

                                string jql = $"jql='parent'='{codigoExterno}'AND'summary'~'{search}'";
                                IRestResponse rsp = client.Ticket.BuscarIssuesComJql(jql);
                             
                                JObject tktIssue = JObject.Parse(rsp.Content.ToString());
                                JArray issues = JsonConvert.DeserializeObject<JArray>(tktIssue["issues"].ToString());

                                foreach (var issue in issues)
                                {
                                    if (phases[log["field"].ToString()].Equals(issue["fields"]["summary"].ToString()))
                                    {
                                        string startDate = "";
                                        string endDate = "";
                                        switch (phases[log["field"].ToString()])
                                        {
                                            case "Planning":
                                                endDate = obj["issue"]["fields"]["customfield_11203"].ToString();
                                                break;
                                            case "Grooming":
                                                endDate = obj["issue"]["fields"]["customfield_11204"].ToString();
                                                break;
                                            case "Desenvolvimento":
                                                startDate = obj["issue"]["fields"]["customfield_11101"].ToString();
                                                endDate = obj["issue"]["fields"]["customfield_11102"].ToString();
                                                break;
                                            case "Review":
                                                endDate = obj["issue"]["fields"]["customfield_11100"].ToString();
                                                break;
                                            case "Review Técnica":
                                                endDate = obj["issue"]["fields"]["customfield_11212"].ToString();
                                                break;
                                            case "Validação em Homologação - Fábrica":
                                                startDate = obj["issue"]["fields"]["customfield_11105"].ToString();
                                                endDate = obj["issue"]["fields"]["customfield_11106"].ToString();
                                                break;
                                            case "Validação em Pre-Produção - Fábrica":
                                                startDate = obj["issue"]["fields"]["customfield_11107"].ToString();
                                                endDate = obj["issue"]["fields"]["customfield_11108"].ToString();
                                                break;
                                            case "Validação em Produção - Fábrica":
                                                endDate = obj["issue"]["fields"]["customfield_11219"].ToString();
                                                break;
                                            case "Validação em QA - Fábrica":
                                                startDate = obj["issue"]["fields"]["customfield_11103"].ToString();
                                                endDate = obj["issue"]["fields"]["customfield_11104"].ToString();
                                                break;
                                            case "Refinamento":
                                                startDate = obj["issue"]["fields"]["customfield_11206"].ToString();
                                                endDate = obj["issue"]["fields"]["customfield_11207"].ToString();
                                                break;
                                        }
                                        if (tktIssue["total"].ToString() != "0")
                                        {
                                            var resp = client.Ticket.UpdateSubtaskDate(tktIssue["issues"][0]["key"].ToString(), startDate, endDate);
                                            Logger.Now.LogEvent("IssueUpdated", "STEFANINI", "UpdateSubtaskDate", resp.IsSuccessful ? "SUCESSO" : $"FALHA: {resp.Content}");
                                        }
                                    }
                                }
                            }
                            else if (storyPoints.ContainsKey(log["field"].ToString()))
                            {
                                string search = storyPoints[log["field"].ToString()].Replace(" - ", " ");

                                string jql = $"jql='parent'='{codigoExterno}'AND'summary'~'{search}'";
                                IRestResponse rsp = client.Ticket.BuscarIssuesComJql(jql);
                                _r = rsp.IsSuccessful;

                                JObject tktIssue = JObject.Parse(rsp.Content.ToString());
                                JArray issues = JsonConvert.DeserializeObject<JArray>(tktIssue["issues"].ToString());
                                int estimate = 0;
                                string key = "";
                                foreach (var issue in issues)
                                {
                                    if (issue["fields"]["summary"].ToString().Equals(storyPoints[log["field"].ToString()]))
                                    {
                                        key = issue["key"].ToString();
                                        estimate = int.Parse(issue["fields"]["timeoriginalestimate"].ToString() == "" ? "0" : issue["fields"]["timeoriginalestimate"].ToString());
                                        int before = int.Parse(log["fromString"].ToString() == "" ? "0" : log["fromString"].ToString());
                                        int after = int.Parse(log["toString"].ToString() == "" ? "0" : log["toString"].ToString());
                                        estimate -= (before * 10) * 3600;
                                        estimate += (after * 10) * 3600;
                                        IRestResponse rr = client.Ticket.UpdateEstimate(key, estimate.ToString());
                                        Logger.Now.LogEvent("IssueUpdated", "STEFANINI", "UpdateEstimate", rr.IsSuccessful ? "SUCESSO" : $"FALHA: {rr.Content}");
                                        break;
                                    }
                                }
                            }
                        }
                        else if (log["fieldtype"].ToString() == "jira")
                        {
                            switch (log["field"].ToString())
                            {
                                case "Attachment": UpdateAttachments(obj); break;
                                //case "assignee": UpdateAssignee(obj); break;
                                case "status": UpdateStatus(obj); break;
                                case "description": UpdateDescription(obj); break;
                                default: break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("IssueUpdate", "STEFANINI", "Exceção gerada", e.Message);
            }
            return _r;
        }

        private void UpdateEstimate(JObject obj)
        {

        }

        private void UpdateDescription(JObject obj)
        {
            string key = obj["issue"]["fields"]["customfield_10707"].ToString();

            JArray changLogs = JsonConvert.DeserializeObject<JArray>(obj["changelog"]["items"].ToString());
            foreach (var log in changLogs)
            {
                if (log["field"].ToString() == "description")
                {
                    client.Ticket.UpdateDescription(key, log["toString"].ToString());
                }
            }
        }

        public void UpdateAssignee(JObject obj)
        {
            Logger.Now.LogEvent("UpdateAssignee", "STEFANINI", "Entrou no método");
            bool isSubTask = Convert.ToBoolean(obj["issue"]["fields"]["issuetype"]["subtask"].ToString());
            object updateIssue = new object();

            if (!isSubTask)
            {
                try
                {
                    string issueKey = obj["issue"]["key"].ToString();
                    string codigoExterno = obj["issue"]["fields"]["customfield_10707"].ToString();
                    string name = obj["issue"]["fields"]["assignee"]["name"].ToString();

                    IRestResponse r = client.Stefanini.BuscarIssuePorChave(issueKey);

                    if (r.IsSuccessful)
                    {
                        var response = client.Ticket.UpdateAssignee(codigoExterno, name);
                        Logger.Now.LogEvent("UpdateAssignee", "STEFANINI", "UpdateEndDate", response.IsSuccessful ? "SUCESSO" : $"FALHA: {response.Content}");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    Logger.Now.LogEvent("UpdateAssignee", "STEFANINI", "Exceção gerada", e.Message);
                }
            }
        }

        private void UpdateAttachments(JObject obj)
        {
            Logger.Now.LogEvent("UpdateAttachments", "STEFANINI", "Entrou no método");
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

                IRestResponse r = client.Stefanini.BuscarIssuePorChave(issueKey);

                if (r.IsSuccessful)
                {
                    var req = client.Stefanini.Buscar(url);

                    JObject stfIssue = JObject.Parse(r.Content.ToString());
                    string key = stfIssue["fields"]["customfield_10707"].ToString();

                    var response = client.Ticket.CreateAttachment(key, req.RawBytes, nome, req.ContentType);
                    Logger.Now.LogEvent("UpdateAttachments", "STEFANINI", response.IsSuccessful ? "SUCESSO" : $"FALHA: {response.Content}");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("UpdateAttachments", "STEFANINI", "Exceção gerada", e.Message);
            }
        }

        public bool UpdateStatus(JObject obj)
        {
            Logger.Now.LogEvent("UpdateStatus", "STEFANINI", "Entrou no método");
            bool isSubTask = Convert.ToBoolean(obj["issue"]["fields"]["issuetype"]["subtask"].ToString());
            bool _r = false;

            if (!isSubTask)
            {
                try
                {
                    string codigoExterno = obj["issue"]["fields"]["customfield_10707"].ToString();

                    string statusNameFormat = obj["issue"]["fields"]["status"]["name"].ToString()
                        .ToUpper()
                        .Trim()
                        .Replace(" ", "")
                        .Replace("-", "TRAÇO")
                        .Replace("/", "BARRA")
                        .Replace("+", "MAIS");

                    int statusId = (Int32)Enum.Parse(typeof(ExpandStatustTicket), statusNameFormat);

                    IRestResponse response = client.Ticket.UpdateStatus(codigoExterno, statusId);
                    _r = response.IsSuccessful;

                    Logger.Now.LogEvent("UpdateStatus", "STEFANINI", "UpdateEndDate", response.IsSuccessful ? "SUCESSO" : $"FALHA: {response.Content}" + $"DETALHES: {codigoExterno}, {statusNameFormat}, {statusId}");
                }
                catch (Exception e)
                {
                    Logger.Now.LogEvent("UpdateStatus", "STEFANINI", "Exceção gerada", e.Message);
                }
            }
            else
            {
                try
                {
                    string parentKey = obj["issue"]["fields"]["parent"]["key"].ToString();
                    IRestResponse r = client.Stefanini.BuscarIssuePorChave(parentKey);

                    JObject parentIssue = JObject.Parse(r.Content.ToString());

                    string codigoExterno = parentIssue["fields"]["customfield_10707"].ToString();

                    string phase = obj["issue"]["fields"]["customfield_10706"]["value"].ToString();

                    string search = phase.Replace(" - ", " ");

                    string jql = $"jql='parent'='{parentKey}'AND'summary'~'{search}'AND'statusCategory'!='Done'";

                    IRestResponse response = client.Ticket.BuscarIssuesComJql(jql);

                    JObject stfIssue = JObject.Parse(response.Content.ToString());

                    if (stfIssue["total"].ToString() == "0")
                    {
                        jql = $"jql='parent'='{codigoExterno}'AND'summary'~'{search}'";
                        IRestResponse resp = client.Ticket.BuscarIssuesComJql(jql);

                        JObject tktIssue = JObject.Parse(resp.Content.ToString());
                        JArray issues = JsonConvert.DeserializeObject<JArray>(tktIssue["issues"].ToString());

                        foreach (var issue in issues)
                        {
                            if (issue["fields"]["summary"].ToString().Equals(phase))
                            {
                                var irr = client.Ticket.UpdateStatus(issue["key"].ToString(), 81);
                                _r = irr.IsSuccessful;
                                Logger.Now.LogEvent("UpdateStatus", "STEFANINI", "UpdateStatus", irr.IsSuccessful ? "SUCESSO" : $"FALHA: {irr.Content}");
                                irr = client.Ticket.UpdateEndDate(issue["key"].ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
                                Logger.Now.LogEvent("UpdateStatus", "STEFANINI", "UpdateEndDate", irr.IsSuccessful ? "SUCESSO" : $"FALHA: {irr.Content}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Now.LogEvent("UpdateStatus", "STEFANINI", "Exceção gerada", e.Message);
                }                
            }

            return _r;
        }
        #endregion

        #region Atualização de worklog
        public bool WorklogCreated(JObject obj)
        {
            Logger.Now.LogEvent("WorklogCreated 0", "STEFANINI", "Entrou no método");
            object newWorklog = new object();
            bool _r = false;

            try
            {
                string user = obj["worklog"]["author"]["name"].ToString();
                string updateUser = obj["worklog"]["updateAuthor"]["name"].ToString();

                if (user != "e_integracaoJira" && updateUser != "e_integracaoJira")
                {

                    //Aqui eu recupero as informações que o evento me disponibiliza.
                    string issueId = obj["worklog"]["issueId"].ToString();
                    double timeSpentSeconds = Convert.ToDouble(obj["worklog"]["timeSpentSeconds"].ToString());

                    //Aqui eu busco a issue que o evento se refere.
                    JObject issue = JsonConvert.DeserializeObject<JObject>(client.Stefanini.BuscarIssuePorChave(issueId).Content.ToString());

                    if (issue["fields"].ToString().Contains("customfield_10706") &&
                            issue["fields"]["customfield_10706"].ToString() != null)
                    {

                        Logger.Now.LogEvent("WorklogCreated 1", "STEFANINI", $"IssueId:{issueId} | Project Phase: {issue["fields"]["customfield_10706"].ToString()}");
                        //Aqui eu recupero as informações da issue sendo:
                        //projectStage = Estágio do projeto que recebera o worklog.
                        //parentKey = Chave da issue pai, para que eu possa recuperar o "código externo".
                        //projectKey = Chave do projeto, para que eu possa executar uma instrução JQL.
                        string projectStage = issue["fields"]["customfield_10706"]["value"].ToString();
                        string parentKey = issue["fields"]["parent"]["key"].ToString();
                        string projectKey = issue["fields"]["project"]["key"].ToString();

                        //Aqui eu busco a issue pai e verifico se possui o código externo
                        JObject parentIssue = JsonConvert.DeserializeObject<JObject>(client.Stefanini.BuscarIssuePorChave(parentKey).Content.ToString());

                        if (parentIssue["fields"].ToString().Contains("customfield_10707"))
                        {
                            Logger.Now.LogEvent("WorklogCreated 2", "STEFANINI", $"Parent Key: {parentKey}, Project Key: {projectKey}, Project stage: {projectStage}");

                            string storyPointCustomField;

                            try
                            {
                                storyPointCustomField = ExpandStoryPoints.Instance.GetField(projectStage);
                            }
                            catch
                            {
                                storyPointCustomField = null;
                            }

                            int storyPoint = 0;

                            if (storyPointCustomField != null)
                            {
                                if (projectStage.Trim().ToUpper() != "DESENVOLVIMENTO")
                                {
                                    storyPoint = parentIssue["fields"].ToString().Contains(storyPointCustomField) &&
                                        parentIssue["fields"][storyPointCustomField].ToString() != "" ?
                                        int.Parse(parentIssue["fields"][storyPointCustomField].ToString()) :
                                        0;
                                }
                                else
                                {
                                    foreach (string field in storyPointCustomField.Split('+'))
                                    {
                                        storyPoint += parentIssue["fields"].ToString().Contains(field.Trim()) &&
                                            parentIssue["fields"][field.Trim()].ToString() != "" ?
                                            int.Parse(parentIssue["fields"][field.Trim()].ToString()) :
                                            0;
                                    }
                                }
                            }

                            string codigoExterno = parentIssue["fields"]["customfield_10707"].ToString();

                            //Aqui eu busco a issue referênciada pelo código externo
                            IRestResponse searchIssueReferency = client.Ticket.BuscarIssuePorChave(codigoExterno);

                            if (searchIssueReferency.IsSuccessful)
                            {
                                Logger.Now.LogEvent("WorklogCreated 3", "STEFANINI", $"Story Point: {storyPoint}, Código externo: {codigoExterno}");

                                JObject issueReferency = JsonConvert.DeserializeObject<JObject>(searchIssueReferency.Content.ToString());
                                string jql = "";

                                //Aqui eu realizo uma instrução jql
                                //Para buscar a issue cujo o estagio do projeto esta sendo referenciado
                                string search = projectStage.Replace(" - ", " ");

                                jql = $"jql=parent=\"{codigoExterno}\"and\"summary\"~\"{search}\"";
                                IRestResponse searchIssueStage = client.Ticket.BuscarIssuesComJql(jql);

                                if (searchIssueStage.IsSuccessful)
                                {
                                    JObject issuesReturn = JsonConvert.DeserializeObject<JObject>(searchIssueStage.Content.ToString());
                                    JArray arrayIssues = JsonConvert.DeserializeObject<JArray>(issuesReturn["issues"].ToString());

                                    string keySubTask = "";

                                    foreach (var item in arrayIssues)
                                    {
                                        if (item["fields"]["summary"].ToString().Equals(projectStage))
                                        {
                                            keySubTask = item["key"].ToString();
                                            break;
                                        }
                                    }

                                    Logger.Now.LogEvent("WorklogCreated 4", "STEFANINI", $"Issue Referency: {keySubTask}");

                                    if (string.IsNullOrEmpty(keySubTask))
                                    {
                                        keySubTask = CreateSubTaskForWorkLog(issueReferency, parentIssue, projectStage, codigoExterno);
                                        Logger.Now.LogEvent("WorklogCreated 7", "STEFANINI", $"Issue Referency: {keySubTask}");
                                    }

                                    //Aqui eu executo uma instrução JQL, 
                                    //para que eu possa recuperar os "timeTraking" de todas as subtask que possuem a mesma referência para o estágio do projeto.
                                    jql = $"jql=project=\"{projectKey}\"and\"parent\"=\"{parentKey}\"and\"cf[10706]\"=\"{projectStage}\"&fields=timetracking&expand=issues.renderedFields";
                                    JObject issuesForProjectStage = JsonConvert.DeserializeObject<JObject>(client.Stefanini.BuscarIssuesComJql(jql).Content.ToString());

                                    //Aqui eu converto as issues retornadas para um array
                                    JArray issues = JsonConvert.DeserializeObject<JArray>(issuesForProjectStage["issues"].ToString());

                                    int originalEstimateSeconds = 0;
                                    int remainingEstimateSeconds = 0;

                                    foreach (var item in issues)
                                    {
                                        originalEstimateSeconds += item["fields"]["timetracking"].ToString().Contains("timeSpentSeconds") ? int.Parse(item["fields"]["timetracking"]["timeSpentSeconds"].ToString()) : 0;
                                        remainingEstimateSeconds += item["fields"]["timetracking"].ToString().Contains("remainingEstimateSeconds") ? int.Parse(item["fields"]["timetracking"]["remainingEstimateSeconds"].ToString()) : 0;

                                    }

                                    Logger.Now.LogEvent("WorklogCreated 8", "STEFANINI", $"Horas logadas: {originalEstimateSeconds}, Horas remanejadas: {remainingEstimateSeconds}");

                                    double newTimeSpentSeconds = 0.0;

                                    if (storyPoint != 0 && originalEstimateSeconds != 0)
                                    {
                                        newTimeSpentSeconds = (((timeSpentSeconds / (originalEstimateSeconds + remainingEstimateSeconds)) * 100) * (storyPoint * 10) / 100) * 3600;
                                    }
                                    else
                                    {
                                        newTimeSpentSeconds = 0;
                                    }

                                    Logger.Now.LogEvent("WorklogCreated 9", "STEFANINI", $"Percentual: {newTimeSpentSeconds}");

                                    if (newTimeSpentSeconds != 0 && newTimeSpentSeconds != 0.0)
                                    {
                                        string spentConvertString = newTimeSpentSeconds.ToString();
                                        if (spentConvertString.Contains(","))
                                        {
                                            spentConvertString.Remove(',', 1);
                                        }
                                        Int64 newSpent = Int64.Parse(spentConvertString);

                                        Logger.Now.LogEvent("WorklogCreated 10", "STEFANINI", $"Percentual corrigido: {newSpent}");

                                        var response = client.Ticket.CreateWorklog(keySubTask, newSpent);

                                        Logger.Now.LogEvent("WorklogCreated 11", "STEFANINI", response.IsSuccessful ? "SUCESSO" : $"FALHA: {response.Content}");

                                        IRestResponse rr = client.Stefanini.BuscarIssuePorChave(obj["worklog"]["issueId"].ToString());

                                        JObject myIssue = JObject.Parse(rr.Content.ToString());
                                        bool isSubTask = Convert.ToBoolean(myIssue["fields"]["issuetype"]["subtask"].ToString());
                                        string prtKey = "";
                                        if (isSubTask)
                                        {
                                            prtKey = myIssue["fields"]["parent"]["key"].ToString();
                                            IRestResponse rsr = client.Stefanini.BuscarIssuePorChave(prtKey);

                                            JObject parent = JObject.Parse(rsr.Content.ToString());

                                            string codigoExt = parent["fields"]["customfield_10707"].ToString();

                                            string phase = myIssue["fields"]["customfield_10706"]["value"].ToString();

                                            string myjql = $"jql='parent'='{codigoExt}'AND'summary'~'{phase}'&fields=worklog";
                                            IRestResponse rsp = client.Ticket.BuscarIssuesComJql(myjql);

                                            JObject tktIssue = JObject.Parse(rsp.Content.ToString());

                                            if (tktIssue["issues"][0]["fields"]["worklog"]["total"].ToString() == "1")
                                            {
                                                string data = DateTime.Now.ToString("yyyy-MM-dd");
                                                IRestResponse irr = client.Ticket.UpdateStartDate(tktIssue["issues"][0]["key"].ToString(), data);
                                                _r = irr.IsSuccessful;
                                                Logger.Now.LogEvent("IssueCreated 12", "STEFANINI", "UpdateStartDate", irr.IsSuccessful ? "SUCESSO" : $"FALHA: {irr.Content}");
                                                irr = client.Ticket.UpdateStatus(tktIssue["issues"][0]["key"].ToString(), 21);
                                                Logger.Now.LogEvent("IssueCreated 13", "STEFANINI", "UpdateStatus", irr.IsSuccessful ? "SUCESSO" : $"FALHA: {irr.Content}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Now.LogEvent("WorklogCreated", "STEFANINI", "Exceção gerada", e.Message, $"DETALHES: {obj}");
            }

            return _r;
        }


        public string CreateSubTaskForWorkLog(JObject issueReferency, JObject parentIssue, string phase, string codigoExterno)
        {
            string projectKey = issueReferency["fields"]["project"]["key"].ToString();

            string startDate = "";
            string endDate = "";
            int originEstimate = 0;

            switch (phase)
            {
                case "Planning":
                    endDate = parentIssue["fields"]["customfield_11203"].ToString();
                    break;
                case "Grooming":
                    endDate = parentIssue["fields"]["customfield_11204"].ToString();
                    break;
                case "Desenvolvimento":
                    startDate = parentIssue["fields"]["customfield_11101"].ToString();
                    endDate = parentIssue["fields"]["customfield_11102"].ToString();
                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11110"].ToString());
                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11111"].ToString());
                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11112"].ToString());
                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11202"].ToString());
                    originEstimate += int.Parse(parentIssue["fields"]["customfield_11121"].ToString());
                    break;
                case "Review":
                    endDate = parentIssue["fields"]["customfield_11100"].ToString();
                    break;
                case "Review Técnica":
                    endDate = parentIssue["fields"]["customfield_11212"].ToString();
                    break;
                case "Validação em Homologação - Fábrica":
                    startDate = parentIssue["fields"]["customfield_11105"].ToString();
                    endDate = parentIssue["fields"]["customfield_11106"].ToString();
                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11123"].ToString());
                    break;
                case "Validação em Pre-Produção - Fábrica":
                    startDate = parentIssue["fields"]["customfield_11107"].ToString();
                    endDate = parentIssue["fields"]["customfield_11108"].ToString();
                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11124"].ToString());
                    break;
                case "Implantação em Produção":
                    endDate = parentIssue["fields"]["customfield_11219"].ToString();
                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11215"].ToString());
                    break;
                case "Validação em QA - Fábrica":
                    startDate = parentIssue["fields"]["customfield_11103"].ToString();
                    endDate = parentIssue["fields"]["customfield_11104"].ToString();
                    originEstimate = int.Parse(parentIssue["fields"]["customfield_11122"].ToString());
                    break;
                case "Refinamento":
                    startDate = parentIssue["fields"]["customfield_11206"].ToString();
                    endDate = parentIssue["fields"]["customfield_11207"].ToString();
                    break;
            }

            originEstimate *= 10;

            if (parentIssue["fields"].ToString().Contains("customfield_11220") &&
                parentIssue["fields"]["customfield_11220"].ToString() != "")
            {
                phase += " - Change Request";
            }

            IRestResponse resp = client.Ticket.CreateSubTask(projectKey, codigoExterno, phase, originEstimate);
            Logger.Now.LogEvent("IssueCreated 5", "STEFANINI", "CreateSubTask", resp.IsSuccessful ? "SUCESSO" : $"FALHA: {resp.Content}");

            string response = "";

            if (resp.IsSuccessful)
            {
                JObject respIssue = JObject.Parse(resp.Content.ToString());

                response = respIssue["key"].ToString();

                IRestResponse rr = client.Ticket.UpdateSubtaskDate(respIssue["key"].ToString(), startDate, endDate);
                Logger.Now.LogEvent("IssueCreated 6", "STEFANINI", "UpdateSubtaskDate", rr.IsSuccessful ? "SUCESSO" : $"FALHA: {rr.Content}");
            }

            return response;
        }
        #endregion
    }
}
