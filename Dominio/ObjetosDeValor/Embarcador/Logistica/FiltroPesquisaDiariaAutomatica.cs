using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaDiariaAutomatica
    {
        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoCarga { get; set; }

        public LocalFreeTime LocalFreeTime { get; set; }

        public StatusDiariaAutomatica Status { get; set; }

    }
}
