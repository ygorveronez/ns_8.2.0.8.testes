using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog
{
    public class ResponseToken
    {
        public Response response { get; set; }
    }

    public class Datum
    {
        public string session_token { get; set; }
        public string profile_name { get; set; }
        public string user_language { get; set; }
        public string application_url { get; set; }
        public string map_type { get; set; }
    }

    public class Properties
    {
        public string action_name { get; set; }
        public List<Datum> data { get; set; }
        public string action_value { get; set; }
        public string description { get; set; }
        public string session_token { get; set; }
    }

    public class Response
    {
        public Properties properties { get; set; }
    }

}
