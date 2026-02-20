using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido
{
    public class FiltroPesquisaLoteLiberacaoComercialPedido
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public SituacaoLoteLiberacaoComercialPedido Situacao { get; set; }
        public List<int> CodigosPedidos { get; set; }
    }
}
