using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC
{
    public class EnvioNFSeRetorno
    {
        [JsonProperty("envia_nfse_resp")]
        public List<DocumentoNFSeRetorno> DocumentosNFSeRetorno { get; set; }
    }
}