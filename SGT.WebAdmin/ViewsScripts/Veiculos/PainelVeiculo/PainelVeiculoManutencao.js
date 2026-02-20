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

var _painelVeiculoManutencao;

var PainelVeiculoManutencao = function () {
    this.DataHoraEntradaManutencao = PropertyEntity({ text: "*Entrada em Manutenção: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.DataHoraPrevisaoSaidaManutencao = PropertyEntity({ text: "*Previsão Saída: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.TipoOrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Ordem Serviço: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataHoraSaidaManutencao = PropertyEntity({ text: "*Saída da Manutenção: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.OrdemServicoFrota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "*Ordem de Serviço:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });

    this.DataHoraEntradaManutencao.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.DataHoraEntradaManutencao.val(novoValor);
    });
    this.DataHoraPrevisaoSaidaManutencao.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.DataHoraPrevisaoSaidaManutencao.val(novoValor);
    });
    this.TipoOrdemServico.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.TipoOrdemServico.val(novoValor);
    });
    this.DataHoraSaidaManutencao.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.DataHoraSaidaManutencao.val(novoValor);
    });
    this.OrdemServicoFrota.val.subscribe(function (novoValor) {
        _indicacaoVeiculo.OrdemServicoFrota.val(novoValor);
    });
}

//*******EVENTOS*******
function loadPainelVeiculoManutencao() {
    _painelVeiculoManutencao = new PainelVeiculoManutencao();
    KoBindings(_painelVeiculoManutencao, "knoutPainelVeiculoManutencao", false);

    _indicacaoVeiculo.DataHoraEntradaManutencao = _painelVeiculoManutencao.DataHoraEntradaManutencao;
    _indicacaoVeiculo.DataHoraPrevisaoSaidaManutencao = _painelVeiculoManutencao.DataHoraPrevisaoSaidaManutencao;
    _indicacaoVeiculo.TipoOrdemServico = _painelVeiculoManutencao.TipoOrdemServico;
    _indicacaoVeiculo.DataHoraSaidaManutencao = _painelVeiculoManutencao.DataHoraSaidaManutencao;
    _indicacaoVeiculo.OrdemServicoFrota = _painelVeiculoManutencao.OrdemServicoFrota;

    BuscarOrdemServico(_painelVeiculoManutencao.OrdemServicoFrota, RetornoBuscarOrdemServico);
    BuscarTipoOrdemServico(_painelVeiculoManutencao.TipoOrdemServico);
}

function emManutencaoClick(e) {
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
                _indicacaoVeiculo.IndicacaoManutencao.val(true);
                _indicacaoVeiculo.IndicacaoViagem.val(false);

                if (_indicacaoVeiculo.SituacaoAtual.val() === EnumSituacaoVeiculo.EmManutencao) {
                    _painelVeiculoManutencao.DataHoraEntradaManutencao.enable(false);
                    _painelVeiculoManutencao.DataHoraPrevisaoSaidaManutencao.enable(false);
                } else {
                    _painelVeiculoManutencao.DataHoraSaidaManutencao.enable(false);
                    _painelVeiculoManutencao.OrdemServicoFrota.enable(false);
                }

                _painelVeiculoModalIndicacaoVeiculo.show();
                $("#liTabManutencao").show();
                $("#liTabViagem").hide();
                $("#liTabLavacao").hide();
                Global.ExibirAba("divManutencao");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function RetornoBuscarOrdemServico(data) {
    _painelVeiculoManutencao.OrdemServicoFrota.val(data.Numero);
    _painelVeiculoManutencao.OrdemServicoFrota.codEntity(data.Codigo);
}