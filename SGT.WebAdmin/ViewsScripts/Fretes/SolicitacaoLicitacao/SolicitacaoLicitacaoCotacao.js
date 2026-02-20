/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="SolicitacaoLicitacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _solicitacaoLicitacaoCotacao;

var _tipoCotacaoOpcoes = [
    { value: 0, text: "Indefinido" },
    { value: 1, text: "Contratação de Frete" },
    { value: 2, text: "Ideia de Frete" }
];

var SolicitacaoLicitacaoCotacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //Resumo
    this.Origem = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Origem:", enable: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Destino:", enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Produto:", enable: ko.observable(true) });
    this.Acondicionamento = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Acondicionamento:", enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Quantidade:", enable: ko.observable(true) });
    this.PeriodoEmbarque = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Período de Embarque:", enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Observação:", enable: ko.observable(true) });

    //Campos
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veícular:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoCotacao = PropertyEntity({ val: ko.observable(0), options: _tipoCotacaoOpcoes, def: 0, text: "Tipo Cotação: ", enable: ko.observable(true) });
    this.ValorTotalNotaFiscal = PropertyEntity({ text: "Valor Total Nota Fiscal:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorTrecho = PropertyEntity({ text: "Valor Trecho:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorTonelada = PropertyEntity({ text: "Valor Tonelada:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorPedagio = PropertyEntity({ text: "Valor Pedágio:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.UsuarioCotacao = PropertyEntity({ text: "Usuário da Cotação:", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadSolicitacaoLicitacaoCotacao() {
    _solicitacaoLicitacaoCotacao = new SolicitacaoLicitacaoCotacao();
    KoBindings(_solicitacaoLicitacaoCotacao, "knockoutCotacao");

    new BuscarTransportadores(_solicitacaoLicitacaoCotacao.Empresa);
    new BuscarModelosVeicularesCarga(_solicitacaoLicitacaoCotacao.ModeloVeicularCarga);
}

function finalizarSolicitacaoLicitacaoClick() {
    if (string.IsNullOrWhiteSpace(_solicitacaoLicitacaoCotacao.ValorTrecho.val()) && string.IsNullOrWhiteSpace(_solicitacaoLicitacaoCotacao.ValorTonelada.val())) {
        exibirMensagem(tipoMensagem.atencao, "Valores obrigatórios", "Favor informar pelo menos o Valor Trecho ou Valor Tonelada!");
        return;
    }

    Salvar(_solicitacaoLicitacaoCotacao, "SolicitacaoLicitacao/Finalizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso");
                _gridSolicitacaoLicitacao.CarregarGrid();
                limparCamposSolicitacaoLicitacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function rejeitarSolicitacaoLicitacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a solicitação criada?", function () {
        executarReST("SolicitacaoLicitacao/Rejeitar", { Codigo: _solicitacaoLicitacao.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Rejeitado com sucesso");
                    _gridSolicitacaoLicitacao.CarregarGrid();
                    limparCamposSolicitacaoLicitacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function limparCamposSolicitacaoLicitacaoCotacao() {
    SetarEnableCamposKnockout(_solicitacaoLicitacaoCotacao, true);
    _solicitacaoLicitacaoCotacao.UsuarioCotacao.visible(false);
}