using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioCargaPedidoEmbarcador
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<int> CodigosCanaisEntrega { get; set; }
        public List<int> CodigosTiposCarga { get; set; }
        public List<int> CodigosGrupoProduto { get; set; }
        public List<double> CodigosDestinatario { get; set; }
        public string PedidoEmbarcador { get; set; }
        public List<int> CodigosGrupoPessoa { get; set; }
        public List<int> CodigosProduto { get; set; }
        public List<int> TipoOperacao { get; set; }
        public List<StatusPedidoEmbarcadorAssai> StatusPedido { get; set; }
    }
}