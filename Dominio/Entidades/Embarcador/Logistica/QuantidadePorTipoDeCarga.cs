namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_QUANTIDADE_POR_TIPO_DE_CARGA_DESCARREGAMENTO", EntityName = "QuantidadePorTipoDeCargaDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento", NameType = typeof(QuantidadePorTipoDeCargaDescarregamento))]
    public class QuantidadePorTipoDeCargaDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "QPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExcecaoCapacidadeDescarregamento", Column = "CEX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExcecaoCapacidadeDescarregamento ExcecaoCapacidadeDescarregamento { get; set; }
        
        //[Obsolete("Migrado para um set")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QPT_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "QPT_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QPT_TOLERANCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int? Tolerancia { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "QPT_TOLERANCIA_CANCELAMENTO_AGENDA_CONFIRMADA", TypeType = typeof(int), NotNull = false)]
        public virtual int? ToleranciaCancelamentoAgendaConfirmada { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "QPT_TOLERANCIA_CANCELAMENTO_AGENDA_NAO_CONFIRMADA", TypeType = typeof(int), NotNull = false)]
        public virtual int? ToleranciaCancelamentoAgendaNaoConfirmada { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Quantidade por tipo de carga do centro {this.CentroDescarregamento.Descricao}";
            }
        }
    }
}
