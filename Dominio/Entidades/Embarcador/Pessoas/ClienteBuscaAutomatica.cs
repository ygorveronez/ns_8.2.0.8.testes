using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_BUSCA_AUTOMATICA", EntityName = "ClienteBuscaAutomatica", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica", NameType = typeof(ClienteBuscaAutomatica))]
    public class ClienteBuscaAutomatica : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica>
    {
        public ClienteBuscaAutomatica() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CBA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CBA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CBA_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParticipante", Column = "CBA_TIPO_PARTICIPANTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoParticipante), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoParticipante TipoParticipante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        public virtual bool Equals(ClienteBuscaAutomatica other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
