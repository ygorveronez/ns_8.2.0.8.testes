/// <reference path="Infracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _resumoInfracao;

/*
 * Declaração das Classes
 */

var ResumoInfracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ text: "Data/Hora: " });
    this.Motorista = PropertyEntity({ text: "Motorista: ", visible: ko.observable(false) });
    this.Numero = PropertyEntity({ text: "Número da Ocorrência: ", visible: ko.observable(false) });
    this.NumeroAtuacao = PropertyEntity({ text: "Número da Autuação: " });
    this.Pessoa = PropertyEntity({ text: "Pessoa: ", visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", visible: ko.observable(false) });
    this.Funcionario = PropertyEntity({ text: "Funcionário: ", visible: ko.observable(false) });


    this.Motorista.val.subscribe(controlarVisibilidadeMotorista);
    this.Pessoa.val.subscribe(controlarVisibilidadePessoa);
};

/*
 * Declaração das Funções de Inicialização
 */

function loadResumoInfracao() {
    _resumoInfracao = new ResumoInfracao();
    KoBindings(_resumoInfracao, "knockoutResumoInfracao");
}

/*
 * Declaração das Funções Públicas
 */

function controlarVisibilidadeMotorista() {
    _resumoInfracao.Motorista.visible(_resumoInfracao.Motorista.val());
}

function controlarVisibilidadePessoa() {
    _resumoInfracao.Pessoa.visible(_resumoInfracao.Pessoa.val());
}

function limparResumoInfracao() {
    _resumoInfracao.Numero.visible(false);

    LimparCampos(_resumoInfracao);
}

function preencherResumoInfracao(dadosResumo) {
    PreencherObjetoKnout(_resumoInfracao, { Data: dadosResumo });

    _resumoInfracao.Numero.visible(true);

    var ocorrenciaVeiculo = dadosResumo.TipoOcorrenciaInfracao == EnumTipoOcorrenciaInfracao.Veiculo;
    var ocorrenciaMotorista = dadosResumo.TipoOcorrenciaInfracao == EnumTipoOcorrenciaInfracao.Motorista;
    var ocorrenciaFuncionario = dadosResumo.TipoOcorrenciaInfracao == EnumTipoOcorrenciaInfracao.Funcionario;

    _resumoInfracao.Pessoa.visible(!string.IsNullOrWhiteSpace(dadosResumo.Pessoa));

    _resumoInfracao.Veiculo.visible(ocorrenciaVeiculo);
    _resumoInfracao.Motorista.visible(ocorrenciaMotorista || ocorrenciaVeiculo);
    _resumoInfracao.Funcionario.visible(ocorrenciaFuncionario);
}