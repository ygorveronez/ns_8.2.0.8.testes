using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO", EntityName = "OrdemServicoFrota", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota", NameType = typeof(OrdemServicoFrota))]
    public class OrdemServicoFrota : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public OrdemServicoFrota()
        {
            DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Número sequencial.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "OSE_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "OSE_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_OPERADOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_OPERADOR_FECHAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario OperadorFechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente LocalManutencao { get; set; }

        [NHibernate.Mapping.Attributes.OneToOne(0, Class = "OrdemServicoFrotaOrcamento", PropertyRef = "OrdemServico", Access = "property", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaOrcamento Orcamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFechamento", Column = "OSE_DATA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramada", Column = "OSE_DATA_PROGRAMADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataProgramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemVeiculo", Column = "OSE_QUILOMETRAGEM_VEICULO", TypeType = typeof(int), NotNull = true)]
        public virtual int QuilometragemVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "OSE_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CondicaoPagamento", Column = "OSE_CONDICAO_PAGAMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "OSE_MOTIVO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "OSE_DESCONTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAnteriorCancelamento", Column = "OSE_SITUACAO_ANTERIOR_CANCELAMENTO", TypeType = typeof(SituacaoOrdemServicoFrota), NotNull = false)]
        public virtual SituacaoOrdemServicoFrota? SituacaoAnteriorCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "OSE_SITUACAO", TypeType = typeof(SituacaoOrdemServicoFrota), NotNull = true)]
        public virtual SituacaoOrdemServicoFrota Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoManutencao", Column = "OSE_TIPO_MANUTENCAO", TypeType = typeof(TipoManutencaoOrdemServicoFrota), NotNull = true)]
        public virtual TipoManutencaoOrdemServicoFrota TipoManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOficina", Column = "OSE_TIPO_OFICINA", TypeType = typeof(TipoOficina), NotNull = false)]
        public virtual TipoOficina? TipoOficina { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.Pneu", Column = "PNU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.PneuEnvioReforma", Column = "PER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma PneuEnvioReforma { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacao", Column = "OSE_DATA_LIBERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReabertura", Column = "OSE_DATA_REABERTURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReabertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLimiteExecucao", Column = "OSE_DATA_LIMITE_EXECUCAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLimiteExecucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "OSE_PRIORIDADE", TypeType = typeof(PrioridadeOrdemServico), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeOrdemServico? Prioridade { get; set; }

        /// <summary>
        /// Utilizada para controlar a sumarização dos dados para análise de resultados
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "OSE_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FROTA_ORDEM_SERVICO_FECHAMENTO_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OSE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OrdemServicoFrotaFechamentoDocumento", Column = "OFD_CODIGO")]
        public virtual ICollection<OrdemServicoFrotaFechamentoDocumento> DocumentosFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OSE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OrdemServicoFrotaFechamentoProduto", Column = "OFP_CODIGO")]
        public virtual ICollection<OrdemServicoFrotaFechamentoProduto> ProdutosFechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaTipo TipoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Horimetro", Column = "OSE_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Horimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancarServicosManualmente", Column = "OSE_LANCAR_SERVICOS_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? LancarServicosManualmente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoServico", Column = "GSF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoServico GrupoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaEnviado", Column = "OSE_ALERTA_ENVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AlertaEnviado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "OSE_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoLiberacaoVeiculo", Column = "OSE_MOTIVO_LIBERACAO_VEICULO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MotivoLiberacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipoLocalManutencao", Column = "OTM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaTipoLocalManutencao TipoLocalManutencao { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoTipoManutencao
        {
            get { return TipoManutencao.ObterDescricao(); }
        }
    }
}
