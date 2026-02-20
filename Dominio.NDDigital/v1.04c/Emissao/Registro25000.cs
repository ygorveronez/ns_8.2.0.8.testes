using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Detalhamento do CTe de anulação de valores
    /// </summary>
    public class Registro25000 : Registro
    {
        #region Construtores

        public Registro25000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Chave de acesso do CT-e a ser substituído
        /// </summary>
        public string chCTe { get; set; }

        /// <summary>
        /// Data de emissão da declaração do tomador não contribuinte do ICMS
        /// </summary>
        public DateTime dEmi { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.chCTe = this.ObterString(dados[1]);
            this.dEmi = this.ObterData(dados[2]);
        }

        #endregion
    }
}
