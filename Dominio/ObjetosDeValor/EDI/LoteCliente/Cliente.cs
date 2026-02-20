using System;

namespace Dominio.ObjetosDeValor.EDI.LoteCliente
{
    public class Cliente
    {
        public int CodigoEmpresa { get; set; }
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public int CodigoCidade { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string EnderecoCompleto { get; set; }
        public string Telefone { get; set; }
        public string Fax { get; set; }
        public string CEP { get; set; }
        public string CnpjCpf { get; set; }
        public string Tipo { get; set; }
        public string RaizCnpjCpf { get; set; }
        public int Filial { get; set; }
        public int Digito { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string InscricaoSuframa { get; set; }
        public string EANFornecedor { get; set; }
        public string GeraCreditoICMS { get; set; }
        public string GeraCreditoPIS { get; set; }
        public string GeraCreditoCOFINS { get; set; }
        public string GeraCreditoIPI { get; set; }
        public string AssistenciaTecnica { get; set; }
        public string AgregaICMSSTValorMercadoria { get; set; }
        public string ExigeICMSSTRecebimento { get; set; }
        public string CalculaIPIValorLiquido { get; set; }
        public string CalculaICMSValorLiquido { get; set; }
        public string CalculaICMSSTValorLiquido { get; set; }
        public string ConsideraDespesaBaseIPI { get; set; }
        public string ConsideraDespesaBaseICMS { get; set; }
        public string ConsideraDespesaBaseICMSST { get; set; }
        public string ConsideraFreteBaseIPI { get; set; }
        public string ConsideraFreteBaseICMS { get; set; }
        public string ConsideraFreteBaseICMSST { get; set; }
        public string ConsideraIPIBaseICMS { get; set; }
        public string ConsideraIPIBaseICMSST { get; set; }
        public string ZonaFranca { get; set; }
        public int Status { get; set; }
        public int MemberShip { get; set; }
        public int NumeroCartaoMemberShip { get; set; }
        public string PessoaFisicaContribuinte { get; set; }
        public string CodigoCEI { get; set; }
        public string CodigoCBO { get; set; }
        public string NumeroPISPASEP { get; set; }
        public string CategoriaTrabalhador { get; set; }
        public string OcorrenciaTrabalhador { get; set; }
        public string CodigoCaixaPostal { get; set; }
        public string CEPCaixaPostal { get; set; }
        public string EnderecoEletronico { get; set; }
        public string ParticipanteProgramaFomeZero { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string EnderecoComplementar { get; set; }
        public string GNFe { get; set; }
        public DateTime DataInicialGNFe { get; set; }
        public string RegimeEspecial { get; set; }
        public string OrigemFornecedor { get; set; }
        public string OrigemMembership { get; set; }
        public string OrigemCliente { get; set; }
        public string QuantidadeTipoNegocio { get; set; }
        public string TipoNegocio { get; set; }
        public string CodigoEndereco { get; set; }
    }
}
