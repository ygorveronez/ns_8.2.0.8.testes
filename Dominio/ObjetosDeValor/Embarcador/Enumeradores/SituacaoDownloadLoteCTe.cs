namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDownloadLoteCTe
    {
        Pendente = 1,
        Finalizado = 2,
        Cancelado = 3,
        Falha = 4
    }

    public static class SituacaoDownloadLoteCTeHelper
    {

        public static string ObterDescricao(this SituacaoDownloadLoteCTe situacao)
        {
            switch (situacao)
            {
                case SituacaoDownloadLoteCTe.Pendente: return "Pendente";
                case SituacaoDownloadLoteCTe.Finalizado: return "Finalizado";
                case SituacaoDownloadLoteCTe.Cancelado: return "Cancelado";
                case SituacaoDownloadLoteCTe.Falha: return "Falha";
                default: return string.Empty;
            }
        }
        public static string ObterCorLinha(this SituacaoDownloadLoteCTe situacao)
        {
            switch (situacao)
            {
                case SituacaoDownloadLoteCTe.Pendente: return "#f5f4d0";
                case SituacaoDownloadLoteCTe.Finalizado: return "#ccebce";
                case SituacaoDownloadLoteCTe.Cancelado: return "#ada6a7";
                case SituacaoDownloadLoteCTe.Falha: return "#fab6c0";
                default: return string.Empty;
            }
        }
    }
}
