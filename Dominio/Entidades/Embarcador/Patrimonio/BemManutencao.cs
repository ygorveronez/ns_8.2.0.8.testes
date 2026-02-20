using System;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_MANUTENCAO", EntityName = "BemManutencao", Name = "Dominio.Entidades.Embarcador.Patrimonio.BemManutencao", NameType = typeof(BemManutencao))]
    public class BemManutencao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "BMA_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGarantia", Column = "BMA_DATA_GARANTIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGarantia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "BMA_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoDefeito", Column = "BMD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoDefeito MotivoDefeito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOrcado", Column = "BMA_VALOR_ORCADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorOrcado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPago", Column = "BMA_VALOR_PAGO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoSaida", Column = "BMA_OBSERVACAO_SAIDA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRetorno", Column = "BMA_OBSERVACAO_RETORNO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "BMA_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusBem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusBem Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bem", Column = "BEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Bem Bem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal.NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get { return Bem.Descricao; }
        }

        public virtual bool Equals(BemManutencao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
