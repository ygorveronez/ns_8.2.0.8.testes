namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFechamentoColetaProdutor
    {
        Todos = 0,
        EmCriacao = 1,
        AgEmissaoCarga = 2,
        Finalizado = 3,
        Cancelado = 4
    }

    public static class SituacaoFechamentoColetaProdutorHelper
    {
        public static string ObterDescricao(this SituacaoFechamentoColetaProdutor situacaoFechamentoColetaProdutor)
        {
            switch (situacaoFechamentoColetaProdutor)
            {
                case SituacaoFechamentoColetaProdutor.Todos: return "";
                case SituacaoFechamentoColetaProdutor.EmCriacao: return "Em Criação";
                case SituacaoFechamentoColetaProdutor.AgEmissaoCarga: return "Ag. Emissão Carga";
                case SituacaoFechamentoColetaProdutor.Finalizado: return "Finalizado";
                case SituacaoFechamentoColetaProdutor.Cancelado: return "Cancelada";
                default: return "";
            }
        }
    }
}
