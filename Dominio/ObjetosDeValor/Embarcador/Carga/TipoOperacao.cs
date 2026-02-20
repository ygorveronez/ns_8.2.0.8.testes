using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class TipoOperacao
    {
        public int Protocolo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public List<string> CNPJsDaOperacao { get; set; }

        public bool BloquearEmissaoDosDestinatario { get; set; }
        public bool BloquearEmissaoDeEntidadeSemCadastro { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalPropostaMultimodal { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal TipoServicoMultimodal { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        public List<string> CNPJsDestinatariosNaoAutorizados { get; set; }
        public bool Atualizar { get; set; }
        public int Codigo { get; set; }
    }
}
