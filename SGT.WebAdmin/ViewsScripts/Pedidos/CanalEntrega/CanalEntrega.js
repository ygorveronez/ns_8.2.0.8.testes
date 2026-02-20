/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
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


//*******MAPEAMENTO KNOUCKOUT*******

var _canalEntrega;
var _pesquisaCanalEntrega;
var _gridCanalEntrega;


var CanalEntrega = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pedidos.CanalEntrega.CodigoIntegracao.getFieldDescription(), issue: 15, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.NivelPrioridade = PropertyEntity({ text: Localization.Resources.Pedidos.CanalEntrega.NivelPrioridade.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), maxlength: 50 });
    this.Ativo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), val: ko.observable(true), issue: 557, options: _status, def: true });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.Principal = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Pedidos.CanalEntrega.EsteCanalEntregaPrincipal });
    this.CanalEntregaPrincipal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pedidos.CanalEntrega.CanalEntregaPrincipal.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(false), required: ko.observable(false) });
    this.GerarCargaAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.CanalEntrega.GerarCargaAutomaticamente, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.LiberarPedidoSemNFeAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.CanalEntrega.LiberarPedidosSemNFeAutomaticamente, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.QuantidadePedidosPermitidosNoCanal = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Pedidos.CanalEntrega.QuantidadePedidosPermitidosCanal.getFieldDescription(), val: ko.observable(0), def: 0 });
    this.NaoUtilizarCapacidadeVeiculoMontagemCarga = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Pedidos.CanalEntrega.EsteCanalUtilizaCapacidadeVeiculoMontagemCarga, visible: ko.observable(true) })
    this.Circuito = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.CanalEntrega.Circuito, visible: ko.observable(true) })

    this.Principal.val.subscribe(function (novoValor) {
        //_canalEntrega.CanalEntregaPrincipal.visible(!novoValor);
        _canalEntrega.CanalEntregaPrincipal.enable(!novoValor);
        _canalEntrega.CanalEntregaPrincipal.required(!novoValor);
        _canalEntrega.CanalEntregaPrincipal.text(Localization.Resources.Pedidos.CanalEntrega.CanalEntregaPrincipal.getRequiredFieldDescription());
        if (novoValor) {
            _canalEntrega.CanalEntregaPrincipal.codEntity(0);
            _canalEntrega.CanalEntregaPrincipal.val("");
            _canalEntrega.CanalEntregaPrincipal.text(Localization.Resources.Pedidos.CanalEntrega.CanalEntregaPrincipal.getFieldDescription());
        }
    });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

var PesquisaCanalEntrega = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), issue: 15, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Ativo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), issue: 557, val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid() });
    
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCanalEntrega.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadCanalEntrega() {

    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCanalEntrega = new PesquisaCanalEntrega();
    KoBindings(_pesquisaCanalEntrega, "knockoutPesquisaCanalEntrega", false, _pesquisaCanalEntrega.Pesquisar.id);

    new BuscarFilial(_pesquisaCanalEntrega.Filial);

    // Instancia canal entrega
    _canalEntrega = new CanalEntrega();
    KoBindings(_canalEntrega, "knockoutCanalEntrega");

    HeaderAuditoria("CanalEntrega", _canalEntrega);

    new BuscarFilial(_canalEntrega.Filial);
    new BuscarCanaisEntrega(_canalEntrega.CanalEntregaPrincipal, null, null, null, true, _canalEntrega.Filial);   
    
    // Inicia busca
    buscarCanalEntrega();
}
function adicionarClick(e, sender) {
    Salvar(_canalEntrega, "CanalEntrega/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridCanalEntrega.CarregarGrid();
                limparCamposCanalEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_canalEntrega, "CanalEntrega/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridCanalEntrega.CarregarGrid();
                limparCamposCanalEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirORegistro, function () {
        ExcluirPorCodigo(_canalEntrega, "CanalEntrega/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridCanalEntrega.CarregarGrid();
                    limparCamposCanalEntrega();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCanalEntrega();
}

function editarCanalEntregaClick(itemGrid) {
    // Limpa os campos
    limparCamposCanalEntrega();

    // Seta o codigo do ProdutoAvaria
    _canalEntrega.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_canalEntrega, "CanalEntrega/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaCanalEntrega.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _canalEntrega.Atualizar.visible(true);
                _canalEntrega.Excluir.visible(true);
                _canalEntrega.Cancelar.visible(true);
                _canalEntrega.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarCanalEntrega() {
    //-- Grid
    // Opcoes
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarCanalEntregaClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "CanalEntrega/ExportarPesquisa",
        titulo: Localization.Resources.Pedidos.CanalEntrega.MotivoAvaria
    };


    // Inicia Grid de busca
    _gridCanalEntrega = new GridViewExportacao(_pesquisaCanalEntrega.Pesquisar.idGrid, "CanalEntrega/Pesquisa", _pesquisaCanalEntrega, menuOpcoes, configExportacao);
    _gridCanalEntrega.CarregarGrid();
}

function limparCamposCanalEntrega() {
    _canalEntrega.Atualizar.visible(false);
    _canalEntrega.Cancelar.visible(false);
    _canalEntrega.Excluir.visible(false);
    _canalEntrega.Adicionar.visible(true);
    LimparCampos(_canalEntrega);
}