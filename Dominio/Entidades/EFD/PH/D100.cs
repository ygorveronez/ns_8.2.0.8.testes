namespace Dominio.Entidades.EFD.PH
{
    public class D100 : RegistroPH
    {
        #region Construtores

        public D100()
            : base("D100")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }
        public int versaoAtoCOTEPE { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            //REG, IND_OPER, IND_EMIT, COD_MOD, COD_SIT, SER, SUB, NUM_DOC e CHV_CT-e.

            this.EscreverDado("1"); //IND_OPER
            this.EscreverDado("0"); //IND_EMIT

            if (!this.CTe.Status.Equals("D") && !this.CTe.Status.Equals("I") && !this.CTe.Status.Equals("C"))
                this.EscreverDado(this.CTe.TomadorPagador != null ? this.CTe.TomadorPagador.CPF_CNPJ_SemFormato : ""); //COD_PART
            else
                this.EscreverDado(""); //COD_PART

            this.EscreverDado(this.CTe.ModeloDocumentoFiscal.Numero); //COD_MOD
            this.EscreverCodigoDaSituacao(); //COD_SIT
            this.EscreverDado(this.CTe.Serie.Numero.ToString()); //SER
            this.EscreverDado(""); //SUB
            this.EscreverDado(this.CTe.Numero.ToString()); //NUM_DOC

            if (!this.CTe.Status.Equals("I"))
                this.EscreverDado(this.CTe.Chave); //CHV_CTE
            else
                this.EscreverDado(""); //CHV_CTE

            if (!this.CTe.Status.Equals("D") && !this.CTe.Status.Equals("I") && !this.CTe.Status.Equals("C"))
            {
                this.EscreverDado(this.CTe.DataEmissao); //DT_DOC
                this.EscreverDado(this.CTe.DataEmissao); //DT_A_P
                this.EscreverDado(this.CTe.TipoCTE.ToString("d")); //TP_CT-e
                this.EscreverDado(this.CTe.ChaveCTESubComp); //CHV_CTE_REF
                this.EscreverDado(this.CTe.ValorFrete); //VL_DOC
                this.EscreverDado(""); //VL_DESC
                this.EscreverIndicadorTipoFrete(); //IND_FRT
                this.EscreverDado(this.CTe.ValorPrestacaoServico); //VL_SERV
                this.EscreverDado(this.CTe.BaseCalculoICMS); //VL_BC_ICMS
                this.EscreverDado(this.CTe.ValorICMS); //VL_ICMS
                this.EscreverDado(""); //VL_NT
                this.EscreverDado(""); //COD_INF
                this.EscreverDado(""); //COD_CTA

                if (versaoAtoCOTEPE >= 12) //2018
                {
                    this.EscreverDado(CTe.LocalidadeInicioPrestacao.CodigoIBGE); //COD_MUN_ORIG
                    this.EscreverDado(CTe.LocalidadeTerminoPrestacao.CodigoIBGE); //COD_MUN_DEST
                }
            }
            else
            {
                this.EscreverDado(""); //DT_DOC
                this.EscreverDado(""); //DT_A_P
                this.EscreverDado(""); //TP_CT-e
                this.EscreverDado(""); //CHV_CTE_REF
                this.EscreverDado(""); //VL_DOC
                this.EscreverDado(""); //VL_DESC
                this.EscreverDado(""); //IND_FRT
                this.EscreverDado(""); //VL_SERV
                this.EscreverDado(""); //VL_BC_ICMS
                this.EscreverDado(""); //VL_ICMS
                this.EscreverDado(""); //VL_NT
                this.EscreverDado(""); //COD_INF
                this.EscreverDado(""); //COD_CTA

                if (versaoAtoCOTEPE >= 12) //2018
                {
                    this.EscreverDado(""); //COD_MUN_ORIG
                    this.EscreverDado(""); //COD_MUN_DEST
                }
            }

            this.EscreverDado("_SPED_PH_"); //IDE_PH
            this.EscreverDado(this.CTe.Codigo.ToString("D"),4); //HIST_IDE
            this.EscreverDado(""); //COD_CCD
            this.EscreverDado(""); //COD_CCC
            this.EscreverDado(this.CTe.CFOP.CodigoCFOP.ToString("D")); //CFOP
            this.EscreverDado(""); //CFOP_DOC
            this.EscreverDado(2); //TIP_FAT
            this.EscreverDado(""); //OBS
            this.EscreverDado(this.CTe.Recebedor?.Localidade.Estado.Sigla ?? this.CTe.Destinatario?.Localidade.Estado.Sigla ?? "", 2); //SIG_EST_DEST
            this.EscreverDado(this.CTe.Expedidor?.Localidade.CodigoIBGE.ToString("D") ?? this.CTe.Remetente?.Localidade.CodigoIBGE.ToString("D") ?? ""); //MUN_ORIG
            this.EscreverDado(this.CTe.TipoServico == Enumeradores.TipoServico.SubContratacao ? "1" : "0"); //SERV_SUBCONTRAT

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        private void EscreverCodigoDaSituacao()
        {
            if (this.CTe.Status.Equals("D"))
                this.EscreverDado("04"); //NF-e, NFC-e ou CT-e - denegado
            else if (this.CTe.Status.Equals("I"))
                this.EscreverDado("05"); //NF-e, NFC-e ou CT-e - Numeração inutilizada
            else if (this.CTe.Status.Equals("C"))
                this.EscreverDado("02"); //Documento cancelado
            else if (this.CTe.Status.Equals("A") && this.CTe.TipoCTE == Enumeradores.TipoCTE.Complemento)
                this.EscreverDado("06"); //Documento Fiscal Complementar
            else
                this.EscreverDado("00"); //Documento regular
        }

        private void EscreverIndicadorTipoFrete()
        {
            if (this.CTe.TipoTomador == Enumeradores.TipoTomador.Destinatario || this.CTe.TipoTomador == Enumeradores.TipoTomador.Remetente)
                this.EscreverDado("1"); //Por conta do destinatário/remetente
            else
                this.EscreverDado("2");
        }

        #endregion
    }
}
