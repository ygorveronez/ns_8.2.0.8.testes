using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.OFX
{
    [XmlRoot("OFX")]
    public class OFX
    {
        [XmlElement("SIGNONMSGSRSV1")]
        public SignonMessages SignonMessages { get; set; }

        [XmlElement("BANKMSGSRSV1")]
        public BankMessages BankMessages { get; set; }
    }

    public class SignonMessages
    {
        [XmlElement("SONRS")]
        public SignonResponse SignonResponse { get; set; }
    }

    public class SignonResponse
    {
        [XmlElement("STATUS")]
        public Status Status { get; set; }

        [XmlElement("DTSERVER")]
        public string ServerDateTime { get; set; }

        [XmlElement("LANGUAGE")]
        public string Language { get; set; }

        [XmlElement("FI")]
        public FinancialInstitution FinancialInstitution { get; set; }
    }

    public class Status
    {
        [XmlElement("CODE")]
        public string Code { get; set; }

        [XmlElement("SEVERITY")]
        public string Severity { get; set; }
    }

    public class FinancialInstitution
    {
        [XmlElement("ORG")]
        public string Organization { get; set; }

        [XmlElement("FID")]
        public string FID { get; set; }
    }

    public class BankMessages
    {
        [XmlElement("STMTTRNRS")]
        public StatementTransactionResponse StatementTransactionResponse { get; set; }
    }

    public class StatementTransactionResponse
    {
        [XmlElement("TRNUID")]
        public string TransactionUID { get; set; }

        [XmlElement("STATUS")]
        public Status Status { get; set; }

        [XmlElement("STMTRS")]
        public StatementTransactions StatementTransactions { get; set; }
    }

    public class StatementTransactions
    {
        [XmlElement("CURDEF")]
        public string CurrencyDefinition { get; set; }

        [XmlElement("BANKACCTFROM")]
        public BankAccountFrom BankAccountFrom { get; set; }

        [XmlElement("BANKTRANLIST")]
        public BankTransactionList BankTransactionList { get; set; }

        [XmlElement("LEDGERBAL")]
        public LedgerBalance LedgerBalance { get; set; }
    }

    public class BankAccountFrom
    {
        [XmlElement("BANKID")]
        public string BankID { get; set; }

        [XmlElement("BRANCHID")]
        public string BranchID { get; set; }

        [XmlElement("ACCTID")]
        public string AccountID { get; set; }

        [XmlElement("ACCTTYPE")]
        public string AccountType { get; set; }
    }

    public class BankTransactionList
    {
        [XmlElement("DTSTART")]
        public string StartDate { get; set; }

        [XmlElement("DTEND")]
        public string EndDate { get; set; }

        [XmlElement("STMTTRN")]
        public List<Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        [XmlElement("TRNTYPE")]
        public string TransactionType { get; set; }

        [XmlElement("DTPOSTED")]
        public string PostedDate { get; set; }

        [XmlElement("TRNAMT")]
        public decimal TransactionAmount { get; set; }

        [XmlElement("FITID")]
        public string FitID { get; set; }

        [XmlElement("MEMO")]
        public string Memo { get; set; }
    }

    public class LedgerBalance
    {
        [XmlElement("BALAMT")]
        public decimal BalanceAmount { get; set; }

        [XmlElement("DTASOF")]
        public string DateAsOf { get; set; }
    }
}
