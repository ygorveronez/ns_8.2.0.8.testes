using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_MANOBRA_ACAO", EntityName = "CentroCarregamentoManobraAcao", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao", NameType = typeof(CentroCarregamentoManobraAcao))]
    public class CentroCarregamentoManobraAcao : EntidadeBase, IEquatable<CentroCarregamentoManobraAcao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoToleranciaInicioManobra", Column = "CMA_TEMPO_TOLERANCIA_INICIO_MANOBRA", TypeType = typeof(int), NotNull = false)]
        public virtual int? TempoToleranciaInicioManobra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManobraAcao", Column = "MAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManobraAcao Acao { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(CentroCarregamentoManobraAcao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
