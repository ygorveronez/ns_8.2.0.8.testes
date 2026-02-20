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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridImpostoIBPTNFe;
var _impostoIBPTNFe;
var _pesquisaImpostoIBPTNFe;

var PesquisaImpostoIBPTNFe = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.NCM = PropertyEntity({ text: "NCM: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridImpostoIBPTNFe.CarregarGrid();
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

var ImpostoIBPTNFe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NCM = PropertyEntity({ text: "*NCM: ", required: true, maxlength: 300 });
    this.Extensao = PropertyEntity({ text: "Extensão: ", required: false, maxlength: 300 });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", required: true, maxlength: 300 });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 300 });

    this.NacionalFederal = PropertyEntity({ text: "*Nacional Federal: ", required: true, getType: typesKnockout.decimal, maxlength: 18 });
    this.ImportadosFederal = PropertyEntity({ text: "*Importados Federal: ", required: true, getType: typesKnockout.decimal, maxlength: 18 });
    this.Estadual = PropertyEntity({ text: "*Estadual: ", required: true, getType: typesKnockout.decimal, maxlength: 18 });
    this.Municipal = PropertyEntity({ text: "*Municipal: ", required: true, getType: typesKnockout.decimal, maxlength: 18 });
    this.VigenciaInicio = PropertyEntity({ text: "*Vigencia Inicio: ", getType: typesKnockout.date, required: true });
    this.VigenciaFim = PropertyEntity({ text: "Vigencia Fim: ", getType: typesKnockout.date, required: true });

    this.Versao = PropertyEntity({ text: "*Versão: ", required: true, maxlength: 300 });
    this.Fonte = PropertyEntity({ text: "*Fonte: ", required: true, maxlength: 300 });

    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo CSV:", val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.ImportarCSV = PropertyEntity({ eventClick: ImportarCSVClick, type: types.event, text: "Importar CSV", visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadImpostoIBPTNFe() {
    _pesquisaImpostoIBPTNFe = new PesquisaImpostoIBPTNFe();
    KoBindings(_pesquisaImpostoIBPTNFe, "knockoutPesquisaImpostoIBPTNFe", false, _pesquisaImpostoIBPTNFe.Pesquisar.id);

    _impostoIBPTNFe = new ImpostoIBPTNFe();
    KoBindings(_impostoIBPTNFe, "knockoutCadastroImpostoIBPTNFe");

    HeaderAuditoria("ImpostoIBPTNFe", _impostoIBPTNFe);

    buscarImpostoIBPTNFes();
}

function ImportarCSVClick(e, sender) {
    var file = document.getElementById(_impostoIBPTNFe.Arquivo.id);
    var formData = new FormData();
    formData.append("upload", file.files[0]);

    _impostoIBPTNFe.Arquivo.requiredClass("form-control");

    if (_impostoIBPTNFe.Arquivo.val() != "") {
        enviarArquivo("ImpostoIBPTNFe/ImportarCSV?callback=?", { Codigo: _impostoIBPTNFe.Codigo.val() }, formData, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);

                _impostoIBPTNFe.Arquivo.requiredClass("form-control");
                _impostoIBPTNFe.Arquivo.val("");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                return;
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios para a importação!");
        if (_impostoIBPTNFe.Arquivo.val() == "")
            _impostoIBPTNFe.Arquivo.requiredClass("form-control is-invalid");
    }
}

function adicionarClick(e, sender) {
    Salvar(e, "ImpostoIBPTNFe/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridImpostoIBPTNFe.CarregarGrid();
                limparCamposImpostoIBPTNFe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ImpostoIBPTNFe/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridImpostoIBPTNFe.CarregarGrid();
                limparCamposImpostoIBPTNFe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o imposto IBPT " + _impostoIBPTNFe.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_impostoIBPTNFe, "ImpostoIBPTNFe/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridImpostoIBPTNFe.CarregarGrid();
                limparCamposImpostoIBPTNFe();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposImpostoIBPTNFe();
}

//*******MÉTODOS*******


function buscarImpostoIBPTNFes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarImpostoIBPTNFe, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridImpostoIBPTNFe = new GridView(_pesquisaImpostoIBPTNFe.Pesquisar.idGrid, "ImpostoIBPTNFe/Pesquisa", _pesquisaImpostoIBPTNFe, menuOpcoes, null);
    _gridImpostoIBPTNFe.CarregarGrid();
}

function editarImpostoIBPTNFe(impostoIBPTNFeGrid) {
    limparCamposImpostoIBPTNFe();
    _impostoIBPTNFe.Codigo.val(impostoIBPTNFeGrid.Codigo);
    BuscarPorCodigo(_impostoIBPTNFe, "ImpostoIBPTNFe/BuscarPorCodigo", function (arg) {
        _impostoIBPTNFe.Arquivo.val("");
        _pesquisaImpostoIBPTNFe.ExibirFiltros.visibleFade(false);
        _impostoIBPTNFe.Atualizar.visible(true);
        _impostoIBPTNFe.Cancelar.visible(true);
        _impostoIBPTNFe.Excluir.visible(true);
        _impostoIBPTNFe.Adicionar.visible(false);
    }, null);
}

function limparCamposImpostoIBPTNFe() {
    _impostoIBPTNFe.Atualizar.visible(false);
    _impostoIBPTNFe.Cancelar.visible(false);
    _impostoIBPTNFe.Excluir.visible(false);
    _impostoIBPTNFe.Adicionar.visible(true);
    LimparCampos(_impostoIBPTNFe);
    _impostoIBPTNFe.Arquivo.val("");
}