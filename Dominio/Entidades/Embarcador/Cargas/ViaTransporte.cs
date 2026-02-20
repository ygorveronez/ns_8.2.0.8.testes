using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VIA_TRANSPORTE", EntityName = "ViaTransporte", Name = "Dominio.Entidades.Embarcador.Cargas.ViaTransporte", NameType = typeof(ViaTransporte))]
    public class ViaTransporte : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.ViaTransporte>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TVT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TVT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoEnvio", Column = "TVT_CODIGO_INTEGRACAO_ENVIO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracaoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TVT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_PADRAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TVT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual bool Equals(ViaTransporte other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
