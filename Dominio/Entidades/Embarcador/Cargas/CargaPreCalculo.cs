using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRE_CALCULO", EntityName = "CargaPreCalculo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo", NameType = typeof(CargaPreCalculo))]
    public class CargaPreCalculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CPC_SITUACAO", TypeType = typeof(SituacaoRetornoDadosFrete), NotNull = false)]
        public virtual SituacaoRetornoDadosFrete Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "CPC_VALOR_TOTAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CPC_OBSERVACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Observacao { get; set; } 
        
    }
}
