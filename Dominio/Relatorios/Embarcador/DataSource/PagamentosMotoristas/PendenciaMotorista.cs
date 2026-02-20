using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista
{
    public sealed class PendenciaMotorista
    {
        public int Codigo { get; set; }
        public SituacaoPendenciaMotorista Situacao { get; set; }
        private DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Observacao { get; set; }
        public string Motorista { get; set; }
        public string CPFMotorista { get; set; }
        public string DescricaoJustificativa { get; set; }
        public string Justificativa { get; set; }
        public string DescricaoPendencia { get; set; }
        public string Pendencia { get; set; }
        public decimal ValorPendencia { get; set; }

        public string CPFMotoristaFormatado
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CPFMotorista))
                {
                    string cpf = Utilidades.String.OnlyNumbers(CPFMotorista);

                    long.TryParse(cpf, out long cpfCnpj);

                    if (cpf.Length > 11)
                        return String.Format(@"{0:00\.000\.000\/0000\-00}", cpfCnpj);
                    else
                        return String.Format(@"{0:000\.000\.000\-00}", cpfCnpj);
                }
                else
                {
                    return string.Empty;
                }
            }
        }



        public string DataFormatada
        {
            get
            {
                return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : "";
            }
        }

        public string SituacaoDescricao
        {
            get
            {
                return this.Situacao.ObterDescricao();
            }
        }
    }
}
