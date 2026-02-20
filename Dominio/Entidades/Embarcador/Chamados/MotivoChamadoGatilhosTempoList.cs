using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_CHAMADO_GATILHOS_TEMPO_LIST", EntityName = "MotivoChamadoGatilhosTempoList", Name = "Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList", NameType = typeof(MotivoChamadoGatilhosTempoList))]
    public class MotivoChamadoGatilhosTempoList : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTL_NIVEL", TypeType = typeof(EscalationList), NotNull = false)]
        public virtual EscalationList Nivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTL_TEMPO", TypeType = typeof(int), NotNull = false)]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamado MotivoChamado { get; set; }       
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

    }
}
