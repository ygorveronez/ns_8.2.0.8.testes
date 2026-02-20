namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS_CTE", EntityName = "ChamadoTMSCTe", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe", NameType = typeof(ChamadoTMSCTe))]
    public class ChamadoTMSCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoTMS", Column = "CHT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChamadoTMS Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescarga", Column = "CHC_VALOR_DESCARGA", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal ValorDescarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.CTe?.Descricao ?? string.Empty) + " - " + (this.Chamado?.Descricao ?? string.Empty);
            }
        }
    }
}
