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

var _motivoDevolucaoEntrega;
var _pesquisaMotivoDevolucaoEntrega;
var _gridMotivoDevolucaoEntrega;

var MotivoDevolucaoEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 556, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });
    this.MotivoChamado = PropertyEntity({ type: types.entity, required: _LiberarCamposV3Trizy, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: "Motivo Chamado (Se informado a devolução precisa ser aprovada em chamado):", issue: 926, enable: ko.observable(true), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.ObrigarFoto = PropertyEntity({ getType: typesKnockout.bool, text: "Ao solicitar uma devolução é obrigatório enviar uma foto?", val: ko.observable(false), def: false, visible: ko.observable(!_LiberarCamposV3Trizy) });
    this.ChecklistSuperApp = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: ko.observable(""), text: "Checklist Super App", idBtnSearch: guid(), required: ko.observable(_LiberarCamposV3Trizy), visible: ko.observable(_LiberarCamposV3Trizy), enable: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: ko.observable(_LiberarCamposV3Trizy), text: "Tipo da Ocorrência", idBtnSearch: guid(), visible: ko.observable(_LiberarCamposV3Trizy) });
    this.EntregaParcialSuperAppId = PropertyEntity({ text: "Motivo Entrega Parcial Super App Id:", required: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.string, val: ko.observable("") });
    this.NaoEntregaSuperAppId = PropertyEntity({ text: "Motivo Não Entrega Super App Id:", required: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.string, val: ko.observable("") });
    
}

var PesquisaMotivoDevolucaoEntrega = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoDevolucaoEntrega.CarregarGrid();
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
function loadMotivoDevolucaoEntrega() {
    _pesquisaMotivoDevolucaoEntrega = new PesquisaMotivoDevolucaoEntrega();
    KoBindings(_pesquisaMotivoDevolucaoEntrega, "knockoutPesquisaMotivoDevolucaoEntrega", false, _pesquisaMotivoDevolucaoEntrega.Pesquisar.id);

    _motivoDevolucaoEntrega = new MotivoDevolucaoEntrega();
    KoBindings(_motivoDevolucaoEntrega, "knockoutMotivoDevolucaoEntrega");
    BuscarMotivoChamado(_motivoDevolucaoEntrega.MotivoChamado);
    BuscarChecklistsSuperApp(_motivoDevolucaoEntrega.ChecklistSuperApp);
    BuscarTipoOcorrencia(_motivoDevolucaoEntrega.TipoOcorrencia);

    HeaderAuditoria("MotivoDevolucaoEntrega", _motivoDevolucaoEntrega);

    buscarMotivoDevolucaoEntrega();
}

function adicionarClick(e, sender) {
    Salvar(_motivoDevolucaoEntrega, "MotivoDevolucaoEntrega/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoDevolucaoEntrega.CarregarGrid();
                limparCamposMotivoDevolucaoEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoDevolucaoEntrega, "MotivoDevolucaoEntrega/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoDevolucaoEntrega.CarregarGrid();
                limparCamposMotivoDevolucaoEntrega();
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
        ExcluirPorCodigo(_motivoDevolucaoEntrega, "MotivoDevolucaoEntrega/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoDevolucaoEntrega.CarregarGrid();
                    limparCamposMotivoDevolucaoEntrega();
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
    limparCamposMotivoDevolucaoEntrega();
}

function editarMotivoDevolucaoEntregaClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoDevolucaoEntrega();

    _motivoDevolucaoEntrega.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoDevolucaoEntrega, "MotivoDevolucaoEntrega/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoDevolucaoEntrega.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoDevolucaoEntrega.Atualizar.visible(true);
                _motivoDevolucaoEntrega.Excluir.visible(true);
                _motivoDevolucaoEntrega.Cancelar.visible(true);
                _motivoDevolucaoEntrega.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarMotivoDevolucaoEntrega() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoDevolucaoEntregaClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    //var configExportacao = {
    //    url: "MotivoDevolucaoEntrega/ExportarPesquisa",
    //    titulo: "Motivo Rejeição"
    //};


    // Inicia Grid de busca
    _gridMotivoDevolucaoEntrega = new GridViewExportacao(_pesquisaMotivoDevolucaoEntrega.Pesquisar.idGrid, "MotivoDevolucaoEntrega/Pesquisa", _pesquisaMotivoDevolucaoEntrega, menuOpcoes, null);
    _gridMotivoDevolucaoEntrega.CarregarGrid();
}

function limparCamposMotivoDevolucaoEntrega() {
    _motivoDevolucaoEntrega.Atualizar.visible(false);
    _motivoDevolucaoEntrega.Cancelar.visible(false);
    _motivoDevolucaoEntrega.Excluir.visible(false);
    _motivoDevolucaoEntrega.Adicionar.visible(true);
    LimparCampos(_motivoDevolucaoEntrega);
}