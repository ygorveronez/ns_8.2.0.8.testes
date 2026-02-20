using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO", EntityName = "ContratoFreteAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto", NameType = typeof(ContratoFreteAcrescimoDesconto))]
    public class ContratoFreteAcrescimoDesconto : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CAD_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO_ORIGINAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFrete ContratoFreteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAD_DISPONIBILIZAR_FECHAMENTO_DE_AGREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarFechamentoDeAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.CIOT CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CAD_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CAD_VALOR", Precision = 18, Scale = 2, TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CAD_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicao", Column = "CAD_MOTIVO_REJEICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentoAutorizado", Column = "CAD_PAGAMENTO_AUTORIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagamentoAutorizado { get; set; }

        /// <summary>
        /// Indica se o valor já foi aplicado no contrato de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAD_VALOR_APLICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorAplicado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteAcrescimoDescontoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<ContratoFreteAcrescimoDescontoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoBaixa", Column = "CAD_CODIGO_INTEGRACAO_BAIXA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracaoBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoLancamento", Column = "CAD_CODIGO_INTEGRACAO_LANCAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracaoLancamento { get; set; }

        public virtual string Descricao
        {
            get { return ContratoFrete.NumeroContrato.ToString(); }
        }
    }
}
