using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CONTAINER", EntityName = "MontagemContainer", Name = "Dominio.Entidades.Embarcador.WMS.MontagemContainer", NameType = typeof(MontagemContainer))]
    public class MontagemContainer : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTC_ID_SEQUENCIAL", TypeType = typeof(int), NotNull = false, UniqueKey = "UK_MONTAGEM_CONTAINER_ID_SEQUENCIAL")]
        public virtual int Id { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "MTC_STATUS", TypeType = typeof(StatusMontagemContainer), NotNull = false)]
        public virtual StatusMontagemContainer Status { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "MTC_NUMERO_BOOKING", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.ContainerTipo TipoContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Porto", Column = "POT_ORIGEM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Porto", Column = "POT_DESTINO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Container Container { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Montagem Container {Id}";
            }
        }
    }
}