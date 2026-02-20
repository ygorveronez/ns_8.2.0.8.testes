namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_COMPONENTES_FRETE_AUXILIAR", EntityName = "CargaComponentesFreteAuxiliar", Name = "Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar", NameType = typeof(CargaComponentesFreteAuxiliar))]
    public class CargaComponentesFreteAuxiliar : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "CCA_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorComponente { get; set; }

        public virtual bool Equals(CargaComponentesFrete other)
        {
            return other.Codigo == Codigo;
        }

        public override int GetHashCode()
        {
            int hashCodigo = Codigo <= 0 ? 0 : Codigo.GetHashCode();

            return hashCodigo;
        }

    }
}
