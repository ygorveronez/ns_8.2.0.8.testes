using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public sealed class PlanejamentoFrotaMesAdicionar
    {
        public int Ano { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int Mes { get; set; }

        public List<int> CodigosVeiculos { get; set; }
    }
}
