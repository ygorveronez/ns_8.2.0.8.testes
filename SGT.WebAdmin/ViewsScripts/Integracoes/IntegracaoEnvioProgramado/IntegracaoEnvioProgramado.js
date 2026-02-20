/// <reference path="../../../ViewsScripts/Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoEntidadeIntegracao.js" />

//#region Variaveis Globais
var _gridIntegracaoEnvioProgramado;
var _pesquisaIntegracaoEnvioProgramado;
var _pesquisaHistoricoIntegracaoEnvioProgramado;
var _gridHistoricoIntegracaoEnvioProgramado;
//#endregion

//#region Funções Constructoras
function PesquisaIntegracaoEnvioProgramado() {

    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), text: "Número Carga:", visible: ko.observable(false) });
    this.NumeroCTE = PropertyEntity({ val: ko.observable(""), text: "Número CTE:", getType: typesKnockout.int, visible: ko.observable(false) });
    this.NumeroOcorrencia = PropertyEntity({ val: ko.observable(""), text: "Número Ocorrência:", visible: ko.observable(false), getType: typesKnockout.int });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação Integração:", val: ko.observable(EnumSituacaoIntegracao.AgIntegracao), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(false), def: EnumSituacaoIntegracao.AgIntegracao });
    this.TipoEntidadeIntegracao = PropertyEntity({ text: "Tipo Origem Integração:", options: EnumTipoEntidadeIntegracao.obterOpcoesPesquisa(), val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.DataIntegracaoInicial = PropertyEntity({ text: ko.observable("Data Integração Inicial: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoFinal = PropertyEntity({ text: ko.observable("Data Integração Final: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoInicial.dateRangeLimit = this.DataIntegracaoFinal;
    this.DataIntegracaoFinal.dateRangeInit = this.DataIntegracaoInicial;

    this.DataProgramadaInicial = PropertyEntity({ text: ko.observable("Data Prevista Inicial: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataProgramadaFinal = PropertyEntity({ text: ko.observable("Data Prevista Final: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataProgramadaInicial.dateRangeLimit = this.DataProgramadaFinal;
    this.DataProgramadaFinal.dateRangeInit = this.DataProgramadaInicial;

    this.DataCriacaoCargaInicial = PropertyEntity({ text: ko.observable("Data Criação Carga Inicial: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataCriacaoCargaFinal = PropertyEntity({ text: ko.observable("Data Criação Carga Final: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataCriacaoCargaInicial.dateRangeLimit = this.DataCriacaoCargaFinal;
    this.DataCriacaoCargaFinal.dateRangeInit = this.DataCriacaoCargaInicial;

    this.DataEmissaoCTEInicial = PropertyEntity({ text: ko.observable("Data Emissão CTE Inicial: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataEmissaoCTEFinal = PropertyEntity({ text: ko.observable("Data Emissão CTE Final: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataEmissaoCTEInicial.dateRangeLimit = this.DataEmissaoCTEFinal;
    this.DataEmissaoCTEFinal.dateRangeInit = this.DataEmissaoCTEInicial;

    this.ReenviarTodasIntegracoesComFalha = PropertyEntity({ text: ko.observable("Reenviar Integrações"), eventClick: ReenviarTodasIntegracoesComFalhaClick, type: types.event, visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (this.SituacaoIntegracao.val() === EnumSituacaoIntegracao.ProblemaIntegracao)
                this.ReenviarTodasIntegracoesComFalha.visible(true);
            else
                this.ReenviarTodasIntegracoesComFalha.visible(false);

            _gridIntegracaoEnvioProgramado.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            let visible = e.ExibirFiltros.visibleFade() == true;
            e.ExibirFiltros.visibleFade(!visible);
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function LoadIntegracaoEnvioProgramado() {
    _pesquisaIntegracaoEnvioProgramado = new PesquisaIntegracaoEnvioProgramado();
    KoBindings(_pesquisaIntegracaoEnvioProgramado, "knockoutIntegracaoEnvioProgramado");

    BuscarIntegracoes();
    ControlarVisualizacaoCampos();
}

//#endregion

//#region Funções Auxiliares

function BuscarIntegracoes() {
    const limiteRegistros = 20;
    const totalRegistrosPorPagina = 20;

    const auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("IntegracaoEnvioProgramado"), tamanho: "15", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    const reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: ReenviarIntegracaoEnvioProgramado, tamanho: "15", icone: "", visibilidade: VisibilidadeOpcaoReenviar };
    const antecipar = { descricao: "Antecipar Envio", id: guid(), evento: "onclick", metodo: AnteciparIntegracaoEnvioProgramado, tamanho: "15", icone: "", visibilidade: VisibilidadeOpcaoAnteciparEnvio };
    const baixar = { descricao: "Histórico de integrações", id: guid(), evento: "onclick", metodo: ExibirHistoricoIntegracaoEnvioProgramado, tamanho: "15", icone: "" };

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(auditar);
    menuOpcoes.opcoes.push(reenviar);
    menuOpcoes.opcoes.push(antecipar);
    menuOpcoes.opcoes.push(baixar);

    _gridIntegracaoEnvioProgramado = new GridView('grid-integracao-envio-programado', "IntegracaoEnvioProgramado/Pesquisar", _pesquisaIntegracaoEnvioProgramado, menuOpcoes, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, undefined, callbackColumnDefaultIntegracaoEnvioProgramado);
    _gridIntegracaoEnvioProgramado.SetPermitirEdicaoColunas(true);
    _gridIntegracaoEnvioProgramado.SetSalvarPreferenciasGrid(true);
    _gridIntegracaoEnvioProgramado.CarregarGrid();
}

function VisibilidadeOpcaoReenviar(data) {
    return data.SituacaoIntegracao == "Falha ao Integrar";
}

function VisibilidadeOpcaoAnteciparEnvio(data) {
    return data.SituacaoIntegracao == "Aguardando Integração";
}
function callbackColumnDefaultIntegracaoEnvioProgramado(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "TempoParaEnvio") {
        if (!dadosLinha.EnvioAntecipado && dadosLinha.DataEnvioProgramada && dadosLinha.NumeroEnvios <= 0) {
            setTimeout(function () {
                $('#' + cabecalho.name + '-' + dadosLinha.DT_RowId)
                    .countdown(moment(dadosLinha.DataEnvioProgramada, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                    .on('update.countdown', function (event) {
                        if (event.offset.totalDays > 0)
                            $(this).text(event.strftime('%-Dd %H:%M:%S'));
                        else
                            $(this).text(event.strftime('%H:%M:%S'));
                    });
            }, 1000);
        }

        return '<span id="' + cabecalho.name + '-' + dadosLinha.DT_RowId + '"></span>';
    }
}

function ReenviarIntegracaoEnvioProgramado(data) {
    executarReST("IntegracaoEnvioProgramado/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoEnvioProgramado.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function AnteciparIntegracaoEnvioProgramado(data) {
    executarReST("IntegracaoEnvioProgramado/AnteciparEnvio", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Envio realizado com sucesso.");
            _gridIntegracaoEnvioProgramado.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}


function ReenviarTodasIntegracoesComFalhaClick() {
    var data = RetornarObjetoPesquisa(_pesquisaIntegracaoEnvioProgramado);
    executarReST("IntegracaoEnvioProgramado/ReenviarTodas", data, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoEnvioProgramado.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ControlarVisualizacaoCampos() {
    _pesquisaIntegracaoEnvioProgramado.TipoEntidadeIntegracao.visible(_tiposOrigemIntegracao.length > 0);

    let possuiOrigemCarga = _tiposOrigemIntegracao.find(tipo => tipo == EnumTipoEntidadeIntegracao.Carga || tipo == EnumTipoEntidadeIntegracao.CTe) != null;
    let possuiOrigemCargaCTe = _tiposOrigemIntegracao.find(tipo => tipo == EnumTipoEntidadeIntegracao.CTe) != null;
    let possuiOrigemOcorrencia = _tiposOrigemIntegracao.find(tipo => tipo == EnumTipoEntidadeIntegracao.CargaOcorrencia) != null;

    if (possuiOrigemCarga || possuiOrigemCargaCTe)
        ControlarVisualizacaoCamposCarga(true);

    ControlarVisualizacaoCamposOcorrencia(possuiOrigemOcorrencia);
}

function ControlarVisualizacaoCamposCarga(possuiOrigemCarga) {
    _pesquisaIntegracaoEnvioProgramado.NumeroCarga.visible(possuiOrigemCarga);
    _pesquisaIntegracaoEnvioProgramado.NumeroCTE.visible(possuiOrigemCarga);
    _pesquisaIntegracaoEnvioProgramado.DataCriacaoCargaInicial.visible(possuiOrigemCarga);
    _pesquisaIntegracaoEnvioProgramado.DataCriacaoCargaFinal.visible(possuiOrigemCarga);
    _pesquisaIntegracaoEnvioProgramado.DataEmissaoCTEInicial.visible(possuiOrigemCarga);
    _pesquisaIntegracaoEnvioProgramado.DataEmissaoCTEFinal.visible(possuiOrigemCarga);
}

function ControlarVisualizacaoCamposOcorrencia(possuiOrigemOcorrencia) {
    _pesquisaIntegracaoEnvioProgramado.NumeroOcorrencia.visible(possuiOrigemOcorrencia);
}

function DownloadArquivosHistoricoIntegracaoEnvioProgramado(data) {
    executarDownload("IntegracaoEnvioProgramado/DownloadArquivosHistoricoIntegracao", { Codigo: data.Codigo });
}
var PesquisaHistoricoIntegracaoEnvioProgramado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function BuscarHistoricoIntegracaoEnvioProgramado(integracao) {
    _pesquisaHistoricoIntegracaoEnvioProgramado = new PesquisaHistoricoIntegracaoEnvioProgramado();
    _pesquisaHistoricoIntegracaoEnvioProgramado.Codigo.val(integracao.Codigo);

    let download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoEnvioProgramado, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoEnvioProgramado = new GridView("tblHistoricoIntegracaoEnvioProgramado", "IntegracaoEnvioProgramado/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoEnvioProgramado, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoEnvioProgramado.CarregarGrid();
}

function ExibirHistoricoIntegracaoEnvioProgramado(integracao) {
    BuscarHistoricoIntegracaoEnvioProgramado(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoEnvioProgramado");
}

//#endregion