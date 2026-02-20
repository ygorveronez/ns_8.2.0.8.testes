using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_SEGURO", EntityName = "SeguroCTE", Name = "Dominio.Entidades.SeguroCTE", NameType = typeof(SeguroCTE))]
    public class SeguroCTE : EntidadeBase, IEquatable<SeguroCTE>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "SEG_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoSeguro), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoSeguro Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJSeguradora", Column = "SEG_CNPJSEGURADORA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSeguradora", Column = "SEG_NOMESEGURADORA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NomeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "SEG_NUMAPOLICE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAverbacao", Column = "SEG_NUMAVERBACAO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string NumeroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "SEG_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case Enumeradores.TipoSeguro.Destinatario:
                        return "Destinatário";
                    case Enumeradores.TipoSeguro.Emitente_CTE:
                        return "Emitente";
                    case Enumeradores.TipoSeguro.Expedidor:
                        return "Expedidor";
                    case Enumeradores.TipoSeguro.Recebedor:
                        return "Recebedor";
                    case Enumeradores.TipoSeguro.Remetente:
                        return "Remetente";
                    case Enumeradores.TipoSeguro.Tomador_Servico:
                        return "Tomador";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(SeguroCTE other)
        {
            if (CNPJSeguradora == other.CNPJSeguradora)
                return true;
            else return false;
        }

        public virtual SeguroCTE Clonar()
        {
            return (SeguroCTE)this.MemberwiseClone();
        }
    }
}
