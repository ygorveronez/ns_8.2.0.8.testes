using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Usuarios.Comissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_COMISSAO", EntityName = "FuncionarioComissao", Name = "Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao", NameType = typeof(FuncionarioComissao))]
    public class FuncionarioComissao : EntidadeBase, IEquatable<FuncionarioComissao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "FCO_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "FCO_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "FCO_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCO_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "FCO_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "FCO_PERCENTUAL_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissaoAcrescimo", Column = "FCO_PERCENTUAL_COMISSAO_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PercentualComissaoAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissaoTotal", Column = "FCO_PERCENTUAL_COMISSAO_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PercentualComissaoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComissao", Column = "FCO_VALOR_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorComissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_OPERADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Titulos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_COMISSAO_TITULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FuncionarioComissaoTitulo", Column = "FCT_CODIGO")]
        public virtual IList<FuncionarioComissaoTitulo> Titulos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Autorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_FUNCIONARIO_COMISSAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaFuncionarioComissao", Column = "AAF_CODIGO")]
        public virtual IList<AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> Autorizacoes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual int QuantidadeTitulos
        {
            get
            {
                int qtdTotal = 0;

                if (this.Titulos != null)
                    qtdTotal = (from o in Titulos.ToList() select o.Codigo).Count();

                return qtdTotal;
            }
        }

        public virtual decimal ValorTotalFinal
        {
            get
            {
                decimal valorTotal = 0;

                if (this.Titulos != null)
                    valorTotal = (from o in Titulos.ToList() select o.ValorFinal).Sum();

                return valorTotal;
            }
        }

        public virtual bool Equals(FuncionarioComissao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
