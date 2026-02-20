using System;

namespace Dominio.Entidades.EFPH
{
    public class _10 : Registro
    {
        #region Construtores

        public _10()
            : base("10")
        {
        }

        #endregion

        #region Propriedades

        public DateTime Data { get; set; }

        public Empresa Empresa { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.CNPJ, 14); //CNPJ do Estabelecimento
            this.EscreverDado(this.Empresa.InscricaoEstadual, 15); //Inscrição Estadual
            this.EscreverDado(this.Data, "MMyyyy"); //Mês/Ano
            this.EscreverDado("", 219); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
