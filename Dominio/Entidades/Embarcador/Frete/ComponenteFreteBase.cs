using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete
{
    public abstract class ComponenteFreteBase: EntidadeBase
    {
        public abstract int Codigo { get; set; }

        public abstract bool IncluirBaseCalculoICMS { get; set; }

        public abstract TipoComponenteFrete TipoComponenteFrete { get; set; }

        public abstract decimal ValorComponente { get; set; }
    }
}
