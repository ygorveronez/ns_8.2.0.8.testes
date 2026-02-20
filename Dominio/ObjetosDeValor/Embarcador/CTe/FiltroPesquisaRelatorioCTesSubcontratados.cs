using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioCTesSubcontratados
    {
        public TipoServicoMultisoftware TipoServicoMultisoftware;
        public DateTime? DataInicialEmissao { get; set; }
        public DateTime? DataFinalEmissao { get; set; }
        public string NumeroCarga { get; set; }
        public int CodigoEmpresaCTeOriginal { get; set; }
        public int CodigoEmpresaCTeSubcontratado { get; set; }
        public List<int> CodigosTransportadores { get; set; }
        public double TransportadorTerceiro { get; set; }
    }
}
