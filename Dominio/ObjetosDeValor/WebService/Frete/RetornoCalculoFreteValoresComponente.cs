namespace Dominio.ObjetosDeValor.WebService.Frete
{
    public class RetornoCalculoFreteValoresComponente
    {
        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public bool IncluirBaseCalculoICMS { get; set; }
    }
}
