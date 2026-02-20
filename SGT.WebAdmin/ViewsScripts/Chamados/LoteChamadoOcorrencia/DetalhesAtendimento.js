/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Enumeradores/EnumTipoIrregularidade.js" />
/// <reference path="../../Enumeradores/EnumAcaoTratativaIrregularidade.js" />

// #region Objetos Globais do Arquivo

var _detalhesAtendimento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhesAtendimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ text: "Carga: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número Chamado: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Responsavel = PropertyEntity({ text: "Responsável: ", val: ko.observable(""), visible: ko.observable(true) });
    this.DataCriacao = PropertyEntity({ text: "Data de Criação: ", val: ko.observable(""), visible: ko.observable(true) });
    this.MotivoChamado = PropertyEntity({ text: "Motivo Chamado: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", val: ko.observable(""), visible: ko.observable(true) });
};



// #endregion Classes

// #region Funções de Inicialização


function loadDetalhesAtendimento() {
    _detalhesAtendimento = new DetalhesAtendimento();
    KoBindings(_detalhesAtendimento, "knoutDetalhesAtendimento");
}



// #endregion Funções de Inicialização



function preencherDadosModalDetalhes(obj) {
    _detalhesAtendimento.Codigo.val(obj.Codigo);
    _detalhesAtendimento.Situacao.val(obj.Situacao);
    _detalhesAtendimento.Carga.val(obj.Carga);
    _detalhesAtendimento.Numero.val(obj.Numero);
    _detalhesAtendimento.Transportador.val(obj.Transportador);
    _detalhesAtendimento.Responsavel.val(obj.Responsavel);
    _detalhesAtendimento.DataCriacao.val(obj.DataCriacao);
    _detalhesAtendimento.MotivoChamado.val(obj.MotivoChamado);
    _detalhesAtendimento.Veiculo.val(obj.Veiculo);
}

function limparDetalhesAtendimento() {
    LimparCampos(_detalhesAtendimento)
}