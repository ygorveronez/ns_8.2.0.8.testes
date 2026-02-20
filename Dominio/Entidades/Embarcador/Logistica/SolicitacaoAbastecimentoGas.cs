using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_ABASTECIMENTO_GAS", EntityName = "SolicitacaoAbastecimentoGas", Name = "Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas", NameType = typeof(SolicitacaoAbastecimentoGas))]
    public class SolicitacaoAbastecimentoGas : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SAG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Obsolete("Alterado para ser por Cliente", true)]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Base { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteBase { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoAbastecimentoGasJustificativa", Column = "AGJ_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SolicitacaoAbastecimentoGasJustificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMedicao", Column = "SAG_DATA_MEDICAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataMedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "SAG_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaAlteracao", Column = "SAG_DATA_ULTIMA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataUltimaAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Abertura", Column = "SAG_ABERTURA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Abertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoBombeio", Column = "SAG_PREVISAO_BOMBEIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PrevisaoBombeio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoTransferenciaRecebida", Column = "SAG_PREVISAO_TRANSFERENCIA_RECEBIDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PrevisaoTransferenciaRecebida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoDemandaDomiciliar", Column = "SAG_PREVISAO_DEMANDA_DOMICILIAR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PrevisaoDemandaDomiciliar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoDemandaEmpresarial", Column = "SAG_PREVISAO_DEMANDA_EMPRESARIAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PrevisaoDemandaEmpresarial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstoqueUltrasystem", Column = "SAG_ESTOQUE_ULTRASYSTEM", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal EstoqueUltrasystem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoTransferenciaEnviada", Column = "SAG_PREVISAO_TRANSFERENCIA_ENVIADA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PrevisaoTransferenciaEnviada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DensidadeAberturaDia", Column = "SAG_DENSIDADE_ABERTURA_DIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal DensidadeAberturaDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoFechamento", Column = "SAG_PREVISAO_FECHAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PrevisaoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumeRodoviarioCarregamentoProximoDia", Column = "SAG_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal VolumeRodoviarioCarregamentoProximoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumeRodoviarioCarregamentoProximoDiaOriginal", Column = "SAG_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA_ORIGINAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal VolumeRodoviarioCarregamentoProximoDiaOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoBombeioProximoDia", Column = "SAG_PREVISAO_BOMBEIO_PROXIMO_DIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PrevisaoBombeioProximoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilidadeTransferenciaProximoDia", Column = "SAG_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal DisponibilidadeTransferenciaProximoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoRestante", Column = "SAG_SALDO_RESTANTE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal SaldoRestante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAG_SITUACAO", TypeType = typeof(SituacaoAprovacaoSolicitacaoGas), NotNull = false)]
        public virtual SituacaoAprovacaoSolicitacaoGas Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionalVolumeRodoviarioCarregamentoProximoDia", Column = "SAG_ADICIONAL_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal AdicionalVolumeRodoviarioCarregamentoProximoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionalDisponibilidadeTransferenciaProximoDia", Column = "SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal AdicionalDisponibilidadeTransferenciaProximoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAdicaoQuantidade", Column = "SAG_DATA_ADICAO_QUANTIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAdicaoQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ADICAO_QUANTIDADE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioAdicaoQuantidade { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"{ClienteBase?.Descricao} - {DataMedicao.ToString("dd/MM/yyyy HH:mm")}";
            }
        }

        public virtual decimal VolumeRodoviarioCarregamentoProximoDiaTotal
        {
            get
            {
                return VolumeRodoviarioCarregamentoProximoDia + AdicionalVolumeRodoviarioCarregamentoProximoDia;
            }
        }

        public virtual decimal DisponibilidadeTransferenciaProximoDiaTotal
        {
            get
            {
                return DisponibilidadeTransferenciaProximoDia + AdicionalDisponibilidadeTransferenciaProximoDia;
            }
        }
    }
}
