
//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar, _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar;

var ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Key = PropertyEntity({ text: "Key".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string, required: true });
    this.Value = PropertyEntity({ text: "Value".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string, required: true });
    
    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar() {

    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar = new ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
    KoBindings(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar, "knockoutMonitorar");

    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick }] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Key", title: "Key", width: "40%" },
        { data: "Value", title: "Value", width: "40%" }
    ];

    _gridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = new BasicDataTable(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
}

function RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar() {

    let data = new Array();

    $.each(_configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list, function (i, monitorar) {
        let monitorarGrid = new Object();

        for (let prop in monitorar)
            monitorarGrid[prop] = monitorar[prop].val;

        data.push(monitorarGrid);
    });

    _gridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.CarregarGrid(data);

}

function EditarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick(data) {
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Atualizar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Cancelar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Excluir.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Adicionar.visible(false);

    PreencherObjetoKnout(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar, { Data: data });
}

function ExcluirConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirORegistro, function () {
        for (let i = 0; i < _configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list.length; i++) {
            if (_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Codigo.val() == _configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list[i].Codigo.val) {
                _configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list.splice(i, 1);
                break;
            }
        }

        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
    });
}

function AdicionarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick(e, sender) {
    let valido = ValidarCamposObrigatorios(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar);

    if (valido) {
        _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Codigo.val(guid());

        _configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list.push(SalvarListEntity(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar));

        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();

        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);
    }
}

function AtualizarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick(e, sender) {
    let valido = ValidarCamposObrigatorios(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar);

    if (valido) {
        for (let i = 0; i < _configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list.length; i++) {
            if (_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Codigo.val() == _configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list[i].Codigo.val) {
                _configuracaoIntegracaoTecnologiaMonitoramento.Monitorar.list[i] = SalvarListEntity(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar);
                break;
            }
        }

        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();

    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);
    }
}

function CancelarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorarClick(data) {
    LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
}

function LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar() {
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Adicionar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Atualizar.visible(false);
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Cancelar.visible(false);
    _configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Excluir.visible(false);

    LimparCampos(_configuracaoIntegracaoTecnologiaMonitoramentoMonitorar);
}