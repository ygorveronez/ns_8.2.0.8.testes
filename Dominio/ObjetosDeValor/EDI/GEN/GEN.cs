using System;

namespace Dominio.ObjetosDeValor.EDI.GEN
{
    public class GEN
    {
        /// <summary>
        /// Business Event Code
        /// </summary>
        public string CodigoEvento { get; set; }

        /// <summary>
        /// Sub-Business Event Code
        /// </summary>
        public string CodigoSubEvento { get; set; }

        /// <summary>
        /// Version of the Extraction Specification upon which the file is based
        /// </summary>
        public string CodigoVersao { get; set; }

        /// <summary>
        /// Identity of the source system of the interface file
        /// </summary>
        public string IdSistema { get; set; }

        /// <summary>
        /// Date of the file generation in the country
        /// </summary>
        public DateTime DataGeracao { get; set; }

        /// <summary>
        /// Legacy system identifier of the Carrefour legal entity of the transaction.
        /// </summary>
        public string UnidadeNegocio { get; set; }

        /// <summary>
        /// Legacy system identifier of the Carrefour legal entity that is the counterpart of the transaction
        /// </summary>
        public string UnidadeNegocioFiliado { get; set; }

        /// <summary>
        /// Value which identifies at the same time the CARREFOUR business activity and its location related to the transaction
        /// </summary>
        public string LocalidadeAtividade { get; set; }

        /// <summary>
        /// Other Carrefour Activity Location involved in the transaction
        /// </summary>
        public string LocalidadeAtividadeFiliado { get; set; }

        /// <summary>
        /// Department or Service identifier within the external system
        /// </summary>
        public string IdDepartamento { get; set; }

        /// <summary>
        /// The date that a transaction is recognized in accounting. The accounting date determines the period in the general ledger to which the transaction is to be posted
        /// </summary>
        public DateTime? DataTransicao { get; set; }

        /// <summary>
        /// Brazilian Nota Fiscal Series
        /// </summary>
        public string SerieNotaFiscal { get; set; }

        /// <summary>
        /// Brazilian Nota Fiscal number
        /// </summary>
        public string NumeroNotaFiscal { get; set; }

        /// <summary>
        /// identifier within Source system
        /// </summary>
        public string IdOrigem { get; set; }

        /// <summary>
        /// Nota Fiscal receiving date
        /// </summary>
        public DateTime? DataNotaFiscal { get; set; }

        /// <summary>
        /// Delivery date
        /// </summary>
        public DateTime? DataEntrega { get; set; }

        /// <summary>
        /// Payment due date
        /// </summary>
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Unique code or number identifying the Vendor
        /// </summary>
        public string IdFornecedor { get; set; }

        /// <summary>
        /// "CNPJ or CPF the Vendo
        /// - preencher somente com números.Ou seja, sem formatação."
        /// </summary>
        public string CNPJFornecedor { get; set; }

        /// <summary>
        /// Brazilian Fiscal Transaction ID
        /// </summary>
        public string IdTransicaoFiscal { get; set; }

        /// <summary>
        /// Name of the ledger group impacted by the accounting entry
        /// </summary>
        public string GrupoContabil { get; set; }

        /// <summary>
        /// Total amount including tax
        /// </summary>
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// ISS Brazilian tax amount
        /// </summary>
        public decimal ValorISS { get; set; }

        /// <summary>
        /// Amount of INSS Brazilian Tax retention
        /// </summary>
        public decimal ValorINSSRetido { get; set; }

        /// <summary>
        /// IRRF brazilian tax amount
        /// </summary>
        public decimal ValorIRRF { get; set; }

        /// <summary>
        /// Leave blank when no special handling is needed. The currently defined codes are: "BLOCK_PAYMENT": the voucher is blocked for payment (but will be posted)
        /// </summary>
        public string TratamentoEspecial { get; set; }

        /// <summary>
        /// Date Conditional Discount
        /// </summary>
        public decimal ValorDescontoCondicional { get; set; }

        /// <summary>
        /// Description of the header content
        /// </summary>
        public string DescricaoCabecalho { get; set; }

        /// <summary>
        /// Define Account GL
        /// </summary>
        public string CodigoMontato { get; set; }

        /// <summary>
        /// Informative for compensation in accounting or reporting
        /// </summary>
        public string ALLOC_NMBR { get; set; }

        /// <summary>
        /// Monetary amount
        /// </summary>
        public decimal Moeda { get; set; }

        /// <summary>
        /// Outgoing Brazilian CFOP
        /// </summary>
        public string CFOP { get; set; }

        /// <summary>
        /// "Field required regarding the Fiscal Transaction Tax Book Mode Required for Individual or Accumulate Tax Book Mode
        /// Brazilian CFOP ingoing
        /// Field required regarding the Fiscal Transaction Tax Book Mode
        /// Required for Individual or Accumulate Tax Book Mode"
        /// </summary>
        public string CFOPEntrada { get; set; }

        /// <summary>
        /// Description of the line content
        /// </summary>
        public string DescricaoLinha { get; set; }

        // Banco

        /// <summary>
        ///  Nome 1
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        ///  Código postal
        /// </summary>
        public string CodigoPostal { get; set; }

        /// <summary>
        ///  Local
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        ///  Chave do país
        /// </summary>
        public string Pais { get; set; }

        /// <summary>
        /// " Código do país do banco
        /// - Fixo ""BR"""
        /// </summary>
        public string PaisBanco { get; set; }

        /// <summary>
        /// Código do Banco  ex. 341, 237,...
        /// </summary>
        public string CodigoBanco { get; set; }

        /// <summary>
        /// " Nº conta bancária
        /// - o dígito de verificação deverá ser informado no campo CTRL_KEY na segunda posição"
        /// </summary>
        public string NumeroConta { get; set; }

        /// <summary>
        ///  Chave de controle de bancos
        /// </summary>
        public string DigitoVerificador { get; set; }

        /// <summary>
        /// " Nº da agência bancária
        /// - o dígito de verificação deverá ser informado no campo CTRL_KEY na primeira posição."
        /// </summary>
        public string NumeroAgencia { get; set; }

        /// <summary>
        /// " Nº ID fiscal 1 (CNPJ)
        /// - preencher somente com números.Ou seja, sem formatação"
        /// </summary>
        public string IDFiscalCNPJ { get; set; }

        /// <summary>
        /// " Nº ID fiscal 2 (CPF)
        /// - preencher somente com números.Ou seja, sem formatação"
        /// </summary>
        public string IDFiscalCPF { get; set; }

        /// <summary>
        ///  Região (estado federal, estado federado, província, condado)
        /// </summary>
        public string Regiao { get; set; }

        /// <summary>
        ///  Chave de instruções para intercâmbio de suporte de dados
        /// </summary>
        public string ChaveIntercambio { get; set; }
    }
}
