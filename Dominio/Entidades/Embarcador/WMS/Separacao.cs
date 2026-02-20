using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEPARACAO", EntityName = "Separacao", Name = "Dominio.Entidades.Embarcador.WMS.Separacao", NameType = typeof(Separacao))]
    public class Separacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoSelecaoSeparacao", Column = "SEP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao SituacaoSelecaoSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Selecao", Column = "SEL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Selecao Selecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cargas", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CAR_CODIGO_CARGA_EMBARCADOR AS NVARCHAR(2000))
                                                                                            from T_SEPARACAO S
                                                                                            JOIN T_SELECAO_CARGA SC ON SC.SEL_CODIGO = S.SEL_CODIGO
                                                                                            JOIN T_CARGA C ON C.CAR_CODIGO = SC.CAR_CODIGO  
                                                                                            WHERE S.SEP_CODIGO = SEP_CODIGO FOR XML PATH('')), 3, 2000)", TypeType = typeof(string), Lazy = true)]
        public virtual string Cargas { get; set; }

        public virtual string Descricao {
            get {
                return Selecao?.Descricao ?? string.Empty;
            }
        }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Produtos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SEPARACAO_PRODUTO_EMBARCADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SEP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SeparacaoProdutoEmbarcador", Column = "SPE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador> Produtos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Funcionarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SEPARACAO_FUNCIONARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SEP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SeparacaoFuncionario", Column = "SEF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.SeparacaoFuncionario> Funcionarios { get; set; }
    }
}
