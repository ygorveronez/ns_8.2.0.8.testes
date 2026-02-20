//*******MAPEAMENTO KNOUCKOUT*******

var _gridValePedagioMDFeManual, _valePedagioMDFeManual;

var ValePedagioMDFeManual = function () {
    this.GridValePedagio = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.FornecedorValePedagio = PropertyEntity({ text: "*CNPJ Fornecedor:", required: true, getType: typesKnockout.cnpj, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConsultarFornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Buscar", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ResponsavelValePedagio = PropertyEntity({ text: "*CNPJ Responsável:", required: true, getType: typesKnockout.cnpj, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConsultarResponsavelValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Buscar", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ComprovanteValePedagio = PropertyEntity({ text: "*Nº Comprovante:", required: true, maxlength: 20, enable: ko.observable(true) });
    this.ValorValePedagio = PropertyEntity({ text: "*Valor:", required: true, enable: ko.observable(true), getType: typesKnockout.decimal });

    this.AdicionarValePedagio = PropertyEntity({ eventClick: AdicionarValePedagioClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

function LoadValePedagio() {

    _valePedagioMDFeManual = new ValePedagioMDFeManual();
    KoBindings(_valePedagioMDFeManual, "tabValePedagio");

    BuscarClientes(_valePedagioMDFeManual.ConsultarFornecedorValePedagio, RetornoFornecedorValePedagio);
    BuscarClientes(_valePedagioMDFeManual.ConsultarResponsavelValePedagio, RetornoResponsavelValePedagio);

    RecarregarGridValePedagio();
}

function RetornoFornecedorValePedagio(data) {
    _valePedagioMDFeManual.FornecedorValePedagio.val(data.CPF_CNPJ);
}

function RetornoResponsavelValePedagio(data) {
    _valePedagioMDFeManual.ResponsavelValePedagio.val(data.CPF_CNPJ);
}

function RecarregarGridValePedagio() {
    if (_gridValePedagioMDFeManual != null) {
        _gridValePedagioMDFeManual.Destroy();
        _gridValePedagioMDFeManual = null;
    }

    var excluir = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: RemoverValePedagioClick,
        tamanho: "9",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    if (_cargaMDFeManual.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "FornecedorValePedagio", title: "Fornecedor", width: "25%", className: "text-align-left" },
        { data: "ResponsavelValePedagio", title: "Responsável", width: "25%", className: "text-align-left" },
        { data: "ComprovanteValePedagio", title: "Nº Comprovante", width: "20%", className: "text-align-left" },
        { data: "ValorValePedagio", title: "Valor", width: "15%", className: "text-align-right" }
    ];

    _gridValePedagioMDFeManual = new BasicDataTable(_valePedagioMDFeManual.GridValePedagio.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridValePedagioMDFeManual.CarregarGrid(_cargaMDFeManual.ListaValePedagio.val());
}

function RemoverValePedagioClick(data) {
    var valePedagio = _cargaMDFeManual.ListaValePedagio.val();

    for (var i = 0; i < valePedagio.length; i++) {
        if (data.Codigo == valePedagio[i].Codigo) {
            valePedagio.splice(i, 1);
            break;
        }
    }

    _cargaMDFeManual.ListaValePedagio.val(valePedagio);
    _gridValePedagioMDFeManual.CarregarGrid(_cargaMDFeManual.ListaValePedagio.val());
}

function AdicionarValePedagioClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_valePedagioMDFeManual);

    _valePedagioMDFeManual.FornecedorValePedagio.requiredClass("form-control");
    _valePedagioMDFeManual.ResponsavelValePedagio.requiredClass("form-control");

    if (valido) {

        if (!ValidarCNPJ(_valePedagioMDFeManual.FornecedorValePedagio.val(), true)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe um CNPJ válido!");
            _valePedagioMDFeManual.FornecedorValePedagio.requiredClass("form-control is-invalid");
            return;
        }

        if (!ValidarCNPJ(_valePedagioMDFeManual.ResponsavelValePedagio.val(), true)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe um CNPJ válido!");
            _valePedagioMDFeManual.ResponsavelValePedagio.requiredClass("form-control is-invalid");
            return;
        }

        var valePedagio = _cargaMDFeManual.ListaValePedagio.val();

        for (var i = 0; i < valePedagio.length; i++) {
            if (_valePedagioMDFeManual.ComprovanteValePedagio.val() == valePedagio[i].ComprovanteValePedagio) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "O número do comprovante informado (" + valePedagio[i].ComprovanteValePedagio + ") já existe.");
                return;
            }
        }

        _valePedagioMDFeManual.Codigo.val(guid());

        valePedagio.push(RetornarObjetoPesquisa(_valePedagioMDFeManual));

        _cargaMDFeManual.ListaValePedagio.val(valePedagio);
        _gridValePedagioMDFeManual.CarregarGrid(_cargaMDFeManual.ListaValePedagio.val());
        LimparCamposValePedagio();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposValePedagio() {
    LimparCampos(_valePedagioMDFeManual);
}