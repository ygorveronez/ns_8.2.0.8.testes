using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class TituloAcrescimoDesconto
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Titulo { get; set; }
        public DateTime DataEmissao { get; set; }
        public string DataLiquidacao { get; set; }
        public string DataBaseLiquidacao { get; set; }
        public decimal ValorTitulo { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string ModeloDocumento { get; set; }
        public decimal ValorDocumento { get; set; }
        public string Empresa { get; set; }
        public double CPFCNPJPessoa { get; set; }
        public string TipoPessoa { get; set; }
        public string Pessoa { get; set; }
        public string GrupoPessoas { get; set; }
        public string SituacaoTitulo { get; set; }
        public int NumeroFatura { get; set; }
        public int NumeroBordero { get; set; }
        public string Justificativa { get; set; }
        public string Observacao { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }
        public string TipoJustificativa { get; set; }
        public string DataEmissaoDocumentos { get; set; }
        public string Usuario { get; set; }
        private DateTime DataAplicacao { get; set; }
        private string CPFMotorista { get; set; }
        public string NomeMotorista { get; set; }
        public string CodigoIntegracaoMotorista { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CPFCNPJPessoaFormatado
        {
            get
            {
                if (CPFCNPJPessoa > 0D)
                {
                    if (TipoPessoa == "J")
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJPessoa);
                    else
                        return string.Format(@"{0:000\.000\.000\-00}", CPFCNPJPessoa);
                }

                return string.Empty;
            }
        }

        public string DataAplicacaoFormatada
        {
            get { return DataAplicacao != DateTime.MinValue ? DataAplicacao.ToDateTimeString() : string.Empty; }
        }

        public string CPFMotoristaFormatado
        {
            get
            {
                if (!string.IsNullOrEmpty(CPFMotorista))
                {
                    return string.Format(@"{0:000\.000\.000\-00}", CPFMotorista);
                }

                return string.Empty;
            }
        }

        #endregion
    }
}
