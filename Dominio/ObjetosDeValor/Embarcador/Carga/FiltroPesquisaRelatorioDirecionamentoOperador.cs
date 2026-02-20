using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioDirecionamentoOperador
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public double CpfCnpjDestinatario { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoCentroCarregamento { get; set; }
        public int CodigoOperador { get; set; }
        public int CodigoRota { get; set; }
        public int CodigoModeloVeiculo { get; set; }
        public int CodigoVeiculo { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public string NumeroCarga { get; set; }
    }
}
