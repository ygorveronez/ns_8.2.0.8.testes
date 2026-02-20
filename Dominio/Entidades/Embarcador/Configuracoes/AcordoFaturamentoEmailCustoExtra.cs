using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_CUSTO_EXTRA", EntityName = "AcordoFaturamentoEmailCustoExtra", Name = "Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra", NameType = typeof(AcordoFaturamentoEmailCustoExtra))]
    public class AcordoFaturamentoEmailCustoExtra : EntidadeBase, IEquatable<AcordoFaturamentoEmailCustoExtra>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcordoFaturamentoCliente", Column = "AFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcordoFaturamentoCliente AcordoFaturamentoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "ALE_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Email;
            }
        }

        public virtual bool Equals(AcordoFaturamentoEmailCustoExtra other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
