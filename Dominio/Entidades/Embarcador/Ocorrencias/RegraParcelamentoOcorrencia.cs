using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PARCELAMENTO_OCORRENCIA", EntityName = "RegraParcelamentoOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia", NameType = typeof(RegraParcelamentoOcorrencia))]
    public class RegraParcelamentoOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RPO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RPO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPO_PERIODO_FATURAMENTO", TypeType = typeof(PeriodoFechamento), NotNull = true)]
        public virtual PeriodoFechamento PeriodoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePeriodos", Column = "RPO_QUANTIDADE_PERIODOS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadePeriodos { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
