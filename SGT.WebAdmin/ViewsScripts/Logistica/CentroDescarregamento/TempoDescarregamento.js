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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoMontagemCarregamentoVrp.js" />
/// <reference path="CentroDescarregamento.js" />
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTempoDescarregamento;
var _tempoDescarregamento;
var _opcoesTipoCarga = new Array();

var TempoDescarregamento = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.TempoPadraoDeEntrega = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.TempoPadraoEntregaMinutos, type: types.local, getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoPadraoDeEntrega.val.subscribe(function (v) { _centroDescarregamento.TempoPadraoDeEntrega.val(v) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 44 });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Gerais.Geral.TipoCarga, idBtnSearch: guid() });
    this.Tempo = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.TempoMinutos.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true), required: true, maxlength: 3 });
    this.TipoTempo = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.TipoTempo.getFieldDescription(), val: ko.observable(EnumTempoDescarregamentoTipoTempo.Cliente), options: EnumTempoDescarregamentoTipoTempo.obterOpcoes() });

    this.SkuDe = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.QtdItensDe.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true), maxlength: 10 });
    this.SkuAte = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.QtdItensAte.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true), maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarTempoDescarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarTempoDescarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarTempoDescarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirTempoDescarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadTempoDescarregamento() {
    _tempoDescarregamento = new TempoDescarregamento();
    KoBindings(_tempoDescarregamento, "knockoutTempoDescarregamento");

    new BuscarModelosVeicularesCarga(_tempoDescarregamento.ModeloVeiculo, null, null, null, null, null, null, _tempoDescarregamento.TipoCarga);
    new BuscarTiposdeCarga(_tempoDescarregamento.TipoCarga);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarTempoDescarregamentoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoTipoCarga", title: Localization.Resources.Gerais.Geral.TipoCarga, width: "20%" },
        { data: "DescricaoModeloVeiculo", title: Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculo, width: "20%" },
        { data: "SKU", title: Localization.Resources.Logistica.CentroDescarregamento.QtdItens, width: "15%" },
        { data: "TipoTempo", title: Localization.Resources.Logistica.CentroDescarregamento.TipoTempo, width: "15%" },
        { data: "Tempo", title: Localization.Resources.Logistica.CentroDescarregamento.Tempo, width: "15%" }
    ];

    _gridTempoDescarregamento = new BasicDataTable(_tempoDescarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridTempoDescarregamento();
}

function RecarregarGridTempoDescarregamento() {

    var data = new Array();

    $.each(_centroDescarregamento.TemposDescarregamento.list, function (i, tempoDescarregamento) {
        var tempoDescarregamentoGrid = new Object();

        tempoDescarregamentoGrid.Codigo = tempoDescarregamento.Codigo.val;
        tempoDescarregamentoGrid.CodigoTipoCarga = tempoDescarregamento.TipoCarga.codEntity;
        tempoDescarregamentoGrid.DescricaoTipoCarga = tempoDescarregamento.TipoCarga.val;

        var sku = "";
        if (tempoDescarregamento.SkuDe.val) sku += Localization.Resources.Logistica.CentroDescarregamento.De + tempoDescarregamento.SkuDe.val;
        if (tempoDescarregamento.SkuAte.val) sku += Localization.Resources.Logistica.CentroDescarregamento.Ate + tempoDescarregamento.SkuAte.val;
        tempoDescarregamentoGrid.SKU = sku.trim();

        tempoDescarregamentoGrid.CodigoModeloVeiculo = tempoDescarregamento.ModeloVeiculo.codEntity;
        tempoDescarregamentoGrid.DescricaoModeloVeiculo = tempoDescarregamento.ModeloVeiculo.val;
        tempoDescarregamentoGrid.TipoTempo = EnumTempoDescarregamentoTipoTempo.obterDescricao(tempoDescarregamento.TipoTempo.val);
        tempoDescarregamentoGrid.Tempo = tempoDescarregamento.Tempo.val + Localization.Resources.Logistica.CentroDescarregamento.Minutos;

        data.push(tempoDescarregamentoGrid);
    });

    _gridTempoDescarregamento.CarregarGrid(data);
}

function EditarTempoDescarregamentoClick(data) {
    var tempoCarregamento = BuscarTemposDescarregamentoPorCodigo(data.Codigo);

    if (tempoCarregamento == null)
        return exibirMensagem(tipoMensagem.falha, Localization.Resources.Logistica.CentroDescarregamento.EditarTempoDescarregamento, Localization.Resources.Logistica.CentroDescarregamento.OcorreuUmaFalhaBuscarDadosEditar);

    // Carrega os campos para editar
    _tempoDescarregamento.Codigo.val(tempoCarregamento.Codigo.val);
    _tempoDescarregamento.ModeloVeiculo.val(tempoCarregamento.ModeloVeiculo.val);
    _tempoDescarregamento.ModeloVeiculo.codEntity(tempoCarregamento.ModeloVeiculo.codEntity);
    _tempoDescarregamento.TipoCarga.val(tempoCarregamento.TipoCarga.val);
    _tempoDescarregamento.TipoCarga.codEntity(tempoCarregamento.TipoCarga.codEntity);
    _tempoDescarregamento.Tempo.val(tempoCarregamento.Tempo.val);
    _tempoDescarregamento.TipoTempo.val(tempoCarregamento.TipoTempo.val);
    _tempoDescarregamento.SkuDe.val(tempoCarregamento.SkuDe.val);
    _tempoDescarregamento.SkuAte.val(tempoCarregamento.SkuAte.val);

    // Habilita botoes
    _tempoDescarregamento.Adicionar.visible(false);
    _tempoDescarregamento.Atualizar.visible(true);
    _tempoDescarregamento.Cancelar.visible(true);
    _tempoDescarregamento.Excluir.visible(true);
}

function ExcluirTempoDescarregamentoClick() {
    ExcluirTemposDescarregamentoPorCodigo(_tempoDescarregamento.Codigo.val());

    RecarregarGridTempoDescarregamento();
    LimparCamposTempoDescarregamento();
}

function CancelarTempoDescarregamentoClick() {
    LimparCamposTempoDescarregamento();
}

function AtualizarTempoDescarregamentoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_tempoDescarregamento);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatororios);
        return;
    }

    for (var i = 0; i < _centroDescarregamento.TemposDescarregamento.list.length; i++) {
        var tempoAValidar = _centroDescarregamento.TemposDescarregamento.list[i];

        if (tempoAValidar.TipoCarga.val == _tempoDescarregamento.TipoCarga.val() &&
            tempoAValidar.ModeloVeiculo.codEntity == _tempoDescarregamento.ModeloVeiculo.codEntity() &&
            tempoAValidar.Codigo.val != _tempoDescarregamento.Codigo.val()
        ) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.TempoJaExiste, Localization.Resources.Logistica.CentroDescarregamento.JaFoiCadastradoTempoTipoCargaModeloVeicularSelecionados);
            return;
        }
    }

    var skuDe = parseInt(_tempoDescarregamento.SkuDe.val()) || 0;
    var skuAte = parseInt(_tempoDescarregamento.SkuAte.val());

    if (!isNaN(skuAte) && skuDe > skuAte) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.FaixaItens, "O valor de \"Qtd. de Itens De\" deve ser menor que \"Qtd. de Itens Até\".");
        return false;
    }

    var tempoDescarregamento = SalvarListEntity(_tempoDescarregamento);
    AtualizarTemposDescarregamentoPorCodigo(_tempoDescarregamento.Codigo.val(), tempoDescarregamento);

    RecarregarGridTempoDescarregamento();
    LimparCamposTempoDescarregamento();
}

function AdicionarTempoDescarregamentoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_tempoDescarregamento);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatororios);
        return;
    }

    for (var i = 0; i < _centroDescarregamento.TemposDescarregamento.list.length; i++) {
        if (_centroDescarregamento.TemposDescarregamento.list[i].TipoCarga.val == _tempoDescarregamento.TipoCarga.val() && _centroDescarregamento.TemposDescarregamento.list[i].ModeloVeiculo.codEntity == _tempoDescarregamento.ModeloVeiculo.codEntity()) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.TempoJaExiste, Localization.Resources.Logistica.CentroDescarregamento.JaFoiCadastradoTempoTipoCargaModeloVeicularSelecionados);
            return;
        }
    }

    var skuDe = parseInt(_tempoDescarregamento.SkuDe.val()) || 0;
    var skuAte = parseInt(_tempoDescarregamento.SkuAte.val());

    if (!isNaN(skuAte) && skuDe > skuAte) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.FaixaItens, "O valor de \"Qtd. de Itens De\" deve ser menor que \"Qtd. de Itens Até\".");
        return false;
    }

    _tempoDescarregamento.Codigo.val(guid());
    _centroDescarregamento.TemposDescarregamento.list.push(SalvarListEntity(_tempoDescarregamento));

    RecarregarGridTempoDescarregamento();
    LimparCamposTempoDescarregamento();
}

function LimparCamposTempoDescarregamento() {
    LimparCampos(_tempoDescarregamento);

    _tempoDescarregamento.Adicionar.visible(true);
    _tempoDescarregamento.Atualizar.visible(false);
    _tempoDescarregamento.Cancelar.visible(false);
    _tempoDescarregamento.Excluir.visible(false);
}

function ChangeTipoCarga(novoValor) {
    _tempoDescarregamento.ModeloVeiculo.codEntity(0);
    _tempoDescarregamento.ModeloVeiculo.val("");
}



//*******MÉTODOS*******

function BuscarTemposDescarregamentoPorCodigo(codigo) {
    // Busca na lista de tempo de descarregamentos o item por codigo
    for (var i = 0; i < _centroDescarregamento.TemposDescarregamento.list.length; i++) {
        if (codigo == _centroDescarregamento.TemposDescarregamento.list[i].Codigo.val) {
            return _centroDescarregamento.TemposDescarregamento.list[i];
        }
    }

    // Se nao encontra, retorna nullo
    return null;
}

function AtualizarTemposDescarregamentoPorCodigo(codigo, tempoDescarregamento) {
    // Busca na lista de tempo de descarregamentos o item por codigo e seta o novo objeto
    for (var i = 0; i < _centroDescarregamento.TemposDescarregamento.list.length; i++) {
        if (codigo == _centroDescarregamento.TemposDescarregamento.list[i].Codigo.val) {
            _centroDescarregamento.TemposDescarregamento.list[i] = tempoDescarregamento;
            break;
        }
    }
}

function ExcluirTemposDescarregamentoPorCodigo(codigo) {
    // Busca na lista de tempo de descarregamentos o item por codigo e recome com splice
    for (var i = 0; i < _centroDescarregamento.TemposDescarregamento.list.length; i++) {
        if (codigo == _centroDescarregamento.TemposDescarregamento.list[i].Codigo.val) {
            _centroDescarregamento.TemposDescarregamento.list.splice(i, 1);
            break;
        }
    }
}