namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações de seguro da carga
    /// </summary>
    public class Registro15400 : Registro
    {
        #region Construtores

        public Registro15400(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Responsável pelo Seguro
        /// 0 - Remetente, 1 - Expedidor, 2 - Recebedor, 3 - Destinatário, 4 - Emitente do CTe, 5 - Tomador de Serviço.
        /// </summary>
        public int respSeg { get; set; }

        /// <summary>
        /// Nome da Seguradora
        /// </summary>
        public string xSeg { get; set; }

        /// <summary>
        /// Número da Apólice
        /// </summary>
        public string nApol { get; set; }

        /// <summary>
        /// Número da Averbação
        /// </summary>
        public string nAver { get; set; }

        /// <summary>
        /// Valor da Carga para efeito de averbação
        /// </summary>
        public decimal vCarga { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.respSeg = this.ObterNumero(dados[1]);
            this.xSeg = this.ObterString(dados[2]);
            this.nApol = this.ObterString(dados[3]);
            this.nAver = this.ObterString(dados[4]);
            this.vCarga = this.ObterValor(dados[5]);
        }

        #endregion
    }
}
