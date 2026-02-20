namespace Dominio.Entidades.Embarcador.Cargas.ValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_VALE_PEDAGIO_DADOS_COMPRA_PRACA", EntityName = "CargaValePedagioDadosCompraPraca", Name = "Dominio.Entidades.Embarcador.Cargas.CargaValePedagioDadosCompraPraca", NameType = typeof(CargaValePedagioDadosCompraPraca))]
    public class CargaValePedagioDadosCompraPraca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConcessionariaCodigo", Column = "CPP_CONCESSIONARIA_CODIGO", TypeType = typeof(int), NotNull = false)]
        public virtual int ConcessionariaCodigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConcessionariaNome", Column = "CPP_CONCESSIONARIA_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ConcessionariaNome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPraca", Column = "CPP_PRACA_CODIGO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoPraca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomePraca", Column = "CPP_NOME_PRACA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomePraca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeRodovia", Column = "CPP_NOME_RODOVIA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeRodovia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEixos", Column = "CPP_NUMERO_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CPP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 11, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaValePedagioDadosCompra", Column = "CVD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaValePedagioDadosCompra CargaValePedagioDadosCompra { get; set; }
    }
}
