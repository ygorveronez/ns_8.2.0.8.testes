using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FTP_ENVIO_OCOREN", EntityName = "ConfiguracaoFTPEnvioOcoren", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren", NameType = typeof(ConfiguracaoFTPEnvioOcoren))]
    public class ConfiguracaoFTPEnvioOcoren : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "CFB_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CFB_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CFB_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "CFB_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "CFB_DIRETORIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Passivo", Column = "CFB_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFB_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFB_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        public virtual bool Equals(ConfiguracaoFTPEnvioOcoren other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
