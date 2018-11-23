using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JiraMensageria.Helpers
{
    public class JiraClient
    {
        public Jira Stefanini   { get; set; } = new Jira(ConfigurationManager.AppSettings["stefanini"], "e_integracaoJira", "@7px&hBc");
        public Jira Ticket      { get; set; } = new Jira(ConfigurationManager.AppSettings["ticket"], "appjirastefanini", "J!r@edenred");
    }

    public class Jira
    {
        private readonly RestClient client;
        private readonly string log;


        public Jira(string url, string user, string pass)
        {
            client = new RestClient(new Uri(url))
            {
                Authenticator = new HttpBasicAuthenticator(user, pass)
            };
            log = (url != "http://172.29.95.62" ? "STEFANINI" : "TICKET");
        }

        public IRestResponse Buscar(string url)
        {
            IRestResponse response = client.Execute(new RestRequest(url, Method.GET)
            {
                RequestFormat = DataFormat.Json
            });

            return response;
        }

        public IRestResponse BuscarPorCustomField(string issueKey, string customFieldId)
        {

            return BuscarIssuesComJql($"jql='cf[{customFieldId}]'~'{issueKey}'");
        }

        public IRestResponse BuscarIssuePorChave(string issueKey)
        {
            IRestResponse response = client.Execute(new RestRequest($"rest/api/2/issue/{issueKey}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            });

            
            return response;

        }

        public IRestResponse BuscarIssuesComJql(string jql)
        {
            IRestResponse response = client.Execute(new RestRequest($"rest/api/2/search?{jql}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            });

            return response;
        }


        public IRestResponse CreateComment(string issueKeyOrId, string comment)
        {
            IRestRequest req = new RestRequest($@"/rest/api/2/issue/{issueKeyOrId}/comment", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            req.AddBody(new
            {
                body = comment,
                visibility = new { }
            });
            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse UpdateStatus(string issueKeyOrId, int statusId)
        {
            IRestRequest req = new RestRequest($"rest/api/2/issue/{issueKeyOrId}/transitions", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                transition = new
                {
                    id = statusId
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse CreateIssue(string projectKey, string summary, int issueType, string priority, string description,
                                         string customfield_10707, string customfield_10708, string origem)
        {
            object newIssue = new
            {
                fields = new
                {
                    project = new
                    {
                        key = projectKey
                    },
                    summary,
                    issuetype = new
                    {
                        id = issueType,
                    },
                    priority = new
                    {
                        id = priority
                    },
                    description,
                    customfield_10707,
                    customfield_10708,
                    customfield_10705 = new
                    {
                        value = origem
                    },
                    customfield_11214 = new
                    {
                        id = "10841"
                    }
                }
            };

            IRestRequest req = new RestRequest("rest/api/2/issue", Method.POST)
            {
                RequestFormat = DataFormat.Json
            }.AddBody(newIssue);

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse CreateSubTask(string projectKey, string parentKey, string summary)
        {
            object newSubTask = new
            {
                fields = new
                {
                    project = new
                    {
                        key = projectKey
                    },
                    parent = new
                    {
                        key = parentKey
                    },
                    summary,
                    issuetype = new
                    {
                        id = "10502"
                    }
                }
            };

            IRestResponse response = client.Execute(new RestRequest("rest/api/2/issue", Method.POST)
            {
                RequestFormat = DataFormat.Json
            }.AddBody(newSubTask));

            return response;
        }


        public IRestResponse CreateSubTask(string projectKey, string parentKey, string summary, int time)
        {
            object newSubTask = new
            {
                fields = new
                {
                    project = new
                    {
                        key = projectKey
                    },
                    parent = new
                    {
                        key = parentKey
                    },
                    summary,
                    issuetype = new
                    {
                        id = "10502"
                    },
                    timetracking = new
                    {
                        originalEstimate = time
                    }
                }
            };

            Buscar("rest/auth/latest/session");
            IRestResponse response = client.Execute(new RestRequest("rest/api/2/issue", Method.POST)
            {
                RequestFormat = DataFormat.Json
            }.AddBody(newSubTask));

            return response;
        }

        public IRestResponse CreateAttachment(string issueKeyOrId, byte[] rawBytes, string fileName, string contentType)
        {
            IRestRequest req = new RestRequest($"/rest/api/2/issue/{issueKeyOrId}/attachments", Method.POST);
            req.AddHeader("X-Atlassian-Token", "nocheck").AddFile("file", rawBytes, fileName, contentType);
            IRestResponse response = client.Execute(req);

            if (response.IsSuccessful)
            {
                Logger.Now.LogEvent("CreateAttachment", log, issueKeyOrId, "SUCESSO");
            }
            else
            {
                Logger.Now.LogEvent("CreateAttachment", log, "FALHA", issueKeyOrId, response.Content);
            }

            return response;
        }

        public IRestResponse UpdateAssignee(string issueKeyOrId, string nome)
        {
            IRestRequest req = new RestRequest($"/rest/api/2/issue/{issueKeyOrId}", Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                fields = new
                {
                    customfield_10219 = new
                    {
                        nome
                    }
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse UpdateEstimate(string issueKeyOrId, string value)
        {
            IRestRequest req = new RestRequest($"/rest/api/2/issue/{issueKeyOrId}/", Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };
            int num = int.Parse(value) / 60 / 60;
            req.AddBody(new
            {
                fields = new
                {
                    timetracking = new
                    {
                        originalEstimate = $"{num.ToString()}h"
                    }
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse CreateWorklog(string issueKeyOrId, double timeSpentSeconds)
        {
            IRestRequest req = new RestRequest($"rest/api/2/issue/{issueKeyOrId}/worklog", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                timeSpentSeconds = (int)(timeSpentSeconds)
            });
            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse UpdateEndDate(string issueKeyOrId, string value)
        {
            IRestRequest req = new RestRequest($"rest/api/2/issue/{issueKeyOrId}/", Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                fields = new
                {
                    customfield_11910 = value
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse UpdateStartDate(string issueKeyOrId, string value)
        {
            IRestRequest req = new RestRequest($"rest/api/2/issue/{issueKeyOrId}/", Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                fields = new
                {
                    customfield_11909 = value
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse UpdateDescription(string issueKeyOrId, string value)
        {
            IRestRequest req = new RestRequest($"rest/api/2/issue/{issueKeyOrId}/", Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                fields = new
                {
                    description = value
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse UpdateStoryPoint(string issueKeyOrId, string value)
        {
            IRestRequest req = new RestRequest($"rest/api/2/issue/{issueKeyOrId}/", Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                fields = new
                {
                    customfield_10006 = int.Parse(value)
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse UpdateSubtaskDate(string issueIdOrKey, string startDate, string endDate)
        {
            IRestRequest req = new RestRequest($"/rest/api/2/issue/{issueIdOrKey}", Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            req.AddBody(new
            {
                fields = new
                {
                    customfield_11907 = startDate == "" ? null : startDate,
                    customfield_11908 = endDate == "" ? null : endDate
                }
            });

            IRestResponse response = client.Execute(req);

            return response;
        }

        public IRestResponse BuscarTransitions(string issueKeyOrId)
        {
            IRestRequest req = new RestRequest($"rest/api/2/issue/{issueKeyOrId}/transitions", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            IRestResponse response = client.Execute(req);

            return response;
        }
    }
}