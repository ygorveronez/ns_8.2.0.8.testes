namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_PAMCARD", EntityName = "CIOTPamcard", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTPamcard", NameType = typeof(CIOTPamcard))]
    public class CIOTPamcard : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Matriz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_AJUSTAR_SALDO_VENCIMENTO_DATA_ENCERRAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarSaldoVencimentoDataEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_ENVIAR_QUANTIDADES_MAIORES_QUE_ZERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarQuantidadesMaioresQueZero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_ASSOCIAR_CARTAO_MOTORISTA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AssociarCartaoMotoristaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_UTILIZAR_DATA_ATUAL_PARA_DEFINIR_VENCIMENTO_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataAtualParaDefinirVencimentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_UTILIZAR_DATA_ATUAL_PARA_DEFINIR_VENCIMENTO_ADIANTAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataAtualParaDefinirVencimentoAdiantamento { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
