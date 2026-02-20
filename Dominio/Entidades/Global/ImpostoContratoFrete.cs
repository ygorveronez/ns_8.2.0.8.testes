using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPOSTO_CONTRATO_FRETE", EntityName = "ImpostoContratoFrete", Name = "Dominio.Entidades.ImpostoContratoFrete", NameType = typeof(ImpostoContratoFrete))]
    public class ImpostoContratoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_PERCENTUAL_BC_IR", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = true)]
        public virtual decimal PercentualBCIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_PERCENTUAL_BC_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = true)]
        public virtual decimal PercentualBCINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_ALIQUOTA_INSS_PATRONAL", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_VALOR_TETO_RETENCAO_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTetoRetencaoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_ALIQUOTA_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = true)]
        public virtual decimal AliquotaSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_ALIQUOTA_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = true)]
        public virtual decimal AliquotaSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_ALIQUOTA_INCRA", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaINCRA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_ALIQUOTA_SALARIO_EDUCACAO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaSalarioEducacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarBaseCalculoAcumulada", Column = "ICF_UTILIZAR_BASE_CALCULO_ACUMULADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarBaseCalculoAcumulada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularPorRaizCNPJ", Column = "ICF_CALCULAR_POR_RAIZ_CNPJ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularPorRaizCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_VALOR_POR_DEPENDENTE_DESCONTO_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorPorDependenteDescontoIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaInicio", Column = "CON_DATA_VIGENCIA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaFim", Column = "CON_DATA_VIGENCIA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoTributaria", Column = "ICF_CODIGO_INTEGRACAO_TRIBUTARIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracaoTributaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICF_REGIME_TRIBUTARIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario? RegimeTributario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "ICF_FISJUR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerceiro", Column = "TPT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro TipoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "INSS", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IMPOSTO_CONTRATO_FRETE_INSS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ICF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "INSSImpostoContratoFrete", Column = "ICI_CODIGO")]
        public virtual IList<Dominio.Entidades.INSSImpostoContratoFrete> INSS { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "IRRF", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IMPOSTO_CONTRATO_FRETE_IR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ICF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "IRImpostoContratoFrete", Column = "ICR_CODIGO")]
        public virtual IList<Dominio.Entidades.IRImpostoContratoFrete> IRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCalculoIrSobreFaixaValorTotal", Column = "UTILIZAR_CALCULO_IR_SOBRE_FAIXA_VALOR_TOTAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCalculoIrSobreFaixaValorTotal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Empresa?.Descricao ?? "";
            }
        }
    }
}
