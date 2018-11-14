using ApiMessageRest.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Messaging;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ApiMessageRest.Controllers
{
    public class StefaniniController : ApiController
    {
        private string CaminhoFilaBase => ConfigurationManager.AppSettings["caminhoFila"];
      
        [HttpPost]
        [Route("stefanini/issue/updated")]
        public void IssueUpdated(JObject obj)
        {
            AddMessageQueue("stefanini/issueupdated", obj);
        }

        [HttpPut]
        [Route("stefanini/status/update")]
        public void UpdateStatus(JObject obj)
        {
            AddMessageQueue("stefanini/updatestatus", obj);
        }

        [HttpPost]
        [Route("stefanini/worklog/created")]
        public void WorklogCreated(JObject obj)
        {
            AddMessageQueue("stefanini/worklogcreated", obj);
        }

        private void AddMessageQueue(string route, JObject jObj)
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
                        msmq.SetPermissions("Todos",MessageQueueAccessRights.FullControl,AccessControlEntryType.Allow);
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
