namespace JiraMensageria.Models
{
    public enum ExpandStatustTicket
    {
        BACKLOG = 61,
        REFINAMENTO = 191,
        AGUARDANDOINCEPTION = 21,
        AGUARDANDOGROOMING = 31,
        AGUARDANDOAPROVAÇÃO = 31,
        COMITEPRIORIZADO = 31,
        PLANNING = 41,
        AGUARDANDOPLANNING = 51,
        DESENVOLVIMENTO = 201,
        REVIEW = 131,
        AGUARDANDOPUBLICAÇÃOQA = 211,
        TESTEQA = 221,
        AGUARDANDOVOBOQA = 221,
        //AGUARDANDOVOBOHOMOLOG = 251,
        PUBLICAÇÃOHOMOLOGAÇÃO = 251,
        AGUARDANDOPUBLICAÇÃOHOMOLOG = 251,
        TESTEHOMOLOG = 231,
        VALIDAÇÃOHOMOLOGAÇÃO = 231,
        AGUARDANDOPUBLICAÇÃOPRÉTRAÇOPROD = 241,
        TESTEPRÉTRAÇOPROD = 241,
        FINALIZADO = 261,
        CANCELED = 271,
    };

    public enum ExpandStatusStefanini
    {
        BACKLOG = 171,
        REFINING = 161,
        INCEPTION = 281,
        GROOMING = 191,
        PLANNING = 41,
        SELECTEDFORDEVELOPMENT = 51,
        REVIEW = 61,
        PUBLICATIONINQA = 81,
        TESTSBARRAQA = 71,
        PUBLICATIONINHOMOLOGATION = 101,
        HOMOLOGATION = 111,
        PRETRAÇOPRODUCTION = 121,
        DONE = 231,
        CANCELED = 201
    }
}