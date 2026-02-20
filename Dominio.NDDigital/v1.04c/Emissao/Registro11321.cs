namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Sem data de entrega
    /// </summary>
    public class Registro11321 : Registro
    {
        #region Construtores

        public Registro11321(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de data/período programado para a entrega
        /// 0 - Sem data definida
        /// </summary>
        public int tpPer { get; set; }
    
        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpPer = this.ObterNumero(dados[1]);
        }

        #endregion
    }
}
