using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_CARREGAMENTO", EntityName = "ControleCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.ControleCarregamento", NameType = typeof(ControleCarregamento))]
    public class ControleCarregamento : EntidadeBase, IEquatable<ControleCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "CCR_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacao", Column = "CCR_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "CCR_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CCR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleCarregamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleCarregamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaJanelaCarregamento JanelaCarregamento { get; set; }

        public virtual bool Equals(ControleCarregamento other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
