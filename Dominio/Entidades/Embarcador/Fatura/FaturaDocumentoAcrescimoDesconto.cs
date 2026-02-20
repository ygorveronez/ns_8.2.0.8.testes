using System;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_DOCUMENTO_ACRESCIMO_DESCONTO", EntityName = "FaturaDocumentoAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto", NameType = typeof(FaturaDocumentoAcrescimoDesconto))]
    public class FaturaDocumentoAcrescimoDesconto : EntidadeBase
    {
        public FaturaDocumentoAcrescimoDesconto()
        {
            DataAplicacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaDocumento", Column = "FDO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.FaturaDocumento FaturaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoJustificativa", Column = "FAD_TIPO_JUSTIFICATIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa TipoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAD_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAD_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "FAD_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "FAD_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "FAD_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "FAD_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAD_DATA_APLICACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAplicacao { get; set; }

        public virtual string Descricao
        {
            get { return Justificativa.Descricao; }
        }

        public virtual string DescricaoTipoJustificativa
        {
            get
            {
                switch (TipoJustificativa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto:
                        return "Desconto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo:
                        return "Acréscimo";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
