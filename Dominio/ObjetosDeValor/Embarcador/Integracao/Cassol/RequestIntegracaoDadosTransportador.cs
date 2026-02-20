using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol
{
    public class RequestIntegracaoDadosTransportador
    {
        [JsonProperty("id")]
        public int NumeroCarga { get; set; }       
              
        [JsonProperty("codveic_caminhao")]
        public int CodigoVeiculo { get; set; }

        [JsonProperty("cod_motorista")]
        public long CodigoMotorista { get; set; }

        [JsonProperty("fl_conferidobox")]
        public string BoxConferido { get; set; }
    }
}
