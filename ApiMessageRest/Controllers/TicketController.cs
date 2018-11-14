using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiMessageRest.Controllers
{
    public class TicketController : ApiController
    {
        private string CaminhoFilaBase => ConfigurationManager.AppSettings["caminhoFila"];
  
        [HttpPost]
        [Route("ticket/comment/created")]
        public void CommentCreated(JObject obj)
        {
            AddMessageQueue("ticket/commentcreated", obj);
        }

        [HttpPost]
        [Route("ticket/issue/created")]
        public void NewIssue(JObject obj)
        {
            AddMessageQueue("ticket/newissue", obj);
        }

        [HttpPost]
        [Route("ticket/issue/updated")]
        public void IssueUpdated(JObject obj)
        {
            AddMessageQueue("ticket/issueupdated", obj);
        }

        private void AddMessageQueue(string route,JObject jObj)
        {
            using (MessageQueueTransaction mqt = new MessageQueueTransaction())
            {
                try
                {
                    mqt.Begin();
                    MessageQueue msmq;

                    if (!MessageQueue.Exists(CaminhoFilaBase))
                    {
                        MessageQueue.Create(CaminhoFilaBase, true);                      
                        msmq = new MessageQueue(CaminhoFilaBase);
                        msmq.SetPermissions("Todos", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
                    }
                    else
                    {
                        msmq = new MessageQueue(CaminhoFilaBase);
                    }

                    if (jObj != null)
                    {
                        Message msg = new Message();
                        msmq.Formatter = new XmlMessageFormatter(new Type[] { typeof(object) });
                        msg.Body = jObj.ToString();
                        msmq.Send(msg, route, mqt);
                        mqt.Commit();
                    }
                    else
                        mqt.Abort();
                }
                catch (Exception ex)
                {
                    mqt.Abort();
                }
                finally
                {
                    mqt.Dispose();
                }
            }
        }
    }
}
