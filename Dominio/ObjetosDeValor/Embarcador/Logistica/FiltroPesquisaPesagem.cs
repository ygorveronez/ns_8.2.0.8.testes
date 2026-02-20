using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaPesagem
    {
        public StatusBalanca Status { get; set; }
        public string NumeroCarga { get; set; }
        public string CodigoPesagem { get; set; }
        public DateTime DataPesagemInicial { get; set; }
        public DateTime DataPesagemFinal { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoMotorista { get; set; }
    }
}
