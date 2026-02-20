namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GatilhoInicialTraking
    {
        PrevisaoDescarga = 1,
        EntradaFronteira = 2,
        EntradaCliente = 3,
        InicioEntrega = 4,
        EntradaParqueamento = 5
    }

    public static class GatilhoInicialTrakingHelper
    {
        public static string ObterDescricao(this GatilhoInicialTraking tipo)
        {
            switch (tipo)
            {
                case GatilhoInicialTraking.PrevisaoDescarga: return "Previsão de Descarga";
                case GatilhoInicialTraking.EntradaFronteira: return "Entrada na Fronteira";
                case GatilhoInicialTraking.EntradaParqueamento: return "Entrada em Parqueamento";
                case GatilhoInicialTraking.EntradaCliente: return "Entrada no Cliente";
                case GatilhoInicialTraking.InicioEntrega: return "Início da Entrega";
                default: return string.Empty;
            }
        }
    }
}
