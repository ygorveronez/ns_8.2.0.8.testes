using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaRelatorioPedidoOcorrencia
    {
        public string NumeroPedido { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataOcorrenciaInicial { get; set; }
        public DateTime DataOcorrenciaFinal { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoTipoOcorrencia { get; set; }
        public double CpfCnpjRemetente { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega? SituacaoEntrega { get; set; }
        public List<int> CodigosTransportadores { get; set; }
    }
}
