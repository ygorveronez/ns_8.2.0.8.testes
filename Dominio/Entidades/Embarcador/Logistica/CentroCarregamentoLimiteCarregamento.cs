using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_CARREGAMENTO_LIMITE_CARREGAMENTO", EntityName = "CentroCarregamentoLimiteCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento", NameType = typeof(CentroCarregamentoLimiteCarregamento))]
    public class CentroCarregamentoLimiteCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "CLC_DIA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAntecedencia", Column = "CLC_DIAS_ANTECEDENCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasAntecedencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimite", Column = "CLC_HORA_LIMITE", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraLimite { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoCarga { get; set; }

        public virtual string Descricao
        {
            get { return $"Limite de carregamento ({Dia.ObterDescricaoResumida()})"; }
        }
    }
}
