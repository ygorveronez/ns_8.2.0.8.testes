
var _HTMLDadosMDFeAquaviario = "", _etapaMDFeAquaviario, _gridDadosMDFeAquaviario;

var EtapaMDFeAquaviario = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDadosMDFeAquaviario.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
};

function buscarDadosMDFeAquaviarioClick(e) {
    _cargaAtual = e;
    $("#" + _cargaAtual.EtapaMDFeAquaviario.idGrid + " .DivMensagemMDF").hide();
    $("#" + _cargaAtual.EtapaMDFeAquaviario.idGrid + " .gridMDFeAquaviario").show();

    let strknoutEtapaMDFeAquaviario = "knoutEtapaMDFeAquaviario" + e.EtapaMDFeAquaviario.idGrid;
    let html = _HTMLDadosMDFeAquaviario.replace("#knoutEtapaMDFeAquaviario", strknoutEtapaMDFeAquaviario);

    $("#" + e.EtapaMDFeAquaviario.idGrid).html(html);

    _etapaMDFeAquaviario = new EtapaMDFeAquaviario();
    KoBindings(_etapaMDFeAquaviario, strknoutEtapaMDFeAquaviario);

    _etapaMDFeAquaviario.Carga.val(_cargaAtual.Codigo.val());

    montarGridDadosMDFeAquaviario();

    if (_cargaAtual.PossuiMDFeAquaviarioGeradoMasNaoVinculado.val()) {
        $("#" + _cargaAtual.EtapaMDFeAquaviario.idGrid + " .MensageMDF").html("Não vinculado ao MDF-e");
        $("#" + _cargaAtual.EtapaMDFeAquaviario.idGrid + " .DivMensagemMDF").show();
        $("#" + _cargaAtual.EtapaMDFeAquaviario.idGrid + " .gridMDFeAquaviario").hide();
    }
}

function montarGridDadosMDFeAquaviario() {
    var cancelar = { descricao: Localization.Resources.Gerais.Geral.Cancelar, id: guid(), metodo: cancelarMDFeAquaviarioClick, icone: "" };
    var baixarDAMDFE = { descricao: "Baixar o DAMDFE (PDF)", id: guid(), metodo: BaixarDAMDFEClick, icone: "" };
    var baixarXML = { descricao: "Baixar o XML", id: guid(), metodo: BaixarXMLMDFeClick, icone: "" };
    var mensagemSEFAZ = { descricao: "Mensagem Sefaz", id: guid(), metodo: retornoSefazClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [baixarDAMDFE, baixarXML, cancelar, mensagemSEFAZ] };

    _gridDadosMDFeAquaviario = new GridView(_etapaMDFeAquaviario.Pesquisar.idGrid, "CargaMDFeAquaviarioManual/Pesquisa", _etapaMDFeAquaviario, menuOpcoes);
    _gridDadosMDFeAquaviario.CarregarGrid();
}

function cancelarMDFeAquaviarioClick(mdfeAquaviario) {
    CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA.codEntity = mdfeAquaviario.CodigoMDFe;

    if (CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA.codEntity > 0)
        executarReST("CargaMDFe/ValidarMDFeAquaviarioTelaCarga", { CodigoMDFeAquaviarioParaCancelamento: CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA.codEntity }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (r.Data.CargaMDFeManualMDFe.Codigo > 0) {
                        CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA.codEntity = r.Data.CargaMDFeManualMDFe.Codigo;
                        CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA.val(r.Data.CargaMDFeManualMDFe.Descricao);

                        location.pathname = "";
                        location.href = "/#Cargas/CargaMDFeManualCancelamento";
                    }
                } else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);

            } else
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);

        });
}

function BaixarDAMDFEClick(e) {
    var data = { Numero: e.CodigoMDFe };
    executarDownload("CargaMDFeAquaviarioManual/DownloadDAMDFE", data);
}

function BaixarXMLMDFeClick(e) {
    var data = { Numero: e.CodigoMDFe };
    executarDownload("CargaMDFeAquaviarioManual/DownloadXML", data);
}

function retornoSefazClick(e) {
    executarReST("CargaMDFeAquaviarioManual/BuscarMensagemSefaz", { Numero: e.CodigoMDFe }, function (r) {
        if (r.Success) {
            if (r.Data) {
                $('#PMensagemRetornoSefaz').html(r.Data);
                Global.abrirModal('divModalRetornoSefaz');
            } else
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);

        } else
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);

    });
}

//*******ETAPA*******

function EtapaMDFeAquaviarioDesabilitada(e) {
    $("#" + e.EtapaMDFeAquaviario.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaMDFeAquaviario.idTab + " .step").attr("class", "step");
    e.EtapaMDFeAquaviario.eventClick = function (e) { buscarDadosMDFeAquaviarioClick(e) };
}

function EtapaMDFeAquaviarioLiberada(e) {
    $("#" + e.EtapaMDFeAquaviario.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeAquaviario.idTab + " .step").attr("class", "step yellow");
    e.EtapaMDFeAquaviario.eventClick = function (e) { buscarDadosMDFeAquaviarioClick(e) };
}

function EtapaMDFeAquaviarioProblema(e) {
    $("#" + e.EtapaMDFeAquaviario.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeAquaviario.idTab + " .step").attr("class", "step red");
    e.EtapaMDFeAquaviario.eventClick = function (e) { buscarDadosMDFeAquaviarioClick(e) };
}

function EtapaMDFeAquaviarioAguardando(e) {
    $("#" + e.EtapaMDFeAquaviario.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeAquaviario.idTab + " .step").attr("class", "step yellow");
    e.EtapaMDFeAquaviario.eventClick = function (e) { buscarDadosMDFeAquaviarioClick(e) };
}

function EtapaMDFeAquaviarioNaoVinculado(e) {
    $("#" + e.EtapaMDFeAquaviario.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeAquaviario.idTab + " .step").attr("class", "step orange");
    e.EtapaMDFeAquaviario.eventClick = function (e) { buscarDadosMDFeAquaviarioClick(e) };
}

function EtapaMDFeAquaviarioAprovada(e) {
    $("#" + e.EtapaMDFeAquaviario.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeAquaviario.idTab + " .step").attr("class", "step green");
    e.EtapaMDFeAquaviario.eventClick = function (e) { buscarDadosMDFeAquaviarioClick(e) };
}