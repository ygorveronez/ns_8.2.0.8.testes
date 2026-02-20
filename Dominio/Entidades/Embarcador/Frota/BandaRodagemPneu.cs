using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU_BANDA_RODAGEM", EntityName = "Frota.BandaRodagemPneu", Name = "Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu", NameType = typeof(BandaRodagemPneu))]
    public class BandaRodagemPneu : EntidadeBase, IEquatable<BandaRodagemPneu>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PBR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PBR_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PBR_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PBR_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneu), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneu Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.MarcaPneu", Column = "PMR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MarcaPneu Marca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(BandaRodagemPneu other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
