var _acrescimoDescontoLiquidacaoFatura, _gridAcrescimoDescontoLiquidacaoFatura, _modalAcrescimoDescontoLiquidacaoFatura;

var AcrescimoDescontoLiquidacaoFatura = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", required: false, maxlength: 500 })

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcrescimoDescontoLiquidacaoFaturaClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaAcrescimoDescontoLiquidacaoFatura, type: types.event, text: "Fechar", icon: "fal fa-window-close", visible: ko.observable(true) });
}

////*******EVENTOS*******

function LoadAcrescimoDescontoLiquidacaoFatura() {
    _acrescimoDescontoLiquidacaoFatura = new AcrescimoDescontoLiquidacaoFatura();
    KoBindings(_acrescimoDescontoLiquidacaoFatura, "knockoutAcrescimoDescontoLiquidacaoFatura");

    new BuscarJustificativas(_acrescimoDescontoLiquidacaoFatura.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.TitulosReceber, EnumTipoFinalidadeJustificativa.Todas]);

    CarregarGridAcrescimoDescontoLiquidacaoFatura();

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_BloquearAcrescimoDesconto, _PermissoesPersonalizadas))
        $("#divContainerAcrescimoDescontoLiquidacaoFatura").addClass("hidden");
    else
        $("#divContainerAcrescimoDescontoLiquidacaoFatura").removeClass("hidden");
    _modalAcrescimoDescontoLiquidacaoFatura = new bootstrap.Modal(document.getElementById("knockoutAcrescimoDescontoLiquidacaoFatura"), { backdrop: true, keyboard: true });
}

function AdicionarAcrescimoDescontoLiquidacaoFaturaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_acrescimoDescontoLiquidacaoFatura);

    if (valido) {

        _acrescimoDescontoLiquidacaoFatura.Codigo.val(guid());

        _liquidarFatura.ListaAcrescimosDescontos.val().push({
            Codigo: _acrescimoDescontoLiquidacaoFatura.Codigo.val(),
            Valor: _acrescimoDescontoLiquidacaoFatura.Valor.val(),
            Justificativa: {
                Codigo: _acrescimoDescontoLiquidacaoFatura.Justificativa.codEntity(),
                Descricao: _acrescimoDescontoLiquidacaoFatura.Justificativa.val(),
            },
            Observacao: _acrescimoDescontoLiquidacaoFatura.Observacao.val()
        });

        RecarregarGridAcrescimoDescontoLiquidacaoFatura();

        FecharTelaAcrescimoDescontoLiquidacaoFatura();

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirAcrescimoDescontoLiquidacaoFaturaClick(data, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o registro?", function () {
        for (var i = 0; i < _liquidarFatura.ListaAcrescimosDescontos.val().length; i++) {
            if (data.Codigo == _liquidarFatura.ListaAcrescimosDescontos.val()[i].Codigo) {
                _liquidarFatura.ListaAcrescimosDescontos.val().splice(i, 1);
                break;
            }
        }

        RecarregarGridAcrescimoDescontoLiquidacaoFatura();
    });
}

////*******METODOS*******

function CarregarGridAcrescimoDescontoLiquidacaoFatura() {

    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirAcrescimoDescontoLiquidacaoFaturaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Justificativa", title: "Justificativa", width: "60%" },
        { data: "Valor", title: "Valor", width: "30%" },
    ];

    _gridAcrescimoDescontoLiquidacaoFatura = new BasicDataTable(_liquidarFatura.GridAcrescimosDescontos.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
    _gridAcrescimoDescontoLiquidacaoFatura.CarregarGrid([]);
}

function RecarregarGridAcrescimoDescontoLiquidacaoFatura() {

    var data = new Array();

    $.each(_liquidarFatura.ListaAcrescimosDescontos.val(), function (i, acrescimoDesconto) {
        var acrescimoDescontoGrid = new Object();

        acrescimoDescontoGrid.Codigo = acrescimoDesconto.Codigo;
        acrescimoDescontoGrid.Justificativa = acrescimoDesconto.Justificativa.Descricao;
        acrescimoDescontoGrid.Valor = acrescimoDesconto.Valor;

        data.push(acrescimoDescontoGrid);
    });

    _gridAcrescimoDescontoLiquidacaoFatura.CarregarGrid(data);
}

function LimparCamposAcrescimoDescontoLiquidacaoFatura() {
    LimparCampos(_acrescimoDescontoLiquidacaoFatura);
}

function AbrirTelaAcrescimoDescontoLiquidacaoFatura() {
    LimparCamposAcrescimoDescontoLiquidacaoFatura();
    _modalAcrescimoDescontoLiquidacaoFatura.show();
}

function FecharTelaAcrescimoDescontoLiquidacaoFatura() {
    _modalAcrescimoDescontoLiquidacaoFatura.hide();
    LimparCamposAcrescimoDescontoLiquidacaoFatura();
}