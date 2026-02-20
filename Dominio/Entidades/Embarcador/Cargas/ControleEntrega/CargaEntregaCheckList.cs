namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_CHECKLIST", EntityName = "CargaEntregaCheckList", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList", NameType = typeof(CargaEntregaCheckList))]
    public class CargaEntregaCheckList : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        /// <summary>
        /// Campo adicionado em 08/07/2021, então pode não estar populado em registros antigos. Se já é 2022, delete esse comentário.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo CheckListTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCheckList", Column = "CEC_TIPO_CHECKLIST", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCheckList), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCheckList TipoCheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Respondido", Column = "CEC_RESPONDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Respondido { get; set; }

        //[NHibernate.Mapping.Attributes.Bag(0, Name = "Perguntas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_CHECKLIST_PERGUNTA")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaCheckListPergunta", Column = "CEP_CODIGO")]
        //public virtual ICollection<CargaEntregaCheckListPergunta> Perguntas { get; set; }
    }
}

