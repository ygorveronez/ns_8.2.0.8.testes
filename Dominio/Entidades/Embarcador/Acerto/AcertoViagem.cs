using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_DE_VIAGEM", EntityName = "AcertoViagem", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoViagem", NameType = typeof(AcertoViagem))]
    public class AcertoViagem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_OPERADOR_INICIO_ACERTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario OperadorInicioAcerto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_OPERADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ACV_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAcerto", Column = "ACV_DATA_ACERTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFechamento", Column = "ACV_DATA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "ACV_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "ACV_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ACV_OBSERVACAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ACV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Etapa", Column = "ACV_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AprovacaoAbastecimento", Column = "ECV_APROVACAO_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AprovacaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AprovacaoPedagio", Column = "ECV_APROVACAO_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AprovacaoPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaSalvo", Column = "ACV_CARGA_SALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaSalvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AbastecimentoSalvo", Column = "ACV_ABASTECIMENTO_SALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AbastecimentoSalvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedagioSalvo", Column = "ACV_PEDAGIO_SALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedagioSalvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DespesaSalvo", Column = "ACV_DESPESA_SALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DespesaSalvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaSalvo", Column = "ACV_OCORRENCIA_SALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaSalvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiariaSalvo", Column = "ACV_DIARIA_SALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiariaSalvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFrota", Column = "VEI_NUMERO_FROTA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroFrota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.SegmentoVeiculo SegmentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalAlimentacaoRepassado", Column = "ACV_VALOR_TOTAL_ALIMENTACAO_REPASSADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalAlimentacaoRepassado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAlimentacaoRepassado", Column = "ACV_VALOR_ALIMENTACAO_REPASSADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAlimentacaoRepassado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAlimentacaoComprovado", Column = "ACV_VALOR_ALIMENTACAO_COMPROVADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAlimentacaoComprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAlimentacaoSaldo", Column = "ACV_VALOR_ALIMENTACAO_SALDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAlimentacaoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalAdiantamentoRepassado", Column = "ACV_VALOR_TOTAL_ADIANTAMENTO_REPASSADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalAdiantamentoRepassado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamentoRepassado", Column = "ACV_VALOR_ADIANTAMENTO_REPASSADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdiantamentoRepassado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamentoComprovado", Column = "ACV_VALOR_ADIANTAMENTO_COMPROVADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdiantamentoComprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamentoSaldo", Column = "ACV_VALOR_ADIANTAMENTO_SALDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdiantamentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACV_QUANTIDADE_DIAS_FOLGA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasFolga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioFolga", Column = "ACV_DATA_INICIO_FOLGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioFolga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalFolga", Column = "ACV_DATA_FINAL_FOLGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalFolga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColaboradorLancamento", Column = "CLS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Usuarios.Colaborador.ColaboradorLancamento ColaboradorLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoAtualAlimentacaoMotorista", Column = "ACV_SALDO_ATUAL_ALIMENTACAO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoAtualAlimentacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoAtualOutrasDepesasMotorista", Column = "ACV_SALDO_ATUAL_OUTRAS_DESPESAS_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoAtualOutrasDepesasMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoPrevistoAlimentacaoMotorista", Column = "ACV_SALDO_PREVISTO_ALIMENTACAO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoPrevistoAlimentacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoPrevistoOutrasDepesasMotorista", Column = "ACV_SALDO_PREVISTO_OUTRAS_DESPESAS_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoPrevistoOutrasDepesasMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FolgasAprovadas", Column = "ACV_FOLGAS_APROVADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FolgasAprovadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalGuarita", Column = "ACV_DATA_FINAL_GUARITA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VariacaoCambial", Column = "ACV_VARIACAO_CAMBIAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal VariacaoCambial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VariacaoCambialReceita", Column = "ACV_VARIACAO_CAMBIAL_RECEITA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal VariacaoCambialReceita { get; set; }

        /// <summary>
        /// Utilizada para controlar a sumarização dos dados para análise de resultados
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "ACV_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cheque", Column = "CHQ_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.Cheque Cheque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaRecebimentoMotoristaAcerto", Column = "ACV_FORMA_RECEBIMENTO_MOTORISTA_ACERTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaRecebimentoMotoristaAcerto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaRecebimentoMotoristaAcerto FormaRecebimentoMotoristaAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACV_DATA_VENCIMENTO_MOTORISTA_ACERTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoMotoristaAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoMotoristaAcerto", Column = "ACV_OBSERVACAO_MOTORISTA_ACERTO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string ObservacaoMotoristaAcerto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_MOTORISTA_ACERTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoMotoristaAcerto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAcertoMotorista", Column = "ACV_OBSERVACAO_ACERTO_MOTORISTA", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string ObservacaoAcertoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMovimentadoFichaMotorista", Column = "ACV_VALOR_MOVIMENTAO_FICHA_MOTORISTA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMovimentadoFichaMotorista { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "ACV_GUID_ASSINATURA", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidAssinatura { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoCarga", Column = "ACC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoCarga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoVeiculo", Column = "AVV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Infracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_VEICULO_TABELA_INFRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoViagemInfracao", Column = "AVI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao> Infracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Tacografos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_VEICULO_TACOGRAFO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoViagemTacografo", Column = "AVT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo> Tacografos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Abastecimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_ABASTECIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoAbastecimento", Column = "ACB_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> Abastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedagios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoPedagio", Column = "ACP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> Pedagios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "UsuariosCargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_USUARIO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoUsuarioCarga", Column = "AUC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoUsuarioCarga> UsuariosCargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Bonificacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_BONIFICACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoBonificacao", Column = "ABO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> Bonificacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Descontos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_DESCONTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoDesconto", Column = "ADE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoDesconto> Descontos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Logs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_LOG_USUARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoLog", Column = "ALO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoLog> Logs { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "OutrasDespesas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_OUTRA_DESPESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoOutraDespesa", Column = "AOD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> OutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ocorrencias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoOcorrencia", Column = "AOC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia> Ocorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Diarias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_DIARIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoDiaria", Column = "ACD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria> Diarias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Adiantamentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_ADIANTAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoAdiantamento", Column = "ADI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento> Adiantamentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cheques", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_CHEQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoCheque", Column = "ACH_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoCheque> Cheques { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DevolucoesMoedaEstrangeira", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_DEVOLUCAO_MOEDA_ESTRANGEIRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoDevolucaoMoedaEstrangeira", Column = "ADM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira> DevolucoesMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VariacoesCambial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_VARIACAO_CAMBIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoVariacaoCambial", Column = "AVC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial> VariacoesCambial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacasVeiculos", Formula = @"SUBSTRING((select DISTINCT ', ' + v.VEI_PLACA from t_acerto_veiculo a
                                                                                            join T_VEICULO v on v.VEI_CODIGO = a.VEI_CODIGO
                                                                                            where a.ACV_CODIGO = ACV_CODIGO FOR XML PATH('')), 3, 4000)", TypeType = typeof(string), Lazy = true)]
        public virtual string PlacasVeiculos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }


        public virtual string DescricaoEtapa
        {
            get
            {
                switch (this.Etapa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Abastecimentos:
                        return "Abastecimentos";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Acerto:
                        return "Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Cargas:
                        return "Cargas";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Ocorrencias:
                        return "Ocorrências";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento:
                        return "Fechamento do acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.OutrasDespesas:
                        return "Outras Despesas";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Diarias:
                        return "Diárias";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Pedagios:
                        return "Pedágios";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento:
                        return "Em Andamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado:
                        return "Fechado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoPeriodo
        {
            get
            {                
                return this.DataInicial.ToString("dd/MM/yyyy") + " até " + (this.DataFinal.HasValue ? this.DataFinal.Value.ToString("dd/MM/yyyy") : string.Empty);
            }
        }

        public virtual string DescricaoAprovacaoPedagio
        {
            get
            {
                if (this.AprovacaoPedagio)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public virtual string DescricaoAprovacaoAbastecimento
        {
            get
            {
                if (this.AprovacaoAbastecimento)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public virtual bool Equals(AcertoViagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
