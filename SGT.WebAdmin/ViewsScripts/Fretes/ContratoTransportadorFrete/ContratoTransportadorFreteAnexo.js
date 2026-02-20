/// <reference path="ContratoTransportadorFrete.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexo;
var _anexo;

/*
 * Declaração das Classes
 */

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoContratoTransportadorFrete() {
    _anexo = new Anexo();
    KoBindings(_anexo, "knockoutContratoTransportadorFreteAnexo");

    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: isExibirOpcaoDownloadAnexo };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: isPermitirGerenciarAnexos };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "20%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "50%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AbrirTelaAnexoClick() {
    Global.abrirModal('knockoutContratoTransportadorFreteAnexo');
}

function FecharTelaAnexoClick() {
    Global.fecharModal('knockoutContratoTransportadorFreteAnexo');
}

function adicionarAnexoClick() {
    if (!isPermitirGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Status não permite adicionar anexo");

    var arquivo = document.getElementById(_anexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

        const fsize = arquivo.files[0].size;
        const tamanhoLimite = 5120;
        const file = Math.round((fsize / 1024));

        if (file >= tamanhoLimite)
            return exibirMensagem(tipoMensagem.atencao, "Anexos", "Arquivo maior do que o limite de 5MB");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexo.Descricao.val(),
        NomeArquivo: _anexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };


    //if (_contratoTransporteFrete.Codigo.val() > 0)
    //    enviarAnexos(_contratoTransporteFrete.Codigo.val(), [anexo]);

    var anexos = obterAnexos();

    anexos.push(anexo);

    _anexo.Anexos.val(anexos.slice());

    LimparCampos(_anexo);
    _anexo.Arquivo.val("");

    arquivo.value = null;
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("ContratoTransporteFreteAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else if (isPermiteRemoverAnexos(registroSelecionado)) {
        executarReST("ContratoTransporteFreteAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Não é permitido remover Anexos de Contratos já cadastrados");
}

/*
 * Declaração das Funções
 */

//function enviarAnexos(codigo, anexos) {
//    var formData = obterFormDataAnexo(anexos);

//    if (formData) {
//        enviarArquivo("ContratoTransporteFreteAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
//            if (retorno.Success) {
//                if (retorno.Data) {
//                    _anexo.Anexos.val(retorno.Data.Anexos);

//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
//                }
//                else
//                    exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
//            }
//            else
//                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
//        });
//    }
//}
function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexo(anexos);

    var p = new promise.Promise();

    if (formData) {

        enviarArquivo("ContratoTransporteFreteAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
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

function enviarArquivosAnexados(codigo) {
    var anexos = obterAnexos();
    
    enviarAnexos(codigo, anexos);
}

function isExibirOpcaoDownloadAnexo(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isPermitirGerenciarAnexos() {
    return true
}

function limparCamposAnexo() {
    LimparCampos(_anexo);

    _anexo.Anexos.val(_anexo.Anexos.def);
}

function obterAnexos() {
    return _anexo.Anexos.val().slice();
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

function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexo.Anexos.val(listaAnexos);
}

function isPossuiAnexo() {
    return obterAnexos().length > 0;
}

function isPermiteRemoverAnexos(registroSelecionado) {
    return isNaN(registroSelecionado.Codigo);
}