using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_PERIODO_CARREGAMENTO", EntityName = "RegrasMultaAtrasoRetiradaPeriodoCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.RegrasMultaAtrasoRetiradaPeriodoCarregamento", NameType = typeof(RegrasMultaAtrasoRetiradaPeriodoCarregamento))]
    public class RegrasMultaAtrasoRetiradaPeriodoCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasMultaAtrasoRetirada", Column = "RMA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasMultaAtrasoRetirada RegrasMultaAtrasoRetirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "RPC_DIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicio", Column = "RPC_HORA_INICIO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTermino", Column = "RPC_HORA_TERMINO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HoraTermino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeHorasContrato", Column = "RPC_QUANTIDADE_HORAS_CONTRATO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeHorasContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargas", Column = "RPC_QUANTIDADE_CARGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCargas { get; set; }

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
    }
}
