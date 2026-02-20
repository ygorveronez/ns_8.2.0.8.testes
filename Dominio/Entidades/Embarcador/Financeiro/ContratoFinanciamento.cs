using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FINANCIAMENTO", EntityName = "ContratoFinanciamento", Name = "Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento", NameType = typeof(ContratoFinanciamento))]
    public class ContratoFinanciamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CFI_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CFI_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "CFI_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "CFI_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CFI_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CFI_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VencimentoPrimeiraParcela", Column = "CFI_VENCIMENTO_PRIMEIRA_PARCELA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VencimentoPrimeiraParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Provisao", Column = "CFI_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Provisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Repetir", Column = "CFI_REPETIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Repetir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dividir", Column = "CFI_DIVIDIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Dividir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Periodicidade", Column = "CFI_PERIODICIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Periodicidade), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Periodicidade Periodicidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOcorrencia", Column = "CFI_NUMERO_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaOcorrencia", Column = "CFI_DIA_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAcrescimo", Column = "CFI_VALOR_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_ACRESCIMO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "CFI_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FINANCIAMENTO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFinanciamentoValor", Column = "CFV_CODIGO")]
        public virtual IList<ContratoFinanciamentoValor> Valores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FINANCIAMENTO_PARCELA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFinanciamentoParcela", Column = "CFP_CODIGO")]
        public virtual IList<ContratoFinanciamentoParcela> Parcelas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FINANCIAMENTO_DOCUMENTO_ENTRADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFinanciamentoDocumentoEntrada", Column = "CDE_CODIGO")]
        public virtual IList<ContratoFinanciamentoDocumentoEntrada> DocumentosEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FINANCIAMENTO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFinanciamentoVeiculo", Column = "CFV_CODIGO")]
        public virtual IList<ContratoFinanciamentoVeiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FINANCIAMENTO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFinanciamentoAnexo", Column = "CFA_CODIGO")]
        public virtual IList<ContratoFinanciamentoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Modalidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FINANCIAMENTO_MODALIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModalidadeContratoFinanciamento", Column = "MCF_CODIGO")]
        public virtual ICollection<ModalidadeContratoFinanciamento> Modalidades { get; set; }

        public virtual string Descricao
        {
            get { return this.NumeroDocumento; }
        }

        public virtual bool Equals(ContratoFinanciamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
