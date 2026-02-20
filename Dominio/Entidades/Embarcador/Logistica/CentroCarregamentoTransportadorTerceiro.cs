using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES_TERCEIROS", EntityName = "CentroCarregamentoTransportadorTerceiro", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro", NameType = typeof(CentroCarregamentoTransportadorTerceiro))]
    public class CentroCarregamentoTransportadorTerceiro : EntidadeBase, IEquatable<CentroCarregamentoTransportadorTerceiro>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Transportador { get; set; }

        /*
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }
        */

        /*
        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES_TERCEIROS_CLIENTES_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }
        */

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(CentroCarregamentoTransportadorTerceiro other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
