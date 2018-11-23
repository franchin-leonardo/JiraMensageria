using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JiraMensageria.Helpers
{
    public class Logger
    {
        private static Logger _instance;

        public static Logger Now
        {
            get
            {
                if (_instance == null)
                    _instance = new Logger();
                return _instance;
            }
        }
        private Logger() { }

        public void NewLog(string type, JObject objReceived, JObject objResponse, JObject responseServer)
        {
            var array = new JArray();
            string path = "";

            if (type.Equals("event"))
            {
                path = Path.Combine($@"C:\logs", "logEvent.json");
            }
            else
            {
                path = Path.Combine($@"C:\logs", "logError.json");
            }

            using (StreamReader read = new StreamReader(path))
            {
                array = JArray.Parse(read.ReadToEnd());
            }

            object newLog = new
            {
                data = DateTime.Now.ToLongDateString(),
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

        public void LogEvent(string type, string baseHost, params string[] args)
        {
            using (StreamWriter writer = File.AppendText(@"C:\logs\logs.log"))
            {
                writer.Write("{0} {1} => LOCAL: {2} | ORIGIN: {3} | CONTENT: ",
                                 DateTime.Now.ToLongDateString(),
                                 DateTime.Now.ToLongTimeString(),
                                 type, baseHost);
                foreach (var item in args)
                {
                    writer.Write($"{item} | ");
                }
                writer.WriteLine();
            }
        }
    }
}