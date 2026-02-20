using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta
{
    public class GestaoDadosColetaDadosNFeAdicionar
    {
        public int CodigoCargaEntrega { get; set; }

        public OrigemGestaoDadosColeta Origem { get; set; }

        public OrigemFotoDadosNFEGestaoDadosColeta OrigemFoto { get; set; }

        public string GuidArquivo { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }
}
