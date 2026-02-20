var _gridObservacaoCTe, _observacaoCTe;

var ObservacaoCTe = function () {
    var self = this;

    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoObservacaoCTe.Contribuinte), options: EnumTipoObservacaoCTe.ObterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.Tipo.getRequiredFieldDescription(), def: EnumTipoObservacaoCTe.Contribuinte, visible: ko.observable(true) });
    this.Identificador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Identificador.getRequiredFieldDescription(), maxlength: 20, visible: ko.observable(true) });
    this.Texto = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Texto.getRequiredFieldDescription(), maxlength: 160, visible: ko.observable(true) });

    this.TagNumeroCarga = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroDaCarga, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNumeroPedidoEmbarcador = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#NumeroPedidoEmbarcador"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroDoPedidoDoEmbarcado, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagPlacaTracao = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#PlacaTracao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.PlacaDaTracao, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNumeroNotaFiscal = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#NumeroNotaFiscal"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroDaNotaFiscal, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNumeroReferenciaEDI = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#NumeroReferenciaEDI"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroDeReferenciaEDI, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCodigoOBSTerminalDestino = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#CodigoOBSTerminalDestino"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CodigoObservacaoContribuinteTerminalDestino, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCodigoOBSTerminalOrigem = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#CodigoOBSTerminalOrigem"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CodigoObservacaoContribuinteTerminalOrigem, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagPlacasReboque = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#PlacasReboque"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.PlacasReboque, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCPFMotorista = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#CPFMotorista"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CPFMotorista, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNomeMotorista = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#NomeMotorista"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NomeMotorista, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCodigoIntegracaoTipoOperacao = PropertyEntity({ eventClick: function (e) { InserirTag(self.Texto.id, "#CodigoIntegracaoTipoOperacao"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CodigoIntegracaoTipoOperacao, enable: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarObservacaoCTeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadObservacaoCTe() {

    _observacaoCTe = new ObservacaoCTe();
    KoBindings(_observacaoCTe, "knockoutObservacaoCTe");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: ExcluirObservacaoCTeClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Pessoas.GrupoPessoas.Tipo, width: "20%" },
        { data: "Identificador", title: Localization.Resources.Pessoas.GrupoPessoas.Identificador, width: "20%" },
        { data: "Texto", title: Localization.Resources.Pessoas.GrupoPessoas.Texto, width: "40%" }
    ];

    _gridObservacaoCTe = new BasicDataTable(_observacaoCTe.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.asc });

    RecarregarGridObservacaoCTe();
}

function RecarregarGridObservacaoCTe() {

    var data = new Array();

    $.each(_grupoPessoas.ObservacoesCTes.list, function (i, observacao) {
        var observacaoGrid = new Object();

        observacaoGrid.Codigo = observacao.Codigo.val;
        observacaoGrid.Tipo = observacao.Tipo.val;
        observacaoGrid.DescricaoTipo = EnumTipoObservacaoCTe.ObterDescricao(observacao.Tipo.val);
        observacaoGrid.Identificador = observacao.Identificador.val;
        observacaoGrid.Texto = observacao.Texto.val;

        data.push(observacaoGrid);
    });

    _gridObservacaoCTe.CarregarGrid(data);
}


function ExcluirObservacaoCTeClick(data) {
    for (var i = 0; i < _grupoPessoas.ObservacoesCTes.list.length; i++) {
        if (data.Codigo == _grupoPessoas.ObservacoesCTes.list[i].Codigo.val) {
            _grupoPessoas.ObservacoesCTes.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridObservacaoCTe();
}

function AdicionarObservacaoCTeClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_observacaoCTe);

    if (valido) {

        _observacaoCTe.Codigo.val(guid());

        _grupoPessoas.ObservacoesCTes.list.push(SalvarListEntity(_observacaoCTe));

        RecarregarGridObservacaoCTe();

        LimparCamposObservacaoCTe();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function LimparCamposObservacaoCTe() {
    LimparCampos(_observacaoCTe);
}