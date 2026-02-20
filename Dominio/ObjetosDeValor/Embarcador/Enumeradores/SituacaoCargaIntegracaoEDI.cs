namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaIntegracaoEDI
    {
        AguardandoEmissao = 0,
        CertificadoVencido = 1,
        Ok = 2,
        ProblemaEmissao = 3,
        NaoImportada = 4,
        Cancelada = 5
    }

    public static class SituacaoCargaIntegracaoEDIHelper
    {
        public static string ObterDescricao(this SituacaoCargaIntegracaoEDI situacaoCarga)
        {
            switch (situacaoCarga)
            {
                case SituacaoCargaIntegracaoEDI.AguardandoEmissao: return Localization.Resources.Enumeradores.SituacaoCargaIntegracaoEDI.AguardandoEmissao;
                case SituacaoCargaIntegracaoEDI.CertificadoVencido: return Localization.Resources.Enumeradores.SituacaoCargaIntegracaoEDI.CertificadoVencido;
                case SituacaoCargaIntegracaoEDI.Ok: return Localization.Resources.Enumeradores.SituacaoCargaIntegracaoEDI.Emitida;
                case SituacaoCargaIntegracaoEDI.Cancelada: return Localization.Resources.Enumeradores.SituacaoCargaIntegracaoEDI.Cancelada;
                case SituacaoCargaIntegracaoEDI.NaoImportada: return Localization.Resources.Enumeradores.SituacaoCargaIntegracaoEDI.ViagemNaoImportada;
                case SituacaoCargaIntegracaoEDI.ProblemaEmissao: return Localization.Resources.Enumeradores.SituacaoCargaIntegracaoEDI.ProblemaEmissao;
                default: return Localization.Resources.Enumeradores.SituacaoCargaIntegracaoEDI.ProblemaEmissao;
            }
        }
    }
}
