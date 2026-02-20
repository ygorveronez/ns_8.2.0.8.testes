using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaAutorizacaoOcorrencia
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoOcorrencia SituacaoOcorrencia { get; set; }
        public EtapaAutorizacaoOcorrencia? EtapaAutorizacao { get; set; }
        public int NumeroOcorrencia { get; set; }
        public List<int> CodigosTipoOcorrencia { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<double> CodigosRecebedor { get; set; }
        public List<int> CodigosFilialVenda { get; set; }
        public List<int> CodigosDestino { get; set; }
        public List<int> CodigosUsuario { get; set; }
        public string NumeroCarga { get; set; }
        public List<int> PrioridadesAprovacao { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public TipoDocumentoCreditoDebito TipoDocumentoCreditoDebito { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public List<int> CodigosClienteComplementar { get; set; }
        public List<int> CodigosVendedor { get; set; }
        public List<int> CodigosSupervisor { get; set; }
        public List<int> CodigosGerente { get; set; }
        public List<string> CodigosUFDestino { get; set; }
        public int NumeroNF { get; set; }
    }
}
