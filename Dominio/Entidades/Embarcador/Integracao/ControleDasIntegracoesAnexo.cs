namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_DAS_INTEGRACOES_ANEXOS", EntityName = "ControleDasIntegracoesAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.ControleDasIntegracoesAnexo", NameType = typeof(ControleDasIntegracoesAnexo))]
    public class ControleDasIntegracoesAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ANX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_DESCRICAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleDasIntegracoes", Column = "CDI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleDasIntegracoes EntidadeAnexo { get; set; }

    }
}
