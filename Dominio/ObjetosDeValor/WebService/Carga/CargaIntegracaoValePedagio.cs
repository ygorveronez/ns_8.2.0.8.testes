using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CargaIntegracaoValePedagio
    {
        public int CodigoIntegracaoValePedagioEmbarcador { get; set; }

        public SituacaoValePedagio SituacaoValePedagio { get; set; }

        public string NumeroValePedagio { get; set; }

        public string IdCompraValePedagio { get; set; }

        public decimal ValorValePedagio { get; set; }

        public string Observacao1 { get; set; }

        public string Observacao2 { get; set; }

        public string Observacao3 { get; set; }

        public string Observacao4 { get; set; }

        public string Observacao5 { get; set; }

        public string Observacao6 { get; set; }

        public string RotaTemporaria { get; set; }

        public string CodigoIntegracaoValePedagio { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar TipoRota { get; set; }

        public Dominio.Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }

        public bool CompraComEixosSuspensos { get; set; }

        public int CodigoRoteiro { get; set; }

        public int CodigoPercurso { get; set; }

        public int QuantidadeEixos { get; set; }

        public bool RecebidoPorIntegracao { get; set; }

        public bool ValidaCompraRemoveuComponentes { get; set; }

        public string NomeTransportador { get; set; }

        public TipoRotaFrete? TipoPercursoVP { get; set; }

        public string CnpjMeioPagamento { get; set; }
    }
}