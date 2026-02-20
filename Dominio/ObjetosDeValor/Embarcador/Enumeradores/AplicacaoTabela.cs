namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AplicacaoTabela
    {
        Todas = 0,
        Carga = 1,
        Ocorrencia = 2,
        CargaEOcorrencia = 3
    }

    public static class AplicacaoTabelaHelper
    {
        public static string ObterDescricao(this AplicacaoTabela aplicacaoTabela)
        {
            switch (aplicacaoTabela)
            {
                case AplicacaoTabela.Todas: return "Todas";
                case AplicacaoTabela.Carga: return "Carga";
                case AplicacaoTabela.Ocorrencia: return "Ocorrência";
                case AplicacaoTabela.CargaEOcorrencia: return "Carga e Ocorrência";
                default: return "Nenhuma";
            }
        }
    }
}
