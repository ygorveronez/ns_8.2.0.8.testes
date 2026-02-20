namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRE_CALCULO_FRETE", EntityName = "CargaPreCalculoFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete", NameType = typeof(CargaPreCalculoFrete))]

    public class CargaPreCalculoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CPF_VALOR_FRETE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorFrete { get; set; }
    }
}
