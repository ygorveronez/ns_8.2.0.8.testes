using System;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaRelatorioExpedicaoVolume
    {
        public int CodigoCarga { get; set; }
        public int CodigoConferente { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroNota { get; set; }
        public string CodigoBarras { get; set; }
        public DateTime DataExpedicaoInicial { get; set; }
        public DateTime DataExpedicaoFinal { get; set; }
        public DateTime DataEmbarqueInicial { get; set; }
        public DateTime DataEmbarqueFinal { get; set; }
    }
}
