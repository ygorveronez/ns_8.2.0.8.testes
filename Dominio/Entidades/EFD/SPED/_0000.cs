using System;

namespace Dominio.Entidades.EFD.SPED
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

        public Empresa Empresa { get; set; }
        public int VersaoAtoCOTEPE { get; set; }
        public Dominio.Enumeradores.FinalidadeDoArquivoSPED FinalidadeArquivo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string PerfilDeApresentacao { get; set; }
        public Dominio.Enumeradores.TipoDeAtividade TipoDeAtividade { get; set; }
        
        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.VersaoAtoCOTEPE.ToString().PadLeft(3,'0')); //COD_VER
            this.EscreverDado(this.FinalidadeArquivo.ToString("D")); //COD_FIN
            this.EscreverDado(this.DataInicial); //DT_INI
            this.EscreverDado(this.DataFinal); //DT_FIN
            this.EscreverDado(this.Empresa.RazaoSocial, 100); //NOME
            this.EscreverDado(this.Empresa.CNPJ); //CNPJ
            this.EscreverDado(""); //CPF
            this.EscreverDado(this.Empresa.Localidade.Estado.Sigla); //UF
            this.EscreverDado(this.Empresa.InscricaoEstadual, 14); //IE
            this.EscreverDado(this.Empresa.Localidade.CodigoIBGE); //COD_MUN
            this.EscreverDado(""); //IM
            this.EscreverDado(this.Empresa.Suframa); //SUFRAMA
            this.EscreverDado(this.PerfilDeApresentacao); //IND_PERFIL
            this.EscreverDado(this.TipoDeAtividade.ToString("D")); //IND_ATIV

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
