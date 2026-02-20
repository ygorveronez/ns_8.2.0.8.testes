using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRETE", EntityName = "Frete", Name = "Dominio.Entidades.Frete", NameType = typeof(Frete))]
    public class Frete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "FRE_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida UnidadeDeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "FRE_VALOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "FRE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "FRE_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrosValores", Column = "FRE_OUTROS_VALORES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal OutrosValores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeExcedente", Column = "FRE_QUANTIDADE_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorExcedente", Column = "FRE_VALOR_EXCEDENTE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "FRE_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "FRE_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FRE_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "FRE_TIPO_PAGAMENTO", TypeType = typeof(Dominio.Enumeradores.TipoPagamentoFrete), Length = 1, NotNull = true)]
        public virtual Dominio.Enumeradores.TipoPagamentoFrete TipoPagamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCliente", Column = "FRE_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoFrete", Column = "FRE_VALOR_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMinimo", Column = "FRE_VALOR_MINIMO_TIPO", TypeType = typeof(Enumeradores.TipoFreteValor), NotNull = false)]
        public virtual Enumeradores.TipoFrete TipoMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAdValorem", Column = "FRE_ADVALOREM", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGris", Column = "FRE_GRIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualGris { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescarga", Column = "FRE_VALOR_DESCARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagioPerc", Column = "FRE_PEDAGIO_PERC", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagioPerc { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiICMS", Column = "FRE_INCLUI_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.IncluiICMSFrete IncluiICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirPedagioBC", Column = "FRE_INCLUI_PEDAGIO_BC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirPedagioBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirOutrosBC", Column = "FRE_INCLUI_OUTROS_BC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirOutrosBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirSeguroBC", Column = "FRE_INCLUI_SEGURO_BC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirSeguroBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirDescargaBC", Column = "FRE_INCLUI_DESCARGA_BC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirDescargaBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirGrisBC", Column = "FRE_INCLUI_GRIS_BC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirGrisBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirAdValoremBC", Column = "FRE_INCLUI_ADVALOREM_BC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirAdValoremBC { get; set; }

        //Campo esta sendo utilizado apenas no calculo de frete feito pelo Javascript (Para o cliente Estrela de Davi)
        [NHibernate.Mapping.Attributes.Property(0, Name = "FreteMinimoComICMS", Column = "FRE_FRETE_MINIMO_COM_ICMS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao FreteMinimoComICMS { get; set; }
    }
}
