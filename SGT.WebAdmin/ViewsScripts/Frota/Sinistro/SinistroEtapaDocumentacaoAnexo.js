/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="SinistroEtapaDocumentacao.js" />
/// <reference path="SinistroEtapaDados.js" />

var _anexoDocumentacao,
    _listaAnexoDocumentacao,
    _gridAnexosDocumentacao;

var AnexoDocumentacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoDocumentacao.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

function loadEtapaDocumentacaoAnexo() {
    _anexoDocumentacao = new AnexoDocumentacao();
    KoBindings(_anexoDocumentacao, "knockoutDocumentacaoAnexo");

    loadGridAnexosDocumentacao();
}

function loadGridAnexosDocumentacao() {
    var linhasPorPagina = 5;

    var opcaoEditar = { descricao: "Download", id: guid(), evento: "onclick", metodo: downloadDocumentacaoAnexoClick, icone: "" };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: removerDocumentacaoAnexoClick, icone: "", visibilidade: obterVisibilidadeExcluirAnexoDocumentacao };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "40%", className: "text-align-left" }
    ];

    _gridAnexosDocumentacao = new BasicDataTable(_etapaDocumentacaoSinistro.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPagina);
    _gridAnexosDocumentacao.CarregarGrid([]);
}

function obterVisibilidadeExcluirAnexoDocumentacao() {
    return _etapaDadosSinistro.Etapa.val() == EnumEtapaSinistro.Documentacao;
}

function recarregarGridDocumentacaoAnexo() {
    _gridAnexosDocumentacao.CarregarGrid(obterAnexos());
}

function adicionarAnexoDocumentacaoClick() {
    //if (!PodeGerenciarAnexos()) Validar Etapa
    //   return exibirMensagem(tipoMensagem.atencao, "Anexos", _anexos.config.msg.situacaonaopermite);

    var file = document.getElementById(_anexoDocumentacao.Arquivo.id);

    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoDocumentacao.Descricao.val(),
        NomeArquivo: _anexoDocumentacao.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    enviarAnexo(anexo);

    file.value = null;
}

function criaEEnviaFormData(anexos) {
    var formData = new FormData();

    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    var dados = {
        Codigo: _etapaDadosSinistro.Codigo.val()
    };

    enviarArquivo("SinistroDocumentacaoAnexo/AnexarArquivos", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _etapaDocumentacaoSinistro.Anexos.val(arg.Data.Anexos);
                Global.fecharModal('divModalDocumentacaoAnexo');
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function enviarAnexo(anexo) {
    var anexos = [anexo];

    criaEEnviaFormData(anexos);
}

function adicionarAnexoDocumentacaoModalClick() {
    $("#divModalDocumentacaoAnexo")
        .modal('show')
        .one('hidden.bs.modal', function () {
            LimparCampos(_anexoDocumentacao);
        });;
}

function downloadDocumentacaoAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("SinistroDocumentacaoAnexo/DownloadAnexo", dados);
}

function removerDocumentacaoAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("SinistroDocumentacaoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
}

function obterAnexos() {
    return _etapaDocumentacaoSinistro.Anexos.val().slice();
}

function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _etapaDocumentacaoSinistro.Anexos.val(listaAnexos);
}