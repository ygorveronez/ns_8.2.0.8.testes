using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU_HISTORICO", EntityName = "Frota.PneuHistorico", Name = "Dominio.Entidades.Embarcador.Frota.PneuHistorico", NameType = typeof(PneuHistorico))]
    public class PneuHistorico : EntidadeBase, IEquatable<PneuHistorico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PNH_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Servicos", Column = "PNH_SERVICOS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Servicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNH_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPneuHistorico), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPneuHistorico Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.Pneu", Column = "PNU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.BandaRodagemPneu", Column = "PBR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BandaRodagemPneu BandaRodagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNH_KM_ATUAL_RODADO", TypeType = typeof(int), NotNull = false)]
        public virtual int KmAtualRodado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoEstimado", Column = "PNH_CUSTO_ESTIMADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal CustoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNH_DATA_HORA_MOVIMENTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "PNH_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        public virtual bool Equals(PneuHistorico other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
