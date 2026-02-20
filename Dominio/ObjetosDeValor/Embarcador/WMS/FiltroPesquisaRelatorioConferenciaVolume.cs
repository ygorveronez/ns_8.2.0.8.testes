using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaRelatorioConferenciaVolume
    {
        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoConferente { get; set; }
        public int CodigoMDFe { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroNota { get; set; }
        public string CodigoBarras { get; set; }
        public DateTime DataConferenciaInicial { get; set; }
        public DateTime DataConferenciaFinal { get; set; }
        public DateTime DataEmbarqueInicial { get; set; }
        public DateTime DataEmbarqueFinal { get; set; }
        public SituacaoRecebimento? Situacao { get; set; }

    }
}