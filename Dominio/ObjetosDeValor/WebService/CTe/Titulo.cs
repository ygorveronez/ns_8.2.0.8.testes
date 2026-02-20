namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class Titulo
    {
        public string DataVencimento { get; set; }
        public string DataPagamento { get; set; }
        public decimal ValorTitulo { get; set; }
        public Embarcador.Enumeradores.StatusTitulo StatusPagamento { get; set; }
        public CTe CTe { get; set; }
    }
}
