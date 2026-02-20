/// <reference path="../../../Enumeradores/EnumCodigoControleImportacao.js" />

var _pacotes;
var _gridPacotes;
var _htmlPacotes;
var _gridHistoricoOrdemCompra;

function Pacotes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Carga = PropertyEntity({ val: ko.observable(0) });

    this.GridPacotes = PropertyEntity({ idGrid: guid() });
    this.AdicionarPacote = PropertyEntity({ eventClick: AdicionarPacoteClick, type: types.event, text: "Adicionar Pacote", visible: ko.observable(true) });
    this.ConsultarPacote = PropertyEntity({ eventClick: function (e, sender) { consultarPacotesClick(e.Carga.val(), sender, false) }, type: types.event, text: "Consultar Pacote", visible: ko.observable(false) });
    this.HistoricoConsulta = PropertyEntity({ eventClick: HistoricoIntegracoesClick, type: types.event, text: "Histório de Consultas", visible: ko.observable(false) });
}

var PesquisaHistoricoConsultaPacotes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Historico = PropertyEntity({ idGrid: guid() });
};

function loadPacotes() {
    $.get("Content/Static/Carga/Pacotes.html?dyn=" + guid(), function (data) {
        _htmlPacotes = data;
    });
}

function carregarPacotes(id, idLiTab, idTab) {
    let possuiPacotes = _cargaAtual != null ? _cargaAtual.ExistePacote.val() : false;
    if (!possuiPacotes) {
        $(idLiTab).hide();
        return;
    }

    let stringDiv = "#IDEtapa";

    if (_idDivAnterior != null && _idDivAnterior != "")
        stringDiv = `${_idDivAnterior}`;

    _htmlPacotes = _htmlPacotes.replaceAll(stringDiv, id);
    $(idTab).html(_htmlPacotes);

    _pacotes = new Pacotes();
    KoBindings(_pacotes, "knockoutPacotes_" + id);

    _pesquisaHistoricoConsultaPacotes = new PesquisaHistoricoConsultaPacotes();
    KoBindings(_pesquisaHistoricoConsultaPacotes, "knockouHistorioConsultaPacote");

    _pacotes.Carga.val(_cargaAtual.Codigo.val());

    BuscarPacotes();

    _pacotes.ConsultarPacote.visible(_cargaAtual.PermiteConsultarPorPacotesLoggi.val());
    _pacotes.HistoricoConsulta.visible(_cargaAtual.PermiteConsultarPorPacotesLoggi.val());

    _idDivAnterior = id;
}

function BuscarPacotes() {
    _gridPacotes = new GridView(_pacotes.GridPacotes.idGrid, "CargaNotasFiscais/BuscarPacotes", _pacotes, null, null);
    _gridPacotes.CarregarGrid();
}

function AdicionarPacoteClick() {
    _gridPacotesAvulsos.CarregarGrid();
    Global.abrirModal("divModalPacotesAvulsos");
}

function HistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracaoConsultaPacotes(integracao);
    Global.abrirModal("divModalHistoricoConsultaPacote");
}

function BuscarHistoricoIntegracaoConsultaPacotes(integracao) {
    _pesquisaHistoricoConsultaPacotes.Codigo.val(integracao.Carga.val());
    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosConsultaPacotesClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoOrdemCompra = new GridView(_pesquisaHistoricoConsultaPacotes.Historico.idGrid, "CargaNotasFiscais/ConsultarHistoricoIntegracao", _pesquisaHistoricoConsultaPacotes, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoOrdemCompra.CarregarGrid();
}

function DownloadArquivosConsultaPacotesClick(historicoConsulta) {
    executarDownload("CargaNotasFiscais/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function consultarPacotesClick(codigoCarga, sender, somenteConsulta) {
    const dados = { CodigoCarga: codigoCarga };

    executarReST("CargaNotasFiscais/ObterConsultaPacotes", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                if (somenteConsulta)
                    return;

                _gridPacotes.CarregarGrid();
                if (!_gridCTesParaEmissao)
                    criarGridCTesParaEmissao(_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe);
                _gridCTesParaEmissao.CarregarGrid();

                preecherDocumentosEmissaoAvancoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}