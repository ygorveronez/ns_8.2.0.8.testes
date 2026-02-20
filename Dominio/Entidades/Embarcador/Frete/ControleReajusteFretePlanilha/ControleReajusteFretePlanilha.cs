using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_REAJUSTE_FRETE_PLANILHA", EntityName = "ControleReajusteFretePlanilha", Name = "Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha", NameType = typeof(ControleReajusteFretePlanilha))]
    public class ControleReajusteFretePlanilha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFP_NUMERO", TypeType = typeof(int))]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFP_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFP_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFP_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "ControleAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_CONTROLE_REAJUSTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RFP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaControleReajusteFretePlanilha", Column = "AAC_CODIGO")]
        public virtual ICollection<AprovacaoAlcadaControleReajusteFretePlanilha> ControleAutorizacoes { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.AgAprovacao:
                        return "Ag. Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Aprovado:
                        return "Aprovado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Rejeitado:
                        return "Rejeitado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.SemRegra:
                        return "Sem Regra";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }
    }
}
