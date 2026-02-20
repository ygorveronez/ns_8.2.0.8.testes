using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Referente dados das Ordens de Coleta
    /// </summary>
    public class Registro16200 : Registro
    {
        #region Construtores

        public Registro16200(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Série da OCC
        /// </summary>
        public string serie { get; set; }

        /// <summary>
        /// Número da OCC
        /// </summary>
        public int nOcc { get; set; }

        /// <summary>
        /// Data de emissão
        /// </summary>
        public DateTime dEmi { get; set; }

        public Registro16210 emiOcc { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.serie = this.ObterString(dados[1]);
            this.nOcc = this.ObterNumero(dados[2]);
            this.dEmi = this.ObterData(dados[3]);
        }

        #endregion
    }
}
