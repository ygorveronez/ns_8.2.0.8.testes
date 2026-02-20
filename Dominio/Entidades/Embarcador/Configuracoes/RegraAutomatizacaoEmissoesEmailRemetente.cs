namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_REGRA_AUTOMATIZACAO_EMISSOES_EMAIL_REMETENTE", EntityName = "RegraAutomatizacaoEmissoesEmailRemetente", Name = "Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente", NameType = typeof(RegraAutomatizacaoEmissoesEmailRemetente))]
    public class RegraAutomatizacaoEmissoesEmailRemetente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutomatizacaoEmissoesEmail", Column = "RAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraAutomatizacaoEmissoesEmail RegraAutomatizacaoEmissoesEmail { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        public virtual string Descricao
        {
            get { return RegraAutomatizacaoEmissoesEmail.Descricao + " - " + Remetente.Descricao; }
        }
    }
}