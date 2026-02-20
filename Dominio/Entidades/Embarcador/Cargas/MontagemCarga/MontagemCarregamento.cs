namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CARREGAMENTO", EntityName = "MontagemCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento", NameType = typeof(MontagemCarregamento))]
    public class MontagemCarregamento : EntidadeBase
    { 
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]

        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento Carregamento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

    }
}
