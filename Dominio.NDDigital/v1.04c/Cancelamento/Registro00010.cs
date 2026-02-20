namespace Dominio.NDDigital.v104.Cancelamento
{
    public class Registro00010 : Registro
    {
        #region Construtores

        public Registro00010(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string Id { get; set; }

        /// <summary>
        /// Identificação do Ambiente
        /// </summary>
        public int tbAmb { get; set; }

        /// <summary>
        /// Serviço solicitado 'CANCELAR'
        /// </summary>
        public string xServ { get; set; }

        /// <summary>
        /// Chave de acesso do CT-e
        /// </summary>
        public string chCTe { get; set; }

        /// <summary>
        /// Número do Protocolo
        /// </summary>
        public int nProt { get; set; }

        /// <summary>
        /// Justificativa
        /// </summary>
        public string xJust { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.Id = this.ObterString(dados[1]);
            this.tbAmb= this.ObterNumero(dados[2]);
            this.xServ = this.ObterString(dados[3]);
            this.chCTe = this.ObterString(dados[4]);
            this.nProt = this.ObterNumero(dados[5]);
            this.xJust = this.ObterString(dados[6]);
        }

        #endregion
    }
}