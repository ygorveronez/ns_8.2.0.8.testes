using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU_RETORNO_REFORMA", EntityName = "Frota.PneuRetornoReforma", Name = "Dominio.Entidades.Embarcador.Frota.PneuRetornoReforma", NameType = typeof(PneuRetornoReforma))]
    public class PneuRetornoReforma : EntidadeBase, IEquatable<PneuRetornoReforma>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRR_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRR_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PRR_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ResponsavelOrcamento", Column = "PRR_RESPONSAVEL_ORCAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ResponsavelOrcamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRR_SERVICO_REALIZADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ServicoRealizadoPneu), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ServicoRealizadoPneu ServicoRealizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SulcoAnterior", Column = "PRR_SULCO_ANTERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal SulcoAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SulcoAtual", Column = "PRR_SULCO_ATUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal SulcoAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMaoObra", Column = "PRR_VALOR_MAO_OBRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorMaoObra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "PRR_VALOR_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorResidualAtualPneu", Column = "PRR_VALOR_RESIDUAL_ATUAL_PNEU", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorResidualAtualPneu { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRR_VIDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.VidaPneu), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.VidaPneu Vida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Almoxarifado", Column = "AMX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Almoxarifado Almoxarifado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.BandaRodagemPneu", Column = "PBR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BandaRodagemPneu BandaRodagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.Pneu", Column = "PNU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.PneuHistorico", Column = "PNH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PneuHistorico PneuHistorico { get; set; }

        public virtual bool Equals(PneuRetornoReforma other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
