/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="LiberacaoSemIntegracaoGR.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _liberacaoSemIntegracaoGRAnexo;
var _gridLiberacaoSemIntegracaoGRAnexo;

/*
 * Declaração das Classes
 */

var LiberacaoSemIntegracaoGRAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Arquivo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _liberacaoSemIntegracaoGRAnexo.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarLiberacaoSemIntegracaoGRAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadLiberacaoSemIntegracaoGRAnexo() {
    _liberacaoSemIntegracaoGRAnexo = new LiberacaoSemIntegracaoGRAnexo();
    KoBindings(_liberacaoSemIntegracaoGRAnexo, "knoutLiberacaoSemIntegracaoGRAnexo");

    loadGridLiberacaoSemIntegracaoGRAnexo();
}

function loadGridLiberacaoSemIntegracaoGRAnexo() {
    var linhasPorPaginas = 5;

    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoLiberacaoSemIntegracaoGRAnexoClick, icone: "", visibilidade: visibleDownloadLiberacaoSemIntegracaoGRAnexo };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: excluirLiberacaoSemIntegracaoGRAnexoClick, icone: "", visibilidade: visibleRemoverLiberacaoSemIntegracaoGRAnexo };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridLiberacaoSemIntegracaoGRAnexo = new BasicDataTable(_liberacaoSemIntegracaoGR.ListaAnexo.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, linhasPorPaginas);
    _gridLiberacaoSemIntegracaoGRAnexo.CarregarGrid([]);
}

function visibleDownloadLiberacaoSemIntegracaoGRAnexo(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverLiberacaoSemIntegracaoGRAnexo(dataRow) {
    return isNaN(dataRow.Codigo) && _liberacaoSemIntegracaoGR.KnoutCarga.ko.SituacaoCarga.val() == EnumSituacoesCarga.Nova;
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarLiberacaoSemIntegracaoGRAnexoClick() {
    var anexo = obterLiberacaoSemIntegracaoGRAnexoSalvar();

    if (!anexo)
        return;

    var listaLiberacaoSemIntegracaoGRAnexo = obterListaLiberacaoSemIntegracaoGRAnexo();

    listaLiberacaoSemIntegracaoGRAnexo.push(anexo);

    _liberacaoSemIntegracaoGR.ListaAnexo.val(listaLiberacaoSemIntegracaoGRAnexo);

    fecharModalLiberacaoSemIntegracaoGRAnexo();
}

function adicionarLiberacaoSemIntegracaoGRAnexoModalClick() {
    exibirModalLiberacaoSemIntegracaoGRAnexo();
}

function excluirLiberacaoSemIntegracaoGRAnexoClick(registroSelecionado) {
    var listaLiberacaoSemIntegracaoGRAnexo = obterListaLiberacaoSemIntegracaoGRAnexo();

    for (var i = 0; i < listaLiberacaoSemIntegracaoGRAnexo.length; i++) {
        if (registroSelecionado.Codigo == listaLiberacaoSemIntegracaoGRAnexo[i].Codigo) {
            listaLiberacaoSemIntegracaoGRAnexo.splice(i, 1);
            break;
        }
    }

    _liberacaoSemIntegracaoGR.ListaAnexo.val(listaLiberacaoSemIntegracaoGRAnexo);
}

function downloadAnexoLiberacaoSemIntegracaoGRAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("CargaLiberacaoSemIntegracaoGRAnexo/DownloadAnexo", data);
}

/*
 * Declaração das Funções Públicas
 */

function enviarArquivosAnexadosLiberacaoSemIntegracaoGRAnexo(codigo) {
    var listaLiberacaoSemIntegracaoGRAnexo = obterListaLiberacaoSemIntegracaoGRAnexo();

    if (listaLiberacaoSemIntegracaoGRAnexo.length == 0)
        return;

    executarReST("CargaLiberacaoSemIntegracaoGRAnexo/ExcluirTodosAnexos", { Codigo: codigo }, function () {
        if (listaLiberacaoSemIntegracaoGRAnexo.length > 0) {
            var formData = obterFormDataLiberacaoSemIntegracaoGRAnexo(listaLiberacaoSemIntegracaoGRAnexo);

            enviarArquivo("CargaLiberacaoSemIntegracaoGRAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, (retorno.Data.Anexos.length > 1) ? Localization.Resources.Cargas.Carga.ArquivosAnexadosComSucesso : Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                    else
                        exibirMensagem(tipoMensagem.aviso, (listaLiberacaoSemIntegracaoGRAnexo.length > 1) ? Localization.Resources.Cargas.Carga.NaoFoiPossivelAnexarOsArquivos : Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        }
    });
}

function obterListaLiberacaoSemIntegracaoGRAnexo() {
    return _liberacaoSemIntegracaoGR.ListaAnexo.val().slice();
}

function recarregarGridLiberacaoSemIntegracaoGRAnexo() {
    var listaLiberacaoSemIntegracaoGRAnexo = obterListaLiberacaoSemIntegracaoGRAnexo();

    _gridLiberacaoSemIntegracaoGRAnexo.CarregarGrid(listaLiberacaoSemIntegracaoGRAnexo);
}

function carregarAnexosLiberacaoSemIntegracaoGR() {
    executarReST("CargaLiberacaoSemIntegracaoGRAnexo/ObterAnexo", { Codigo: _liberacaoSemIntegracaoGR.KnoutCarga.ko.Codigo.val() }, function (retorno) {
        if (retorno.Success)
            _liberacaoSemIntegracaoGR.ListaAnexo.val(retorno.Data.Anexos);
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarAnexos);
    });
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalLiberacaoSemIntegracaoGRAnexo() {
    Global.abrirModal("divModalLiberacaoSemIntegracaoGRAnexo");
    $("#divModalLiberacaoSemIntegracaoGRAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_liberacaoSemIntegracaoGRAnexo);
        _liberacaoSemIntegracaoGRAnexo.Arquivo.val("");
    });
}

function fecharModalLiberacaoSemIntegracaoGRAnexo() {
    Global.fecharModal("divModalLiberacaoSemIntegracaoGRAnexo");
}

function obterLiberacaoSemIntegracaoGRAnexoSalvar() {
    var arquivo = document.getElementById(_liberacaoSemIntegracaoGRAnexo.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);
        return undefined;
    }

    return {
        Codigo: guid(),
        Descricao: _liberacaoSemIntegracaoGRAnexo.Descricao.val(),
        NomeArquivo: _liberacaoSemIntegracaoGRAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };
}

function obterFormDataLiberacaoSemIntegracaoGRAnexo(anexos) {
    var formData = new FormData();

    anexos.forEach(function (anexo) {
        formData.append("Arquivo", anexo.Arquivo);
        formData.append("Descricao", anexo.Descricao);
    });

    return formData;
}
