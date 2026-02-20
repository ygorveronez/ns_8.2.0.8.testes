namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class NotaFiscal
    {
        public string CNPJEmissor { get; set; }
        public string Chave { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public decimal Valor { get; set; }
    }
}
