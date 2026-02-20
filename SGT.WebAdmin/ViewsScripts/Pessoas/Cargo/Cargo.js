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

var _representante;
var _pesquisaCargo;
var _gridCargo;

var Cargo = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.ValorFaturamento = PropertyEntity({ text: "Valor do Faturamento Mínimo:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(true) });
    this.ValorBonificacao = PropertyEntity({ text: "Valor Bonificação:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(true) });

    this.ComissaoPadrao = PropertyEntity({ text: "% Comissão Padrão:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(false) });
    this.MediaEquivalente = PropertyEntity({ text: "% Média Equivalente:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(true) });
    this.SinistroEquivalente = PropertyEntity({ text: "% Sinistro Equivalente:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(true) });
    this.AdvertenciaEquivalente = PropertyEntity({ text: "% Advertência Equivalente:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(true) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaCargo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargo.CarregarGrid();
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
function loadCargo() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCargo = new PesquisaCargo();
    KoBindings(_pesquisaCargo, "knockoutPesquisaCargo", false, _pesquisaCargo.Pesquisar.id);

    // Instancia ProdutoAvaria
    _representante = new Cargo();
    KoBindings(_representante, "knockoutCargo");

    HeaderAuditoria("Cargo", _representante);

    verificaMultiEmbarcador();
    // Inicia busca
    buscarCargo();
}

function adicionarClick(e, sender) {
    Salvar(_representante, "Cargo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCargo.CarregarGrid();
                limparCamposCargo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_representante, "Cargo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCargo.CarregarGrid();
                limparCamposCargo();
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
        ExcluirPorCodigo(_representante, "Cargo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCargo.CarregarGrid();
                    limparCamposCargo();
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
    limparCamposCargo();
}

function editarCargoClick(itemGrid) {
    // Limpa os campos
    limparCamposCargo();

    // Seta o codigo do ProdutoAvaria
    _representante.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_representante, "Cargo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaCargo.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _representante.Atualizar.visible(true);
                _representante.Excluir.visible(true);
                _representante.Cancelar.visible(true);
                _representante.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarCargo() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCargoClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "Cargo/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };


    // Inicia Grid de busca
    _gridCargo = new GridView(_pesquisaCargo.Pesquisar.idGrid, "Cargo/Pesquisa", _pesquisaCargo, menuOpcoes);
    _gridCargo.CarregarGrid();
}

function limparCamposCargo() {
    _representante.Atualizar.visible(false);
    _representante.Cancelar.visible(false);
    _representante.Excluir.visible(false);
    _representante.Adicionar.visible(true);
    LimparCampos(_representante);
}

function verificaMultiEmbarcador() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _representante.ValorFaturamento.visible(false);
        _representante.ValorBonificacao.visible(false);
        _representante.MediaEquivalente.visible(false);
        _representante.SinistroEquivalente.visible(false);
        _representante.AdvertenciaEquivalente.visible(false);

        $("#descricao").removeClass("col-md-3");
        $("#descricao").addClass("col-md-9");
    }
}