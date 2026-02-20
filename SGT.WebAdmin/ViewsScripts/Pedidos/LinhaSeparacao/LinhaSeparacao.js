/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Enumeradores/EnumResponsavelAvaria.js" />
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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Filial.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _LinhaSeparacao;
var _pesquisaLinhaSeparacao;
var _gridLinhaSeparacao;


var _simNao = [
    { text: "SIM", value: true },
    { text: "NÃO", value: false }
];

var LinhaSeparacao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", issue: 15, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), issue: 70, visible: ko.observable(true), required: true });
    this.Roteiriza = PropertyEntity({ text: "Roteiriza: ", val: ko.observable(true), options: _simNao, def: true });
    this.NivelPrioridade = PropertyEntity({ text: "Nível de prioridade:", getType: typesKnockout.int, val: ko.observable(""), maxlength: 50 });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(true), issue: 557, options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaLinhaSeparacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", issue: 15, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ text: "Situação: ", issue: 557, val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLinhaSeparacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PesquisaAgrupamentos = function () {
    this.Filial = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var _gridLinhaSeparacaoAgrupa;
var _linhaSeparacaoAgrupa;
var _linhasSeparacaoAgrupa = new Array();

var _pesquisaAgrupamentos;
var _linhaSeparacaoAgrupaTable;
var _gridLinhaSeparacaoAgrupaTable;

var LinhaSeparacaoAgrupa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LinhaSeparacao = PropertyEntity({ text: ko.observable(""), val: ko.observable(""), getType: typesKnockout.string });
    this.Filial = PropertyEntity({ text: ko.observable(""), getType: typesKnockout.string });
    this.Grid = PropertyEntity({ type: types.local });
    this.Agrupar = PropertyEntity({ type: types.event, text: "Adicionar linhas separação", idBtnSearch: guid(), issue: 53 });
    this.Salvar = PropertyEntity({ eventClick: salvarAgrupamentosClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

var LinhaSeparacaoAgrupaTable = function () {
    this.Filial = PropertyEntity({ text: ko.observable(""), getType: typesKnockout.string });
    this.Grid = PropertyEntity({ type: types.local });
}

//*******EVENTOS*******
function loadLinhaSeparacao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaLinhaSeparacao = new PesquisaLinhaSeparacao();
    KoBindings(_pesquisaLinhaSeparacao, "knockoutPesquisaLinhaSeparacao", false, _pesquisaLinhaSeparacao.Pesquisar.id);

    new BuscarFilial(_pesquisaLinhaSeparacao.Filial);

    _LinhaSeparacao = new LinhaSeparacao();
    KoBindings(_LinhaSeparacao, "knockoutLinhaSeparacao");

    HeaderAuditoria("LinhaSeparacao", _LinhaSeparacao);

    new BuscarFilial(_LinhaSeparacao.Filial);

    //linhaSeparacaoAgrupa();

    linhaSeparacaoAgrupaTable();

    // Inicia busca
    buscarLinhaSeparacao();
}

function linhaSeparacaoAgrupa() {
    _linhaSeparacaoAgrupa = new LinhaSeparacaoAgrupa();
    KoBindings(_linhaSeparacaoAgrupa, "knoutAgrupamentos");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirLinhaSeparacaoAgrupa(_linhaSeparacaoAgrupa.Agrupar, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridLinhaSeparacaoAgrupa = new BasicDataTable(_linhaSeparacaoAgrupa.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLinhasSeparacao(_linhaSeparacaoAgrupa.Agrupar, function (r) {
        if (r != null) {
            for (var i = 0; i < r.length; i++)
                _linhasSeparacaoAgrupa.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

            _gridLinhaSeparacaoAgrupa.CarregarGrid(_linhasSeparacaoAgrupa);
        }
    }, _gridLinhaSeparacaoAgrupa);
    _linhaSeparacaoAgrupa.Agrupar.basicTable = _gridLinhaSeparacaoAgrupa;

}

function linhaSeparacaoAgrupaTable() {

    _pesquisaAgrupamentos = new PesquisaAgrupamentos();
    _linhaSeparacaoAgrupaTable = new LinhaSeparacaoAgrupaTable();
    KoBindings(_linhaSeparacaoAgrupaTable, "knoutAgrupamentosTable");

    var quantidadePorPagina = 1000;

    var inforEditarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarAgrupamentoTable
    };

    _gridLinhaSeparacaoAgrupaTable = new GridView(_linhaSeparacaoAgrupaTable.Grid.id, "LinhaSeparacao/Agrupamentos", _pesquisaAgrupamentos, null, null, 100, null, null, null, null, quantidadePorPagina, inforEditarColuna, null, null, null, null);
    _gridLinhaSeparacaoAgrupaTable.SetScrollHorizontal(true);
    _gridLinhaSeparacaoAgrupaTable.setTamanhoPadraoPorColuna(25);

    reloadGridAgrupamentosTable(false);
}

function AtualizarAgrupamentoTable(dataRow, row, head, callbackTabPress) {

    var ls1 = dataRow.Codigo;
    var nameLS2 = head.data;
    var value = dataRow[nameLS2];
    var ls2 = nameLS2.replace("LS_", '');

    if (ls1 != ls2) {

        var dados = {
            CodigoLS1: ls1,
            CodigoLS2: ls2,
            Agrupa: value
        };

        executarReST("LinhaSeparacao/SalvarAgrupamento", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data === false) {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                } else {
                    reloadGridAgrupamentosTable(true);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    }

}

function reloadGridAgrupamentosTable(modal) {
    _gridLinhaSeparacaoAgrupaTable.CarregarGrid();

    if (modal)
        Global.abrirModal("divModalAgrupamentosTable");
}

function RecarregarGridLinhasSeparacaoAgrupa() {
    _gridLinhaSeparacaoAgrupa.CarregarGrid(_linhasSeparacaoAgrupa);
    Global.abrirModal("divModalAgrupamentos");
}

function adicionarClick(e, sender) {
    Salvar(_LinhaSeparacao, "LinhaSeparacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridLinhaSeparacao.CarregarGrid();
                limparCamposLinhaSeparacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_LinhaSeparacao, "LinhaSeparacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridLinhaSeparacao.CarregarGrid();
                limparCamposLinhaSeparacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_LinhaSeparacao, "LinhaSeparacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridLinhaSeparacao.CarregarGrid();
                    limparCamposLinhaSeparacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposLinhaSeparacao();
}

function editarLinhaSeparacaoClick(itemGrid) {
    // Limpa os campos
    limparCamposLinhaSeparacao();

    // Seta o codigo do ProdutoAvaria
    _LinhaSeparacao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_LinhaSeparacao, "LinhaSeparacao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaLinhaSeparacao.ExibirFiltros.visibleFade(false);
                // Alternas os campos de CRUD
                _LinhaSeparacao.Atualizar.visible(true);
                _LinhaSeparacao.Excluir.visible(true);
                _LinhaSeparacao.Cancelar.visible(true);
                _LinhaSeparacao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


//*******MÉTODOS*******
function buscarLinhaSeparacao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLinhaSeparacaoClick, tamanho: "10", icone: "" };
    //var agrupa = { descricao: "Agrupamentos carregamento", id: guid(), metodo: agrupamentoClick, icone: "", title: "Permite relacionar quais linhas de separação são agrupadas na montagem de carregamento." };
    var agrupaTable = { descricao: "Agrupamentos", id: guid(), metodo: agrupamentoTableClick, icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 15,
        opcoes: [editar, /*agrupa,*/ agrupaTable]
    };

    var configExportacao = {
        url: "LinhaSeparacao/ExportarPesquisa",
        titulo: "Linha Separação"
    };

    // Inicia Grid de busca
    _gridLinhaSeparacao = new GridViewExportacao(_pesquisaLinhaSeparacao.Pesquisar.idGrid, "LinhaSeparacao/Pesquisa", _pesquisaLinhaSeparacao, menuOpcoes, configExportacao);
    _gridLinhaSeparacao.CarregarGrid();

}

function limparCamposLinhaSeparacao() {
    _LinhaSeparacao.Atualizar.visible(false);
    _LinhaSeparacao.Cancelar.visible(false);
    _LinhaSeparacao.Excluir.visible(false);
    _LinhaSeparacao.Adicionar.visible(true);
    LimparCampos(_LinhaSeparacao);
}

function agrupamentoClick(data) {

    _linhaSeparacaoAgrupa.Codigo.val(data.Codigo);
    _linhaSeparacaoAgrupa.LinhaSeparacao.text(data.Descricao);
    _linhaSeparacaoAgrupa.Filial.text(data.Filial);

    var dados = { Codigo: data.Codigo };
    executarReST("LinhaSeparacao/PesquisaAgrupa", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _linhasSeparacaoAgrupa = new Array();
                _linhasSeparacaoAgrupa.push({ Codigo: data.Codigo, Descricao: data.Descricao });

                var r = arg.Data;
                for (var i = 0; i < r.length; i++)
                    _linhasSeparacaoAgrupa.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

                RecarregarGridLinhasSeparacaoAgrupa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function agrupamentoTableClick(data) {

    _linhaSeparacaoAgrupaTable.Filial.text(data.Filial);
    _pesquisaAgrupamentos.Filial.val(data.CodigoFilial);
    reloadGridAgrupamentosTable(true);

    //var dados = { Filial: data.CodigoFilial };
    //executarReST("LinhaSeparacao/Agrupamentos", dados, function (arg) {
    //    if (arg.Success) {
    //        if (arg.Data !== false) {
    //            _linhasSeparacaoAgrupa = new Array();
    //            _linhasSeparacaoAgrupa.push({ Codigo: data.Codigo, Descricao: data.Descricao });

    //            var r = arg.Data;
    //            for (var i = 0; i < r.length; i++)
    //                _linhasSeparacaoAgrupa.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

    //            RecarregarGridLinhasSeparacaoAgrupa();
    //        }
    //        else
    //            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
    //    } else {
    //        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    //    }
    //}, null);
}

function excluirLinhaSeparacaoAgrupa(knoutLinhaSeparacaoAgrupa, data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o agrupamento: " + data.Descricao + " ?", function () {
        var agrupaGrid = knoutLinhaSeparacaoAgrupa.basicTable.BuscarRegistros();
        for (var i = 0; i < agrupaGrid.length; i++) {
            if (data.Codigo == agrupaGrid[i].Codigo) {
                agrupaGrid.splice(i, 1);
                break;
            }
        }
        knoutLinhaSeparacaoAgrupa.basicTable.CarregarGrid(agrupaGrid);
    });
}

function salvarAgrupamentosClick() {
    var registros = _linhaSeparacaoAgrupa.Agrupar.basicTable.BuscarRegistros();
    var dados = {
        Codigo: _linhaSeparacaoAgrupa.Codigo.val(),
        Agrupar: JSON.stringify(registros)
    };
    executarReST("LinhaSeparacao/SalvarAgrupa", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                Global.fecharModal("divModalAgrupamentos");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}