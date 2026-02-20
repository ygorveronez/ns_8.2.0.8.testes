using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_ELETRONICO", EntityName = "PagamentoEletronico", Name = "Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico", NameType = typeof(PagamentoEletronico))]
    public class PagamentoEletronico : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PAE_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalidadePagamentoEletronico", Column = "PAE_MODALIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico ModalidadePagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaPagamentoEletronico", Column = "PAE_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaPagamentoEletronico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaPagamentoEletronico TipoContaPagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalidadePagamentoEletronico", Column = "PAE_FINALIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FinalidadePagamentoEletronico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FinalidadePagamentoEletronico FinalidadePagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTitulos", Column = "PAE_QUANTIDADE_TITULOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeTitulos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "PAE_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "PAE_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "PAE_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAutorizacaoPagamentoEletronico", Column = "PAE_SITUACAO_AUTORIZACAO_PAGAMENTO_ELETRONICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico? SituacaoAutorizacaoPagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoPagamentoEletronico", Column = "PAE_TIPO_SERVICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoPagamentoEletronico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoPagamentoEletronico? TipoServicoPagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaLancamentoPagamentoEletronico", Column = "PAE_FORMA_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaLancamentoPagamentoEletronico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaLancamentoPagamentoEletronico? FormaLancamentoPagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoUsoEmpresaPagamentoEletronico", Column = "PAE_DESCRICAO_USO_EMPRESA_PAGAMENTO_ELETRONICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DescricaoUsoEmpresaPagamentoEletronico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DescricaoUsoEmpresaPagamentoEletronico DescricaoUsoEmpresaPagamentoEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Titulos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_ELETRONICO_TITULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoEletronicoTitulo", Column = "PET_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> Titulos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(PagamentoEletronico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
