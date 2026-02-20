using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações do Cte normal
    /// </summary>
    public class Registro15000 : Registro
    {
        #region Construtores

        public Registro15000(string registro)
            : base(registro)
        {
            this.docAnt = new List<Registro15300>();
            this.seg = new List<Registro15400>();
            this.peri = new List<Registro21000>();

            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro15100 infCarga { get; set; }

        public Registro15200 contQt { get; set; }

        public List<Registro15300> docAnt { get; set; }

        public List<Registro15400> seg { get; set; }

        public Registro15900 infModal { get; set; }

        public List<Registro21000> peri { get; set; }

        public Registro22050 cobr { get; set; }

        public Registro22100 infCteSub { get; set; }
        
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
