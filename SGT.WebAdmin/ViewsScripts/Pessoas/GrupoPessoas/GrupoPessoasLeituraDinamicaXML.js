/// <reference path="../../Consultas/LeituraDinamicaXmlOrigem.js" />
/// <reference path="../../Consultas/LeituraDinamicaXmlOrigemTagFilha.js" />
/// <reference path="../../Consultas/LeituraDinamicaXmlDestino.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFinanceira.js" />
/// <reference path="../../Enumeradores/EnumTipoFiltrarConteudo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _grupoPessoasLeituraDinamicaXML;
var _gridLeituraDinamicaXML;
var arrayGrupoPessoasLeituraDinamicaXML = new Array();

var GrupoPessoasLeituraDinamicaXML = function () {
    this.CodigoLeituraDinamicaXML = PropertyEntity({ type: types.string });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumento.NFe), enable: ko.observable(true), text: Localization.Resources.Pessoas.GrupoPessoas.TipoDeDocumento.getFieldDescription(), options: EnumTipoDocumento.obterOpcoesLeituraDinamicaXML(), def: EnumTipoDocumento.NFe });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Descricao.getRequiredFieldDescription(), getType: typesKnockout.string, required: true, enable: ko.observable(true), visible: ko.observable(true), val: ko.observable("") });
    this.LeituraDinamicaXmlOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.OrigemDaInformacao.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LeituraDinamicaXmlOrigemTagFilha = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.TagFilha.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.LeituraDinamicaXmlDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.DestinoDaInformacao.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.FiltrarTag = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.FiltrarTagxCampo.getFieldDescription()), getType: typesKnockout.string, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false), val: ko.observable("") });
    this.FiltrarPrimeiroDisponivel = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.FiltrarPrimeiroDisponivel.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.HabilitarFiltrarConteudo = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.HabilitarFiltrarConteudo.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.FiltrarConteudoTextoInicial = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.FiltrarConteudoTextoInicial.getRequiredFieldDescription()), getType: typesKnockout.string, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.FiltrarConteudoTextoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.FiltrarConteudoTextoFinal.getRequiredFieldDescription(), getType: typesKnockout.string, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoFiltrarConteudo = PropertyEntity({ val: ko.observable(EnumTipoFiltrarConteudo.TextoLivre), enable: ko.observable(true), text: Localization.Resources.Pessoas.GrupoPessoas.TipoFiltrarConteudo.getFieldDescription(), options: EnumTipoFiltrarConteudo.obterOpcoes(), def: EnumTipoFiltrarConteudo.TextoLivre, visible: ko.observable(false) });
    this.RemoverCaracteres = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.RemoverCaracteres.getFieldDescription()), getType: typesKnockout.string, required: false, enable: ko.observable(true), visible: ko.observable(false), val: ko.observable("") });
    this.SubstituirVirgulaPorPonto = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.SubstituirPontoPorVirgula.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarGrupoPessoasLeituraDinamicaXMLClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirGrupoPessoasLeituraDinamicaXMLClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Excluir, visible: ko.observable(false), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarGrupoPessoasLeituraDinamicaXMLClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.LeituraDinamicaXML = PropertyEntity({ type: types.local });

    this.LeituraDinamicaXmlOrigem.val.subscribe(function (novoValor) {
        switch (novoValor) {
            case "InfCpl":
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.val("");
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.visible(false);
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.required(false);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.visible(false);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.required(false);
                _grupoPessoasLeituraDinamicaXML.FiltrarPrimeiroDisponivel.visible(false);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.visible(false);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.val(true);
                break;
            case "ObsCont":
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.val("");
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.visible(false);
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.required(false);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.text(Localization.Resources.Pessoas.GrupoPessoas.FiltrarTagxCampo.getRequiredFieldDescription())
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.visible(true);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.required(true);
                _grupoPessoasLeituraDinamicaXML.FiltrarPrimeiroDisponivel.visible(true);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.visible(true);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.val(false);
                break;
            case "Vol":
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.visible(true);
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.required(true);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.text(Localization.Resources.Pessoas.GrupoPessoas.FiltrarTagEsp.getRequiredFieldDescription())
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.visible(true);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.required(true);
                _grupoPessoasLeituraDinamicaXML.FiltrarPrimeiroDisponivel.visible(true);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.visible(true);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.val(false);
                break;
            default:
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.val("");
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.visible(false);
                _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.required(false);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.visible(false);
                _grupoPessoasLeituraDinamicaXML.FiltrarTag.required(false);
                _grupoPessoasLeituraDinamicaXML.FiltrarPrimeiroDisponivel.visible(false);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.visible(false);
                _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.val(false);
                break;
        }
    });

    this.FiltrarPrimeiroDisponivel.val.subscribe(function (novoValor) {
        if (novoValor == true) {
            _grupoPessoasLeituraDinamicaXML.FiltrarTag.val("");
            _grupoPessoasLeituraDinamicaXML.FiltrarTag.visible(false);
            _grupoPessoasLeituraDinamicaXML.FiltrarTag.required(false);
        }
        else {
            _grupoPessoasLeituraDinamicaXML.FiltrarTag.visible(true);
            _grupoPessoasLeituraDinamicaXML.FiltrarTag.required(true);
        }
    });

    this.HabilitarFiltrarConteudo.val.subscribe(function (novoValor) {
        if (novoValor == true) {
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.visible(true);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.visible(true);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.required(true);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.required(true);
            _grupoPessoasLeituraDinamicaXML.TipoFiltrarConteudo.visible(true);
        }
        else {
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.visible(false);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.visible(false);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.required(false);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.required(false);
            _grupoPessoasLeituraDinamicaXML.TipoFiltrarConteudo.visible(false);
            _grupoPessoasLeituraDinamicaXML.TipoFiltrarConteudo.val(EnumTipoFiltrarConteudo.TextoLivre);
        }
    });

    this.TipoFiltrarConteudo.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoFiltrarConteudo.TextoLivre) {
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.visible(true);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.text(Localization.Resources.Pessoas.GrupoPessoas.FiltrarConteudoTextoInicial.getRequiredFieldDescription());
        }
        else {
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.val("");
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.visible(false);
            _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.text(Localization.Resources.Pessoas.GrupoPessoas.FiltrarConteudoExpressãoRegular.getRequiredFieldDescription());
        }
    });

    this.LeituraDinamicaXmlDestino.val.subscribe(function (novoValor) {
        switch (novoValor) {
            case "Metros Cubicos":
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.visible(true);
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.visible(true);
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.val("");
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.val(true);
                break;
            case "Nº Controle Cliente":
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.visible(true);
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.visible(false);
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.val("");
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.val(false);
                break;
            case "Nº Pallets":
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.visible(true);
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.visible(false);
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.val("");
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.val(false);
                break;
            default:
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.visible(false);
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.visible(false);
                _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.val("");
                _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.val(false);
                break;
        }
    });
}

function loadGrupoPessoasLeituraDinamicaXML() {
    _grupoPessoasLeituraDinamicaXML = new GrupoPessoasLeituraDinamicaXML();
    KoBindings(_grupoPessoasLeituraDinamicaXML, "knockoutLeituraDinamicaXML");

    BuscarLeituraDinamicaXmlOrigem(_grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem);
    BuscarLeituraDinamicaXmlOrigemTagFilha(_grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha, null, _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem);
    BuscarLeituraDinamicaXmlDestino(_grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino);

    loadGridGrupoPessoasLeituraDinamicaXML();
}

function loadGridGrupoPessoasLeituraDinamicaXML() {
    var linhasPorPagina = 5;
    var opcaoEditar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: EditarGrupoPessoasLeituraDinamicaXMLClick, icone: "", visiblidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoDocumento", visible: false },
        { data: "Descricao", title: "Descrição", width: "25%", className: "text-align-left" },
        { data: "CodigoLeituraDinamicaXmlOrigem", visible: false },
        { data: "LeituraDinamicaXmlOrigem", title: Localization.Resources.Pessoas.GrupoPessoas.OrigemDaInformacao.getFieldDescription(), width: "25%", className: "text-align-left" },
        { data: "CodigoLeituraDinamicaXmlOrigemTagFilha", visible: false },
        { data: "LeituraDinamicaXmlOrigemTagFilha", title: Localization.Resources.Pessoas.GrupoPessoas.TagFilha.getFieldDescription(), width: "25%", className: "text-align-left" },
        { data: "CodigoLeituraDinamicaXmlDestino", visible: false },
        { data: "LeituraDinamicaXmlDestino", title: Localization.Resources.Pessoas.GrupoPessoas.DestinoDaInformacao.getFieldDescription(), width: "25%", className: "text-align-left" },
        { data: "FiltrarTag", visible: false },
        { data: "FiltrarPrimeiroDisponivel", visible: false },
        { data: "HabilitarFiltrarConteudo", visible: false },
        { data: "FiltrarConteudoTextoInicial", visible: false },
        { data: "FiltrarConteudoTextoFinal", visible: false },
        { data: "TipoFiltrarConteudo", visible: false },
        { data: "RemoverCaracteres", visible: false },
        { data: "SubstituirVirgulaPorPonto", visible: false }
    ];

    _gridLeituraDinamicaXML = new BasicDataTable(_grupoPessoasLeituraDinamicaXML.LeituraDinamicaXML.id, header, menuOpcoes, null, null, linhasPorPagina);
    _gridLeituraDinamicaXML.CarregarGrid([]);
}

function AdicionarGrupoPessoasLeituraDinamicaXMLClick() {
    if (!ValidarCamposObrigatorios(_grupoPessoasLeituraDinamicaXML)) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    //if (!verificarGrupoPessoasLeituraDinamicaXMLTabela()) {
        //exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.TabelaJaPossuiRegistroDocumento);
        //return;
    //}

    let leituraDinamicaXML = {
        Codigo: guid(),
        TipoDocumento: _grupoPessoasLeituraDinamicaXML.TipoDocumento.val(),
        Descricao: _grupoPessoasLeituraDinamicaXML.Descricao.val(),
        CodigoLeituraDinamicaXmlOrigem: _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem.codEntity(),
        LeituraDinamicaXmlOrigem: _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem.val(),
        CodigoLeituraDinamicaXmlOrigemTagFilha: _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.codEntity(),
        LeituraDinamicaXmlOrigemTagFilha: _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.val(),
        CodigoLeituraDinamicaXmlDestino: _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino.codEntity(),
        LeituraDinamicaXmlDestino: _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino.val(),
        FiltrarTag: _grupoPessoasLeituraDinamicaXML.FiltrarTag.val(),
        FiltrarPrimeiroDisponivel: _grupoPessoasLeituraDinamicaXML.FiltrarPrimeiroDisponivel.val(),
        HabilitarFiltrarConteudo: _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.val(),
        FiltrarConteudoTextoInicial: _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.val(),
        FiltrarConteudoTextoFinal: _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.val(),
        TipoFiltrarConteudo: _grupoPessoasLeituraDinamicaXML.TipoFiltrarConteudo.val(),
        RemoverCaracteres: _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.val(),
        SubstituirVirgulaPorPonto: _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.val(),
    }
    arrayGrupoPessoasLeituraDinamicaXML.push(leituraDinamicaXML);
    LimparCamposGrupoPessoasLeituraDinamicaXML();
}

function AtualizarGrupoPessoasLeituraDinamicaXMLClick() {
    if (!ValidarCamposObrigatorios(_grupoPessoasLeituraDinamicaXML)) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    //if (!verificarGrupoPessoasLeituraDinamicaXMLTabela(true)) {
        //exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.TabelaJaPossuiRegistroDocumento);
        //return;
    //}

    for (let i = 0; i < arrayGrupoPessoasLeituraDinamicaXML.length; i++) {
        if (arrayGrupoPessoasLeituraDinamicaXML[i].Codigo == _grupoPessoasLeituraDinamicaXML.CodigoLeituraDinamicaXML.val()) {
            arrayGrupoPessoasLeituraDinamicaXML[i].TipoDocumento = _grupoPessoasLeituraDinamicaXML.TipoDocumento.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].Descricao = _grupoPessoasLeituraDinamicaXML.Descricao.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].CodigoLeituraDinamicaXmlOrigem = _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem.codEntity();
            arrayGrupoPessoasLeituraDinamicaXML[i].LeituraDinamicaXmlOrigem = _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].CodigoLeituraDinamicaXmlOrigemTagFilha = _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.codEntity();
            arrayGrupoPessoasLeituraDinamicaXML[i].LeituraDinamicaXmlOrigemTagFilha = _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].CodigoLeituraDinamicaXmlDestino = _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino.codEntity();
            arrayGrupoPessoasLeituraDinamicaXML[i].LeituraDinamicaXmlDestino = _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].FiltrarTag = _grupoPessoasLeituraDinamicaXML.FiltrarTag.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].FiltrarPrimeiroDisponivel = _grupoPessoasLeituraDinamicaXML.FiltrarPrimeiroDisponivel.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].HabilitarFiltrarConteudo = _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].FiltrarConteudoTextoInicial = _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].FiltrarConteudoTextoFinal = _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].TipoFiltrarConteudo = _grupoPessoasLeituraDinamicaXML.TipoFiltrarConteudo.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].RemoverCaracteres = _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.val();
            arrayGrupoPessoasLeituraDinamicaXML[i].SubstituirVirgulaPorPonto = _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.val();
            break;
        }
    }

    LimparCamposGrupoPessoasLeituraDinamicaXML();
    esconderBotoesGrupoPessoasLeituraDinamicaXML();
}

function ExcluirGrupoPessoasLeituraDinamicaXMLClick() {
    for (let i = 0; i < arrayGrupoPessoasLeituraDinamicaXML.length; i++) {
        if (arrayGrupoPessoasLeituraDinamicaXML[i].Codigo == _grupoPessoasLeituraDinamicaXML.CodigoLeituraDinamicaXML.val()) {
            arrayGrupoPessoasLeituraDinamicaXML.splice(i, 1);
            break;
        }
    }

    LimparCamposGrupoPessoasLeituraDinamicaXML();
    esconderBotoesGrupoPessoasLeituraDinamicaXML();
}

function EditarGrupoPessoasLeituraDinamicaXMLClick(registroSelecionado) {
    mostrarBotoesGrupoPessoasLeituraDinamicaXML();
    _grupoPessoasLeituraDinamicaXML.CodigoLeituraDinamicaXML.val(registroSelecionado.Codigo);
    _grupoPessoasLeituraDinamicaXML.TipoDocumento.val(registroSelecionado.TipoDocumento);
    _grupoPessoasLeituraDinamicaXML.Descricao.val(registroSelecionado.Descricao);
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem.codEntity(registroSelecionado.CodigoLeituraDinamicaXmlOrigem);
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem.val(registroSelecionado.LeituraDinamicaXmlOrigem);
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.codEntity(registroSelecionado.CodigoLeituraDinamicaXmlOrigemTagFilha);
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.val(registroSelecionado.LeituraDinamicaXmlOrigemTagFilha);
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino.codEntity(registroSelecionado.CodigoLeituraDinamicaXmlDestino);
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino.val(registroSelecionado.LeituraDinamicaXmlDestino);
    _grupoPessoasLeituraDinamicaXML.FiltrarTag.val(registroSelecionado.FiltrarTag);
    _grupoPessoasLeituraDinamicaXML.FiltrarPrimeiroDisponivel.val(registroSelecionado.FiltrarPrimeiroDisponivel);
    _grupoPessoasLeituraDinamicaXML.HabilitarFiltrarConteudo.val(registroSelecionado.HabilitarFiltrarConteudo);
    _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.val(registroSelecionado.FiltrarConteudoTextoInicial);
    _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.val(registroSelecionado.FiltrarConteudoTextoFinal);
    _grupoPessoasLeituraDinamicaXML.TipoFiltrarConteudo.val(registroSelecionado.TipoFiltrarConteudo);
    _grupoPessoasLeituraDinamicaXML.RemoverCaracteres.val(registroSelecionado.RemoverCaracteres);
    _grupoPessoasLeituraDinamicaXML.SubstituirVirgulaPorPonto.val(registroSelecionado.SubstituirVirgulaPorPonto);
}

function LimparCamposGrupoPessoasLeituraDinamicaXML() {
    _gridLeituraDinamicaXML.CarregarGrid(arrayGrupoPessoasLeituraDinamicaXML);
    _grupoPessoasLeituraDinamicaXML.Descricao.val("");
    _grupoPessoasLeituraDinamicaXML.FiltrarTag.val("");
    _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoInicial.val("");
    _grupoPessoasLeituraDinamicaXML.FiltrarConteudoTextoFinal.val("");
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigem.val("");
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlDestino.val("");
    _grupoPessoasLeituraDinamicaXML.LeituraDinamicaXmlOrigemTagFilha.val("");
}

function RecarregarGridGrupoPessoasLeituraDinamicaXML() {
    _gridLeituraDinamicaXML.CarregarGrid(arrayGrupoPessoasLeituraDinamicaXML);
}

function mostrarBotoesGrupoPessoasLeituraDinamicaXML() {
    _grupoPessoasLeituraDinamicaXML.Adicionar.visible(false);
    _grupoPessoasLeituraDinamicaXML.Atualizar.visible(true);
    _grupoPessoasLeituraDinamicaXML.Excluir.visible(true);
}

function esconderBotoesGrupoPessoasLeituraDinamicaXML() {
    _grupoPessoasLeituraDinamicaXML.Adicionar.visible(true);
    _grupoPessoasLeituraDinamicaXML.Atualizar.visible(false);
    _grupoPessoasLeituraDinamicaXML.Excluir.visible(false);
}

function verificarGrupoPessoasLeituraDinamicaXMLTabela(atualizar = false) {
    if (atualizar) {
        for (let i = 0; i < arrayGrupoPessoasLeituraDinamicaXML.length; i++) {
            if (arrayGrupoPessoasLeituraDinamicaXML[i].Codigo != _grupoPessoasLeituraDinamicaXML.CodigoLeituraDinamicaXML.val()) {
                if (arrayGrupoPessoasLeituraDinamicaXML[i].ModeloDocumentoFiscal == _grupoPessoasLeituraDinamicaXML.ModeloDocumento.val()
                    || arrayGrupoPessoasLeituraDinamicaXML[i].CodigoModeloDocumento == _grupoPessoasLeituraDinamicaXML.ModeloDocumento.codEntity())
                    return false;
            }
        }
        return true;
    }

    for (let i = 0; i < arrayGrupoPessoasLeituraDinamicaXML.length; i++) {
        if (arrayGrupoPessoasLeituraDinamicaXML[i].ModeloDocumentoFiscal == _grupoPessoasLeituraDinamicaXML.ModeloDocumento.val()
            || arrayGrupoPessoasLeituraDinamicaXML[i].CodigoModeloDocumento == _grupoPessoasLeituraDinamicaXML.ModeloDocumento.codEntity())
            return false;
    }
    return true;
}