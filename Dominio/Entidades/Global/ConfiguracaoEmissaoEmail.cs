namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG_EMISSAO_EMAIL", EntityName = "ConfiguracaoEmissaoEmail", Name = "Dominio.Entidades.ConfiguracaoEmissaoEmail", NameType = typeof(ConfiguracaoEmissaoEmail))]
    public class ConfiguracaoEmissaoEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfigEmailDocTransporte", Column = "EPC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte Email { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "CEE_TIPO_DOCUMENTO", TypeType = typeof(Enumeradores.TipoDocumento), NotNull = false)]
        public virtual Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emitir", Column = "CEE_EMITIR", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Emitir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEmitir", Column = "CEE_TEMPO_EMITIR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEmitir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agrupar", Column = "CEE_AGRUPAR", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail Agrupar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CEE_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        //A = Automático
        //F = Frimesa
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CEE_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PalavraChave", Column = "CEE_PALAVRA_CHAVE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string PalavraChave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TamanhoPalavra", Column = "CEE_TAMANHO_PALAVRA", TypeType = typeof(int), NotNull = false)]
        public virtual int TamanhoPalavra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMDFe", Column = "CEE_GERAR_MDFE", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao GerarMDFe { get; set; }
    }
}
