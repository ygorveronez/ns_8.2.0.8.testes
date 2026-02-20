using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Carga
{
    public sealed class TipoOperacao
    {
        public string CodigoIntegracao { get; set; }

        public string Descricao { get; set; }

        public List<string> CNPJsDaOperacao { get; set; }

        public bool BloquearEmissaoDosDestinatario { get; set; }

        public bool BloquearEmissaoDeEntidadeSemCadastro { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalPropostaMultimodal { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal TipoServicoMultimodal { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        public List<string> CNPJsDestinatariosNaoAutorizados { get; set; }

        public bool Atualizar { get; set; }
    }
}
