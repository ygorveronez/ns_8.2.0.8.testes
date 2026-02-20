using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class RetornoPagamento
    {
        public string Comando { get; set; }
        public string NossoNumero { get; set; }
        public string Banco { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataPagamento { get; set; }
        public decimal ValorRetorno { get; set; }
        public decimal ValorRecebido { get; set; }
        public DateTime DataImportacao { get; set; }
        public int CodigoTitulo { get; set; }
        public DateTime VencimentoTitulo { get; set; }
        public DateTime EmissaoTitulo { get; set; }
        public decimal ValorTitulo { get; set; }
        public string NossoNumeroTitulo { get; set; }
        public string Fornecedor { get; set; }
        public string Agendamento { get; set; }
        public string NomeBanco { get; set; }
        public string NomeBancoPessoa { get; set; }
        public string NomeArquivo { get; set; }
        public string DescricaoAgendamento
        {
            get
            {
                switch (this.Agendamento)
                {
                    case "BD":
                        return "Agendado com Sucesso " + this.Agendamento;
                    case "CC":
                        return "Código de Barras Inválido " + this.Agendamento;
                    case "CD":
                        return "Código de Barras Inválido " + this.Agendamento;
                    case "CDCC":
                        return "Código de Barras Inválido " + this.Agendamento;
                    case "CCCD":
                        return "Código de Barras Inválido " + this.Agendamento;
                    case "YA":
                        return "Boleto não registrado " + this.Agendamento;
                    case "BB":
                        return "Número inválido do documento " + this.Agendamento;
                    case "00":
                        return "Quitação do título " + this.Agendamento;
                    default:
                        return this.Agendamento;
                }
            }
        }
        public string DescricaoDataVencimento
        {
            get
            {
                if (DataVencimento != DateTime.MinValue)
                    return DataVencimento.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoDataPagamento
        {
            get
            {
                if (DataPagamento != DateTime.MinValue)
                    return DataPagamento.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoDataImportacao
        {
            get
            {
                if (DataImportacao != DateTime.MinValue)
                    return DataImportacao.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoVencimentoTitulo
        {
            get
            {
                if (VencimentoTitulo != DateTime.MinValue)
                    return VencimentoTitulo.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoEmissaoTitulo
        {
            get
            {
                if (EmissaoTitulo != DateTime.MinValue)
                    return EmissaoTitulo.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
    }
}
