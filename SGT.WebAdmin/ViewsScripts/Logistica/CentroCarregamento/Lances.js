var _lancesKnockout;
var _gridLances;

var Lances = function () {
    this.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.LiberarCotacaoAposEsgotarTempoLimiteDeConfirmacaoDoTransportadorParaCargasAutomatica, getType: typesKnockout.bool, val: ko.observable(false) });

    this.NumeroLanceDe = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.DeLanceNumero.getRequiredFieldDescription(), getType: typesKnockout.int, val: ko.observable(), required: true });
    this.NumeroLanceAte = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.AteLanceNumero.getRequiredFieldDescription(), getType: typesKnockout.int, val: ko.observable(), required: true });
    this.PorcentagemLance = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PorcentagemMaximaDeTolerancia.getRequiredFieldDescription(), getType: typesKnockout.decimal, val: ko.observable(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoLance, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.ListaLances = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });

    this.ListaLances.val.subscribe(
        function () {
            recarregarGridLances();
        }
    );
}

function loadLances() {
    _lancesKnockout = new Lances();
    KoBindings(_lancesKnockout, "knockoutLances");

    loadGridLances();
}

function loadGridLances() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirConfiguracaoLance };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroLanceDe", visible: false },
        { data: "NumeroLanceAte", visible: false },
        { data: "PorcentagemLance", visible: false },
        { data: "NumeroLance", title: Localization.Resources.Logistica.CentroCarregamento.NumeroLance, width: "70%" },
        { data: "PorcentagemLanceDescricao", title: Localization.Resources.Logistica.CentroCarregamento.PorcentagemLance, width: "20%" }
    ];

    _gridLances = new BasicDataTable(_lancesKnockout.ListaLances.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridLances();
}

function recarregarGridLances() {
    var data = new Array();

    $.each(_lancesKnockout.ListaLances.val(), function (i, lance) {
        var configuracaoLance = new Object();

        configuracaoLance.Codigo = lance.Codigo;
        configuracaoLance.NumeroLanceDe = parseInt(lance.NumeroLanceDe);
        configuracaoLance.NumeroLanceAte = parseInt(lance.NumeroLanceAte);
        configuracaoLance.PorcentagemLance = parseFloat(lance.PorcentagemLance);
        configuracaoLance.NumeroLance = parseInt(lance.NumeroLanceDe) == parseInt(lance.NumeroLanceAte) ? lance.NumeroLanceDe + "°" : Localization.Resources.Logistica.CentroCarregamento.DoAte.format(lance.NumeroLanceDe, lance.NumeroLanceAte);
        configuracaoLance.PorcentagemLanceDescricao = lance.PorcentagemLance + "%";

        data.push(configuracaoLance);
    });

    _gridLances.CarregarGrid(data);
}

function preencherLancesSalvar(centroCarregamento) {
    centroCarregamento["Lances"] = JSON.stringify(_lancesKnockout.ListaLances.val());
    centroCarregamento["LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente"] = _lancesKnockout.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.val();
}

function preencherLances(data) {
    _lancesKnockout.ListaLances.val(data.ListaLances);
    _lancesKnockout.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.val(data.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente);
}

function adicionarConfiguracaoLance() {
    if (!ValidarCamposObrigatorios(_lancesKnockout)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    if (parseInt(_lancesKnockout.NumeroLanceAte.val()) < parseInt(_lancesKnockout.NumeroLanceDe.val())) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Logistica.CentroCarregamento.NumeroDeLanceFinalNaoPodeSerMenorQueNumeroDoLanceFinal);
        return;
    }

    if (!verificarExistenciaLance()) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Logistica.CentroCarregamento.JaExisteUmaConfiguracaoDeLanceContendoOsNumerosEscolhidos);
        return;
    }

    var data = [];
    var lance = new Object();

    lance.Codigo = guid();
    lance.NumeroLanceDe = _lancesKnockout.NumeroLanceDe.val();
    lance.NumeroLanceAte = _lancesKnockout.NumeroLanceAte.val();
    lance.PorcentagemLance = _lancesKnockout.PorcentagemLance.val();

    data = _lancesKnockout.ListaLances.val();
    data.push(lance);

    _lancesKnockout.ListaLances.val(data);

    LimparCampos(_lancesKnockout);
}

function excluirConfiguracaoLance(registroSelecionado) {
    var listaLances = _lancesKnockout.ListaLances.val();

    for (var i = 0; i < listaLances.length; i++) {
        if (listaLances[i].Codigo == registroSelecionado.Codigo)
            listaLances.splice(i, 1);
    }

    _lancesKnockout.ListaLances.val(listaLances);
}

function verificarExistenciaLance() {
    var lista = _lancesKnockout.ListaLances.val();
    var numerosExistentes = [];
    var numerosDeTeste = [];    

    //Preenchendo lista com numeros que já existem no grid.
    for (var i = 0; i < lista.length; i++) {
        if (lista[i].NumeroLanceDe == lista[i].NumeroLanceAte)
            numerosExistentes.push(parseInt(lista[i].NumeroLanceDe));
        else {
            for (var j = parseInt(lista[i].NumeroLanceDe); j <= parseInt(lista[i].NumeroLanceAte); j++)
                numerosExistentes.push(j);
        }
    }

    //Preenchendo lista com numeros a serem adicionados
    for (var i = parseInt(_lancesKnockout.NumeroLanceDe.val()); i <= parseInt(_lancesKnockout.NumeroLanceAte.val()); i++) {
        numerosDeTeste.push(i);
    }

    for (var i = 0; i < numerosDeTeste.length; i++)
        if (numerosExistentes.includes(numerosDeTeste[i]))
            return false;

    return true;
}

function limparCamposLances() {
    _lancesKnockout.ListaLances.val([]);
    LimparCampos(_lancesKnockout);
}