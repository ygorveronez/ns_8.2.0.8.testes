namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Sem hora de entrega
    /// </summary>
    public class Registro11327 : Registro
    {
        #region Construtores

        public Registro11327(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de hora/período programado para a entrega
        /// 0- Sem hora definida
        /// </summary>
        public int tpHor { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpHor = this.ObterNumero(dados[1]);
        }

        #endregion
    }
}
