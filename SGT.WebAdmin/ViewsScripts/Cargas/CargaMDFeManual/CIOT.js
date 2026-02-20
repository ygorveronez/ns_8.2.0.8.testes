//*******MAPEAMENTO KNOUCKOUT*******

var _gridCIOTMDFeManual, _ciotMDFeManual;

var CIOTMDFeManual = function () {
    this.GridCIOT = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.NumeroCIOT = PropertyEntity({ text: "*Número CIOT:", required: true, maxlength: 12, enable: ko.observable(true) });
    this.ResponsavelCIOT = PropertyEntity({ text: "*CNPJ Responsável:", required: true, getType: typesKnockout.cnpj, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConsultarResponsavelCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Buscar", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ValorFrete = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Frete: ", required: false, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorAdiantamento = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Adiantamento: ", required: false, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(true) });
    this.FormaPagamento = PropertyEntity({ text: "Forma de Pagamento", options: EnumFormaPagamentoCIOT.obterOpcoes(), val: ko.observable(EnumFormaPagamentoCIOT.NaoSelecionado), def: EnumFormaPagamentoCIOT.NaoSelecionado, visible: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: "Data Vencimento: ", required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.FormaPagamento.val.subscribe(function (novoValor) {
        if (novoValor === EnumFormaPagamentoCIOT.AVista) {
            _ciotMDFeManual.DataVencimento.visible(false);
            _ciotMDFeManual.ValorAdiantamento.visible(false);
            _ciotMDFeManual.DataVencimento.required(false);
        } else if (novoValor === EnumFormaPagamentoCIOT.APrazo) {
            _ciotMDFeManual.DataVencimento.visible(true);
            _ciotMDFeManual.DataVencimento.required(true);
            _ciotMDFeManual.ValorAdiantamento.visible(true);
        } else {
            _ciotMDFeManual.DataVencimento.visible(false);
            _ciotMDFeManual.DataVencimento.required(false);
            _ciotMDFeManual.ValorAdiantamento.visible(false);
        }
    });

    this.AdicionarCIOT = PropertyEntity({ eventClick: AdicionarCIOTClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

function LoadCIOT() {

    _ciotMDFeManual = new CIOTMDFeManual();
    KoBindings(_ciotMDFeManual, "tabCIOT");

    BuscarClientes(_ciotMDFeManual.ConsultarResponsavelCIOT, RetornoResponsavelCIOT);

    RecarregarGridCIOT();
}

function RetornoResponsavelCIOT(data) {
    _ciotMDFeManual.ResponsavelCIOT.val(data.CPF_CNPJ);
}

function RecarregarGridCIOT() {
    if (_gridCIOTMDFeManual != null) {
        _gridCIOTMDFeManual.Destroy();
        _gridCIOTMDFeManual = null;
    }

    var excluir = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: RemoverCIOTClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    if (_cargaMDFeManual.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    // adicionar campos: formaPagamento, valorAdiantamento (se maior que 0), valorFrete (se maior que 0) e dataVencimento (se existir)
    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroCIOT", title: "Número", width: "30%", className: "text-align-left" },
        { data: "ResponsavelCIOT", title: "Responsável", width: "60%", className: "text-align-left" }
    ];

    _gridCIOTMDFeManual = new BasicDataTable(_ciotMDFeManual.GridCIOT.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridCIOTMDFeManual.CarregarGrid(_cargaMDFeManual.ListaCIOT.val());
}

function RemoverCIOTClick(data) {
    var ciot = _cargaMDFeManual.ListaCIOT.val();

    for (var i = 0; i < ciot.length; i++) {
        if (data.Codigo == ciot[i].Codigo) {
            ciot.splice(i, 1);
            break;
        }
    }

    _cargaMDFeManual.ListaCIOT.val(ciot);
    _gridCIOTMDFeManual.CarregarGrid(_cargaMDFeManual.ListaCIOT.val());
}

function AdicionarCIOTClick(e, sender) {
    _ciotMDFeManual.ResponsavelCIOT.requiredClass("form-control");

    const valorCIOT = _ciotMDFeManual.NumeroCIOT.val();

    if (valorCIOT && valorCIOT.trim() && valorCIOT.length !== 12) {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "O Campo CIOT deve conter 12 digitos");
        return _ciotMDFeManual.CIOT.requiredClass("form-control  is-invalid");
    }

    const formaPagamento = _ciotMDFeManual.FormaPagamento.val();
    if (formaPagamento === EnumFormaPagamentoCIOT.NaoSelecionado) {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "O campo Forma de Pagamento deve ser informado");
        return _ciotMDFeManual.FormaPagamento.requiredClass("form-control  is-invalid");
    }

    if (_ciotMDFeManual.DataVencimento.visible()) {
        if (!_ciotMDFeManual.DataVencimento.val()) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "A 'Data Vencimento' é obrigatória para pagamento a prazo.");
            return _ciotMDFeManual.DataVencimento.requiredClass("form-control  is-invalid");
        }

        var partes = _ciotMDFeManual.DataVencimento.val().split('/');
        var dataVencimento = new Date(partes[2], partes[1] - 1, partes[0]);

        var hoje = new Date();
        hoje.setHours(0, 0, 0, 0);

        if (dataVencimento < hoje) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "A 'Data Vencimento' não pode ser menor que a data atual.");
            return _ciotMDFeManual.DataVencimento.requiredClass("form-control  is-invalid");
        }
    }

    if (_ciotMDFeManual.FormaPagamento.val() == EnumFormaPagamentoCIOT.APrazo) {
        const valorAdiantamento = _ciotMDFeManual.ValorAdiantamento.val() ?? 0;
        const valorFrete = _ciotMDFeManual.ValorFrete.val() ?? 0;

        if (valorAdiantamento <= 0) {
            _ciotMDFeManual.ValorAdiantamento.requiredClass("form-control  is-invalid");
            return exibirMensagem(tipoMensagem.atencao, "Atenção!", "'Valor adiantamento' é obrigatório quando 'À prazo'"); 
        }

        if (valorAdiantamento >= valorFrete){
            _ciotMDFeManual.ValorAdiantamento.requiredClass("form-control  is-invalid");
            _ciotMDFeManual.ValorFrete.requiredClass("form-control  is-invalid");
            return exibirMensagem(tipoMensagem.atencao, "Atenção!", "'Valor Adiantamento' deve ser menor que o 'Valor Frete'"); 
        }
    }

    var valido = ValidarCamposObrigatorios(_ciotMDFeManual);

    if (valido) {

        if (!ValidarCNPJ(_ciotMDFeManual.ResponsavelCIOT.val(), true)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe um CNPJ válido!");
            _ciotMDFeManual.ResponsavelCIOT.requiredClass("form-control is-invalid");
            return;
        }

        var ciot = _cargaMDFeManual.ListaCIOT.val();

        for (var i = 0; i < ciot.length; i++) {
            if (_ciotMDFeManual.NumeroCIOT.val() == ciot[i].NumeroCIOT) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "O CIOT informado (" + ciot[i].NumeroCIOT + ") já existe.");
                return;
            }
        }

        _ciotMDFeManual.Codigo.val(guid());

        ciot.push(RetornarObjetoPesquisa(_ciotMDFeManual));

        _cargaMDFeManual.ListaCIOT.val(ciot);
        _gridCIOTMDFeManual.CarregarGrid(_cargaMDFeManual.ListaCIOT.val());
        LimparCamposCIOT();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposCIOT() {
    LimparCampos(_ciotMDFeManual);
}