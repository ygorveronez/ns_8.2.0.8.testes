namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA_TEMPO_DESCARGA_FAIXA_PESO", EntityName = "TipoCargaTempoDescargaFaixaPeso", Name = "Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso", NameType = typeof(TipoCargaTempoDescargaFaixaPeso))]
    public class TipoCargaTempoDescargaFaixaPeso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoDeCarga { get; set; }

        /// <summary>
        /// Início da faixa (Kg)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Inicio", Column = "TCF_INICIO", TypeType = typeof(int), NotNull = true)]
        public virtual int Inicio { get; set; }

        /// <summary>
        /// Fim da faixa (Kg)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Fim", Column = "TCF_FIM", TypeType = typeof(int), NotNull = true)]
        public virtual int Fim { get; set; }

        /// <summary>
        /// Tempo da descarga, em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarga", Column = "TCF_TEMPO_DESCARGA", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoDescarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TipoDeCarga?.Descricao  + " = " + this.Inicio + " - " + this.Fim;
            }
        }
    }
}
