namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public sealed class DadosPagamentoProvedor
    {
        public int ProtocoloCarga { get; set; }

        public bool IndicLiberacaoOk { get; set; }

        public decimal ValorTotalProvedor { get; set; }
    }
}
