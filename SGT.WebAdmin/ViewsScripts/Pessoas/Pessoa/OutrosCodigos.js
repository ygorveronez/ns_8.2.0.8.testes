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
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="Pessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoChavePix.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOutroCodigoIntegracao;
var _outrosCodigos;

var OutrosCodigos = function () {

    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoIntegracao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoDeIntegracaoVTEX.getFieldDescription()), maxlength: 50, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadOutrosCodigos() {
    // Checar se tem integração VTEX e só mostrar se tiver

    _outrosCodigos = new OutrosCodigos();
    KoBindings(_outrosCodigos, "knockoutOutrosCodigos");

    executarReST("ConfiguracaoVtex/VerificarExisteIntegracao", {}, (dados) => {
        if (dados.Data.temIntegracao) {
            $("#liTabOutrosCodigos").show();


            var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarOutroCodigoIntegracaoClick }] };

            var header = [
                { data: "Codigo", visible: false },
                { data: "CodigoIntegracao", title: Localization.Resources.Gerais.Geral.CodigoIntegracao, width: "80%" }
            ];

            _gridOutroCodigoIntegracao = new BasicDataTable(_outrosCodigos.Grid.id, header, menuOpcoes);
            recarregarGridOutroCodigoIntegracao();
        }
    });
}

function recarregarGridOutroCodigoIntegracao() {
    var data = new Array();
    if (_gridOutroCodigoIntegracao == undefined)
        return;

    $.each(_pessoa.OutrosCodigosIntegracao.list, function (i, outroCodigoIntegracao) {

        var outroCodigoIntegracaoGrid = new Object();
        outroCodigoIntegracaoGrid.CodigoIntegracao = outroCodigoIntegracao.CodigoIntegracao.val;
        outroCodigoIntegracaoGrid.Codigo = outroCodigoIntegracao.Codigo.val;
        data.push(outroCodigoIntegracaoGrid);
    });
    _gridOutroCodigoIntegracao.CarregarGrid(data);
}


function editarOutroCodigoIntegracaoClick(data) {
    _outrosCodigos.Atualizar.visible(true);
    _outrosCodigos.Cancelar.visible(true);
    _outrosCodigos.Excluir.visible(true);
    _outrosCodigos.Adicionar.visible(false);
    EditarListEntity(_outrosCodigos, data);
}

function excluirOutroCodigoIntegracaoClick() {
    for (var i = 0; i < _pessoa.OutrosCodigosIntegracao.list.length; i++) {
        outroCodigos = _pessoa.OutrosCodigosIntegracao.list[i];
        if (_outrosCodigos.Codigo.val() == outroCodigos.Codigo.val)
            _pessoa.OutrosCodigosIntegracao.list.splice(i, 1);
    }
    limparCamposOutrosCodigos();
    recarregarGridOutroCodigoIntegracao();
}

function adicionarOutroCodigoIntegracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_outrosCodigos);
    if (tudoCerto) {
        var existe = false;
        $.each(_pessoa.OutrosCodigosIntegracao.list, function (i, outroCodigoIntegracao) {
            if (outroCodigoIntegracao.CodigoIntegracao.val == _outrosCodigos.CodigoIntegracao.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _pessoa.OutrosCodigosIntegracao.list.push(SalvarListEntity(_outrosCodigos));
            recarregarGridOutroCodigoIntegracao();
            $("#" + _outrosCodigos.CodigoIntegracao.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
        }
        limparCamposOutrosCodigos();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}

function atualizarOutroCodigoIntegracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_outrosCodigos);
    if (tudoCerto) {
        $.each(_pessoa.OutrosCodigosIntegracao.list, function (i, outroCodigoIntegracao) {
            if (outroCodigoIntegracao.Codigo.val == _outrosCodigos.Codigo.val()) {
                _outrosCodigos.Codigo.val(_outrosCodigos.CodigoIntegracao.val());
                AtualizarListEntity(_outrosCodigos, outroCodigoIntegracao)
                return false;
            }
        });
        recarregarGridOutroCodigoIntegracao();
        limparCamposOutrosCodigos();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}


function cancelarOutroCodigoIntegracaoClick(e) {
    limparCamposOutrosCodigos();
}

function limparCamposOutrosCodigos() {

    _outrosCodigos.Atualizar.visible(false);
    _outrosCodigos.Excluir.visible(false);
    _outrosCodigos.Cancelar.visible(false);
    _outrosCodigos.Adicionar.visible(true);

    LimparCampos(_outrosCodigos);
}