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

var _gridPorto;
var _porto;
var _pesquisaPorto;

var PesquisaPorto = function () {

    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPorto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Porto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.CodigoIATA = PropertyEntity({ text: "*Sigla Porto:", required: ko.observable(true), maxlength: 50 });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cidade:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", required: ko.observable(false), maxlength: 50 });
    this.FormaEmissaoSVM = PropertyEntity({ val: ko.observable(EnumFormaEmissaoSVM.Nenhum), options: EnumFormaEmissaoSVM.obterOpcoes(), def: EnumFormaEmissaoSVM.Nenhum, text: "*Emissão de UM Container por SVM: ", required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoDocumento = PropertyEntity({ text: "Cod. Documentação: ", required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.CodigoMercante = PropertyEntity({ text: "Cod. Mercante: ", required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.QuantidadeHorasFaturamentoAutomatico = PropertyEntity({ text: "Qtd. Horas Faturamento Automático: ", getType: typesKnockout.int, required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.AtivarDespachanteComoConsignatario = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar despachante como consignatário", val: ko.observable(false), def: false, visible: true });
    this.DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino = PropertyEntity({ getType: typesKnockout.bool, text: "Dividir em cargas de acordo com a quantidade de container recebido (Porto Destino)", val: ko.observable(false), def: false, visible: true });
    this.DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem = PropertyEntity({ getType: typesKnockout.bool, text: "Dividir em cargas de acordo com a quantidade de container recebido (Porto Origem)", val: ko.observable(false), def: false, visible: true });
    this.CriarSequenciaCargasMesmoComPedidoExistente = PropertyEntity({ getType: typesKnockout.bool, text: "Criar sequência de cargas mesmo com pedido existente", val: ko.observable(false), def: false, visible: true });
    this.GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar 01 carga SVM por carga MTL quando o porto for destino e origem", val: ko.observable(false), def: false, visible: true });
    this.GerarUmaCargaSVMPorCargaMTLQuandoPortoOrigem = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar 01 carga SVM por carga MTL quando o porto for origem", val: ko.observable(false), def: false, visible: false });
    this.DiasAntesDoPodParaEnvioDaDocumentacao = PropertyEntity({ text: "Dias antes do POD para envio da documentação para o despachante: ", getType: typesKnockout.int, required: ko.observable(false), maxlength: 30, configInt: { precision: 0, allowZero: true, thousands: "" }, visible: ko.observable(true), enable: ko.observable(false) });
    this.RKST = PropertyEntity({ text: "RKST: ", required: ko.observable(false), maxlength: 10});
    this.AtivarNulo = PropertyEntity({ val: ko.observable(true) });

    this.AtivarNulo.val.subscribe((value) => {
        if (value) {
            _porto.DiasAntesDoPodParaEnvioDaDocumentacao.val("");
            _porto.DiasAntesDoPodParaEnvioDaDocumentacao.enable(false);
        }
        if (!value) {
            _porto.DiasAntesDoPodParaEnvioDaDocumentacao.enable(true);
            if (!(_porto.DiasAntesDoPodParaEnvioDaDocumentacao.val() > 0))
                _porto.DiasAntesDoPodParaEnvioDaDocumentacao.val(0);
        }
    });


    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadPorto() {

    _porto = new Porto();
    KoBindings(_porto, "knockoutCadastroPorto");

    _pesquisaPorto = new PesquisaPorto();
    KoBindings(_pesquisaPorto, "knockoutPesquisaPorto", false, _pesquisaPorto.Pesquisar.id);

    HeaderAuditoria("Porto", _porto);

    new BuscarLocalidades(_porto.Localidade);
    new BuscarTransportadores(_porto.Empresa);

    buscarPortos();
}


function adicionarClick(e, sender) {
    if (!validarRKST()) return;
    Salvar(e, "Porto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPorto.CarregarGrid();
                limparCamposPorto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!validarRKST()) return;
    Salvar(e, "Porto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPorto.CarregarGrid();
                limparCamposPorto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o porto selecionado?", function () {
        ExcluirPorCodigo(_porto, "Porto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridPorto.CarregarGrid();
                limparCamposPorto();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPorto();
}

//*******MÉTODOS*******


function buscarPortos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPorto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPorto = new GridView(_pesquisaPorto.Pesquisar.idGrid, "Porto/Pesquisa", _pesquisaPorto, menuOpcoes, null);
    _gridPorto.CarregarGrid();
}

function editarPorto(portoGrid) {
    limparCamposPorto();
    _porto.Codigo.val(portoGrid.Codigo);
    BuscarPorCodigo(_porto, "Porto/BuscarPorCodigo", function (arg) {
        _pesquisaPorto.ExibirFiltros.visibleFade(false);
        _porto.Atualizar.visible(true);
        _porto.Cancelar.visible(true);
        _porto.Excluir.visible(true);
        _porto.Adicionar.visible(false);

        controllarVisibilidadeCampoDias();
    }, null);
}

function limparCamposPorto() {
    _porto.Atualizar.visible(false);
    _porto.Cancelar.visible(false);
    _porto.Excluir.visible(false);
    _porto.Adicionar.visible(true);
    _porto.AtivarNulo.val(true);
    _porto.DiasAntesDoPodParaEnvioDaDocumentacao.val("");
    _porto.DiasAntesDoPodParaEnvioDaDocumentacao.enable(false);
    LimparCampos(_porto);
}

function controllarVisibilidadeCampoDias() {

    if (_porto.DiasAntesDoPodParaEnvioDaDocumentacao.val() === '')
        return _porto.AtivarNulo.val(true)

    if (_porto.DiasAntesDoPodParaEnvioDaDocumentacao.val() >= 0)
        _porto.AtivarNulo.val(false)


}

function validarRKST() {
    var val = _porto.RKST.val();
    // Permite vazio, mas se preenchido, não pode ter espaço
    if (val && /\s/.test(val)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "O campo RKST não pode conter espaços em branco.");
        return false;
    }
    return true;
}