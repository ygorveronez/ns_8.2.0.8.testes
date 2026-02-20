using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ValeAvulso
    {
        #region Propriedades

        public string NumeroVale { get; set; }

        public string EmpresaNome { get; set; }

        public string EmpresaLocalidade { get; set; }

        public decimal Valor { get; set; }

        public string DataDia { get; set; }

        public string DataMes { get; set; }

        public string DataAno { get; set; }

        public string PessoaNome { get; set; }

        public string PessoaCPFCNPJ { get; set; }

        public string Correspondente { get; set; }

        public TipoDocumentoValeAvulso TipoDocumento { get; set; }

        #endregion Propriedades

        #region Propriedades com regras

        public string ValorPorExtenso
        {
            get { return Utilidades.Conversor.DecimalToWords(Valor); }
        }

        public string TipoDocumentoValeAvulso
        {
            get { return TipoDocumento.ObterDescricao(); }
        }

        #endregion Propriedades com regras
    }
}