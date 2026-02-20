using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRETE_TIPO_VEICULO", EntityName = "FretePorTipoDeVeiculo", Name = "Dominio.Entidades.FretePorTipoDeVeiculo", NameType = typeof(FretePorTipoDeVeiculo))]
    public class FretePorTipoDeVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FTV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "FTV_CLIENTE_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "FTV_CLIENTE_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "FTV_LOCALIDADE_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade CidadeOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "FTV_LOCALIDADE_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade CidadeDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoVeiculo", Column = "VTI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoVeiculo TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "FTV_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "FTV_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarPedagioBcICMS", Column = "FTV_ADICIONAR_PEDAGIO_BC_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao AdicionarPedagioBcICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescarga", Column = "FTV_VALOR_DESCARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarDescargaBcICMS", Column = "FTV_ADICIONAR_DESCARBA_BC_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao AdicionarDescargaBcICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGris", Column = "FTV_PERCENTUAL_GRIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualGris { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarGrisBcICMS", Column = "FTV_ADICIONAR_GRIS_BC_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao AdicionarGrisBcICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAdValorem", Column = "FTV_ADVALORE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdValorem", Column = "FTV_VALOR_ADVALORE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarAdValoremBcICMS", Column = "FTV_ADICIONAR_ADVALORE_BC_ICMS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao AdicionarAdValoremBcICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiICMS", Column = "FTV_INCLUI_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.IncluiICMSFrete IncluiICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "FTV_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "FTV_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FTV_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "FTV_TIPO_PAGAMENTO", TypeType = typeof(Dominio.Enumeradores.TipoPagamentoFrete), Length = 1, NotNull = true)]
        public virtual Dominio.Enumeradores.TipoPagamentoFrete TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "FTV_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

    }
}
