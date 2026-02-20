using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_MANOBRA", EntityName = "Manobra", Name = "Dominio.Entidades.Embarcador.Logistica.Manobra", NameType = typeof(Manobra))]
    public class Manobra : EntidadeBase, IEquatable<Manobra>
    {
        [Id(0, Name = "Codigo", Type = "System.Int32", Column = "MNB_CODIGO")]
        [Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Property(0, Name = "DataCriacao", Column = "MNB_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [Property(0, Name = "DataFim", Column = "MNB_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [Property(0, Name = "DataInicio", Column = "MNB_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [Property(0, Name = "DataMaximaIniciar", Column = "MNB_DATA_MAXIMA_INICIAR", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataMaximaIniciar { get; set; }

        [Property(0, Name = "Situacao", Column = "MNB_SITUACAO_MANOBRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobra), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobra Situacao { get; set; }

        [ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [ManyToOne(0, Class = "AreaVeiculoPosicao", Column = "AVP_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual AreaVeiculoPosicao LocalDestino { get; set; }

        [ManyToOne(0, Class = "ManobraAcao", Column = "MAC_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual ManobraAcao Acao { get; set; }

        [ManyToOne(0, Class = "OcorrenciaPatio", Column = "ORP_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual GestaoPatio.OcorrenciaPatio OcorrenciaPatio { get; set; }

        [ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO_TRACAO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Veiculo Tracao { get; set; }

        [Set(0, Name = "Reboques", Cascade = "all", Lazy = CollectionLazy.True, Table = "T_MANOBRA_REBOQUE")]
        [Key(1, Column = "MNB_CODIGO")]
        [ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual AreaVeiculoPosicao ObterLocalAtual()
        {
            if (Tracao != null)
                return Tracao.LocalAtual;

            if (Reboques?.Count > 0)
                return Reboques.FirstOrDefault().LocalAtual;

            return null;
        }

        public virtual Cargas.ModeloVeicularCarga ObterModeloVeicularCarga()
        {
            if (Tracao != null)
                return Tracao.ModeloVeicularCarga;

            if (Reboques?.Count > 0)
                return Reboques.FirstOrDefault().ModeloVeicularCarga;

            return null;
        }

        public virtual string ObterTempo()
        {
            if (!DataFim.HasValue)
                return "";

            TimeSpan tempo = (DataFim.Value - DataCriacao);

            return $"{(tempo.Days > 0 ? $"{tempo.Days}d " : "")}{tempo.ToString("hh':'mm':'ss")}";
        }

        public virtual string ObterTempoAguardando()
        {
            DateTime dataInicio = DataInicio.HasValue ? DataInicio.Value : DateTime.Now;
            TimeSpan tempo = (dataInicio - DataCriacao);

            return $"{(tempo.Days > 0 ? $"{tempo.Days}d " : "")}{tempo.ToString("hh':'mm':'ss")}";
        }

        public virtual string ObterTempoEmManobra()
        {
            if (!DataInicio.HasValue || !DataFim.HasValue)
                return "";

            TimeSpan tempo = (DataFim.Value - DataInicio.Value);

            return $"{(tempo.Days > 0 ? $"{tempo.Days}d " : "")}{tempo.ToString("hh':'mm':'ss")}";
        }

        public virtual bool Equals(Manobra other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
