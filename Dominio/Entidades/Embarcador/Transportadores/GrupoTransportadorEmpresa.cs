namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_TRANSPORTADOR_EMPRESA", EntityName = "GrupoTransportadorEmpresa", Name = "Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa", NameType = typeof(GrupoTransportadorEmpresa))]
    public class GrupoTransportadorEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTransportador", Column = "GRT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoTransportador GrupoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        public virtual string Descricao 
        {
            get { return $"{GrupoTransportador.Descricao} - {Empresa.Descricao}"; }
        }
    }
}
