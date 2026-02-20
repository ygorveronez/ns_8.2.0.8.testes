/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeModeloCarroceria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloCarroceria;
var _modeloCarroceria;
var _pesquisaModeloCarroceria;

var PesquisaModeloCarroceria = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridModeloCarroceria.CarregarGrid();
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

var ModeloCarroceria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:",issue: 586, required: true, maxlength: 150 });
    this.PercentualAdicionalFrete = PropertyEntity({ text: "% Adicional Frete:", issue: 858,  maxlength: 5, getType: typesKnockout.decimal });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("Componente de Frete:"),issue: 85, idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });
    this.Prioridade = PropertyEntity({ val: ko.observable(EnumPrioridadeModeloCarroceria.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });
    this.ObrigatorioInformarDataValidadeAdicionalCarroceria = PropertyEntity({ text: "Obrigatório informar data de validade do adicional de carroceria?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CodigoIntegracao = PropertyEntity({ text: "Cod. Integração:", maxlength: 50 });
    

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.PercentualAdicionalFrete.val.subscribe(function (novoValor) {
        var percentualAdicional = Globalize.parseFloat(_modeloCarroceria.PercentualAdicionalFrete.val());

        if (!isNaN(percentualAdicional) && percentualAdicional > 0) {
            _modeloCarroceria.ComponenteFrete.required(true);
            _modeloCarroceria.ComponenteFrete.text("*Componente de Frete:");
        } else {
            _modeloCarroceria.ComponenteFrete.required(false);
            _modeloCarroceria.ComponenteFrete.text("Componente de Frete:");
        }
    });
}

//*******EVENTOS*******

function LoadModeloCarroceria() {

    _pesquisaModeloCarroceria = new PesquisaModeloCarroceria();
    KoBindings(_pesquisaModeloCarroceria, "knockoutPesquisaModeloCarroceria");

    _modeloCarroceria = new ModeloCarroceria();
    KoBindings(_modeloCarroceria, "knockoutCadastroModeloCarroceria");

    HeaderAuditoria("ModeloCarroceria", _modeloCarroceria);

    new BuscarComponentesDeFrete(_modeloCarroceria.ComponenteFrete);

    buscarMarcasEquipamento();
}

function AdicionarClick(e, sender) {
    Salvar(_modeloCarroceria, "ModeloCarroceria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridModeloCarroceria.CarregarGrid();
                LimparCamposModeloCarroceria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function AtualizarClick(e, sender) {
    Salvar(_modeloCarroceria, "ModeloCarroceria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridModeloCarroceria.CarregarGrid();
                LimparCamposModeloCarroceria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o modelo de carroceria " + _modeloCarroceria.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_modeloCarroceria, "ModeloCarroceria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridModeloCarroceria.CarregarGrid();
                    LimparCamposModeloCarroceria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function CancelarClick(e) {
    LimparCamposModeloCarroceria();
}

//*******MÉTODOS*******


function EditarModeloCarroceria(marcaEquipamentoGrid) {
    LimparCamposModeloCarroceria();
    _modeloCarroceria.Codigo.val(marcaEquipamentoGrid.Codigo);
    BuscarPorCodigo(_modeloCarroceria, "ModeloCarroceria/BuscarPorCodigo", function (arg) {
        _pesquisaModeloCarroceria.ExibirFiltros.visibleFade(false);
        _modeloCarroceria.Atualizar.visible(true);
        _modeloCarroceria.Cancelar.visible(true);
        _modeloCarroceria.Excluir.visible(true);
        _modeloCarroceria.Adicionar.visible(false);
    }, null);
}


function buscarMarcasEquipamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarModeloCarroceria, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridModeloCarroceria = new GridView(_pesquisaModeloCarroceria.Pesquisar.idGrid, "ModeloCarroceria/Pesquisa", _pesquisaModeloCarroceria, menuOpcoes, null);
    _gridModeloCarroceria.CarregarGrid();
}


function LimparCamposModeloCarroceria() {
    _modeloCarroceria.Atualizar.visible(false);
    _modeloCarroceria.Cancelar.visible(false);
    _modeloCarroceria.Excluir.visible(false);
    _modeloCarroceria.Adicionar.visible(true);
    LimparCampos(_modeloCarroceria);
}

function ExibirCamposObrigatorios() {
    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios!");
}