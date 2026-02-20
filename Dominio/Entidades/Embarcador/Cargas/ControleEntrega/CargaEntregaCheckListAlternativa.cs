namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_CHECKLIST_PERGUNTA_ALTERNATIVA", EntityName = "CargaEntregaCheckListAlternativa", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa", NameType = typeof(CargaEntregaCheckListAlternativa))]
    public class CargaEntregaCheckListAlternativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaCheckListPergunta", Column = "CEP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaEntregaCheckListPergunta CargaEntregaCheckListPergunta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_VALOR", TypeType = typeof(int), NotNull = true)]
        public virtual int Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_MARCADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Marcado { get; set; }

    }
}