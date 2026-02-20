/*
 * Declaração de Objetos Globais do Arquivo
 */

var _anexoTravamentoChave;
var _gridAnexoTravamentoChave;
var _listaAnexosTravamentoChave;

/*
 * Declaração das Classes
 */

var AnexoTravamentoChave = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoTravamentoChave.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoTravamentoChaveClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexoTravamentoChave = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoTravamentoChave();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoTravamentoChaveModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadTravamentoChaveAnexo() {
    _anexoTravamentoChave = new AnexoTravamentoChave();
    KoBindings(_anexoTravamentoChave, "knockoutTravamentoChaveAnexoAdicionar");

    _listaAnexosTravamentoChave = new ListaAnexoTravamentoChave();
    KoBindings(_listaAnexosTravamentoChave, "knockoutTravamentoChaveAnexo");

    loadGridAnexoTravamentoChave();
}

function loadGridAnexoTravamentoChave() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoTravamentoChaveClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoTravamentoChaveClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoTravamentoChave = new BasicDataTable(_listaAnexosTravamentoChave.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoTravamentoChave.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoTravamentoChaveClick() {
    var arquivo = document.getElementById(_anexoTravamentoChave.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoTravamentoChave)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoTravamentoChave.Descricao.val(),
        NomeArquivo: _anexoTravamentoChave.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosTravamentoChave(_travamentoChave.Codigo.val(), [anexo]);

    var listaAnexos = obterAnexosTravamentoChave();

    listaAnexos.push(anexo);

    _listaAnexosTravamentoChave.Anexos.val(listaAnexos.slice());

    _anexoTravamentoChave.Arquivo.val("");

    Global.fecharModal('divModalAdicionarAnexoTravamentoChave');
}

function adicionarAnexoTravamentoChaveModalClick() {
    Global.abrirModal('divModalAdicionarAnexoTravamentoChave');
    $("#divModalAdicionarAnexoTravamentoChave").one('hidden.bs.modal', function () {
        LimparCampos(_anexoTravamentoChave);
    });
}

function downloadAnexoTravamentoChaveClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("TravamentoChaveAnexo/DownloadAnexo", dados);
}

function removerAnexoTravamentoChaveClick(registroSelecionado) {
    executarReST("TravamentoChaveAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoLocalTravamentoChave(registroSelecionado);
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

function limparAnexoTravamentoChave() {
    //_listaAnexo.Adicionar.visible(!isAcessoTransportador());
    _listaAnexosTravamentoChave.Anexos.val(new Array());
}

function preencherAnexoTravamentoChave(dadosAnexos) {
    _listaAnexosTravamentoChave.Anexos.val(dadosAnexos);
}

/*
 * Declaração das Funções Privadas
 */

function enviarAnexosTravamentoChave(codigo, anexos) {
    var formData = obterFormDataAnexosTravamentoChave(anexos);

    if (formData) {
        enviarArquivo("TravamentoChaveAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    buscarAnexosTravamentoChave();
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function buscarAnexosTravamentoChave() {
    executarReST("TravamentoChaveAnexo/ObterAnexo", { Codigo: _travamentoChave.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _listaAnexosTravamentoChave.Anexos.val(arg.Data.Anexos);
                recarregarGridAnexoTravamentoChave();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function obterAnexosTravamentoChave() {
    return _listaAnexosTravamentoChave.Anexos.val().slice();
}

function obterFormDataAnexosTravamentoChave(anexos) {
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

function recarregarGridAnexoTravamentoChave() {
    var anexos = obterAnexosTravamentoChave();

    _gridAnexoTravamentoChave.CarregarGrid(anexos);
}

function removerAnexoLocalTravamentoChave(registroSelecionado) {
    var listaAnexos = obterAnexosTravamentoChave();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexosTravamentoChave.Anexos.val(listaAnexos);
}
