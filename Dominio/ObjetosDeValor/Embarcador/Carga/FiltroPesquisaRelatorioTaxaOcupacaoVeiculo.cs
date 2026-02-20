using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioTaxaOcupacaoVeiculo
    {
        public DateTime DataCriacaoInicial { get; set; }
        public DateTime DataCriacaoFinal { get; set; }
        public DateTime DataJanelaCarregamentoInicial { get; set; }
        public DateTime DataJanelaCarregamentoFinal { get; set; }
        public int CodigoTransportador { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public int CodigoCentroCarregamento { get; set; }
        public int CodigoRota { get; set; }
        public int CodigoDestino { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public int CodigoModeloVeiculo { get; set; }
        public double CpfCnpjDestinatario { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
    }
}
