using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class FiltroPesquisaContratoFrete
    {
        public int NumeroContrato { get; set; }
        public string Carga { get; set; }
        public List<SituacaoContratoFrete> SituacaoContrato { get; set; }
        public double TransportadorTerceiro { get; set; }
        public bool? Bloqueado { get; set; }
        public string NumeroCIOT { get; set; }
        public DateTime DataInicialContratoFrete { get; set; }
        public DateTime DataFinalContratoFrete { get; set; }
        
    }
}
