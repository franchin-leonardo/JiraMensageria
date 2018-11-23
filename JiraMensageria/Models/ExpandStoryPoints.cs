using System.Collections.Generic;
using System.Linq;

namespace JiraMensageria.Models
{
    public class ExpandStoryPoints
    {
        private readonly Dictionary<string, string> Values = new Dictionary<string, string>();
        private static ExpandStoryPoints _instance;

        public static ExpandStoryPoints Instance { get { if (_instance == null) _instance = new ExpandStoryPoints(); return _instance; } }

        private ExpandStoryPoints()
        {
            Values.Add("Ticket Story Points Homologation Tests", "customfield_11123");
            Values.Add("Desenvolvimento", "customfield_11113 " +
                "+ customfield_11110 + customfield_11111 + customfield_11112 + customfield_11202 + customfield_11121");
            Values.Add("Validação em Homologação - Fábrica", "customfield_11123");
            Values.Add("Validação em QA - Fábrica", "customfield_11122");
            Values.Add("Validação em Pre-Produção - Fábrica", "customfield_11124");
            Values.Add("Implantação em Produção", "customfield_11215");
        }

        public string GetField(string statusProject)
        {
            return Values[statusProject];
        }
    }
}