namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GatilhoFinalTraking
    {
        SaidaCliente = 1,
        SaidaFronteira = 2,
        FimEntrega = 3,
        SaidaParqueamento = 4

    }

    public static class GatilhoFinalTrakingHelper
    {
        public static string ObterDescricao(this GatilhoFinalTraking tipo)
        {
            switch (tipo)
            {
                case GatilhoFinalTraking.SaidaCliente: return "Saída do Cliente";
                case GatilhoFinalTraking.SaidaFronteira: return "Saída da Fronteira";
                case GatilhoFinalTraking.FimEntrega: return "Fim da Entrega";
                case GatilhoFinalTraking.SaidaParqueamento: return "Saída de Parqueamento";
                default: return string.Empty;
            }
        }
    }
}
