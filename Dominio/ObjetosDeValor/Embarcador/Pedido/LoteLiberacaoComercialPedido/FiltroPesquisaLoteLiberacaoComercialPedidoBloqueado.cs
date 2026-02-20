using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido
{
    public class FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado
    {
        public int CodigoFilial { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> CodigosPedidos { get; set; }
        public List<double> CodigosDestinatarios { get; set; }
        public List<int> CodigosSituacaoComercialPedido { get; set; }
        public List<int> CodigosVendedores { get; set; }
        public List<int> CodigosGerentes { get; set; }
        public List<int> CodigosSupervisores { get; set; }
        public List<int> CodigosCanalEntregas { get; set; }
        public List<int> CodigosGrupoPessoas { get; set; }
        public List<int> CodigosCategorias { get; set; }
        public List<int> CodigosRegioes { get; set; }
    }
}
