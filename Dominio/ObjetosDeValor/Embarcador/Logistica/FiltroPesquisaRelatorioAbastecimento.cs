using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaRelatorioConsolidacaoGas
    {
        public List<double> Bases { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public TipoFilialAbastecimentoGas TipoFilial { get; set; }
        public SimNao DisponibilidadeTransferencia { get; set; }
        public SimNao VolumeRodoviario { get; set; }
    }
}
