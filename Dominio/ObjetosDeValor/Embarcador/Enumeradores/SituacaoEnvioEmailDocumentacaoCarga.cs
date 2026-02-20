namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEnvioEmailDocumentacaoCarga
    {
        PendenteDeEnvio = 0,
        EnviadoComSucesso = 1
    }

    public static class SituacaoEnvioEmailDocumentacaoCargaHelper
    {
        public static string ObterDescricao(this SituacaoEnvioEmailDocumentacaoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoEnvioEmailDocumentacaoCarga.PendenteDeEnvio: return "Pendente de Envio";
                case SituacaoEnvioEmailDocumentacaoCarga.EnviadoComSucesso: return "Enviado com Sucesso";
                default: return string.Empty;
            }
        }
    }
}
