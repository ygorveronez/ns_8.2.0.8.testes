using System;
using System.Collections.Generic;
using System.Linq;


namespace Dominio.Entidades.Embarcador.Cargas.ColetaEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_COLETA_ENTREGA", EntityName = "FluxoColetaEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega", NameType = typeof(FluxoColetaEntrega))]
    public class FluxoColetaEntrega : EntidadeBase
    {
        public FluxoColetaEntrega() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgSenha", Column = "FCE_DATA_AG_SENHA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAgSenha", Column = "FCE_TEMPO_AG_SENHA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgSenha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgPendenciaAlocarVeiculo", Column = "FCE_DATA_AG_PENDENCIA_ALOCAR_VEICULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgPendenciaAlocarVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAgPendenciaAlocarVeiculo", Column = "FCE_TEMPO_PENDENCIA_AG_ALOCAR_VEICULO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgPendenciaAlocarVeiculo{ get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVeiculoAlocado", Column = "FCE_DATA_VEICULO_ALOCADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVeiculoAlocado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoVeiculoAlocado", Column = "FCE_TEMPO_VEICULO_ALOCADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoVeiculoAlocado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaidaCD", Column = "FCE_DATA_SAIDA_CD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaCD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "FCE_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSaidaCD", Column = "FCE_TEMPO_SAIDA_CD", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoSaidaCD { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChegadaFornecedor", Column = "FCE_DATA_CHEGADA_FORNECEDOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoChegadaFornecedor", Column = "FCE_TEMPO_CHEGADA_FORNECEDOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoChegadaFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoCTe", Column = "FCE_DATA_EMISSAO_CTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoCTe { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEmissaoCTe", Column = "FCE_TEMPO_EMISSAO_CTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoMDFe", Column = "FCE_DATA_EMISSAO_MDFE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEmissaoMDFe", Column = "FCE_TEMPO_EMISSAO_MDFE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoEmissaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoCTeSubContratacao", Column = "FCE_DATA_EMISSAO_CTE_SUBCONTRATACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoCTeSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEmissaoCTeSubContratacao", Column = "FCE_TEMPO_EMISSAO_CTE_SUBCONTRATACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoEmissaoCTeSubContratacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaidaFornecedor", Column = "FCE_DATA_SAIDA_FORNECEDOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSaidaFornecedor", Column = "FCE_TEMPO_SAIDA_FORNECEDOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoSaidaFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChegadaCD", Column = "FCE_DATA_CHEGADA_CD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaCD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoChegadaCD", Column = "FCE_TEMPO_CHEGADA_CD", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoChegadaCD { get; set; }
        

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgOcorrencia", Column = "FCE_DATA_AG_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAgOcorrencia", Column = "FCE_TEMPO_AG_OCORRENCIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoAgOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacao", Column = "FCE_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoFinalizacao", Column = "FCE_TEMPO_FINALIZACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TempoFinalizacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAtualLiberada", Column = "FCE_ETAPA_ATUAL_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaAtualLiberada { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "FCE_SITUACAO_ETAPA_FLUXO_COLETA_ENTREGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoColetaEntrega SituacaoEtapaFluxoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAtual", Column = "FCE_ETAPA_ATUAL", TypeType = typeof(int), NotNull = true)]
        public virtual int EtapaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEE_ETAPA_FLUXO_COLETA_ENTREGA_ETAPA_ATUAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega EtapaFluxoColetaEntregaEtapaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Etapas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FLUXO_COLETA_ENTREGA_ETAPAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FluxoColetaEntregaEtapas", Column = "FEE_CODIGO")]
        public virtual ICollection<FluxoColetaEntregaEtapas> Etapas { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Carga.CodigoCargaEmbarcador;
            }
        }

        public virtual List<FluxoColetaEntregaEtapas> EtapasOrdenadas {
            get
            {
                return (from o in this.Etapas orderby o.Ordem ascending select o).ToList();
            }
        }
    }
}