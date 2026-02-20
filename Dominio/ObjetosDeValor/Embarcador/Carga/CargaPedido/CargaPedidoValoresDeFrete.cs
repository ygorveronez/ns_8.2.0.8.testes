namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedidoValoresDeFrete
    {
        public int CodigoCargaPedido { get; set; }
        public int CodigoPedido { get; set; }
        public string NomeRemetente { get; set; }
        public string NomeDestinatario { get; set; }
        public string NumeroPedido { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorFreteFilialEmissora { get; set; }
        public decimal ValorFreteAntesDaAlteracaoManual { get; set; }
        public decimal ValorFreteFilialEmissoraAntesDaAlteracaoManual { get; set; }
        public decimal ValorFreteDatabase { get; set; }
        public decimal ValorFreteFilialEmissoraDatabase { get; set; }
        public bool FreteAlteradoManualmente()
        {
            return (ValorFreteAntesDaAlteracaoManual != ValorFrete);
        }
        public bool FreteFilialEmissoraAlteradoManualmente()
        {
            return (ValorFreteFilialEmissoraAntesDaAlteracaoManual != ValorFreteFilialEmissora);
        }
    }
}