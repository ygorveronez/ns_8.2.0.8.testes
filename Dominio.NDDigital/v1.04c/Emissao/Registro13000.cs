using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Valores da prestação de serviço
    /// </summary>
    public class Registro13000 : Registro
    {
        #region Construtores

        public Registro13000(string registro)
            : base(registro)
        {
            this.comp = new List<Registro13110>();

            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Valor total da prestação do serviço
        /// </summary>
        public decimal vTPrest { get; set; }

        /// <summary>
        /// Valor a Receber
        /// </summary>
        public decimal vRec { get; set; }

        public List<Registro13110> comp { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.vTPrest = this.ObterValor(dados[1]);
            this.vRec = this.ObterValor(dados[2]);
        }

        #endregion
    }
}
