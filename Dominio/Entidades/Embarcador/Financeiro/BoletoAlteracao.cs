using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_ALTERACAO", EntityName = "BoletoAlteracao", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao", NameType = typeof(BoletoAlteracao))]
    public class BoletoAlteracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "BAL_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Etapa", Column = "BAL_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "BAL_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoInicial", Column = "BAL_DATA_VENCIMENTO_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoFinal", Column = "BAL_DATA_VENCIMENTO_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoInicial", Column = "BAL_DATA_EMISSAO_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoFinal", Column = "BAL_DATA_EMISSAO_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.BoletoConfiguracao?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoBoletoAlteracaoEtapa
        {
            get
            {
                switch (this.Etapa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Alteracao:
                        return "Alteração";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Email:
                        return "Envio de E-mail";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Impresao:
                        return "Impressão";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Remessa:
                        return "Geração de Remessa";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Selecao:
                        return "Seleção de Boletos";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoBoletoAlteracaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus.Finalizado:
                        return "Finalizado";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(BoletoAlteracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
