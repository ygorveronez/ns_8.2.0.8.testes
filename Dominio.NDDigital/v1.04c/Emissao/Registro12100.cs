using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações do remetente das mercadorias transportadas
    /// </summary>
    public class Registro12100 : Registro
    {
        #region Construtores

        public Registro12100(string registro)
            : base(registro)
        {
            this.infNF = new List<Registro12120>();
            this.infNFe = new List<Registro12130>();
            this.infOutros = new List<Registro12140>();

            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string CPF_CNPJ { get; set; }

        /// <summary>
        /// Inscrição Estadual
        /// </summary>
        public string IE { get; set; }

        /// <summary>
        /// Razão social
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// Nome fantasia
        /// </summary>
        public string xFant { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public string fone { get; set; }

        public string email { get; set; }

        /// <summary>
        /// Atividade
        /// </summary>
        public int ativ { get; set; }

        public Registro12110 enderReme { get; set; }

        public List<Registro12120> infNF { get; set; }

        public List<Registro12130> infNFe { get; set; }

        public List<Registro12140> infOutros { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CPF_CNPJ = this.ObterString(dados[1]);
            this.IE = this.ObterString(dados[2]);
            this.xNome = this.ObterString(dados[3]);
            this.xFant = this.ObterString(dados[4]);
            this.fone = this.ObterString(dados[5]);
            this.email = this.ObterString(dados[6]);
            this.ativ = this.ObterNumero(dados[7]);
        }

        #endregion
    }
}
