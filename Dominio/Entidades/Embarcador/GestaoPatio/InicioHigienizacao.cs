using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INICIO_HIGIENIZACAO", EntityName = "InicioHigienizacao", Name = "Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao", NameType = typeof(InicioHigienizacao))]
    public class InicioHigienizacao : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HGI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HGI_DATA_HIGIENIZACAO_INICIADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHigienizacaoIniciada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaInicioHigienizacaoLiberada", Column = "HGI_INICIO_HIGIENIZACAO_LIBERADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaInicioHigienizacaoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HGI_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoInicioHigienizacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInicioHigienizacao Situacao { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Início da higienização da carga {Carga.CodigoCargaEmbarcador}" : $"Início da higienização da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
