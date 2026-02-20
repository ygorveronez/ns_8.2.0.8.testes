using System;

namespace Dominio.Entidades.EDI.CONEMB.v31
{
    public class DCC : Registro
    {
        #region Construtores

        public DCC(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
            : base("329")
        {
            this.CTe = cte;

           if (this.CTe == null)
                throw new ArgumentNullException("cte", "O CT-e não pode ser nulo para gerar um registro DCC.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Metodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("31", 5); //2.    TIPO DO MEIO DE TRANSPORTE (CONTINUAÇÃO)
            this.EscreverDado(0m, 13, 2); //3.    VALOR TOTAL DE DESPESAS EXTRAS/ADICIONAIS
            this.EscreverDado(0m, 13, 2); //4.    VALOR TOTAL DO ISS
            this.EscreverDado(' ', 10); //5.    FILIAL EMISSORA DO CONHECIMENTO ORIGINADOR DESTE CONHECIMENTO – TRANSPORTADORA CONTRATANTE
            this.EscreverDado(' ', 5); //6.    SÉRIE DO CONHECIMENTO ORIGINADOR DESTE CONHECIMENTO – TRANSPORTADORA CONTRATANTE
            this.EscreverDado(' ', 12); //7.    NÚMERO DO CONHECIMENTO ORIGINADOR DESTE CONHECIMENTO – TRANSPORTADORA CONTRATANTE
            this.EscreverDado(' ', 15); //8.    Codigo da solicitacao de coleta
            this.EscreverDado(' ', 20); //9.   IDENTIFICADOR DO DOCUMENTO DE TRANSPORTE DO EMBARCADOR
            this.EscreverDado(' ', 20); //10.    IDENTIFICACAO DO DOCUMENTO DE AUTORIZACAO DE CARREGAMENTO
            this.EscreverDado(this.CTe.Chave, 44); //11.    CHAVE DO CONHECIMENTO
            this.EscreverDado("57", 2); //12.    FILLER
            this.EscreverDado(' ', 514); //12.    FILLER
 
            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion

    }
}
