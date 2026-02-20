using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class RetornoContratacaoVeiculo
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("sucesso")]
        public string Sucesso { get; set; }

        [JsonProperty("codigo_retorno")]
        public int CodigoRetorno { get; set; }

        [JsonProperty("mensagem_retorno")]
        public string Mensagem { get; set; }

    }
}
