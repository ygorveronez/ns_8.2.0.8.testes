/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
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
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaGuarita.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridExpedicao;
var _pesquisaExpedicao;

var PesquisaExpedicao = function () {

    var data = moment().format("DD/MM/YYYY");

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaGuarita.Todas), options: EnumSituacaoCargaGuarita.obterOpcoesPesquisaExpedicao(), def: EnumSituacaoCargaGuarita.Todas, text: "Situação na Guarita:" });
    this.DataCarregamento = PropertyEntity({ text: "*Data de Carregamento:", getType: typesKnockout.date, val: ko.observable(data), def: data, required: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:",issue: 69, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:",issue: 145, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:",issue: 143, idBtnSearch: guid() });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:",issue: 320, idBtnSearch: guid(), required: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaExpedicao) === true) {
                ExecutarPesquisaExpedicao();
            } else
                exibirMensagem(tipoMensagem.atencao, "Atenção!", "Verifique os campos obrigatórios!");
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function loadExpedicao() {
    _pesquisaExpedicao = new PesquisaExpedicao();
    KoBindings(_pesquisaExpedicao, "knockoutPesquisaExpedicao", _pesquisaExpedicao.Pesquisar.id);

    new BuscarTransportadores(_pesquisaExpedicao.Transportador, null, null, true, null);
    new BuscarMotoristas(_pesquisaExpedicao.Motorista, null, _pesquisaExpedicao.Transportador);
    new BuscarVeiculos(_pesquisaExpedicao.Veiculo, null, _pesquisaExpedicao.Transportador);
    new BuscarCentrosCarregamento(_pesquisaExpedicao.CentroCarregamento, RetornoConsultaCentroCarregamento, null, null, true);

    buscarCargasExpedicao();
    BuscarCentroCarregamentoPadrao();
    loadEnvioDetalhesCargaEmail();
    loadDetalhePedido();
}


//*******MÉTODOS*******

function buscarCargasExpedicao() {

    var detalhes = { descricao: "Detalhes da Carga", id: guid(), evento: "onclick", metodo: detalhesCargaClick, icone: "" };
    var confirmarCarregamento = { descricao: "Carregamento Finalizado", id: guid(), evento: "onclick", metodo: finalizarCarregamentoClick, icone: "", visibilidade: VisibilidadeFinalizarCarregamento };
    var downloadDetalhesCarga = { descricao: "Baixar Detalhes da Carga", id: guid(), evento: "onclick", metodo: downloadDetalhesCargaClick, icone: "" };
    var enviarDetalhesCargaEmail = { descricao: "Enviar Detalhes da Carga por E-mail", id: guid(), evento: "onclick", metodo: abrirTelaEnvioDetalhesCargaEmailClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 7, opcoes: [detalhes, downloadDetalhesCarga, enviarDetalhesCargaEmail, confirmarCarregamento] };
    _gridExpedicao = new GridView(_pesquisaExpedicao.Pesquisar.idGrid, "Expedicao/Pesquisa", _pesquisaExpedicao, menuOpcoes, { column: 1, dir: orderDir.asc }, 25);
    _gridExpedicao.CarregarGrid();
}

function finalizarCarregamentoClick(row) {
    exibirConfirmacao("Confirmação", "Você realmente deseja confirmar o carregamento desta carga?", function () {
        var data = { codigo: row.Codigo };
        executarReST("Expedicao/CarregamentoFinalizado", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmação de carregamento realizada com sucesso!");
                    ExecutarPesquisaExpedicao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function BuscarCentroCarregamentoPadrao() {
    executarReST("DadosPadrao/ObterCentroCarregamento", {}, function (r) {
        if (r.Success && r.Data) {
            RetornoConsultaCentroCarregamento(r.Data);
            ExecutarPesquisaExpedicao();
        }
    });
}

function RetornoConsultaCentroCarregamento(dados) {
    _pesquisaExpedicao.CentroCarregamento.val(dados.Descricao);
    _pesquisaExpedicao.CentroCarregamento.codEntity(dados.Codigo);
    
    executarReST("Expedicao/ObterEmailsCentroCarregamento", { CentroCarregamento: dados.Codigo }, function (r) {
        if (r.Success && r.Data) {
            _emailsEnvioDetalhesCarga = r.Data || new Array();
            recarregarGridDetalhesCargaEmail();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function VisibilidadeFinalizarCarregamento(e) {
    if (e.CarregamentoFinalizado == "Sim") {
        return false;
    } else {
        return true;
    }
}

function ExecutarPesquisaExpedicao() {
    _gridExpedicao.CarregarGrid();
    _pesquisaExpedicao.Pesquisar.visibleFade(true);
}

function downloadDetalhesCargaClick(e) {
    executarDownload("Expedicao/DownloadDetalhesCarga", { Codigo: e.Codigo });
}

function detalhesCargaClick(row) {
    exibirDetalhesPedidos(row.CodigoCarga, function (knoutDetalhe) {
        HeaderAuditoria("CargaJanelaCarregamentoGuarita", knoutDetalhe);

        knoutDetalhe.ValorFrete.visible(false);
    });
}