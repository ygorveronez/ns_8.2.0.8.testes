using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class VeiculoAtrelado
    {
        /// <summary>
        /// Codigo Veiculo Integração
        /// </summary>
        [JsonProperty("CdVeiculoAtrelado")]
        public string CodigoIntegracaoVeiculoAtrelado { get; set; }
    }
}
