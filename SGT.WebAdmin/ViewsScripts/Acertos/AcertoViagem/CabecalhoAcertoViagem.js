/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="OcorrenciaAcertoViagem.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _cabecalhoAcertoViagem;

var CabecalhoAcertoViagem = function () {
    this.NumeroAcerto = PropertyEntity({ text: "Número Acerto: ", getType: typesKnockout.int, val: ko.observable(""), visible: true });
    this.DescricaoPeriodo = PropertyEntity({ text: "Período: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Placas = PropertyEntity({ text: "Placas: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.NomeMotorista = PropertyEntity({ text: "Motorista: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.LimparNovoAcerto = PropertyEntity({ eventClick: LimparNovoAcertoClick, type: types.event, text: " Limpar / Novo Acerto ", visible: ko.observable(true), enable: ko.observable(true) });
}


//*******EVENTOS*******

function LimparNovoAcertoClick(e, sender) {
    limparCamposAcertoViagem();
    CarregarCargas();
    CarregarOcorrenciasAcerto();
    LimparOcultarAbas();
    CarregarPedagios();
    CarregarDespesas();
    CarregarAbastecimentos();
    CarregarFechamento();
    PosicionarEtapa();
    LimparFechamentoAcerto();
    $("#knockoutCabecalhoAcerto").hide();

    _acertoViagem.CancelarAcerto.visible(false);
    _acertoViagem.SalvarObservacaoAcerto.visible(false);

    $("#" + _etapaAcertoViagem.Etapa1.idTab).click();
}

function LimparFechamentoAcerto() {   
    LimparCampos(_fechamentoAcertoViagem);
    _gridDescontoMotorista.CarregarGrid();
    _gridBonificacoesMotorista.CarregarGrid();
    _gridDevolucoesMoedaEstrangeira.CarregarGrid();
    _gridVariacoesCambial.CarregarGrid();
    _gridAdiantamentos.CarregarGrid();
    RecarregarGridCheques();
    RecarregarGridFolgas();
    

    //var data = {
    //    Codigo: 0
    //};

    //executarReST("AcertoFechamento/DadosLimpoFechamentoAcerto", data, function (arg) {
    //    if (arg.Success) {
    //        if (arg.Data != null) {
    //            _gridDescontoMotorista.CarregarGrid();
    //            _gridBonificacoesMotorista.CarregarGrid();
    //            _gridDevolucoesMoedaEstrangeira.CarregarGrid();
    //            _gridVariacoesCambial.CarregarGrid();
    //            _gridAdiantamentos.CarregarGrid();

    //            var dataFechamento = { Data: arg.Data };
    //            PreencherObjetoKnout(_fechamentoAcertoViagem, dataFechamento);

    //            RecarregarGridCheques();
    //            RecarregarGridFolgas();
    //            if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
    //                if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
    //                    _fechamentoAcertoViagem.VisualizarRecibo.enable(true);
    //                    _fechamentoAcertoViagem.VisualizarRecibo.visible(true);
    //                } else
    //                    _fechamentoAcertoViagem.VisualizarRecibo.visible(false);
    //            } else {
    //                _fechamentoAcertoViagem.VisualizarRecibo.visible(false);
    //            }
    //        }
    //    } else {
    //        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
    //    }
    //});
}

function loadCabecalhoAcertoViagem(callback) {
    _cabecalhoAcertoViagem = new CabecalhoAcertoViagem();
    KoBindings(_cabecalhoAcertoViagem, "knockoutCabecalhoAcerto");
}

//*******MÉTODOS*******

function CarregarDadosCabecalho(dados) {
    var dataCabecalho = { Data: dados };
    PreencherObjetoKnout(_cabecalhoAcertoViagem, dataCabecalho);
    $("#knockoutCabecalhoAcerto").show();
}