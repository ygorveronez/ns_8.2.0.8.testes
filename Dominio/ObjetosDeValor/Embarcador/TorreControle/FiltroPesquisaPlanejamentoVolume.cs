using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaPlanejamentoVolume
    {
        public List<double> Destinatarios { get; set; }
        public List<double> Remetentes { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoTipoCarga { get; set; }
        public int CodigoTransportador { get; set; }
        public DateTime? DataProgramacaoCargaInicial { get; set; }
        public DateTime? DataProgramacaoCargaFinal { get; set; }

    }
}
