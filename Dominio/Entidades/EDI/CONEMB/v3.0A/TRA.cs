using System;

namespace Dominio.Entidades.EDI.CONEMB.v30A
{
    public class TRA : Registro
    {

        #region Construtores

        public TRA(Dominio.Entidades.Empresa transportadora)
            : base("321")
        {
            this.Transportadora = transportadora;

            if (this.Transportadora == null)
                throw new ArgumentNullException("transportadora", "A transportadora não pode ser nula para gerar um registro TRA.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.Empresa Transportadora { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Transportadora.CNPJ, 14);        //2.		C.G.C.
            this.EscreverDado(this.Transportadora.RazaoSocial, 40); //3.		RAZÃO SOCIAL
            this.EscreverDado(' ', 623);                            //4.		FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion

    }
}
