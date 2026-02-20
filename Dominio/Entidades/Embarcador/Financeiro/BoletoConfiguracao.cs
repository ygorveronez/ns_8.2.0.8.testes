using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_CONFIGURACAO", EntityName = "BoletoConfiguracao", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao", NameType = typeof(BoletoConfiguracao))]
    public class BoletoConfiguracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BoletoBanco", Column = "BCF_BANCO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco BoletoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoBanco", Column = "BCF_DESCRICAO_BANCO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBanco", Column = "BCF_NUMERO_BANCO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoBanco", Column = "BCF_DIGITO_BANCO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DigitoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TagBanco", Column = "BCF_TAG_BANCO", TypeType = typeof(int), NotNull = false)]
        public virtual int TagBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TamanhoMaximoNossoNumero", Column = "BCF_TAMANHO_MAXIMO_NOSSO_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int TamanhoMaximoNossoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProximoNumeroNossoNumero", Column = "BCF_PROXIMO_NUMERO_NOSSO_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int ProximoNumeroNossoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAgencia", Column = "BCF_NUMERO_AGENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "BCF_DIGITO_AGENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTransmissao", Column = "BCF_CODIGO_TRANSIMSSAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoTransmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCedente", Column = "BCF_CODIGO_CEDENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaracteristicaTitulo", Column = "BCF_CARACTERISTICA_TITULO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo CaracteristicaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "BCF_NUMERO_CONTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoConta", Column = "BCF_DIGITO_CONTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DigitoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConvenio", Column = "BCF_NUMERO_CONVENIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroConvenio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Modalidade", Column = "BCF_MODALIDADE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Modalidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ResponsavelEmissao", Column = "BCF_RESPONSAVEL_EMISSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao ResponsavelEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TagTitulo", Column = "BCF_TAG_TITULO", TypeType = typeof(int), NotNull = false)]
        public virtual int TagTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarteira", Column = "BCF_TIPO_CARTEIRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCarteira), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCarteira TipoCarteira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EspecieTitulo", Column = "BCF_ESPECIE_TITULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string EspecieTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CarteiraTitulo", Column = "BCF_CARTEIRA_TITULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CarteiraTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JurosAoMes", Column = "BCF_JUROS_AM_TITULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal JurosAoMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasProtesto", Column = "BCF_DIAS_PROTEST_TITULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DiasProtesto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMulta", Column = "BCF_PERCENTUAL_MULTA_TITULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualMulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBancoTitulo", Column = "BCF_CODIGO_BANCO_TITULO", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string CodigoBancoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProtesto", Column = "BCF_CODIGO_PROTESTO_TITULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoProtesto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMulta", Column = "BCF_CODIGO_MULTA_TITULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoMulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoRemessa", Column = "BCF_CAMINHO_REMESSA_TITULO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalPagamento", Column = "BCF_LOCAL_PAGAMENTO_TITULO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string LocalPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Instrucao", Column = "BCF_INSTRUCAO_TITULO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Instrucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BoletoTipoCNAB", Column = "BCF_TIPO_CNAB", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB BoletoTipoCNAB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BoletoTipoLayout", Column = "BCF_TIPO_LAYOUT", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout BoletoTipoLayout { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_BENEFICIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Beneficiario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_SACADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Sacador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aceite", Column = "BCF_ACEITE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Aceite { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_LIQUIDACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoLiquidacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_BAIXA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoBaixa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_TARIFA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoTarifa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_JUROS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoJuros { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_DESCONTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_PORTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Portador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroInicialSequenciaRemessa", Column = "BCF_NUMERO_INICIAL_SEQUENCIA_REMESSA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroInicialSequenciaRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiquidarComValorIntegral", Column = "BCF_LIQUIDAR_COM_VALOR_INTEGRAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiquidarComValorIntegral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaConfiguracaoPagamentoEletronico", Column = "BCF_UTILIZA_CONFIGURACAO_PAGAMENTO_ELETRONICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaConfiguracaoPagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescimoDiaDataMoraJuros", Column = "BCF_ACRESCIMO_DIA_DATA_MORA_JUROS", TypeType = typeof(int), NotNull = false)]
        public virtual int AcrescimoDiaDataMoraJuros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescimoDiaDataMulta", Column = "BCF_ACRESCIMO_DIA_DATA_MULTA", TypeType = typeof(int), NotNull = false)]
        public virtual int AcrescimoDiaDataMulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoInstrucaoMovimento", Column = "BFC_CODIGO_INSTRUCAO_MOVIMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoInstrucaoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmail", Column = "BCF_ASSUNTO_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string AssuntoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemEmail", Column = "BCF_MENSAGEM_EMAIL", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string MensagemEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgenciaConta", Column = "BCF_DIGITO_AGENCIA_CONTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DigitoAgenciaConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "BCF_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFinalidadeTED", Column = "BCF_CODIGO_FINALIDADE_TED", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoFinalidadeTED { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDiasRecebimentoAposVencimento", Column = "BCF_QUANTIDADE_DIAS_RECEBIMENTO_APOS_VENCIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasRecebimentoAposVencimento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroBanco + " - " + this.DescricaoBanco;
            }
        }

        public virtual string DescricaoBoletoTipoCNAB
        {
            get
            {
                switch (this.BoletoTipoCNAB)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB240:
                        return "CNAB 240";                        
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB400:
                        return "CNAB 400";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB240PIX:
                        return "CNAB 240 PIX";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB500PIX:
                        return "CNAB 500 PIX";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB750PIX:
                        return "CNAB 750 PIX";                        
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoBoletoTipoLayout
        {
            get
            {
                switch (this.BoletoTipoLayout)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.Boleto10:
                        return "Boleto 1.0";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.Boleto20:
                        return "Boleto 2.0";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.Boleto30Alpha:
                        return "Boleto 3.0 Alpha";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoCarne:
                        return "Boleto Carne";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoFatura:
                        return "Boleto Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoFR:
                        return "Boleto FR";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoBeneficiario:
                        return "Boleto C/ Beneficiário";
                    default:
                        return "";
                }
            }
        }

        public virtual string ArquivoBoletoTipoLayout
        {
            get
            {
                switch (this.BoletoTipoLayout)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.Boleto10:
                        return "Boleto.fr3";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.Boleto20:
                        return "BoletoNovo.fr3";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.Boleto30Alpha:
                        return "BoletoNovoAlpha.fr3";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoCarne:
                        return "BoletoCarne.fr3";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoFatura:
                        return "BoletoFatura.fr3";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoFR:
                        return "BoletoFR.fr3";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout.BoletoBeneficiario:
                        return "BoletoBeneficiario.fr3";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCaracteristicaTitulo
        {
            get
            {
                switch (this.CaracteristicaTitulo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo.Caucionada:
                        return "Caucionada";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo.Descontada:
                        return "Descontada";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo.Simples:
                        return "Simples";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo.Vendor:
                        return "Vendor";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo.Vinculada:
                        return "Vinculada";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoResponsavelEmissao
        {
            get
            {
                switch (this.ResponsavelEmissao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao.BancoEmite:
                        return "Banco Emite";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao.BancoNaoReemite:
                        return "Banco Não Reemite";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao.BanroReemite:
                        return "Banco Reemite";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao.Emitente:
                        return "Emitente";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoCarteira
        {
            get
            {
                switch (this.TipoCarteira)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCarteira.Registrada:
                        return "Registrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCarteira.Simples:
                        return "Simples";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(BoletoConfiguracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
