namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoComprovei
    {
        AgendamentoEntrega = 1,
        BaixaDocumentos = 2
    }

    public static class TipoEnvioIntegracaoComproveiHelper
    {
        public static string ObterDescricao(this TipoIntegracaoComprovei tipoIntegracao)
        {
            switch (tipoIntegracao)
            {
                case TipoIntegracaoComprovei.AgendamentoEntrega: return "Agendamento de Entrega";
                case TipoIntegracaoComprovei.BaixaDocumentos: return "Baixa de Documentos";
                default: return string.Empty;
            }
        }
    }
}