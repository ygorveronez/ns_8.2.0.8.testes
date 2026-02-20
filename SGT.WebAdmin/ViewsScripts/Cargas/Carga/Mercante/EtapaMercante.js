
var _HTMLDadosMercante = "";
var _etapaMercante;
var _gridDadosMercante;

var EtapaMercante = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroControle = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroControleCte.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.string });
    this.NumeroCE = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CEMercante.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.string });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDadosMercante.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
}

function buscarDadosMercanteClick(e) {
    _cargaAtual = e;

    let strknoutEtapaMercante = "knoutEtapaMercante" + e.EtapaMercante.idGrid;
    let html = _HTMLDadosMercante.replace("#knoutEtapaMercante", strknoutEtapaMercante);

    $("#" + e.EtapaMercante.idGrid).html(html);

    _etapaMercante = new EtapaMercante();
    KoBindings(_etapaMercante, strknoutEtapaMercante);

    _etapaMercante.Carga.val(_cargaAtual.Codigo.val());

    montarGridDadosMercante();
}

function montarGridDadosMercante() {
    var editarColuna = { permite: true, callback: callbackEditarColunaDadosMercante, atualizarRow: true };
    var configExportacao = {
        url: "AlteracaoArquivoMercante/ExportarPesquisa",
        titulo: "Conferência Arquivo Mercante"
    };

    _gridDadosMercante = new GridViewExportacao(_etapaMercante.Pesquisar.idGrid, "AlteracaoArquivoMercante/Pesquisa", _etapaMercante, null, configExportacao, null, 10, null, null, editarColuna);
    _gridDadosMercante.CarregarGrid();
}

function callbackEditarColunaDadosMercante(cte, row, head, callbackTabPress) {
    var dados = {
        Codigo: cte.Codigo,
        NumeroManifesto: cte.NumeroManifesto,
        NumeroCE: cte.NumeroCE,
        NumeroManifestoTransbordo: cte.NumeroManifestoTransbordo
    };
    executarReST("AlteracaoArquivoMercante/AtualizarConhecimentosConferencia", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro alterado com sucesso.");

                if (arg.Data.TodosCTesComManifesto || arg.Data.TodosCTesComManifesto)
                    EtapaMercanteAprovada(_cargaAtual);
                else
                    EtapaMercanteLiberada(_cargaAtual);

                CompararEAtualizarGridEditableDataRow(cte, arg.Data);
                setTimeout(function () {
                    _gridDadosMercante.AtualizarDataRow(row, cte);
                }, 500);

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 99999999);
            }
        } else {
            if (arg.Data !== false) {

                if (arg.Data.TodosCTesComManifesto || arg.Data.TodosCTesComManifesto)
                    EtapaMercanteAprovada(_cargaAtual);
                else
                    EtapaMercanteLiberada(_cargaAtual);

                CompararEAtualizarGridEditableDataRow(cte, arg.Data);
                setTimeout(function () {
                    _gridDadosMercante.AtualizarDataRow(row, cte);
                }, 500);
                
            }
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 99999999);
        }
    });
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridDadosMercante.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem, 99999999);
}

function EtapaMercanteDesabilitada(e) {
    $("#" + e.EtapaMercante.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaMercante.idTab + " .step").attr("class", "step");
    e.EtapaMercante.eventClick = function (e) { buscarDadosMercanteClick(e) };
}

function EtapaMercanteLiberada(e) {
    $("#" + e.EtapaMercante.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMercante.idTab + " .step").attr("class", "step yellow");
    e.EtapaMercante.eventClick = function (e, sender) { buscarDadosMercanteClick(e) };

}

function EtapaMercanteProblema(e) {
    $("#" + e.EtapaMercante.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMercante.idTab + " .step").attr("class", "step red");
    e.EtapaMercante.eventClick = function (e, sender) { buscarDadosMercanteClick(e) };

}

function EtapaMercanteAguardando(e) {
    $("#" + e.EtapaMercante.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMercante.idTab + " .step").attr("class", "step yellow");
    e.EtapaMercante.eventClick = function (e, sender) { buscarDadosMercanteClick(e) };

}

function EtapaMercanteAprovada(e) {
    $("#" + e.EtapaMercante.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMercante.idTab + " .step").attr("class", "step green");
    e.EtapaMercante.eventClick = function (e, sender) { buscarDadosMercanteClick(e) };

}