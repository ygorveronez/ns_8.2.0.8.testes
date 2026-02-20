using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_RESTRICAO_FILA_CARREGAMENTO", EntityName = "ClienteRestricaoFilaCarregamento", Name = "Dominio.Entidades.ClienteRestricaoFilaCarregamento", NameType = typeof(ClienteRestricaoFilaCarregamento))]
    public class ClienteRestricaoFilaCarregamento : EntidadeBase, IEquatable<ClienteRestricaoFilaCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CRF_TIPO", TypeType = typeof(Enumeradores.TipoTomador), NotNull = true)]
        public virtual Enumeradores.TipoTomador Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        public virtual bool Equals(ClienteRestricaoFilaCarregamento other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
