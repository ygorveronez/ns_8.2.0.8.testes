using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class MontarCarga
    {
        [JsonProperty(PropertyName = "auth", Required = Required.Default)]
        public MontarCargaAutenticacao Autenticacao { get; set; }

        [JsonProperty(PropertyName = "parameters", Required = Required.Default)]
        public MontarCargaParametros Parametros { get; set; }

        [JsonProperty(PropertyName = "objetos", Required = Required.Default)]
        public List<MontarCargaObjeto> Pedidos { get; set; }
    }
}
