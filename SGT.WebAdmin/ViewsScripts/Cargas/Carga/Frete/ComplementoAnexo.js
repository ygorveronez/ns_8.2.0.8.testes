/// <reference path="Complemento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexoComplemento;
var _anexoComplemento;

/*
 * Declaração das Classes
 */

var AnexoComplemento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexoComplemento.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexoComplemento);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoCargaComplementoFreteAnexo() {
    _anexoComplemento = new AnexoComplemento();
    KoBindings(_anexoComplemento, "knockoutCargaComplementoFreteAnexo");

    loadGridAnexoComplemento();
}

function loadGridAnexoComplemento() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoComplementoClick, icone: "", visibilidade: isExibirOpcaoDownloadAnexoComplemento };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoComplementoClick, icone: "", visibilidade: isPermiteRemoverAnexosComplemento };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "20%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "50%", className: "text-align-left" }
    ];

    _gridAnexoComplemento = new BasicDataTable(_anexoComplemento.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoComplemento.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AbrirAnexoComplementoClick() {
    Global.abrirModal('divModalCargaComplementoFreteAnexo');
}

function FecharAnexoComplementoClick() {
    Global.fecharModal('divModalCargaComplementoFreteAnexo');
}

function adicionarAnexoClick() {
    if (!isPermitirGerenciarAnexosComplemento())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo);

    var arquivo = document.getElementById(_anexoComplemento.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoComplemento.Descricao.val(),
        NomeArquivo: _anexoComplemento.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var anexos = obterAnexosComplemento();

    anexos.push(anexo);

    _anexoComplemento.Anexos.val(anexos.slice());

    LimparCampos(_anexoComplemento);
    _anexoComplemento.Arquivo.val("");

    arquivo.value = null;
}

function downloadAnexoComplementoClick(registroSelecionado) {
    executarDownload("CargaComplementoFreteAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoComplementoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocalComplemento(registroSelecionado);
    else if (isPermiteRemoverAnexosComplemento(registroSelecionado)) {
        executarReST("CargaComplementoFreteAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    removerAnexoLocalComplemento(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, "Não é permitido remover Anexos");
}

/*
 * Declaração das Funções
 */

function enviarAnexosComplemento(codigo, anexos) {
    var formData = obterFormDataAnexo(anexos);

    var p = new promise.Promise();

    if (formData) {

        enviarArquivo("CargaComplementoFreteAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    recarregarGridAnexo();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivosAnexadosSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            p.done();
        });

    } else {
        p.done();
    }

    return p;
}

function enviarArquivosAnexadosComplemento(codigo) {
    var anexos = obterAnexosComplemento();

    enviarAnexosComplemento(codigo, anexos);
}

function isExibirOpcaoDownloadAnexoComplemento(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isPermitirGerenciarAnexosComplemento() {
    return true
}

function limparCamposAnexo() {
    LimparCampos(_anexoComplemento);
    if (!_anexoComplemento || !_anexoComplemento.Anexos)
        return;

    _anexoComplemento.Anexos.val(_anexoComplemento.Anexos.def);
}

function obterAnexosComplemento() {
    return _anexoComplemento.Anexos.val().slice();
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

function recarregarGridAnexoComplemento() {
    var anexos = obterAnexosComplemento();

    _gridAnexoComplemento.CarregarGrid(anexos);
}

function removerAnexoLocalComplemento(registroSelecionado) {
    var listaAnexos = obterAnexosComplemento();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexoComplemento.Anexos.val(listaAnexos);
}

function isPossuiAnexoComplmento() {
    return obterAnexosComplemento().length > 0;
}

function isPermiteRemoverAnexosComplemento(registroSelecionado) {
    return isNaN(registroSelecionado.Codigo);
}