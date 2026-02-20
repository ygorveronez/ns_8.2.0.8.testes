
//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao, _configuracaoIntegracaoTecnologiaMonitoramentoOpcao;

var ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Key = PropertyEntity({ text: "Key".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string, required: true });
    this.Value = PropertyEntity({ text: "Value".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string, required: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao() {

    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao = new ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
    KoBindings(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao, "knockoutOpcoes");

    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick }] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Key", title: "Key", width: "40%" },
        { data: "Value", title: "Value", width: "40%" }
    ];

    _gridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new BasicDataTable(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
}

function RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao() {

    let data = new Array();

    $.each(_configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list, function (i, Opcao) {
        let OpcaoGrid = new Object();

        for (let prop in Opcao)
            OpcaoGrid[prop] = Opcao[prop].val;

        data.push(OpcaoGrid);
    });

    _gridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.CarregarGrid(data);

}

function EditarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick(data) {
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Atualizar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Cancelar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Excluir.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Adicionar.visible(false);

    PreencherObjetoKnout(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao, { Data: data });
}

function ExcluirConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirORegistro, function () {
        for (let i = 0; i < _configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list.length; i++) {
            if (_configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Codigo.val() == _configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list[i].Codigo.val) {
                _configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list.splice(i, 1);
                break;
            }
        }

        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
    });
}

function AdicionarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick(e, sender) {
    let valido = ValidarCamposObrigatorios(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao);

    if (valido) {
        _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Codigo.val(guid());

        _configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list.push(SalvarListEntity(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao));

        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();

        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);
    }
}

function AtualizarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick(e, sender) {
    let valido = ValidarCamposObrigatorios(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao);

    if (valido) {
        for (let i = 0; i < _configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list.length; i++) {
            if (_configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Codigo.val() == _configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list[i].Codigo.val) {
                _configuracaoIntegracaoTecnologiaMonitoramento.Opcoes.list[i] = SalvarListEntity(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao);
                break;
            }
        }

        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();

    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);
    }
}

function CancelarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcaoClick(data) {
    LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
}

function LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao() {
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Adicionar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Atualizar.visible(false);
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Cancelar.visible(false);
    _configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Excluir.visible(false);

    LimparCampos(_configuracaoIntegracaoTecnologiaMonitoramentoOpcao);
}