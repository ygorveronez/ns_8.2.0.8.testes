using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_PARAMETRO_BASE_CALCULO", EntityName = "ParametroBaseCalculoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete", NameType = typeof(ParametroBaseCalculoTabelaFrete))]
    public class ParametroBaseCalculoTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoObjeto", Column = "TBC_CODIGO_OBJETO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoObjeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TBC_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_OBSERVACAO_TERCEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirObservacaoCTe", Column = "TBC_IMPRIMIR_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirObservacaoCTe { get; set; }

        /// <summary>
        /// 
        /// Utilizado para o ajuste da tabela de frete (não é utilizado para cálculo).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoGarantidoOriginal", Column = "TBC_VALOR_MINIMO_GARANTIDO_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoGarantidoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoGarantido", Column = "TBC_VALOR_MINIMO_GARANTIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoGarantido { get; set; }

        /// <summary>
        /// Utilizado para o ajuste da tabela de frete (não é utilizado para cálculo).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMaximoOriginal", Column = "TBC_VALOR_MAXIMO_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMaximo", Column = "TBC_VALOR_MAXIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_BASE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_BASE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_ENTREGA_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorEntregaExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorEntregaExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_PACOTE_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPacoteExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_PACOTE_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPacoteExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_PALLET_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPalletExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_PALLET_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPalletExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_QUILOMETRAGEM_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuilometragemExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuilometragemExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_PESO_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPesoExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_PESO_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPesoExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_AJUDANTE_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAjudanteExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_HORA_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorHoraExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAjudanteExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_PERCENTUAL_PAGAMENTO_AGREGADO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ItensBaseCalculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Inverse = true, Table = "T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemParametroBaseCalculoTabelaFrete", Column = "TPI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> ItensBaseCalculo { get; set; }

        public virtual ParametroBaseCalculoTabelaFrete Clonar()
        {
            return (ParametroBaseCalculoTabelaFrete)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get { return ""; }
        }
    }
}
