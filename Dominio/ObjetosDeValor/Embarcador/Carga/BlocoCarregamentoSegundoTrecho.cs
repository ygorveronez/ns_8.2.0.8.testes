namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class BlocoCarregamentoSegundoTrecho
    {
        public int CodigoCarregamentoSegundoTrecho { get; set; }

        public int CodigoPedido { get; set; }

        public double CpfCnpjExpedidor { get; set; }

        public int OrdemCarregamento { get; set; }

        public int OrdemEntrega { get; set; }
    }
}
