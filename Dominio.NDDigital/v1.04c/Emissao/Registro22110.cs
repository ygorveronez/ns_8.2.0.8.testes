namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo para quando o tomador do serviço for contribuinte de ICMS
    /// </summary>
    public class Registro22110 : Registro
    {
        #region Construtores

        public Registro22110(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro22111 refNFe { get; set; }

        public Registro22112 refNF { get; set; }

        public Registro22113 refCTe { get; set; }

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
