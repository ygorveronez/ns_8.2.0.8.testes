/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _infracaoDuplicada;
var _buscaInfracoes;

/*
 * Declaração das Classes
 */
var InfracaoDuplicar = function () {
    this.Infracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadInfracaoDuplicar() {
    _infracaoDuplicar = new InfracaoDuplicar();

    _buscaInfracoes = new BuscarInfracoes(_infracaoDuplicar.Infracao, PreencherInfracao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function duplicarInfracaoClick() {
    exibirModalDuplicarInfracao();
}

function exibirModalDuplicarInfracao() {
    _buscaInfracoes.AbrirBusca();
}

function PreencherInfracao(data) {
    _dadosInfracao.Local.val(data.Local);
    _dadosInfracao.Data.val(data.Data);

    _dadosInfracao.Cidade.codEntity(data.CidadeCodigo);
    _dadosInfracao.Cidade.val(data.Cidade);

    _dadosInfracao.OrgaoEmissor.codEntity(data.OrgaoEmissorCodigo);
    _dadosInfracao.OrgaoEmissor.val(data.OrgaoEmissor);

    _dadosInfracao.Veiculo.codEntity(data.VeiculoCodigo);
    _dadosInfracao.Veiculo.val(data.Veiculo);

    _dadosInfracao.Motorista.codEntity(data.MotoristaCodigo);
    _dadosInfracao.Motorista.val(data.Motorista);
}