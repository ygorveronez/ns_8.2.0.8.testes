/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridControleArquivo;
var _pesquisaControleArquivo;
var _controleArquivo;
var _lancamentoArquivo;
var _gridAnexo;
var atualizando = false;
var _modalControleArquivo;

var PesquisaControleArquivo = function () {

    this.Descricao = PropertyEntity({ text: "Descrição: ", visible: ko.observable(true), maxlength: 200, val: ko.observable("") });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataVencimentoInicial = PropertyEntity({ text: "Vencimento Inicial: ", visible: ko.observable(true), getType: typesKnockout.date });
    this.DataVencimentoFinal = PropertyEntity({ text: "Vencimento Final: ", visible: ko.observable(true), getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisouNovamente = true;
            _gridControleArquivo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
};

var ControleArquivo = function () {
    this.ControleArquivo = PropertyEntity({ type: types.event, text: "Painel", idGrid: guid(), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarControleArquivoClick, type: types.event, text: "Adicionar Arquivo", visible: ko.observable(true) });
};

var LancamentoArquivo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.DataVencimento = PropertyEntity({ text: "Data Vencimento:", getType: typesKnockout.date, val: ko.observable(""), def: "", required: false, enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", maxlength: 2000, enable: ko.observable(true), required: true });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 2000, enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Cliente:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GerouAlertaCliente = PropertyEntity({ getType: typesKnockout.bool, visible: ko.observable(false) });
    this.GerouAlertaEmpresa = PropertyEntity({ getType: typesKnockout.bool, visible: ko.observable(false) });

    this.CodigoAnexo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _lancamentoArquivo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

//*******EVENTOS*******
function loadControleArquivo() {
    //-- Knouckout
    // Instancia pesquisa    
    _pesquisaControleArquivo = new PesquisaControleArquivo();
    KoBindings(_pesquisaControleArquivo, "knockoutPesquisaControleArquivo", false, _pesquisaControleArquivo.Pesquisar.id);

    _controleArquivo = new ControleArquivo();
    KoBindings(_controleArquivo, "knockoutControleArquivo", false);

    _lancamentoArquivo = new LancamentoArquivo();
    KoBindings(_lancamentoArquivo, "knoutControleArquivo");

    new BuscarClientes(_pesquisaControleArquivo.Cliente);
    new BuscarClientes(_lancamentoArquivo.Cliente);

    HeaderAuditoria("ControleArquivo", _controleArquivo);

    // Inicia busca
    buscarControleArquivo();
    loadGridAnexo();

    $('#divModalControleArquivo').on('hidden.bs.modal', function () {
        atualizando = false;
        //_gridControleArquivo.CarregarGrid();
    });
    _modalControleArquivo = new bootstrap.Modal(document.getElementById("divModalControleArquivo"), { backdrop: true, keyboard: true });
}

function AdicionarControleArquivoClick(e, sender) {
    limparCampos();

    _modalControleArquivo.show();
    _lancamentoArquivo.Cliente.get$().focus();
}

//*******MÉTODOS*******
function buscarControleArquivo() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarControleArquivoClick, tamanho: "15", icone: "" };
    var auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ControleArquivo", "Codigo"), tamanho: "15", icone: "" };
    var download = { descricao: "Download", id: guid(), evento: "onclick", metodo: downloadControleArquivoClick, tamanho: "15", icone: "" };
    var enviarAlerta = { descricao: "Enviar Alerta", id: guid(), evento: "onclick", metodo: enviarAlertaClick, tamanho: "15", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [editar, download, enviarAlerta, auditar],
        tamanho: "10",
        descricao: "Opções"
    };

    var configExportacao = {
        url: "ControleArquivo/ExportarPesquisa",
        titulo: "Controle de Arquivo"
    };

    // Inicia Grid de busca
    _gridControleArquivo = new GridViewExportacao(_controleArquivo.ControleArquivo.idGrid, "ControleArquivo/Pesquisa", _pesquisaControleArquivo, menuOpcoes, configExportacao);
    _gridControleArquivo.CarregarGrid();
}

function SalvarClick(e, sender) {
    Salvar(_lancamentoArquivo, "ControleArquivo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                if (!atualizando)
                    enviarArquivosAnexados(arg.Data.Codigo);

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                _modalControleArquivo.hide();
                _gridControleArquivo.CarregarGrid();
                limparCampos();
                atualizando = false;
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, null);
}

function auditoriaClick(e) {

}

function downloadControleArquivoClick(e) {
    executarDownload("ControleArquivoAnexo/DownloadTodosAnexo", { Codigo: e.Codigo });
    _gridControleArquivo.CarregarGrid();
}

function enviarAlertaClick(e) {
    executarReST("ControleArquivo/EnviarAlerta", { Codigo: e.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alerta enviado com sucesso.");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function editarControleArquivoClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _lancamentoArquivo.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_lancamentoArquivo, "ControleArquivo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                atualizando = true;
                _lancamentoArquivo.Anexos.val(arg.Data.Anexos);
                _modalControleArquivo.show();d
                _lancamentoArquivo.Cliente.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function limparSumarizadores() {
    //_controleArquivo.Status.removeAll();
}

function limparCampos() {
    LimparCampos(_lancamentoArquivo);
    _lancamentoArquivo.Anexos.val(_lancamentoArquivo.Anexos.def);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "" };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_lancamentoArquivo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

function adicionarAnexoClick() {
    var arquivo = document.getElementById(_lancamentoArquivo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _lancamentoArquivo.DescricaoAnexo.val(),
        NomeArquivo: _lancamentoArquivo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_lancamentoArquivo.Codigo.val() > 0)
        enviarAnexos(_lancamentoArquivo.Codigo.val(), [anexo]);
    else {
        var anexos = obterAnexos();

        anexos.push(anexo);

        _lancamentoArquivo.Anexos.val(anexos.slice());
    }

    LimparCamposAnexo();

    arquivo.value = null;
}

function LimparCamposAnexo() {
    _lancamentoArquivo.DescricaoAnexo.val("");
    _lancamentoArquivo.NomeArquivo.val("");
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("ControleArquivoAnexo/DownloadAnexoOverride", { Codigo: registroSelecionado.Codigo });
    _gridControleArquivo.CarregarGrid();
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("ControleArquivoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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


function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexo(anexos);

    if (formData) {
        enviarArquivo("ControleArquivoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _lancamentoArquivo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function enviarArquivosAnexados(codigo) {
    var anexos = obterAnexos();

    enviarAnexos(codigo, anexos);
}

function obterAnexos() {
    return _lancamentoArquivo.Anexos.val().slice();
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

    _lancamentoArquivo.Anexos.val(listaAnexos);
}
