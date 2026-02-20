/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoDespesaFinanceira.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _regiaoPrazoEntrega, _gridRegiaoPrazoEntrega;

var RegiaoPrazoEntrega = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.PadraoTempo = PropertyEntity({ val: ko.observable(EnumPadraoTempoDiasMinutos.Minutos), options: EnumPadraoTempoDiasMinutos.obterOpcoes(), def: EnumPadraoTempoDiasMinutos.Minutos, text: "*Padrão de Tempo:" });
    this.TempoDeViagemEmMinutos = PropertyEntity({ val: ko.observable(""), text: "*Tempo de Viagem (Minutos):", issue: 834, visible: ko.observable(true), required: ko.observable(true) });
    this.TempoDeViagemEmDias = PropertyEntity({ text: "*Tempo de Viagem (Dias):", issue: 834, val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 2, visible: ko.observable(false), required: ko.observable(false) });

    this.PadraoTempo.val.subscribe(function (valor) {
        if (valor == EnumPadraoTempoDiasMinutos.Minutos) {
            _regiaoPrazoEntrega.TempoDeViagemEmMinutos.visible(true);
            _regiaoPrazoEntrega.TempoDeViagemEmMinutos.required(true);
            _regiaoPrazoEntrega.TempoDeViagemEmDias.visible(false);
            _regiaoPrazoEntrega.TempoDeViagemEmDias.required(false);
        }
        else if (valor == EnumPadraoTempoDiasMinutos.Dias) {
            _regiaoPrazoEntrega.TempoDeViagemEmDias.visible(true);
            _regiaoPrazoEntrega.TempoDeViagemEmDias.required(true);
            _regiaoPrazoEntrega.TempoDeViagemEmMinutos.visible(false);
            _regiaoPrazoEntrega.TempoDeViagemEmMinutos.required(false);
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarRegiaoPrazoEntregaClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadRegiaoPrazoEntrega() {
    _regiaoPrazoEntrega = new RegiaoPrazoEntrega();
    KoBindings(_regiaoPrazoEntrega, "knockoutRegiaoPrazoEntrega");

    new BuscarTiposdeCarga(_regiaoPrazoEntrega.TipoDeCarga);
    new BuscarFilial(_regiaoPrazoEntrega.Filial);
    new BuscarTiposOperacao(_regiaoPrazoEntrega.TipoOperacao);

    LoadGridRegiaoPrazoEntrega();
}

function AdicionarRegiaoPrazoEntregaClick() {
    if (!ValidarCamposObrigatorios(_regiaoPrazoEntrega)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    var existe = false;
    for (var i = 0; i < _regiao.RegioesPrazoEntrega.list.length; i++) {
        if (_regiaoPrazoEntrega.Filial.codEntity() == _regiao.RegioesPrazoEntrega.list[i].Filial.codEntity && _regiaoPrazoEntrega.TipoDeCarga.codEntity() == _regiao.RegioesPrazoEntrega.list[i].TipoDeCarga.codEntity && _regiaoPrazoEntrega.TipoOperacao.codEntity() == _regiao.RegioesPrazoEntrega.list[i].TipoOperacao.codEntity) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "Já existe uma regra cadastrada com esses dados.");
            existe = true;
            break;
        }
    }
    if (existe)
        return;

    _regiaoPrazoEntrega.Codigo.val(guid());
    _regiao.RegioesPrazoEntrega.list.push(SalvarListEntity(_regiaoPrazoEntrega));

    LimparCamposRegiaoPrazoEntrega();
}

function ExcluirRegiaoPrazoEntregaClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o registro?", function () {
        for (var i = 0; i < _regiao.RegioesPrazoEntrega.list.length; i++) {
            if (data.Codigo == _regiao.RegioesPrazoEntrega.list[i].Codigo.val) {
                _regiao.RegioesPrazoEntrega.list.splice(i, 1);
                break;
            }
        }

        LimparCamposRegiaoPrazoEntrega();
    });
}

////*******MÉTODOS*******

function LoadGridRegiaoPrazoEntrega() {
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { ExcluirRegiaoPrazoEntregaClick(data); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Filial", title: "Filial", width: "30%" },
        { data: "TipoOperacao", title: "Tipo de Operação", width: "40%" },
        { data: "TipoDeCarga", title: "Tipo de Carga", width: "40%" },
        { data: "PadraoTempo", title: "Padrão Tempo", width: "30%" },
        { data: "Tempo", title: "Tempo (Dias/Minutos)", width: "30%" }
    ];

    _gridRegiaoPrazoEntrega = new BasicDataTable(_regiaoPrazoEntrega.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridRegiaoPrazoEntrega();
}

function RecarregarGridRegiaoPrazoEntrega() {
    var data = new Array();
    $.each(_regiao.RegioesPrazoEntrega.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.TipoOperacao = item.TipoOperacao.val;
        itemGrid.TipoDeCarga = item.TipoDeCarga.val;
        itemGrid.Filial = item.Filial.val;
        itemGrid.Tempo = item.TempoDeViagemEmDias.val > 0 ? item.TempoDeViagemEmDias.val : item.TempoDeViagemEmMinutos.val;
        itemGrid.PadraoTempo = EnumPadraoTempoDiasMinutos.obterDescricao(item.PadraoTempo.val);

        data.push(itemGrid);
    });

    _gridRegiaoPrazoEntrega.CarregarGrid(data);
}

function LimparCamposRegiaoPrazoEntrega() {
    LimparCampos(_regiaoPrazoEntrega);
    RecarregarGridRegiaoPrazoEntrega();
}