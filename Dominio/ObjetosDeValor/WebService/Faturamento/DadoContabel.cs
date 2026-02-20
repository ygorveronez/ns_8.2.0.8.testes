namespace Dominio.ObjetosDeValor.WebService.Faturamento
{
    public class DadoContabel
    {
        public decimal Valor { get; set; }
        public ContaContabil ContaContabil { get; set; }
        public Dominio.ObjetosDeValor.WebService.Faturamento.CentroResultado CentroResultado { get; set; }
    }
}
