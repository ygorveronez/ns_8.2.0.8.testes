using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public sealed class FiltroPesquisaRelatorioProdutoEmbarcador
    {
        public DateTime? DataPedidoInicial { get; set; }
        public DateTime? DataPedidoFinal { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public List<int> CodigosCanaisEntrega { get; set; }
        public List<int> CodigosTiposCarga { get; set; }
        public List<int> CodigosGrupoProduto { get; set; }
        public List<double> CodigosDestinatario { get; set; }
        public List<int> PedidosEmbarcador { get; set; }
        public List<int> CodigosGrupoPessoa { get; set; }
        public List<int> CodigosProduto { get; set; }
        public List<int> TipoOperacao { get; set; }
        public List<StatusPedidoEmbarcadorAssai> StatusPedido { get; set; }
    }
}
