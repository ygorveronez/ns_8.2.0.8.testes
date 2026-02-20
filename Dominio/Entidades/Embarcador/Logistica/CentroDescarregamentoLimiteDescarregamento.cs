using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_DESCARREGAMENTO_LIMITE_DESCARREGAMENTO", EntityName = "CentroDescarregamentoLimiteDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento", NameType = typeof(CentroDescarregamentoLimiteDescarregamento))]
    public class CentroDescarregamentoLimiteDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "CLD_DIA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaDoMes", Column = "CLD_DIA_DO_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaDoMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mes", Column = "CLD_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasAntecedencia", Column = "CLD_HORAS_ANTECEDENCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int HorasAntecedencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoCarga { get; set; }

        public virtual string Descricao
        {
            get { return $"Limite de descarregamento ({Dia.ObterDescricaoResumida()})"; }
        }
    }
}
