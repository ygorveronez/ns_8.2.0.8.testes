using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_TOMADOR", EntityName = "RegraTomador", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraTomador", NameType = typeof(RegraTomador))]
    public class RegraTomador : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemFilial", Column = "RTP_ORIGEM_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrigemFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinoFilial", Column = "RTP_DESTINO_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DestinoFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "PED_TIPO_TOMADOR", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "RTP_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RTP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Observacao;
            }
        }


        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string DescricaoTipoTomador
        {
            get
            {
                switch (this.TipoTomador)
                {
                    case Enumeradores.TipoTomador.Remetente:
                        return "Remetente";
                    case Enumeradores.TipoTomador.Expedidor:
                        return "Expedidor";
                    case Enumeradores.TipoTomador.Recebedor:
                        return "Recebedor";
                    case Enumeradores.TipoTomador.Destinatario:
                        return "Destinatário";
                    case Enumeradores.TipoTomador.Intermediario:
                        return "Intermediário";
                    case Enumeradores.TipoTomador.Outros:
                        return "Outros";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(RegraTomador other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
