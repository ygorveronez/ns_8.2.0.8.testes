/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoManutencaoOrdemServicoFrota.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeOrdemServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _indicadoresManutencao;
var _pesquisaIndicadoresManutencao;
var _AreaIndicadoresManutencao;
var _HTMLDetalheIndicadoresManutencao;
var _knoutsOrdensServico = new Array();
var _paginasPrevistas = 1;
var _paginaAtual = 1;
var _tempoParaTroca = 300000;
var _timeOutPainel;
var _pesquisouNovamente = false;
var _itensPorPagina = 20;
var _habilitarPainel = false;

var PesquisaIndicadoresManutencao = function () {
    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumSituacaoOrdemServicoFrota.ObterOpcoes(), text: "Situação:", visible: ko.observable(true) });
    this.TipoOrdemServico = PropertyEntity({ val: ko.observable(""), options: EnumTipoOficina.ObterOpcoesPesquisa(), text: "Tipo Ordem Serviço:", visible: ko.observable(true) });
    this.TipoManutencao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumTipoManutencaoOrdemServicoFrota.ObterOpcoes(), text: "Tipo de Manutenção:", visible: ko.observable(true) });
    this.NumeroInicial = PropertyEntity({ getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Nº Final:", getType: typesKnockout.int, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Local de Manutenção:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MarcaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoServico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Segmento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CidadePessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cidade Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.UFPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "UF Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OperadorLancamentoDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador Lançamento Documento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OperadorFinalizaDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador Finalizou Documento:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataInicialInclusao = PropertyEntity({ text: "Data Ini. Inclusão:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalInclusao = PropertyEntity({ text: "Data Fin. Inclusão:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialInclusao.dateRangeLimit = this.DataFinalInclusao;
    this.DataFinalInclusao.dateRangeInit = this.DataInicialInclusao;
    this.Mecanicos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Mecânico:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicialLimiteExecucao = PropertyEntity({ text: "Data Inicial Limite Execução:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalLimiteExecucao = PropertyEntity({ text: "Data Final Limite Execução:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialLimiteExecucao.dataRangeLimit = this.DataFinalLimiteExecucao;
    this.DataFinalLimiteExecucao.dataRangeInit = this.DataInicialLimiteExecucao;
    this.Prioridade = PropertyEntity({ text: "Prioridade: ", val: ko.observable(""), options: EnumPrioridadeOrdemServico.obterOpcoesPesquisa() });

    this.HabilitarPainel = PropertyEntity({ getType: typesKnockout.bool, eventChange: HabilitarPainelOnChange, val: ko.observable(false), def: false, text: "Habilitar visualização em formato de painel?", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarClick, type: types.event, text: "Pesquisar", visible: ko.observable(false) });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaOrdemServico.Visible.visibleFade() == true) {
                _pesquisaOrdemServico.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaOrdemServico.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var AreaIndicadoresManutencao = function () {

    this.OrdemServico = PropertyEntity();
    this.CarregandoOrdemServico = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: OrdemServicoPesquisaScroll });

}

var IndicadoresManutencao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });

    this.InfoOrdemServico = PropertyEntity({ eventClick: null, type: types.event, cssClass: ko.observable("well well-pedido no-padding padding-5") });
    this.Numero = PropertyEntity({ text: "Número: ", val: ko.observable("") });
    this.DataProgramada = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", val: ko.observable("") });
    this.Motorista = PropertyEntity({ text: "Motorista: ", val: ko.observable("") });
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable("") });
    this.NumeroFrota = PropertyEntity({ text: "N° Frota: ", val: ko.observable("") });
    this.Prioridade = PropertyEntity({ text: "N° Frota: ", val: ko.observable("") });
    this.LocalManutencao = PropertyEntity({ text: "Local: ", val: ko.observable("") });
    this.DescricaoOS = PropertyEntity({ text: "Descricao OS: ", val: ko.observable("") });
    this.QuantidadeDiasAberto = PropertyEntity({ text: "Quantidade Dias Aberto: ", val: ko.observable("") });
    this.DescricaoPrioridade = PropertyEntity({ text: "Prioridade: ", val: ko.observable(""), cssClass: ko.observable("btn btn-danger btn-lg btn-icon rounded-circle"), visible: ko.observable(true) });
    this.TipoManutencao = PropertyEntity({ val: ko.observable(EnumTipoManutencaoOrdemServicoFrota.Corretiva), def: EnumTipoManutencaoOrdemServicoFrota.Corretiva });
    this.Detalhes = PropertyEntity({ eventClick: DetalhesClick, type: types.event, text: "Detalhes", visible: ko.observable(true), val: ko.observable("") });

    this.Cor = PropertyEntity({ text: "Cor: ", val: ko.observable("") });

}


function loadIndicadoresManutencao(callback) {

    LimparTimeOutPainel();

    _pesquisaIndicadoresManutencao = new PesquisaIndicadoresManutencao();
    KoBindings(_pesquisaIndicadoresManutencao, "knockoutPesquisaIndicadoresManutencao", false, _pesquisaIndicadoresManutencao.Pesquisar.id);

    _indicadoresManutencao = new IndicadoresManutencao();
    KoBindings(_indicadoresManutencao, "knockoutIndicadoresManutencao");

    HeaderAuditoria("IndicadoresManutencao", _indicadoresManutencao);

    _AreaIndicadoresManutencao = new AreaIndicadoresManutencao();
    KoBindings(_AreaIndicadoresManutencao, "knockoutIndicadoresManutencao");

    $.get("Content/Static/Frota/IndicadoresManutencao.html?dyn=" + guid(), function (data) {

        _HTMLDetalheIndicadoresManutencao = data;
        if (callback != null)
            callback();
    });


    $(window).one('hashchange', function () {
        LimparTimeOutPainel();
    });

}


function DetalhesClick(OSSelecionada) {

    $.get("Content/Static/Frota/OrdemServico.html?dyn=" + guid(), function (htmlOrdemServico) {
        Global.abrirModal("divModalOrdemServico");
        $("#ModaisOrdemServico").html(htmlOrdemServico);

        buscarOrdemServicoPorCodigo(OSSelecionada.Codigo.val(), null);

        CODIGO_ORDEM_SERVICO_PARA_TELA_ORDEM_SERVICO = OSSelecionada.Codigo.val()

        LoadOrdemServico();
    });
}

function buscarOrdemServicoPorCodigo(codigo, callback) {
    executarReST("OrdemServico/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            _indicadoresManutencao.InfoOrdemServico = arg.Data;
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}

function BuscarIndicadoresManutencao(callback) {
    limparIndicadoresManutencao();
    let data = RetornarObjetoPesquisa(_pesquisaIndicadoresManutencao);
    data.Inicio = _AreaIndicadoresManutencao.Inicio.val();
    data.Limite = 20;
    _AreaIndicadoresManutencao.CarregandoOrdemServico.val(true);

    executarReST("IndicadoresManutencao/Pesquisa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                let retorno = arg.Data;
                _AreaIndicadoresManutencao.Total.val(retorno.Quantidade);
                _AreaIndicadoresManutencao.Inicio.val(_AreaIndicadoresManutencao.Inicio.val() + data.Limite);


                for (let i = 0; i < retorno.Registros.length; i++) {
                    let ordemServico = retorno.Registros[i];
                    let knoutIndicadoresManutencao = new IndicadoresManutencao();

                    let html = _HTMLDetalheIndicadoresManutencao.replace(/#knoutIndicadoresManutencao/g, knoutIndicadoresManutencao.InfoOrdemServico.id)
                        .replace(/#ribbonPreventiva/g, knoutIndicadoresManutencao.InfoOrdemServico.id + "_ribbonPreventiva")
                        .replace(/#ribbonCorretiva/g, knoutIndicadoresManutencao.InfoOrdemServico.id + "_ribbonCorretiva")
                        .replace(/#ribbonAmbas/g, knoutIndicadoresManutencao.InfoOrdemServico.id + "_ribbonAmbas")
                        .replace(/#knoutDivInformacoes/g, knoutIndicadoresManutencao.InfoOrdemServico.id + "_divInformacoes")
                        .replace(/#knoutDivPrioridade/g, knoutIndicadoresManutencao.InfoOrdemServico.id + "_divPrioridade")
                        .replace(/#knoutClassPrioridade/g, PreencherPrioridade(ordemServico, knoutIndicadoresManutencao.InfoOrdemServico.id));

                    ajustarColunaCard(ordemServico, html);

                    InformarTipoManutencaoOrdemServicoFrota(ordemServico, knoutIndicadoresManutencao.InfoOrdemServico.id);

                    KoBindings(knoutIndicadoresManutencao, knoutIndicadoresManutencao.InfoOrdemServico.id);

                    let dataKnout = { Data: ordemServico };
                    PreencherObjetoKnout(knoutIndicadoresManutencao, dataKnout);
                    _knoutsOrdensServico.push(knoutIndicadoresManutencao);
                    $('#container').removeClass('d-none');

                }

            }

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function pesquisarClick() {
    limparIndicadoresManutencao()
    BuscarIndicadoresManutencao();

}
function OrdemServicoPesquisaScroll(e, sender) {
    let elem = sender.target;
    if (_AreaIndicadoresManutencao.Inicio.val() < _AreaIndicadoresManutencao.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight - 5)) {
        BuscarIndicadoresManutencao();
    }
}

function InformarTipoManutencaoOrdemServicoFrota(IndicadoresManutencao, idDivIndicadoresManutencao) {

    if (IndicadoresManutencao.TipoManutencao.Codigo == EnumTipoManutencaoOrdemServicoFrota.Corretiva)
        $("#" + idDivIndicadoresManutencao + "_ribbonCorretiva").show();
    else if (IndicadoresManutencao.TipoManutencao.Codigo == EnumTipoManutencaoOrdemServicoFrota.Preventiva)
        $("#" + idDivIndicadoresManutencao + "_ribbonPreventiva").show();
    else
        $("#" + idDivIndicadoresManutencao + "_ribbonAmbas").show();
}


function PreencherPrioridade(IndicadoresManutencao, idDivIndicadoresManutencao) {


    if (IndicadoresManutencao.Prioridade == EnumPrioridadeOrdemServico.Urgente)
        return "btn btn-danger btn-lg btn-icon rounded-circle";
    else if (IndicadoresManutencao.Prioridade == EnumPrioridadeOrdemServico.Alto)
        return "btn btn-warning btn-lg btn-icon rounded-circle";
    else if (IndicadoresManutencao.Prioridade == EnumPrioridadeOrdemServico.Medio)
        return "btn btn-info btn-lg btn-icon rounded-circle";
    else if (IndicadoresManutencao.Prioridade == EnumPrioridadeOrdemServico.Baixo)
        return "btn btn-success btn-lg btn-icon rounded-circle";
    else
        return "btn btn-default btn-lg btn-icon rounded-circle";


}
function limparIndicadoresManutencao() {
    LimparCampos(_indicadoresManutencao);

    _AreaIndicadoresManutencao.Inicio.val(0);

    $("#knockoutIndicadoresManutencaoInternaEmAberto").html("");
    $('#knockoutIndicadoresManutencaoInternaFinalizada').removeClass('ocultarPaginacao');
    $('#knockoutIndicadoresManutencaoExternaEmAberto').removeClass('ocultarPaginacao');
    $("#knockoutIndicadoresManutencaoExternaFinalizada").html("");
    $("#knockoutIndicadoresManutencao").html("");
}
function validarPaginacao() {
    if (_paginaAtual >= _paginasPrevistas) {
        _paginaAtual = 1;
        _paginou = false;
    } else {
        _paginaAtual++;
        _paginou = true;
    }
}

function LimparTimeOutPainel() {
    if (_timeOutPainel !== null)
        clearTimeout(_timeOutPainel);
}

function executarPesquisaTimeOut(retornoData) {
    if (_habilitarPainel) {

        if (retornoData !== null && retornoData !== undefined)
            _paginasPrevistas = retornoData.recordsTotal / (_itensPorPagina);



        validarPaginacao();
        _timeOutPainel = setTimeout(function () {
            if (_habilitarPainel) {
                _pesquisouNovamente = false;
                carregarOrdensServico(executarPesquisaTimeOut);
            }
        }, _tempoParaTroca);

    }
}




function HabilitarPainelOnChange(e, sender) {
    _pesquisouNovamente = true;
    _AreaIndicadoresManutencao.Inicio.val(0);
    _AreaIndicadoresManutencao.Total.val(_itensPorPagina);
    if ($('#' + e.HabilitarPainel.id).is(':checked')) {
        _pesquisaIndicadoresManutencao.HabilitarPainel.val(true);
        _habilitarPainel = true;
        _paginaAtual = 1;
        executarPesquisaTimeOut();
        openFullscreen();
    } else {

        $('#knockoutIndicadoresManutencaoInternaEmAberto').removeClass('ocultarPaginacao');
        $('#knockoutIndicadoresManutencaoInternaFinalizada').removeClass('ocultarPaginacao');
        $('#knockoutIndicadoresManutencaoExternaEmAberto').removeClass('ocultarPaginacao');
        $('#knockoutIndicadoresManutencaoExternaFinalizada').removeClass('ocultarPaginacao');
        $('#knockoutIndicadoresManutencao').removeClass('ocultarPaginacao');
        _habilitarPainel = false;
        _pesquisaIndicadoresManutencao.HabilitarPainel.val(false);
    }
}


function ajustarColunaCard(data, html) {

    if (data.TipoOficina == EnumTipoOficina.Interna) {
        if (data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.EmManutencao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.EmDigitacao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.AguardandoAprovacao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.AgNotaFiscal ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.AgAutorizacao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.Rejeitada) {
            $("#knockoutIndicadoresManutencaoInternaEmAberto").append(html);
            return;
        }

        if (data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.Finalizada ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.Cancelada) {
            $("#knockoutIndicadoresManutencaoInternaFinalizada").append(html);
            return;
        }
    }

    if (data.TipoOficina == EnumTipoOficina.Externa) {
        if (data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.EmManutencao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.EmDigitacao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.AguardandoAprovacao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.AgNotaFiscal ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.AgAutorizacao ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.Rejeitada) {
            $("#knockoutIndicadoresManutencaoExternaEmAberto").append(html);
            return;
        }

        if (data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.Finalizada ||
            data.SituacaoOrdemServicoFrota == EnumSituacaoOrdemServicoFrota.Cancelada) {
            $("#knockoutIndicadoresManutencaoExternaFinalizada").append(html);
            return;
        }
    }

    $("#knockoutIndicadoresManutencao").append(html);


}

function carregarOrdensServico(callback) {

    BuscarIndicadoresManutencao();
    executarPesquisaTimeOut();
}

const fullscreenDiv = document.getElementById('panelKnockoutIndicadoresManutencao');
function openFullscreen() {
    // Redimensionar a div quando a janela é redimensionada
    window.addEventListener('resize', resizeOverlay);
    // Redimensionar a div inicialmente
    resizeOverlay();
    exibirFullscreen();
}

function resizeOverlay() {
    if (isFullscreen()) {
        fullscreenDiv.style.width = window.innerWidth + 'px';
        fullscreenDiv.style.height = window.innerHeight + 'px';
    } else {
        fullscreenDiv.style.width = ''; // Limpar a largura para que a div volte ao tamanho original
        fullscreenDiv.style.height = ''; // Limpar a altura para que a div volte ao tamanho original
    }
}

function exibirFullscreen() {
    if (fullscreenDiv.requestFullscreen) {
        fullscreenDiv.requestFullscreen();
    } else if (fullscreenDiv.mozRequestFullScreen) { /* Firefox */
        fullscreenDiv.mozRequestFullScreen();
    } else if (fullscreenDiv.webkitRequestFullscreen) { /* Chrome, Safari and Opera */
        fullscreenDiv.webkitRequestFullscreen();
    } else if (fullscreenDiv.msRequestFullscreen) { /* IE/Edge */
        fullscreenDiv.msRequestFullscreen();
    }
}

function isFullscreen() {
    return document.fullscreenElement || document.webkitFullscreenElement || document.mozFullScreenElement || document.msFullscreenElement;
}

document.addEventListener('fullscreenchange', function () {

    if (isFullscreen()) {
        fullscreenDiv.classList.add('fullscreen-overlay');
        fullscreenDiv.style.pointerEvents = "none";
    }
    else {
        // Remover a classe quando sair do modo de tela cheia
        fullscreenDiv.classList.remove('fullscreen-overlay');
        fullscreenDiv.style.pointerEvents = "auto";
        _pesquisaIndicadoresManutencao.HabilitarPainel.val(false);
        LimparTimeOutPainel();
    }
});