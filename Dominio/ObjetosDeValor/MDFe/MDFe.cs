using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MDFe
{
    public class MDFe
    {

        public CTe.Empresa Emitente { get; set; }

        public string UFCarregamento { get; set; }

        public string UFDescarregamento { get; set; }

        public string CIOT { get; set; }

        public string ObservacaoFisco { get; set; }

        public string ObservacaoContribuinte { get; set; }

        public Dominio.Enumeradores.UnidadeMedidaMDFe UnidadeMedidaMercadoria { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public decimal PesoBrutoMercadoria { get; set; }

        public string DataEmissao { get; set; }

        public int NumeroCarga { get; set; }

        public int NumeroUnidade { get; set; }

        public List<Percurso> Percursos { get; set; }

        public List<Lacre> Lacres { get; set; }

        public List<MunicipioCarregamento> MunicipiosDeCarregamento { get; set; }

        public List<MunicipioDescarregamento> MunicipiosDeDescarregamento { get; set; }

        public CTe.Veiculo Veiculo { get; set; }

        public List<CTe.Veiculo> Reboques { get; set; }

        public List<CTe.Motorista> Motoristas { get; set; }

        public List<ValePedagio> ValesPedagio { get; set; }

        public List<Contratante> Contratantes { get; set; }

        public List<Seguro> Seguros { get; set; }

        public List<CIOT> ListaCIOT { get; set; }

        public InformacoesPagamento InformacoesPagamento { get; set; }

        public string Versao { get; set; }

        public Dominio.Enumeradores.TipoCargaMDFe? TipoCargaMDFe { get; set; }
        public string ProdutoPredominanteDescricao { get; set; }
        public string ProdutoPredominanteCEAN { get; set; }
        public string ProdutoPredominanteNCM { get; set; }

        public int NumeroMDFe { get; set; }
        public int SerieMDFe { get; set; }
        public bool ControlaNumeroSerieForaDoEmbarcador { get; set; }
    }
}
