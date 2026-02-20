/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoEnvioHUBOfertas.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaHUBIntegracao;
var _gridHUBIntegracao;
var _pesquisaHistoricoIntegracao;
var _dataGridCarregada = [];

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function RetornoConsultaCarga(data) {
    _pesquisaHUBIntegracao.Carga.codEntity(data.Codigo);
    _pesquisaHUBIntegracao.Carga.val(data.CodigoCargaEmbarcador);
}

function CargaBlur() {
    if (_pesquisaHUBIntegracao.Carga.val() == "")
        _pesquisaHUBIntegracao.Carga.codEntity(0);
}

var PesquisaHUB = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), def: EnumSituacaoIntegracaoCarga.Todas });
    this.TipoEnvioHUBOfertas = PropertyEntity({ text: "Tipo de Envio: ", val: ko.observable(EnumTipoEnvioHUBOfertas.Todas), options: EnumTipoEnvioHUBOfertas.obterOpcoesPesquisa(), def: EnumTipoEnvioHUBOfertas.Todas });
    this.AtualizouCadastro = PropertyEntity({ text: "Atualizou situação após última consulta? ", val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), options: Global.ObterOpcoesPesquisaBooleano("Simples para o Normal", "Normal para o Simples"), def: EnumSituacaoIntegracaoCarga.Todas });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Carga: ", issue: 629, idBtnSearch: guid(), eventChange: CargaBlur, enable: ko.observable(true) });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ReenviarIntegracoes = PropertyEntity({ text: "Reenviar integrações com Falha", type: types.event, eventClick: reenviarMultiplasIntegracoesClick, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadHUBIntegracao() {

    _pesquisaHUBIntegracao = new PesquisaHUB();
    KoBindings(_pesquisaHUBIntegracao, "knockoutPesquisaHUB", false, _pesquisaHUBIntegracao.Pesquisar.id);

    var codigoCarga = sessionStorage.getItem('CodigoCarga');
    var codigoCargaEmbarcador = sessionStorage.getItem('CodigoCargaEmbarcador');

    if (codigoCarga && codigoCarga > 0 && codigoCargaEmbarcador) {
        _pesquisaHUBIntegracao.Carga.val(codigoCargaEmbarcador);
        _pesquisaHUBIntegracao.Carga.codEntity(codigoCarga);

        sessionStorage.removeItem('CodigoCarga');
        sessionStorage.removeItem('CodigoCargaEmbarcador');
    }

    new BuscarTransportadores(_pesquisaHUBIntegracao.Transportador);
    new BuscarCargas(_pesquisaHUBIntegracao.Carga, RetornoConsultaCarga, null, null, null, [EnumSituacoesCarga.Cancelada, EnumSituacoesCarga.EmCancelamento, EnumSituacoesCarga.Anulada], null, null, null, null, true, null, null, null, null, null, true);

    BuscarIntegracoesHUB();
}

function reenviarMultiplasIntegracoesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reenviar todas as integrações que estão com falha de integração?", function () {
        executarReST("IntegracaoHUBOfertas/ReenviarMultiplasIntegracoes", null, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    _gridHUBIntegracao.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    });
}

//*******MÉTODOS*******

function BuscarIntegracoesHUB() {

    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("EmpresaIntegracao"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    var configExportacao = {
        url: "IntegracaoHUBOfertas/ExportarPesquisa",
        titulo: "Integrações HUB"
    };

    _gridHUBIntegracao = new GridViewExportacao(_pesquisaHUBIntegracao.Pesquisar.idGrid, "IntegracaoHUBOfertas/Pesquisa", _pesquisaHUBIntegracao, menuOpcoes, configExportacao, null, 10);
    RecarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("IntegracaoHUBOfertas/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridHUBIntegracao.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    $("#divModalHistoricoIntegracao").modal("show");
}

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "IntegracaoHUBOfertas/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("IntegracaoHUBOfertas/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function RecarregarGrid(cb) {
    _gridHUBIntegracao.CarregarGrid(function (data) {
        _dataGridCarregada = data.data;

        if (cb != null) cb();
    });
}