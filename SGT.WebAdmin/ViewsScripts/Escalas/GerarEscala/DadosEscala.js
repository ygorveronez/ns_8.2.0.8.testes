/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="DadosEscalaModelosRestricaoRodagem.js" />
/// <reference path="DadosEscalaProduto.js" />
/// <reference path="GerarEscala.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosEscala;

/*
 * Declaração das Classes
 */

var DadosEscala = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEscala = PropertyEntity({ text: "*Data da Escala: ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 400, enable: ko.observable(true) });

    this.DataEscala.val.subscribe(carregarProdutosPorDataEscala);
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDadosEscala() {
    _dadosEscala = new DadosEscala();
    KoBindings(_dadosEscala, "knockouDadosEscala");

    _dadosEscala.DataEscala.minDate(Global.DataAtual());
    _dadosEscala.DataEscala.val(moment().add(1, 'days').format("DD/MM/YYYY"));

    loadDadosEscalaProduto();
    loadModeloRestricaoRodagem();
}

/*
 * Declaração das Funções Públicas
 */

function adicionarEscala() {
    if (!validarCamposObrigatoriosDadosEscala())
        return;

    executarReST("GerarEscala/AdicionarEscala", obterDadosEscalaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Escala criada com sucesso.");
                editarGerarEscala(retorno.Data);
                recarregarGridGerarEscala();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function limparCamposDadosEscala() {
    LimparCampos(_dadosEscala);
    limparCamposDadosEscalaProduto();
    limparCamposModeloRestricaoRodagem();
    controlarCamposDadosEscalaHabilitados();
}

function preencherDadosEscala(dadosEscala) {
    PreencherObjetoKnout(_dadosEscala, { Data: dadosEscala });

    controlarCamposDadosEscalaHabilitados();
    preencherDadosEscalaProduto(dadosEscala.Produtos);
    preencherModeloRestricaoRodagem(dadosEscala.ModelosRestricaoRodagem);
}

/*
 * Declaração das Funções Privadas
 */

function controlarCamposDadosEscalaHabilitados() {
    var permitirEditarDadosEscala = isPermitirEditarDadosEscala();

    _dadosEscala.DataEscala.enable(permitirEditarDadosEscala);
    _dadosEscala.Observacao.enable(permitirEditarDadosEscala);
}

function obterDadosEscalaSalvar() {
    var dadosEscala = RetornarObjetoPesquisa(_dadosEscala);

    dadosEscala["Produtos"] = obterDadosEscalaProdutoSalvar();
    dadosEscala["ModelosRestricaoRodagem"] = obterModeloRestricaoRodagemSalvar();

    return dadosEscala;
}

function validarCamposObrigatoriosDadosEscala() {
    if (!ValidarCamposObrigatorios(_dadosEscala)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return false;
    }

    if (!isDadosEscalaProdutoInformado()) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Nenhum produto encontrado para gerar a escala!");
        return false;
    }

    return true;
}
