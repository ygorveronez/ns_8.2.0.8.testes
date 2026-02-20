using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaTrajeto
    {
        public string Carga { get; set; }
        public SituacaoTrajeto? SituacaoTrajeto { get; set; }
    }
}
