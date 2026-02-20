using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Em intervalo de tempo especificado
    /// </summary>
    public class Registro11329 : Registro
    {
        #region Construtores

        public Registro11329(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de hora/período programado para a entrega
        /// 4 - No intervalo de tempo
        /// </summary>
        public int tpHor { get; set; }

        /// <summary>
        /// Hora inicial
        /// </summary>
        public TimeSpan hIni { get; set; }

        /// <summary>
        /// Hora Final
        /// </summary>
        public TimeSpan hFim { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpHor = this.ObterNumero(dados[1]);
            this.hIni = this.ObterHora(dados[2]);
            this.hFim = this.ObterHora(dados[3]);
        }

        #endregion
    }
}
