using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista
{
    public sealed class PagamentoMotorista
    {
        public int Codigo {get; set;}
        public int NumeroPagamento {get; set;}
        public SituacaoPagamentoMotorista Situacao {get; set;}
        public EtapaPagamentoMotorista Etapa {get; set;}
        public string Carga {get; set;}
        public string Motorista {get; set;}
        public decimal Valor {get; set;}
        public DateTime DataPagamento {get; set;}
        public string TipoPagamento {get; set;}
        public string PlanoSaida {get; set;}
        public string PlanoEntrada {get; set;}
        public string Operador { get; set; }
        public string Observacao {get; set;}
        public MoedaCotacaoBancoCentral Moeda {get; set;}
        public decimal ValorMoeda {get; set;}
        public decimal ValorOriginalMoeda {get; set;}
        public string CPFMotorista { get; set; }
        public int Acerto { get; set; }
        public string Favorecido { get; set; }
        private DateTime DataEfetivacao { get; set; }
        public string Tomador { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroDocumentoComplementar { get; set; }
        private string CNPJTomador { get; set; }
        public decimal ValorIRRF { get; set; }
        public decimal ValorINSS { get; set; }
        public decimal ValorSEST { get; set; }
        public decimal ValorSENAT { get; set; }
        public DateTime Data {  get; set; }
        public decimal ValorLiquido { get; set; }
        public string CPFMotoristaFormatado
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CPFMotorista)) {
                    string cpf = Utilidades.String.OnlyNumbers(CPFMotorista);

                    long.TryParse(cpf, out long cpfCnpj);

                    if (cpf.Length > 11)
                        return String.Format(@"{0:00\.000\.000\/0000\-00}", cpfCnpj);
                    else
                        return String.Format(@"{0:000\.000\.000\-00}", cpfCnpj);
                } else
                {
                    return string.Empty;
                }
            }
        }

        public string CNPJTomadorFormatado
        {
            get { return CNPJTomador.ObterCnpjFormatado(); }
        }

        public string SituacaoDescricao
        {
            get
            {
                return this.Situacao.ObterDescricao();
            }
        }
        
        public string EtapaDescricao
        {
            get
            {
                return this.Etapa.ObterDescricao();
            }
        }

        public string DataPagamentoFormatada
        {
            get
            {
                return DataPagamento != DateTime.MinValue ? DataPagamento.ToString("dd/MM/yyyy") : "";
            }
        }

        public string MoedaDescricao
        {
            get
            {
                return this.Moeda.ObterDescricao();
            }
        }

        public string DataEfetivacaoFormatada
        {
            get
            {
                return DataEfetivacao != DateTime.MinValue ? DataEfetivacao.ToString("dd/MM/yyyy HH:mm:ss") : "";
            }
        }

        public string DataLancamentoFormatada
        {
            get
            {
                return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : "";
            }
        }
    }
}
