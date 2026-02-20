using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class OrdemEmbarqueHistoricoIntegracao
    {
        public int CodigoOrdemEmbarque { get; set; }

        public int CodigoPedidoAdicionado { get; set; }

        public int CodigoPedidoRemovido { get; set; }

        public int CodigoUsuario { get; set; }

        public string JsonRequisicao { get; set; }

        public string JsonRetorno { get; set; }

        public string ProblemaIntegracao { get; set; }

        public SituacaoIntegracao SituacaoIntegracao { get; set; }

        public TipoArquivoIntegracaoCTeCarga TipoArquivoIntegracao { get; set; }

        public TipoOrdemEmbarqueHistoricoIntegracao TipoHistoricoIntegracao { get; set; }
    }
}
