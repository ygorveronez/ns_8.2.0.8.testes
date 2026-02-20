//#region Variaveis Globais
var _gridAgrupamentoNotas;
var _agrupamentoNotasPrechekin;
var _stageContainerAgrupamentos;
var _stageContainerAgrupamentosEntrega;
var _pesquisaAgrupamentosStageContainer;
var _htmlNotasCargaPreChekin;
var _idDivAnterior;
//#endregion

//#region Funções Contructoras

function StageNotaAgrupamento(agrupamento) {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.NumeroStages = PropertyEntity({ val: ko.observable("") });
    this.DataPreChekin = PropertyEntity({ val: ko.observable("") });
    this.Placa = PropertyEntity({ val: ko.observable("") });
    this.NroFRS = PropertyEntity({ val: ko.observable("") });
    //this.ValorFRS = PropertyEntity({ val: ko.observable(0) });
    this.ValorFrete = PropertyEntity({ val: ko.observable(0) });
    this.Perfil = PropertyEntity({ val: ko.observable("") });
    this.OrigemxDestino = PropertyEntity({ val: ko.observable("") });
    this.CanalVenda = PropertyEntity({ val: ko.observable("") });
    this.ListaNotas = PropertyEntity({ val: ko.observable(new Array()) });

    this.GridNotasAgrupamento;

    PreencherObjetoKnout(this, { Data: agrupamento });
}

function StageContainerAgrupamentos() {
    this.StageAgrupada = ko.observableArray(new Array());
}

function StageContainerAgrupamentosEntrega() {
    this.StageAgrupadaEntrega = ko.observableArray(new Array());
}

function PesquisaAgrupamentosStageContainer() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

function loadCargaNotasPreckin() {
    $.get("Content/Static/Carga/NotasEtapaPrechekin.html?dyn=" + guid(), function (data) {
        _htmlNotasCargaPreChekin = data;
    });
}

function loadAgrupamentoNotasPrechekin(id, idLiTab, idTab) {
    let possuiConsolidacao = _cargaAtual != null ? _cargaAtual.TipoOperacao.PossuiConsolidacao : false;
    if (!possuiConsolidacao) {
        //$("#liTabEtapaNFeStage_" + _cargaAtual.EtapaInicioTMS.idGrid).hide();
        $(idLiTab).hide();
        return;
    }
    let stringDiv = '#IDEtapaInicio';

    if (_idDivAnterior != null && _idDivAnterior != "")
        stringDiv = `${_idDivAnterior}`;

    _htmlNotasCargaPreChekin = _htmlNotasCargaPreChekin.replaceAll(stringDiv, id);
    $(idTab).html(_htmlNotasCargaPreChekin);

    _stageContainerAgrupamentos = new StageContainerAgrupamentos();
    KoBindings(_stageContainerAgrupamentos, "knockoutStageAgrupamentoContainer_" + id);

    _stageContainerAgrupamentosEntrega = new StageContainerAgrupamentosEntrega();
    KoBindings(_stageContainerAgrupamentosEntrega, "knockoutStageAgrupamentoContainerEntrega_" + id);

    _pesquisaAgrupamentosStageContainer = new PesquisaAgrupamentosStageContainer();
    BuscarAgrupamentosNotas();

    _idDivAnterior = id;
}


//#endregion

//#region Funções Auxiliares

function BuscarAgrupamentosNotas(e) {
    if (_cargaAtual == null && e == null)
        return;
    let codigoCarga = 0;
    if (!e && _cargaAtual.Codigo.val() > 0)
        codigoCarga = _cargaAtual.Codigo.val();
    else if (e != null && e.Codigo.val() > 0)
        codigoCarga = e.Codigo.val();

    executarReST("CargaNotasFiscais/BuscarDadosNotasFiscaisPreChekin", { Carga: codigoCarga }, function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);

        if (_stageContainerAgrupamentos == null)
            return;
        else {
            _stageContainerAgrupamentos.StageAgrupada.removeAll();
            _stageContainerAgrupamentosEntrega.StageAgrupadaEntrega.removeAll();
        }
        let stageColeta = retorno.Data.StageColeta != null ? retorno.Data.StageColeta : [];
        let stageEntrega = retorno.Data.StageEntrega != null ? retorno.Data.StageEntrega : [];

        for (var i = 0; i < stageColeta.length; i++) {
            let stageAgrupada = stageColeta[i];
            let knoutstageAgrupada = new StageNotaAgrupamento(stageAgrupada);

            _stageContainerAgrupamentos.StageAgrupada.push(knoutstageAgrupada);

            knoutstageAgrupada.GridNotasAgrupamento = CriarGridNotasPreChekin(stageAgrupada.Codigo, "grid-notas-agrupamento-");
            knoutstageAgrupada.GridNotasAgrupamento.CarregarGrid(stageAgrupada.ListaNotas);
        }

        for (var i = 0; i < stageEntrega.length; i++) {
            let stageAgrupada = stageEntrega[i];
            let knoutstageAgrupada = new StageNotaAgrupamento(stageAgrupada);

            _stageContainerAgrupamentosEntrega.StageAgrupadaEntrega.push(knoutstageAgrupada);

            knoutstageAgrupada.GridNotasAgrupamento = CriarGridNotasPreChekin(stageAgrupada.Codigo, "grid-notas-agrupamento-entrega-");
            knoutstageAgrupada.GridNotasAgrupamento.CarregarGrid(stageAgrupada.ListaNotas);
        }
    });
}

function CriarGridNotasPreChekin(codigoAgrupamento, id) {
    var header = [
        { data: "CodigoNota", visible: false },
        { data: "ChaveNota", title: "Chave Nota", width: "20%", className: "text-align-center" },
        { data: "DataEmissao", title: "Data Emissão", width: "10%", className: "text-align-center" },
        { data: "Status", title: "Status", width: "10%", className: "text-align-center" }
    ];

    let opcaoDownloadPDF = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos + " PDF", id: guid(), evento: "onclick", metodo: DownloadPDFNotaPrechekin, tamanho: "20", icone: "" };
    let opcaoDownloadXML = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos + " XML", id: guid(), evento: "onclick", metodo: DownloadXmlNotaPrechekin, tamanho: "20", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoDownloadPDF, opcaoDownloadXML] };

    return new BasicDataTable(id + codigoAgrupamento, header, menuOpcoes);
}

function DownloadXmlNotaPrechekin(e) {
    executarDownload("CargaNotasFiscais/DownloadXML", { Codigo: e.CodigoNota });
}


function DownloadPDFNotaPrechekin(e) {
    executarDownload("CargaNotasFiscais/DownloadDANFE", { Codigo: e.CodigoNota });
}

//#endregion