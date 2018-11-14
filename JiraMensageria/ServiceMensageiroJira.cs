using JiraMensageria.Controller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Messaging;
using System.ServiceProcess;
using System.Threading;

namespace JiraMensageria
{
    public partial class ServiceMensageiroJira : ServiceBase
    {
        private Thread thread;

        public ServiceMensageiroJira()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
         
            thread = new Thread(ExecutaFilaJira)
            {
                Name = "Execute Thread of Messaging Service",
                IsBackground = true
            };
            thread.Start();
            

        }

        protected override void OnStop()
        {
            if (!thread.Join(3000))
                thread.Abort();
        }

        private void ExecutaFilaJira()
        {
            string result = "";
            string jira = "";
            string evento = "";

            StefaniniController stf = new StefaniniController();
            TicketController tkt = new TicketController();

            try
            {
                MessageQueue msmq = new MessageQueue(ConfigurationManager.AppSettings["caminhoFila"].ToString());
                msmq.MessageReadPropertyFilter.Priority = true;
                msmq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                //int count = msmq.GetAllMessages().Length;

                while (msmq.CanRead)
                {
                    Message item = msmq.Receive();
                    result = item.Body.ToString();

                    string[] route = item.Label.Split('/');

                    if (route.Length > 0)
                    {
                        jira = route[0];
                        evento = route[1];
                    }

                    var jObj = JObject.Parse(result);

                    //TICKET
                    if (jira == "ticket")
                    {
                        if (evento == "commentcreated")
                        {
                            tkt.CommentCreated(jObj);
                        }
                        else if (evento == "newissue")
                        {
                            tkt.NewIssue(jObj);
                        }
                        else if (evento == "issueupdated")
                        {
                            tkt.IssueUpdated(jObj);
                        }
                        else if (evento == "issuecreated")
                        {
                            tkt.IssueCreate(jObj);
                        }
                    }
                    //STEFANINI
                    else if (jira == "stefanini")
                    {
                        if (evento == "worklogcreated")
                        {
                            stf.WorklogCreated(jObj);
                        }
                        else if (evento == "issueupdated")
                        {
                            stf.IssueUpdated(jObj);
                        }
                        else if (evento == "updatestatus")
                        {
                            stf.UpdateStatus(jObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogController.LogMensageria(ex.Message, evento, jira);
            }


        }
    }
}
