using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_OPCOES", EntityName = "CheckListOpcoes", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes", NameType = typeof(CheckListOpcoes))]
    public class CheckListOpcoes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_CATEGORIA", TypeType = typeof(CategoriaOpcaoCheckList), NotNull = false)]
        public virtual CategoriaOpcaoCheckList Categoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_ASSUNTO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_APLICACAO", TypeType = typeof(AplicacaoOpcaoCheckList), NotNull = false)]
        public virtual AplicacaoOpcaoCheckList Aplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TAG_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TagIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TIPO", TypeType = typeof(TipoOpcaoCheckList), NotNull = false)]
        public virtual TipoOpcaoCheckList Tipo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_RESPOSTA_IMPEDITIVA", TypeType = typeof(CheckListResposta), NotNull = false)]
        public virtual CheckListResposta? RespostaImpeditiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Obrigatorio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TIPO_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TIPO_HORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoHora { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TIPO_DECIMAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoDecimal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_PERMITE_NA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteNaoAplica { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_EXIBIR_SOMENTE_PARA_FRETES_ONDE_REMETENTE_FOR_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirSomenteParaFretesOndeRemetenteForTomador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Alternativas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECK_LIST_ALTERNATIVA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CheckListAlternativa", Column = "CLA_CODIGO")]
        public virtual ICollection<CheckListAlternativa> Alternativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TIPO_CHECK", TypeType = typeof(TipoCheckListGuarita), NotNull = false)]
        public virtual TipoCheckListGuarita TipoCheckListGuarita { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListTipo CheckListTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistRelacaoPergunta", Column = "CRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChecklistRelacaoPergunta RelacaoPergunta { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistOpcoesRelacaoCampo", Column = "CRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChecklistOpcoesRelacaoCampo RelacaoCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_ETAPA_CHECKLIST", TypeType = typeof(EtapaCheckList), NotNull = false)]
        public virtual EtapaCheckList EtapaCheckList { get; set; }

        public virtual string ObterAssunto()
        {
            if (Categoria == CategoriaOpcaoCheckList.Outro)
                return Assunto;
            
            return Categoria.ObterDescricao();
        }
    }
}