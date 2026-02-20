using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TRAJETO", EntityName = "CargaTrajeto", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaTrajeto", NameType = typeof(CargaTrajeto))]
    public class CargaTrajeto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualConcluido", Column = "CTM_PERCENTUAL_CONCLUIDO", TypeType = typeof(int), NotNull = false)]
        public virtual int PercentualConcluido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemTotal", Column = "CTM_QUILOMETRAGEM_TOTAL", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal QuilometragemTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoTrajeto", Column = "CTM_SITUACAO_TRAJETO", TypeType = typeof(SituacaoTrajeto), NotNull = false)]
        public virtual SituacaoTrajeto SituacaoTrajeto { get; set; }
    }
}
