namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_CARGA_PERGUNTA", EntityName = "CheckListCargaPergunta", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta", NameType = typeof(CheckListCargaPergunta))]
    public class CheckListCargaPergunta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Obrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_CATEGORIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList Categoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_SUB_CATEGORIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SubCategoriaOpcaoCheckList), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SubCategoriaOpcaoCheckList Subcategoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_RESPOSTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta? Resposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_TIPO_INFORMATIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoInformativo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoInformativo TipoInformativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCarga", Column = "CLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListCarga CheckListCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistRelacaoPergunta", Column = "CRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChecklistRelacaoPergunta RelacaoPergunta { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistOpcoesRelacaoCampo", Column = "CRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChecklistOpcoesRelacaoCampo RelacaoCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_RESPOSTA_IMPEDITIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CheckListResposta? RespostaImpeditiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLP_TAG_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TagIntegracao { get; set; }

        public virtual string DescricaoComObrigatoriedade
        {
            get
            {
                return this.Obrigatorio ? $"*{this.Descricao}" : this.Descricao;
            }
        }
    }
}