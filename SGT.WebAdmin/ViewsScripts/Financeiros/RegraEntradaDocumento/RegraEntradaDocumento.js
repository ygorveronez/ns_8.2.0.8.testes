/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/CFOP.js" />
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
/// <reference path="../../Enumeradores/EnumIndicadorPagamentoDoucumentoEntrada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraEntradaDocumento;
var _regraEntradaDocumento;
var _pesquisaRegraEntradaDocumento;

var _gridNCM;
var _ncm;
var NCMMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.NCM = PropertyEntity({ type: types.map, val: "" });
};

var _gridFornecedor;
var _fornecedor;
var FornecedorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoFornecedor = PropertyEntity({ type: types.map, val: "" });
    this.Fornecedor = PropertyEntity({ type: types.map, val: "" });
};

var PesquisaRegraEntradaDocumento = function () {
    this.NCM = PropertyEntity({ text: "NCM: " });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa / Fornecedor:", idBtnSearch: guid() });
    this.NaturezaDaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza da Operação:", idBtnSearch: guid() });
    this.CFOPDentro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Dentro do Estado:", idBtnSearch: guid() });
    this.CFOPFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Fora do Estado:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Descricao = PropertyEntity({ text: "Descrição: ", types: types.text });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraEntradaDocumento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var RegraEntradaDocumento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 1000 });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Pessoa / Fornecedor:"), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.NaturezaDaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza da Operação:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.CFOPDentro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Dentro do Estado:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.CFOPFora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Fora do Estado:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.IndicadorPagamento = PropertyEntity({ val: ko.observable(EnumIndicadorPagamentoDocumentoEntrada.APrazo), options: EnumIndicadorPagamentoDocumentoEntrada.obterOpcoes(), def: EnumIndicadorPagamentoDocumentoEntrada.APrazo, text: "*Indicador de Pagamento: ", enable: ko.observable(true) });

    //Checkbox
    this.ObrigarInformarVeiculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Obrigar informar o veículo/equipamento nos itens?", def: false });
    this.FinalizarFaturarNotaAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Finalizar e faturar a nota automaticamente se todos os dados estiverem preenchidos?", def: false });
    this.TributarICMS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não Triburar ICMS?", def: false });
    this.MultiplosFornecedores = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Habilita múltiplos fornecedores a regra?", def: false, eventChange: MultiplosFornecedoresChange });
    this.NaoFinalizarQuandoArlaEstiverAssociadaReboqueEquipamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não finalizar quando uma ARLA estiver associada a Reboque ou Equipamento?", def: false });

    this.NaoFinalizarQuandoArlaTiverQuantidadeSuperior = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não finalizar quando uma ARLA estiver com quantidade superior?", def: false });
    this.QuantidadeSuperiorArla = PropertyEntity({ text: "*Quantidade superior a:", getType: typesKnockout.decimal, maxlength: 11, configDecimal: { precision: 4, allowZero: false, allowNegative: false }, required: ko.observable(false), visible: ko.observable(false) });
    this.NaoFinalizarQuandoArlaTiverQuantidadeSuperior.val.subscribe(function (novoValor) {
        _regraEntradaDocumento.QuantidadeSuperiorArla.required(novoValor);
        _regraEntradaDocumento.QuantidadeSuperiorArla.visible(novoValor);
        if (!novoValor)
            _regraEntradaDocumento.QuantidadeSuperiorArla.val("");
    });

    this.NaoFinalizarDocumentoSemProdutoPreCadastrado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não finalizar documento se não tiver produtos pré-cadastrados.", def: false });
    this.ListaNCM = PropertyEntity({ type: types.listEntity, list: new Array(), text: "NCM:", codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid() });
    this.NCM = PropertyEntity({ text: "*NCM: ", required: ko.observable(false), val: ko.observable(""), maxlength: 8 });
    this.AdicionarNCM = PropertyEntity({ eventClick: adicionarNCMClick, type: types.event, text: "Adicionar NCM", visible: ko.observable(true) });

    this.ListaFornecedor = PropertyEntity({ type: types.listEntity, list: new Array(), text: "Fornecedor:", codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid() });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa / Fornecedor:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.AdicionarFornecedor = PropertyEntity({ eventClick: adicionarFornecedorClick, type: types.event, text: "Adicionar Fornecedor", visible: ko.observable(true) });

    this.FornecedorFiltro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa / Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.FornecedorCNPJFiltro = PropertyEntity({ getType: typesKnockout.cnpj, text: "CNPJ Fornecedor:", visible: ko.observable(true) });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            recarregarGridFornecedor();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadRegraEntradaDocumento() {

    _regraEntradaDocumento = new RegraEntradaDocumento();
    KoBindings(_regraEntradaDocumento, "knockoutCadastroRegraEntradaDocumento");

    HeaderAuditoria("RegraEntradaDocumento", _regraEntradaDocumento);

    _pesquisaRegraEntradaDocumento = new PesquisaRegraEntradaDocumento();
    KoBindings(_pesquisaRegraEntradaDocumento, "knockoutPesquisaRegraEntradaDocumento", false, _pesquisaRegraEntradaDocumento.Pesquisar.id);

    new BuscarEmpresa(_regraEntradaDocumento.Empresa);
    new BuscarClientes(_regraEntradaDocumento.Fornecedor);
    new BuscarClientes(_regraEntradaDocumento.FornecedorFiltro);
    new BuscarClientes(_regraEntradaDocumento.Pessoa);
    new BuscarNaturezasOperacoesNotaFiscal(_regraEntradaDocumento.NaturezaDaOperacao);
    new BuscarCFOPNotaFiscal(_regraEntradaDocumento.CFOPDentro, null, null, true, null, null, null, null, null, _regraEntradaDocumento.NaturezaDaOperacao);
    new BuscarCFOPNotaFiscal(_regraEntradaDocumento.CFOPFora, null, null, false, null, null, null, null, null, _regraEntradaDocumento.NaturezaDaOperacao);

    new BuscarEmpresa(_pesquisaRegraEntradaDocumento.Empresa);
    new BuscarClientes(_pesquisaRegraEntradaDocumento.Pessoa);
    new BuscarNaturezasOperacoesNotaFiscal(_pesquisaRegraEntradaDocumento.NaturezaDaOperacao);
    new BuscarCFOPNotaFiscal(_pesquisaRegraEntradaDocumento.CFOPDentro);
    new BuscarCFOPNotaFiscal(_pesquisaRegraEntradaDocumento.CFOPFora);

    $("#liTabFornecedores").hide();
    buscarRegraEntradaDocumentos();
    loadNCM();
    loadFornecedor();
}

function MultiplosFornecedoresChange(e, sender) {
    if ($("#" + _regraEntradaDocumento.MultiplosFornecedores.id).is(':checked')) {
        $("#liTabFornecedores").show();
        _regraEntradaDocumento.Pessoa.required(false);
        _regraEntradaDocumento.Pessoa.text("Pessoa / Fornecedor");
    }
    else {
        $("#liTabFornecedores").hide();
        _regraEntradaDocumento.Pessoa.required(true);
        _regraEntradaDocumento.Pessoa.text("*Pessoa / Fornecedor");
    }
}

function adicionarNCMClick(e, sender) {
    _regraEntradaDocumento.NCM.requiredClass("form-control");
    var tudoCerto = false;
    tudoCerto = _regraEntradaDocumento.NCM.val() != "";
    if (tudoCerto) {
        var existe = false;
        $.each(_regraEntradaDocumento.ListaNCM.list, function (i, NCM) {
            if (NCM.NCM.val == _regraEntradaDocumento.NCM.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _ncm = new NCMMap();
            _ncm.Codigo.val = guid();
            _ncm.NCM.val = _regraEntradaDocumento.NCM.val();

            _regraEntradaDocumento.ListaNCM.list.push(_ncm);
            recarregarGridNCM();
            $("#" + _regraEntradaDocumento.NCM.id).focus();
            _regraEntradaDocumento.NCM.requiredClass("form-control");
        } else {
            exibirMensagem("aviso", "NCM já informado", "O NCM " + _regraEntradaDocumento.NCM.val() + " já foi informado para esta regra.");
        }
        _regraEntradaDocumento.NCM.val("");
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
        _regraEntradaDocumento.NCM.requiredClass("form-control is-invalid");
    }
}

function adicionarFornecedorClick(e, sender) {
    _regraEntradaDocumento.Fornecedor.requiredClass("form-control");
    var tudoCerto = false;
    tudoCerto = _regraEntradaDocumento.Fornecedor.val() != "";
    if (tudoCerto) {
        var existe = false;
        $.each(_regraEntradaDocumento.ListaFornecedor.list, function (i, Fornecedor) {
            if (Fornecedor.CodigoFornecedor.val == _regraEntradaDocumento.Fornecedor.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _fornecedor = new FornecedorMap();
            _fornecedor.Codigo.val = guid();
            _fornecedor.CodigoFornecedor.val = _regraEntradaDocumento.Fornecedor.codEntity();
            _fornecedor.Fornecedor.val = _regraEntradaDocumento.Fornecedor.val();

            _regraEntradaDocumento.ListaFornecedor.list.push(_fornecedor);
            recarregarGridFornecedor();
            $("#" + _regraEntradaDocumento.Fornecedor.id).focus();
            _regraEntradaDocumento.Fornecedor.requiredClass("form-control");
        } else {
            exibirMensagem("aviso", "Fornecedor já informado", "O Fornecedor " + _regraEntradaDocumento.Fornecedor.val() + " já foi informado para esta regra.");
        }
        _regraEntradaDocumento.Fornecedor.val("");
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
        _regraEntradaDocumento.Fornecedor.requiredClass("form-control is-invalid");
    }
}

function excluirNCMClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o NCM " + data.NCM + "?", function () {
        var listaAtualizada = new Array();
        $.each(_regraEntradaDocumento.ListaNCM.list, function (i, NCM) {
            if (NCM.Codigo.val != data.Codigo) {
                listaAtualizada.push(NCM);
            }
        });
        _regraEntradaDocumento.ListaNCM.list = listaAtualizada;
        recarregarGridNCM();
    });
}

function excluirFornecedorClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Fornecedor " + data.Fornecedor + "?", function () {
        var listaAtualizada = new Array();
        $.each(_regraEntradaDocumento.ListaFornecedor.list, function (i, Fornecedor) {
            if (Fornecedor.Codigo.val != data.Codigo) {
                listaAtualizada.push(Fornecedor);
            }
        });
        _regraEntradaDocumento.ListaFornecedor.list = listaAtualizada;
        recarregarGridFornecedor();
    });
}

function adicionarClick(e, sender) {
    Salvar(e, "RegraEntradaDocumento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridRegraEntradaDocumento.CarregarGrid();
                limparCamposRegraEntradaDocumento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "RegraEntradaDocumento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRegraEntradaDocumento.CarregarGrid();
                limparCamposRegraEntradaDocumento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a regra de entrada?", function () {
        ExcluirPorCodigo(_regraEntradaDocumento, "RegraEntradaDocumento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraEntradaDocumento.CarregarGrid();
                    limparCamposRegraEntradaDocumento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRegraEntradaDocumento();
}

//*******MÉTODOS*******

function loadNCM() {
    var editar = { descricao: "Remover", id: guid(), evento: "onclick", metodo: excluirNCMClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "NCM", title: "NCM", width: "90%", className: "text-align-left" }
    ];
    _gridNCM = new BasicDataTable(_regraEntradaDocumento.ListaNCM.idGrid, header, menuOpcoes);
    recarregarGridNCM();
}

function loadFornecedor() {
    var editar = { descricao: "Remover", id: guid(), evento: "onclick", metodo: excluirFornecedorClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoFornecedor", visible: false },
        { data: "Fornecedor", title: "Fornecedor", width: "90%", className: "text-align-left" }
    ];
    _gridFornecedor = new BasicDataTable(_regraEntradaDocumento.ListaFornecedor.idGrid, header, menuOpcoes);
    recarregarGridFornecedor();
}

function recarregarGridNCM() {
    var data = new Array();
    $.each(_regraEntradaDocumento.ListaNCM.list, function (i, NCM) {
        var obj = new Object();
        obj.Codigo = NCM.Codigo.val;
        obj.NCM = NCM.NCM.val;
        data.push(obj);
    });
    _gridNCM.CarregarGrid(data);
}

function recarregarGridFornecedor() {
    var data = new Array();

    var listaFiltrada = _regraEntradaDocumento.ListaFornecedor.list;
    if (_regraEntradaDocumento.FornecedorFiltro.codEntity() > 0) {
        listaFiltrada = listaFiltrada.filter(function (arg) {
            return arg.CodigoFornecedor.val.includes(_regraEntradaDocumento.FornecedorFiltro.codEntity());
        });
    }

    if (_regraEntradaDocumento.FornecedorCNPJFiltro.val() != "") {
        listaFiltrada = listaFiltrada.filter(function (arg) {
            return _regraEntradaDocumento.FornecedorCNPJFiltro.val().replace(/\D/g, "").includes(arg.CodigoFornecedor.val);
        });
    }

    $.each(listaFiltrada, function (i, Fornecedor) {
        var obj = new Object();
        obj.Codigo = Fornecedor.Codigo.val;
        obj.CodigoFornecedor = Fornecedor.CodigoFornecedor.val;
        obj.Fornecedor = Fornecedor.Fornecedor.val;
        data.push(obj);
    });
    _gridFornecedor.CarregarGrid(data);
}

function buscarRegraEntradaDocumentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraEntradaDocumento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraEntradaDocumento = new GridView(_pesquisaRegraEntradaDocumento.Pesquisar.idGrid, "RegraEntradaDocumento/Pesquisa", _pesquisaRegraEntradaDocumento, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridRegraEntradaDocumento.CarregarGrid();
}

function editarRegraEntradaDocumento(RegraEntradaDocumentoGrid) {
    limparCamposRegraEntradaDocumento();
    _regraEntradaDocumento.Codigo.val(RegraEntradaDocumentoGrid.Codigo);
    BuscarPorCodigo(_regraEntradaDocumento, "RegraEntradaDocumento/BuscarPorCodigo", function (arg) {
        _pesquisaRegraEntradaDocumento.ExibirFiltros.visibleFade(false);
        _regraEntradaDocumento.Atualizar.visible(true);
        _regraEntradaDocumento.Cancelar.visible(true);
        _regraEntradaDocumento.Excluir.visible(true);
        _regraEntradaDocumento.Adicionar.visible(false);
        recarregarGridNCM();
        recarregarGridFornecedor();
        MultiplosFornecedoresChange();
    }, null);
}

function limparCamposRegraEntradaDocumento() {
    _regraEntradaDocumento.Atualizar.visible(false);
    _regraEntradaDocumento.Cancelar.visible(false);
    _regraEntradaDocumento.Excluir.visible(false);
    _regraEntradaDocumento.Adicionar.visible(true);
    LimparCampos(_regraEntradaDocumento);

    _regraEntradaDocumento.Pessoa.required(true);
    _regraEntradaDocumento.Pessoa.text("*Pessoa / Fornecedor");

    recarregarGridNCM();
    recarregarGridFornecedor();
    MultiplosFornecedoresChange();
    Global.ResetarAbas();
}
