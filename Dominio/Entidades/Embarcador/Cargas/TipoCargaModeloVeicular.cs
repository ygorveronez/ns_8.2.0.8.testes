namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA_MODELO_VEICULAR", EntityName = "TipoCargaModeloVeicular", Name = "Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular", NameType = typeof(TipoCargaModeloVeicular))]
    public class TipoCargaModeloVeicular : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "CMV_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarga", Column = "CMV_TEMPO_DESCARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDescarga { get; set; }



        public virtual string Descricao
        {
            get
            {
                return this.TipoDeCarga?.Descricao  + " - " + this.ModeloVeicularCarga.Descricao;
            }
        }
    }
}
