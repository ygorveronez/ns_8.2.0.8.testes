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
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPedidoSimplificado;
var _pedidoSimplificado;
var _pesquisaPedidoSimplificado;

var PesquisaPedidoSimplificado = function () {
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo de Remetente: ", issue: 306, eventChange: TipoPessoaPesquisaChange, visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPedido.Aberto), options: EnumSituacaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPedido.Aberto, text: "Situação: " });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataColeta = PropertyEntity({ text: "Data Coleta: ", getType: typesKnockout.date });
    this.NumeroPedidoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número Pedido no Embarcador:", issue: 902 });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), def: "", text: "Número do Pedido:", configInt: { precision: 0, allowZero: true }, getType: typesKnockout.int, visible: ko.observable(true) });
    this.NotaFiscal = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da NF:", configInt: { precision: 0, allowZero: false }, getType: typesKnockout.int, visible: ko.observable(true) });

    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da Carga:" });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", issue: 145, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.CidadePoloOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Polo de Origem:", issue: 831, idBtnSearch: guid() });
    this.CidadePoloDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Polo de Destino:", issue: 831, idBtnSearch: guid() });
    this.PaisOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "País de Origem:", idBtnSearch: guid() });
    this.PaisDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "País de Destino:", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal de Entrega:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.FuncionarioVendedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Vendedor:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoSimplificado.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PedidoSimplificado = function () {
    var data = Global.DataHora(EnumTipoOperacaoDate.Add, 2, 'd');

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataColeta = PropertyEntity({ text: "*Data Coleta ", getType: typesKnockout.dateTime, required: true, issue: 2, enable: false, val: ko.observable(data), def: data });
    this.PalletsFracionado = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, text: "*Nº Pilha:", required: true });
    this.ObservacaoInterna = PropertyEntity({ text: "Observação Interna: ", maxlength: 2000, required: false });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Remetente:", issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Destinatário:", issue: 52, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo de Operação:", issue: 121, idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Modelo Veicular da Carga:", issue: 44, idBtnSearch: guid() });
};

var CRUDPedidoSimplificado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadPedidoSimplificado() {
    _pedidoSimplificado = new PedidoSimplificado();
    KoBindings(_pedidoSimplificado, "knockoutCadastroPedidoSimplificado");

    HeaderAuditoria("Pedido", _pedidoSimplificado);

    _crudPedidoSimplificado = new CRUDPedidoSimplificado();
    KoBindings(_crudPedidoSimplificado, "knockoutCRUDPedidoSimplificado");

    _pesquisaPedidoSimplificado = new PesquisaPedidoSimplificado();
    KoBindings(_pesquisaPedidoSimplificado, "knockoutPesquisaPedidoSimplificado", false, _pesquisaPedidoSimplificado.Pesquisar.id);

    new BuscarClientes(_pesquisaPedidoSimplificado.Remetente);
    new BuscarGruposPessoas(_pesquisaPedidoSimplificado.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarLocalidades(_pesquisaPedidoSimplificado.Origem);
    new BuscarClientes(_pesquisaPedidoSimplificado.Destinatario);
    new BuscarClientes(_pesquisaPedidoSimplificado.Expedidor);
    new BuscarClientes(_pesquisaPedidoSimplificado.Recebedor);
    new BuscarClientes(_pesquisaPedidoSimplificado.Tomador);
    new BuscarVeiculos(_pesquisaPedidoSimplificado.Veiculo);
    new BuscarMotorista(_pesquisaPedidoSimplificado.Motorista);
    new BuscarLocalidades(_pesquisaPedidoSimplificado.Destino);
    new BuscarLocalidadesPolo(_pesquisaPedidoSimplificado.CidadePoloOrigem);
    new BuscarLocalidadesPolo(_pesquisaPedidoSimplificado.CidadePoloDestino);
    new BuscarPaises(_pesquisaPedidoSimplificado.PaisOrigem);
    new BuscarPaises(_pesquisaPedidoSimplificado.PaisDestino);
    new BuscarTiposdeCarga(_pesquisaPedidoSimplificado.TipoCarga);
    new BuscarCanaisEntrega(_pesquisaPedidoSimplificado.CanalEntrega);
    new BuscarTiposOperacao(_pesquisaPedidoSimplificado.TipoOperacao);
    new BuscarFuncionario(_pesquisaPedidoSimplificado.FuncionarioVendedor);

    new BuscarClientes(_pedidoSimplificado.Remetente);
    new BuscarClientes(_pedidoSimplificado.Destinatario);
    new BuscarTiposOperacao(_pedidoSimplificado.TipoOperacao);
    new BuscarModelosVeicularesCarga(_pedidoSimplificado.ModeloVeicularCarga);

    buscarPedidoSimplificado();
}

function TipoPessoaPesquisaChange() {
    if (_pesquisaPedidoSimplificado.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        _pesquisaPedidoSimplificado.Remetente.visible(true);
        _pesquisaPedidoSimplificado.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaPedidoSimplificado.GrupoPessoa);
    }
    else if (_pesquisaPedidoSimplificado.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _pesquisaPedidoSimplificado.GrupoPessoa.visible(true);
        _pesquisaPedidoSimplificado.Remetente.visible(false);
        LimparCampoEntity(_pesquisaPedidoSimplificado.Remetente);
    }
}

function adicionarClick(e, sender) {
    Salvar(_pedidoSimplificado, "PedidoSimplificado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var dadosRetorno = "";
                if (arg.Data.Numero !== "")
                    dadosRetorno = "Pedido " + arg.Data.Numero;
                if (arg.Data.NumeroCarga !== "")
                    dadosRetorno += ", Carga " + arg.Data.NumeroCarga;

                exibirMensagem(tipoMensagem.ok, "Sucesso", dadosRetorno + " cadastrado com sucesso");
                _gridPedidoSimplificado.CarregarGrid();
                limparCamposPedidoSimplificado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_pedidoSimplificado, "PedidoSimplificado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPedidoSimplificado.CarregarGrid();
                limparCamposPedidoSimplificado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o pedido?", function () {
        ExcluirPorCodigo(_pedidoSimplificado, "PedidoSimplificado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPedidoSimplificado.CarregarGrid();
                    limparCamposPedidoSimplificado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPedidoSimplificado();
}

//*******MÉTODOS*******


function buscarPedidoSimplificado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPedidoSimplificado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPedidoSimplificado = new GridView(_pesquisaPedidoSimplificado.Pesquisar.idGrid, "PedidoSimplificado/Pesquisa", _pesquisaPedidoSimplificado, menuOpcoes, null);
    _gridPedidoSimplificado.CarregarGrid();
}

function editarPedidoSimplificado(pedidoSimplificadoGrid) {
    limparCamposPedidoSimplificado();
    _pedidoSimplificado.Codigo.val(pedidoSimplificadoGrid.Codigo);
    BuscarPorCodigo(_pedidoSimplificado, "PedidoSimplificado/BuscarPorCodigo", function (arg) {
        _pesquisaPedidoSimplificado.ExibirFiltros.visibleFade(false);
        _crudPedidoSimplificado.Atualizar.visible(true);
        _crudPedidoSimplificado.Cancelar.visible(true);
        _crudPedidoSimplificado.Excluir.visible(true);
        _crudPedidoSimplificado.Adicionar.visible(false);
    }, null);
}

function limparCamposPedidoSimplificado() {
    _crudPedidoSimplificado.Atualizar.visible(false);
    _crudPedidoSimplificado.Cancelar.visible(false);
    _crudPedidoSimplificado.Excluir.visible(false);
    _crudPedidoSimplificado.Adicionar.visible(true);
    LimparCampos(_pedidoSimplificado);
    _pedidoSimplificado.DataColeta.val(Global.DataHora(EnumTipoOperacaoDate.Add, 2, 'd'));
}