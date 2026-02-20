/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoControleReajusteFretePlanilha.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _CRUDControleReajusteFretePlanilha;

var CRUDControleReajusteFretePlanilha = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
    this.Reprocessar = PropertyEntity({ eventClick: reprocessarClick, type: types.event, text: "Reprocessar", idGrid: guid(), visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparLancamentoClick, type: types.event, text: "Limpar", idGrid: guid(), visible: ko.observable(true) });
}



//*******EVENTOS*******
function loadCRUD() {

    _CRUDControleReajusteFretePlanilha = new CRUDControleReajusteFretePlanilha();
    KoBindings(_CRUDControleReajusteFretePlanilha, "knockoutCRUD");
}

function limparLancamentoClick(e, sender) {
    LimparCamposControle();
}

function adicionarClick(e, sender) {
    if (_controleReajusteFretePlanilha.Planilha.file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Planilha", "Nenhum arquivo selecionado.");

    Salvar(_controleReajusteFretePlanilha, "ControleReajusteFretePlanilha/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var formData = FormDataPlanilha();
                enviarArquivo("ControleReajusteFretePlanilha/Atualizar", arg.Data, formData, function () {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridControleReajusteFretePlanilha.CarregarGrid();
                    LimparCamposControle();
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function reprocessarClick(e, sender) {
    executarReST("ControleReajusteFretePlanilha/ReprocessarRegras", { Codigo: _controleReajusteFretePlanilha.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PossuiRegra) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Reajuste está aguardando aprovação.");
                    _aprovacao.PossuiRegras.val(true);
                    _gridAutorizacoes.CarregarGrid();
                    _controleReajusteFretePlanilha.Situacao.val(arg.Data.Situacao);
                    SetarEtapa();
                    PreencherResumo(arg.Data.Resumo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar o Reajuste.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarClick(e, sender) {
    executarReST("ControleReajusteFretePlanilha/Cancelar", { Codigo: _controleReajusteFretePlanilha.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de reajuste Cancelado.");
                _gridControleReajusteFretePlanilha.CarregarGrid();
                LimparCamposControle();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******METODOS*******
function AlterarBootesCRUD(arg) {
    var situacao = arg.Data.Situacao;

    _CRUDControleReajusteFretePlanilha.Adicionar.visible(false);

    if (situacao == EnumSituacaoControleReajusteFretePlanilha.SemRegra)
        _CRUDControleReajusteFretePlanilha.Reprocessar.visible(true);
    else if (situacao == EnumSituacaoControleReajusteFretePlanilha.AgAprovacao)
        _CRUDControleReajusteFretePlanilha.Cancelar.visible(true);
}

function ResetarBotoesCRUD() {
    _CRUDControleReajusteFretePlanilha.Adicionar.visible(true);
    _CRUDControleReajusteFretePlanilha.Reprocessar.visible(false);
    _CRUDControleReajusteFretePlanilha.Cancelar.visible(false);
}

function FormDataPlanilha() {
    var formData = new FormData();
    formData.append("Arquivo", _controleReajusteFretePlanilha.Planilha.file.files[0]);

    return formData;
}