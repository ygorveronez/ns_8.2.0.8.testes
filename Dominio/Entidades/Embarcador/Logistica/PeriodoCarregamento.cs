using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_PERIODO_CARREGAMENTO", EntityName = "PeriodoCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento", NameType = typeof(PeriodoCarregamento))]
    public class PeriodoCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExcecaoCapacidadeCarregamento", Column = "CEX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExcecaoCapacidadeCarregamento ExcecaoCapacidadeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExclusividadeCarregamento", Column = "ECC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExclusividadeCarregamento ExclusividadeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "PEC_DIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeCarregamentoVolume", Column = "PEC_CAPACIDADE_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeCarregamentoCubagem", Column = "PEC_CAPACIDADE_CARREGAMENTO_CUBAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagem { get; set; } //SÓ APARECE SE MARCADO VOLUME E CUBAGEM

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeCarregamentoSimultaneo", Column = "PEC_CAPACIDADE_CARREGAMENTO_SIMULTANEO", TypeType = typeof(int), NotNull = true)]
        public virtual int CapacidadeCarregamentoSimultaneo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaExcessoTempo", Column = "PEC_TOLERANCIA_EXCESSO_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int ToleranciaExcessoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicio", Column = "PEC_HORA_INICIO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTermino", Column = "PEC_HORA_TERMINO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraTermino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoOperacaoSimultaneo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_PERIODO_CARREGAMENTO_TIPO_OPERACAO_SIMULTANEO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoCarregamentoTipoOperacaoSimultaneo", Column = "PTS_CODIGO")]
        public virtual ICollection<PeriodoCarregamentoTipoOperacaoSimultaneo> TipoOperacaoSimultaneo { get; set; }

        public virtual string Descricao
        {
            get { return $"{DiaSemana} {HoraInicio.ToString(@"hh\:mm")} até {HoraTermino.ToString(@"hh\:mm")}"; }
        }

        public virtual string DescricaoPeriodo
        {
            get { return $"Das {HoraInicio.ToString(@"hh\:mm")} as {HoraTermino.ToString(@"hh\:mm")}"; }
        }

        public virtual string DiaSemana
        {
            get { return Dia.ObterDescricaoResumida(); }
        }

        public virtual int ToleranciaPorLimiteCarregamentos
        {
            get
            {
                CentroCarregamento centroCarregamento = CentroCarregamento ?? ExcecaoCapacidadeCarregamento?.CentroCarregamento ?? ExclusividadeCarregamento?.CentroCarregamento;

                return (centroCarregamento?.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas) ? 0 : ToleranciaExcessoTempo;
            }
        }
    }
}
