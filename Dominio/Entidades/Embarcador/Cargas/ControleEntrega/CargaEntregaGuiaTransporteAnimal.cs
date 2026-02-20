namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_GTA", EntityName = "CargaEntregaGuiaTransporteAnimal", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal", NameType = typeof(CargaEntregaGuiaTransporteAnimal))]
    public class CargaEntregaGuiaTransporteAnimal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GTA_CODIGO_BARRAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoBarras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GTA_NUMERO_NF", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GTA_SERIE", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GTA_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        public virtual string Descricao => $"GTA - {(NumeroNotaFiscal ?? string.Empty)} - " + (this.CargaEntrega?.Descricao ?? string.Empty);



    }
}

