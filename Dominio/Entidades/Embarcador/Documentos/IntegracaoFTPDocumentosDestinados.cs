using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_FTP_DOCUMENTO_DESTINADO_EMPRESA", EntityName = "IntegracaoFTPDocumentosDestinados", Name = "Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados", NameType = typeof(IntegracaoFTPDocumentosDestinados))]
    public class IntegracaoFTPDocumentosDestinados : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IFD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "IFD_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "IFD_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "IFD_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "IFD_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioInput", Column = "IFD_DIRETORIO_IMPUT", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioInput { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioOutput", Column = "IFD_DIRETORIO_OUTPUT", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioOutput { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioXML", Column = "IFD_DIRETORIO_XML", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Passivo", Column = "IFD_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFD_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFD_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioImputXML", Column = "IFD_DIRETORIO_IMPUT_XML", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioImputXML { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.EnderecoFTP ?? string.Empty;
            }
        }

        public virtual bool Equals(IntegracaoFTPDocumentosDestinados other)
        {

            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
