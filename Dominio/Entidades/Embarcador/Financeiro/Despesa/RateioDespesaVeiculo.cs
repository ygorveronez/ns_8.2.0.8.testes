using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro.Despesa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_DESPESA_VEICULO", EntityName = "RateioDespesaVeiculo", Name = "Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo", NameType = typeof(RateioDespesaVeiculo))]
    public class RateioDespesaVeiculo : EntidadeBase
    {
        public RateioDespesaVeiculo()
        {
            DataLancamento = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TRD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDespesaFinanceira", Column = "TID_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDespesaFinanceira TipoDespesa { get; set; }

        /// <summary>
        /// Utilizado para rateio específico para um centro de resultado e não dos veículos.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_TIPO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_RATEAR_PELO_PERCENTUAL_FATURADO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearPeloPercentualFaturadoDoVeiculoNoPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRD_RATEAR_DESPESA_UMA_VEZ_POR_MES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearDespesaUmaVezPorMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaMesRateio", Column = "TRD_DIA_MES_RATEIO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaMesRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Origem", Column = "TRD_ORIGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemRateioDespesaVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemRateioDespesaVeiculo? Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentoFinanceiro", Column = "MOV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MovimentoFinanceiro MovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFinanciamento", Column = "CFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFinanciamento ContratoFinanciamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Infracao", Column = "INF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.Infracao Infracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaTMS DocumentoEntradaTMS { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "SegmentosVeiculos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RATEIO_DESPESA_VEICULO_SEGMENTO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TRD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SegmentoVeiculo", Column = "VSE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> SegmentosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RATEIO_DESPESA_VEICULO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TRD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RateioDespecaVeiculoVeiculo", Column = "RDV_CODIGO")]
        public virtual IList<Despesa.RateioDespesaVeiculoValorVeiculo> Veiculos { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RATEIO_DESPESA_VEICULO_CENTRO_RESULTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TRD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RateioDespecaVeiculoCentroResultado", Column = "RDC_CODIGO")]
        public virtual IList<Despesa.RateioDespesaVeiculoValorCentroResultado> CentroResultados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Colaborador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotorista.PagamentoMotoristaTMS PagamentoMotorista { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
