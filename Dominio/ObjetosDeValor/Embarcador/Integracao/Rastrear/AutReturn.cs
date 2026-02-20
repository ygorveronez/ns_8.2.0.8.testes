using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear
{

    public class TokenInfo
    {
        public bool authenticated { get; set; }
        public string created { get; set; }
        public string expiration { get; set; }
        public string accessToken { get; set; }
        public string message { get; set; }
    }

    public class AutReturn
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<string> validationErrors { get; set; }
        public int cdOP { get; set; }
        public TokenInfo tokenInfo { get; set; }
    }

    public class Aut
    {
        public string usuario { get; set; }
        public string senha { get; set; }
    }


    public class Posicao
    {
        public int cdViag { get; set; }
        public string nrPlacaCavalo { get; set; }
        public DateTime dtPos { get; set; }
        public string vlLat { get; set; }
        public string vlLong { get; set; }
        public DateTime dtReceb { get; set; }
        public bool flTpPos { get; set; }
        public bool flIgnicao { get; set; }
        public int vlVelocidade { get; set; }
        public string flSensorPortaCab { get; set; }
        public string flSensorPortaBau { get; set; }
        public string flSensorDesengCarreta { get; set; }
        public string flBloqueado { get; set; }
        public string flSireneLed { get; set; }
        public string flQuintaRoda { get; set; }
        public string flSensorPanico { get; set; }
        public string flTravaBau { get; set; }
        public string flAlerta { get; set; }
    }

    public class ResponsePosicoes
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<string> validationErrors { get; set; }
        public List<Posicao> posicoes { get; set; }
    }


}
