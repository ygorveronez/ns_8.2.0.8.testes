using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_LOTE", EntityName = "OcorrenciaLote", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote", NameType = typeof(OcorrenciaLote))]
    public class OcorrenciaLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OLO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OLO_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OLO_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OLO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaLote), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaLote Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OLO_TIPO_RATEIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRateioOcorrenciaLote), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRateioOcorrenciaLote TipoRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OLO_VALOR_FRETE_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OLO_MOTIVO_REJEICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MotivoRejeicao { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }
    }
}
