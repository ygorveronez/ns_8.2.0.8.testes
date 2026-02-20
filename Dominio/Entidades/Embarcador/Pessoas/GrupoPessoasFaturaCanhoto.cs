using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_FATURA_CANHOTO", EntityName = "GrupoPessoasFaturaCanhoto", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto", NameType = typeof(GrupoPessoasFaturaCanhoto))]
    public class GrupoPessoasFaturaCanhoto : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarEnvioCanhoto", Column = "GRP_HABILITAR", TypeType = typeof(bool))]
        public virtual bool HabilitarEnvioCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoCanhoto", Column = "GFC_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracaoCanhoto), NotNull = true)]
        public virtual TipoIntegracaoCanhoto TipoIntegracaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "GFC_ENDERECO_FTP", TypeType = typeof(string), Length = 150)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "GFC_USUARIO", TypeType = typeof(string), Length = 50)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "GFC_SENHA", TypeType = typeof(string), Length = 50)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "GFC_PORTA", TypeType = typeof(string), Length = 10)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "GFC_DIRETORIO", TypeType = typeof(string), Length = 400)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Passivo", Column = "GFC_PASSIVO", TypeType = typeof(bool))]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarSFTP", Column = "GFC_UTILIZAR_SFTP", TypeType = typeof(bool))]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SSL", Column = "GFC_UTILIZAR_SSL", TypeType = typeof(bool))]
        public virtual bool SSL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nomenclatura", Column = "GFC_NOMENCLATURA", TypeType = typeof(string), Length = 1000)]
        public virtual string Nomenclatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExtensaoArquivo", Column = "GFC_EXTENSAO_ARQUIVO", TypeType = typeof(string), Length = 1000)]
        public virtual string ExtensaoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }
    }
}
