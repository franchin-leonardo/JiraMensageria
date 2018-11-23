using JiraMensageria.Controller;
using JiraMensageria.Helpers;
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

            MessageQueue msmq = new MessageQueue(ConfigurationManager.AppSettings["caminhoFila"].ToString());
            msmq.MessageReadPropertyFilter.Priority = true;
            msmq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });                       

            while (msmq.CanRead)
            {
                try
                {
                    Message item = msmq.Peek();
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
                        switch (evento)
                        {
                            //case "commentcreated"   : tkt.CommentCreated(jObj); break;
                            case "newissue"         : tkt.NewIssue(jObj); break;
                            case "issueupdated"     : tkt.IssueUpdated(jObj); break;
                            case "issuecreated"     : tkt.IssueCreate(jObj, "projetctKey", "ticket"); break;
                            default: break;
                        }                      
                    }
                    //STEFANINI
                    else if (jira == "stefanini")
                    {
                        switch (evento)
                        {
                            case "worklogcreated"   : stf.WorklogCreated(jObj); break;
                           // case "issueupdated"     : stf.IssueUpdated(jObj); break;
                            case "updatestatus"     : stf.UpdateStatus(jObj); break;                           
                            default: break;
                        }                      
                    }

                    msmq.ReceiveById(item.Id);
                }
                catch (Exception ex)
                {
                    Logger.Now.LogEvent(evento, jira, ex.Message);
                }
            }
           


        }
    }
}
