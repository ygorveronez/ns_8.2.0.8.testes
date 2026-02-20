/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="CargaControleExpedicao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosCargaControleExpedicao;
var _gridAnexosCargaControleExpedicao;

var AnexosCargaControleExpedicao = function () {
    this.CargaControleExpedicao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexos, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosCargaControleExpedicao.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosCargaControleExpedicao();
    });
    this.QuantidadeImagensEsperada = PropertyEntity({ val: ko.observable(0), def: 0, text: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(false) });
    this.NotaFiscalServico = PropertyEntity({ val: ko.observable(false), def: false, text: ko.observable(Localization.Resources.Gerais.Geral.UmaNFS), getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoCargaControleExpedicao, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosCargaControleExpedicao() {
    _anexosCargaControleExpedicao = new AnexosCargaControleExpedicao();
    KoBindings(_anexosCargaControleExpedicao, "knockoutCadastroAnexosCargaControleExpedicao");

    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoCargaControleExpedicaoClick, icone: "", visibilidade: visibleDownloadCargaControleExpedicao };
    var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoCargaControleExpedicaoClick, icone: "", visibilidade: true };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download, remover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    var linhasPorPaginas = 7;
    _gridAnexosCargaControleExpedicao = new BasicDataTable(_anexosCargaControleExpedicao.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosCargaControleExpedicao.CarregarGrid([]);
}

function visibleDownloadCargaControleExpedicao(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function downloadAnexoCargaControleExpedicaoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("CargaControleExpedicaoAnexo/DownloadAnexo", data);
}

function removerAnexoCargaControleExpedicaoClick(dataRow) {
    var listaAnexosCargasControleExpedicaos = GetAnexosCargaControleExpedicao();
    RemoverAnexoCargaControleExpedicao(dataRow, listaAnexosCargasControleExpedicaos, _anexosCargaControleExpedicao, "CargaControleExpedicaoAnexo/ExcluirAnexo");
}

function gerenciarAnexosCargaControleExpedicaoClick(data) {
    LimparCamposAnexosCargaControleExpedicao();
    _anexosCargaControleExpedicao.CargaControleExpedicao.val(data.Codigo);
    executarReST("CargaControleExpedicaoAnexo/ObterAnexo", { Codigo: data.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexosCargaControleExpedicao.Anexos.val(retorno.Data.Anexos);
                recarregarGridAnexo();
                Global.abrirModal('divModalGerenciarAnexosCargaControleExpedicao');
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function adicionarAnexoCargaControleExpedicao() {
    var file = document.getElementById(_anexosCargaControleExpedicao.Arquivo.id);

    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexosCargaControleExpedicao.Descricao.val(),
        NomeArquivo: _anexosCargaControleExpedicao.NomeArquivo.val(),
        Arquivo: file.files[0],
    };

    EnviarAnexoCargaControleExpedicao(anexo);

    var codigoLinhagrid = _anexosCargaControleExpedicao.CargaControleExpedicao.val();

    LimparCamposAnexosCargaControleExpedicao();

    _anexosCargaControleExpedicao.CargaControleExpedicao.val(codigoLinhagrid);
}

//*******MÉTODOS*******

function LimparCamposAnexosCargaControleExpedicao() {
    var file = document.getElementById(_anexosCargaControleExpedicao.Arquivo.id);
    LimparCampos(_anexosCargaControleExpedicao);
    _anexosCargaControleExpedicao.Arquivo.val("");
    recarregarGridAnexo();
    file.value = null;
}

function GetAnexosCargaControleExpedicao() {
    // Retorna um clone do array para não prender a referencia
    return _anexosCargaControleExpedicao.Anexos.val().slice();
}

function CarregarAnexosCargaControleExpedicao(data) {
    _anexosCargaControleExpedicao.Anexos.val(data.Anexos);
    _anexosCargaControleExpedicao.QuantidadeImagensEsperada.val(data.QuantidadeImagensEsperada);

    var diferencaAnexos = _anexosCargaControleExpedicao.QuantidadeImagensEsperada.val() - data.Anexos.length;
    if (diferencaAnexos > 0) {
        _anexosCargaControleExpedicao.QuantidadeImagensEsperada.text(Localization.Resources.Gerais.Geral.AindaFaltamSerSincronizado + diferencaAnexos + Localization.Resources.Gerais.Geral.AnexoNovamenteAtendimentoFaltantes);
        _anexosCargaControleExpedicao.QuantidadeImagensEsperada.visible(true);
    } else {
        _anexosCargaControleExpedicao.QuantidadeImagensEsperada.text("");
        _anexosCargaControleExpedicao.QuantidadeImagensEsperada.visible(false);
    }
}

function RenderizarGridAnexosCargaControleExpedicao() {
    var anexosCargasControleExpedicaos = GetAnexosCargaControleExpedicao();

    var lista = new Array();
    anexosCargasControleExpedicaos.forEach(function (anexo, i) {
        var data = new Object();

        data.Codigo = anexo.Codigo;
        data.Descricao = anexo.Descricao;
        data.NomeArquivo = anexo.NomeArquivo;

        lista.push(data);
    });

    _gridAnexosCargaControleExpedicao.CarregarGrid(lista);
}

function EnviarArquivosAnexadosCargaControleExpedicao(cb) {
    // Busca a lista
    var anexosCargasControleExpedicaos = GetAnexosCargaControleExpedicao();

    if (anexosCargasControleExpedicaos.length > 0) {
        var dados = {
            CargaControleExpedicao: cb.Codigo
        }
        CriaEEnviaFormDataCargaControleExpedicao(anexosCargasControleExpedicaos, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoCargaControleExpedicao(dataRow, listaAnexosCargasControleExpedicaos, ko, url) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.DesejaRealmenteRemoverAnexoAtendimento, function () {
        // Funcao auxiliar
        var RemoveDaGrid = function () {
            listaAnexosCargasControleExpedicaos.forEach(function (anexo, i) {
                if (dataRow.Codigo == anexo.Codigo) {
                    listaAnexosCargasControleExpedicaos.splice(i, 1);
                }
            });

            ko.Anexos.val(listaAnexosCargasControleExpedicaos);
        }
        executarReST(url, dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    RemoveDaGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    });
}

function CriaEEnviaFormDataCargaControleExpedicao(anexosCargasControleExpedicaos, dados, cb) {
    var formData = new FormData();
    anexosCargasControleExpedicaos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("CargaControleExpedicaoAnexo/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosCargaControleExpedicao.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
            if (cb)
                cb();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function EnviarAnexoCargaControleExpedicao(anexo) {
    var anexosCargasControleExpedicaos = [anexo];
    var dados = {
        Codigo: _anexosCargaControleExpedicao.CargaControleExpedicao.val()
    }

    CriaEEnviaFormDataCargaControleExpedicao(anexosCargasControleExpedicaos, dados);
}

function limparCargaControleExpedicaoAnexos() {
    LimparCampos(_anexosCargaControleExpedicao);
    _anexosCargaControleExpedicao.Anexos.val(_anexosCargaControleExpedicao.Anexos.def);
    _anexosCargaControleExpedicao.Adicionar.visible(true);
}

function isPossuiAnexo() {
    return GetAnexosCargaControleExpedicao().length > 0;
}

function recarregarGridAnexo() {
    var anexos = GetAnexosCargaControleExpedicao();

    _gridAnexosCargaControleExpedicao.CarregarGrid(anexos);
}