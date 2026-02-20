namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class Seguro
    {
        public Dominio.Enumeradores.TipoSeguro ResponsavelSeguro { get; set; }
        public string Seguradora { get; set; }
        public string CNPJSeguradora { get; set; }
        public string Apolice { get; set; }
        public string Averbacao { get; set; }
        public decimal Valor { get; set; }
    }
}
