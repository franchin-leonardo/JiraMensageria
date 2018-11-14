using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraMensageria.Controller
{
    public static class LogController
    {
       
        public static bool LogMensageria(string error, string evento, string jira)
        {
            string caminhoLog = ConfigurationManager.AppSettings["caminhoLog"].ToString();

            if (CreateDir(caminhoLog))
            {
                caminhoLog = @"\log" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "-") + ".txt";

                if (!File.Exists(caminhoLog))
                {
                    File.Create(caminhoLog).Dispose();

                    using (TextWriter tw = new StreamWriter(caminhoLog))
                    {
                        tw.WriteLine(String.Format("Erro mengareria : {0} - Evento : {1} - Jira : {2}", error, evento, jira));
                    }
                }
            }

            return false;
        }

        private static bool CreateDir(string caminhoLog)
        {
            
            if (!System.IO.Directory.Exists(caminhoLog))
            {
                System.IO.Directory.CreateDirectory(caminhoLog);
                return true;
            }

            return false;
        }
    }
}
