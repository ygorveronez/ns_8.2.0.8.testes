namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoArquivoIntegracaoCTeCarga
    {
        EnvioParaProcessamento = 0,
        RetornoDoProcessamento = 1
    }

    public static class TipoArquivoIntegracaoCTeCargaHelper
    {
        public static string ObterDescricao(this TipoArquivoIntegracaoCTeCarga tipo)
        {
            switch (tipo)
            {
                case TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento: return "Envio para processamento";
                case TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento: return "Retorno do processamento";
                default: return string.Empty;
            }
        }
    }
}
