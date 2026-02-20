using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class XMLNFe
    {
        public string Chave { get; set; }

        public string NaturezaOP { get; set; }

        public int TipoOperacao { get; set; }

        public string Empresa { get; set; }

        public string DataEmissao { get; set; }

        public string Remetente { get; set; }

        public string RemetenteIE { get; set; }

        public string RemetenteNome { get; set; }

        public string RemetenteLogradouro { get; set; }

        public string RemetenteCEP { get; set; }

        public string RemetenteBairro { get; set; }

        public string RemetenteNumero { get; set; }

        public string RemetenteCidade { get; set; }

        public string RemetenteUF { get; set; }

        public string Destinatario { get; set; }

        public string DestinatarioIE { get; set; }

        public string DestinatarioNome { get; set; }

        public string DestinatarioLogradouro { get; set; }

        public string DestinatarioCEP { get; set; }

        public string DestinatarioBairro { get; set; }

        public string DestinatarioNumero { get; set; }

        public string DestinatarioCidade { get; set; }

        public string DestinatarioUF { get; set; }

        public Dominio.ObjetosDeValor.Cliente DestinatarioExportacao { get; set; }

        public string Numero { get; set; }

        public string Modelo { get; set; }

        public string Serie { get; set; }

        public decimal Peso { get; set; }

        public decimal PesoLiquido { get; set; }

        public decimal Volume { get; set; }
        public decimal Cubagem { get; set; }

        public string FormaPagamento { get; set; }

        public string Placa { get; set; }

        public List<string> NumeroDosCTesUtilizados { get; set; }

        public string Observacao { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal BaseCalculoST { get; set; }

        public decimal ValorST { get; set; }

        public decimal ValorTotalProdutos { get; set; }

        public decimal ValorSeguro { get; set; }

        public decimal ValorDesconto { get; set; }

        public decimal ValorImpostoImportacao { get; set; }

        public decimal ValorIPI { get; set; }

        public decimal ValorPIS { get; set; }

        public decimal ValorCOFINS { get; set; }

        public decimal ValorOutros { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorAdicionalEntrega { get; set; }

        public decimal ValorAdValorem { get; set; }

        public decimal ValorDescarga { get; set; }

        public decimal PercentualSobreNf { get; set; }

        public decimal ValorSobreNf { get; set; }

        public decimal ValorGris { get; set; }

        public Dominio.Enumeradores.IncluiICMSFrete incluirICMS { get; set; }

        public Dominio.Enumeradores.TipoRateioTabelaFreteValor TipoRaterio { get; set; }

        public bool Adicionada { get; set; }

        public string NumeroRomaneio { get; set; }

        public string NumeroCTeAnterior { get; set; }

        public string ChaveCTeAnterior { get; set; }

        public string EmissorCTeAnterior { get; set; }

        public decimal AliquotaICMS { get; set; }

        public string ObservacaoCTe { get; set; }
        public string Modalidade { get; set; }
        public string Pedido { get; set; }

        public string NumeroDT { get; set; }
        public string ObsPlaca { get; set; }
        public string ObsTransporte { get; set; }
    }
}
