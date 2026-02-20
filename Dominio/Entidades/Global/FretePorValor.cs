using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRETE_VALOR", EntityName = "FretePorValor", Name = "Dominio.Entidades.FretePorValor", NameType = typeof(FretePorValor))]
    public class FretePorValor   : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FRV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "FRV_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeDestino { get; set; }
                                                                     
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoFrete", Column = "FRV_VALOR_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSobreNF", Column = "FRV_PERC_NOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualSobreNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "FRV_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "FRV_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "FRV_TIPO", TypeType = typeof(Enumeradores.TipoFreteValor), NotNull = true)]
        public virtual Enumeradores.TipoFreteValor Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FRV_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "FRV_TIPO_PAGAMENTO", TypeType = typeof(Dominio.Enumeradores.TipoPagamentoFrete), Length = 1, NotNull = true)]
        public virtual Dominio.Enumeradores.TipoPagamentoFrete TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "FRV_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRateio", Column = "FRV_TIPO_RATEIO", TypeType = typeof(Dominio.Enumeradores.TipoRateioTabelaFreteValor), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.TipoRateioTabelaFreteValor TipoRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiICMS", Column = "FRV_INCLUI_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.IncluiICMSFrete IncluiICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirPedagioBC", Column = "FRV_INCLUI_PEDAGIO_BC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirPedagioBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "FRV_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCliente", Column = "FRV_TIPO_CLIENTE", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoCliente { get; set; }
    }
}
