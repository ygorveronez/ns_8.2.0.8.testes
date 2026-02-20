using System;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TERCEIRO_CTE", EntityName = "ContratoFreteCTe", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe", NameType = typeof(ContratoFreteCTe))]
    public class ContratoFreteCTe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PreConhecimentoDeTransporteEletronico PreCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiro CTeTerceiro { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.CargaCTe?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(ContratoFreteCTe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
