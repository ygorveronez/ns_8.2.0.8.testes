using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaCargaAdicionarFluxoPatio
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoTransportador { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public List<double> CpfCnpjDestinatario { get; set; }

        public List<double> CpfCnpjRemetente { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }
    }
}
