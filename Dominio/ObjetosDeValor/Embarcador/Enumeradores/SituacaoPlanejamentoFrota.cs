namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPlanejamentoFrota
    {
        EmAnaliseTransportador = 0,
        EmAnaliseEmbarcador = 1,
        ListaDiariaGerada = 2
    }

    public static class SituacaoPlanejamentoFrotaHelper
    {
        public static string ObterDescricao(this SituacaoPlanejamentoFrota situacao)
        {
            switch (situacao)
            {
                case SituacaoPlanejamentoFrota.EmAnaliseEmbarcador: return "Em Análise Embarcador";
                case SituacaoPlanejamentoFrota.EmAnaliseTransportador: return "Em Análise Transportador";
                case SituacaoPlanejamentoFrota.ListaDiariaGerada: return "Lista Diária Gerada";
                default: return string.Empty;
            }
        }
    }
}
