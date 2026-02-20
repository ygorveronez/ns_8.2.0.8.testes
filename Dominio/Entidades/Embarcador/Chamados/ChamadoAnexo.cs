namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_ANEXOS", EntityName = "ChamadoAnexo", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo", NameType = typeof(ChamadoAnexo))]
    public class ChamadoAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACH_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACH_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACH_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaFiscalServico", Column = "ACH_NOTA_FISCAL_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotaFiscalServico { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

        public virtual string NomeArquivoSemExtensao
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(NomeArquivo); }
        }
    }
}
