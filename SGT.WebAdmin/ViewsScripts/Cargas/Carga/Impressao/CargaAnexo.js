/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridCargaAnexo;
var _knoutCargaAnexo,
    _knoutCargaAnexoCadastro;

var CargaAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });

    this.Adicionar = PropertyEntity({
        eventClick: adicionarAnexoCargaModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(false)
    });

    this.DownloadAnexos = PropertyEntity({
        eventClick: DownloadAnexosClick, type: types.event, text: Localization.Resources.Gerais.Geral.downloadTodosArquivos, visible: ko.observable(false)
    });
}

var CadastroCargaAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _knoutCargaAnexoCadastro.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCargaAnexoCadastroClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCargaAnexo() {
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoCargaClick, icone: "" };
    var opcaoDownload = { descricao: Localization.Resources.Cargas.Carga.Download, id: guid(), metodo: downloadCargaAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };

    _gridCargaAnexo = new GridView("tblAnexosCarga", "CargaAnexo/PesquisaAnexo", _knoutCargaAnexo, menuOpcoes);
}

function loadCargaAnexo() {
    _knoutCargaAnexo = new CargaAnexo();
    KoBindings(_knoutCargaAnexo, "knockoutCargaAnexo");

    _knoutCargaAnexoCadastro = new CadastroCargaAnexo();
    KoBindings(_knoutCargaAnexoCadastro, "knockoutCargaAnexoAdicionar");

    loadGridCargaAnexo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadCargaAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("CargaAnexo/DownloadAnexo", dados);
}

function removerAnexoCargaClick(registroSelecionado) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        executarReST("CargaAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    _gridCargaAnexo.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
        });
    });
}

function adicionarCargaAnexoCadastroClick() {
    if (!ValidarCamposObrigatorios(_knoutCargaAnexoCadastro))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);

    var arquivo = document.getElementById(_knoutCargaAnexoCadastro.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _knoutCargaAnexoCadastro.Descricao.val(),
        NomeArquivo: _knoutCargaAnexoCadastro.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var formData = obterFormDataAnexos([anexo]);

    if (!formData)
        return;

    enviarArquivo("CargaAnexo/AnexarArquivos?callback=?", { Codigo: _knoutCargaAnexo.Codigo.val() }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                Global.fecharModal("modalCadastroAnexoCargaGuarita");
                _gridCargaAnexo.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
    });
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

function adicionarAnexoCargaModalClick() {
    Global.abrirModal("modalCadastroAnexoCargaGuarita");
    $("#modalCadastroAnexoCargaGuarita").on("hidden.bs.modal", function () { LimparCampos(_knoutCargaAnexoCadastro); });
}


/*
 * Declaração das Funções Públicas
 */

function recarregarGridCargaAnexo() {
    _gridCargaAnexo.CarregarGrid();
    Global.abrirModal("divModalCargaAnexo");
}


function DownloadAnexosClick() {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
        executarDownload("CargaAnexo/DownloadAnexos", { Codigo: _knoutCargaAnexo.Codigo.val() });
    });
    




}