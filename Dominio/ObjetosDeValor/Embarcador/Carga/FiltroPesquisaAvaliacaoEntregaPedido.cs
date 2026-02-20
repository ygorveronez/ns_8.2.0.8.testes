using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaAvaliacaoEntregaPedido
    {
        public string NumeroCarga { get; set; }

        public DateTime? DataAvaliacaoInicial { get; set; }

        public DateTime? DataAvaliacaoFinal { get; set; }

        public List<int> CodigosMotivos { get; set; }

        public List<int> CodigosTransportadores { get; set; }

        public List<int> CodigosVeiculos { get; set; }

        public List<double> CnpjsDestinatarios { get; set; }
    }
}
