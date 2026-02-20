using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE", EntityName = "FechamentoFrete", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete", NameType = typeof(FechamentoFrete))]
    public class FechamentoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_NUMERO", TypeType = typeof(int))]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador Contrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_TOTAL_ACRESCIMOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalAcrescimos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_TOTAL_DESCONTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalDescontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_TOTAL_ACRESCIMOS_APLICAR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalAcrescimosAplicar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_TOTAL_DESCONTOS_APLICAR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalDescontosAplicar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_TOTAL_KM_FRANQUIA", TypeType = typeof(int), NotNull = false)]
        public virtual int? TotalKmFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_VALOR_BASE_FRANQUIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_VALOR_COMPLEMENTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComplementos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_VALOR_FRANQUIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_VALOR_FRANQUIA_POR_KM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorFranquiaPorKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_VALOR_FRANQUIA_POR_KM_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorFranquiaPorKmExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_VALOR_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete), NotNull = false)]
        public virtual SituacaoFechamentoFrete Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_DATA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_NAO_EMITIR_COMPLEMENTO", TypeType = typeof(bool))]
        public virtual bool NaoEmitirComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_ANO", TypeType = typeof(int))]
        public virtual int Ano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_MES", TypeType = typeof(int))]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_PERIODO", TypeType = typeof(int))]
        public virtual int Periodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_AGUARDANDO_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteValorContrato { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FECHAMENTO_FRETE_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FEF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FechamentoFreteCarga", Column = "FFC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FECHAMENTO_FRETE_CTE_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FEF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FechamentoFreteCTeIntegracao", Column = "FIC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ocorrencias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FECHAMENTO_FRETE_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FEF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FechamentoFreteOcorrencia", Column = "FFO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia> Ocorrencias { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get { return $"{Numero} - {Contrato.Descricao} ({DataInicio.ToString("dd/MM/yyyy")} até {DataFim.ToString("dd/MM/yyyy")})"; }
        }

        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ObterComponenteFreteValorContrato()
        {
            return Situacao.IsFechamentoIniciado() ? ComponenteFreteValorContrato : Contrato.ComponenteFreteValorContrato;
        }
    }

}
