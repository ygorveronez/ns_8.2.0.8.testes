namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoModeloEmail
    {
        Padrao = 1,
        AgendamentoEntrega = 2,
        GestaoCustoContabilDevolucao = 3,
        ImprocedenciaCenarioPosEntregaDevolucao = 4,
        AgendamentoColeta = 5

    }

    public static class TipoModeloEmailHelper
    {
        public static string ObterDescricao(this TipoModeloEmail tipo)
        {
            switch (tipo)
            {
                case TipoModeloEmail.Padrao: return "Padrão";
                case TipoModeloEmail.AgendamentoEntrega: return "Agendamento de entrega";
                case TipoModeloEmail.GestaoCustoContabilDevolucao: return "Gestão de Custo e Contábil Devolução";
                case TipoModeloEmail.ImprocedenciaCenarioPosEntregaDevolucao: return "Improcedência cenário Pós Entrega Devolução";
                case TipoModeloEmail.AgendamentoColeta: return "Agendamento de Coleta";
                default: return string.Empty;
            }
        }
    }
}
