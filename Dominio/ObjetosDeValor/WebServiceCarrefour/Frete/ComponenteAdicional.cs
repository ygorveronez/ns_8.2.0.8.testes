namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Frete
{
    public sealed class ComponenteAdicional
    {
        public Componente Componente { get; set; }

        public decimal ValorComponente { get; set; }

        public bool IncluirBaseCalculoICMS { get; set; }

        public bool DescontarValorTotalAReceber { get; set; }

        public bool IncluirTotalReceber { get; set; }
    }
}
