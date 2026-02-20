/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexo;
var _gridAnexoTipoOperacao;
var _anexo;
var _anexoTipoOperacao;

/*
 * Declaração das Classes
 */

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.AnexoTipoOperacao = PropertyEntity({ eventClick: anexoTipoOperacaoClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.VisualizarAnexosTipoOperacao.getFieldDescription(), visible: ko.observable(true) });
}

var AnexoTipoOperacao = function () {
    this.AnexosTipoOperacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.AnexosTipoOperacao.val.subscribe(recarregarGridAnexoTipoOperacao)
}


/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    _anexo = new Anexo();
    KoBindings(_anexo, "knockoutPedidoAnexos");

    _anexoTipoOperacao = new AnexoTipoOperacao();
    KoBindings(_anexoTipoOperacao, "modalDetalheAnexoTipoOperacao");

    loadGridAnexoTipoOperacao();
    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("PedidoAnexo"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: isExibirOpcaoDownloadAnexo };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: isPermitirGerenciarAnexos };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover, auditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

function loadGridAnexoTipoOperacao() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoTipoOperacaoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridAnexoTipoOperacao = new BasicDataTable(_anexoTipoOperacao.AnexosTipoOperacao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoTipoOperacao.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoClick() {
    if (!isPermitirGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Pedidos.Pedido.StatusNaoPermiteAdicionarAnexo);

    var arquivo = document.getElementById(_anexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexo.Descricao.val(),
        NomeArquivo: _anexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_pedido.Codigo.val() > 0)
        enviarAnexos(_pedido.Codigo.val(), [anexo]);
    else {
        var anexos = obterAnexos();

        anexos.push(anexo);

        _anexo.Anexos.val(anexos.slice());
    }

    LimparCampos(_anexo);

    arquivo.value = null;
}

function anexoTipoOperacaoClick() {
    Global.abrirModal('modalDetalheAnexoTipoOperacao');
    $("#modalDetalheAnexoTipoOperacao").one('hidden.bs.modal', function () {
        LimparCampos(_anexoTipoOperacao);
    });
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("PedidoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function downloadAnexoTipoOperacaoClick(registroSelecionado) {
    executarDownload("TipoOperacaoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else if (isPermitirGerenciarAnexos()) {
        executarReST("PedidoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    removerAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Pedidos.Pedido.StatusNaoPermiteRemoverAnexo);
}

/*
 * Declaração das Funções
 */

function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexo(anexos);

    if (formData) {
        enviarArquivo("PedidoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _anexo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function enviarArquivosAnexados(codigo) {
    var anexos = obterAnexos();

    enviarAnexos(codigo, anexos);
}

function isExibirOpcaoDownloadAnexo(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isPermitirGerenciarAnexos() {
    return true;
}

function limparCamposAnexo() {
    LimparCampos(_anexo);

    _anexo.Anexos.val(_anexo.Anexos.def);
}

function obterAnexos() {
    return _anexo.Anexos.val().slice();
}

function obterAnexosTipoOperacao() {
    return _anexoTipoOperacao.AnexosTipoOperacao.val().slice();
}

function obterFormDataAnexo(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function recarregarGridAnexo() {
    var anexos = obterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function recarregarGridAnexoTipoOperacao() {
    var anexosTipoOperacao = obterAnexosTipoOperacao();

    _gridAnexoTipoOperacao.CarregarGrid(anexosTipoOperacao);
}


function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexo.Anexos.val(listaAnexos);
}
