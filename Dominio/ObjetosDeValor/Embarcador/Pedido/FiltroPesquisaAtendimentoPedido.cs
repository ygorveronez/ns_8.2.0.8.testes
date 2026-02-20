using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaAtendimentoPedido
    {
        public string NumeroPedido { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public bool ApenasPedidosComChat { get; set; }
        public double Remetente { set { Remetentes = value > 0 ? new List<double>() { value } : null; } }

        public int NumeroNotaFiscal { set { NumeroNotasFiscais = value > 0 ? new List<int>() { value } : null; } }

        public List<int> NumeroNotasFiscais { get; set; }

        public List<double> Remetentes { get; set; }
    }
}
