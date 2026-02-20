/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumOrigemOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoDocumentoOcorrencia.js" />
/// <reference path="Ocorrencia.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var configuracaoTipoOcorrencia = null;

/*
 * Declaração das Funções Públicas
 */

function CarregaTipoOcorrenciaGrid(data) {
    return executarReST("TipoOcorrencia/BuscarPorCodigo", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data)
                configuracaoTipoOcorrencia = r.Data;
            else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                configuracaoTipoOcorrencia = null;
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            configuracaoTipoOcorrencia = null;
        }
    });
}

function retornoTipoOcorrenciaGrid(data) {
    executarReST("TipoOcorrencia/BuscarPorCodigo", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data)
                retornoTipoOcorrencia(data, r.Data);
            else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                configuracaoTipoOcorrencia = null;
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            configuracaoTipoOcorrencia = null;
        }
    });
}

function tipoOcorrenciaApagado() {
    configuracaoTipoOcorrencia = null;

    _ocorrencia.TipoOcorrencia.porPeriodo = false;
    _ocorrencia.TipoOcorrencia.periodo = EnumPeriodoAcordoContratoFreteTransportador.NaoPossui;
    _ocorrencia.TipoOcorrencia.tipoEmissaoDocumentoOcorrencia = EnumTipoEmissaoDocumentoOcorrencia.Todos;
    _ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val(false);

    ocultarExibicaoCamposParametros();
    controlarPeriodoOcorrencia();
    controlarExigenciaChamadoParaAbrirOcorrencia();
}

/*
 * Declaração das Funções Privadas
 */

function retornoTipoOcorrencia(data, config) {
    var cargaPreenchida = {
        Codigo: _ocorrencia.Carga.codEntity(),
        Descricao: _ocorrencia.Carga.val(),
        Moeda: _ocorrencia.Moeda.val(),
        ValorCotacaoMoeda: _ocorrencia.ValorCotacaoMoeda.val(),
    };
    if (_ocorrencia.SituacaoOcorrencia.val() != EnumSituacaoOcorrencia.AgInformacoes && !_ocorrencia.NaoLimparCarga.val())
        limparCamposOcorrencia();
    _ocorrencia.CTesParaComplemento.visibleFade(true);
    _ocorrencia.EmitirDocumentoParaFilialEmissoraComPreCTe.val(config.EmitirDocumentoParaFilialEmissoraComPreCTe);
    _ocorrencia.TipoEmissaoDocumentoOcorrencia.val(data.TipoEmissaoDocumentoOcorrencia);

    configuracaoTipoOcorrencia = config;
    _ocorrencia.TipoOcorrencia.codEntity(data.Codigo);
    _ocorrencia.TipoOcorrencia.val(data.Descricao);
    _ocorrencia.TipoOcorrencia.entityDescription(data.Descricao);
    _ocorrencia.TipoOcorrencia.porPeriodo = data.OrigemOcorrencia != EnumOrigemOcorrencia.PorCarga;
    _ocorrencia.TipoOcorrencia.periodo = data.PeriodoOcorrencia;
    _ocorrencia.TipoOcorrencia.tipoEmissaoDocumentoOcorrencia = data.TipoEmissaoDocumentoOcorrencia;
    _ocorrencia.TipoOcorrencia.origemOcorrencia = data.OrigemOcorrencia;
    _ocorrencia.TipoOcorrencia.CalculaValorPorTabelaFrete = config.CalculaValorPorTabelaFrete;
    _ocorrencia.TipoOcorrencia.NaoCalcularValorOcorrenciaAutomaticamente = config.NaoCalcularValorOcorrenciaAutomaticamente;
    _ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga = config.OcorrenciaComplementoValorFreteCarga;
    _ocorrencia.TipoOcorrencia.FiltrarOcorrenciasPeriodoPorFilial = config.FiltrarOcorrenciasPeriodoPorFilial;
    _ocorrencia.TipoOcorrencia.NaoGerarDocumento = config.NaoGerarDocumento;
    _ocorrencia.TipoOcorrencia.PermiteSelecionarTomador = config.PermiteSelecionarTomador;
    _ocorrencia.TipoOcorrencia.GerarApenasUmComplemento = config.GerarApenasUmComplemento;
    _ocorrencia.TipoOcorrencia.PermiteInformarValor = config.PermiteInformarValor;

    controlarExibicaoCamposParametros(_ocorrencia.TipoOcorrencia.codEntity());
    controlarPeriodoOcorrencia();
    controlarExigenciaChamadoParaAbrirOcorrencia();

    if (_ocorrencia.SituacaoOcorrencia.val() != EnumSituacaoOcorrencia.AgInformacoes) {
        _ocorrencia.DividirOcorrencia.val(false);
        _ocorrencia.ObservacaoCTe.val("");
        _ocorrencia.ObservacaoCTeDestino.val("");
        _ocorrencia.ObservacaoOcorrencia.val("");
        _ocorrencia.ObservacaoOcorrenciaDestino.val("");
        _ocorrencia.ValorOcorrencia.enable(false);
        _ocorrencia.ValorOcorrencia.val("");
        _ocorrencia.ValorOcorrenciaDestino.val("0,00");

        if (_ocorrencia.TipoOcorrencia.porPeriodo) {
            _tipoSelecaoOcorrenciaPorPeriodo = true;
            LimparCampoEntity(_ocorrencia.Carga);
            FluxoOcorrenciaPorPeriodo(true);
        }
        else {
            _tipoSelecaoOcorrenciaPorPeriodo = false;
            FluxoOcorrenciaPorPeriodo(false);
            _ocorrencia.Carga.codEntity(cargaPreenchida.Codigo);
            _ocorrencia.Carga.val(cargaPreenchida.Descricao);
            _ocorrencia.Moeda.val(cargaPreenchida.Moeda);
            _ocorrencia.ValorCotacaoMoeda.val(cargaPreenchida.ValorCotacaoMoeda);
        }

        if (configuracaoTipoOcorrencia.OrigemOcorrencia == EnumOrigemOcorrencia.PorContrato) {
            executarReST("OcorrenciaContratoMotorista/BuscarDadosContrato", { TipoOcorrencia: data.Codigo }, function (r) {
                if (r.Success) {
                    _ocorrencia.ContratoFreteTransportador.val(r.Data.ContratoDescricao);
                    _ocorrencia.ContratoFreteTransportador.entityDescription(r.Data.ContratoDescricao);
                    _ocorrencia.ContratoFreteTransportador.Contrato = r.Data;
                    _ocorrencia.ContratoFreteTransportador.codEntity(r.Data.Contrato);

                    ConsultaInformacaoOcorrenciaPorContrato();
                }
            });
        }

        if (configuracaoTipoOcorrencia.OcorrenciaPorQuantidade) {
            _ocorrencia.Quantidade.visible(true)
            _ocorrencia.Quantidade.enable(true);
            _ocorrencia.ValorOcorrencia.enable(false);
        }
        else
            _ocorrencia.Quantidade.visible(false);

        _ocorrencia.ContratoFreteTransportador.visible(configuracaoTipoOcorrencia.OcorrenciaDestinadaFranquias);
        _ocorrencia.Tomador.visible(configuracaoTipoOcorrencia.PermiteSelecionarTomador);
        _ocorrencia.Tomador.enable(configuracaoTipoOcorrencia.PermiteSelecionarTomador);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            _ocorrencia.TipoTomador.enable(configuracaoTipoOcorrencia.PermiteSelecionarTomador);

        DefineModoCalculoOcorrencia();
    }

    if (data.CodigoComponenteFrete > 0)
        retornoComponenteFrete({ Codigo: data.CodigoComponenteFrete, Descricao: data.DescricaoComponenteFrete, TipoComponenteFrete: data.TipoComponenteFrete });
    else
        componenteFreteBlur();

    _selecionarProdutos = config.InformarProdutoLancamentoOcorrencia;
    if (config.InformarProdutoLancamentoOcorrencia && _gridCTe != null)
        _gridCTe.CarregarGrid();

    visibilidadeUtilizarSelecaoPorNotasFiscaisCTe();
    ExibeNotificarDebitosAtivos();

    var valorOcorrencia = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());    
    $.each(_CTesImportadosParaComplemento, function (i, obj) {
        _ocorrencia.ValorOcorrencia.val(Globalize.format(obj.ValorCTeComplemetarImportado + valorOcorrencia, "n2"));
    });
}

function FluxoOcorrenciaPorPeriodo(porPeriodo) {
    _ocorrencia.Carga.visible(!porPeriodo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _ocorrencia.Empresa.visible(porPeriodo);
        _ocorrencia.Empresa.required = porPeriodo;
    }

    if (porPeriodo) {
        _ocorrencia.Filial.visible(_ocorrencia.TipoOcorrencia.FiltrarOcorrenciasPeriodoPorFilial);
        _ocorrencia.Filial.required = _ocorrencia.TipoOcorrencia.FiltrarOcorrenciasPeriodoPorFilial;
    }

    // Oculta as grids desnecessarias
    if (_ocorrencia.TipoOcorrencia.origemOcorrencia != EnumOrigemOcorrencia.PorCarga && _ocorrencia.CTesParaComplemento.visibleFade())
        _ocorrencia.CTesParaComplemento.visibleFade(!porPeriodo);

    if (_ocorrencia.TipoOcorrencia.origemOcorrencia != EnumOrigemOcorrencia.PorPeriodo && _ocorrencia.CargasParaComplemento.visibleFade())
        _ocorrencia.CargasParaComplemento.visibleFade(porPeriodo);

    if (_ocorrencia.TipoOcorrencia.origemOcorrencia != EnumOrigemOcorrencia.PorPeriodo && _ocorrencia.VeiculosImprodutivos.visibleFade())
        _ocorrencia.VeiculosImprodutivos.visibleFade(porPeriodo);

    if (_ocorrencia.TipoOcorrencia.origemOcorrencia != EnumOrigemOcorrencia.PorContrato && _ocorrencia.VeiculosContrato.visibleFade())
        _ocorrencia.VeiculosContrato.visibleFade(porPeriodo);

    if (_ocorrencia.TipoOcorrencia.origemOcorrencia != EnumOrigemOcorrencia.PorContrato && _ocorrencia.MotoristasContrato.visibleFade())
        _ocorrencia.MotoristasContrato.visibleFade(porPeriodo);

    if (porPeriodo)
        periodoCargaBlur();
}

function ConsultaInformacaoOcorrenciaPorContrato() {
    var ocorrenciaComVeiculo = configuracaoTipoOcorrencia.OcorrenciaComVeiculo;

    // Quando ocorrencia tiver veículo, a grid de consulta é por documento e agrupada por determinados criterioss
    // Se não tiver, a Grid de busca é por veiculos cadastrados no contrato
    if (ocorrenciaComVeiculo) {
        _ocorrencia.Veiculo.visible(true);
        _ocorrencia.Veiculo.required = true;

        _ocorrencia.DocumentosAgrupadosDoVeiculo.visibleFade(true);

        _ocorrencia.VeiculosContrato.visible(false);
        _ocorrencia.MotoristasContrato.visible(false);

        _gridDocumentosAgrupados.CarregarGrid();
    } else {
        _ocorrencia.Veiculo.visible(false);
        _ocorrencia.Veiculo.required = false;

        _ocorrencia.DocumentosAgrupadosDoVeiculo.visibleFade(false);

        _ocorrencia.VeiculosContrato.visible(true);
        _ocorrencia.MotoristasContrato.visible(true);
    }
}