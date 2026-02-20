using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Valores da prestação de serviço
    /// </summary>
    public class Registro23100 : Registro
    {
        #region Construtores

        public Registro23100(string registro)
            : base(registro)
        {
            this.compComp = new List<Registro23110>();

            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Valor total da prestação complementado
        /// </summary>
        public decimal vTPrest { get; set; }

        public List<Registro23110> compComp { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.vTPrest = this.ObterValor(dados[1]);
        }

        #endregion
    }
}
