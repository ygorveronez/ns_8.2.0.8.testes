using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TERCEIRO_VALOR", EntityName = "ContratoFreteValor", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor", NameType = typeof(ContratoFreteValor))]
    public class ContratoFreteValor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoJustificativa", Column = "CFV_TIPO_JUSTIFICATIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa TipoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AplicacaoValor", Column = "CFV_APLICACAO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete AplicacaoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CFV_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFV_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TaxaTerceiro", Column = "TAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TaxaTerceiro TaxaTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeradoAutomaticamente", Column = "CFV_GERADO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PendenciaContratoFreteFuturo", Column = "PCF_PENDENCIA_CONTRATO_FRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PendenciaContratoFreteFuturo PendenciaContratoFrete { get; set; }

        public virtual ContratoFreteValor Clonar()
        {
            return (ContratoFreteValor)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get { return this.Valor.ToString("n2"); }
        }

        public virtual string DescricaoAplicacaoValor
        {
            get { return AplicacaoValor.ObterDescricao(); }
        }

        public virtual string DescricaoTipoJustificativa
        {
            get { return TipoJustificativa.ObterDescricao(); }
        }
    }
}
