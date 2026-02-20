namespace Dominio.ObjetosDeValor.WebService.CargoX
{
    public class ComponenteAdicional
    {
        public Componente Componente { get; set; }

        public decimal ValorComponente { get; set; }

        public bool IncluirBaseCalculoICMS { get; set; }
    }
}
