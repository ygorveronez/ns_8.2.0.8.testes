using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BORDERO", EntityName = "Bordero", Name = "Dominio.Entidades.Embarcador.Financeiro.Bordero", NameType = typeof(Bordero))]
    public class Bordero : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BOR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_DATA_BASE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_DATA_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_TIPO_PESSOA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_VALOR_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorACobrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_VALOR_TOTAL_ACRESCIMO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_VALOR_TOTAL_DESCONTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_VALOR_TOTAL_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalACobrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_OBSERVACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_IMPRIMIR_OBSERVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirObservacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_AGENCIA_DIGITO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_NUMERO_CONTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOR_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "BOR_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento TipoPagamentoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BORDERO_TITULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BOR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BorderoTitulo", Column = "BOT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo> Titulos { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento:
                        return "Em Andamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Quitado:
                        return "Liquidado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoConta
        {
            get { return TipoConta.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get
            {
                return Numero.ToString();
            }
        }
    }
}
