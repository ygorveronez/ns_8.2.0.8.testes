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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="BoletoAlteracao.js" />
/// <reference path="EtapaBoletoAlteracao.js" />
/// <reference path="ImpressaoBoletoAlteracao.js" />
/// <reference path="EmailBoletoAlteracao.js" />
/// <reference path="RemessaBoletoAlteracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _alteracaoBoletoAlteracao;
var _gridBoletosAlteracao;

var AlteracaoBoletoAlteracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NovaDataVencimento = PropertyEntity({ text: "Nova Data Vencimento: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.AlterarBoletos = PropertyEntity({ eventClick: AlterarBoletosClick, type: types.event, text: "Alterar p/ Todos", visible: ko.observable(true), enable: ko.observable(true) });
    
    this.Boletos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    
    this.Anterior = PropertyEntity({ eventClick: AnteriorAlteracaoBoletoAlteracaoClick, type: types.event, text: "Anterior", visible: ko.observable(true), enable: ko.observable(true) });
    this.Proximo = PropertyEntity({ eventClick: ProximoAlteracaoBoletoAlteracaoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadAlteracaoBoletoAlteracao() {
    _alteracaoBoletoAlteracao = new AlteracaoBoletoAlteracao();
    KoBindings(_alteracaoBoletoAlteracao, "knockoutAlteracaoBoletoAlteracao");

    BuscarBoletosAlteracao();
}

function AlterarBoletosClick(e, sender) {
    if (_alteracaoBoletoAlteracao.NovaDataVencimento.val() != undefined && _alteracaoBoletoAlteracao.NovaDataVencimento.val() != "") {
        var data = { NovaDataVencimento: _alteracaoBoletoAlteracao.NovaDataVencimento.val(), Codigo: _alteracaoBoletoAlteracao.Codigo.val() };
        executarReST("BoletoAlteracao/AlterarDataVencimento", data, function (arg) {
            if (arg.Success) {
                BuscarBoletosAlteracao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem("Atenção", "Campo Obrigatório", "Por Favor, informe a nova data de vencimento.");
    }
}

function AnteriorAlteracaoBoletoAlteracaoClick(e, sender) {
    var data = { Codigo: _alteracaoBoletoAlteracao.Codigo.val(), Etapa: EnumBoletoAlteracaoEtapa.Selecao };
    executarReST("BoletoAlteracao/AtualizarEtapa", data, function (arg) {
        if (arg.Success) {
            _boletoAlteracao.Etapa.val(arg.Data.Etapa);
            PosicionarEtapa(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ProximoAlteracaoBoletoAlteracaoClick(e, sender) {
    var data = { Codigo: _alteracaoBoletoAlteracao.Codigo.val(), Etapa: EnumBoletoAlteracaoEtapa.Impressao };
    executarReST("BoletoAlteracao/AtualizarEtapa", data, function (arg) {
        if (arg.Success) {
            _boletoAlteracao.Etapa.val(arg.Data.Etapa);
            PosicionarEtapa(arg.Data);            
            _boletoAlteracao.Codigo.val(arg.Data.Codigo);
            _alteracaoBoletoAlteracao.Codigo.val(arg.Data.Codigo);
            _emailBoletoAlteracao.Codigo.val(arg.Data.Codigo);
            _remessaBoletoAlteracao.Codigo.val(arg.Data.Codigo);
            _impressaoBoletoAlteracao.Codigo.val(arg.Data.Codigo);

            $("#knockoutImpressaoBoletoAlteracao").show();
            _etapaAtual = 3;
            $("#" + _etapaBoletoAlteracao.Etapa3.idTab).click();

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa de alteração concluída, siga a etapa 3.");

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function BuscarBoletosAlteracao() {
    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: true };
    _gridBoletosAlteracao = new GridView(_alteracaoBoletoAlteracao.Boletos.idGrid, "BoletoAlteracao/PesquisaBoletosAlteracao", _alteracaoBoletoAlteracao, null, null, null, null, null, null, null, null, editarColuna);
    _gridBoletosAlteracao.CarregarGrid();
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, DataVencimentoAlterado: dataRow.DataVencimentoAlterado, Observacao: dataRow.Observacao };;
    executarReST("BoletoAlteracao/SalvarAlteracoes", data, function (arg) {
        if (arg.Success) {
            //BuscarBoletosAlteracao();
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}


function limparCamposAlteracao() {
    LimparCampos(_alteracaoBoletoAlteracao);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}