using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog
{
    public class ResponsePosicoes
    {
        public RespPosicao response { get; set; }
    }

    public class RespPosicao
    {
        public PropertiesResponsePosicao properties { get; set; }
    }

    public class PropertiesResponsePosicao
    {
        public string action_name { get; set; }
        public List<posicao> data { get; set; }
        public string action_value { get; set; }
        public string description { get; set; }
        public string session_token { get; set; }
    }

    public class posicao
    {
        public string vehicle_id { get; set; }
        public string unit_id { get; set; }
        public string group_id { get; set; }
        public string group_name { get; set; }
        public string client_id { get; set; }
        public string client_name { get; set; }
        public string unit_serial { get; set; }
        public string license_nmbr { get; set; }
        public string chassis_number { get; set; }
        public string last_communication_time { get; set; }
        public string last_position_time { get; set; }
        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string speed { get; set; }
        public string direction { get; set; }
        public string status { get; set; }
        public string last_event_time { get; set; }
        public string last_event_type { get; set; }
        public string current_driver { get; set; }
        public string current_driver_number { get; set; }
        public string driver_name { get; set; }
        public string worker_id { get; set; }
        public string current_drive { get; set; }
        public string last_mileage { get; set; }
    }

}
