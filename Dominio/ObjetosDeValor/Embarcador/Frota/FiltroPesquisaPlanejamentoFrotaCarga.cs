using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaPlanejamentoFrotaCarga
    {
        public string CodigoCargaEmbarcador { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosFilialVenda { get; set; }
        public string NumeroPedido { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public DateTime Data { get; set; }
        public int CodigoFuncionarioVendedor { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosVeiculos { get; set; }
        public List<long> CodigosExpedidores { get; set; }
        public List<int> CodigosCarga { get; set; }
        public List<int> CodigosDestinos { get; set; }
        public List<double> CodigoClienteDestino { get; set; }
        public List<double> CodigoClienteOrigem { get; set; }
        public List<int> CodigosOrigem { get; set; }
        public List<string> EstadosOrigem { get; set; }
        public List<string> EstadosDestino { get; set; }
        public List<int> CodigosResponsavelVeiculo { get; set; }
        public List<int> CodigosCentroResultado { get; set; }
        public List<double> CodigosFronteiraRotaFrete { get; set; }
        public List<int> CodigosPaisDestino { get; set; }
        public List<int> CodigosPaisOrigem { get; set; }
        public List<int> CodigosTipoOperacaoDiferenteDe { get; set; }

    }
}
