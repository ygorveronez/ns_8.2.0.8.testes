using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta
{
    public class FiltroPesquisaGestaoDadosColeta
    {
        public int CodigoTransportador { get; set; }
        public int CodigoFilialEmbarcador { get; set; }
        public double CodigoCliente { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public DateTime DataInicialCriacaoCarga { get; set; }
        public DateTime DataFinalCriacaoCarga { get; set; }
        public int DestinoCarga { get; set; }
        public int OrigemCarga { get; set; }

        public SituacaoGestaoDadosColetaRetornoConfirmacao? RetornoConfirmacao { get; set; }

        public SituacaoGestaoDadosColeta? SituacaoGestaoDadosColeta { get; set; }
    }
}
