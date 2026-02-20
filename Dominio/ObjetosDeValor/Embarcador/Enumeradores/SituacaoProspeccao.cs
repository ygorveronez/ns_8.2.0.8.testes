namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoProspeccao
    {
        Pendente = 0,
        Vendido = 1,
        NaoVendido = 2,
        PlanejamentoPreparacao = 3,
        LevantamentoNecessidades = 4,
        Proposta = 5,
        Negociacao = 6,
        Fechado = 7,
        NaoFechado = 8,
        PosVenda = 9
    }

    public static class SituacaoProspeccaoHelper
    {
        public static string ObterDescricao(this SituacaoProspeccao situacaoProspeccao)
        {
            switch (situacaoProspeccao)
            {
                case SituacaoProspeccao.NaoVendido: return "Não Vendido";
                case SituacaoProspeccao.Vendido: return "Vendido";
                case SituacaoProspeccao.Pendente: return "Pendente";
                case SituacaoProspeccao.PlanejamentoPreparacao: return "Planejamento e Preparação";
                case SituacaoProspeccao.LevantamentoNecessidades: return "Levantamento de Necessidades";
                case SituacaoProspeccao.Proposta: return "Proposta";
                case SituacaoProspeccao.Negociacao: return "Negociação";
                case SituacaoProspeccao.Fechado: return "Fechado";
                case SituacaoProspeccao.NaoFechado: return "Não Fechado";
                case SituacaoProspeccao.PosVenda: return "Pós-venda";
                default: return string.Empty;
            }
        }
    }
}