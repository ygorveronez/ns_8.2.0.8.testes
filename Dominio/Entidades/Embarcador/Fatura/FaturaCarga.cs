using System;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_CARGA", EntityName = "FaturaCarga", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaCarga", NameType = typeof(FaturaCarga))]
    public class FaturaCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusFaturaCarga", Column = "FAC_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga StatusFaturaCarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Carga.Descricao;
            }
        }
        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.StatusFaturaCarga)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.Faturada:
                        return "Será totalmente faturado nesta Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.FaturadaParcial:
                        return "Será faturado parcialmente nesta Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.NaoFaturada:
                        return "Não será faturado nesta Fatura";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(FaturaCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
