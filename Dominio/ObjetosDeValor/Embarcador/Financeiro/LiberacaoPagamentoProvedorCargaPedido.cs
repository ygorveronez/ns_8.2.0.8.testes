namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class LiberacaoPagamentoProvedorCargaPedido
    {
        public int Codigo { get; set; }
        public int CodigoCarga { get; set; }
        public string Provedor { get; set; }
        ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor DocumentoProvedor { get; set; }
        public string EmpresaTomador { get; set; }
        public string Carga { get; set; }
        public string Localidade { get; set; }
        public decimal ValorTotalPrestacao { get; set; }
    }
}
