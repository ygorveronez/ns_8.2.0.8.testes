using System;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_PRECO_COMBUSTIVEL", EntityName = "TabelaPrecoCombustivel", Name = "Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel", NameType = typeof(TabelaPrecoCombustivel))]
    public class TabelaPrecoCombustivel : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOleo", Column = "TOL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOleo TipoOleo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigencia", Column = "TPC_DATA_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorExterno", Column = "TPC_VALOR_EXTERNO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorExterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorInterno", Column = "TPC_VALOR_INTERNO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorInterno { get; set; }
    }
}