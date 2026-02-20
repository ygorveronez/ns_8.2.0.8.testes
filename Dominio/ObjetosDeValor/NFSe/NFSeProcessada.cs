using Dominio.ObjetosDeValor.CTe;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NFSe
{
    public class NFSeProcessada
    {
        public int Numero { get; set; }

        public int NumeroRPS { get; set; }

        public string Serie { get; set; }

        public string SerieRPS { get; set; }

        public int NumeroCarga { get; set; }

        public int NumeroUnidade { get; set; }

        public string Status { get; set; }

        public string DataEmissao { get; set; }

        public string DataEmissaoRPS { get; set; }

        public string HoraEmissaoRPS { get; set; }

        public string CFOP { get; set; }

        public CTe.Empresa Emitente { get; set; }

        public Natureza Natureza { get; set; }

        public CTe.Cliente Tomador { get; set; }

        public List<Item> Itens { get; set; }

        public int CodigoIBGECidadePrestacaoServico { get; set; }

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

        public IBSCBS IBSCBS { get; set; }
        
        public string OutrasInformacoes { get; set; }

        public Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        public string Protocolo { get; set; }

        public string DataProtocolo { get; set; }

        public string CodigoTipoOperacao { get; set; }

        public string Romaneio { get; set; }

        public string Placa { get; set; }

        public string TipoVeiculo { get; set; }

        public string TipoCalculo { get; set; }

        public decimal ValorDespesa { get; set; }

        public List<Documentos> Documentos { get; set; }

        public string CodigoControleInternoCliente { get; set; }
    }
}
