using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Com hora de entrega
    /// </summary>
    public class Registro11328 : Registro
    {
        #region Construtores

        public Registro11328(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de hora/período programado para a entrega
        /// 1-No horário
        /// 2-Até o horário
        /// 3-A partir do horário
        /// </summary>
        public int tpHor { get; set; }

        /// <summary>
        /// Hora programada
        /// </summary>
        public TimeSpan hProg { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpHor = this.ObterNumero(dados[1]);
            this.hProg = this.ObterHora(dados[2]); 
        }

        #endregion
    }
}
