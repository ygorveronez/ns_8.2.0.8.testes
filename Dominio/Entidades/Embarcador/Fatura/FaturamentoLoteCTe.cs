using System;

namespace Dominio.Entidades.Embarcador.Fatura
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURAMENTO_LOTE_CTE", EntityName = "FaturamentoLoteCTe", Name = "Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe", NameType = typeof(FaturamentoLoteCTe))]
    public class FaturamentoLoteCTe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico ConhecimentoDeTransporteEletronico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoLote", Column = "FAL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoLote FaturamentoLote { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ConhecimentoDeTransporteEletronico.Descricao;
            }
        }

        public virtual bool Equals(FaturamentoLoteCTe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
