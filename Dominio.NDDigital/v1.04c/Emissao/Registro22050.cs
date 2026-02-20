using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Dados da cobrança do CT-e
    /// </summary>
    public class Registro22050 : Registro
    {
        #region Construtores

        public Registro22050(string registro)
            : base(registro)
        {
            this.dup = new List<Registro22056>();

            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro22052 fat { get; set; }

        public List<Registro22056> dup { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
        }

        #endregion
    }
}
