using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_VALE_PALLET", EntityName = "ValePallet", Name = "Dominio.Entidades.Embarcador.Pallets.ValePallet", NameType = typeof(ValePallet))]
    public class ValePallet : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VLP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_DATA_RECOLHIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecolhimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_QUANTIDADE_RECOLHIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeRecolhimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_NOTA_FISCAL_RECOLHIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NotaFiscalRecolhimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_SITUACAO", TypeType = typeof(SituacaoValePallet), NotNull = true)]
        public virtual SituacaoValePallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DevolucaoPallet", Column = "PDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DevolucaoPallet Devolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Representante", Column = "REP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.Representante Representante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPallets", Column = "FEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPallets Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLP_ADICIONAR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarAoFechamento { get; set; }

        public virtual string Descricao
        {
            get { return this.Numero.ToString(); }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }
    }
}
