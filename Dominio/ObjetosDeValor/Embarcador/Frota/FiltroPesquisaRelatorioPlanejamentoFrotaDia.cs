using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaRelatorioPlanejamentoFrotaDia
    {
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public List<int> CodigosVeiculo { get; set; }
        public string Placa { get; set; }
        public bool? Roteirizado { get; set; }
        public bool? Situacao { get; set; }
    }
}
