using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCA_CARREGAMENTO", EntityName = "DocaCarregamento", Name = "Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento", NameType = typeof(DocaCarregamento))]
    public class DocaCarregamento : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AreaVeiculoPosicao", Column = "AVP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.AreaVeiculoPosicao LocalCarregamento { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "DCA_PREVISTO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? InicioPrevisto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCA_DATA_INFORMACAO_DOCA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInformacaoDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCA_NUMERO_DOCA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaDocaCarregamentoLiberada", Column = "DCA_DOCA_CARREGAMENTO_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaDocaCarregamentoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocaCarregamento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocaCarregamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiLaudo", Column = "DCA_POSSUI_LAUDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiLaudo { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? Carga.CodigoCargaEmbarcador : PreCarga.NumeroPreCarga; }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocaCarregamento.AgInformarDoca:
                        return "Ag Informar Doca";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocaCarregamento.Informada:
                        return "Doca Informada";
                    default:
                        return "";
                }
            }
        }
    }
}
