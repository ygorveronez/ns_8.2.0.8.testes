using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaConsultaEntregaAtrasada
    {
        public string NumeroCarga { get; set; }
        public int NumeroNota { get; set; }
        public DateTime? DataPrevisaoEntregaInicial { get; set; }
        public DateTime? DataPrevisaoEntregaFinal { get; set; }
        public int TipoOperacao { get; set; }
        public int Transportador { get; set; }
        public double Cliente { get; set; }
    }
}
