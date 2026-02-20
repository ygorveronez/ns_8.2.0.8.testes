namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DATA_PREFERENCIAL_DESCARGA", EntityName = "DataPreferencialDescarga", Name = "Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga", NameType = typeof(DataPreferencialDescarga))]
    public class DataPreferencialDescarga : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPD_DIA_PREFERENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaPreferencial { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "DPD_DIAS_BLOQUEIO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasBloqueio { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"{CentroDescarregamento.Descricao} - Dia {DiaPreferencial}";
            }
        }
    }
}
