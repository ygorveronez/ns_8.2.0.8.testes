namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro11110 : Registro
    {
        #region Construtores

        public Registro11110(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// 0 - Remetente;
        /// 1 - Expedidor;
        /// 2 - Recebedor;
        /// 3 - Destinatário.
        /// </summary>
        public int toma { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.toma = this.ObterNumero(dados[1]);
        }

        #endregion
    }
}
