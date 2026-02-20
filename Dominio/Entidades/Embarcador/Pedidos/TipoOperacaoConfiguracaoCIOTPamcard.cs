namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_CONFIGURACAO_CIOT_PAMCARD", EntityName = "TipoOperacaoConfiguracaoCIOTPamcard", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard", NameType = typeof(TipoOperacaoConfiguracaoCIOTPamcard))]
    public class TipoOperacaoConfiguracaoCIOTPamcard : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTC_UTILIZAR_CONFIGURACAO_PERSONALIZADA_PARCELAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarConfiguracaoPersonalizadaParcelas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTC_EFETIVACAO_ADIANTAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTC_STATUS_ADIANTAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus? StatusAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTC_EFETIVACAO_ABASTECIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTC_STATUS_ABASTECIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus? StatusAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTC_EFETIVACAO_SALDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTC_STATUS_SALDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus? StatusSaldo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração do CIOT Pamcard";
            }
        }
    }
}
