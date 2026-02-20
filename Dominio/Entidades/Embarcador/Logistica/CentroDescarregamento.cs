using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO", EntityName = "CentroDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento", NameType = typeof(CentroDescarregamento))]
    public class CentroDescarregamento : EntidadeBase, IEquatable<CentroDescarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CED_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CED_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocas", Column = "CED_NUMERO_DOCAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDocas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportador", Column = "CED_TIPO_TRANSPORTADOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroDescarregamento), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroDescarregamento TipoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarCargaAutomaticamenteParaTransportadoras", Column = "CED_LIBERAR_CARGA_AUTOMATICAMENTE_TRANSPORTADORAS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberarCargaAutomaticamenteParaTransportadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CED_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_TEMPO_PADRAO_DE_ENTREGA ", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearJanelaDescarregamentoExcedente", Column = "CED_BLOQUEAR_JANELA_DESCARREGAMENTO_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearJanelaDescarregamentoExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirBuscarAteFimDaJanela", Column = "CED_PERMITIR_BUSCAR_NO_DIA_POSTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirBuscarAteFimDaJanela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCapacidadeDescarregamentoPorPeso", Column = "CED_UTILIZAR_CAPACIDADE_DESCARREGAMENTO_POR_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCapacidadeDescarregamentoPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCapacidadeDescarregamentoPesoLiquido", Column = "CED_UTILIZAR_CAPACIDADE_DESCARREGAMENTO_PESO_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCapacidadeDescarregamentoPesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_TIPO_CAPACIDADE_DESCARREGAMENTO_POR_PESO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCapacidadeDescarregamentoPorPeso), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCapacidadeDescarregamentoPorPeso? TipoCapacidadeDescarregamentoPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_SEGUNDA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_TERCA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_QUARTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_QUINTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_SEXTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_SABADO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_DOMINGO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoDomingo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_LIMITE_PADRAO", TypeType = typeof(int), NotNull = false)]
        public virtual int LimitePadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaPesoDescarregamento", Column = "CED_PERCENTUAL_TOLERANCIA_PESO_DESCARREGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualToleranciaPesoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_EXIBIR_JANELA_DESCARGA_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirJanelaDescargaPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_APROVAR_AUTOMATICAMENTE_DESCARGA_COM_HORARIO_DISPONIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AprovarAutomaticamenteDescargaComHorarioDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_PERMITIR_GERACAO_JANELA_PARA_CARGA_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGeracaoJanelaParaCargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_DEFINIR_NAO_COMPARECIMENTO_AUTOMATICO_APOS_PRAZO_DATA_AGENDADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_USAR_LAYOUT_AGENDAMENTO_POR_CAIXA_ITEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarLayoutAgendamentoPorCaixaItem { get; set; }

        [Obsolete("Não utilizar. Migrado para o campo TempoPadraoSugestaoHorario")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAdicionaisSugestaoDataEntrega", Column = "CED_DIAS_ADICIONAIS_SUGESTAO_DATA_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAdicionaisSugestaoDataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoPadraoSugestaoHorario", Column = "CED_TEMPO_PADRAO_SUGESTAO_HORARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoSugestaoHorario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BuscarSenhaViaIntegracao", Column = "CED_BUSCAR_SENHA_VIA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarSenhaViaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_PERMITIR_GERAR_DESCARGA_ARMAZEM_EXTERNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGerarDescargaArmazemExterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_EXIGE_APROVACAO_CARGA_PARA_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeAprovacaoCargaParaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_GERAR_FLUXO_PATIO_APOS_CONFIRMACAO_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFluxoPatioAposConfirmacaoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosPermitidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_VEICULO_PERMITIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> VeiculosPermitidos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TemposDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_TEMPO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TempoDescarregamento", Column = "TED_CODIGO")]
        public virtual ICollection<TempoDescarregamento> TemposDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LimitesAgendamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_LIMITE_AGENDAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LimiteAgendamento", Column = "LAD_CODIGO")]
        public virtual ICollection<LimiteAgendamento> LimitesAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "QuantidadesPorTipoDeCargaDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_QUANTIDADE_POR_TIPO_DE_CARGA_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "QuantidadePorTipoDeCargaDescarregamento", Column = "QPT_CODIGO")]
        public virtual ICollection<QuantidadePorTipoDeCargaDescarregamento> QuantidadesPorTipoDeCargaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Emails", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_EMAIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroDescarregamentoEmail", Column = "CDE_CODIGO")]
        public virtual ICollection<CentroDescarregamentoEmail> Emails { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PeriodosDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoDescarregamento", Column = "PED_CODIGO")]
        public virtual ICollection<PeriodoDescarregamento> PeriodosDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PrevisoesDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PREVISAO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PrevisaoDescarregamento", Column = "PRD_CODIGO")]
        public virtual ICollection<PrevisaoDescarregamento> PrevisoesDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ExcecoesCapacidadeDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Inverse = true, Table = "T_CENTRO_DESCARREGAMENTO_EXCECAO_CAPACIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ExcecaoCapacidadeDescarregamento", Column = "CEX_CODIGO")]
        public virtual ICollection<ExcecaoCapacidadeDescarregamento> ExcecoesCapacidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "OperadoresLogistica", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_CENTRO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorLogistica", Column = "OPL_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica> OperadoresLogistica { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LimitesDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_LIMITE_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroDescarregamentoLimiteDescarregamento", Column = "CLD_CODIGO")]
        public virtual ICollection<CentroDescarregamentoLimiteDescarregamento> LimitesDescarregamento { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CED_CAPACIDADE_DESCARREGAMENTO_POR_DIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CapacidadeDescaregamentoPorDia { get; set; }


        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual int ObterCapacidadeDescarregamento(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            switch (diaSemana)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Segunda:
                    return CapacidadeDescarregamentoSegunda;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Terca:
                    return CapacidadeDescarregamentoTerca;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quarta:
                    return CapacidadeDescarregamentoQuarta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quinta:
                    return CapacidadeDescarregamentoQuinta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sexta:
                    return CapacidadeDescarregamentoSexta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sabado:
                    return CapacidadeDescarregamentoSabado;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo:
                    return CapacidadeDescarregamentoDomingo;
                default:
                    return 0;
            }
        }

        public virtual bool Equals(CentroDescarregamento other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
