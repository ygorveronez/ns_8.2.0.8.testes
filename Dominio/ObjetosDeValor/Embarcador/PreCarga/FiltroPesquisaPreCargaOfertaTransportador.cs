using System;

namespace Dominio.ObjetosDeValor.Embarcador.PreCarga
{
    public sealed class FiltroPesquisaPreCargaOfertaTransportador
    {
        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public string NumeroPreCarga { get; set; }
    }
}
