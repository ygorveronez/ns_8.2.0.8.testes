
//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoIntegracaoTecnologiaMonitoramentoConta, _configuracaoIntegracaoTecnologiaMonitoramentoConta;

var ConfiguracaoIntegracaoTecnologiaMonitoramentoConta = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Nome = PropertyEntity({ text: "Nome".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.TipoComunicacaoIntegracao = PropertyEntity({ val: ko.observable(EnumTipoComunicacaoIntegracao.WebService), options: EnumTipoComunicacaoIntegracao.ObterOpcoes(), def: EnumTipoComunicacaoIntegracao.WebService, text: "Tipo de Comunicação".getRequiredFieldDescription() });
    this.Protocolo = PropertyEntity({ val: ko.observable(EnumProtocolo.HTTPS), options: EnumProtocolo.ObterOpcoes(), def: EnumProtocolo.HTTPS, text: "Protocolo".getRequiredFieldDescription() });
    this.Servidor = PropertyEntity({ text: "Servidor".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Porta = PropertyEntity({ text: "Porta".getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, configInt: { allowZero: true } });
    this.URI = PropertyEntity({ text: "URI".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Usuario = PropertyEntity({ text: "Usuario".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Senha = PropertyEntity({ text: "Senha".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.BancoDeDados = PropertyEntity({ text: "Banco de Dados".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Charset = PropertyEntity({ text: "Charset".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Diretorio = PropertyEntity({ text: "Diretório".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ArquivoControle = PropertyEntity({ text: "Arquivo Controle".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ParametrosAdicionais = PropertyEntity({ text: "Parâmetros Adicionais".getFieldDescription(), maxlength: 2000, val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.SolicitanteSenha = PropertyEntity({ text: "Solicitante Senha".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.SolicitanteId = PropertyEntity({ text: "Solicitante Id".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.RastreadorId = PropertyEntity({ text: "Rastreador Id".getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Habilitada = PropertyEntity({ text: "Habilitada", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.BuscarDadosVeiculos = PropertyEntity({ text: "Buscar dados veículos", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.UsaPosicaoFrota = PropertyEntity({ text: "Usa posição frota", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadConfiguracaoIntegracaoTecnologiaMonitoramentoConta() {

    _configuracaoIntegracaoTecnologiaMonitoramentoConta = new ConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
    KoBindings(_configuracaoIntegracaoTecnologiaMonitoramentoConta, "knockoutContas");

    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick }] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Nome", width: "40%" },
        { data: "Servidor", title: "Servidor", width: "40%" }
    ];

    _gridConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new BasicDataTable(_configuracaoIntegracaoTecnologiaMonitoramentoConta.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
}

function RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta() {

    let data = new Array();

    $.each(_configuracaoIntegracaoTecnologiaMonitoramento.Contas.list, function (i, conta) {
        let contaGrid = new Object();

        for (let prop in conta)
            contaGrid[prop] = conta[prop].val;

        data.push(contaGrid);
    });

    _gridConfiguracaoIntegracaoTecnologiaMonitoramentoConta.CarregarGrid(data);

}

function EditarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick(data) {
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Atualizar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Cancelar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Excluir.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Adicionar.visible(false);

    PreencherObjetoKnout(_configuracaoIntegracaoTecnologiaMonitoramentoConta, { Data: data });
}

function ExcluirConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirORegistro, function () {
        for (let i = 0; i < _configuracaoIntegracaoTecnologiaMonitoramento.Contas.list.length; i++) {
            if (_configuracaoIntegracaoTecnologiaMonitoramentoConta.Codigo.val() == _configuracaoIntegracaoTecnologiaMonitoramento.Contas.list[i].Codigo.val) {
                _configuracaoIntegracaoTecnologiaMonitoramento.Contas.list.splice(i, 1);
                break;
            }
        }

        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
    });
}

function AdicionarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick(e, sender) {
    let valido = ValidarCamposObrigatorios(_configuracaoIntegracaoTecnologiaMonitoramentoConta);

    if (valido) {
        _configuracaoIntegracaoTecnologiaMonitoramentoConta.Codigo.val(guid());

        _configuracaoIntegracaoTecnologiaMonitoramento.Contas.list.push(SalvarListEntity(_configuracaoIntegracaoTecnologiaMonitoramentoConta));

        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta();

        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);
    }
}

function AtualizarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick(e, sender) {
    let valido = ValidarCamposObrigatorios(_configuracaoIntegracaoTecnologiaMonitoramentoConta);

    if (valido) {
        for (let i = 0; i < _configuracaoIntegracaoTecnologiaMonitoramento.Contas.list.length; i++) {
            if (_configuracaoIntegracaoTecnologiaMonitoramentoConta.Codigo.val() == _configuracaoIntegracaoTecnologiaMonitoramento.Contas.list[i].Codigo.val) {
                _configuracaoIntegracaoTecnologiaMonitoramento.Contas.list[i] = SalvarListEntity(_configuracaoIntegracaoTecnologiaMonitoramentoConta);
                break;
            }
        }

        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
        LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoConta();

    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);
    }
}

function CancelarConfiguracaoIntegracaoTecnologiaMonitoramentoContaClick(data) {
    LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
}

function LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoConta() {
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Adicionar.visible(true);
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Atualizar.visible(false);
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Cancelar.visible(false);
    _configuracaoIntegracaoTecnologiaMonitoramentoConta.Excluir.visible(false);

    LimparCampos(_configuracaoIntegracaoTecnologiaMonitoramentoConta);
}