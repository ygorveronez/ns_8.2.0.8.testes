//*******MAPEAMENTO KNOUCKOUT*******

var _geracaoMovimentoFinanceiroCentroResultado, _crudGeracaoMovimentoFinanceiroCentroResultado, _tipoLancamentoFinanceiroSemOrcamento, _PermissoesPersonalizadas;

var _TipoDocumento = [
    { text: "Manual", value: EnumTipoDocumentoMovimento.Manual },
    { text: "Nota de Entrada", value: EnumTipoDocumentoMovimento.NotaEntrada },
    { text: "Documento Emitido", value: EnumTipoDocumentoMovimento.CTe },
    { text: "Faturamento", value: EnumTipoDocumentoMovimento.Faturamento },
    { text: "Recibo", value: EnumTipoDocumentoMovimento.Recibo },
    { text: "Pagamento", value: EnumTipoDocumentoMovimento.Pagamento },
    { text: "Recebimento", value: EnumTipoDocumentoMovimento.Recebimento },
    { text: "Nota de Saída", value: EnumTipoDocumentoMovimento.NotaSaida },
    { text: "Acerto de Viagem", value: EnumTipoDocumentoMovimento.Acerto },
    { text: "Contrato de Frete", value: EnumTipoDocumentoMovimento.ContratoFrete },
    { text: "Outros", value: EnumTipoDocumentoMovimento.Outros }
];

var GeracaoMovimentoLoteCentroResultado = function () {
    this.DataMovimento = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date });
    this.DataBase = PropertyEntity({ text: "*Data Base: ", required: true, getType: typesKnockout.date });
    this.ValorMovimento = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Manual), options: _TipoDocumento, text: "*Tipo do Documento: ", def: EnumTipoDocumentoMovimento.Manual, required: true });
    this.NumeroDocumento = PropertyEntity({ text: "*Nº Documento: ", required: true });

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Movimento:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });
    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Entrada:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(false) });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Saída:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), required: false, val: ko.observable(""), enable: ko.observable(true) });

    this.CentrosResultado = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDGeracaoMovimentoLoteCentroResultado = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Gerar Movimentos Financeiros", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar / Limpar Campos", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadGeracaoMovimentoLoteCentroResultado() {

    _geracaoMovimentoFinanceiroCentroResultado = new GeracaoMovimentoLoteCentroResultado();
    KoBindings(_geracaoMovimentoFinanceiroCentroResultado, "knockoutDetalhes");

    HeaderAuditoria("knockoutDetalhes", _geracaoMovimentoFinanceiroCentroResultado);

    _crudGeracaoMovimentoFinanceiroCentroResultado = new CRUDGeracaoMovimentoLoteCentroResultado();
    KoBindings(_crudGeracaoMovimentoFinanceiroCentroResultado, "knockoutCRUDGeracaoMovimentoLoteCentroResultado");

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarPlanoEntradaSaida, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        new BuscarPlanoConta(_geracaoMovimentoFinanceiroCentroResultado.PlanoDebito, "Selecione a Conta Analítica (Entrada)", "Contas Analíticas (Entrada)", null, EnumAnaliticoSintetico.Analitico);
        new BuscarPlanoConta(_geracaoMovimentoFinanceiroCentroResultado.PlanoCredito, "Selecione a Conta Analítica (Saída)", "Contas Analíticas (Saída)", null, EnumAnaliticoSintetico.Analitico);
        _geracaoMovimentoFinanceiroCentroResultado.PlanoDebito.enable(true);
        _geracaoMovimentoFinanceiroCentroResultado.PlanoCredito.enable(true);
    }

    new BuscarTipoMovimento(_geracaoMovimentoFinanceiroCentroResultado.TipoMovimento, null, null, RetornoTipoMovimento, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);
    new BuscarClientes(_geracaoMovimentoFinanceiroCentroResultado.Pessoa, RetornoBuscarClientes);
    new BuscarGruposPessoas(_geracaoMovimentoFinanceiroCentroResultado.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Ambos);

    LoadCentroResultado();
    CarregaUsuarioLogado();
}

function RetornoBuscarClientes(data) {
    _geracaoMovimentoFinanceiroCentroResultado.Pessoa.val(data.Nome);
    _geracaoMovimentoFinanceiroCentroResultado.Pessoa.codEntity(data.Codigo);

    if (data.CodigoGrupo > 0) {
        _geracaoMovimentoFinanceiroCentroResultado.GrupoPessoa.val(data.DescricaoGrupo);
        _geracaoMovimentoFinanceiroCentroResultado.GrupoPessoa.codEntity(data.CodigoGrupo);
    }
}

function RetornoTipoMovimento(data) {
    _geracaoMovimentoFinanceiroCentroResultado.TipoMovimento.codEntity(data.Codigo);
    _geracaoMovimentoFinanceiroCentroResultado.TipoMovimento.val(data.Descricao);

    if (data.CodigoDebito > 0) {
        _geracaoMovimentoFinanceiroCentroResultado.PlanoDebito.codEntity(data.CodigoDebito);
        _geracaoMovimentoFinanceiroCentroResultado.PlanoDebito.val(data.PlanoDebito);
    }

    if (data.CodigoCredito > 0) {
        _geracaoMovimentoFinanceiroCentroResultado.PlanoCredito.codEntity(data.CodigoCredito);
        _geracaoMovimentoFinanceiroCentroResultado.PlanoCredito.val(data.PlanoCredito);
    }
}

function AdicionarClick(e, sender) {
    Salvar(_geracaoMovimentoFinanceiroCentroResultado, "GeracaoMovimentoLoteCentroResultado/GerarMovimentos", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Movimentos gerados com sucesso.");
                LimparCamposGeracaoMovimentoLoteCentroResultado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function CancelarClick(e) {
    LimparCamposGeracaoMovimentoLoteCentroResultado();
}

//*******MÉTODOS*******

function LimparCamposGeracaoMovimentoLoteCentroResultado() {
    LimparCampos(_geracaoMovimentoFinanceiroCentroResultado);

    LimparCamposCentroResultado();
    RecarregarGridCentrosResultado();

    Global.ResetarAbas();
}

function CarregaUsuarioLogado() {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                _tipoLancamentoFinanceiroSemOrcamento = arg.Data.TipoLancamentoFinanceiroSemOrcamento;

                if (_tipoLancamentoFinanceiroSemOrcamento != EnumTipoLancamentoFinanceiroSemOrcamento.Liberar) {
                    _geracaoMovimentoFinanceiroCentroResultado.TipoMovimento.required(true);
                    _geracaoMovimentoFinanceiroCentroResultado.TipoMovimento.text("*Tipo de Movimento:");
                }
            }
        }
    });
}