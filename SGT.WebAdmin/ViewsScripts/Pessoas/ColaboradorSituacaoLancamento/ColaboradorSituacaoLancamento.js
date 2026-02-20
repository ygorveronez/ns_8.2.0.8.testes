/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/ColaboradorSituacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLancamentoColaborador.js" />
/// <reference path="ColaboradorSituacaoLancamentoAnexo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridColaboradorSituacaoLancamento;
var _colaboradorSituacaoLancamento;
var _pesquisaColaboradorSituacaoLancamento;
var _crudColaboradorSituacaoLancamento;
var operadorSituacaoLancamentoLogado = null;

var PesquisaColaboradorSituacaoLancamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoLancamentoColaborador.Todos), options: EnumSituacaoLancamentoColaborador.obterOpcoesPesquisa(), def: EnumSituacaoLancamentoColaborador.Todos });
    this.Colaborador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid() });
    this.ColaboradorSituacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Situação Colaborador:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridColaboradorSituacaoLancamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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
};

var ColaboradorSituacaoLancamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: ko.observable(true), maxlength: 500, enable: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador:", enable: ko.observable(false) });
    this.DataLancamento = PropertyEntity({ text: "Data Lançamento: ", enable: ko.observable(false), val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "*Situação:", val: ko.observable(EnumSituacaoLancamentoColaborador.Agendado), options: EnumSituacaoLancamentoColaborador.obterOpcoes(), def: EnumSituacaoLancamentoColaborador.Agendado, enable: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable(""), enable: ko.observable(true) });

    this.Colaborador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Colaborador:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.ColaboradorSituacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Situação Colaborador:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDColaboradorSituacaoLancamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadColaboradorSituacaoLancamento() {
    _colaboradorSituacaoLancamento = new ColaboradorSituacaoLancamento();
    KoBindings(_colaboradorSituacaoLancamento, "knockoutCadastroColaboradorSituacaoLancamento");

    HeaderAuditoria("ColaboradorLancamento", _colaboradorSituacaoLancamento);

    _crudColaboradorSituacaoLancamento = new CRUDColaboradorSituacaoLancamento();
    KoBindings(_crudColaboradorSituacaoLancamento, "knockoutCRUDColaboradorSituacaoLancamento");

    _pesquisaColaboradorSituacaoLancamento = new PesquisaColaboradorSituacaoLancamento();
    KoBindings(_pesquisaColaboradorSituacaoLancamento, "knockoutPesquisaColaboradorSituacaoLancamento", _pesquisaColaboradorSituacaoLancamento.Pesquisar.id);

    new BuscarFuncionario(_pesquisaColaboradorSituacaoLancamento.Colaborador, null, null, null, true, null, null, null, true);
    new BuscarSituacoesColaborador(_pesquisaColaboradorSituacaoLancamento.ColaboradorSituacao);

    new BuscarFuncionario(_colaboradorSituacaoLancamento.Colaborador, null, null, null, true);
    new BuscarSituacoesColaborador(_colaboradorSituacaoLancamento.ColaboradorSituacao);

    buscarColaboradorSituacaoLancamento();

    loadColaboradorSituacaoLancamentoAnexo();
    loadColaboradorSituacaoLancamentoIntegracao();
    PreencheOperadorLogado();
}

function adicionarClick(e, sender) {
    resetarTabs();

    Salvar(_colaboradorSituacaoLancamento, "ColaboradorSituacaoLancamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_colaboradorSituacaoLancamento.ListaAnexosNovos.list.length > 0)
                    EnviarColaboradorSituacaoLancamentoAnexos(arg.Data.Codigo);
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridColaboradorSituacaoLancamento.CarregarGrid();
                    limparCamposColaboradorSituacaoLancamento();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    resetarTabs();

    if (_colaboradorSituacaoLancamento.ListaAnexosExcluidos.list.length > 0)
        _colaboradorSituacaoLancamento.ListaAnexosExcluidos.text = JSON.stringify(_colaboradorSituacaoLancamento.ListaAnexosExcluidos.list);

    Salvar(_colaboradorSituacaoLancamento, "ColaboradorSituacaoLancamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_colaboradorSituacaoLancamento.ListaAnexosNovos.list.length > 0)
                    EnviarColaboradorSituacaoLancamentoAnexos(_colaboradorSituacaoLancamento.Codigo.val());
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridColaboradorSituacaoLancamento.CarregarGrid();
                    limparCamposColaboradorSituacaoLancamento();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar o Lançamento " + _colaboradorSituacaoLancamento.Descricao.val() + "?", function () {
        executarReST("ColaboradorSituacaoLancamento/Cancelar", { Codigo: _colaboradorSituacaoLancamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso.");
                    _gridColaboradorSituacaoLancamento.CarregarGrid();
                    limparCamposColaboradorSituacaoLancamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function limparClick(e) {
    limparCamposColaboradorSituacaoLancamento();
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a situação do Colaborador?", function () {
        ExcluirPorCodigo(_colaboradorSituacaoLancamento, "ColaboradorSituacaoLancamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridColaboradorSituacaoLancamento.CarregarGrid();
                limparCamposColaboradorSituacaoLancamento();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        }, null);
    });
}

//*******MÉTODOS*******

function EnviarColaboradorSituacaoLancamentoAnexos(codigoColaboradorSituacaoLancamento) {
    var file;
    var fileCount = _colaboradorSituacaoLancamento.ListaAnexosNovos.list.length;
    var documentos = new Array();

    for (var i = 0; i < _colaboradorSituacaoLancamento.ListaAnexosNovos.list.length; i++) {
        file = _colaboradorSituacaoLancamento.ListaAnexosNovos.list[i].Arquivo;
        var formData = new FormData();
        formData.append("upload", file);
        var data = {
            ListaAnexos: "",
            DescricaoAnexo: _colaboradorSituacaoLancamento.ListaAnexosNovos.list[i].DescricaoAnexo.val,
            CodigoColaboradorSituacaoLancamento: codigoColaboradorSituacaoLancamento
        };
        enviarArquivo("ColaboradorSituacaoLancamento/EnviarAnexos?callback=?", data, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    documentos.push({ Codigo: codigoColaboradorSituacaoLancamento });
                    if (documentos.length == fileCount) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                        _gridColaboradorSituacaoLancamento.CarregarGrid();
                        limparCamposColaboradorSituacaoLancamento();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function PreencheOperadorLogado() {
    var _fillName = function () {
        _colaboradorSituacaoLancamento.Operador.val(operadorSituacaoLancamentoLogado.Nome);
    }

    if (operadorSituacaoLancamentoLogado != null) _fillName();

    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                operadorSituacaoLancamentoLogado = {
                    Nome: arg.Data.Nome
                };
                _fillName();
            }
        }
    });
}

function buscarColaboradorSituacaoLancamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarColaboradorSituacaoLancamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridColaboradorSituacaoLancamento = new GridView(_pesquisaColaboradorSituacaoLancamento.Pesquisar.idGrid, "ColaboradorSituacaoLancamento/Pesquisa", _pesquisaColaboradorSituacaoLancamento, menuOpcoes, null);
    _gridColaboradorSituacaoLancamento.CarregarGrid();
}

function editarColaboradorSituacaoLancamento(produtoGrid) {
    limparCamposColaboradorSituacaoLancamento();
    _colaboradorSituacaoLancamento.Codigo.val(produtoGrid.Codigo);
    BuscarPorCodigo(_colaboradorSituacaoLancamento, "ColaboradorSituacaoLancamento/BuscarPorCodigo", function (arg) {
        _pesquisaColaboradorSituacaoLancamento.ExibirFiltros.visibleFade(false);
        _crudColaboradorSituacaoLancamento.Atualizar.visible(true);
        _crudColaboradorSituacaoLancamento.Cancelar.visible(true);
        _crudColaboradorSituacaoLancamento.Limpar.visible(true);
        _crudColaboradorSituacaoLancamento.Excluir.visible(true);
        _crudColaboradorSituacaoLancamento.Adicionar.visible(false);

        RecarregarGridColaboradorSituacaoLancamentoAnexo();
        recarregarColaboradorSituacaoLancamentoIntegracao();

        if (_colaboradorSituacaoLancamento.Situacao.val() == EnumSituacaoLancamentoColaborador.Cancelado) {
            _crudColaboradorSituacaoLancamento.Cancelar.visible(false);
            _crudColaboradorSituacaoLancamento.Atualizar.visible(false);
            SetarEnableCamposKnockout(_colaboradorSituacaoLancamento, false);
            SetarEnableCamposKnockout(_colaboradorSituacaoLancamentoAnexo, false);
        }
        else if (_colaboradorSituacaoLancamento.Situacao.val() == EnumSituacaoLancamentoColaborador.Finalizado) {
            _crudColaboradorSituacaoLancamento.Cancelar.visible(true);
            _crudColaboradorSituacaoLancamento.Atualizar.visible(true);
            SetarEnableCamposKnockout(_colaboradorSituacaoLancamento, false);
            _colaboradorSituacaoLancamento.DataFinal.enable(true);
            SetarEnableCamposKnockout(_colaboradorSituacaoLancamentoAnexo, false);
        }

    }, null);
}

function limparCamposColaboradorSituacaoLancamento() {
    _crudColaboradorSituacaoLancamento.Atualizar.visible(false);
    _crudColaboradorSituacaoLancamento.Cancelar.visible(false);
    _crudColaboradorSituacaoLancamento.Limpar.visible(false);
    _crudColaboradorSituacaoLancamento.Adicionar.visible(true);
    _crudColaboradorSituacaoLancamento.Excluir.visible(false);
    LimparCampos(_colaboradorSituacaoLancamento);
    _colaboradorSituacaoLancamento.DataLancamento.val(Global.DataAtual());
    PreencheOperadorLogado();

    LimparCamposColaboradorSituacaoLancamentoAnexo();
    limparCamposColaboradorSituacaoLancamentoIntegracao();

    RecarregarGridColaboradorSituacaoLancamentoAnexo();

    SetarEnableCamposKnockout(_colaboradorSituacaoLancamento, true);
    SetarEnableCamposKnockout(_colaboradorSituacaoLancamentoAnexo, true);
    _colaboradorSituacaoLancamento.Numero.enable(false);
    _colaboradorSituacaoLancamento.DataLancamento.enable(false);
    _colaboradorSituacaoLancamento.Operador.enable(false);
    _colaboradorSituacaoLancamento.Situacao.enable(false);

    resetarTabs();
}

function resetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}