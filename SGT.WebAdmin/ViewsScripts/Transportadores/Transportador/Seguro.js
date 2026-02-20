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
/// <reference path="../../Enumeradores/EnumResponsavelSeguro.js" />
/// <reference path="../../Consultas/ApoliceSeguro.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _seguro;
var _gridSeguro;

var Seguro = function () {
    this.Transportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.UsarTipoOperacaoApolice = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Transportador.UsarApolicesParaTipoOperacao.getFieldDescription(), val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarTipoOperacaoApolice.val.subscribe(function (usar) {
        _transportador.UsarTipoOperacaoApolice.val(usar);
    });

    this.LiberarEmissaoSemAverbacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.LiberarParaCargasParaEmissaoMesmoAverbarDocumentos, visible: ko.observable(false) });
    this.Apolice = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Apolice.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TipoOperacao.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Desconto = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Desconto.getFieldDescription(), configDecimal: { precision: 4 }, getType: typesKnockout.decimal, visible: ko.observable(false) });

    this.Apolices = PropertyEntity({ type: types.local, text: Localization.Resources.Transportadores.TransportadorApolices, idGrid: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarSeguroClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadSeguro() {
    _seguro = new Seguro();
    KoBindings(_seguro, "knockoutSeguros");

    new BuscarApolicesSeguro(_seguro.Apolice, null, null, null, apoliceSeguroChange);
    new BuscarTiposOperacao(_seguro.TipoOperacao);

    if (_CONFIGURACAO_TMS.NaoPermiteEmitirCargaSemAverbacao) {
        _seguro.LiberarEmissaoSemAverbacao.visible(true);
    }

    //-- Grid Seguros
    // Opcoes
    var remover = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: removerSeguroClick, icone: "" };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Transportadores.Transportador.Opcoes, tamanho: 1, opcoes: [remover] };

    // Grid
    var linhasPorPaginas = 7;
    _gridSeguro = new GridView(_seguro.Apolices.idGrid, "Transportador/PesquisaSeguros", _seguro, menuOpcoes, null, linhasPorPaginas);
}

function apoliceSeguroChange(apolice) {
    _seguro.Apolice.val(apolice.NumeroApolice + ' - ' + apolice.Seguradora);
    _seguro.Apolice.codEntity(apolice.Codigo);

    if (apolice.EnumResponsavel == EnumResponsavelSeguro.Embarcador)
        _seguro.Desconto.visible(true);
    else
        _seguro.Desconto.visible(false);
}

function removerSeguroClick(dataRow, row) {
    executarReST("Transportador/ExcluirSeguroPorCodigo", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                _gridSeguro.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function adicionarSeguroClick(e, sender) {
    Salvar(_seguro, "Transportador/AdicionarSeguro", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridSeguro.CarregarGrid();
                limparCamposSeguro();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}




//*******MÉTODOS*******
function limparCamposSeguro() {
    _seguro.Desconto.val(_seguro.Desconto.def);
    _seguro.Desconto.visible(false);
    _seguro.Apolice.val(_seguro.Apolice.def);
    _seguro.Apolice.codEntity(_seguro.Apolice.defCodEntity);
    _seguro.TipoOperacao.val(_seguro.TipoOperacao.def);
    _seguro.TipoOperacao.codEntity(_seguro.TipoOperacao.defCodEntity);
}

function LimparSeguroTransportador() {
    _seguro.Desconto.visible(false);
    LimparCampos(_seguro);
    $("#liTabSeguros").hide();
}