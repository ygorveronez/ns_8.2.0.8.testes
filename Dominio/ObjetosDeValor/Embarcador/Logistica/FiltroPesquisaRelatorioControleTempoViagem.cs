using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaRelatorioControleTempoViagem
    {
        public List<int> CodigosCargas { get; set; }
        public List<int> NumerosNota { get; set; }
        public List<int> Destinos { get; set; }
        public List<int> Transportadores { get; set; }
        public DateTime DataFaturaInicial { get; set; }
        public DateTime DataFaturaFinal { get; set; }
        public DateTime PrevisaoEntregaInicial { get; set; }
        public DateTime PrevisaoEntregaFinal { get; set; }
        public DateTime DataEntregaRealInicial { get; set; }
        public DateTime DataEntregaRealFinal { get; set; }
        public int Performance { get; set; }
        public int DiasRetornoComprovante { get; set; }
        public int TempoViagem { get; set; }
        public DateTime DataRetornoComprovanteInicial { get; set; }
        public DateTime DataRetornoComprovanteFinal { get; set; }
        public string DocumentoVenda { get; set; }
        public string RazaoSocialDestinatario { get; set; }
        public decimal ValorNotaInicial { get; set; }
        public decimal ValorNotaFinal { get; set; }

    }
}
