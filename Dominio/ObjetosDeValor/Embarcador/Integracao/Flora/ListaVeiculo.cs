using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class ListaVeiculo
    {

        [JsonProperty("Veiculo")]
        public Veiculo ListaVeiculos { get; set; }
    }
}
