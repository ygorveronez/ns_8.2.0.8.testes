/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosDocumentoAssinado;
var _gridAnexosDocumentoAssinado;

var AnexosDocumentoAssinado = function () {
    this.DocumentoAssinado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Arquivo, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosDocumentoAssinado.NomeArquivo.val(nomeArquivo);
    });

    this.DocumentosAssinados = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.DocumentosAssinados.val.subscribe(function () {
        RenderizarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentoAssinadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosDocumentoAssinado() {
    _anexosDocumentoAssinado = new AnexosDocumentoAssinado();
    KoBindings(_anexosDocumentoAssinado, "knockoutDocumentoAssinados");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoDocumentoAssinadoClick, icone: "", visibilidade: visibleDownloadAnexo };
    var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoDocumentoAssinadoClick, icone: "" };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 5;
    _gridAnexosDocumentoAssinado = new BasicDataTable(_anexosDocumentoAssinado.DocumentosAssinados.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosDocumentoAssinado.CarregarGrid([]);
}

function downloadAnexoDocumentoAssinadoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("TermoQuitacaoDocumentoAssinado/DownloadAnexo", data);
}

function removerAnexoDocumentoAssinadoClick(dataRow) {
    var listaAnexos = GetAnexos();
    RemoverAnexoDocumentoAssinado(dataRow, listaAnexos, _anexosDocumentoAssinado, "TermoQuitacaoDocumentoAssinado/ExcluirAnexo");
}

function gerenciarAnexosClick() {
    Global.abrirModal('divModalGerenciarAnexos');
}

function adicionarAnexoDocumentoAssinadoClick() {

    // Busca o input de arquivos
    var file = document.getElementById(_anexosDocumentoAssinado.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosDocumentoAssinado.Descricao.val(),
        NomeArquivo: _anexosDocumentoAssinado.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a DocumentoAssinado, envia o anexo direto
    if (_termoQuitacao.Codigo.val() > 0) {
        EnviarAnexo(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaDocumentosAssinados = GetAnexos();
        listaDocumentosAssinados.push(anexo);
        _anexosDocumentoAssinado.DocumentosAssinados.val(listaDocumentosAssinados.slice());
    }

    // Limpa os campos
    LimparCampos(_anexosDocumentoAssinado);
    _anexosDocumentoAssinado.Arquivo.val('');

    file.value = null;
}

//*******MÉTODOS*******

function GetAnexos() {
    // Retorna um clone do array para não prender a referencia
    return _anexosDocumentoAssinado.DocumentosAssinados.val().slice();
}

function RenderizarGridAnexos() {
    // Busca a lista
    var anexos = GetAnexos();

    // E chama o metodo da grid
    _gridAnexosDocumentoAssinado.CarregarGrid(anexos);
}

function EnviarArquivosAnexadosDocumentoAssinado(codigoDocumentoAssinado) {
    // Busca a lista
    var anexos = GetAnexos();

    if (anexos.length > 0) {
        var dados = {
            Codigo: codigoDocumentoAssinado,
        }

        CriaEEnviaFormData(anexos, dados);
    }
}

function RemoverAnexoDocumentoAssinado(dataRow, documentosAssinados, knout, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        documentosAssinados.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                documentosAssinados.splice(i, 1);
            }
        });

        knout.DocumentosAssinados.val(documentosAssinados);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {

        // Exclui do sistema
        executarReST(url, dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    RemoveDaGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function CriaEEnviaFormData(anexos, dados) {
    // Busca todos file
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });
    enviarArquivo("TermoQuitacaoDocumentoAssinado/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                console.log(arg.Data)
                _anexosDocumentoAssinado.DocumentosAssinados.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            limparDocumentoAssinadoAnexos();
        }
    });
}

function EnviarAnexo(anexo) {
    var anexos = [anexo];
    var dados = {
        Codigo: _termoQuitacao.Codigo.val(),
    }

    CriaEEnviaFormData(anexos, dados);
}

function limparDocumentoAssinadoAnexos() {
    LimparCampos(_anexosDocumentoAssinado);
    _anexosDocumentoAssinado.DocumentosAssinados.val(_anexosDocumentoAssinado.DocumentosAssinados.def);
}

function visibleDownloadAnexo(dataRow) {
    return !isNaN(dataRow.Codigo);
}