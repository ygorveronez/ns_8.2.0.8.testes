using Dominio.Entidades.Embarcador.PagamentoMotorista;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE_TRANSPORTADORAS", EntityName = "ModalidadeTransportadoraPessoas", Name = "Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoras", NameType = typeof(ModalidadeTransportadoraPessoas))]
    public class ModalidadeTransportadoraPessoas : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>
    {
        public ModalidadeTransportadoraPessoas() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Valor Percentual que desconta do CT-e quando Subcontrata o Terceiro
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "MOT_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDesconto { get; set; }

        /// <summary>
        /// Valor Percentual que cobra sobre o valor do CT-e quando é subcontratado pelo Terceiro
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobranca", Column = "MOT_PERCENTUAL_COBRANCA", TypeType = typeof(decimal), Scale = 5, Precision = 19, NotNull = true)]
        public virtual decimal PercentualCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_PERCENTUAL_ADIANTAMENTO_FRETES_TERCEIRO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAdiantamentoFretesTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_PERCENTUAL_ABASTECIMENTO_FRETES_TERCEIRO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAbastecimentoFretesTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RNTRC", Column = "MOT_RNTRC", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string RNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTe", Column = "MOT_OBS_CTE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCIOT", Column = "MOT_GERAR_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadePessoas", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas ModalidadePessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportador", Column = "MOT_TIPO_TRANSPORTADOR", TypeType = typeof(TipoProprietarioVeiculo), NotNull = false)]
        public virtual TipoProprietarioVeiculo TipoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoContratoFrete", Column = "MOT_OBSERVACAO_CONTRATO_FRETE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_EXIGE_CANHOTO_FECHAMENTO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeCanhotoFechamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_TEXTO_ADICIONAL_CONTRATO_FRETE", TypeType = typeof(string), Length = 100000, NotNull = false)]
        public virtual string TextoAdicionalContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_RETER_IMPOSTOS_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ReterImpostosContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_DIAS_VENCIMENTO_ADIANTAMENTO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasVencimentoAdiantamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_DIAS_VENCIMENTO_SALDO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasVencimentoSaldoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_CODIGO_INSS", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_DATA_EMISSAO_RNTRC", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoRNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_DATA_VENCIMENTO_RNTRC", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoRNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_FAVORECIDO_CIOT", TypeType = typeof(TipoFavorecidoCIOT), NotNull = false)]
        public virtual TipoFavorecidoCIOT? TipoFavorecidoCIOT { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_PAGAMENTO_CIOT", TypeType = typeof(TipoPagamentoCIOT), NotNull = false)]
        public virtual TipoPagamentoCIOT? TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_NUMERO_CARTAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCartao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_TIPO_GERACAO_CIOT", TypeType = typeof(TipoGeracaoCIOT), NotNull = false)]
        public virtual TipoGeracaoCIOT? TipoGeracaoCIOT { get; set; }

        /*
        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_FORMA_ABERTURA_CIOT_PERIODO", TypeType = typeof(FormaAberturaCIOTPeriodo), NotNull = false)]
        public virtual FormaAberturaCIOTPeriodo? FormaAberturaCIOTPeriodo { get; set; }
        */

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoTributaria", Column = "MOT_CODIGO_INTEGRACAO_TRIBUTARIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracaoTributaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_TIPO_QUITACAO_CIOT", TypeType = typeof(TipoQuitacaoCIOT), NotNull = false)]
        public virtual TipoQuitacaoCIOT? TipoQuitacaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_TIPO_ADIANTAMENTO_CIOT", TypeType = typeof(TipoQuitacaoCIOT), NotNull = false)]
        public virtual TipoQuitacaoCIOT? TipoAdiantamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_TIPO_PAGAMENTO_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete? TipoPagamentoContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "MOT_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_NAO_SOMAR_VALOR_PEDAGIO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoSomarValorPedagioContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarPagamentoTerceiro", Column = "MOT_GERAR_PAGAMENTO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPagamentoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_QUANTIDADE_DEPENDENTES", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeDependentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_CODIGO_PROVEDOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerceiro", Column = "TPT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro TipoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOT_HABILITAR_DATA_FIXA_VENCIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarDataFixaVencimento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ModalidadePessoas?.DescricaoTipoModalidade ?? string.Empty;
            }
        }

        public virtual bool Equals(ModalidadeTransportadoraPessoas other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoTipoTransportador
        {
            get
            {
                switch (TipoTransportador)
                {
                    case TipoProprietarioVeiculo.Outros:
                        return "Outros";
                    case TipoProprietarioVeiculo.TACAgregado:
                        return "TAC Agregado";
                    case TipoProprietarioVeiculo.TACIndependente:
                        return "TAC Independente";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual bool? ReterImpostosContratoFreteWithTipoTerceiro
        {
            get
            {
                if (this.TipoTerceiro != null && this.TipoTerceiro.ReterImpostosContratoFrete != null)
                {
                    return this.TipoTerceiro.ReterImpostosContratoFrete;
                }
                return this.ReterImpostosContratoFrete;
            }
        }

        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOTWithTipoTerceiro
        {
            get
            {
                if (this.TipoTerceiro != null && this.TipoTerceiro.ConfiguracaoCIOT != null)
                {
                    return this.TipoTerceiro.ConfiguracaoCIOT;
                }
                return this.ConfiguracaoCIOT;
            }
        }
    }
}
