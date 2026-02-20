using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_GLOBALIZADO_FORNECEDOR", EntityName = "EmailGlobalizadoFornecedor", Name = "Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor", NameType = typeof(EmailGlobalizadoFornecedor))]
    public class EmailGlobalizadoFornecedor : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EGF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "EGF_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmail", Column = "EGF_CORPO_EMAIL", Type = "StringClob", NotNull = false)]
        public virtual string CorpoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "EGF_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "EGF_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmail), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmail Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarTodosFornecedores", Column = "EGF_ENVIAR_TODOS_FORNECEDORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTodosFornecedores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMAIL_GLOBALIZADO_FORNECEDOR_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EGF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmailGlobalizadoFornecedorAnexo", Column = "ANX_CODIGO")]
        public virtual IList<EmailGlobalizadoFornecedorAnexo> Anexos { get; set; }
    }
}
