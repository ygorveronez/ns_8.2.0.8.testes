using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog
{
    public class RequestPosicoes
    {
        public ActionRequestPosicoes action { get; set; }
    }

    public class ActionRequestPosicoes
    {
        public string name { get; set; }
        public List<ParameterRequestPosicoes> parameters { get; set; }
        public string session_token { get; set; }
    }

    public class ParameterRequestPosicoes
    {
        public string last_time { get; set; }
        public string license_nmbr { get; set; }
        public string group_id { get; set; }
        public string version { get; set; }
    }

}
