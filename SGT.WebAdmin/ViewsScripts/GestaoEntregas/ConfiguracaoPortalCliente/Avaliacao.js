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

var _avalicaoPedido;
var _gridAvalicaoPedido;

var AvalicaoPedido = function () {
    this.Grid = PropertyEntity({ idGrid: guid() });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Titulo = PropertyEntity({ type: types.map, text: Localization.Resources.Gerais.Geral.Titulo.getFieldDescription()});
    this.Descricao = PropertyEntity({ type: types.map, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 1000 });
    this.Ordem = PropertyEntity({ type: types.map, getType: typesKnockout.int, text: Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.Ordem.getFieldDescription()});

    this.Adicionar = PropertyEntity({ eventClick: adicionarPerguntaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPerguntaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirPerguntaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPerguntaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function loadConfiguracaoAvaliacao() {
    _avalicaoPedido = new AvalicaoPedido();
    KoBindings(_avalicaoPedido, "knockoutAvalicaoPedido");

    loadPerguntas();
}

function loadPerguntas() {
    var editar = {
        descricao: Localization.Resources.Gerais.Geral.Editar,
        id: guid(),
        evento: "onclick",
        metodo: editarPergunta,
        tamanho: 10,
        icone: ""
    };
    var auditar = {
        descricao: Localization.Resources.Gerais.Geral.Auditar,
        id: guid(),
        evento: "onclick",
        metodo: OpcaoAuditoria("PortalClientePerguntaAvaliacao", null, _avalicaoPedido),
        tamanho: 10,
        icone: ""
    };
    var menuOpcoes = {
        tipo: PermiteAuditar() ? TypeOptionMenu.list : TypeOptionMenu.link,
        descricao: Localization.Resources.Gerais.Geral.Opcoes,
        tamanho: 10,
        opcoes: [editar]
    };

    if (PermiteAuditar())
        menuOpcoes.opcoes.push(auditar);

    _gridAvalicaoPedido = new GridView(_avalicaoPedido.Grid.idGrid, "ConfiguracaoPortalCliente/Perguntas", null, menuOpcoes);
    _gridAvalicaoPedido.CarregarGrid();
}

function adicionarPerguntaClick(e, sender) {
    Salvar(e, "ConfiguracaoPortalCliente/AdicionarPergunta", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadoComSucesso);
                _gridAvalicaoPedido.CarregarGrid();
                LimparCamposAvaliacaoPedido();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarPerguntaClick(e, sender) {
    Salvar(e, "ConfiguracaoPortalCliente/AtualizarPergunta", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridAvalicaoPedido.CarregarGrid();
                LimparCamposAvaliacaoPedido();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function excluirPerguntaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, + (string.format(Localization.Resources.GestaoEntregas.ConfiguracaoPortalCliente.RealmenteDesejaExcluirAPergunta, e.Titulo.val())), function () {
        var dados = {
            Codigo: e.Codigo.val(),
        };
        if (sender != null) {
            var btn = $("#" + sender.currentTarget.id);
            btn.button('loading');
        }

        executarReST("ConfiguracaoPortalCliente/ExcluirPergunta", dados, function (arg) {
            if (sender != null) btn.button('reset');

            if (arg.Success) {
                if (arg.Data != false) {
                    
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    LimparCamposAvaliacaoPedido();
                    _gridAvalicaoPedido.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarPerguntaClick(e, sender) {
    LimparCamposAvaliacaoPedido();
}




//*******METODOS*******
function editarPergunta(dataRow) {
    _avalicaoPedido.Codigo.val(dataRow.Codigo);

    _avalicaoPedido.Titulo.val(dataRow.Titulo);
    _avalicaoPedido.Descricao.val(dataRow.Conteudo);
    _avalicaoPedido.Ordem.val(dataRow.Ordem);

    _avalicaoPedido.Atualizar.visible(true);
    _avalicaoPedido.Excluir.visible(true);
    _avalicaoPedido.Cancelar.visible(true);
    _avalicaoPedido.Adicionar.visible(false);
}

function LimparCamposAvaliacaoPedido() {
    _avalicaoPedido.Adicionar.visible(true);
    _avalicaoPedido.Cancelar.visible(true);
    _avalicaoPedido.Atualizar.visible(false);
    _avalicaoPedido.Excluir.visible(false);
    LimparCampos(_avalicaoPedido);
}