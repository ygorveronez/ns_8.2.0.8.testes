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
/// <reference path="../../Enumeradores/EnumTipoClienteCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumStatusCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="TabelaComissaoMotorista.js" />

var _bonificacao;
var _gridMedias, _gridRepresentacaos, _gridFaturamentoDia, _gridRotasFretes;

var MediaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.MediaInicial = PropertyEntity({ type: types.map, val: "" });
    this.MediaFinal = PropertyEntity({ type: types.map, val: "" });
    this.PercentualAcrescimoComissaoMedia = PropertyEntity({ type: types.map, val: "" });
    this.ValorBonificacaoMedia = PropertyEntity({ type: types.map, val: "" });
    this.CodigoJustificativaBonificacaoMedia = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoJustificativaBonificacaoMedia = PropertyEntity({ type: types.map, val: "" });
}

var FaturamentoDiaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.FaturamentoInicial = PropertyEntity({ type: types.map, val: "" });
    this.FaturamentoFinal = PropertyEntity({ type: types.map, val: "" });
    this.PercentualAcrescimoComissaoFaturamentoDia = PropertyEntity({ type: types.map, val: "" });
}

var RepresentacaoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.PercentualRepresentacao = PropertyEntity({ type: types.map, val: "" });
    this.PercentualAcrescimoComissaoRepresentacao = PropertyEntity({ type: types.map, val: "" });
    this.ValorBonificacaoRepresentacao = PropertyEntity({ type: types.map, val: "" });
    this.CodigoJustificativaBonificacaoRepresentacao = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoJustificativaBonificacaoRepresentacao = PropertyEntity({ type: types.map, val: "" });
}

var RotaFreteMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoRotaFrete = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoRotaFrete = PropertyEntity({ type: types.map, val: "" });
    this.ValorBonificacaoRotaFrete = PropertyEntity({ type: types.map, val: "" });
    this.CodigoJustificativaBonificacaoRotaFrete = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoJustificativaBonificacaoRotaFrete = PropertyEntity({ type: types.map, val: "" });
}

var Bonificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AtivarBonificacaoMediaCombustivel = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Ativar bonificação por média de combustível? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.MediaInicial = PropertyEntity({ text: "*Média Inicial:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true }, required: ko.observable(false) });
    this.MediaFinal = PropertyEntity({ text: "*Média Final:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false) });
    this.PercentualAcrescimoComissaoMedia = PropertyEntity({ text: "% + Comissão:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false) });
    this.ValorBonificacaoMedia = PropertyEntity({ text: "Valor Bonificação:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false) });
    this.JustificativaBonificacaoMedia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.AdicionarMedia = PropertyEntity({ type: types.event, eventClick: AdicionarMediaClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.Medias = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AtivarBonificacaoRepresentacaoCombustivel = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Ativar bonificação por representação do consumo do diesel? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.PercentualRepresentacao = PropertyEntity({ text: "*% Representação:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false) });
    this.PercentualAcrescimoComissaoRepresentacao = PropertyEntity({ text: "% + Comissão:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true }, required: ko.observable(false) });
    this.ValorBonificacaoRepresentacao = PropertyEntity({ text: "Valor Bonificação:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true }, required: ko.observable(false) });
    this.JustificativaBonificacaoRepresentacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.AdicionarRepresentacao = PropertyEntity({ type: types.event, eventClick: AdicionarRepresentacaoClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.Representacaos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AtivarBonificacaoFaturamentoDia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Ativar comissão por faixa de faturamento? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.FaturamentoInicial = PropertyEntity({ text: "*Faturamento Inicial:", getType: typesKnockout.decimal, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true }, required: ko.observable(false) });
    this.FaturamentoFinal = PropertyEntity({ text: "*Faturamento Final:", getType: typesKnockout.decimal, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: false }, required: ko.observable(false) });
    this.PercentualAcrescimoComissaoFaturamentoDia = PropertyEntity({ text: "% + Comissão:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false) });    
    this.AdicionarFaturamentoDia = PropertyEntity({ type: types.event, eventClick: AdicionarFaturamentoDiaClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.FaturamentoDia = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AtivarBonificacaoRotaFrete = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Ativar bonificação por Rota Frete? ", idFade: guid(), visibleFade: ko.observable(false) });   
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Rota Frete:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.ValorBonificacaoRotaFrete = PropertyEntity({ text: "*Valor Bonificação:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false) });
    this.JustificativaBonificacaoRotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.AdicionarRotaFrete = PropertyEntity({ type: types.event, eventClick: AdicionarRotaFreteClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.RotasFretes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AtivarBonificacaoMediaCombustivel.val.subscribe(function (novoValor) {
        if (novoValor) {
            _bonificacao.AtivarBonificacaoMediaCombustivel.visibleFade(true);
            _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.val(false);
            _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.visibleFade(false);
            _bonificacao.AtivarBonificacaoFaturamentoDia.val(false);
            _bonificacao.AtivarBonificacaoFaturamentoDia.visibleFade(false);
            _bonificacao.AtivarBonificacaoRotaFrete.val(false);
            _bonificacao.AtivarBonificacaoRotaFrete.visibleFade(false);            
        }
    });

    this.AtivarBonificacaoRepresentacaoCombustivel.val.subscribe(function (novoValor) {
        if (novoValor) {
            _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.visibleFade(true);
            _bonificacao.AtivarBonificacaoMediaCombustivel.val(false);
            _bonificacao.AtivarBonificacaoMediaCombustivel.visibleFade(false);
            _bonificacao.AtivarBonificacaoFaturamentoDia.val(false);
            _bonificacao.AtivarBonificacaoFaturamentoDia.visibleFade(false);
            _bonificacao.AtivarBonificacaoRotaFrete.val(false);
            _bonificacao.AtivarBonificacaoRotaFrete.visibleFade(false);
        }
    });

    this.AtivarBonificacaoFaturamentoDia.val.subscribe(function (novoValor) {
        if (novoValor) {
            _bonificacao.AtivarBonificacaoFaturamentoDia.visibleFade(true);
            _bonificacao.AtivarBonificacaoMediaCombustivel.val(false);
            _bonificacao.AtivarBonificacaoMediaCombustivel.visibleFade(false);
            _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.val(false);
            _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.visibleFade(false);
            _bonificacao.AtivarBonificacaoRotaFrete.val(false);
            _bonificacao.AtivarBonificacaoRotaFrete.visibleFade(false);
        }
    });   

    this.AtivarBonificacaoRotaFrete.val.subscribe(function (novoValor) {
        if (novoValor) {
            _bonificacao.AtivarBonificacaoRotaFrete.visibleFade(true);
            _bonificacao.AtivarBonificacaoFaturamentoDia.val(false);
            _bonificacao.AtivarBonificacaoFaturamentoDia.visibleFade(false);
            _bonificacao.AtivarBonificacaoMediaCombustivel.val(false);
            _bonificacao.AtivarBonificacaoMediaCombustivel.visibleFade(false);
            _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.val(false);
            _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.visibleFade(false);                        
        }
    }); 

};

//*******EVENTOS*******

function loadBonificacao() {
    _bonificacao = new Bonificacao();
    KoBindings(_bonificacao, "knockoutBonificacao");

    BuscarRotasFrete(_bonificacao.RotaFrete);
    BuscarJustificativas(_bonificacao.JustificativaBonificacaoRotaFrete, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas]);
    BuscarJustificativas(_bonificacao.JustificativaBonificacaoMedia, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas]);
    BuscarJustificativas(_bonificacao.JustificativaBonificacaoRepresentacao, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas]);

    carregarGridMedia();
    carregarGridRotaFrete();
    carregarGridFaturamentoDia();
    carregarGridRepresentacao();
}

//********MEDIA********

function carregarGridMedia() {
    const excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirMedia(data);
        }, tamanho: "10", icone: ""
    };
    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    const header = [{ data: "Codigo", visible: false },
    { data: "CodigoJustificativaBonificacaoMedia", visible: false },
    { data: "MediaInicial", title: "Media Inicial", width: "10%" },
    { data: "MediaFinal", title: "Media Final", width: "10%" },
    { data: "PercentualAcrescimoComissaoMedia", title: "% + Comissão", width: "10%" },
    { data: "ValorBonificacaoMedia", title: "Valor Bonificação", width: "10%" },
    { data: "DescricaoJustificativaBonificacaoMedia", title: "Justificativa", width: "30%" }];

    _gridMedias = new BasicDataTable(_bonificacao.Medias.idGrid, header, menuOpcoes);
    RecarregarGridMedia();
}

function excluirMedia(e) {
    for (let i = 0; i < _bonificacao.Medias.list.length; i++) {
        const media = _bonificacao.Medias.list[i];
        if (media?.Codigo !== null && e !== null && e.Codigo !== null && e.Codigo === media?.Codigo) {
            _bonificacao.Medias.list.splice(i, 1);
            break;
        }
    }
    RecarregarGridMedia();
}

function RecarregarGridMedia() {
    const data = new Array();

    for (let i = 0; i < _bonificacao.Medias.list.length; i++) {
        const media = _bonificacao.Medias.list[i];

        data.push({
            Codigo: media.Codigo,
            CodigoJustificativaBonificacaoMedia: media.CodigoJustificativaBonificacaoMedia,
            MediaInicial: media.MediaInicial,
            MediaFinal: media.MediaFinal,
            PercentualAcrescimoComissaoMedia: media.PercentualAcrescimoComissaoMedia,
            ValorBonificacaoMedia: media.ValorBonificacaoMedia,
            DescricaoJustificativaBonificacaoMedia: media.DescricaoJustificativaBonificacaoMedia,
        });
    }
    _gridMedias.CarregarGrid(data);
}

function AdicionarMediaClick(e, sender) {
    let tudoCerto = true;
    if (_bonificacao.MediaFinal.val() === "" || _bonificacao.MediaFinal.val() === "0,00")
        tudoCerto = false;
    if (_bonificacao.ValorBonificacaoMedia.val() !== "" && _bonificacao.ValorBonificacaoMedia.val() !== "0,00" && (_bonificacao.JustificativaBonificacaoMedia.codEntity() === 0 || _bonificacao.JustificativaBonificacaoMedia.codEntity() === ""))
        tudoCerto = false;

    if (tudoCerto) {
        const mediaFinal = Globalize.parseFloat(_bonificacao.MediaFinal.val());
        const mediaInicial = Globalize.parseFloat(_bonificacao.MediaInicial.val());
        if (mediaInicial > mediaFinal)
            return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "A média inicial não pode ser maior que a final!");
    }

    if (tudoCerto) {
        const map = new Object();

        map.Codigo = guid();
        map.MediaInicial = _bonificacao.MediaInicial.val();
        map.MediaFinal = _bonificacao.MediaFinal.val();
        map.PercentualAcrescimoComissaoMedia = _bonificacao.PercentualAcrescimoComissaoMedia.val();
        map.ValorBonificacaoMedia = _bonificacao.ValorBonificacaoMedia.val();
        map.DescricaoJustificativaBonificacaoMedia = _bonificacao.JustificativaBonificacaoMedia.val();
        map.CodigoJustificativaBonificacaoMedia = _bonificacao.JustificativaBonificacaoMedia.codEntity();

        _bonificacao.Medias.list.push(map);

        RecarregarGridMedia();
        limparDadosMedia();
        $("#" + _bonificacao.MediaInicial.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento da faixa de média!");
    }
}

function limparDadosMedia() {
    LimparCampoEntity(_bonificacao.JustificativaBonificacaoMedia);
    _bonificacao.MediaInicial.val("");
    _bonificacao.MediaFinal.val("");
    _bonificacao.PercentualAcrescimoComissaoMedia.val("");
    _bonificacao.ValorBonificacaoMedia.val("");
}


//********ROTA FRETE********

function carregarGridRotaFrete() {
    const excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirRotaFrete(data);
        }, tamanho: "10", icone: ""
    };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    const header = [{ data: "Codigo", visible: false },
        { data: "CodigoJustificativaBonificacaoRotaFrete", visible: false },    
        { data: "CodigoRotaFrete", visible: false },    
        { data: "DescricaoRotaFrete", title: "Rota Frete", width: "30%" },
        { data: "ValorBonificacaoRotaFrete", title: "Valor Bonificação", width: "10%" },
        { data: "DescricaoJustificativaBonificacaoRotaFrete", title: "Justificativa", width: "30%" }];

    _gridRotasFretes = new BasicDataTable(_bonificacao.RotasFretes.idGrid, header, menuOpcoes);
    RecarregarGridRotaFrete();
}

function excluirRotaFrete(e) {
    for (let i = 0; i < _bonificacao.RotasFretes.list.length; i++) {
        const rotaFrete = _bonificacao.RotasFretes.list[i];
        if (rotaFrete !== null && rotaFrete?.Codigo !== null && e !== null && e.Codigo !== null && e.Codigo === rotaFrete?.Codigo) {
            _bonificacao.RotasFretes.list.splice(i, 1);
        }
    };
    RecarregarGridRotaFrete();
}

function RecarregarGridRotaFrete() {
    const data = new Array();

    for (let i = 0; i < _bonificacao.RotasFretes.list.length; i++) {
        const rotaFrete = _bonificacao.RotasFretes.list[i];

        data.push({
            Codigo: rotaFrete.Codigo,
            CodigoJustificativaBonificacaoRotaFrete: rotaFrete.CodigoJustificativaBonificacaoRotaFrete,
            CodigoRotaFrete: rotaFrete.CodigoRotaFrete,
            ValorBonificacaoRotaFrete: rotaFrete.ValorBonificacaoRotaFrete,
            DescricaoJustificativaBonificacaoRotaFrete: rotaFrete.DescricaoJustificativaBonificacaoRotaFrete,
            DescricaoRotaFrete: rotaFrete.DescricaoRotaFrete,
        });
    };
    _gridRotasFretes.CarregarGrid(data);
}

function AdicionarRotaFreteClick(e, sender) {
    let tudoCerto = true;
    if (_bonificacao.ValorBonificacaoRotaFrete.val() === "" || _bonificacao.ValorBonificacaoRotaFrete.val() === "0,00" || (_bonificacao.JustificativaBonificacaoRotaFrete.codEntity() === 0 || _bonificacao.JustificativaBonificacaoRotaFrete.codEntity() === "") || (_bonificacao.RotaFrete.codEntity() === 0 || _bonificacao.RotaFrete.codEntity() === ""))
        tudoCerto = false;

    if (tudoCerto) {
        const map = new Object();

        map.Codigo = guid();
        map.ValorBonificacaoRotaFrete = _bonificacao.ValorBonificacaoRotaFrete.val();
        map.DescricaoJustificativaBonificacaoRotaFrete = _bonificacao.JustificativaBonificacaoRotaFrete.val();
        map.CodigoJustificativaBonificacaoRotaFrete = _bonificacao.JustificativaBonificacaoRotaFrete.codEntity();
        map.DescricaoRotaFrete = _bonificacao.RotaFrete.val();
        map.CodigoRotaFrete = _bonificacao.RotaFrete.codEntity();

        _bonificacao.RotasFretes.list.push(map);

        RecarregarGridRotaFrete();
        limparDadosRotaFrete();
        $("#" + _bonificacao.RotaFrete.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento da bonificação por rota!");
    }
}

function limparDadosRotaFrete() {
    LimparCampoEntity(_bonificacao.JustificativaBonificacaoRotaFrete);
    LimparCampoEntity(_bonificacao.RotaFrete);
    _bonificacao.PercentualAcrescimoComissaoRotaFrete.val("");
    _bonificacao.ValorBonificacaoRotaFrete.val("");
}

//********FATURAMENTO DIA********

function carregarGridFaturamentoDia() {
    const excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirFaturamentoDia(data);
        }, tamanho: "10", icone: ""
    };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    const header = [{ data: "Codigo", visible: false },    
        { data: "FaturamentoInicial", title: "Fat. Inicial", width: "20%" },
        { data: "FaturamentoFinal", title: "Fat. Final", width: "20%" },
        { data: "PercentualAcrescimoComissaoFaturamentoDia", title: "% + Comissão", width: "20%" }];

    _gridFaturamentoDia = new BasicDataTable(_bonificacao.FaturamentoDia.idGrid, header, menuOpcoes);
    RecarregarGridFaturamentoDia();
}

function excluirFaturamentoDia(e) {
    for (let i = 0; i < _bonificacao.FaturamentoDia.list.length; i++) {
        const faturamento = _bonificacao.FaturamentoDia.list[i];
        if (faturamento !== null && faturamento?.Codigo !== null && e !== null && e.Codigo !== null && e.Codigo === faturamento?.Codigo) {
            _bonificacao.FaturamentoDia.list.splice(i, 1);
        }
    };
    RecarregarGridFaturamentoDia();
}

function RecarregarGridFaturamentoDia() {
    const data = new Array();

    for (let i = 0; i < _bonificacao.FaturamentoDia.list.length; i++) {
        const faturamento = _bonificacao.FaturamentoDia.list[i];

        data.push({
            Codigo: faturamento.Codigo,
            FaturamentoInicial: faturamento.FaturamentoInicial,
            FaturamentoFinal: faturamento.FaturamentoFinal,
            PercentualAcrescimoComissaoFaturamentoDia: faturamento.PercentualAcrescimoComissaoFaturamentoDia,
        });
    };
    _gridFaturamentoDia.CarregarGrid(data);
}

function AdicionarFaturamentoDiaClick(e, sender) {
    let tudoCerto = true;
    if (_bonificacao.FaturamentoFinal.val() === "" || _bonificacao.FaturamentoFinal.val() === "0,00")
        tudoCerto = false;    

    if (tudoCerto) {
        const faturamentoFinal = Globalize.parseFloat(_bonificacao.FaturamentoFinal.val());
        const faturamentoInicial = Globalize.parseFloat(_bonificacao.FaturamentoInicial.val());
        if (faturamentoInicial > faturamentoFinal)
            return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "A média inicial não pode ser maior que a final!");
    }

    if (tudoCerto) {
        const map = new Object();

        map.Codigo = guid();
        map.FaturamentoInicial = _bonificacao.FaturamentoInicial.val();
        map.FaturamentoFinal = _bonificacao.FaturamentoFinal.val();
        map.PercentualAcrescimoComissaoFaturamentoDia = _bonificacao.PercentualAcrescimoComissaoFaturamentoDia.val();

        _bonificacao.FaturamentoDia.list.push(map);

        RecarregarGridFaturamentoDia();
        limparDadosFaturamentoDia();
        $("#" + _bonificacao.FaturamentoInicial.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento da faixa de faturamento por dia!");
    }
}

function limparDadosFaturamentoDia() {
    _bonificacao.FaturamentoInicial.val("");
    _bonificacao.FaturamentoFinal.val("");
    _bonificacao.PercentualAcrescimoComissaoFaturamentoDia.val("");
}

//********REPRESENTACAO********

function carregarGridRepresentacao() {
    const excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirRepresentacao(data);
        }, tamanho: "10", icone: ""
    };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    const header = [{ data: "Codigo", visible: false },
    { data: "CodigoJustificativaBonificacaoRepresentacao", visible: false },
    { data: "PercentualRepresentacao", title: "% Representacao", width: "10%" },
    { data: "PercentualAcrescimoComissaoRepresentacao", title: "% + Comissão", width: "10%" },
    { data: "ValorBonificacaoRepresentacao", title: "Valor Bonificação", width: "10%" },
    { data: "DescricaoJustificativaBonificacaoRepresentacao", title: "Justificativa", width: "30%" }];

    _gridRepresentacaos = new BasicDataTable(_bonificacao.Representacaos.idGrid, header, menuOpcoes);
    RecarregarGridRepresentacao();
}

function excluirRepresentacao(e) {
    for (let i = 0; i < _bonificacao.Representacaos.list.length; i++) {
        const representacao = _bonificacao.Representacaos.list[i];
        if (representacao !== null && representacao?.Codigo !== null && e !== null && e.Codigo !== null && e.Codigo === representacao?.Codigo) {
            _bonificacao.Representacaos.list.splice(i, 1);
        }
    };
    RecarregarGridRepresentacao();
}

function RecarregarGridRepresentacao() {
    const data = new Array();

    for (let i = 0; i < _bonificacao.Representacaos.list.length; i++) {
        const representacao = _bonificacao.Representacaos.list[i];
        data.push({
            Codigo: representacao.Codigo,
            CodigoJustificativaBonificacaoRepresentacao: representacao.CodigoJustificativaBonificacaoRepresentacao,
            PercentualRepresentacao: representacao.PercentualRepresentacao,
            PercentualAcrescimoComissaoRepresentacao: representacao.PercentualAcrescimoComissaoRepresentacao,
            ValorBonificacaoRepresentacao: representacao.ValorBonificacaoRepresentacao,
            DescricaoJustificativaBonificacaoRepresentacao: representacao.DescricaoJustificativaBonificacaoRepresentacao,
        });
    };
    _gridRepresentacaos.CarregarGrid(data);
}

function AdicionarRepresentacaoClick(e, sender) {
    let tudoCerto = true;
    if (_bonificacao.PercentualRepresentacao.val() === "" || _bonificacao.PercentualRepresentacao.val() === "0,00")
        tudoCerto = false;
    if (_bonificacao.ValorBonificacaoRepresentacao.val() !== "" && _bonificacao.ValorBonificacaoRepresentacao.val() !== "0,00" && (_bonificacao.JustificativaBonificacaoRepresentacao.codEntity() === 0 || _bonificacao.JustificativaBonificacaoRepresentacao.codEntity() === ""))
        tudoCerto = false;

    if (tudoCerto) {
        const map = new Object();

        map.Codigo = guid();
        map.PercentualRepresentacao = _bonificacao.PercentualRepresentacao.val();        
        map.PercentualAcrescimoComissaoRepresentacao = _bonificacao.PercentualAcrescimoComissaoRepresentacao.val();
        map.ValorBonificacaoRepresentacao = _bonificacao.ValorBonificacaoRepresentacao.val();
        map.DescricaoJustificativaBonificacaoRepresentacao = _bonificacao.JustificativaBonificacaoRepresentacao.val();
        map.CodigoJustificativaBonificacaoRepresentacao = _bonificacao.JustificativaBonificacaoRepresentacao.codEntity();

        _bonificacao.Representacaos.list.push(map);

        RecarregarGridRepresentacao();
        limparDadosRepresentacao();
        $("#" + _bonificacao.PercentualRepresentacao.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento da faixa de representação!");
    }
}

function limparDadosRepresentacao() {
    LimparCampoEntity(_bonificacao.JustificativaBonificacaoRepresentacao);
    _bonificacao.PercentualRepresentacao.val("");    
    _bonificacao.PercentualAcrescimoComissaoRepresentacao.val("");
    _bonificacao.ValorBonificacaoRepresentacao.val("");
}

//********METODOS********

function LimparCamposBonificacao() {
    _bonificacao.Medias.list = new Array();
    _bonificacao.RotasFretes.list = new Array();
    _bonificacao.FaturamentoDia.list = new Array();
    _bonificacao.Representacaos.list = new Array();

    _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.val(false);
    _bonificacao.AtivarBonificacaoRepresentacaoCombustivel.visibleFade(false);
    _bonificacao.AtivarBonificacaoMediaCombustivel.val(false);
    _bonificacao.AtivarBonificacaoMediaCombustivel.visibleFade(false);
    _bonificacao.AtivarBonificacaoFaturamentoDia.val(false);
    _bonificacao.AtivarBonificacaoFaturamentoDia.visibleFade(false);
    _bonificacao.AtivarBonificacaoRotaFrete.val(false);
    _bonificacao.AtivarBonificacaoRotaFrete.visibleFade(false);

    LimparCampos(_bonificacao);
}