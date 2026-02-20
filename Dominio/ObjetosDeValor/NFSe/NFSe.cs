using Dominio.ObjetosDeValor.CTe;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NFSe
{
    public class NFSe
    {
        public CTe.Empresa Emitente { get; set; }

        public Natureza Natureza { get; set; }

        public CTe.Cliente Tomador { get; set; }

        public CTe.Cliente Intermediario { get; set; }

        public List<Item> Itens { get; set; }

        public List<Documentos> Documentos { get; set; }

        public int CodigoIBGECidadePrestacaoServico { get; set; }

        public string SerieSubstituicao { get; set; }

        public int? NumeroSubstituicao { get; set; }

        public int? NumeroRPS { get; set; }

        public string DataEmissao { get; set; }

        public decimal ValorServicos { get; set; }

        public decimal ValorDeducoes { get; set; }

        public decimal ValorPIS { get; set; }

        public decimal ValorCOFINS { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorIR { get; set; }

        public decimal ValorCSLL { get; set; }

        public bool ISSRetido { get; set; }

        public decimal ValorISSRetido { get; set; }

        public decimal ValorOutrasRetencoes { get; set; }

        public decimal ValorDescontoIncondicionado { get; set; }

        public decimal ValorDescontoCondicionado { get; set; }

        public decimal AliquotaISS { get; set; }

        public decimal BaseCalculoISS { get; set; }

        public decimal ValorISS { get; set; }

        public string OutrasInformacoes { get; set; }

        public decimal PesoKg { get; set; }

        public int CodigoVeiculo { get; set; }

        public IBSCBS IBSCBS { get; set; }

    }
}
