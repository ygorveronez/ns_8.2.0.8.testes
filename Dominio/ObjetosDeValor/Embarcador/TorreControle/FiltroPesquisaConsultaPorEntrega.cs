using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaConsultaPorEntrega
    {
        public string NumeroCarga { get; set; }
        public string NumeroNota { get; set; }
        public string Status { get; set; }
        public string Placa { get; set; }
        public DateTime DataPrevisaoEntregaInicial { get; set; }
        public DateTime DataPrevisaoEntregaFinal { get; set; }
        public DateTime DataCriacaoCargaInicial { get; set; }
        public DateTime DataCriacaoCargaFinal { get; set; }
        public int Operacao { get; set; }
        public int Transportador { get; set; }
    }
}
