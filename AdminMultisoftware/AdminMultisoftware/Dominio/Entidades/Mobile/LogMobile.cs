using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Mobile
{
    /// <summary>
    /// Representa um log do app. Pode ser um erro ou não.
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_MOBILE", EntityName = "LogMobile", Name = "AdminMultisoftware.Dominio.Entidades.Mobile.LogMobile", NameType = typeof(LogMobile))]
    public class LogMobile : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LOG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Motorista que gerou o erro. Pode ser null, já que um erro pode ocorrer antes do login.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UsuarioMobile", Column = "LOG_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UsuarioMobile Motorista { get; set; }

        /// <summary>
        /// Id do cliente em que o erro aconteceu. Pode ser null, já que um erro pode ocorrer antes do login.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IdClienteMultisoftware", Column = "LOG_ID_CLIENTE_MULTISOFTWARE", TypeType = typeof(int), NotNull = false)]
        public virtual int IdClienteMultisoftware { get; set; }

        /// <summary>
        /// Data em que o registro aconteceu no app.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRegistroApp", Column = "LOG_DATA_REGISTRO_APP", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRegistroApp { get; set; }

        /// <summary>
        /// Data em que o registro foi salvo no banco.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "LOG_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        /// <summary>
        /// Versão do app quand o log foi enviado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoApp", Column = "LOG_VERSAO_APP", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string VersaoApp { get; set; }

        /// <summary>
        /// Versão do sistema operacional.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoSistemaOperacional", Column = "LOG_VERSAO_SISTEMA_OPERACIONAL", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string VersaoSistemaOperacional { get; set; }

        /// <summary>
        /// Marca do aparelho. Pode ser null porque pode acontecer de não conseguirmos em alguns casos.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "MarcaAparelho", Column = "LOG_MARCA_APARELHO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string MarcaAparelho { get; set; }

        /// <summary>
        /// Modelo do aparelho.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloAparelho", Column = "LOG_MODELO_APARELHO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string ModeloAparelho { get; set; }

        /// <summary>
        /// Mensagem do log. Contém uma descrição básica do log para pesquisa.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "LOG_MENSAGEM", TypeType = typeof(string), Length = 8192, NotNull = true)]
        public virtual string Mensagem { get; set; }

        /// <summary>
        /// Informações extra do log em JSON. Pode ser usado arbritrariamente para enviar dados de um evento que aconteceu.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Extra", Column = "LOG_EXTRA", TypeType = typeof(string), Length = 65536, NotNull = true)]
        public virtual string Extra { get; set; }

        /// <summary>
        /// Booleano representando se o log é um erro que ocorreu ou não.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Erro", Column = "LOG_ERRO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Erro { get; set; }

    }
}
