using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_REGRA_AUTOMATIZACAO_EMISSOES_EMAIL", EntityName = "RegraAutomatizacaoEmissoesEmail", Name = "Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail", NameType = typeof(RegraAutomatizacaoEmissoesEmail))]
    public class RegraAutomatizacaoEmissoesEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RAM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailDestino", Column = "RAM_EMAIL_DESTINO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string EmailDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RAM_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Remetentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AUTOMATIZACAO_EMISSOES_EMAIL_REMETENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegraAutomatizacaoEmissoesEmailRemetente", Column = "RAR_CODIGO")]
        public virtual ICollection<RegraAutomatizacaoEmissoesEmailRemetente> Remetentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Destinatarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AUTOMATIZACAO_EMISSOES_EMAIL_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegraAutomatizacaoEmissoesEmailDestinatario", Column = "RAD_CODIGO")]
        public virtual ICollection<RegraAutomatizacaoEmissoesEmailDestinatario> Destinatarios { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return string.Empty;
                }
            }
        }
    }
}