/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../Consultas/CentrosDescarregamento.js" />
/// <reference path="DatasPreferenciaisDescargaFornecedorCategoria.js" />

var _datasPreferenciaisDescarga;
var _pesquisaDatasPreferenciaisDescarga;
var _gridDatasPreferenciaisDescarga;
var _gridPesquisaDatasPreferenciaisDescarga;

var DatasPreferenciaisDescarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DiasAnterioresBloqueados = PropertyEntity({ text: "*Dias anteriores bloqueados:", val: ko.observable(0), getType: typesKnockout.int, required: true });
    this.DataPreferencial = PropertyEntity({ text: "*Dia Preferêncial:", val: ko.observable(0), getType: typesKnockout.int, required: true });
    this.CentroDescarregamento = PropertyEntity({ text: "*Centro Descarregamento:", val: ko.observable(""), type: types.entity, required: true, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.AdicionarDataPreferencial = PropertyEntity({ text: "Adicionar", idBtnSearch: guid(), eventClick: abrirModalAdicionarDataPreferencial });
    this.Adicionar = PropertyEntity({ text: "Adicionar", idBtnSearch: guid(), eventClick: adicionarDataPreferencialClick, visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ text: "Excluir", idBtnSearch: guid(), eventClick: excluirDataPreferencialClick, visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", idBtnSearch: guid(), eventClick: atualizarDataPreferencialClick, visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ text: "Limpar", idBtnSearch: guid(), eventClick: limparCamposDatasPreferenciaisDescarga, visible: ko.observable(true) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        ManterArquivoServidor: true,
        UrlImportacao: "DataPreferenciaDescarga/Importar",
        UrlConfiguracao: "DataPreferenciaDescarga/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O042_DataPreferenciaDescarga,
        CallbackImportacao: function (retorno) {
            _gridPesquisaDatasPreferenciaisDescarga.CarregarGrid();
        }
    });

    this.ListaFornecedorCategoria = PropertyEntity({ type: types.local, val: ko.observable([]), def: [] });

    this.Grid = PropertyEntity({ type: types.local });
}

var PesquisaDatasPreferenciasDescarga = function () {
    this.CentroDescarregamento = PropertyEntity({ text: "Centro Descarregamento:", val: ko.observable(""), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.DiaPreferencial = PropertyEntity({ text: "Dia Preferêncial:", val: ko.observable(0), getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarDatasPreferenciaisDescarga, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
}

function loadDatasPreferenciaisDescarga() {
    _datasPreferenciaisDescarga = new DatasPreferenciaisDescarga();
    KoBindings(_datasPreferenciaisDescarga, "knockoutDatasPreferenciaisDescarga");

    _pesquisaDatasPreferenciaisDescarga = new PesquisaDatasPreferenciasDescarga();
    KoBindings(_pesquisaDatasPreferenciaisDescarga, "knockoutPesquisaDatasPreferenciaisDescarga");

    new BuscarCentrosDescarregamento(_datasPreferenciaisDescarga.CentroDescarregamento);
    new BuscarCentrosDescarregamento(_pesquisaDatasPreferenciaisDescarga.CentroDescarregamento);

    $("#" + _datasPreferenciaisDescarga.DataPreferencial.id).on('change', function () {
        if (parseInt(_datasPreferenciaisDescarga.DataPreferencial.val()) > 30)
            _datasPreferenciaisDescarga.DataPreferencial.val("30");
    });

    loadGridDatasPreferenciaisDescarga();
    loadGridPesquisaDatasPreferenciaisDescarga();
    loadDatasPreferenciasDescargaFornecedorCategoria();
}

function loadGridPesquisaDatasPreferenciaisDescarga() {
    var configExportacao = {
        url: "DataPreferenciaDescarga/ExportarPesquisa",
        titulo: "Datas Preferenciais Descarga"
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [{
            descricao: "Editar", id: guid(),
            metodo: editarDataPreferencialDescarga
        }]
    };

    _gridPesquisaDatasPreferenciaisDescarga = new GridViewExportacao(_pesquisaDatasPreferenciaisDescarga.Pesquisar.idGrid, "DataPreferenciaDescarga/Pesquisa", _pesquisaDatasPreferenciaisDescarga, menuOpcoes, configExportacao, null, 5);
    
    pesquisarDatasPreferenciaisDescarga();
}

function loadGridDatasPreferenciaisDescarga() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirDataPreferencialDescargaFornecedorCategoria }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoFornecedor", visible: false },
        { data: "CodigoGrupoFornecedor", visible: false },
        { data: "CodigoCategoria", visible: false },
        { data: "Fornecedor", title: "Fornecedor", width: "35%" },
        { data: "GrupoFornecedor", title: "Grupo Fornecedor", width: "35%" },
        { data: "Categoria", title: "Categoria", width: "30%" }
    ];

    _gridDatasPreferenciaisDescarga = new BasicDataTable(_datasPreferenciaisDescarga.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridDatasPreferenciaisDescarga();
}

function excluirDataPreferencialDescargaFornecedorCategoria(registroSelecionado) {
    for (var i = 0; i < _datasPreferenciaisDescarga.ListaFornecedorCategoria.val().length; i++) {
        if (_datasPreferenciaisDescarga.ListaFornecedorCategoria.val()[i].Codigo == registroSelecionado.Codigo) {
            _datasPreferenciaisDescarga.ListaFornecedorCategoria.val().splice(i, 1);
            break;
        }
    }

    recarregarGridDatasPreferenciaisDescarga();
}

function editarDataPreferencialDescarga(registroSelecionado) {
    limparCamposDatasPreferenciaisDescarga();

    executarReST("DataPreferenciaDescarga/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                controlarVisibilidadeBotoes(true);

                PreencherObjetoKnout(_datasPreferenciaisDescarga, { Data: arg.Data.Dados });
                _datasPreferenciaisDescarga.ListaFornecedorCategoria.val(arg.Data.ListaFornecedorCategoria);

                recarregarGridDatasPreferenciaisDescarga();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function pesquisarDatasPreferenciaisDescarga() {
    _gridPesquisaDatasPreferenciaisDescarga.CarregarGrid();
}

function adicionarDataPreferencialClick() {
    var dados = $.extend(RetornarObjetoPesquisa(_datasPreferenciaisDescarga), { ListaFornecedorCategoria: JSON.stringify(_datasPreferenciaisDescarga.ListaFornecedorCategoria.val()) });

    if (!ValidarCamposObrigatorios(_datasPreferenciaisDescarga) || _datasPreferenciaisDescarga.ListaFornecedorCategoria.val().length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Falha", "Preencha os campos obrigatórios e adicione pelo menos um registro na tabela.");
        return;
    }

    executarReST("DataPreferenciaDescarga/Adicionar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso");
                limparCamposDatasPreferenciaisDescarga();
                _gridPesquisaDatasPreferenciaisDescarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function atualizarDataPreferencialClick() {
    var dados = $.extend(RetornarObjetoPesquisa(_datasPreferenciaisDescarga), { ListaFornecedorCategoria: JSON.stringify(_datasPreferenciaisDescarga.ListaFornecedorCategoria.val()) });

    if (!ValidarCamposObrigatorios(_datasPreferenciaisDescarga) || _datasPreferenciaisDescarga.ListaFornecedorCategoria.val().length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Falha", "Preencha os campos obrigatórios e adicione pelo menos um registro na tabela.");
        return;
    }

    executarReST("DataPreferenciaDescarga/Atualizar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposDatasPreferenciaisDescarga();
                _gridPesquisaDatasPreferenciaisDescarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function excluirDataPreferencialClick() {
    executarReST("DataPreferenciaDescarga/ExcluirPorCodigo", { Codigo: _datasPreferenciaisDescarga.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Removido com sucesso");
                limparCamposDatasPreferenciaisDescarga();
                _gridPesquisaDatasPreferenciaisDescarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function abrirModalAdicionarDataPreferencial() {
    $("#modalAdicionarDatasPreferenciasDescarga")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_datasPreferenciasFornecedorCategoria);
        });
}

function recarregarGridDatasPreferenciaisDescarga() {
    _gridDatasPreferenciaisDescarga.CarregarGrid(_datasPreferenciaisDescarga.ListaFornecedorCategoria.val());
}

function limparCamposDatasPreferenciaisDescarga() {
    _pesquisaDatasPreferenciaisDescarga.ExibirFiltros.visibleFade(false);

    controlarVisibilidadeBotoes(false);

    LimparCampos(_datasPreferenciaisDescarga);
    _datasPreferenciaisDescarga.ListaFornecedorCategoria.val([]);
    recarregarGridDatasPreferenciaisDescarga();
}

function controlarVisibilidadeBotoes(atualizando) {
    _datasPreferenciaisDescarga.Adicionar.visible(!atualizando);
    _datasPreferenciaisDescarga.Atualizar.visible(atualizando);
    _datasPreferenciaisDescarga.Excluir.visible(atualizando);
}