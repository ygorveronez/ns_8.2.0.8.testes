using System;

namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _0000 : Registro
    {
        #region Construtores

        public _0000()
            : base("0000")
        {
        }

        #endregion

        #region Propriedades

        public string Versao { get; set; }

        public Dominio.Enumeradores.TipoEscrituracao TipoEscrituracao { get; set; }

        public Dominio.Enumeradores.IndicadorDeSituacaoEspecial IndicadorSituacaoEspecial { get; set; }

        public string NumeroReciboAnterior { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public Empresa Empresa { get; set; }

        public Dominio.Enumeradores.TipoDeAtividadeSPEDContribuicoes TipoDeAtividade { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Versao); //COD_VER
            this.EscreverDado(this.TipoEscrituracao.ToString("D")); //TP_ESCRIT
            this.EscreverDado(this.IndicadorSituacaoEspecial.ToString("D")); //IND_SIT_ESP
            this.EscreverDado(this.NumeroReciboAnterior); //NUM_REC_ANTERIOR
            this.EscreverDado(this.DataInicial); //DT_INI
            this.EscreverDado(this.DataFinal); //DT_FIN
            this.EscreverDado(this.Empresa.RazaoSocial, 100); //NOME
            this.EscreverDado(this.Empresa.CNPJ); //CNPJ
            this.EscreverDado(this.Empresa.Localidade.Estado.Sigla); //UF
            this.EscreverDado(this.Empresa.Localidade.CodigoIBGE); //COD_MUN
            this.EscreverDado(this.Empresa.Suframa, 9); //IE
            this.EscreverDado(""); //IND_NAT_PJ
            this.EscreverDado(this.TipoDeAtividade.ToString("D")); //IND_ATIV

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
