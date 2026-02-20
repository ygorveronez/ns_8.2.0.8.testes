/*
 * Declaração de Objetos Globais do Arquivo
 */

var _anexoGuarita;
var _gridAnexoGuarita;
var _listaAnexosGuarita;

/*
 * Declaração das Classes
 */

var AnexoGuarita = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoGuarita.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoGuaritaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexoGuarita = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoGuarita();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoGuaritaModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGuaritaAnexo() {
    _anexoGuarita = new AnexoGuarita();
    KoBindings(_anexoGuarita, "knockoutGuaritaAnexoAdicionar");

    _listaAnexosGuarita = new ListaAnexoGuarita();
    KoBindings(_listaAnexosGuarita, "knockoutGuaritaAnexo");

    loadGridAnexoGuarita();
}

function loadGridAnexoGuarita() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoGuaritaClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoGuaritaClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoGuarita = new BasicDataTable(_listaAnexosGuarita.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoGuarita.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoGuaritaClick() {
    var arquivo = document.getElementById(_anexoGuarita.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoGuarita)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoGuarita.Descricao.val(),
        NomeArquivo: _anexoGuarita.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosGuarita(_guaritaFluxoPatio.Codigo.val(), [anexo]);

    var listaAnexos = obterAnexosGuarita();

    listaAnexos.push(anexo);

    _listaAnexosGuarita.Anexos.val(listaAnexos.slice());

    _anexoGuarita.Arquivo.val("");

    Global.fecharModal('divModalAdicionarAnexoGuarita');
} 

function adicionarAnexoGuaritaModalClick() {
    Global.abrirModal('divModalAdicionarAnexoGuarita');
    $("#divModalAdicionarAnexoGuarita").one('hidden.bs.modal', function () {
        LimparCampos(_anexoGuarita);
    });
}

function downloadAnexoGuaritaClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("GuaritaAnexo/DownloadAnexo", dados);
}

function removerAnexoGuaritaClick(registroSelecionado) {
    executarReST("GuaritaAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoLocalGuarita(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparAnexoGuarita() {
    //_listaAnexo.Adicionar.visible(!isAcessoTransportador());
    _listaAnexosGuarita.Anexos.val(new Array());
}

function preencherAnexoGuarita(dadosAnexos) {
    _listaAnexosGuarita.Anexos.val(dadosAnexos);
}

/*
 * Declaração das Funções Privadas
 */

function enviarAnexosGuarita(codigo, anexos) {
    var formData = obterFormDataAnexosGuarita(anexos);

    if (formData) {
        enviarArquivo("GuaritaAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    buscarAnexosGuarita();
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function buscarAnexosGuarita() {
    executarReST("GuaritaAnexo/ObterAnexo", { Codigo: _guaritaFluxoPatio.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _listaAnexosGuarita.Anexos.val(arg.Data.Anexos);
                recarregarGridAnexoGuarita();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function obterAnexosGuarita() {
    return _listaAnexosGuarita.Anexos.val().slice();
}

function obterFormDataAnexosGuarita(anexos) {
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

function recarregarGridAnexoGuarita() {
    var anexos = obterAnexosGuarita();

    _gridAnexoGuarita.CarregarGrid(anexos);
}

function removerAnexoLocalGuarita(registroSelecionado) {
    var listaAnexos = obterAnexosGuarita();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexosGuarita.Anexos.val(listaAnexos);
}
