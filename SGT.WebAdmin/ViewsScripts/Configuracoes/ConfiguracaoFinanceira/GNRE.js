//*******MAPEAMENTO KNOCKOUT*******

var _configuracaoGNRE, _CRUDConfiguracaoGNRE;

var ConfiguracaoGNRE = function () {
    this.GerarGNREParaCTesEmitidos = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar GNRE para CT-es emitidos:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GerarGNREAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar GNRE automaticamente", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AlertarDisponibilidadeGNREParaCarga = PropertyEntity({ getType: typesKnockout.bool, text: "Alertar disponibilidade de GNRE para a carga", val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.ListaConfiguracoesRegistros = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.ConfiguracoesRegistros = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.GerarGNREParaCTesEmitidos.val.subscribe(function (novoValor) {
        if (novoValor === true)
            $("#knockoutConfiguracaoFinanceiraGNRERegistro, #opcoes-gnre").removeClass("d-none");
        else
            $("#knockoutConfiguracaoFinanceiraGNRERegistro, #opcoes-gnre").addClass("d-none");
    });
};

var CRUDConfiguracaoGNRE = function () {
    this.Salvar = PropertyEntity({ eventClick: SalvarConfiguracaoGNREClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadConfiguracaoGNRE() {

    _configuracaoGNRE = new ConfiguracaoGNRE();
    KoBindings(_configuracaoGNRE, "knockoutConfiguracaoFinanceiraGNRE");

    _CRUDConfiguracaoGNRE = new CRUDConfiguracaoGNRE();
    KoBindings(_CRUDConfiguracaoGNRE, "knockoutCRUDConfiguracaoFinanceiraGNRE");

    LoadConfiguracaoGNRERegistro();
}

function SalvarConfiguracaoGNREClick(e, sender) {
    _configuracaoGNRE.ListaConfiguracoesRegistros.val(JSON.stringify(_configuracaoGNRE.ConfiguracoesRegistros.val()));

    Salvar(_configuracaoGNRE, "ConfiguracaoFinanceira/SalvarConfiguracaoGNRE", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}