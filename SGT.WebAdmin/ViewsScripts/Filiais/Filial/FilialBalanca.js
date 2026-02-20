/// <reference path="Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _filialBalanca;
var _CRUDFilialBalanca;
var _gridFilialBalanca;

var FilialBalanca = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.MarcaBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.MarcaBalanca.getRequiredFieldDescription(), maxlength: 50, required: true });
    this.ModeloBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.ModeloBalanca.getRequiredFieldDescription(), maxlength: 50, required: true });
    this.HostConsultaBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.HostConsultaBalanca.getRequiredFieldDescription(), maxlength: 50, required: true });
    this.PortaBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PortaBalanca.getRequiredFieldDescription(), getType: typesKnockout.int, required: true });
    this.TamanhoRetornoBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.TamanhoRetornoBalanca.getFieldDescription(), getType: typesKnockout.int });
    this.PosicaoInicioPesoBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PosicaoInicioPesoBalanca.getFieldDescription(), getType: typesKnockout.int });
    this.PosicaoFimPesoBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PosicaoFimPesoBalanca.getFieldDescription(), getType: typesKnockout.int });
    this.CasasDecimaisPesoBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CasasDecimaisPesoBalanca.getFieldDescription(), getType: typesKnockout.int });
    this.QuantidadeConsultasPesoBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.QuantidadeConsultasPesoBalanca.getFieldDescription(), getType: typesKnockout.int });
    this.PercentualToleranciaPesoBalanca = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PercentualToleranciaPesoBalanca.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 6 });
    this.PercentualToleranciaPesagemEntrada = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PercentualToleranciaPesagemEntrada.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 6 });
    this.PercentualToleranciaPesagemSaida = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PercentualToleranciaPesagemSaida.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 6 });

    this.Grid = PropertyEntity({ type: types.local });
};

var CRUDFilialBalanca = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarBalancaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarBalancaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirBalancaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadFilialBalanca() {
    _filialBalanca = new FilialBalanca();
    KoBindings(_filialBalanca, "knockoutBalanca");

    _CRUDFilialBalanca = new CRUDFilialBalanca();
    KoBindings(_CRUDFilialBalanca, "knockoutCRUDBalanca");

    LoadGridFilialBalanca();
}

function AdicionarBalancaClick() {
    if (!ValidarCamposObrigatorios(_filialBalanca)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    _filialBalanca.Codigo.val(guid());
    _filial.Balancas.list.push(SalvarListEntity(_filialBalanca));

    limparCamposFilialBalanca();
}

function AtualizarBalancaClick() {
    if (!ValidarCamposObrigatorios(_filialBalanca)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    for (var i = 0; i < _filial.Balancas.list.length; i++) {
        if (_filialBalanca.Codigo.val() == _filial.Balancas.list[i].Codigo.val) {
            _filial.Balancas.list[i] = SalvarListEntity(_filialBalanca);
            break;
        }
    }

    limparCamposFilialBalanca();
}

function ExcluirBalancaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        for (var i = 0; i < _filial.Balancas.list.length; i++) {
            if (_filialBalanca.Codigo.val() == _filial.Balancas.list[i].Codigo.val) {
                _filial.Balancas.list.splice(i, 1);
                break;
            }
        }

        limparCamposFilialBalanca();
    });
}

////*******MÉTODOS*******

function LoadGridFilialBalanca() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarFilialBalancaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Marca", title: Localization.Resources.Filiais.Filial.MarcaBalanca, width: "30%" },
        { data: "Modelo", title: Localization.Resources.Filiais.Filial.ModeloBalanca, width: "30%" },
        { data: "HostConsulta", title: Localization.Resources.Filiais.Filial.HostConsultaBalanca, width: "20%" },
        { data: "Porta", title: Localization.Resources.Filiais.Filial.PortaBalanca, width: "20%" }
    ];

    _gridFilialBalanca = new BasicDataTable(_filialBalanca.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridFilialBalanca();
}

function EditarFilialBalancaClick(data) {
    for (var i = 0; i < _filial.Balancas.list.length; i++) {
        if (data.Codigo == _filial.Balancas.list[i].Codigo.val) {

            var item = _filial.Balancas.list[i];

            _filialBalanca.Codigo.val(item.Codigo.val);
            _filialBalanca.MarcaBalanca.val(item.MarcaBalanca.val);
            _filialBalanca.ModeloBalanca.val(item.ModeloBalanca.val);
            _filialBalanca.HostConsultaBalanca.val(item.HostConsultaBalanca.val);
            _filialBalanca.PortaBalanca.val(item.PortaBalanca.val);

            _filialBalanca.TamanhoRetornoBalanca.val(item.TamanhoRetornoBalanca.val);
            _filialBalanca.PosicaoInicioPesoBalanca.val(item.PosicaoInicioPesoBalanca.val);
            _filialBalanca.PosicaoFimPesoBalanca.val(item.PosicaoFimPesoBalanca.val);
            _filialBalanca.CasasDecimaisPesoBalanca.val(item.CasasDecimaisPesoBalanca.val);
            _filialBalanca.QuantidadeConsultasPesoBalanca.val(item.QuantidadeConsultasPesoBalanca.val);
            _filialBalanca.PercentualToleranciaPesoBalanca.val(item.PercentualToleranciaPesoBalanca.val);
            _filialBalanca.PercentualToleranciaPesagemEntrada.val(item.PercentualToleranciaPesagemEntrada.val);
            _filialBalanca.PercentualToleranciaPesagemSaida.val(item.PercentualToleranciaPesagemSaida.val);

            _CRUDFilialBalanca.Adicionar.visible(false);
            _CRUDFilialBalanca.Atualizar.visible(true);
            _CRUDFilialBalanca.Excluir.visible(true);

            break;
        }
    }
}

function RecarregarGridFilialBalanca() {
    var data = new Array();

    $.each(_filial.Balancas.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Marca = item.MarcaBalanca.val;
        itemGrid.Modelo = item.ModeloBalanca.val;
        itemGrid.HostConsulta = item.HostConsultaBalanca.val;
        itemGrid.Porta = item.PortaBalanca.val;

        data.push(itemGrid);
    });

    _gridFilialBalanca.CarregarGrid(data);
}

function limparCamposFilialBalanca() {
    LimparCampos(_filialBalanca);

    _CRUDFilialBalanca.Adicionar.visible(true);
    _CRUDFilialBalanca.Atualizar.visible(false);
    _CRUDFilialBalanca.Excluir.visible(false);

    RecarregarGridFilialBalanca();
}