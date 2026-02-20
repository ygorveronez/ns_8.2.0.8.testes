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
/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaJanelaCarregamentoIntegracao;
var _gridJanelaCarregamentoIntegracao;
var _pesquisaHistoricoIntegracaoJanelaCarregamento;
var _gridHistoricoIntegracaoJanelaCarregamento;
//var _buscandoApenasNaoReconhecidos = true;
//var _atualizarAoFechar = false;
//var _dataGridCarregada = [];
//var _indexCarregamentoAberto = -1;

//var _pesquisaHistoricoIntegracao;



var PesquisaJanelaCarregamentoIntegracao = function () {
    this.NumeroCargaEmbarcador = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Logistica.JanelaCarregamentoIntegracao.NumeroDaCarga.getFieldDescription() });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroShipment = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoIntegracao.NumeroShipment.getFieldDescription(), val: ko.observable(""), def: "", visible: false });
    this.SituacaoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoIntegracao.SituacaoDaIntegracao.getFieldDescription(), val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa() });
    this.Filial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CentroCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoIntegracao.CentroDeCarregamento.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJanelaCarregamentoIntegracao.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var PesquisaHistoricoIntegracaoJanelaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function loadJanelaCarregamentoIntegracao() {
    _pesquisaJanelaCarregamentoIntegracao = new PesquisaJanelaCarregamentoIntegracao();
    KoBindings(_pesquisaJanelaCarregamentoIntegracao, "knockoutPesquisaJanelaCarregamentoIntegracao", false, _pesquisaJanelaCarregamentoIntegracao.Pesquisar.id);

    new BuscarFilial(_pesquisaJanelaCarregamentoIntegracao.Filial);
    new BuscarCentrosCarregamento(_pesquisaJanelaCarregamentoIntegracao.CentroCarregamento);

    CarregarGridIntegracaoJanelaCarregamento();
}

//*******MÉTODOS*******

function CarregarGridIntegracaoJanelaCarregamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Logistica.JanelaCarregamentoIntegracao.ReenviarRetorno, id: guid(), metodo: ReenviarIntegracaoJanelaCarregamento, visibilidade: VisibilidadeReenviarRetorno });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoJanelaCarregamento });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaJanelaCarregamentoIntegracao") });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Logistica.JanelaCarregamentoIntegracao.EnviarDocumentosReprovados, id: guid(), metodo: EnviarDocumentosReprovados, visibilidade: VisibilidadeEnvioDocumentosReprovados });

    var configExportacao = {
        url: "JanelaCarregamentoIntegracao/ExportarPesquisa",
        titulo: Localization.Resources.Logistica.JanelaCarregamentoIntegracao.IntegracaoJanelaCarregamento
    };

    _gridJanelaCarregamentoIntegracao = new GridViewExportacao(_pesquisaJanelaCarregamentoIntegracao.Pesquisar.idGrid, "JanelaCarregamentoIntegracao/Pesquisa", _pesquisaJanelaCarregamentoIntegracao, menuOpcoes, configExportacao, null, 10);
    _gridJanelaCarregamentoIntegracao.CarregarGrid();
}

function ReenviarIntegracaoJanelaCarregamento(data) {
    executarReST("JanelaCarregamentoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.ReenvioSolicitadoComSucesso);
                _gridJanelaCarregamentoIntegracao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function EnviarDocumentosReprovados(data) {
    executarReST("JanelaCarregamentoIntegracao/EnviarDocumentosReprovados", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.JanelaCarregamentoIntegracao.ReenvioSolicitadoComSucesso);
                _gridJanelaCarregamentoIntegracao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoJanelaCarregamento(data) {
    BuscarHistoricoIntegracaoJanelaCarregamento(data);
    Global.abrirModal("divModalHistoricoIntegracaoJanelaCarregamento");
}

function BuscarHistoricoIntegracaoJanelaCarregamento(data) {
    _pesquisaHistoricoIntegracaoJanelaCarregamento = new PesquisaHistoricoIntegracaoJanelaCarregamento();
    _pesquisaHistoricoIntegracaoJanelaCarregamento.Codigo.val(data.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoJanelaCarregamento, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoJanelaCarregamento = new GridView("tblHistoricoIntegracaoJanelaCarregamento", "JanelaCarregamentoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoJanelaCarregamento, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoJanelaCarregamento.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoJanelaCarregamento(historicoConsulta) {
    executarDownload("JanelaCarregamentoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function VisibilidadeReenviarRetorno(data) {
    if (data.TipoRetornoRecebimento == 0 && data.TipoEvento == 0)
        return (data.SituacaoIntegracao == EnumSituacaoIntegracao.ProblemaIntegracao || data.Mensagem == "Não recomendado");
    
    return data.TipoRetornoRecebimento == EnumTipoRetornoRecebimento.Retorno && data.TipoEvento == EnumTipoEventoIntegracaoJanelaCarregamento.RetornoLeilao && (data.SituacaoIntegracao == EnumSituacaoIntegracao.ProblemaIntegracao || data.SituacaoIntegracao == EnumSituacaoIntegracao.Integrado);
}

function VisibilidadeEnvioDocumentosReprovados(data) {
    if (!string.IsNullOrWhiteSpace(data.Mensagem))
        return data.Mensagem == "Documento Reprovado" || data.Mensagem == "Dados Enviados - Pendente de Anexo";

    return false;
}