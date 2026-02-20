using Dominio.Entidades.Embarcador.Acerto;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESPESA_VIAGEM", EntityName = "DespesaViagem", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem", NameType = typeof(DespesaViagem))]
    public class DespesaViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DVC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaTMS PagamentoMotoristaTMS { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaDiariaPeriodo", Column = "TDP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaDiariaPeriodo TabelaDiariaPeriodo { get; set; }
    }
}
