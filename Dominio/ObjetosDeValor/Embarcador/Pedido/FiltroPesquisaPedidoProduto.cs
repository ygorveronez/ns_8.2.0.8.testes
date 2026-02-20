using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaPedidoProduto
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public double Remetente { get; set; }
        public double Destinatario { get; set; }
        public List<int> CodigosProduto { get; set; }
    }
}
