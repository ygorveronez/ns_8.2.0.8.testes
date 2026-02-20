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
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/MarcaVeiculo.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoVeiculo.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _painelVeiculoViagem;

var PainelVeiculoViagem = function () {
    this.DataHoraSaidaInicioViagem = PropertyEntity({ text: "*Inicio de Viagem: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.DataHoraPrevisaoRetornoInicioViagem = PropertyEntity({ text: "*Previsão Fim de Viagem: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.LocalidadeDestinoInicioViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "*Destino:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });
    this.DataHoraRetornoViagem = PropertyEntity({ text: "*Data Retorno: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.LocalidadeRetornoViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "*Retorno:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });

    this.DataHoraSaidaInicioViagem.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.DataHoraSaidaInicioViagem.val(novoValor);
    });
    this.DataHoraPrevisaoRetornoInicioViagem.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.DataHoraPrevisaoRetornoInicioViagem.val(novoValor);
    });
    this.LocalidadeDestinoInicioViagem.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.LocalidadeDestinoInicioViagem.val(novoValor);
    });
    this.DataHoraRetornoViagem.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.DataHoraRetornoViagem.val(novoValor);
    });
    this.LocalidadeRetornoViagem.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.LocalidadeRetornoViagem.val(novoValor);
    });
}

//*******EVENTOS*******
function loadPainelVeiculoViagem() {
    _painelVeiculoViagem = new PainelVeiculoViagem();
    KoBindings(_painelVeiculoViagem, "knoutPainelVeiculoViagem", false);

    BuscarLocalidades(_painelVeiculoViagem.LocalidadeDestinoInicioViagem);
    BuscarLocalidades(_painelVeiculoViagem.LocalidadeRetornoViagem);

    _indicacaoVeiculo.DataHoraSaidaInicioViagem = _painelVeiculoViagem.DataHoraSaidaInicioViagem;
    _indicacaoVeiculo.DataHoraPrevisaoRetornoInicioViagem = _painelVeiculoViagem.DataHoraPrevisaoRetornoInicioViagem;
    _indicacaoVeiculo.LocalidadeDestinoInicioViagem = _painelVeiculoViagem.LocalidadeDestinoInicioViagem;
    _indicacaoVeiculo.DataHoraRetornoViagem = _painelVeiculoViagem.DataHoraRetornoViagem;
    _indicacaoVeiculo.LocalidadeRetornoViagem = _painelVeiculoViagem.LocalidadeRetornoViagem;
}

function emViagemClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _indicacaoVeiculo.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_indicacaoVeiculo, "PainelVeiculo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PainelVeiculoViagem != null)
                    PreencherObjetoKnout(_painelVeiculoViagem, { Data: arg.Data.PainelVeiculoViagem });
                if (arg.Data.PainelVeiculoManutencao != null)
                    PreencherObjetoKnout(_painelVeiculoManutencao, { Data: arg.Data.PainelVeiculoManutencao });

                _naoPaginar = true;
                RecarregarListaReboques();
                _indicacaoVeiculo.AvisadoCarregamento.visible(false);
                _indicacaoVeiculo.VeiculoVazio.visible(false);
                _indicacaoVeiculo.LocalAtual.visible(false);
                _indicacaoVeiculo.DataHoraIndicacao.visible(true);

                _indicacaoVeiculo.IndicacaoVeiculoVazio.val(false);
                _indicacaoVeiculo.IndicacaoAvisoCarregamento.val(false);
                _indicacaoVeiculo.IndicacaoManutencao.val(false);
                _indicacaoVeiculo.IndicacaoViagem.val(true);

                if (_indicacaoVeiculo.SituacaoAtual.val() === EnumSituacaoVeiculo.EmViagem) {
                    _painelVeiculoViagem.DataHoraSaidaInicioViagem.enable(false);
                    _painelVeiculoViagem.DataHoraPrevisaoRetornoInicioViagem.enable(false);
                    _painelVeiculoViagem.LocalidadeDestinoInicioViagem.enable(false);
                } else {
                    _painelVeiculoViagem.DataHoraRetornoViagem.enable(false);
                    _painelVeiculoViagem.LocalidadeRetornoViagem.enable(false);
                }

                _painelVeiculoModalIndicacaoVeiculo.show();
                $("#liTabManutencao").hide();
                $("#liTabLavacao").hide();
                $("#liTabViagem").show();
                Global.ExibirAba("divViagem");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}