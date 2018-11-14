using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraMensageria.Models
{
    public enum ExpandStatustTicket
    {

        CANCELADO = 271,
        BACKLOG = 61,
        AGUARDANDOPLANNING = 51,
        RELEASE = 261,
        TESTEHOMOLOG = 231,
        REFINAMENTO = 191,
        DESENVOLVIMENTO = 51,
        AGUARDANDOGROOMING = 31,
        AGUARDANDOINCEPTION = 21,
        AGUARDANDOAPROVAÇÃO = 31,
        COMITEPRIORIZADO = 31,
        PLANNING = 41,
        AGUARDANDOPUBLICAÇÃOPRÉTRAÇOPROD = 241,
        TESTEPRÉTRAÇOPROD = 241,
        AGUARDANDOPUBLICAÇÃOHOMOLOG = 251,
        AGUARDANDOPUBLICAÇÃOQA = 211,
        REVIEW = 131,
        TESTEQA = 221,
        AGUARDANDOVOBOQA = 221,
    };

    public enum ExpandStatusStefanini
    {
        BACKLOG = 10404
    }

    public enum ExpandIssues
    {
        HISTÓRIA = 10001,
        STORY = 10001,
        BUG = 10005,
        PROBLEMA = 10005,
        BLOCK = 10007,
        DAILYANDALIGNMENTS = 10008,
        RITUALS = 10010,
        SUBTRAÇOECOMERCIALD = 10025,
        SUBTRAÇOREQUIREMENT = 10021,
        SUBTRAÇOIMP = 10023,
        SUBTRAÇOATIVIDADE = 10023,
        SUBTRAÇOTEST = 10024,
        SUBTRAÇOBLOCK = 10027,
        SUBTRAÇODAILYANDALIGNMENTS = 10028,
        SUBTRAÇOBUG = 10026,
        SUBTRAÇOVALUEACTIVATION = 10019,
        OVERHEADSTEFANINI = 10018,
        TRAINING = 10015,
        IDLE = 10009,
        SUBTRAÇOCONTROLPOINT = 10601,
        TASK = 10029,
        TAREFA = 10029,
        SUBTRAÇOTASK = 10030,
        SUBTRAÇOTAREGA = 10030,
        CHANGEREQUEST = 11304,
    }

    public class GroupTypes
    {
        public const string MÉXICO = "TWMEX";
        public const string BRASIL = "TWBRA";
        public const string CHILE = "TWCHI";
    }
}
