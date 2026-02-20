namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGeracaoCancelamentoAutomatico
    {
        NaoInformado = 0,        
        PendenteGerarDesbloqueioTitulo = 1,
        DesbloqueioTituloGerado = 2
    }

    public static class SituacaoGeracaoCancelamentoAutomaticoHelper
    {
        public static string ObterDescricao(this SituacaoGeracaoCancelamentoAutomatico modo)
        {
            switch (modo)
            {
                case SituacaoGeracaoCancelamentoAutomatico.NaoInformado: return "Não Informado";
                case SituacaoGeracaoCancelamentoAutomatico.PendenteGerarDesbloqueioTitulo: return "Gerar Desbloqueio de Título";
                case SituacaoGeracaoCancelamentoAutomatico.DesbloqueioTituloGerado: return "Desbloqueio de Título Gerado";
                default: return "";
            }
        }
    }
}
