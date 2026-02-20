using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FIM_HIGIENIZACAO", EntityName = "FimHigienizacao", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao", NameType = typeof(FimHigienizacao))]
    public class FimHigienizacao : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HGF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HGF_DATA_HIGIENIZACAO_FINALIZADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHigienizacaoFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaFimHigienizacaoLiberada", Column = "HGF_FIM_HIGIENIZACAO_LIBERADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaFimHigienizacaoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HGF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFimHigienizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFimHigienizacao Situacao { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Fim da higienização da carga {Carga.CodigoCargaEmbarcador}" : $"FIm da Higienização da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
