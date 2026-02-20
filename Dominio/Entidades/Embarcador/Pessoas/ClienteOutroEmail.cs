using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_OUTRO_EMAIL", EntityName = "ClienteOutroEmail", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail", NameType = typeof(ClienteOutroEmail))]
    public class ClienteOutroEmail : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "COE_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailStatus", Column = "COE_EMAIL_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string EmailStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmail", Column = "COE_EMAIL_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmail), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmail TipoEmail { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Email;
            }
        }

        public virtual string DescricaoTipoEmail
        {
            get
            {
                switch (this.TipoEmail)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Administrativo:
                        return "Administrativo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Cobranca:
                        return "Cobrança";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Financeiro:
                        return "Financeiro";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros:
                        return "Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Pessoal:
                        return "Pessoal";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Principal:
                        return "Principal";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.SAC:
                        return "SAC";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Coleta:
                        return "Coleta";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Agendamento:
                        return "Agendamento";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(ClienteOutroEmail other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
