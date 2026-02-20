using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class DadosNFe
    {
        public string Chave { get; set; }
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
        public decimal ValorFrete { get; set; }
        public decimal ValorOutros { get; set; }
        public string NaturezaOP { get; set; }
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
        public decimal BaseCalculoIBSCBS { get; set; }
        public decimal AliquotaIBSEstadual { get; set; }
        public decimal PercentualReducaoIBSEstadual { get; set; }
        public decimal ValorReducaoIBSEstadual { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public decimal AliquotaIBSMunicipal { get; set; }
        public decimal PercentualReducaoIBSMunicipal { get; set; }
        public decimal ValorReducaoIBSMunicipal { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal PercentualReducaoCBS { get; set; }
        public decimal ValorReducaoCBS { get; set; }
        public decimal ValorCBS { get; set; }
        public int TipoOperacao { get; set; }
        public string Empresa { get; set; }
        public string CNPJTransportador { get; set; }
        public string DataEmissao { get; set; }
        public Dominio.Entidades.Cliente Remetente { get; set; }
        public string RemetenteUF { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa PessoaDestinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa PessoaRemetente { get; set; }
        public string Destinatario { get; set; }
        public string DestinatarioUF { get; set; }
        public Dominio.ObjetosDeValor.Cliente DestinatarioExportacao { get; set; }
        public string Numero { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal Volume { get; set; }
        public decimal ValorLiquido { get; set; }
        public int Fatura { get; set; }
        public double NumeroDaFatura { get; set; }
        public string Especie { get; set; }
        public Dominio.Enumeradores.TipoPagamento FormaPagamento { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> Produtos { get; set; }
        public string Placa { get; set; }
        public List<string> NumeroDosCTesUtilizados { get; set; }
        public string Observacao { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao> ObservacaoContribuinte { get; set; }
        public string XPedido { get; set; }
        public string Protocolo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete ModalidadeFrete { get; set; }
        public string LocalEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Retirada Retirada { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Volume> Volumes { get; set; }

        public void SetarPessoaRemetente(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeEmit infNFeEmit)
        {
            PessoaRemetente = new Pessoas.Pessoa()
            {
                CPFCNPJ = infNFeEmit.Item,
                NomeFantasia = infNFeEmit.xNome,
                RazaoSocial = infNFeEmit.xNome,
                RGIE = infNFeEmit.IE
            };
        }
        public void SetarPessoaDestinatario(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDest infNFeDest)
        {
            PessoaDestinatario = new Pessoas.Pessoa()
            {
                CPFCNPJ = infNFeDest.Item,
                NomeFantasia = Utilidades.String.RemoveAllSpecialCharacters(infNFeDest.xNome),
                RazaoSocial = Utilidades.String.RemoveAllSpecialCharacters(infNFeDest.xNome),
                RGIE = infNFeDest.IE
            };
        }

        public void SetarRetirada(MultiSoftware.NFe.v400.NotaFiscal.TLocal retirada)
        {
            Retirada = new Pessoas.Retirada()
            {
                CNPJ = retirada.Item,
                Nome = retirada.xNome,
                Logradouro = retirada.xLgr,
                Numero = retirada.nro,
                CodigoMunicipio = retirada.cMun,
                Municipio = retirada.xMun,
                UF = retirada.UF,
                CEP = retirada.CEP,
                CodigoPais = retirada.cPais,
                Pais = retirada.xPais,
                Telefone = retirada.fone
            };
        }
    }
}
