/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="AcompanhamentoChecklist.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosChecklist;
var _gridAnexosChecklist;
var _checklistCargaAtual;

var AnexosChecklistAcompanhamento = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Arquivo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        const nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosChecklist.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RecarregarGridAnexosAcompanhamento();
    });
};

//*******EVENTOS*******

function loadAcompanhamentoAnexosChecklist(knoutChecklistCargaAtual) {
    _checklistCargaAtual = knoutChecklistCargaAtual;

    _anexosChecklist = new AnexosChecklistAcompanhamento();
    KoBindings(_anexosChecklist, "knockoutCadastroAnexosChecklist");

    const download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoChecklistAcompanhamentoClick, icone: "", visibilidade: visibleDownloadChecklistAcompanhamento };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    const linhasPorPaginas = 7;
    _gridAnexosChecklist = new BasicDataTable(_anexosChecklist.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosChecklist.CarregarGrid([]);

    ObterAnexosAcompanhamentoChecklist(_checklistCargaAtual.CodigoChecklist.val());
}

function visibleDownloadChecklistAcompanhamento(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function downloadAnexoChecklistAcompanhamentoClick(dataRow) {
    const data = { Codigo: dataRow.Codigo };
    executarDownload("CargaJanelaCarregamentoTransportadorChecklistAnexo/DownloadAnexo", data);
}

function ObterAnexosAcompanhamentoChecklist(codigo) {
    if (codigo === 0)
        return;

    executarReST("CargaJanelaCarregamentoTransportadorChecklistAnexo/ObterAnexo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexosChecklist.Anexos.val(retorno.Data.Anexos);
                RecarregarGridAnexosAcompanhamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function RecarregarGridAnexosAcompanhamento() {
    const anexos = _anexosChecklist.Anexos.val().slice();

    _checklistCargaAtual.Anexos.val(anexos);
    _gridAnexosChecklist.CarregarGrid(anexos);
}
