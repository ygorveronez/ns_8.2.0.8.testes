//#region Declaração de Variáveis Globais do Arquivo

var _anexoNFS;
var _gridAnexos;
var _listaAnexos;

//#endregion

//#region Declarações dos Objetos

var AnexoNFSManualAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoNFS.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexos = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGrid();
    });

    this.Adicionar = PropertyEntity({ eventClick: abrirModalAdicionarAnexos, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

//#endregion

//#region Inicializadores

function loadAnexosInformarNFS() {
    _anexoNFS = new AnexoNFSManualAnexo();
    KoBindings(_anexoNFS, "knockoutAdicionarAnexo");

    _listaAnexos = new ListaAnexos();
    KoBindings(_listaAnexos, "knockoutListaAnexos");

    LoadGridAnexo();
}

function LoadGridAnexo() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexo, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexo, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexos = new BasicDataTable(_listaAnexos.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexos.CarregarGrid([]);
}

//#endregion

//#region Eventos

function downloadAnexo(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("AnexoLancamentoNFSManual/DownloadAnexo", dados);
}

function removerAnexo(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocalmente(registroSelecionado);
    else {
        executarReST("AnexoLancamentoNFSManual/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoLocalmente(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function abrirModalAdicionarAnexos() {
    Global.abrirModal('knockoutAdicionarAnexo');

    $("#knockoutAdicionarAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_anexoNFS);
    });
}

function adicionarAnexoClick() {
    var arquivo = document.getElementById(_anexoNFS.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoNFS)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoNFS.Descricao.val(),
        NomeArquivo: _anexoNFS.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var listaAnexos = obterListaAnexos();

    listaAnexos.push(anexo);
    _listaAnexos.Anexos.val(listaAnexos.slice());
    _anexoNFS.Arquivo.val("");
    Global.fecharModal("knockoutAdicionarAnexo");
    enviarAnexo(anexo);
}

//#endregion

//#region Funções privadas

function obterListaAnexos() {
    return _listaAnexos.Anexos.val().slice();
}

function recarregarGrid() {
    var anexos = obterListaAnexos();
    _gridAnexos.CarregarGrid(anexos);
}

function removerAnexoLocalmente(registroSelecionado) {
    var listaAnexos = obterListaAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexos.Anexos.val(listaAnexos);
}

function enviarAnexo(anexo) {
    var codigo = _listaAnexos.Codigo.val();

    if (!anexo) {
        return;
    }

    var formData = obterFormDataAnexos([anexo]);

    if (formData) {
        enviarArquivo("AnexoLancamentoNFSManual/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    _listaAnexos.Anexos.val(retorno.Data.Anexos);
                    Global.fecharModal("divModalAnexosProdutor");
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexo.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexos(anexos) {
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

//#endregion