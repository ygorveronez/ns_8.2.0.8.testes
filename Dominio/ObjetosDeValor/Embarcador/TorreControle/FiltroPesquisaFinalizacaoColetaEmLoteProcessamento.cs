using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaFinalizacaoColetaEmLoteProcessamento
    {
        public List<int> CodigosCarga { get; set; }
        public SituacaoProcessamentoFinalizacaoColetaEntregaEmLote? Situacao { get; set; }
        public DateTime? DataInicialProcessamento { get; set; }
        public DateTime? DataFinalProcessamento { get; set; }
    }
}
