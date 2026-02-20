using System;
using CsvHelper.Configuration.Attributes;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ResumoContasAReceber
    {
        [Name("Sumarização")]
        [Index(0)]
        public string TipoSumarizacao { get; set; }

        [Name("Tipo Doc.")]
        [Index(1)]
        public string TipoDocumento { get; set; }

        [Name("Modelo Doc.")]
        [Index(2)]
        public string ModeloDocumento { get; set; }

        [Name("Tipo do CT-e")]
        [Index(3)]
        public string TipoCTe { get; set; }

        [Name("Grupo de Pessoas")]
        [Index(4)]
        public string GrupoPessoasTomador { get; set; }

        [Name("Valor")]
        [Index(5)]
        public decimal Valor { get; set; }

        [Name("Valor Total CT-e")]
        [Index(6)]
        public decimal ValorReceber { get; set; }

        [Name("Documento")]
        [Index(7)]
        public int NumeroDocumento { get; set; }

        [Name("Série")]
        [Index(8)]
        public int Serie { get; set; }

        [Name("Título")]
        [Index(9)]
        public int NumeroTitulo { get; set; }
        
        [Name("Data de Emissão")]
        [Index(10)]
        public DateTime DataEmissao { get; set; }

        [Name("Data de Vencimento")]
        [Index(11)]
        public string DataVencimento { get; set; }

        [Name("Data de Chegada do Último Canhoto")]
        [Index(12)]
        public string DataChegadaUltimoCanhoto { get; set; }

        [Name("Data do Último Canhoto")]
        [Index(13)]
        public string DataEnvioUltimoCanhoto { get; set; }

        [Name("Ocorrência")]
        [Index(14)]
        public int NumeroOcorrencia { get; set; }

        [Name("Carga")]
        [Index(15)]
        public string NumeroCarga { get; set; }

        [Name("Fatura")]
        [Index(16)]
        public string Fatura { get; set; }

        [Name("Número do Pedido do Cliente")]
        [Index(17)]
        public string NumeroPedidoCliente { get; set; }

        [Name("Número da Ocorrência do Cliente")]
        [Index(18)]
        public string NumeroOcorrenciaCliente { get; set; }

        [Name("Número da NF")]
        [Index(19)]
        public string NumeroNotaFiscal { get; set; }

        [Name("CPF/CNPJ do Tomador")]
        [Index(20)]
        public string CPFCNPJTomadorFormatado
        {
            get
            {
                if (TipoPessoaTomador == "J")
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJTomador);
                else
                    return String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJTomador);
            }
        }

        [Name("Nome do Tomador")]
        [Index(21)]
        public string NomeTomador { get; set; }

        [Name("CPF/CNPJ do Remetente")]
        [Index(22)]
        public string CPFCNPJRemetenteFormatado
        {
            get
            {
                if (CPFCNPJDestinatario > 0d)
                {
                    if (TipoPessoaRemetente == "J")
                        return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJRemetente);
                    else
                        return String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJRemetente);
                }

                return string.Empty;
            }
        }

        [Name("Nome do Remetente")]
        [Index(23)]
        public string NomeRemetente { get; set; }

        [Name("Origem")]
        [Index(24)]
        public string Origem { get; set; }

        [Name("UF da Origem")]
        [Index(25)]
        public string UFOrigem { get; set; }

        [Name("CPF/CNPJ do Destinatário")]
        [Index(26)]
        public string CPFCNPJDestinatarioFormatado
        {
            get
            {
                if (CPFCNPJDestinatario > 0d)
                {
                    if (TipoPessoaDestinatario == "J")
                        return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJDestinatario);
                    else
                        return String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJDestinatario);
                }

                return string.Empty;
            }
        }

        [Name("Nome do Destinatário")]
        [Index(27)]
        public string NomeDestinatario { get; set; }

        [Name("Destino")]
        [Index(28)]
        public string Destino { get; set; }

        [Name("UF do Destino")]
        [Index(29)]
        public string UFDestino { get; set; }

        [Name("Placas")]
        [Index(30)]
        public string Placas { get; set; }

        [Name("Frotas")]
        [Index(31)]
        public string Frotas { get; set; }

        [Name("Motoristas")]
        [Index(32)]
        public string Motoristas { get; set; }

        [Name("UF da Empresa")]
        [Index(33)]
        public string UFEmpresa { get; set; }

        [Name("Chave de Acesso do CT-e")]
        [Index(34)]
        public string ChaveAcessoCTe { get; set; }

        [Name("Observação do CT-e")]
        [Index(35)]
        public string ObservacaoCTe { get; set; }

        [Name("Nº Documento Título Original")]
        [Index(36)]
        public string NumeroDocumentoTituloOriginal { get; set; }

        [Name("Data NF")]
        [Index(37)]
        public string DescricaoDataNotaFiscal
        {
            get
            {
                if (DataNotaFiscal != DateTime.MinValue)
                    return DataNotaFiscal.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        [Name("Data Doc. Origem")]
        [Index(38)]
        public string DescricaoDataDocumento
        {
            get
            {
                if (DataDocumento != DateTime.MinValue)
                    return DataDocumento.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        [Ignore]
        public string TipoPessoaTomador { get; set; }
        [Ignore]
        public double CPFCNPJTomador { get; set; }

        [Ignore]
        public decimal Saldo { get; set; }
        
        [Ignore]
        public double CPFCNPJRemetente { get; set; }
        
        [Ignore]
        public string TipoPessoaRemetente { get; set; }

        [Ignore]
        public double CPFCNPJDestinatario { get; set; }

        [Ignore]
        public string TipoPessoaDestinatario { get; set; }

        [Ignore]
        public DateTime DataNotaFiscal { get; set; }

        [Ignore]
        public DateTime DataDocumento { get; set; }
    }
}
