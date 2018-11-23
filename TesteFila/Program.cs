using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Messaging;
using JiraMensageria.Controller;
using System.IO;
using JiraMensageria.Helpers;
using System.Net.NetworkInformation;

namespace TesteFila
{
    class Program
    {
        public static void Ping(string url)
        {
            Uri uri = new Uri(url);
            Ping ping = new Ping();
            ping.Send(uri.Host);           
        }

        static void Main(string[] args)
        {
            string result = "";
            string jira = "";
            string evento = "";
            bool r = false;
            string uriTkt = ConfigurationManager.AppSettings["ticket"].ToString();
            string uriStf = ConfigurationManager.AppSettings["stefanini"].ToString();

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
                            case "commentcreated"   : r = tkt.CommentCreated(jObj); break;
                            case "newissue"         : r = tkt.NewIssue(jObj); break;
                            case "issueupdated"     : r = tkt.IssueUpdated(jObj); break;
                            case "issuecreated"     : r = tkt.IssueCreate(jObj, "projetctKey", "ticket"); break;
                            default: break;
                        }

                        try
                        {
                            Ping(uriTkt);
                        }
                        catch (Exception ex)
                        {
                            Logger.Now.LogEvent(evento, jira, String.Format("NÃO FOI POSSIVEL CONECTAR AO SERVIDOR JIRA  - {0}", ex.Message));
                        }
                    }
                    //STEFANINI
                    else if (jira == "stefanini")
                    {
                        switch (evento)
                        {
                            case "worklogcreated"   : r = stf.WorklogCreated(jObj); break;
                            case "issueupdated"     : r = stf.IssueUpdated(jObj); break;
                            case "updatestatus"     : r = stf.UpdateStatus(jObj); break;
                            default: break;                       
                        }

                        if (r)
                        {
                            msmq.Receive();
                        }
                        else
                        {
                            try
                            {
                                Ping(uriStf);
                            }
                            catch (Exception ex)
                            {
                                Logger.Now.LogEvent(evento, jira, String.Format("NÃO FOI POSSIVEL CONECTAR AO SERVIDOR JIRA  - {0}",ex.Message));
                            }
                            
                        }
                    }                 

                }
                catch (Exception ex)
                {
                    Logger.Now.LogEvent(evento, jira, ex.Message);
                }
            }



        }

    }
}
