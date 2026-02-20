namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFERENCIA_SEPARACAO", EntityName = "ConferenciaSeparacao", Name = "Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao", NameType = typeof(ConferenciaSeparacao))]
    public class ConferenciaSeparacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_CODIGO_BARRAS", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CodigoBarras { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaControleExpedicao", Column = "CCX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao Expedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRecebimentoMercadoria", Column = "COS_TIPO_RECEBIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria? TipoRecebimentoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFaltante", Column = "COS_QUANTIDADE_FALTANTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeFaltante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumeCarga", Column = "COS_VOLUMES_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal VolumeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_CODIGOS_VALIDADOS", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CodigosValidados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COS_NUMERO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Numero { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.CodigoBarras;
            }
        }
    }
}
