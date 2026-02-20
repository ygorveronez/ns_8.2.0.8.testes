/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroAreaVeiculoPosicao;
var _CRUDcadastroAreaVeiculoPosicao;
var _gridAreaVeiculoPosicao;
var _areaVeiculoPosicao;

/*
 * Declaração das Classes
 */

var CRUDCadastroAreaVeiculoPosicao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarAreaVeiculoPosicaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarAreaVeiculoPosicaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirAreaVeiculoPosicaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroAreaVeiculoPosicao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.QRCode = PropertyEntity({});
    
}

var AreaVeiculoPosicao = function () {
    this.ListaPosicao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaPosicao.val.subscribe(function () {
        recarregarGridAreaVeiculoPosicao();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAreaVeiculoPosicaoModalClick, type: types.event, text: "Adicionar Posição" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAreaVeiculoPosicao() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var opcaoBaixarQrCode = { descricao: "Baixar QR Code", id: guid(), metodo: baixarQrCodeAreaVeiculoPosicaoClick, icone: "", visibilidade: isOpcaoBaixarQrCodeVisivel };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarAreaVeiculoPosicaoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 20, opcoes: [opcaoBaixarQrCode, opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "QRCode", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%", orderable: false }
    ];

    _gridAreaVeiculoPosicao = new BasicDataTable(_areaVeiculoPosicao.ListaPosicao.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridAreaVeiculoPosicao.CarregarGrid([]);
}

function loadAreaVeiculoPosicao() {
    _areaVeiculoPosicao = new AreaVeiculoPosicao();
    KoBindings(_areaVeiculoPosicao, "knockoutAreaVeiculoPosicao");

    _cadastroAreaVeiculoPosicao = new CadastroAreaVeiculoPosicao();
    KoBindings(_cadastroAreaVeiculoPosicao, "knockoutCadastroAreaVeiculoPosicao");

    _CRUDcadastroAreaVeiculoPosicao = new CRUDCadastroAreaVeiculoPosicao();
    KoBindings(_CRUDcadastroAreaVeiculoPosicao, "knockoutCRUDCadastroAreaVeiculoPosicao");

    loadGridAreaVeiculoPosicao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAreaVeiculoPosicaoClick() {
    if (ValidarCamposObrigatorios(_cadastroAreaVeiculoPosicao)) {

        var posicao = obterCadastroAreaVeiculoPosicaoSalvar();

        _areaVeiculoPosicao.ListaPosicao.val().push(posicao);

        adicionarAreaVeiculoDesenho(posicao);

        recarregarGridAreaVeiculoPosicao();
        fecharModalCadastroAreaVeiculoPosicao();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function adicionarAreaVeiculoPosicaoModalClick() {
    _cadastroAreaVeiculoPosicao.Codigo.val(guid());
    _cadastroAreaVeiculoPosicao.QRCode.val(guid());

    controlarBotoesCadastroAreaVeiculoPosicaoHabilitados(false);

    exibirModalCadastroAreaVeiculoPosicao();
}

function baixarQrCodeAreaVeiculoPosicaoClick(registroSelecionado) {
    executarDownload("AreaVeiculoPosicao/BaixarQrCodePosicao", { Codigo: registroSelecionado.Codigo });
}

function atualizarAreaVeiculoPosicaoClick() {
    if (ValidarCamposObrigatorios(_cadastroAreaVeiculoPosicao)) {
        var listaPosicao = obterListaPosicao();

        listaPosicao.forEach(function (posicao, i) {
            if (_cadastroAreaVeiculoPosicao.Codigo.val() == posicao.Codigo) {
                var posicao = obterCadastroAreaVeiculoPosicaoSalvar();
                listaPosicao.splice(i, 1, posicao);
                editarAreaVeiculoDesenho(posicao);
            }
        });

        _areaVeiculoPosicao.ListaPosicao.val(listaPosicao);

        recarregarGridAreaVeiculoPosicao();
        fecharModalCadastroAreaVeiculoPosicao();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function editarAreaVeiculoPosicaoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroAreaVeiculoPosicao, { Data: registroSelecionado });

    controlarBotoesCadastroAreaVeiculoPosicaoHabilitados(true);

    exibirModalCadastroAreaVeiculoPosicao();
}

function excluirAreaVeiculoPosicaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a posição?", function () {
        removerAreaVeiculoPosicao(_cadastroAreaVeiculoPosicao.Codigo.val());
        fecharModalCadastroAreaVeiculoPosicao();
    });
}

/*
 * Declaração das Funções Públicas
 */

function associarVeiculoPosicaoDesenho() {
    for (var i = 0; i < _areaVeiculoPosicao.ListaPosicao.val().length; i++) {
        var area = obterJsonAreaVeiculoDesenhoPorID(_areaVeiculoPosicao.ListaPosicao.val()[i].Codigo);
        _areaVeiculoPosicao.ListaPosicao.val()[i].Desenho = area;
    }
}

function obterAreaVeiculoPosicaoSalvar() {
    
    associarVeiculoPosicaoDesenho();

    var listaPosicao = obterListaPosicao();

    return JSON.stringify(listaPosicao);
}

function preencherAreaVeiculoPosicao(dadosPosicao) {
    _areaVeiculoPosicao.ListaPosicao.val(dadosPosicao);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroAreaVeiculoPosicaoHabilitados(isEdicao) {
    _CRUDcadastroAreaVeiculoPosicao.Adicionar.visible(!isEdicao);
    _CRUDcadastroAreaVeiculoPosicao.Atualizar.visible(isEdicao);
    _CRUDcadastroAreaVeiculoPosicao.Excluir.visible(isEdicao);
}

function exibirModalCadastroAreaVeiculoPosicao() {
    Global.abrirModal('divModalCadastroAreaVeiculoPosicao');
    $("#divModalCadastroAreaVeiculoPosicao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroAreaVeiculoPosicao);
    });
}

function fecharModalCadastroAreaVeiculoPosicao() {
    Global.fecharModal('divModalCadastroAreaVeiculoPosicao');
}

function isOpcaoBaixarQrCodeVisivel(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo) && (registroSelecionado.Codigo > 0);
}

function limparCamposAreaVeiculoPosicao() {
    preencherAreaVeiculoPosicao([]);
}

function obterCadastroAreaVeiculoPosicaoSalvar() {
    return {
        Codigo: _cadastroAreaVeiculoPosicao.Codigo.val(),
        Descricao: _cadastroAreaVeiculoPosicao.Descricao.val(),
        QRCode: _cadastroAreaVeiculoPosicao.QRCode.val()
    };
}

function obterListaPosicao() {
    return _areaVeiculoPosicao.ListaPosicao.val().slice();
}

function recarregarGridAreaVeiculoPosicao() {
    var listaPosicao = obterListaPosicao();

    _gridAreaVeiculoPosicao.CarregarGrid(listaPosicao);
}

function removerAreaVeiculoPosicao(codigo) {
    excluirAreaVeiculoDesenho(codigo);

    var listaPosicao = obterListaPosicao();

    listaPosicao.forEach(function (posicao, i) {
        if (codigo == posicao.Codigo)
            listaPosicao.splice(i, 1);
    });

    _areaVeiculoPosicao.ListaPosicao.val(listaPosicao);

    
}