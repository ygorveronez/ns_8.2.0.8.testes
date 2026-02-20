namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MercadoLivreHandlingUnitDetailSituacao
    {
        Pend_Download = 0,
        Pend_Processamento = 1,
        Concluido = 2,
        Desconsiderado = 3
    }

    public static class MercadoLivreHandlingUnitDetailSituacaoHelper
    {
        public static string ObterDescricao(this MercadoLivreHandlingUnitDetailSituacao situacao)
        {
            switch (situacao)
            {
                case MercadoLivreHandlingUnitDetailSituacao.Pend_Download: return "Pend. Download";
                case MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento: return "Pend. Processamento";
                case MercadoLivreHandlingUnitDetailSituacao.Concluido: return "Concluido";
                case MercadoLivreHandlingUnitDetailSituacao.Desconsiderado: return "Desconsiderado";
                default: return string.Empty;
            }
        }
    }
}
