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

var _anexosTermoQuitacao;
var _gridAnexosTermoQuitacao;

var AnexosTermoQuitacao = function () {
    this.TermoQuitacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Arquivo, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosTermoQuitacao.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoTermoQuitacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosTermoQuitacao() {
    _anexosTermoQuitacao = new AnexosTermoQuitacao();
    KoBindings(_anexosTermoQuitacao, "knockoutCadastroAnexos");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoTermoQuitacaoClick, icone: "", visibilidade: visibleDownloadAnexo };
    var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoTermoQuitacaoClick, icone: "" };

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
    _gridAnexosTermoQuitacao = new BasicDataTable(_anexosTermoQuitacao.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosTermoQuitacao.CarregarGrid([]);
}

function downloadAnexoTermoQuitacaoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("TermoQuitacaoFinanceiroAnexo/DownloadAnexo", data);
}

function removerAnexoTermoQuitacaoClick(dataRow) {
    var listaAnexos = GetAnexos();
    RemoverAnexoTermoQuitacao(dataRow, listaAnexos, _anexosTermoQuitacao, "TermoQuitacaoFinanceiroAnexo/ExcluirAnexo");
}

function gerenciarAnexosClick() {
    Global.abrirModal('divModalGerenciarAnexos');
}

function adicionarAnexoTermoQuitacaoClick() {

    // Busca o input de arquivos
    var file = document.getElementById(_anexosTermoQuitacao.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosTermoQuitacao.Descricao.val(),
        NomeArquivo: _anexosTermoQuitacao.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a TermoQuitacao, envia o anexo direto
    if (_termoQuitacao.Codigo.val() > 0) {
        EnviarAnexo(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexos();
        listaAnexos.push(anexo);
        _anexosTermoQuitacao.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos
    LimparCampos(_anexosTermoQuitacao);
    _anexosTermoQuitacao.Arquivo.val('');

    file.value = null;
}

//*******MÉTODOS*******

function GetAnexos() {
    // Retorna um clone do array para não prender a referencia
    return _anexosTermoQuitacao.Anexos.val().slice();
}

function RenderizarGridAnexos() {
    // Busca a lista
    var anexos = GetAnexos();

    // E chama o metodo da grid
    _gridAnexosTermoQuitacao.CarregarGrid(anexos);
}

function EnviarArquivosAnexadosTermoQuitacao(codigoTermoQuitacao) {
    // Busca a lista
    var anexos = GetAnexos();

    if (anexos.length > 0) {
        var dados = {
            Codigo: codigoTermoQuitacao,
        }

        CriaEEnviaFormData(anexos, dados);
    }
}

function RemoverAnexoTermoQuitacao(dataRow, listaAnexos, knout, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        knout.Anexos.val(listaAnexos);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {

        // Exclui do sistema
        executarReST(url, dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
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

    enviarArquivo("TermoQuitacaoFinanceiroAnexo/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosTermoQuitacao.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            limparTermoQuitacaoAnexos();
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

function limparTermoQuitacaoAnexos() {
    LimparCampos(_anexosTermoQuitacao);
    _anexosTermoQuitacao.Anexos.val(_anexosTermoQuitacao.Anexos.def);
}

function visibleDownloadAnexo(dataRow) {
    return !isNaN(dataRow.Codigo);
}