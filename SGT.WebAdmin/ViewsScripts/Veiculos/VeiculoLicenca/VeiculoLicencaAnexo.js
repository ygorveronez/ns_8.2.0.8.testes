/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _licencaVeiculoAnexo;
var _gridLicencaVeiculoAnexo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var LicencaVeiculoAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });    
    this.Arquivo = PropertyEntity({ type: types.file, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.DANFSE.val().replace('C:\\fakepath\\', '') }) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexo, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _licencaVeiculoAnexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexoLicencaVeiculo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoLicencaVeiculoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadLicencaVeiculoAnexos() {
    _licencaVeiculoAnexo = new LicencaVeiculoAnexo();
    KoBindings(_licencaVeiculoAnexo, "knockoutLicencaVeiculoAnexos");

    loadGridLicencaVeiculoAnexos();
}

function loadGridLicencaVeiculoAnexos() {
    var linhasPorPagina = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoLicencaVeiculoClick, icone: "", visibilidade: isExibirOpcaoDownloadAnexoLicencaVeiculo };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoLicencaVeiculoClick, icone: "", visibilidade: isExibirOpcaoRemoverAnexoLicencaVeiculo };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridLicencaVeiculoAnexo = new BasicDataTable(_licencaVeiculoAnexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPagina);
    _gridLicencaVeiculoAnexo.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarAnexoLicencaVeiculoClick() {
    if (!ValidarCamposObrigatorios(_licencaVeiculoAnexo))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);

    var arquivo = document.getElementById(_licencaVeiculoAnexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexo, Localization.Resources.Veiculos.VeiculoLicenca.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _licencaVeiculoAnexo.Descricao.val(),
        NomeArquivo: _licencaVeiculoAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_licencaVeiculo.Codigo.val() > 0)
        enviarAnexosLicencaVeiculo(_licencaVeiculo.Codigo.val(), [anexo]);
    else {
        var anexos = obterAnexosLicencaVeiculo();

        anexos.push(anexo);

        _licencaVeiculoAnexo.Anexos.val(anexos.slice());
    }

    LimparCampos(_licencaVeiculoAnexo);
    _licencaVeiculoAnexo.Arquivo.val("");
    _licencaVeiculoAnexo.Arquivo.val(Localization.Resources.Veiculos.VeiculoLicenca.SelecioneUmaImagemParaAnexar.getFieldDescription());    

    arquivo.value = null;
}

function downloadAnexoLicencaVeiculoClick(registroSelecionado) {
    executarDownload("VeiculoLicencaAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoLicencaVeiculoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocalLicencaVeiculo(registroSelecionado);
    else {
        executarReST("VeiculoLicencaAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    removerAnexoLocalLicencaVeiculo(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function enviarAnexosLicencaVeiculo(codigo, anexos) {
    var formData = obterFormDataAnexoLicencaVeicuo(anexos);
    var p = new promise.Promise();

    if (formData) {
        enviarArquivo("VeiculoLicencaAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _licencaVeiculoAnexo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Veiculos.VeiculoLicenca.ArquivosDaLicencaAnexadosComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.VeiculoLicenca.NaoFoiPossivelAnexarArquivoDaLicenca, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            p.done();
        });
    }
    else
        p.done();

    return p;
}

function isExibirOpcaoDownloadAnexoLicencaVeiculo(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isExibirOpcaoRemoverAnexoLicencaVeiculo() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe
}

function obterAnexosLicencaVeiculo() {
    return _licencaVeiculoAnexo.Anexos.val().slice();
}

function obterFormDataAnexoLicencaVeicuo(anexos) {
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

function recarregarGridAnexoLicencaVeiculo() {
    var anexos = obterAnexosLicencaVeiculo();

    _gridLicencaVeiculoAnexo.CarregarGrid(anexos);
}

function removerAnexoLocalLicencaVeiculo(registroSelecionado) {
    var listaAnexos = obterAnexosLicencaVeiculo();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _licencaVeiculoAnexo.Anexos.val(listaAnexos);
}

// #endregion Funções Privadas
