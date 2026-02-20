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
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Enumeradores/EnumDownloadRealizado.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridBoletoRemessa;
var _boletoRemessa;
var _pesquisaBoletoRemessa;

var PesquisaBoletoRemessa = function () {
    this.NumeroSequencial = PropertyEntity({ text: "Número da Remessa: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração Boleto (Banco):", idBtnSearch: guid() });
    this.DownloadRealizado = PropertyEntity({ text: "Status Download:", val: ko.observable(EnumDownloadRealizado.Todos), options: EnumDownloadRealizado.obterOpcoesPesquisa(), def: EnumDownloadRealizado.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBoletoRemessa.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var BoletoRemessa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroSequencial = PropertyEntity({ text: "*Número Sequencial: ", required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: false });
    this.DataGeracao = PropertyEntity({ text: "*Data da Geração: ", required: true, getType: typesKnockout.date, enable: false });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Configuração Boleto (Banco):", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: false });
    this.CaminhoArquivo = PropertyEntity({ text: "*Caminho arquivo: ", required: true, maxlength: 300, enable: false });
    this.RemessaDeCancelamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Remessa de Cancelamento?", enable: ko.observable(false) });

    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir / Limpar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Download = PropertyEntity({ eventClick: downloadClick, type: types.event, text: "Download", visible: ko.observable(false) });
    this.LimparBoletos = PropertyEntity({ eventClick: limparDadosBoletosClick, type: types.event, text: "Limpar dados de todos os Boletos", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadBoletoRemessa() {
    _boletoRemessa = new BoletoRemessa();
    KoBindings(_boletoRemessa, "knockoutCadastroBoletoRemessa");

    HeaderAuditoria("BoletoRemessa", _boletoRemessa);

    _pesquisaBoletoRemessa = new PesquisaBoletoRemessa();
    KoBindings(_pesquisaBoletoRemessa, "knockoutPesquisaBoletoRemessa", false, _pesquisaBoletoRemessa.Pesquisar.id);

    new BuscarBoletoConfiguracao(_boletoRemessa.BoletoConfiguracao);
    new BuscarBoletoConfiguracao(_pesquisaBoletoRemessa.BoletoConfiguracao);

    buscarBoletoRemessas();
}

function downloadClick() {
    var dados = { Codigo: _boletoRemessa.Codigo.val() };
    executarDownload("BoletoRemessa/DownloadRemessa", dados);
}

function excluirClick() {
    if (_boletoRemessa.RemessaDeCancelamento.val()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é permitido excluir Remessa de Cancelamento.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir e limpar todos os títulos que possuem esta remessa?", function () {
        ExcluirPorCodigo(_boletoRemessa, "BoletoRemessa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído / Limpado com sucesso");
                    _gridBoletoRemessa.CarregarGrid();
                    limparCamposBoletoRemessa();
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }, null);
    });
}

function limparDadosBoletosClick() {
    if (_boletoRemessa.RemessaDeCancelamento.val()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível limpar dados de Remessa de Cancelamento.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja limpar os dados de todos os boletos desta remessa?", function () {
        var dados = { Codigo: _boletoRemessa.Codigo.val() };
        executarReST("BoletoRemessa/LimparDadosBoletos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Boleto(s) da remessa excluído(s) do sistema com sucesso.");
                    _gridBoletoRemessa.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

function cancelarClick(e) {
    limparCamposBoletoRemessa();
}

//*******MÉTODOS*******

function buscarBoletoRemessas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBoletoRemessa, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBoletoRemessa = new GridView(_pesquisaBoletoRemessa.Pesquisar.idGrid, "BoletoRemessa/Pesquisa", _pesquisaBoletoRemessa, menuOpcoes, { column: 2, dir: orderDir.desc });
    _gridBoletoRemessa.CarregarGrid();
}

function editarBoletoRemessa(BoletoRemessaGrid) {
    limparCamposBoletoRemessa();
    _boletoRemessa.Codigo.val(BoletoRemessaGrid.Codigo);
    BuscarPorCodigo(_boletoRemessa, "BoletoRemessa/BuscarPorCodigo", function (arg) {
        _pesquisaBoletoRemessa.ExibirFiltros.visibleFade(false);
        _boletoRemessa.Cancelar.visible(true);
        _boletoRemessa.Excluir.visible(true);
        _boletoRemessa.Download.visible(true);
        _boletoRemessa.LimparBoletos.visible(true);
    }, null);
}

function limparCamposBoletoRemessa() {
    _boletoRemessa.Cancelar.visible(false);
    _boletoRemessa.Excluir.visible(false);
    _boletoRemessa.Download.visible(false);
    _boletoRemessa.LimparBoletos.visible(false);
    LimparCampos(_boletoRemessa);
}