using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_ESCALA", EntityName = "EscalaVeiculo", Name = "Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo", NameType = typeof(EscalaVeiculo))]
    public class EscalaVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "VES_SITUACAO", TypeType = typeof(SituacaoEscalaVeiculo), NotNull = true)]
        public virtual SituacaoEscalaVeiculo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EscalaVeiculoHistorico", Column = "VEH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EscalaVeiculoHistorico UltimoHistorico { get; set; }
    }
}
