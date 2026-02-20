using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_CHAMADO_GATILHOS_NA_CARGA", EntityName = "MotivoChamadoGatilhosNaCarga", Name = "Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga", NameType = typeof(MotivoChamadoGatilhosNaCarga))]
    public class MotivoChamadoGatilhosNaCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MGC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamado MotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Gatilho", Column = "MGC_GATILHO", TypeType = typeof(TipoMotivoChamadoGatilhoNaCarga), NotNull = true)]
        public virtual TipoMotivoChamadoGatilhoNaCarga Gatilho { get; set; }
    }
}
