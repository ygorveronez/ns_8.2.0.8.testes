namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracaoCargaEDI
    {
        Todas = 0,
        AgIntegracao = 1,
        Integrado = 2,
        Falha = 3 
    }

    public static class SituacaoIntegracaoCargaEDIHelper
    {
        public static string ObterDescricao(this SituacaoIntegracaoCargaEDI situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracaoCargaEDI.AgIntegracao: return Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.AguardandoIntegracao;
                case SituacaoIntegracaoCargaEDI.Integrado: return Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                case SituacaoIntegracaoCargaEDI.Falha: return Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
                default: return Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.Todas;
            }
        }
    }
}
