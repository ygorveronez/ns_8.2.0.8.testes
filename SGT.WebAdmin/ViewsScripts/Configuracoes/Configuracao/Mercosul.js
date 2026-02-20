/// <reference path="Configuracao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pais.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroSistemaMercosul;
var _configuracaoMercosul;
var _gridPaisMercosul;

/*
 * Declaração das Classes
 */

var CadastroPaisesMercosul = function () {
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "País:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid() });
    this.UltimoCrt = PropertyEntity({ text: "Último CRT:", getType: typesKnockout.string, val: ko.observable(0) });
    this.UltimoMicDta = PropertyEntity({ text: "Último Mic/DTA:", getType: typesKnockout.string, val: ko.observable(0) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPaisMercosulClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var PaisesMercosul = function () {
    this.ListaPaisesMercosul = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaPaisesMercosul.val.subscribe(function () {
        recarregarGridPaisesMercosul();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoPaisesMercosulModalClick, type: types.event, text: "Adicionar País", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridConfiguracaoPisesMercosul() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Excluir", id: guid(), metodo: removerConfiguracaoPaisesMercosul, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };
    var header = [
        { data: "Pais", title: "País", width: "30%", className: "text-align-left" },
        { data: "Empresa", title: "Empresa", width: "30%", className: "text-align-left" },
        { data: "UltimoCrt", title: "Último CRT", width: "30%", className: "text-align-left" },
        { data: "UltimoMicDta", title: "Último MIC/DTA", width: "30%", className: "text-align-left" }
    ];

    _gridPaisMercosul = new BasicDataTable(_configuracaoMercosul.ListaPaisesMercosul.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridPaisMercosul.CarregarGrid([]);
}

function loadConfiguracaoMercosul() {
    $("#container-configuracao-Mercosul").appendTo("#configuracao-mercosul");

    _configuracaoMercosul = new PaisesMercosul();
    KoBindings(_configuracaoMercosul, "knockoutConfiguracaoMercosul");

    _cadastroSistemaMercosul = new CadastroPaisesMercosul();
    KoBindings(_cadastroSistemaMercosul, "knockoutCadastroPaisMercosul");

    loadGridConfiguracaoPisesMercosul();

    new BuscarPaises(_cadastroSistemaMercosul.Pais);
    new BuscarEmpresa(_cadastroSistemaMercosul.Empresa);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPaisMercosulClick() {
    if (ValidarCamposObrigatorios(_cadastroSistemaMercosul)) {
        var novoPais = obterConfiguracaoPisesMercosulSalvar();
        var paisExistente = _configuracaoMercosul.ListaPaisesMercosul.val().find(function (pais) {
            return pais.Pais === novoPais.Pais && pais.Empresa === novoPais.Empresa;
        });

        if (paisExistente) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "Este país e empresa já estão na lista!");
        } else {
            _configuracaoMercosul.ListaPaisesMercosul.val().push(novoPais);
            recarregarGridPaisesMercosul();
            fecharModalCadastroPaisesMercosul();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function adicionarConfiguracaoPaisesMercosulModalClick() {
    exibirModalCadastroPaisesMercosul();
}

function preencherConfiguracaoPaisesMercosul(dadosConfiguracaoPaisesMercosul) {
    _configuracaoMercosul.ListaPaisesMercosul.val(dadosConfiguracaoPaisesMercosul.PaisesMercosul);
}

function preencherConfiguracaoPaisesMercosulSalvar(configuracao) {
    configuracao["PaisesMercosul"] = obterListaPaisesMercosulSalvar();
}

function exibirModalCadastroPaisesMercosul() {
    Global.abrirModal('divModalCadastroPaisMercosul');
    $("#divModalCadastroPaisMercosul").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroSistemaMercosul);
    }); 
}

function fecharModalCadastroPaisesMercosul() {
    Global.fecharModal('divModalCadastroPaisMercosul');
}

function obterConfiguracaoPisesMercosulSalvar() {
    return {
        Pais: _cadastroSistemaMercosul.Pais.val(),
        Empresa: _cadastroSistemaMercosul.Empresa.val(),
        UltimoCrt: _cadastroSistemaMercosul.UltimoCrt.val(),
        UltimoMicDta: _cadastroSistemaMercosul.UltimoMicDta.val(),       
    };
}

function obterListaPaisMercosul() {
    return _configuracaoMercosul.ListaPaisesMercosul.val().slice();
}

function obterListaPaisesMercosulSalvar() {
    var listaPaisesMercosul = obterListaPaisMercosul();
    var listaPaisesMercosulRetornar = new Array();

    listaPaisesMercosul.forEach(function (PaisesMmercosul) {
        listaPaisesMercosulRetornar.push(PaisesMmercosul.Pais);
        listaPaisesMercosulRetornar.push(PaisesMmercosul.Empresa);
        listaPaisesMercosulRetornar.push(PaisesMmercosul.UltimoCrt);
        listaPaisesMercosulRetornar.push(PaisesMmercosul.UltimoMicDta);
    });

    return JSON.stringify(listaPaisesMercosulRetornar);
}

function recarregarGridPaisesMercosul() {
    var listaPaisesMercosul = obterListaPaisMercosul();

    _gridPaisMercosul.CarregarGrid(listaPaisesMercosul);
}

function removerConfiguracaoPaisesMercosul(registroSelecionado) {
    var listaPaisesMercosul = obterListaPaisMercosul();

    listaPaisesMercosul.forEach(function (paisesMercosul, i) {
        if (registroSelecionado.Pais == paisesMercosul.Pais) {
            listaPaisesMercosul.splice(i, 1);
        }
    });

    _configuracaoMercosul.ListaPaisesMercosul.val(listaPaisesMercosul);
}