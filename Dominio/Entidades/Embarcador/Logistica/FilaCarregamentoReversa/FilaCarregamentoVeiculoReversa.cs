using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_VEICULO_REVERSA", EntityName = "FilaCarregamentoVeiculoReversa", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa", NameType = typeof(FilaCarregamentoVeiculoReversa))]
    public class FilaCarregamentoVeiculoReversa : EntidadeBase, IEquatable<FilaCarregamentoVeiculoReversa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FVR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "FVR_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "FVR_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "FVR_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FVR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculoReversa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculoReversa Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoVeiculo", Column = "FLV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoVeiculo FilaCarregamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoRetornoCarga", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Retornos.TipoRetornoCarga TipoRetornoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoConjuntoVeiculo", Column = "FCV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoConjuntoVeiculo ConjuntoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AreaVeiculoPosicao", Column = "AVP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AreaVeiculoPosicao LocalDescarregamento { get; set; }

        public virtual bool Equals(FilaCarregamentoVeiculoReversa other)
        {
            return this.Codigo == other.Codigo;
        }

        public virtual bool IsPermiteRemocao()
        {
            return (
                (Situacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculoReversa.AguardandoDescarregamento) &&
                (FilaCarregamentoVeiculo == null)
            );
        }

        public virtual string ObterTempo()
        {
            if (!DataFim.HasValue)
                return "";

            TimeSpan tempo = (DataFim.Value - DataCriacao);

            return $"{(tempo.Days > 0 ? $"{tempo.Days}d " : "")}{tempo.ToString("hh':'mm':'ss")}";
        }
    }
}
