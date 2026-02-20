using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.EDI.CONEMB.v31
{
    public class CEM : Registro
    {
        #region Construtores

        public CEM(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.InformacaoCargaCTE informacaoCarga, List<Dominio.Entidades.ComponentePrestacaoCTE> componentesDaPrestacao, List<Dominio.Entidades.DocumentosCTE> notasFiscais)
            : base("322")
        {
            this.CTe = cte;
            this.InformacaoCarga = informacaoCarga;
            this.ComponentesDaPrestacao = componentesDaPrestacao;
            this.NotasFiscais = notasFiscais;

            if (this.CTe == null)
                throw new ArgumentNullException("cte", "O CT-e não pode ser nulo para gerar um registro CEM.");

            if (this.InformacaoCarga == null)
                throw new ArgumentNullException("informacaoCarga", "A informação da carga não pode ser nula para gerar um registro CEM.");

            if (this.ComponentesDaPrestacao == null || this.ComponentesDaPrestacao.Count() <= 0)
                throw new ArgumentNullException("componentesDaPrestacao", "Os componentes da prestação não podem ser nulos para gerar um registro CEM.");

            if (this.NotasFiscais == null || this.NotasFiscais.Count <= 0)
                throw new ArgumentNullException("informacaoCarga", "As notas fiscais não podem ser nulas para gerar um registro CEM.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }
        private Dominio.Entidades.InformacaoCargaCTE InformacaoCarga { get; set; }
        private List<Dominio.Entidades.ComponentePrestacaoCTE> ComponentesDaPrestacao { get; set; }
        private List<Dominio.Entidades.DocumentosCTE> NotasFiscais { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            for (var i = 0; i < this.NotasFiscais.Count(); i += 40)
            {
                this.EscreverDado(this.CTe.Empresa.RazaoSocial, 10); //2.		FILIAL EMISSORA DO CONHECIMENTO
                this.EscreverDado(this.CTe.Serie.Numero.ToString(), 5); //3.		SÉRIE DO CONHECIMENTO
                this.EscreverDado(this.CTe.Numero.ToString(), 12); //4.		NÚMERO DO CONHECIMENTO
                this.EscreverDado(this.CTe.DataEmissao); //5.		DATA DE EMISSÃO
                this.EscreverDado(this.CTe.TipoPagamento == Enumeradores.TipoPagamento.Pago ? "C" : "F"); //6.		CONDIÇÃO DE FRETE
                this.EscreverDado(this.InformacaoCarga.Quantidade, 5, 2); //7.		PESO TRANSPORTADO
                this.EscreverDado(this.CTe.ValorFrete, 13, 2); //8.		VALOR TOTAL DO FRETE
                this.EscreverDado(this.CTe.BaseCalculoICMS, 13, 2); //9.		BASE DE CÁLCULO PARA APURAÇÃO ICMS
                this.EscreverDado(this.CTe.AliquotaICMS, 2, 2); //10.		% DE TAXA DO ICMS
                this.EscreverDado(this.CTe.ValorICMS, 13, 2); //11.		VALOR DO ICMS
                this.EscreverDado(this.InformacaoCarga.Quantidade > 0 ? (this.CTe.ValorFrete / this.InformacaoCarga.Quantidade) : 0m, 13, 2); //12.		VALOR DO FRETE POR PESO/VOLUME
                this.EscreverDado((from obj in this.ComponentesDaPrestacao where obj.Nome.ToLower().Equals("valor frete") || obj.Nome.ToLower().Equals("frete valor") select obj.Valor).Sum(), 13, 2); //13.		FRETE VALOR
                this.EscreverDado(0m, 13, 2); //14.		VALOR SEC – CAT 
                this.EscreverDado(0m, 13, 2); //15.		VALOR ITR
                this.EscreverDado((from obj in this.ComponentesDaPrestacao where obj.Nome.ToLower().Contains("despacho") select obj.Valor).Sum(), 13, 2); //16.		VALOR DO DESPACHO
                this.EscreverDado((from obj in this.ComponentesDaPrestacao where obj.Nome.ToLower().Contains("pedagio") || obj.Nome.ToLower().Contains("pedágio") select obj.Valor).Sum(), 13, 2); //17.		VALOR DO PEDÁGIO
                this.EscreverDado((from obj in this.ComponentesDaPrestacao where obj.Nome.ToLower().Contains("seguro") select obj.Valor).Sum(), 13, 2); //18.		VALOR ADEME
                this.EscreverDado(this.CTe.CST.Equals("60") ? "1" : "2"); //19.		SUBSTITUIÇÃO TRIBUTÁRIA?
                this.EscreverDado(' ', 3); //20.		NATUREZA DE OPERAÇÃO
                this.EscreverDado(this.CTe.Empresa.CNPJ, 14); //21.		C.G.C. DO EMISSOR DO CONHECIMENTO
                this.EscreverDado(this.CTe.Remetente.CPF_CNPJ_SemFormato); //22.		C.G.C. DA EMBARCADORA

                for (var j = 0; j < 40; j++)
                {
                    if (this.NotasFiscais.Count() > (i + j))
                    {
                        if (!string.IsNullOrWhiteSpace(this.NotasFiscais[(i + j)].Serie))
                            this.EscreverDado(this.NotasFiscais[(i + j)].Serie, 3); //SÉRIE DA NOTA FISCAL
                        else if (!string.IsNullOrWhiteSpace(this.NotasFiscais[(i + j)].ChaveNFE))
                            this.EscreverDado(this.NotasFiscais[(i + j)].ChaveNFE.Substring(23, 2), 3); //SÉRIE DA NOTA FISCAL
                        else
                            this.EscreverDado(' ', 3); //SÉRIE DA NOTA FISCAL

                        this.EscreverDado(int.Parse(this.NotasFiscais[(i + j)].Numero), 8); //NÚMERO DA NOTA FISCAL
                    }
                    else
                    {
                        this.EscreverDado(' ', 3); //SÉRIE DA NOTA FISCAL
                        this.EscreverDado(' ', 8); //NÚMERO DA NOTA FISCAL
                    }
                }

                this.EscreverDado("I"); //103.		AÇÃO DO DOCUMENTO
                this.EscreverDado("N"); //104.		TIPO DO CONHECIMENTO
                this.EscreverDado(i == 0 ? "U" : "C", 1); //105.		INDICAÇÃO DE CONTINUIDADE/REPETIÇÃO DOS DADOS DO CONHECIMENTO, QUANDO HOUVER MAIS DE 40 NFs
                this.EscreverDado(this.CTe.CFOP.CodigoCFOP, 5); //106.		FILLER// Deve ir o c'odigo da CFOP - Cesar 25/01/2014
                //this.EscreverDado(' ', 5); //106.		FILLER

                this.FinalizarRegistro();
            }

            return this.StringRegistro.ToString();
        }

        #endregion
    }
}
