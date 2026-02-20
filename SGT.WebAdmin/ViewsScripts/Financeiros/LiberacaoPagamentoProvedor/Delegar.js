/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _delegar;

var Delegar = function () {
    this.UsuarioDelegado = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadDelegar() {
    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");
    
    new BuscarFuncionario(_delegar.UsuarioDelegado);
}

function delegarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar a solicitação?", function () {
        var dados = {
            CodigoPagamentoProvedor: _documentosProvedor.CodigoPagamentoProvedor.val(),
            UsuarioDelegado: _delegar.UsuarioDelegado.codEntity(),
        };

        executarReST("AutorizacaoProvedor/Delegar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                    _gridRegrasLiberacaoPagamentoProvedor.CarregarGrid();
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

function LimparDelegar() {
    LimparCampos(_delegar);
}