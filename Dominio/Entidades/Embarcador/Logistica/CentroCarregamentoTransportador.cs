using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES", EntityName = "CentroCarregamentoTransportador", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador", NameType = typeof(CentroCarregamentoTransportador))]
    public class CentroCarregamentoTransportador : EntidadeBase, IEquatable<CentroCarregamentoTransportador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES_CLIENTES_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LocalidadesDestino", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES_LOCALIDADES_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> LocalidadesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposDeCarga", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES_TIPOS_DE_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposDeCarga { get; set; }


        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(CentroCarregamentoTransportador other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
