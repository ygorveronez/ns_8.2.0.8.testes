using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_STAGE_AGRUPAMENTO_COMPOSICAO_FRETE", EntityName = "StageAgrupamentoComposicaoFrete", Name = "Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete", NameType = typeof(StageAgrupamentoComposicaoFrete))]

    public class StageAgrupamentoComposicaoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "StageAgrupamento", Column = "STG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento StageAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SCF_FORMULA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Formula { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "SAF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SCF_VALORES_FORMULA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ValoresFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SCF_DESCRICAO_COMPONENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "SCF_VALOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCalculado", Column = "SCF_VALOR_CALCULADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCalculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParametro", Column = "SCF_TIPO_PARAMETRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete TipoParametro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCampoValor", Column = "SCF_TIPO_CAMPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoCampoValor { get; set; }

        public virtual string Descricao
        {
            get { return (TipoParametro == TipoParametroBaseTabelaFrete.ComponenteFrete || TipoParametro == TipoParametroBaseTabelaFrete.ValorFreteLiquido) ? DescricaoComponente : TipoParametro.ObterDescricao(); }
        }

    }
}
