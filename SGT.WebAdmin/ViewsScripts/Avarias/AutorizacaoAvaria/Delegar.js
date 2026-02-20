/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _delegarSelecionados;
var _delegar;

var DelegarSelecionados = function () {
    this.UsuarioDelegado = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarAvariaSelecionadosClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDelegarSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var Delegar = function () {
    this.UsuarioDelegado = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadDelegar() {
    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");

    _delegarSelecionados = new DelegarSelecionados();
    KoBindings(_delegarSelecionados, "knockoutDelegarAvaria");
    
    new BuscarFuncionario(_delegar.UsuarioDelegado);
    new BuscarFuncionario(_delegarSelecionados.UsuarioDelegado);
}

function delegarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar a solicitação?", function () {
        var dados = {
            Solicitacao: _solicitacaoAvaria.Codigo.val(),
            UsuarioDelegado: _delegar.UsuarioDelegado.codEntity(),
        };

        executarReST("AutorizacaoAvaria/DelegarSolicitacao", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    buscarAvarias();
                    _gridRegras.CarregarGrid();
                    LimparDelegar();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function delegarAvariaSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar todas as solicitações selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAvarias);
        var delegar = RetornarObjetoPesquisa(_delegarSelecionados);

        dados.UsuarioDelegado = delegar.UsuarioDelegado;
        dados.SelecionarTodos = _pesquisaAvarias.SelecionarTodos.val();
        dados.AvariasSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosSelecionados());
        dados.AvariasNaoSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAvaria/DelegarMultiplasSolicitacoes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    _gridRegras.CarregarGrid();
                    buscarAvarias();
                    cancelarDelegarSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarDelegarSelecionadosClick() {
    LimparCampos(_delegarSelecionados);
    Global.fecharModal("divModalDelegarAvaria");
}




//*******MÉTODOS*******

function CarregarDelegar(situacao) {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        if (!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar && (situacao == EnumSituacaoAvaria.AgLote || situacao == EnumSituacaoAvaria.AgAprovacao))
            $("#liDelegar").show();
        else
            $("#liDelegar").hide();
    }
}

function LimparDelegar() {
    LimparCampos(_delegar);
}