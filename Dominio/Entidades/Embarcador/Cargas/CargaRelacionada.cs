namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_RELACIONADA", EntityName = "CargaRelacionada", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaRelacionada", NameType = typeof(CargaRelacionada))]
    public class CargaRelacionada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
          
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_RELACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga CargaRelacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Relacionamento {Carga.CodigoCargaEmbarcador} - {CargaRelacao.CodigoCargaEmbarcador}";
            }
        }
    }
}
