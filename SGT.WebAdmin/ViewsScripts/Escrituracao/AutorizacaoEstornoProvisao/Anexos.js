
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _autorizacao;
var _gridAnexos;
var _anexos;

/*
 * Declaração das Classes
 */

var Anexos = function () {
    this.AnexosAdicionar = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
    this.Anexos = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
    this.SalvarAnexos = PropertyEntity({ eventClick: salvarAnexosClick, type: types.event, text: "Salvar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".jpg,.tif,.pdf", text: "Upload", icon: "fal fa-file", visible: ko.observable(true) });
}

var Regra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */


function loadAnexos() {
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutAnexo");

    loadGridAnexoAdicionar();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function limparAnexosAdicionar() {
    _anexos.Anexos.list = new Array();
    RecarregarGridAnexoAdicionar();
}

function loadGridAnexoAdicionar() {

    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoRemover, opcaoDownload] };
    _anexos.Upload.file = document.getElementById(_anexos.Upload.idFile);
    var header = [
        { data: "Codigo", visible: false },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "100%", className: "text-align-left" }
    ];

    _gridAdicionarAnexo = new BasicDataTable(_anexos.AnexosAdicionar.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAdicionarAnexo.CarregarGrid([]);
}
function UploadChange() {
    var data = new Array();
    if (_anexos.Upload.file.files.length > 0) {
        for (var i = 0; i <= _anexos.Upload.file.files.length - 1; i++) {
            data.push({ Arquivo: _anexos.Upload.file.files[i], Descricao: ""})
        }
    }
    var formData = obterFormDataAnexos(data);
    var dados = { Codigo: _estornoProvisao.CodigoSolicitacao.val() }

    enviarAnexos(formData, dados);

    RecarregarGridAnexoAdicionar();
}
function RecarregarGridAnexoAdicionar() {
    var dados = []
    for (let i = 0; i <= _anexos.Anexos.list.length - 1; i++) {
        let arquivo = _anexos.Anexos.list[i];
        dados.push({ Codigo: arquivo.Codigo, NomeArquivo: arquivo.NomeArquivo })
    }
    _gridAdicionarAnexo.CarregarGrid(dados);
}

function removerAnexoClick(e) {
    _anexos.Anexos.list = _anexos.Anexos.list.filter((obj) => obj.Codigo !== e.Codigo);
    var dados = { Codigo: e.Codigo };
    executarReST("EstornoProvisaoSolicitacaoAnexo/ExcluirAnexo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) 
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso");
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
    RecarregarGridAnexoAdicionar();
}

function salvarAnexosClick(e) {
    var formData = obterFormDataAnexos(_anexos.Anexos.list);
    var dados = { Codigo: _estornoProvisao.CodigoSolicitacao.val() }
    enviarAnexos(formData, dados);
}

function enviarAnexos(formData, dados) {
    if (formData) {
        enviarArquivo("EstornoProvisaoSolicitacaoAnexo/AnexarArquivos", dados, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    _anexos.Anexos.list = retorno.Data.Anexos;
                    RecarregarGridAnexoAdicionar();
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Não foi possível anexar os arquivos.", retorno.Msg);
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
            formData.append("Descricao", anexo.Arquiv);
        });

        return formData;
    }

    return undefined;
}

function downloadAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    if (isNaN(registroSelecionado.Codigo) || !(registroSelecionado.Codigo > 0)) {
        exibirMensagem("atencao", "Não é possível fazer o download do anexo pois o mesmo ainda não foi enviado.");
        return;
    }
    executarDownload("EstornoProvisaoSolicitacaoAnexo/DownloadAnexo", dados);
}