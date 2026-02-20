using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Transportadora
    {
        [JsonProperty("documents")]
        public List<Documento> Documentos { get; set; }
    }

}
