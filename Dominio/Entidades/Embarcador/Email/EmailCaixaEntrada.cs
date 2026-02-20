using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_CAIXA_ENTRADA", EntityName = "EmailCaixaEntrada", Name = "Dominio.Entidades.Email.EmailCaixaEntrada", NameType = typeof(EmailCaixaEntrada))]
    public class EmailCaixaEntrada : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Remetente", Column = "ECE_REMETENTE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Remetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Assunto", Column = "ECE_ASSUNTO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "ECE_DATA_RECEBIMENTO", TypeType = typeof(DateTime),  NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfigEmailDocTransporte", Column = "EPC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte ConfigEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Anexos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_EMAIL_CAIXA_ENTRADA_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ECE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmailAnexos", Column = "EAN_CODIGO", NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual ICollection<Dominio.Entidades.Embarcador.Email.EmailAnexos> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "ECE_TIPO_SERVICO", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = false)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServico { get; set; }
        

        public virtual bool Equals(EmailCaixaEntrada other)
        {

            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
