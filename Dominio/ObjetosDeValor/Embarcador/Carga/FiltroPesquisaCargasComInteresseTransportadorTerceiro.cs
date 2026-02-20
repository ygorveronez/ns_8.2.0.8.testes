using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaCargasComInteresseTransportadorTerceiro
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int CodigoCentroCarregamento { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoCarga { get; set; }
        public SituacaoCargaJanelaCarregamentoTransportador? Situacao { get; set; }
        public long CodigoClienteTerceiro { get; set; }

    }
}
