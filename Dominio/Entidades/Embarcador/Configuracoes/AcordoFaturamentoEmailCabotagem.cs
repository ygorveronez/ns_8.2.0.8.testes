using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_CABOTAGEM", EntityName = "AcordoFaturamentoEmailCabotagem", Name = "Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem", NameType = typeof(AcordoFaturamentoEmailCabotagem))]
    public class AcordoFaturamentoEmailCabotagem : EntidadeBase, IEquatable<AcordoFaturamentoEmailCabotagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcordoFaturamentoCliente", Column = "AFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcordoFaturamentoCliente AcordoFaturamentoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "AEC_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Email;
            }
        }

        public virtual bool Equals(AcordoFaturamentoEmailCabotagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
