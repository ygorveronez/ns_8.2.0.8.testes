using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REENVIO_INTEGRACAO_EDI", EntityName = "ReenvioIntegracaoEDI", Name = "Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI", NameType = typeof(ReenvioIntegracaoEDI))]
    public class ReenvioIntegracaoEDI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "RIE_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REENVIO_INTEGRACAO_EDI_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.Carga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Layouts", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REENVIO_INTEGRACAO_EDI_LAYOUT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LayoutEDI", Column = "LAY_CODIGO")]
        public virtual IList<Dominio.Entidades.LayoutEDI> Layouts { get; set; }
                
        public virtual string Descricao
        {
            get
            {
                return "Envio de - " + this.Usuario.Nome;
            }
        }

        public virtual bool Equals(CargaPedido other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
