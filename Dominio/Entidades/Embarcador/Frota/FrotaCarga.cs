using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_CARGA", EntityName = "FrotaCarga", Name = "Dominio.Entidades.Embarcador.Frota.FrotaAlocadoCarga", NameType = typeof(FrotaCarga))]
    public class FrotaCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota", Column = "FRT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.Frota Frota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamento", Column = "FRC_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevistaInicioViagem", Column = "FRC_DATA_PREVISTA_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevistaInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevistaFimViagem", Column = "FRC_DATA_PREVISTA_FIM_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevistaFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_INICIO_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_FINAL_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalFinalViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoComprometimentoFrota", Column = "RFC_SITUACAO_COMPROMETIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprometimentoFrota), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprometimentoFrota? SituacaoComprometimentoFrota { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}