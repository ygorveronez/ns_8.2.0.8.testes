namespace Dominio.ObjetosDeValor.WebService.CRT
{
    public class ComponentePrestacaoCRT
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public bool IncluiNaBaseDeCalculoDoICMS { get; set; }
        public bool IncluiNoTotalAReceber { get; set; }
    }
}
