using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class RetornoBoleto
    {
        public string Comando { get; set; }
        public string NossoNumero { get; set; }
        public string Banco { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public decimal ValorRetorno { get; set; }
        public decimal ValorDocumento { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorTarifa { get; set; }
        public decimal ValorRecebido { get; set; }
        public DateTime DataCredito { get; set; }
        public string CodigoRejeicao { get; set; }
        public DateTime DataBaixa { get; set; }
        public DateTime DataImportacao { get; set; }
        public int CodigoTitulo { get; set; }
        public DateTime VencimentoTitulo { get; set; }
        public DateTime EmissaoTitulo { get; set; }
        public decimal ValorTitulo { get; set; }
        public string NossoNumeroTitulo { get; set; }
        public int Sequencia { get; set; }
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string DescricaoComando { get; set; }
    }
}
