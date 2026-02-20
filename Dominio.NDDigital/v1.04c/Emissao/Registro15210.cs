using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo de informações dos lacres dos containers
    /// </summary>
    public class Registro15210 :Registro 
    {
        #region Construtores

        public Registro15210(string registro)
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
