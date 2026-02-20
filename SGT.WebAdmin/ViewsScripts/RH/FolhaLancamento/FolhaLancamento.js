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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/FolhaInformacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFolhaLancamento;
var _folhaLancamento;
var _pesquisaFolhaLancamento;
var _importarFolhaLancamento;

var PesquisaFolhaLancamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.NumeroEvento = PropertyEntity({ text: "Número Evento:", getType: typesKnockout.int });
    this.NumeroContrato = PropertyEntity({ text: "Número Contrato:", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid() });
    this.FolhaInformacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Informação da Folha:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFolhaLancamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var FolhaLancamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 1000 });
    this.NumeroEvento = PropertyEntity({ text: "*Número Evento: ", required: ko.observable(true), getType: typesKnockout.int });
    this.NumeroContrato = PropertyEntity({ text: "Número Contrato: ", required: ko.observable(false), getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", getType: typesKnockout.date, required: ko.observable(true) });

    this.Base = PropertyEntity({ text: "*Valor Base:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true) });
    this.Referencia = PropertyEntity({ text: "Valor Referência:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true) });
    this.DataCompetencia = PropertyEntity({ text: ko.observable("Data Competência:"), getType: typesKnockout.date, required: ko.observable(false) });

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário:", idBtnSearch: guid(), required: ko.observable(true) });
    this.FolhaInformacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Informação da Folha:", idBtnSearch: guid(), required: ko.observable(true) });
};

var CRUDFolhaLancamento = function () {
    this.MultiploTitulo = PropertyEntity({ eventClick: multiploTituloClick, type: types.event, text: "Múltiplos Lançamentos", visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
};

var ImportarFolhaLancamento = function () {
    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataCompetencia = PropertyEntity({ text: ko.observable("*Data Competência:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivo para leitura:", val: ko.observable(""), visible: ko.observable(true), required: ko.observable(true) });

    this.Importar = PropertyEntity({ eventClick: importarDocumentoClick, type: types.event, text: "Importar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadFolhaLancamento() {
    _folhaLancamento = new FolhaLancamento();
    KoBindings(_folhaLancamento, "knockoutCadastroFolhaLancamento");

    HeaderAuditoria("FolhaLancamento", _folhaLancamento);

    _crudFolhaLancamento = new CRUDFolhaLancamento();
    KoBindings(_crudFolhaLancamento, "knockoutCRUDFolhaLancamento");

    _pesquisaFolhaLancamento = new PesquisaFolhaLancamento();
    KoBindings(_pesquisaFolhaLancamento, "knockoutPesquisaFolhaLancamento", false, _pesquisaFolhaLancamento.Pesquisar.id);

    _importarFolhaLancamento = new ImportarFolhaLancamento();
    KoBindings(_importarFolhaLancamento, "knockoutImportarFolhaLancamento");

    new BuscarFuncionario(_pesquisaFolhaLancamento.Funcionario);
    new BuscarFolhaInformacao(_pesquisaFolhaLancamento.FolhaInformacao);
    new BuscarFuncionario(_folhaLancamento.Funcionario);
    new BuscarFolhaInformacao(_folhaLancamento.FolhaInformacao, retornoFolhaInformacao);

    if (_CONFIGURACAO_TMS.GerarTituloFolhaPagamento) {
        _folhaLancamento.DataCompetencia.required(true);
        _folhaLancamento.DataCompetencia.text("*Data Competência:");
    }

    buscarFolhaLancamento();
}

function retornoFolhaInformacao(data) {
    _folhaLancamento.FolhaInformacao.val(data.Descricao);
    _folhaLancamento.FolhaInformacao.codEntity(data.Codigo);
    _folhaLancamento.NumeroEvento.val(data.CodigoIntegracao);
}

function multiploTituloClick(e, sender) {
    new LancarFolhas();
}

function adicionarClick(e, sender) {
    Salvar(_folhaLancamento, "FolhaLancamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridFolhaLancamento.CarregarGrid();
                limparCamposFolhaLancamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_folhaLancamento, "FolhaLancamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridFolhaLancamento.CarregarGrid();
                limparCamposFolhaLancamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o lançamento da folha?", function () {
        ExcluirPorCodigo(_folhaLancamento, "FolhaLancamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridFolhaLancamento.CarregarGrid();
                limparCamposFolhaLancamento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposFolhaLancamento();
}

function importarClick() {
    limparCamposImportarFolha();
    Global.abrirModal('divModalImportarFolhaLancamento');
}

function importarDocumentoClick() {
    var valido = ValidarCamposObrigatorios(_importarFolhaLancamento);
    var file = document.getElementById(_importarFolhaLancamento.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var formData = new FormData();
        formData.append("upload", file.files[0]);

        enviarArquivo("FolhaLancamento/ImportarFolha?callback=?",
            {
                DataInicial: _importarFolhaLancamento.DataInicial.val(),
                DataFinal: _importarFolhaLancamento.DataFinal.val(),
                DataCompetencia: _importarFolhaLancamento.DataCompetencia.val()
            }, formData, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Registros importados com sucesso.");
                        _gridFolhaLancamento.CarregarGrid();
                        Global.fecharModal('divModalImportarFolhaLancamento');
                        limparCamposImportarFolha();

                        exibirMensagem(tipoMensagem.aviso, "Dados retorno importação", arg.Msg, 200000);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 200000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

//*******MÉTODOS*******

function buscarFolhaLancamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFolhaLancamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFolhaLancamento = new GridView(_pesquisaFolhaLancamento.Pesquisar.idGrid, "FolhaLancamento/Pesquisa", _pesquisaFolhaLancamento, menuOpcoes, null);
    _gridFolhaLancamento.CarregarGrid();
}

function editarFolhaLancamento(folhaLancamentoGrid) {
    limparCamposFolhaLancamento();
    _folhaLancamento.Codigo.val(folhaLancamentoGrid.Codigo);
    BuscarPorCodigo(_folhaLancamento, "FolhaLancamento/BuscarPorCodigo", function (arg) {
        _pesquisaFolhaLancamento.ExibirFiltros.visibleFade(false);
        _crudFolhaLancamento.Atualizar.visible(true);
        _crudFolhaLancamento.Cancelar.visible(true);
        _crudFolhaLancamento.Excluir.visible(true);
        _crudFolhaLancamento.Adicionar.visible(false);
    }, null);
}

function limparCamposFolhaLancamento() {
    _crudFolhaLancamento.Atualizar.visible(false);
    _crudFolhaLancamento.Cancelar.visible(false);
    _crudFolhaLancamento.Excluir.visible(false);
    _crudFolhaLancamento.Adicionar.visible(true);
    LimparCampos(_folhaLancamento);
}

function limparCamposImportarFolha() {
    _importarFolhaLancamento.Arquivo.val("");
    LimparCampos(_importarFolhaLancamento);
}