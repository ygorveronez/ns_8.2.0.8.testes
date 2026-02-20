/// <reference path="FluxoCompra.js" />
/// <reference path="CotacaoMercadoria.js" />
/// <reference path="CotacaoFornecedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cotacao;
var _CRUDCotacao;

var Cotacao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoFluxoCompra = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 2000, required: true, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "*Data Emissão: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.DataPrevisao = PropertyEntity({ text: "*Data Previsão: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });

    this.Mercadorias = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.Fornecedores = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDCotacao = function () {
    this.SalvarCotacao = PropertyEntity({ eventClick: SalvarCotacaoClick, type: types.event, text: "Salvar Cotação", visible: ko.observable(true) });
    this.EnviarCotacao = PropertyEntity({ eventClick: EnviarCotacaoClick, type: types.event, text: "Enviar Cotação para Fornecedor", visible: ko.observable(true) });
    this.AvancarCotacaoRetornada = PropertyEntity({ eventClick: AvancarCotacaoRetornadaClick, type: types.event, text: "Avançar Cotação Retornada", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCotacao() {
    _cotacao = new Cotacao();
    KoBindings(_cotacao, "knockoutCadastroCotacao");

    _CRUDCotacao = new CRUDCotacao();
    KoBindings(_CRUDCotacao, "knockoutCRUDCotacao");

    LoadCotacaoMercadoria();
    LoadCotacaoFornecedor();
}

function SalvarCotacaoClick() {
    Salvar(_cotacao, "FluxoCompraCotacao/AtualizarCotacao", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cotação atualizada com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function EnviarCotacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja enviar a cotação para o fornecedor?<br>Tenha certeza de que a mesma já foi salva!", function () {
        var dados = {
            Codigo: _cotacao.Codigo.val(),
            CodigoFluxoCompra: _fluxoCompra.Codigo.val()
        };
        executarReST("FluxoCompraCotacao/DisponibilizarCotacaoParaFornecedor", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cotação disponibilizada com sucesso!");

                    _fluxoCompra.EtapaAtual.val(EnumEtapaFluxoCompra.RetornoCotacao);
                    controleCamposCotacaoFluxoCompra();
                    controleCamposCotacaoRetornoFluxoCompra();

                    RecarregarGridPesquisa();
                    SetarEtapaFluxoCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function AvancarCotacaoRetornadaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja avançar a cotação e salvar os novos fornecedores?", function () {
        Salvar(_cotacao, "FluxoCompraCotacao/AvancarCotacaoRetornada", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa de Cotação avançada com sucesso!");

                    _fluxoCompra.EtapaAtual.val(EnumEtapaFluxoCompra.RetornoCotacao);
                    _fluxoCompra.VoltouParaEtapaAtual.val(false);

                    controleCamposCotacaoFluxoCompra();
                    controleCamposCotacaoRetornoFluxoCompra();

                    RecarregarGridPesquisa();
                    SetarEtapaFluxoCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

////*******MÉTODOS*******

function CarregarCotacaoFluxoCompra() {
    BuscarPorCodigo(_cotacao, "FluxoCompraCotacao/BuscarCotacaoPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _cotacao.CodigoFluxoCompra.val(_fluxoCompra.Codigo.val());

                RecarregarGridCotacaoMercadoria();
                RecarregarGridCotacaoFornecedor();

                if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.Cotacao && !_fluxoCompra.VoltouParaEtapaAtual.val() && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto) {
                    _gridCotacaoMercadoria.HabilitarOpcoes();
                    _gridCotacaoFornecedor.HabilitarOpcoes();
                } else {
                    _gridCotacaoMercadoria.DesabilitarOpcoes();
                    _gridCotacaoFornecedor.DesabilitarOpcoes();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function controleCamposCotacaoFluxoCompra() {
    _CRUDCotacao.SalvarCotacao.visible(false);
    _CRUDCotacao.EnviarCotacao.visible(false);
    _CRUDCotacao.AvancarCotacaoRetornada.visible(false);
    _CRUDCotacaoMercadoria.Adicionar.visible(false);
    _CRUDCotacaoFornecedor.Adicionar.visible(false);

    if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.Cotacao && !_fluxoCompra.VoltouParaEtapaAtual.val() && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto) {
        _CRUDCotacao.SalvarCotacao.visible(true);
        _CRUDCotacao.EnviarCotacao.visible(true);
        _CRUDCotacaoMercadoria.Adicionar.visible(true);
        _CRUDCotacaoFornecedor.Adicionar.visible(true);
    }
    else {
        SetarEnableCamposKnockout(_cotacao, false);
        SetarEnableCamposKnockout(_cotacaoMercadoria, false);
        SetarEnableCamposKnockout(_cotacaoFornecedor, false);
    }

    if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.Cotacao && _fluxoCompra.VoltouParaEtapaAtual.val() && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto) {
        _CRUDCotacao.AvancarCotacaoRetornada.visible(true);
        _CRUDCotacaoFornecedor.Adicionar.visible(true);
        SetarEnableCamposKnockout(_cotacaoFornecedor, true);
    }
}

function setarCotacaoFluxoCompra(codigo) {
    _cotacao.Codigo.val(codigo);
}

function LimparCamposCotacaoFluxoCompra() {
    _CRUDCotacao.SalvarCotacao.visible(true);
    _CRUDCotacao.EnviarCotacao.visible(true);

    SetarEnableCamposKnockout(_cotacao, true);
    SetarEnableCamposKnockout(_cotacaoMercadoria, true);
    SetarEnableCamposKnockout(_cotacaoFornecedor, true);

    LimparCampos(_cotacao);
    LimparCamposCotacaoMercadoria();
    LimparCamposCotacaoFornecedor();

    Global.ResetarAbas();
}