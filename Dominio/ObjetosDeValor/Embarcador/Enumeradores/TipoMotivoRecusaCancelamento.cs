namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotivoRecusaCancelamento
    {
        Todos = 0,
        Recusa = 1,
        Cancelamento = 2,
    }


    public static class TipoMotivoRecusaCancelamentoHelper
    {

        public static string ObterDescricao(this TipoMotivoRecusaCancelamento TipoMotivoRecusaCancelamento)
        {
            switch (TipoMotivoRecusaCancelamento)
            {
                case TipoMotivoRecusaCancelamento.Todos: return "Todos";
                case TipoMotivoRecusaCancelamento.Recusa: return "Recusa";
                case TipoMotivoRecusaCancelamento.Cancelamento: return "Cancelamento";
                default: return "";
            }
        }

    }
}
