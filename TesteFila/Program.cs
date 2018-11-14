using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Messaging;
using JiraMensageria.Controller;
using System.IO;

namespace TesteFila
{
    class Program
    {
        static void Main(string[] args)
        {
            string result = "";
            string jira = "";
            string evento = "";

            StefaniniController stf = new StefaniniController();
            TicketController tkt = new TicketController();

            Console.WriteLine("1");

            try
            {
                Console.WriteLine("2");
                MessageQueue msmq = new MessageQueue(ConfigurationManager.AppSettings["caminhoFila"].ToString());
                msmq.MessageReadPropertyFilter.Priority = true;
                msmq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                //int count = msmq.GetAllMessages().Length;

                Console.WriteLine("3");

                while (msmq.CanRead)
                {

                    Console.WriteLine("4");
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
                        Console.WriteLine("5");
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

                Console.WriteLine("6");
            }
            catch (Exception ex)
            {

                Console.WriteLine("7");
                LogController.LogMensageria(ex.Message, evento, jira);
            }

            Console.WriteLine("8");

            Console.ReadKey();
        }     

    }
}
