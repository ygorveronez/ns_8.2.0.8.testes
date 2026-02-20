/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _documentacaoEntradaFiscal;
var _gridDocumentacaoEntradaFiscal;
var _gridVisualizarNFesOrigem;

function DocumentacaoEntradaFiscal() {
    this.Codigo = PropertyEntity({ val: _informacoesDevolucao.CodigoDevolucao.val });
    this.GridNFesOrigem = PropertyEntity({ id: guid() });
    this.Grid = PropertyEntity({ id: guid(), visible: ko.observable(false) });
    this.ObservacaoDocumentacaoEntradaFiscal = PropertyEntity({ text: "Observações", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), maxlength: 500 });
    this.AdicionarNFesTransferenciaPallet = PropertyEntity({ text: 'Adicionar NFes de Transferência de Pallets', val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });
    this.AdicionarNotaFiscalDevolucao = PropertyEntity({ text: 'Adicionar NFD', val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });
    this.AdicionarNotaFiscalPermuta = PropertyEntity({ text: 'Adicionar NF Permuta', val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });
    this.NumeroNotaFiscalDevolucao = PropertyEntity({ text: 'Número NFD', val: ko.observable(0), getType: typesKnockout.int, maxlength: 11 });
    this.SerieNotaFiscalDevolucao = PropertyEntity({ text: 'Série NFD', val: ko.observable(""), getType: typesKnockout.string, maxlength: 3 });
    this.NumeroNotaFiscalPermuta = PropertyEntity({ text: 'Número NF Permuta', val: ko.observable(0), getType: typesKnockout.int, maxlength: 11 });
    this.SerieNotaFiscalPermuta = PropertyEntity({ text: 'Série NF Permuta', val: ko.observable(""), getType: typesKnockout.string, maxlength: 3 });
    this.TotalQuantidadePallets = PropertyEntity({ text: "Total Pallets:", getType: typesKnockout.int, val: ko.observable(0), enable: ko.observable(true), visible: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });
    this.ChaveNFe = PropertyEntity({
        getType: typesKnockout.string,
        val: ko.observable("").extend({
            validation: {
                validator: function (value) {
                    return /^\d*$/.test(value);
                },
            }
        }),
        text: "Chave:",
        required: ko.observable(true),
        visible: ko.observable(true),
        maxlength: 44
    });

    this.ChaveNFe.val.subscribe(function (newValue) {
        const cleaned = newValue.replace(/\D/g, '');
        if (cleaned !== newValue) {
            this.ChaveNFe.val(cleaned);
        }
    }.bind(this));

    this.Confirmar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            confirmarDocumentacaoEntradaFiscalClick();
        }, text: "Confirmar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.SalvarNotaFiscalPermuta = PropertyEntity({
        type: types.event, eventClick: function (e) {
            salvarNotaFiscalPermuta();
        }, text: "Salvar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.SalvarNotaFiscalDevolucao = PropertyEntity({
        type: types.event, eventClick: function (e) {
            salvarNotaFiscalDevolucao();
        }, text: "Salvar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.Adicionar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            adicionarNotaDevolucaoClick();
        }, text: "Adicionar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
}
function loadDocumentacaoEntradaFiscal(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {

            $.get("Content/Static/Carga/GestaoDevolucao/DocumentacaoEntradaFiscal.html?dyn=" + guid(), function (data) {
                $("#container-principal-content").html(data);
                _documentacaoEntradaFiscal = new DocumentacaoEntradaFiscal();

                KoBindings(_documentacaoEntradaFiscal, "knockoutDocumentacaoEntradaFiscal");

                PreencherObjetoKnout(_documentacaoEntradaFiscal, r)
                controlarAcoesContainerPrincipal(etapa, _documentacaoEntradaFiscal);

                controlarAcoesContainerPrincipal(etapa, _documentacaoEntradaFiscal);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();

                _documentacaoEntradaFiscal.AdicionarNFesTransferenciaPallet.val(r.Data.AdicionarNFesTransferenciaPallet);
                if (_documentacaoEntradaFiscal.AdicionarNFesTransferenciaPallet.val()) {
                    loadGridNFes();
                    $("#" + _documentacaoEntradaFiscal.Grid.val() + "tfoot").show();
                } else {
                    $("#" + _documentacaoEntradaFiscal.Grid.val() + "tfoot").hide();
                }

            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function loadGridNFes() {
    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: null,
        callbackSelecionado: null,
        callbackSelecionarTodos: null,
        somenteLeitura: false,
        permitirSelecionarSomenteUmRegistro: false
    };

    var visualizarNota = { descricao: "Visualizar Nota", id: guid(), metodo: visualizarNotasFiscaisOrigem, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [visualizarNota] };

    _gridDocumentacaoEntradaFiscal = new GridView(_documentacaoEntradaFiscal.Grid.id, "GestaoDevolucao/PesquisaNFesPallet", _documentacaoEntradaFiscal, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, null, null, null, null, null);
    _gridDocumentacaoEntradaFiscal.SetPermitirEdicaoColunas(false);
    _gridDocumentacaoEntradaFiscal.SetSalvarPreferenciasGrid(false);
    _gridDocumentacaoEntradaFiscal.CarregarGrid();
}
function adicionarChaveClick() {
    Global.abrirModal("divModalAdicionarChaveNota");
}

function adicionarNotaDevolucaoClick() {
    let dados = {
        ChaveNFe: _documentacaoEntradaFiscal.ChaveNFe.val(),
        Codigo: _documentacaoEntradaFiscal.Codigo.val(),
        ObservacaoDocumentacaoEntradaFiscal: _documentacaoEntradaFiscal.ObservacaoDocumentacaoEntradaFiscal.val(),
        AdicionarNFesTransferenciaPallet: _documentacaoEntradaFiscal.AdicionarNFesTransferenciaPallet.val(),
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja realmente adicionar a nota fiscal?", function () {
        executarReST('GestaoDevolucao/VincularNotasPallets', dados, function (arg) {
            if (arg.Success) {
                LimparCampo(_documentacaoEntradaFiscal.ChaveNFe);
                Global.fecharModal("divModalAdicionarChaveNota");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
                _documentacaoEntradaFiscal.TotalQuantidadePallets.val(arg.Data.TotalQuantidadePallets);
                loadGridNFes();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function confirmarDocumentacaoEntradaFiscalClick() {
    let dados = RetornarObjetoPesquisa(_documentacaoEntradaFiscal);

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja realmente avançar a etapa?", function () {
        executarReST('GestaoDevolucao/ConfirmarDocumentacaoEntradaFiscal', dados, function (arg) {
            if (arg.Success) {
                _gridGestaoDevolucaoDevolucoes.CarregarGrid();
                LimparCampo(_documentacaoEntradaFiscal.ChaveNFe);
                Global.fecharModal("divModalAdicionarChaveNota");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function salvarNotaFiscalPermuta() {
    const dados = RetornarObjetoPesquisa(_documentacaoEntradaFiscal);

    executarReST("GestaoDevolucao/AtualizarDadosNotaFiscalPermuta", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados de Nota Fiscal de Permuta salvos.")
            } else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function salvarNotaFiscalDevolucao() {
    const dados = RetornarObjetoPesquisa(_documentacaoEntradaFiscal);

    executarReST("GestaoDevolucao/AtualizarDadosNotaFiscalDevolucao", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados de Nota Fiscal de Devolução salvos.")
            } else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function visualizarNotasFiscaisOrigem() {
    loadGridVisualizacaoNFesOrigem();
    Global.abrirModal("divModalVisualizarNotasFiscais");
}

function loadGridVisualizacaoNFesOrigem() {
    _gridVisualizarNFesOrigem = new GridView(_documentacaoEntradaFiscal.GridNFesOrigem.id, "GestaoDevolucao/PesquisaNotasFiscaisOrigem", _documentacaoEntradaFiscal, null, null, 25, null, null, null, null, null, null, null, null, null, null, null);
    _gridVisualizarNFesOrigem.CarregarGrid();
}
