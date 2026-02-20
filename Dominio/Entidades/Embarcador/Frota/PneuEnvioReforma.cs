using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU_ENVIO_REFORMA", EntityName = "Frota.PneuEnvioReforma", Name = "Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma", NameType = typeof(PneuEnvioReforma))]
    public class PneuEnvioReforma : EntidadeBase, IEquatable<PneuEnvioReforma>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoEstimado", Column = "PER_CUSTO_ESTIMADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal CustoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PER_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PER_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Hodometro", Column = "PER_HODOMETRO", TypeType = typeof(int), NotNull = true)]
        public virtual int Hodometro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SulcoAnterior", Column = "PER_SULCO_ANTERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal SulcoAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SulcoAtual", Column = "PER_SULCO_ATUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal SulcoAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PER_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.Pneu", Column = "PNU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.PneuHistorico", Column = "PNH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PneuHistorico PneuHistorico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoVeiculoFrota", Column = "SEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoVeiculoFrota ServicoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo TipoOrdemServico { get; set; }

        public virtual bool Equals(PneuEnvioReforma other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
