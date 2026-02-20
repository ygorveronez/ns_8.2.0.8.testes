using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Com data de entrega
    /// </summary>
    public class Registro11322 : Registro
    {
        #region Construtores

        public Registro11322(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de data/período programado para a entrega
        /// 1-Na data,
        /// 2-Até a data,
        /// 3-A partir da data
        /// </summary>
        public int tpPer { get; set; }

        /// <summary>
        /// Data programada
        /// </summary>
        public DateTime dProg { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpPer = this.ObterNumero(dados[1]);
            this.dProg = this.ObterData(dados[2]);
        }

        #endregion
    }
}
