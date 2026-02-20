using Dominio.Interfaces.Embarcador.Entidade;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_TOKEN_GERADO_TRANSPORTADOR", EntityName = "SolicitacaoTokenTransportador", Name = "Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador", NameType = typeof(SolicitacaoTokenTransportador))]
    public class SolicitacaoTokenTransportador : EntidadeBase,IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "STT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "STT_TOKEN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Token { get; set; }
          
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "STT_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "SolicitacaoToken", Column = "STO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken SolicitacaoToken { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

    }
}

