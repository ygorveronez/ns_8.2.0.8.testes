using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_ARQUIVOS_DICA", EntityName = "ConfiguracaoArquivosDica", Name = "Dominio.Entidades.ConfiguracaoArquivosDica", NameType = typeof(ConfiguracaoArquivosDica))]

    public class ConfiguracaoArquivosDica : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "save-update")]
        public virtual ConfiguracaoEmpresa Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "DUP_FUNCIONARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "CAD_DATA_HORA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CAD_NOME_ARQUIVO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "CAD_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CaminhoArquivo { get; set; }

    }
}
