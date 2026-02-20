var _regraCancelamentoPedido;


var RegraCancelamentoPedido = function () {
    this.PermiteDesistenciaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirDesistênciaDaCarga, issue: 0, val: ko.observable(false), def: false });
    this.PermiteDesistenciaCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirDesistenciaDoCarregamento, issue: 0, val: ko.observable(false), def: false });
    this.CobrarDesistenciaCargaAposHorario = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.CobrarDesistenciaDaCargaSomenteAposUmHorario, issue: 0, val: ko.observable(false), def: false });
    this.HoraCobrarDesistenciaCarga = PropertyEntity({ getType: typesKnockout.time, text: Localization.Resources.Pedidos.TipoOperacao.CobrarAposAs.getFieldDescription(), val: ko.observable(""), def: "" });
    this.PercentualCobrarDesistenciaCarga = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.PercentualCobrar.getFieldDescription(), val: ko.observable(""), def: "" });
    this.PercentualCobrarDesistenciaCarregamento = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.PercentualCobrar.getFieldDescription(), val: ko.observable(""), def: "" });
}

function LoadRegraCancelamentoPedido() {
    _regraCancelamentoPedido = new RegraCancelamentoPedido();
    KoBindings(_regraCancelamentoPedido, "tabCancelamentoPedido");

    _tipoOperacao.PermiteDesistenciaCarga = _regraCancelamentoPedido.PermiteDesistenciaCarga;
    _tipoOperacao.PermiteDesistenciaCarregamento = _regraCancelamentoPedido.PermiteDesistenciaCarregamento;
    _tipoOperacao.CobrarDesistenciaCargaAposHorario = _regraCancelamentoPedido.CobrarDesistenciaCargaAposHorario;
    _tipoOperacao.HoraCobrarDesistenciaCarga = _regraCancelamentoPedido.HoraCobrarDesistenciaCarga;
    _tipoOperacao.PercentualCobrarDesistenciaCarga = _regraCancelamentoPedido.PercentualCobrarDesistenciaCarga;
    _tipoOperacao.PercentualCobrarDesistenciaCarregamento = _regraCancelamentoPedido.PercentualCobrarDesistenciaCarregamento;
}