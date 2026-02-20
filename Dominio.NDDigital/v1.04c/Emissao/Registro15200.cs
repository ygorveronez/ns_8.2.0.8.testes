using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo de informações dos cointainers da quantidade da carga
    /// </summary>
    public class Registro15200 : Registro
    {
        #region Construtores

        public Registro15200(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Número do container
        /// </summary>
        public string nCont { get; set; }

        /// <summary>
        /// Data prevista da entrega
        /// </summary>
        public DateTime dPrev { get; set; }

        public Registro15210 lacContQt { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.nCont = this.ObterString(dados[1]);
            this.dPrev = this.ObterData(dados[2]);
        }

        #endregion
    }
}
