using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Entrega em determinado período
    /// </summary>
    public class Registro11323 : Registro
    {
        #region Construtores

        public Registro11323(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de data/período programado para a entrega
        /// 4 - No período
        /// </summary>
        public int tpPer { get; set; }

        /// <summary>
        /// Data inicial
        /// </summary>
        public DateTime dIni { get; set; }

        /// <summary>
        /// Data final
        /// </summary>
        public DateTime dFim { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpPer = this.ObterNumero(dados[1]);
            this.dIni = this.ObterData(dados[2]);
            this.dFim = this.ObterData(dados[3]);
        }

        #endregion
    }
}
