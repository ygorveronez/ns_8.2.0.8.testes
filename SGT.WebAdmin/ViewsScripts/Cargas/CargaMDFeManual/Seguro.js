/// <reference path="../../Enumeradores/EnumTipoSeguroMDFe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoSeguro = [
    { text: "1 - Emitente", value: EnumTipoSeguroMDFe.Emitente },
    { text: "2 - Contratante", value: EnumTipoSeguroMDFe.Contratante }
]

var _gridSeguroMDFeManual, _seguroMDFeManual;

var SeguroMDFeManual = function () {
    this.GridSeguro = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.UsarDadosCTe = PropertyEntity({ val: ko.observable(true), text: ko.observable("Usar os dados de seguro dos CT-es para emissão do MDF-e?"), def: true, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TipoSeguro = PropertyEntity({ val: ko.observable(EnumTipoSeguroMDFe.Emitente), options: _tipoSeguro, text: "*Tipo Seguro: ", def: EnumTipoSeguroMDFe.Emitente, enable: ko.observable(true) });
    this.CNPJSeguradoraSeguro = PropertyEntity({ text: "CNPJ Seguradora:", required: false, getType: typesKnockout.cnpj, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConsultarCNPJSeguradoraSeguro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Buscar", idBtnSearch: guid(), enable: ko.observable(true) });
    this.NomeSeguradoraSeguro = PropertyEntity({ text: "Seguradora:", required: false, maxlength: 30, enable: ko.observable(true) });
    this.NomeResponsavelSeguro = PropertyEntity({ text: "CNPJ Responsável:", required: false, getType: typesKnockout.cnpj, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConsultarNomeResponsavelSeguro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Buscar", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ApoliceSeguro = PropertyEntity({ text: "*Nº Apólice:", required: true, maxlength: 20, enable: ko.observable(true) });
    this.AverbacaoSeguro = PropertyEntity({ text: "Nº Averbação:", required: false, maxlength: 40, enable: ko.observable(true) });

    this.AdicionarSeguro = PropertyEntity({ eventClick: AdicionarSeguroClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });

};

function LoadSeguro() {

    _seguroMDFeManual = new SeguroMDFeManual();
    KoBindings(_seguroMDFeManual, "tabSeguro");

    BuscarClientes(_seguroMDFeManual.ConsultarCNPJSeguradoraSeguro, RetornoCNPJSeguradoraSeguro);
    BuscarClientes(_seguroMDFeManual.ConsultarNomeResponsavelSeguro, RetornoNomeResponsavelSeguro);

    RecarregarGridSeguro();

    $("#" + _seguroMDFeManual.UsarDadosCTe.id).click(usarDadosSeguroCTeClick);
}

function usarDadosSeguroCTeClick() {
    _cargaMDFeManual.UsarSeguroCTe.val(_seguroMDFeManual.UsarDadosCTe.val());
}

function RetornoNomeResponsavelSeguro(data) {
    _seguroMDFeManual.NomeResponsavelSeguro.val(data.CPF_CNPJ);
}

function RetornoCNPJSeguradoraSeguro(data) {
    _seguroMDFeManual.CNPJSeguradoraSeguro.val(data.CPF_CNPJ);
    _seguroMDFeManual.NomeSeguradoraSeguro.val(data.Nome);
}

function RecarregarGridSeguro() {
    if (_gridSeguroMDFeManual != null) {
        _gridSeguroMDFeManual.Destroy();
        _gridSeguroMDFeManual = null;
    }

    var excluir = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: RemoverSeguroClick,
        tamanho: "8",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    if (_cargaMDFeManual.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoSeguro", title: "Tipo", width: "10%", className: "text-align-left" },
        { data: "CNPJSeguradoraSeguro", title: "CNPJ Seguradora", width: "15%", className: "text-align-left" },
        { data: "NomeSeguradoraSeguro", title: "Seguradora", width: "20%", className: "text-align-left" },
        { data: "NomeResponsavelSeguro", title: "Responsável", width: "20%", className: "text-align-left" },
        { data: "ApoliceSeguro", title: "Apólice", width: "10%", className: "text-align-left" },
        { data: "AverbacaoSeguro", title: "Averbação", width: "10%", className: "text-align-left" }
    ];

    _gridSeguroMDFeManual = new BasicDataTable(_seguroMDFeManual.GridSeguro.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridSeguroMDFeManual.CarregarGrid(_cargaMDFeManual.ListaSeguro.val());
}

function RemoverSeguroClick(data) {
    var seguro = _cargaMDFeManual.ListaSeguro.val();

    for (var i = 0; i < seguro.length; i++) {
        if (data.Codigo == seguro[i].Codigo) {
            seguro.splice(i, 1);
            break;
        }
    }

    _cargaMDFeManual.ListaSeguro.val(seguro);
    _gridSeguroMDFeManual.CarregarGrid(_cargaMDFeManual.ListaSeguro.val());
}

function AdicionarSeguroClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_seguroMDFeManual);
    _seguroMDFeManual.CNPJSeguradoraSeguro.requiredClass("form-control");
    _seguroMDFeManual.NomeResponsavelSeguro.requiredClass("form-control");    

    if (valido) {

        if (_seguroMDFeManual.CNPJSeguradoraSeguro.val() != "" && !ValidarCNPJ(_seguroMDFeManual.CNPJSeguradoraSeguro.val(), true)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe um CNPJ válido!");
            _seguroMDFeManual.CNPJSeguradoraSeguro.requiredClass("form-control is-invalid");
            return;
        }

        if (_seguroMDFeManual.NomeResponsavelSeguro.val() != "" && !ValidarCNPJ(_seguroMDFeManual.NomeResponsavelSeguro.val(), true)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe um CNPJ válido!");
            _seguroMDFeManual.NomeResponsavelSeguro.requiredClass("form-control is-invalid");
            return;
        }

        var seguro = _cargaMDFeManual.ListaSeguro.val();

        for (var i = 0; i < seguro.length; i++) {
            if (_seguroMDFeManual.AverbacaoSeguro.val() != "" && _seguroMDFeManual.AverbacaoSeguro.val() == seguro[i].AverbacaoSeguro) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "O número de averbação informado (" + seguro[i].AverbacaoSeguro + ") já existe.");
                return;
            }
        }

        _seguroMDFeManual.Codigo.val(guid());

        seguro.push(RetornarObjetoPesquisa(_seguroMDFeManual));

        _cargaMDFeManual.ListaSeguro.val(seguro);
        _gridSeguroMDFeManual.CarregarGrid(_cargaMDFeManual.ListaSeguro.val());
        LimparCamposSeguro();
        _seguroMDFeManual.UsarDadosCTe.val(false);
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposSeguro() {
    LimparCampos(_seguroMDFeManual);
}