using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações complementares do CTe
    /// </summary>
    public class Registro11300 : Registro
    {
        #region Construtores

        public Registro11300(string registro)
            : base(registro)
        {
            this.obsCont = new List<Registro11340>();
            this.obsFisco = new List<Registro11350>();

            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Característica adicional do transporte
        /// </summary>
        public string xCaracAd { get; set; }

        /// <summary>
        /// Característica adicional do serviço
        /// </summary>
        public string xCaracSer { get; set; }

        /// <summary>
        /// Funcionário emissor do CTe
        /// </summary>
        public string xEmi { get; set; }

        /// <summary>
        /// Municipio origem para efeito de cálculo do frete
        /// </summary>
        public string origCalc { get; set; }

        /// <summary>
        /// Municipio destino para efeito de cálculo do frete
        /// </summary>
        public string destCalc { get; set; }

        /// <summary>
        /// Observações gerais
        /// </summary>
        public string xObs { get; set; }

        public Registro11310 fluxo { get; set; }

        public Registro11311 pass { get; set; }

        public Registro11320 entrega { get; set; }

        public List<Registro11340> obsCont { get; set; }

        public List<Registro11350> obsFisco { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.xCaracAd = this.ObterString(dados[1]);
            this.xCaracSer = this.ObterString(dados[2]);
            this.xEmi = this.ObterString(dados[3]);
            this.origCalc = this.ObterString(dados[4]);
            this.destCalc = this.ObterString(dados[5]);
            this.xObs = this.ObterString(dados[6]);
        }

        #endregion
    }
}
