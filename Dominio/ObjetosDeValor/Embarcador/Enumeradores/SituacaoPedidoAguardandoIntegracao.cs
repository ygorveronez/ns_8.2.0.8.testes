namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPedidoAguardandoIntegracao
    {
        AgIntegracao = 0,
        Integrado = 1,
        ProblemaIntegracao = 2,
        AgRetorno = 3,
        AgGerarCarga = 4,
        ProblemaGerarCarga = 5,
        ErroGenerico = 6
    }

    public static class SituacaoPedidoAguardandoIntegracaoHelper
    {
        public static string ObterDescricao(this SituacaoPedidoAguardandoIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoPedidoAguardandoIntegracao.AgIntegracao: return "Aguardando Integração";
                case SituacaoPedidoAguardandoIntegracao.AgRetorno: return "Aguardando Retorno";
                case SituacaoPedidoAguardandoIntegracao.Integrado: return "Integrado";
                case SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao: return "Falha ao Integrar";
                case SituacaoPedidoAguardandoIntegracao.AgGerarCarga: return "Aguardando Gerar Carga";
                case SituacaoPedidoAguardandoIntegracao.ProblemaGerarCarga: return "Problema Geração de Carga";
                case SituacaoPedidoAguardandoIntegracao.ErroGenerico: return "Erro Generico";
                default: return string.Empty;
            }
        }
    }
}
