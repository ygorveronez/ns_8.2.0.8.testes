using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frotas
{
    public class AbastecimentoTicketLog
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int CodigoTransacao { get; set; }
        public decimal ValorTransacao { get; set; }
        private DateTime DataTransacao { get; set; }
        public string Fornecedor { get; set; }
        private double CNPJFornecedor { get; set; }
        public decimal Litros { get; set; }
        public string TipoCombustivel { get; set; }
        public string Veiculo { get; set; }
        public decimal ValorLitro { get; set; }
        public int Quilometragem { get; set; }
        private DateTime DataIntegracao { get; set; }
        public int CodigoAbastecimento { get; set; }
        private string Situacao { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataTransacaoFormatada
        {
            get { return DataTransacao != DateTime.MinValue ? DataTransacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataIntegracaoFormatada
        {
            get { return DataIntegracao != DateTime.MinValue ? DataIntegracao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case "A":
                        return "Aberto";
                    case "I":
                        return "Inconsistente";
                    case "F":
                        return "Fechado";
                    case "G":
                        return "Agrupado";
                    default:
                        return "";
                }
            }
        }

        public string CNPJFornecedorFormatado
        {
            get { return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJFornecedor); }
        }

        #endregion
    }
}
