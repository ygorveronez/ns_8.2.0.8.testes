using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaSeparacaoPedidoPedidos
    {
        public int CodigoDestino { get; set; }

        public int CodigoPedido { get; set; }

        public int CodigoOrigem { get; set; }

        public int CodigoSeparacaoPedido { get; set; }

        public List<int> CodigosFilial { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public double CpfCnpjLocalExpedicao { get; set; }

        public List<double> CpfCnpjRemetentes { get; set; }

        public DateTime DataFinal { get; set; }

        public DateTime DataInicial { get; set; }

        public int Inicio { get; set; }

        public int Limite { get; set; }

        public List<int> NumerosNotaFiscal { get; set; }

        public bool SomentePedidosDeReentrega { get; set; }

        public bool SomentePedidosEmAberto { get; set; }
        public List<int> CodigosPedidos { get; set; }
    }
}
