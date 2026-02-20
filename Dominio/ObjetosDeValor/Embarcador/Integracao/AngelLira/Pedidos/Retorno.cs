using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos
{
    public class DataRetorno
    {
        public Data data { get; set; }
    }
    public class Error
    {
        public string ownId { get; set; }
        public string message { get; set; }
    }

    public class sucess
    {
        public string ownId { get; set; }
        public string id { get; set; }
    }

    public class Data
    {
        public List<sucess> success { get; set; }
        public List<Error> error { get; set; }
    }


    public class Step
    {
        public int id { get; set; }
        public string desc { get; set; }
    }

    public class RetornoConsultaStatus
    {
        public string ownId { get; set; }
        public string message { get; set; }
        public int id { get; set; }
        public Step step { get; set; }
    }
}
