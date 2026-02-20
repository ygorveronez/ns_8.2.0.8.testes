using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_ESCALA_HISTORICO", EntityName = "EscalaVeiculoHistorico", Name = "Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico", NameType = typeof(EscalaVeiculoHistorico))]
    public class EscalaVeiculoHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEH_DATA_PREVISAO_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEH_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "VEH_SITUACAO", TypeType = typeof(SituacaoEscalaVeiculo), NotNull = true)]
        public virtual SituacaoEscalaVeiculo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EscalaVeiculo", Column = "VES_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EscalaVeiculo EscalaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRemocaoVeiculoEscala", Column = "MRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRemocaoVeiculoEscala MotivoRemocao { get; set; }
    }
}
