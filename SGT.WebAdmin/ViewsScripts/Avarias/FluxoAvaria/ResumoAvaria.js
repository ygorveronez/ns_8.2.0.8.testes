/// <reference path="SolicitacaoAvaria.js" />
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

var _resumoAvaria;

var ResumoAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroAvaria = PropertyEntity({ text: "Numero da Avaria: ", visible: ko.observable(false) });
    this.Viagem = PropertyEntity({ text: "Viagem: " });
    this.MotivoAvaria = PropertyEntity({ text: "Motivo da Avaria: " });
    this.Solicitante = PropertyEntity({ text: "Solicitante: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.DataAvaria = PropertyEntity({ text: "Data da Avaria: " });
    this.Transportador = PropertyEntity({ text: "Transportador: " });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS) });
    this.Percurso = PropertyEntity({ text: "Percurso: " });
    this.Veiculo = PropertyEntity({ text: "Veículo: " });
    this.Motorista = PropertyEntity({ text: "Motorista: " });
    this.ValorAvaria = PropertyEntity({ text: "Valor da Avaria: " });
    this.ValorDesconto = PropertyEntity({ text: "Valor do Desconto : " });
    this.TipoOperacao = PropertyEntity({ text: "Tipo Operação: " });
    this.Lote = PropertyEntity({ text: "Lote: " });
    this.CentroResultado = PropertyEntity({ text: "Centro de Resultado: " });
};

//*******EVENTOS*******

function loadResumoAvaria() {
    _resumoAvaria = new ResumoAvaria();
    KoBindings(_resumoAvaria, "knockoutResumoAvaria");
}

//*******MÉTODOS*******

function PreecherResumoAvaria() {
    BuscarPorCodigo(_resumoAvaria, "FluxoAvaria/ResumoSolicitacao", function (arg) {
        _resumoAvaria.NumeroAvaria.visible(true);
    }, function (e) {
        _resumoAvaria.NumeroAvaria.visible(false);
        exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
    });
}

function limparResumo() {
    _resumoAvaria.NumeroAvaria.visible(false);
    LimparCampos(_resumoAvaria);
}

function AtualizarValorAvaria() {
    executarReST("FluxoAvaria/ValorAvaria", { Codigo: _fluxoAvaria.Codigo.val() }, function (arg) {
        if (arg.Success && arg.Data)
            _resumoAvaria.ValorAvaria.val(arg.Data.ValorAvaria);
    });
}