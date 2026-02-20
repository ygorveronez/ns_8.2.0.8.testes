namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_REGRA_AUTOMATIZACAO_EMISSOES_EMAIL_DESTINATARIO", EntityName = "RegraAutomatizacaoEmissoesEmailDestinatario", Name = "Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario", NameType = typeof(RegraAutomatizacaoEmissoesEmailDestinatario))]
    public class RegraAutomatizacaoEmissoesEmailDestinatario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutomatizacaoEmissoesEmail", Column = "RAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraAutomatizacaoEmissoesEmail RegraAutomatizacaoEmissoesEmail { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        public virtual string Descricao
        {
            get { return RegraAutomatizacaoEmissoesEmail.Descricao + " - " + Destinatario.Descricao; }
        }
    }
}
