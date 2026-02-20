using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaFilaCarregamentoVeiculoPreCarga
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoPreCarga { get; set; }

        public List<int> CodigosDestinos { get; set; }

        public List<int> CodigosRegioesDestino { get; set; }

        public DateTime? DataProgramada { get; set; }

        public List<string> SiglasEstadosDestino { get; set; }
    }
}
