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


//*******MAPEAMENTO KNOUCKOUT*******

var _provisaoManual;
var _pesquisaProvisaoManual;
var _gridProvisaoManual;

var ProvisaoManual = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    this.ValorProvisionado = PropertyEntity({ enable: ko.observable(true), visible: ko.observable(true), text: "*Valor Provisionado: ", required: true, def: "", val: ko.observable(""), getType: typesKnockout.decimal });

    this.DataInicio = PropertyEntity({ text: "*Data Inicio:", required: true, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "*Data Fim:", required: true, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    
    this.Descricao = PropertyEntity({ text: "*Senha Agendamento:", required: false, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false) });
    this.Motorista = PropertyEntity({ text: "Motorista:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    
    this.CentroResultado = PropertyEntity({ type: types.entity, required: true, codEntity: ko.observable(0), text: "*Centro de Resultado:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, required: true, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid() });

    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaProvisaoManual = function () {
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity,  codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProvisaoManual.CarregarGrid();
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
}


//*******EVENTOS*******
function loadProvisaoManual() {
    _pesquisaProvisaoManual = new PesquisaProvisaoManual();
    KoBindings(_pesquisaProvisaoManual, "knockoutPesquisaProvisaoManual", false, _pesquisaProvisaoManual.Pesquisar.id);

    _provisaoManual = new ProvisaoManual();
    KoBindings(_provisaoManual, "knockoutProvisaoManual");

    new BuscarFilial(_provisaoManual.Filial);
    new BuscarCentroResultado(_provisaoManual.CentroResultado);

    HeaderAuditoria("ProvisaoManual", _provisaoManual);

    buscarProvisaoManual();
}

function adicionarClick(e, sender) {
    Salvar(_provisaoManual, "ProvisaoManual/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProvisaoManual.CarregarGrid();
                limparCamposProvisaoManual();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_provisaoManual, "ProvisaoManual/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProvisaoManual.CarregarGrid();
                limparCamposProvisaoManual();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_provisaoManual, "ProvisaoManual/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProvisaoManual.CarregarGrid();
                    limparCamposProvisaoManual();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposProvisaoManual();
}

function editarProvisaoManualClick(itemGrid) {
    // Limpa os campos
    limparCamposProvisaoManual();

    _provisaoManual.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_provisaoManual, "ProvisaoManual/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProvisaoManual.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _provisaoManual.Atualizar.visible(true);
                _provisaoManual.Excluir.visible(true);
                _provisaoManual.Cancelar.visible(true);
                _provisaoManual.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProvisaoManual() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProvisaoManualClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "ProvisaoManual/ExportarPesquisa",
        titulo: "Motivo Rejeição"
    };


    // Inicia Grid de busca
    _gridProvisaoManual = new GridViewExportacao(_pesquisaProvisaoManual.Pesquisar.idGrid, "ProvisaoManual/Pesquisa", _pesquisaProvisaoManual, menuOpcoes, configExportacao);
    _gridProvisaoManual.CarregarGrid();
}

function limparCamposProvisaoManual() {
    _provisaoManual.Atualizar.visible(false);
    _provisaoManual.Cancelar.visible(false);
    _provisaoManual.Excluir.visible(false);
    _provisaoManual.Adicionar.visible(true);
    LimparCampos(_provisaoManual);
}