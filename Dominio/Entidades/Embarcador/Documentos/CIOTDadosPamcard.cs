namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_DADOS_PAMCARD", EntityName = "CIOTDadosPamcard", Name = "Dominio.Entidades.Embarcador.Documentos.CIOTDadosPamcard", NameType = typeof(CIOTDadosPamcard))]
    public class CIOTDadosPamcard : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.CIOT CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDP_EFETIVACAO_ADIANTAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDP_STATUS_ADIANTAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus? StatusAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDP_EFETIVACAO_ABASTECIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDP_STATUS_ABASTECIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus? StatusAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDP_EFETIVACAO_SALDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDP_STATUS_SALDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus? StatusSaldo { get; set; }

    }
}
