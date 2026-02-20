using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO", EntityName = "CentroCarregamento", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamento", NameType = typeof(CentroCarregamento))]
    public class CentroCarregamento : EntidadeBase, IEquatable<CentroCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CEC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CEC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CEC_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocas", Column = "CEC_NUMERO_DOCAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDocas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicarTemposVeiculos", Column = "CEC_INDICAR_TEMPOS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicarTemposVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAguardarConfirmacaoTransportador", Column = "CEC_TEMPO_AGUARDAR_CONFIRMACAO_TRANSPORTADOR", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoAguardarConfirmacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAguardarAprovacaoTransportador", Column = "CEC_TEMPO_AGUARDAR_APROVACAO_TRANSPORTADOR", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoAguardarAprovacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente", Column = "CEC_TEMPO_AGUARDAR_CONFIRMACAO_TRANSPORTADOR_PARA_CARGA_LIBERADA_AUTOMATICAMENTE", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_LIBERAR_PARA_COTACAO_APOS_LIMITE_CONFIRMACAO_TRANSPORTADOR_PARA_CARGA_LIBERADA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoBloqueioEscolhaTransportador", Column = "CEC_TEMPO_BLOQUEIO_ESCOLHA_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoBloqueioEscolhaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEmMinutosLiberacao", Column = "CEC_TEMPO_LIBERACAO_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEmMinutosLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_SEGUNDA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_TERCA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_QUARTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_QUINTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_SEXTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_SABADO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_DOMINGO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoDomingo { get; set; }


        //capacidade carregamento cubagem - Definido quando selecionado capacidade por Volume e Cubagem

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_CUBAGEM_SEGUNDA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagemSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_CUBAGEM_TERCA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagemTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_CUBAGEM_QUARTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagemQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_CUBAGEM_QUINTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagemQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_CUBAGEM_SEXTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagemSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_CUBAGEM_SABADO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagemSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAPACIDADE_CARREGAMENTO_CUBAGEM_DOMINGO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagemDomingo { get; set; }

        //

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_ATRASO_SEGUNDA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaAtrasoSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_ATRASO_TERCA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaAtrasoTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_ATRASO_QUARTA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaAtrasoQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_ATRASO_QUINTA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaAtrasoQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_ATRASO_SEXTA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaAtrasoSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_ATRASO_SABADO", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaAtrasoSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_ATRASO_DOMINGO", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaAtrasoDomingo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_JANELA_CARREGAMENTO_ABA_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool JanelaCarregamentoAbaPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_JANELA_CARREGAMENTO_ABA_EXCEDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool JanelaCarregamentoAbaExcedentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_JANELA_CARREGAMENTO_ABA_RESERVAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool JanelaCarregamentoAbaReservas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_JANELA_CARREGAMENTO_EXIBIR_SITUACAO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool JanelaCarregamentoExibirSituacaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITE_MARCAR_CARGA_COMO_NAO_COMPARECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteMarcarCargaComoNaoComparecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_ESCOLHER_HORARIO_CARREGAMENTO_POR_LISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscolherHorarioCarregamentoPorLista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIBIR_VISUALIZACAO_DOS_TIPOS_DE_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirVisualizacaoDosTiposDeOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteTransportadorVisualizarMenorLanceLeilao", Column = "CEC_PERMITE_TRANSPORTADOR_VISUALIZAR_MENOR_LANCE_LEILAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTransportadorVisualizarMenorLanceLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes", Column = "CEC_PERMITE_TRANSPORTADOR_INFORMAR_SOMENTE_VEICULOS_RESTRITOS_AOS_CLIENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga", Column = "CEC_NAO_PERMITE_TRANSPORTADOR_INFORMAR_HORARIO_SE_CLIENTE_POSSUI_JANELA_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITE_TRANSPORTADOR_SELECIONAR_HORARIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTransportadorSelecionarHorarioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_INTERVALO_SELECAO_HORARIO_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloSelecaoHorarioCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TEMPO_MAXIMO_MODIFICAR_HORARIO_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMaximoModificarHorarioCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TOLERANCIA_DATA_RETROATIVA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaDataRetroativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_LIMITE_ALTERACOES_HORARIO_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteAlteracoesHorarioTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_BLOQUEAR_COMPONENTES_FRETE_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearComponentesDeFreteJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIBIR_NOTAS_FISCAIS_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNotasFiscaisJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_BLOQUEAR_TROCA_DATA_LISTA_HORARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearTrocaDataListaHorarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIBIR_DADOS_AVANCADOS_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDadosAvancadosJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITIR_TRANSPORTADOR_IMPRIMIR_ORDEM_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorImprimirOrdemColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_RETORNAR_JANELA_CARREGAMENTO_PARA_AGUARDANDO_LIBERACAO_TRANSPORTADORES_APOS_REJEICAO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarJanelaCarregamentoParaAgLiberacaoParaTransportadoresAposRejeicaoDoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RepassarCargaCasoNaoExistaVeiculoDisponivel", Column = "CEC_REPASSAR_CARGA_CASO_NAO_EXISTA_VEICULO_DISPONIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RepassarCargaCasoNaoExistaVeiculoDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_DATA_CARREGAMENTO_OBRIGATORIA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DataCarregamentoObrigatoriaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_NAO_PERMITIR_ALTERAR_DATA_CARREGAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAlterarDataCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportador", Column = "CEC_TIPO_TRANSPORTADOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento TipoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportadorTerceiro", Column = "CEC_TIPO_TRANSPORTADOR_TERCEIRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento TipoTransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_JANELA_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJanelaCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJanelaCarregamento TipoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_PEDIDO_MONTAGEM_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoMontagemCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoMontagemCarregamento TipoPedidoMontagemCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_EDICAO_PALLET_PRODUTO_MONTAGEM_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEdicaoPalletProdutoMontagemCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEdicaoPalletProdutoMontagemCarregamento TipoEdicaoPalletProdutoMontagemCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIBIR_SOMENTE_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirSomenteJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_ORDENACAO_JANELA_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOrdenacaoJanelaCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOrdenacaoJanelaCarregamento TipoOrdenacaoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarCargaManualmenteParaTransportadores", Column = "CEC_LIBERAR_CARGA_MANUALMENTE_PARA_TRANSPORTADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCargaManualmenteParaTransportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarCargaManualmenteParaTransportadoresTerceiros", Column = "CEC_LIBERAR_CARGA_MANUALMENTE_PARA_TRANSPORTADORES_TERCEIROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCargaManualmenteParaTransportadoresTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITIR_MATRIZ_SELECIONAR_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMatrizSelecionarFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarCargaAutomaticamenteParaTransportadoras", Column = "CEC_LIBERAR_CARGA_AUTOMATICAMENTE_TRANSPORTADORAS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberarCargaAutomaticamenteParaTransportadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarCargaAutomaticamenteParaTransportadorasTerceiros", Column = "CEC_LIBERAR_CARGA_AUTOMATICAMENTE_TRANSPORTADORAS_TERCEIROS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberarCargaAutomaticamenteParaTransportadorasTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarCargaAutomaticamenteParaTransportadorasForaRota", Column = "CEC_LIBERAR_CARGA_AUTOMATICAMENTE_TRANSPORTADORAS_FORA_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCargaAutomaticamenteParaTransportadorasForaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_AGUARDAR_CONFIRMACAO_TRANSPORTADOR_PARA_CARGA_LIBERADA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirLiberarCargaTransportadorExclusivo", Column = "CEC_PERMITIR_LIBERAR_CARGA_TRANSPORTADOR_EXCLUSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLiberarCargaTransportadorExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirTransportadorInformarValorFrete", Column = "CEC_PERMITIR_TRANSPORTADOR_INFORMAR_VALOR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermitirTransportadorInformarValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ManterComponentesTabelaFrete", Column = "CEC_MANTER_COMPONENTES_TABELA_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ManterComponentesTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCapacidadeCarregamentoPorPeso", Column = "CEC_UTILIZAR_CAPACIDADE_CARREGAMENTO_POR_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCapacidadeCarregamentoPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_CAPACIDADE_CARREGAMENTO_POR_PESO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCapacidadeCarregamentoPorPeso), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCapacidadeCarregamentoPorPeso? TipoCapacidadeCarregamentoPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_CAPACIDADE_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCapacidadeCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCapacidadeCarregamento? TipoCapacidadeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_LIMITE_CARREGAMENTOS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.LimiteCarregamentosCentroCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.LimiteCarregamentosCentroCarregamento LimiteCarregamentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CEC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearVeiculoSemEspelhamento", Column = "CEC_BLOQUEAR_VEICULO_SEM_ESPELHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearVeiculoSemEspelhamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearVeiculoSemEspelhamentoTelaCarga", Column = "CEC_BLOQUEAR_VEICULO_SEM_ESPELHAMENTO_TELA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearVeiculoSemEspelhamentoTelaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailParaTransportadorAoDisponibilizarCarga", Column = "CEC_ENVIAR_EMAIL_PARA_TRANSPORTADOR_AO_DISPONIBILIZAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailParaTransportadorAoDisponibilizarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CEC_LATIDUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CEC_LONGITUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_LIMITE_CARGAS_POR_MOTORISTA_POR_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteCargasPorMotoristaPorDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_LIMITE_CARGAS_POR_VEICULO_POR_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteCargasPorVeiculoPorDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaMinimaEntrarFilaCarregamento", Column = "CEC_DISTANCIA_MINIMA_ENTRAR_FILA_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaMinimaEntrarFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteCargasPorLocalCarregamento", Column = "CEC_LIMITE_CARGAS_POR_LOCAL_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteCargasPorLocalCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TEMPO_ENCOSTA_DOCA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEncostaDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TEMPO_TOLERANCIA_PEDIDO_ROTEIRIZAR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoToleranciaPedidoRoteirizar { get; set; }

        /// <summary>
        /// Atributo utilizado para processo de montagem de carga automático, para validar o máximo de destinos de um carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_QTDE_MAX_ENTREGAS_ROTEIRIZAR", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaEntregasRoteirizar { get; set; }

        /// <summary>
        /// Atributo utilizado para processo de montagem de carga automático, para validar o máximo de destinos de um carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_QTDE_MAX_PEDIDOS_SESSAO_ROTEIRIZAR", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaPedidosSessaoRoteirizar { get; set; }

        /// <summary>
        /// Atributo utilizado para processo de montagem de carga automático, para considerar o tipo de veiculo que o cliente recebe
        /// Não considera quantidade de disponibilidade de frota do centro de carregamento
        /// Sistema irá fechar as cargas da maior capacidade de veiculo do centro de descarga do clietne.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_UTILIZAR_DISP_FROTA_CENTRO_DESC_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDispFrotaCentroDescCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CONSIDERAR_TEMPO_DESLOCAMENTO_PRIMEIRA_ENTREGA_ROTEIRIZAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarTempoDeslocamentoPrimeiraEntrega { get; set; }

        /// <summary>
        /// Attibuto utilizando ao gerar os carregamentos automáticos para não gerar carregamentos alem da disponibilidade de frota para 
        /// o modelo veicular padrão do tipo de carga dos pedidos.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_GERAR_CARREGAMENTO_ALEM_DISP_FROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCarregamentosAlemDaDispFrota { get; set; }

        /// <summary>
        /// Utilizando para identificar se o centro de carregametno faz montagem de carregamento separando pedidos/produto.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "MontagemCarregamentoPedidoProduto", Column = "CEC_MONTAGEM_CARREGAMENTO_PEDIDO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCarregamentoPedidoProduto { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "MontagemCarregamentoPedidoIntegral", Column = "CEC_MONTAGEM_CARREGAMENTO_PEDIDO_INTEGRAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCarregamentoPedidoIntegral { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "MontagemCarregamentoColetaEntrega", Column = "CEC_MONTAGEM_CARREGAMENTO_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool MontagemCarregamentoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRoteirizacaoColetaEntrega", Column = "CEC_TIPO_ROTEIRIZACAO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRoteirizacaoColetaEntrega), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRoteirizacaoColetaEntrega TipoRoteirizacaoColetaEntrega { get; set; }

        /// <summary>
        /// Utilizando para identificar se o centro de carregametno faz montagem de carregamento por otimização utilizando GoogleOrTools (Vehicle Routing Problem).
        /// Quando for informado, não roteiriza por pedido/produto e nem olha para disponibilidade frota cliente..
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_MONTAGEM_CARREGAMENTO_VRP", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP TipoMontagemCarregamentoVRP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_SIMULADOR_FRETE_CRITERIO_SELECAO_TRANSPORTADOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SimuladorFreteCriterioSelecaoTransportador SimuladorFreteCriterioSelecaoTransportador { get; set; }

        /// <summary>
        /// Utilizado para controlar a ocupação dos veiculos ao gerar o carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_OCUPACAO_MONTAGEM_CARREGAMENTO_VRP", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOcupacaoMontagemCarregamentoVRP TipoOcupacaoMontagemCarregamentoVRP { get; set; }

        /// <summary>
        /// Utilizando para identificar q quebra de produtos no carregamento se é a nivel de unidade ou Caixa
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_NIVEL_QUEBRA_PRODUTO_ROTEIRIZAR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar NivelQuebraProdutoRoteirizar { get; set; }

        /// <summary>MontagemCarregamentoPedidoProduto
        /// Tempo máximo de veiculo em Rota utilizado quando o TipoMontagemCarregamentoVRP for TimeWindow.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_MONTAGEM_CARREGAMENTO_TEMPO_MAX_CARGA_ROTA", TypeType = typeof(int), NotNull = false)]
        public virtual int CarregamentoTempoMaximoRota { get; set; }

        /// <summary>
        /// Se é para agrupar pedidos do mesmo destinatário.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_MONTAGEM_CARREGAMENTO_AGRUPAR_PEDIDOS_MESMO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparPedidosMesmoDestinatario { get; set; }

        /// <summary>
        /// Utilizado para roteirização de leite, onde deve-se gerar carregamentos para 2 dias por coletas 24 e 48 horas.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_MONTAGEM_CARREGAMENTO_GERAR_DOIS_DIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCarregamentoDoisDias { get; set; }

        /// <summary>
        /// Utilizando para apresentar o Layout resumo de carregamentos na tela de montagem carga mapa. aba resumo carregamentos...
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TIPO_RESUMO_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoResumoCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoResumoCarregamento TipoResumoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_DIAS_ADICIONAIS_ALOCACAO_CARGA_JANELA_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAdicionaisAlocacaoCargaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargasComoExcedentesNaJanela", Column = "CEC_CARGAS_COMO_EXCEDENTE_JANELA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CargasComoExcedentesNaJanela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_VINCULAR_MOTORISTA_FILA_CARREGAMENTO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularMotoristaFilaCarregamentoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIBIR_DETALHES_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDetalhesCargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_OCULTAR_EDICAO_DATA_HORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarEdicaoDataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_UTILIZAR_CONTROLE_MANOBRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarControleManobra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITIR_SELECAO_PERIODO_CARREGAMENTO_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecaoPeriodoCarregamentoJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITIR_ALTERAR_MODELO_VEICULAR_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITIR_INFORMAR_AREA_VEICULO_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarAreaVeiculoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_GERAR_GUARITA_MESMO_SEM_VEICULO_INFORMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarGuaritaMesmoSemVeiculoInformado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_SE_DATA_INFORMADA_FOR_INFERIOR_DATA_ATUAL_UTILIZAR_DATA_ATUAL_COMO_REFERENCIA_HORARIO_INICIAL_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeDataInformadaForInferiorDataAtualUtilizarDataAtualComoReferenciaHorarioInicialJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_GERAR_JANELA_CARREGAMENTO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarJanelaCarregamentoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITIR_GERACAO_JANELA_PARA_CARGA_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGeracaoJanelaParaCargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_UTILIZAR_NUMERO_REDUZIDO_DE_COLUNAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeroReduzidoDeColunas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManobraAcao", Column = "MAC_CODIGO_ACAO_MANOBRA_PADRAO_INICIO_CARREGAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManobraAcao AcaoManobraPadraoInicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManobraAcao", Column = "MAC_CODIGO_ACAO_MANOBRA_PADRAO_INICIO_REVERSA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManobraAcao AcaoManobraPadraoInicioReversa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManobraAcao", Column = "MAC_CODIGO_ACAO_MANOBRA_PADRAO_FIM_CARREGAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManobraAcao AcaoManobraPadraoFimCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManobraAcao", Column = "MAC_CODIGO_ACAO_MANOBRA_PADRAO_FIM_REVERSA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManobraAcao AcaoManobraPadraoFimReversa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMaximoDiferencaValorCotacao", Column = "CEC_PERCENTUAL_MAXIMO_DIFERENCA_VALOR_COTACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualMaximoDiferencaValorCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMinimoDiferencaValorCotacao", Column = "CEC_PERCENTUAL_MINIMO_DIFERENCA_VALOR_COTACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualMinimoDiferencaValorCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaPesoCarregamento", Column = "CEC_PERCENTUAL_TOLERANCIA_PESO_CARREGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualToleranciaPesoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteRecorrencia", Column = "CEC_LIMITE_RECORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteRecorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TEMPO_MINUTOS_ESCOLHA_AUTOMATICA_COTACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinutosEscolhaAutomaticaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PONTUACAO_DESCONTAR_TRANSPORTADOR_POR_ESCOLHA_AUTOMATICA_COTACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExibirValorFretePortalTransportador", Column = "CEN_NAO_EXIBIR_VALOR_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoExibirValorFretePortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITIR_INFORMAR_VALOR_FRETE_CARGAS_ATRIBUIDAS_AO_TRANSPORTADOR_NA_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoToleranciaChegadaAtraso", Column = "CEC_TEMPO_TOLERANCIA_CHEGADA_ATRASO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoToleranciaChegadaAtraso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TEMPO_TOLERANCIA_CARGA_FECHADA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoToleranciaCargaFechada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_BLOQUEIO_MARCACAO_INTERESSE_ANTES_DIAS_VENCIMENTO_CERTIFICADO_APOLICE_SEGURO", TypeType = typeof(int), NotNull = false)]
        public virtual int BloqueioMarcacaoInteresseAntesDiasVencimentoCertificadoApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarIntegracaoGR", Column = "CEN_NAO_VALIDAR_INTEGRACAO_GR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoValidarIntegracaoGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TEMPO_PADRAO_TERMINO_CARREGAMENTO_PARA_VALIDAR_DISPONIBILIDADE_DESCARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIGIR_TERMO_ACEITE_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirTermoAceiteTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITE_TRANSPORTADOR_INFORMAR_OBSERVACOES_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIGIR_TRANSPORTADOR_CONFIRMAR_MDFE_NAO_ENCERRADO_FORA_DO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirTransportadorConfirmarMDFeNaoEncerradoForaDoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIGIR_TRANSPORTADOR_INFORMAR_MOTIVO_AO_REJEITAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirTransportadorInformarMotivoAoRejeitarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_NOTIFICAR_SOMENTE_ALTERACAO_COTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarSomenteAlteracaoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_NAO_ENVIAR_NOTIFICACAO_CARGA_REJEITADA_PARA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarNotificacaoCargaRejeitadaParaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TERMO_ACEITE", Type = "StringClob", NotNull = false)]
        public virtual string TermoAceite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAMPOS_VISIVEIS_TRANSPORTADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CamposVisiveisTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CAMPOS_VISIVEIS_JANELA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CamposVisiveisJanela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAdvertenciaTransportador", Column = "MAT_CODIGO_CHEGADA_ATRASO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador MotivoAdvertenciaChegadaEmAtraso { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "UsuariosNotificacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_USUARIO_NOTIFICACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> UsuariosNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TIPO_CARGA_BLOQUEAR_LIBERACAO_AUTOMATICA_PARA_TRANSPORTADORAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Veiculos { get; set; }

        /// <summary>
        /// são os transportadores autorizados a liberar a carga para faturamento após informar o veículo e o motorista.
        /// </summary>
        [NHibernate.Mapping.Attributes.Set(0, Name = "TransportadoresAutorizadosLiberarFaturamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADOR_AUTORIZADO_LIBERAR_FATURAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> TransportadoresAutorizadosLiberarFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TemposCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TEMPO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TempoCarregamento", Column = "TEC_CODIGO")]
        public virtual ICollection<TempoCarregamento> TemposCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_ENVIAR_NOTIFICACOES_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNotificacoesPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Emails", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_EMAIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoEmail", Column = "CCE_CODIGO")]
        public virtual ICollection<CentroCarregamentoEmail> Emails { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PeriodosCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_PERIODO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoCarregamento", Column = "PEC_CODIGO")]
        public virtual ICollection<PeriodoCarregamento> PeriodosCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PrevisoesCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_PREVISAO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PrevisaoCarregamento", Column = "PRC_CODIGO")]
        public virtual ICollection<PrevisaoCarregamento> PrevisoesCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DisponibilidadesFrota", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_DISPONIBILIDADE_FROTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoDisponibilidadeFrota", Column = "CDF_CODIGO")]
        public virtual ICollection<CentroCarregamentoDisponibilidadeFrota> DisponibilidadesFrota { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ExcecoesCapacidadeCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Inverse = true, Table = "T_CENTRO_CARREGAMENTO_EXCECAO_CAPACIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ExcecaoCapacidadeCarregamento", Column = "CEX_CODIGO")]
        public virtual ICollection<ExcecaoCapacidadeCarregamento> ExcecoesCapacidadeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ExpedicoesCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Inverse = true, Table = "T_CENTRO_CARREGAMENTO_CONTROLE_EXPEDICAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ExpedicaoCarregamento", Column = "EXC_CODIGO")]
        public virtual ICollection<ExpedicaoCarregamento> ExpedicoesCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "OperadoresLogistica", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_CENTRO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorLogistica", Column = "OPL_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica> OperadoresLogistica { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Docas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_DOCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoDoca", Column = "CCD_CODIGO")]
        public virtual ICollection<CentroCarregamentoDoca> Docas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AcoesManobra", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_MANOBRA_ACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoManobraAcao", Column = "CMA_CODIGO")]
        public virtual ICollection<CentroCarregamentoManobraAcao> AcoesManobra { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoTransportador", Column = "CTR_CODIGO")]
        public virtual ICollection<CentroCarregamentoTransportador> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TransportadoresTerceiros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES_TERCEIROS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoTransportadorTerceiro", Column = "CTT_CODIGO")]
        public virtual ICollection<CentroCarregamentoTransportadorTerceiro> TransportadoresTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LimitesCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_LIMITE_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoLimiteCarregamento", Column = "CLC_CODIGO")]
        public virtual ICollection<CentroCarregamentoLimiteCarregamento> LimitesCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.OneToOne(0, Class = "CentroCarregamentoConfigPadrao", PropertyRef = "CentroCarregamento", Access = "property", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamentoConfigPadrao ConfiguracaoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicioViagemPrevista", Column = "CEC_HORA_INICIO_VIAGEM_PREVISTA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicioViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_HABILITAR_TERMO_CHEGADA_HORARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTermoChegadaHorario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_TERMO_CHEGADA_HORARIO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string TermoChegadaHorario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasTrabalho", Column = "CEC_HORAS_TRABALHO", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasTrabalho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_EXIBIR_FILIAL_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirFilialJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_NAO_PERMITIR_GERAR_CARREGAMENTOS_QUANDO_EXISTIR_PEDIDOS_ATRASADOS_AGENDAMENTO_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PERMITE_TRANSPORTADOR_VISUALIZAR_COLOCACAO_DENTRE_LANCES_LEILAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTransportadorVisualizarColocacaoDentreLancesLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_DIAS_ATRASO_PERMITIDOS_PEDIDOS_AGENDAMENTO_PEDIDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAtrasoPermitidosPedidosAgendamentoPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoBloquearCapacidadeExcedida", Column = "CEC_NAO_BLOQUEAR_CAPACIDADE_EXCEDIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoBloquearCapacidadeExcedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarNotificacoesCargasRejeitadasPorEmail", Column = "CEC_ENVIAR_NOTIFICACOES_CARGAS_REJEITADAS_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNotificacoesCargasRejeitadasPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailAlertaLeilaoParaTransportadorOfertado", Column = "CEC_ENVIAR_EMAIL_ALERTA_LEILAO_PARA_TRANSPORTADOR_OFERTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailAlertaLeilaoParaTransportadorOfertado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente", Column = "CEC_ENVIAR_EMAIL_QUANDO_VENCEDOR_NAO_FOR_DEFINIDO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRetira", Column = "CEC_OBSERVACAO_RETIRA", TypeType = typeof(string), NotNull = false, Length = 5000)]
        public virtual string ObservacaoRetira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirConfirmacaoParticipacaoLeilao", Column = "CEC_EXIGE_CONFIRMACAO_PARTICIPACAO_LEILAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirConfirmacaoParticipacaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirQueTransportadorAltereHorarioDoCarregamento", Column = "CEC_PERMITIR_QUE_TRANSPORTADOR_ALTERE_HORARIO_DO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirQueTransportadorAltereHorarioDoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemConfirmacaoLeilao", Column = "CEC_MENSAGEM_CONFIRMACAO_LEILAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MensagemConfirmacaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente", Column = "CEC_ESCOLHER_PRIMEIRO_TRANSPORTADOR_AO_MARCAR_INTERESSE_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarRegraParaOfertarCarga", Column = "CEC_ATIVAR_REGRA_PARA_OFERTAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarRegraParaOfertarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados", Column = "CEC_PERMITE_TRANSPORTADOR_ENVIAR_INTEGRACAO_GR_DOCUMENTOS_REPROVADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota", Column = "CEC_LIBERAR_JANELA_CARREGAMENTO_SOMENTE_COM_AGENDAMENTO_REALIZADO_CLIENTE_AGENDADO_SEM_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido", Column = "CEC_VALIDAR_SE_DATA_CARREGAMENTO_ATENDE_AGENDAMENTO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento", Column = "CEC_PEGAR_OBRIGATORIAMENTE_HORARIO_DA_PRIMEIRA_COLETA_PARA_DATA_DE_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirAgendarCargasNoMesmoDia", Column = "CEC_NAO_PERMITIR_AGENDAR_CARGAS_NO_MESMO_DIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAgendarCargasNoMesmoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteDeCargasAtivasPorMotorista", Column = "CEC_LIMITE_DE_CARGAS_ATIVAS_POR_MOTORISTA", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteDeCargasAtivasPorMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado", Column = "CEC_ENVIAR_EMAIL_CONFIRMACAO_AGENDAMENTO_QUANDO_SITUACAO_AGENDAMENTO_FOR_FINALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_CONSIDERAR_PESO_PALLET_PESO_TOTAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarPesoPalletPesoTotalCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirChecklistAoConfirmarDadosTransporteMultiTransportador", Column = "CEC_EXIGIR_CHECKLIST_AO_CONFIRMAR_DADOS_TRANSPORTE_MULTITRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirChecklistAoConfirmarDadosTransporteMultiTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_NAO_GERAR_CARREGAMENTO_FORA_CAPACIDADE_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCarregamentoForaCapacidadeModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportadorSecundario", Column = "CEC_TIPO_TRANSPORTADOR_SECUNDARIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento? TipoTransportadorSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente", Column = "CEC_TEMPO_AGUARDAR_INTERESSE_TRANSPORTADOR_PARA_CARGA_LIBERADA_AUTOMATICAMENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarControleVisualizacaoTransportadorasTerceiros", Column = "CEC_GERAR_CONTROLE_VISUALIZACAO_TRANSPORTADOR_TERCEIROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarControleVisualizacaoTransportadorasTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEC_PREENCHER_AUTOMATICAMENTE_DADOS_CENTRO_TELA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreencherAutomaticamenteDadosCentroTelaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportadorTerceiroSecundario", Column = "CEC_TIPO_TRANSPORTADOR_TERCEIRO_SECUNDARIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento? TipoTransportadorTerceiroSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente", Column = "CEC_TEMPO_AGUARDAR_INTERESSE_TRANSPORTADOR_TERCEIRO_PARA_CARGA_LIBERADA_AUTOMATICAMENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAguardarConfirmacaoTransportadorTerceiroParaCargaLiberadaAutomaticamente", Column = "CEC_TEMPO_AGUARDAR_CONFIRMACAO_TRANSPORTADOR_TERCEIRO_PARA_CARGA_LIBERADA_AUTOMATICAMENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoAguardarConfirmacaoTransportadorTerceiroParaCargaLiberadaAutomaticamente { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual int ObterCapacidadeCarregamento(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana, bool porCubagem)
        {
            switch (diaSemana)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Segunda:
                    return !porCubagem ? CapacidadeCarregamentoSegunda : CapacidadeCarregamentoCubagemSegunda;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Terca:
                    return !porCubagem ? CapacidadeCarregamentoTerca : CapacidadeCarregamentoCubagemTerca;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quarta:
                    return !porCubagem ? CapacidadeCarregamentoQuarta : CapacidadeCarregamentoCubagemQuarta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quinta:
                    return !porCubagem ? CapacidadeCarregamentoQuinta : CapacidadeCarregamentoCubagemQuinta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sexta:
                    return !porCubagem ? CapacidadeCarregamentoSexta : CapacidadeCarregamentoCubagemSexta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sabado:
                    return !porCubagem ? CapacidadeCarregamentoSabado : CapacidadeCarregamentoCubagemSabado;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo:
                    return !porCubagem ? CapacidadeCarregamentoDomingo : CapacidadeCarregamentoCubagemDomingo;
                default:
                    return 0;
            }
        }

        public virtual int ObterToleranciaAtraso(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            switch (diaSemana)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Segunda:
                    return ToleranciaAtrasoSegunda;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Terca:
                    return ToleranciaAtrasoTerca;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quarta:
                    return ToleranciaAtrasoQuarta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quinta:
                    return ToleranciaAtrasoQuinta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sexta:
                    return ToleranciaAtrasoSexta;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sabado:
                    return ToleranciaAtrasoSabado;
                case ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo:
                    return ToleranciaAtrasoDomingo;
                default:
                    return 0;
            }
        }

        public virtual bool Equals(CentroCarregamento other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion
    }
}
