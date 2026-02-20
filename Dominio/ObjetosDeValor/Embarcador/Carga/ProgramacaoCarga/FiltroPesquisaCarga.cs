using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga
{
    public sealed class FiltroPesquisaCarga
    {
        public int CodigoFilial { get; set; }

        public List<int> CodigosDestinos { get; set; }

        public List<int> CodigosModelosVeicularesCarga { get; set; }

        public List<int> CodigosRegioesDestino { get; set; }

        public List<int> CodigosTiposCarga { get; set; }

        public List<int> CodigosTiposOperacao { get; set; }

        public List<DateTime> DatasFinalizacaoEmissao { get; set; }

        public List<string> SiglasEstadosDestino { get; set; }
    }
}
