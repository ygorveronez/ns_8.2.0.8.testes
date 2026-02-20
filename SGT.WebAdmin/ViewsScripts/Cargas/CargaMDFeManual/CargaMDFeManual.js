//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaMDFeManual;
var _cargaMDFeManual;
var _cargaMDFeManualInformacoesBancarias;
var _pesquisaCargaMDFeManual;
var _crudCargaMDFeManual;

var _situacaoMDFeManualPesquisa = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoMDFeManual.EmDigitacao, text: "Em Digitação" },
    { value: EnumSituacaoMDFeManual.EmEmissao, text: "Em Emissão" },
    { value: EnumSituacaoMDFeManual.Finalizado, text: "Finalizado" },
    { value: EnumSituacaoMDFeManual.Rejeicao, text: "Rejeitado" },
    { value: EnumSituacaoMDFeManual.Cancelado, text: "Cancelado" },
    { value: EnumSituacaoMDFeManual.ProcessandoIntegracao, text: "ProcessandoIntegracao" }
];

var PesquisaCargaMDFeManual = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.MDFe = PropertyEntity({ text: "Número MDF-e:", maxlength: 12, enable: ko.observable(true) });
    this.CTe = PropertyEntity({ text: "Número CT-e:", maxlength: 12, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), issue: 69, visible: ko.observable(true), enable: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", issue: 16, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", issue: 145, idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoMDFeManualPesquisa, def: "", text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaMDFeManual.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CargaMDFeManual = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Destino:", idBtnSearch: guid(), issue: 16, visible: ko.observable(false), required: false, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid(), issue: 69, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.ExigeConfirmacaoTracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(""), idBtnSearch: guid(), issue: 143, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(false), required: false, enable: ko.observable(true) });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo (Carreta 2):", idBtnSearch: guid(), visible: ko.observable(false), required: false, enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idGrid: guid(), issue: 145, visible: ko.observable(true), enable: ko.observable(true) });
    this.NomeMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PercentualExecucao = PropertyEntity({ getType: typesKnockout.text, text: '% Execucão', required: ko.observable(false), maxlength: 6, ConfigDecimal: { precision: 2, allowZero: true }, def: "0,00", enable: ko.observable(true), eventChange: FormatarPercentualChange });

    this.AdicionarMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar Motorista", idBtnSearch: guid(), visible: ko.observable(!_CONFIGURACAO_TMS.UtilizaControlePercentualExecucao), enable: ko.observable(true) });

    this.AdicionarMotoristaComissao = PropertyEntity({ eventClick: adicionarMotoristaClick, text: "Adicionar Motorista", type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarMotoristaComissao = PropertyEntity({ eventClick: atualizarMotoristaClick, type: types.event, text: "Atualizar Motorista", visible: ko.observable(false), enable: ko.observable(true) });
    this.ExcluirMotoristaComissao = PropertyEntity({ eventClick: excluirMotoristaClick, type: types.event, text: "Excluir Motorista", visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarMotoristaComissao = PropertyEntity({ eventClick: limparMotorista, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });

    this.AdicionarMotoristaNaCarga = PropertyEntity({ val: ko.observable(false), text: "Adicionar o(s) motorista(s) na carga", def: true, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Motoristas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.UsarDadosCTe = PropertyEntity({ val: ko.observable(true), text: ko.observable("Usar os dados dos CT-es para emissão do MDF-e?"), def: true, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UsarSeguroCTe = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoMDFeManual.EmDigitacao), def: EnumSituacaoMDFeManual.EmDigitacao });
    this.SituacaoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoMDFeManual.EmDigitacao), def: EnumSituacaoMDFeManual.EmDigitacao });

    this.CTes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Cargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.NFes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Destinos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaDestinos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.Percurso = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Lacres = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaLacres = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.InformacoesBancarias = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CIOT = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaCIOT = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.Seguro = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaSeguro = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.ValePedagio = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaValePedagio = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.Observacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

var CargaMDFeManualInformacoesBancarias = function () {
    let self = this;

    this.TipoPagamento = PropertyEntity({ text: "Tipo", options: EnumTipoPagamentoMDFe.ObterOpcoes(), val: ko.observable(EnumTipoPagamentoMDFe.NaoSelecionado), def: EnumTipoPagamentoMDFe.NaoSelecionado, visible: ko.observable(true) });
    this.CNPJInstituicaoPagamento = PropertyEntity({ text: ko.observable("CNPJ Instituição de Pagamento: "), maxlength: 20, required: ko.observable(false), getType: typesKnockout.cnpj, visible: ko.observable(false) });
    this.Conta = PropertyEntity({ text: ko.observable("Banco: "), required: ko.observable(false), maxlength: 20, visible: ko.observable(false) });
    this.Agencia = PropertyEntity({ text: ko.observable("Agencia: "), required: ko.observable(false), maxlength: 20, visible: ko.observable(false) });
    this.TipoChavePIX = PropertyEntity({ text: "Tipo Chave PIX :", options: EnumTipoChavePix.obterOpcoes(), val: ko.observable(EnumTipoChavePix.Nenhum), def: EnumTipoChavePix.Nenhum, visible: ko.observable(true) });
    this.ChavePIXCPFCNPJ = PropertyEntity({ text: ko.observable("Chave PIX: "), maxlength: 18, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.cpfCnpj });
    this.ChavePIXEmail = PropertyEntity({ text: ko.observable("Chave PIX: "), maxlength: 200, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.email });
    this.ChavePIXCelular = PropertyEntity({ text: ko.observable("Chave PIX: "), maxlength: 15, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.phone });
    this.ChavePIXAleatoria = PropertyEntity({ text: ko.observable("Chave PIX: "), maxlength: 200, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.text });

    this.TipoChavePIX.val.subscribe(function (tipoPix) {
        ObterCampoPixMDFeManual(self, self.TipoPagamento.val(), tipoPix, null);
    });

    this.TipoPagamento.val.subscribe(function (tipoPagamento) {
        const tipoBanco = tipoPagamento === EnumTipoPagamentoMDFe.Banco;
        const tipoIpef = tipoPagamento === EnumTipoPagamentoMDFe.Ipef;

        self.Conta.visible(tipoBanco);
        self.Conta.required(tipoBanco);
        self.Agencia.visible(tipoBanco);
        self.Agencia.required(tipoBanco);

        self.CNPJInstituicaoPagamento.visible(tipoIpef);
        self.CNPJInstituicaoPagamento.required(tipoIpef);

        ObterCampoPixMDFeManual(self, tipoPagamento, self.TipoChavePIX.val(), null);
    });
}

var CRUDCargaMDFeManual = function () {
    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Emitir = PropertyEntity({ eventClick: EmitirClick, type: types.event, text: "Emitir", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function ObterCampoPixMDFeManual(objeto, tipoPagamento, tipoChavePix, valor) {
    objeto.TipoChavePIX.visible(false);
    objeto.ChavePIXCPFCNPJ.visible(false);
    objeto.ChavePIXCPFCNPJ.required(false);
    objeto.ChavePIXEmail.visible(false);
    objeto.ChavePIXEmail.required(false);
    objeto.ChavePIXCelular.visible(false);
    objeto.ChavePIXCelular.required(false);
    objeto.ChavePIXAleatoria.visible(false);
    objeto.ChavePIXAleatoria.required(false);

    if (tipoPagamento === EnumTipoPagamentoMDFe.PIX) {
        objeto.TipoChavePIX.visible(true);
        objeto.TipoChavePIX.val(tipoChavePix);

        switch (tipoChavePix) {
            case EnumTipoChavePix.CPFCNPJ:
                if (valor) objeto.ChavePIXCPFCNPJ.val(valor);
                objeto.ChavePIXCPFCNPJ.visible(true);
                objeto.ChavePIXCPFCNPJ.required(true);
                break;
            case EnumTipoChavePix.Email:
                if (valor) objeto.ChavePIXEmail.val(valor);
                objeto.ChavePIXEmail.visible(true);
                objeto.ChavePIXEmail.required(true);
                break;
            case EnumTipoChavePix.Celular:
                if (valor) objeto.ChavePIXCelular.val(valor);
                objeto.ChavePIXCelular.visible(true);
                objeto.ChavePIXCelular.required(true);
                break;
            case EnumTipoChavePix.Aleatoria:
                if (valor) objeto.ChavePIXAleatoria.val(valor);
                objeto.ChavePIXAleatoria.visible(true);
                objeto.ChavePIXAleatoria.required(true);
                break;
        }
    }
}

function LoadCargaMDFeManual() {
    _cargaMDFeManual = new CargaMDFeManual();
    KoBindings(_cargaMDFeManual, "tabDadosGerais");

    _cargaMDFeManualInformacoesBancarias = new CargaMDFeManualInformacoesBancarias();
    KoBindings(_cargaMDFeManualInformacoesBancarias, "tabInformacoesBancarias");

    _pesquisaCargaMDFeManual = new PesquisaCargaMDFeManual();
    KoBindings(_pesquisaCargaMDFeManual, "knockoutPesquisaCargaMDFeManual", false, _pesquisaCargaMDFeManual.Pesquisar.id);

    _crudCargaMDFeManual = new CRUDCargaMDFeManual();
    KoBindings(_crudCargaMDFeManual, "knockoutCRUDCargaMDFeManual");

    HeaderAuditoria("CargaMDFeManual", _cargaMDFeManual);

    new BuscarLocalidadesBrasil(_cargaMDFeManual.Origem, "Buscar Origem", "Origens", RetornoConsultaOrigem);
    new BuscarLocalidadesBrasil(_cargaMDFeManual.Destino, "Buscar Destino", "Destinos", RetornoConsultaDestino);
    new BuscarTransportadores(_cargaMDFeManual.Empresa, callbackTransportador);
    new BuscarTransportadores(_pesquisaCargaMDFeManual.Empresa);

    BuscarVeiculos(_cargaMDFeManual.Veiculo, RetornoConsultaVeiculo, _cargaMDFeManual.Empresa, null, null, true);
    BuscarReboques(_cargaMDFeManual.Reboque);
    BuscarReboques(_cargaMDFeManual.SegundoReboque);
    //BuscarMotoristas(_cargaMDFeManual.Motorista, null, _cargaMDFeManual.Empresa);

    BuscarLocalidadesBrasil(_pesquisaCargaMDFeManual.Origem, "Buscar Origem", "Origens");
    BuscarLocalidadesBrasil(_pesquisaCargaMDFeManual.Destino, "Buscar Destino", "Destinos");
    BuscarVeiculos(_pesquisaCargaMDFeManual.Veiculo);
    BuscarMotoristas(_pesquisaCargaMDFeManual.Motorista);
    BuscarCargas(_pesquisaCargaMDFeManual.Carga);

    $("#" + _cargaMDFeManual.UsarDadosCTe.id).click(usarDadosCTeClick);
    LimparPropriedadesVeiculo();

    LoadConexaoSignalRCargaMDFeManual();
    LoadMotoristas();
    loadEtapasMDFe();
    loadDestinos();
    LoadCTes();
    LoadCargas();
    LoadNFes();
    LoadPercurso();
    LoadLacre();
    LoadCIOT();
    LoadSeguro();
    LoadValePedagio();
    LoadObservacao();
    loadMDFe();
    LoadImpressaoMDFeManual();
    BuscarCargaMDFeManual();
    LoadIntegracoesMDFeManual();


    $("#" + _cargaMDFeManualInformacoesBancarias.ChavePIXCelular.id).mask("(00) 00000-0000", { selectOnFocus: true, clearIfNotMatch: true });

    $("#" + _cargaMDFeManualInformacoesBancarias.ChavePIXCPFCNPJ.id).mask('000.000.000-000', {
        selectOnFocus: true,
        translation: {
            '0': { pattern: /[0-9]/ }
        },
        onKeyPress: function (val, e, field, options) {
            var cleanVal = val.replace(/\D/g, '');
            var masks = ['000.000.000-000', '00.000.000/0000-00'];
            var mask = (cleanVal.length > 11) ? masks[1] : masks[0];

            field.mask(mask, {
                selectOnFocus: true,
                translation: {
                    '0': { pattern: /[0-9]/ }
                }
            });
        }
    });

    $("#" + _cargaMDFeManualInformacoesBancarias.ChavePIXEmail.id).on('input blur', function () {
        var email = this.value;
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email && !emailRegex.test(email)) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }

        _cargaMDFeManualInformacoesBancarias.ChavePIXEmail.val(email);
    });


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
        $("#tabNFes").hide();
    }
    else {
        LayoutMultiNFe();
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe) {
        _cargaMDFeManual.Empresa.visible(false);
        _cargaMDFeManual.Empresa.required = false;
        _pesquisaCargaMDFeManual.Empresa.visible(false);
        SetarEnableCamposKnockout(_cargasMDFeManual, true);
        SetarEnableCamposKnockout(_ctesMDFeManual, true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _cargaMDFeManual.Empresa.text("*Empresa/Filial: ");
        _pesquisaCargaMDFeManual.Empresa.text("*Empresa/Filial: ");
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        SetarEnableCamposKnockout(_cargasMDFeManual, false);
        SetarEnableCamposKnockout(_ctesMDFeManual, false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liTabObservacao").hide();
    }

    if (_CONFIGURACAO_TMS.PermitirAdicionarMotoristaCargaMDFeManual) {
        _cargaMDFeManual.AdicionarMotoristaNaCarga.val(true);
        _cargaMDFeManual.AdicionarMotoristaNaCarga.visible(true);
    }
}

function LayoutMultiNFe() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        $("#tabNFes").show();
        Global.ExibirAba("knockoutNFes");
        $("#tabCargas").hide();
        $("#tabCTes").hide();

        _cargaMDFeManual.AdicionarMotoristaNaCarga.val(false);
        _cargaMDFeManual.AdicionarMotoristaNaCarga.visible(false);
        _cargaMDFeManual.Empresa.visible(false);
        _cargaMDFeManual.UsarDadosCTe.text("Usar os dados das NF-es para emissão do MDF-e?");
        _seguroMDFeManual.UsarDadosCTe.text("Não informar os dados do seguro para o MDF-e?");
        _seguroMDFeManual.UsarDadosCTe.val(false);

        executarReST("DocumentoEntrada/BuscarEmpresaUsuario", { Codigo: 0 }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _cargaMDFeManual.Empresa.codEntity(arg.Data.Codigo);
                    _cargaMDFeManual.Empresa.val(arg.Data.Nome);
                    _cargaMDFeManual.Empresa.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);

    }
}

function callbackTransportador(data) {
    _cargaMDFeManual.Empresa.codEntity(data.Codigo);
    _cargaMDFeManual.Empresa.val(data.Descricao);
    SetarEnableCamposKnockout(_cargasMDFeManual, true);
    SetarEnableCamposKnockout(_ctesMDFeManual, true);
    _cargaMDFeManual.Empresa.enable(false);

    if (data.EmpresaPropria)
        DesativarSelecaoCTe();
    else
        HabilitarSelecaoCTe();
}

function usarDadosCTeClick() {
    if (_cargaMDFeManual.UsarDadosCTe.val()) {
        _cargaMDFeManual.Destino.visible(false);
        _cargaMDFeManual.Destino.required = false;
        $("#liDestinos").show();
    } else {
        _cargaMDFeManual.Destino.visible(true);
        _cargaMDFeManual.Destino.required = true;
        $("#liDestinos").hide();
    }
}

function RetornoConsultaVeiculo(veiculo) {
    _cargaMDFeManual.Veiculo.val(veiculo.ConjuntoPlacasComModeloVeicularEFrota);
    _cargaMDFeManual.Veiculo.codEntity(veiculo.Codigo);

    if (veiculo.CodigoMotorista > 0) 
        _cargaMDFeManual.Motorista.basicTable.CarregarGrid([{ Codigo: veiculo.CodigoMotorista, Descricao: veiculo.Motorista, PercentualExecucao: Globalize.format("0,00", "n2") }]);   
}

function ExibirVeiculosSeparados(numeroReboques) {
    _cargaMDFeManual.Veiculo.text("*Tração (Cavalo):");

    _cargaMDFeManual.Reboque.visible(true);
    _cargaMDFeManual.Reboque.text("Veículo (Carreta):");
    _cargaMDFeManual.Reboque.required = true;

    if (numeroReboques >= 2) {
        _cargaMDFeManual.Reboque.text("*Veículo (Carreta 1):");
        _cargaMDFeManual.SegundoReboque.visible(true);
        _cargaMDFeManual.SegundoReboque.required = true;
    }
}

function VerificarRegraVeiculo() {
    var cargas = _gridCargas.BuscarRegistros();

    var especificarReboques = cargas.some(function (carga) {
        return carga.ExigeConfirmacaoTracao;
    });

    var numeroReboques = Math.max.apply(Math, cargas.map(function (carga) {
        return parseInt(carga.NumeroReboques);
    }));

    LimparPropriedadesVeiculo();
    _cargaMDFeManual.ExigeConfirmacaoTracao.val(especificarReboques);

    if (especificarReboques && numeroReboques > 0)
        ExibirVeiculosSeparados(numeroReboques);
}

function RetornoConsultaOrigem(origem) {
    _cargaMDFeManual.Origem.val(origem.Descricao);
    _cargaMDFeManual.Origem.codEntity(origem.Codigo);
    BuscarPercursoMDFe();
}

function RetornoConsultaDestino(destino) {
    _cargaMDFeManual.Destino.val(destino.Descricao);
    _cargaMDFeManual.Destino.codEntity(destino.Codigo);
    BuscarPercursoMDFe();
}

function SalvarClick() {
    if (!ValidarCamposObrigatorios(_cargaMDFeManual)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    if (_ctesMDFeManual.CtesInfo.basicTable.BuscarRegistros().length <= 0 && _cargasMDFeManual.CargasInfo.basicTable.BuscarRegistros().length <= 0 && _nfesMDFeManual.NFesInfo.basicTable.BuscarRegistros().length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar ao menos um CT-e, Carga ou NF-e para realizar a emissão do MDF-e.");
        return;
    }
    if (_mapaPercursoMDFe.ValidarPassagens() !== true)
        return;

    PreencherDadosGerais();
    limparMotorista();

    Salvar(_cargaMDFeManual, "CargaMDFeManual/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso.");
                PreencherObjetoKnout(_cargaMDFeManual, arg);
                SetarDadosGeraisCargaMDFeManual(arg);
                _gridCargaMDFeManual.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function EmitirClick(e, sender) {
    if (!ValidarCamposObrigatorios(_cargaMDFeManual)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    if (_ctesMDFeManual.CtesInfo.basicTable.BuscarRegistros().length <= 0 && _cargasMDFeManual.CargasInfo.basicTable.BuscarRegistros().length <= 0 && _nfesMDFeManual.NFesInfo.basicTable.BuscarRegistros().length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar ao menos um CT-e ou uma Carga para realizar a emissão do MDF-e.");
        return;
    }

    if (_mapaPercursoMDFe.ValidarPassagens() !== true)
        return;

    PreencherDadosGerais();

    exibirConfirmacao("Confirmação", "Realmente deseja autorizar a emissão do MDF-e?", function (confi) {
        Salvar(_cargaMDFeManual, "CargaMDFeManual/Emitir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Emissão realizada com sucesso.");
                    PreencherObjetoKnout(_cargaMDFeManual, arg);
                    SetarDadosGeraisCargaMDFeManual(arg);
                    _gridCargaMDFeManual.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    BuscarCargaMDFeManual();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, sender);
    });
}

function CancelarClick() {
    LimparCamposCargaMDFeManual();
    SetarEtapaMDFe();
}

//*******MÉTODOS*******

function PreencherDadosGerais() {
    var ctes = new Array();
    var cargas = new Array();
    var nfes = new Array();

    $.each(_ctesMDFeManual.CtesInfo.basicTable.BuscarRegistros(), function (i, cte) {
        ctes.push(cte.Codigo);
    });

    $.each(_cargasMDFeManual.CargasInfo.basicTable.BuscarRegistros(), function (i, cte) {
        cargas.push(cte.Codigo);
    });

    $.each(_nfesMDFeManual.NFesInfo.basicTable.BuscarRegistros(), function (i, cte) {
        nfes.push(cte.Chave);
    });

    var dadosBancarios = RetornarObjetoPesquisa(_cargaMDFeManualInformacoesBancarias);
    var chavePIX = "";

    switch (dadosBancarios.TipoChavePIX) {
        case EnumTipoChavePix.CPFCNPJ: chavePIX = _cargaMDFeManualInformacoesBancarias.ChavePIXCPFCNPJ.val(); break;
        case EnumTipoChavePix.Email: chavePIX = _cargaMDFeManualInformacoesBancarias.ChavePIXEmail.val(); break;
        case EnumTipoChavePix.Celular: chavePIX = _cargaMDFeManualInformacoesBancarias.ChavePIXCelular.val(); break;
        case EnumTipoChavePix.Aleatoria: chavePIX = _cargaMDFeManualInformacoesBancarias.ChavePIXAleatoria.val(); break;
    }

    dadosBancarios["ChavePIX"] = chavePIX;

    _cargaMDFeManual.CTes.val(JSON.stringify(ctes));
    _cargaMDFeManual.Cargas.val(JSON.stringify(cargas));
    _cargaMDFeManual.NFes.val(JSON.stringify(nfes));

    _cargaMDFeManual.Percurso.val(JSON.stringify(_mapaPercursoMDFe.GetEstadosPassagem()));
    _cargaMDFeManual.Lacres.val(JSON.stringify(_cargaMDFeManual.ListaLacres.val()));
    _cargaMDFeManual.CIOT.val(JSON.stringify(_cargaMDFeManual.ListaCIOT.val()));
    _cargaMDFeManual.InformacoesBancarias.val(JSON.stringify(dadosBancarios));
    _cargaMDFeManual.Destinos.val(JSON.stringify(_cargaMDFeManual.ListaDestinos.val()));
    _cargaMDFeManual.Seguro.val(JSON.stringify(_cargaMDFeManual.ListaSeguro.val()));
    _cargaMDFeManual.ValePedagio.val(JSON.stringify(_cargaMDFeManual.ListaValePedagio.val()));
    _cargaMDFeManual.Motoristas.val(JSON.stringify(_cargaMDFeManual.Motorista.basicTable.BuscarRegistros()));
    _cargaMDFeManual.Observacao.val(JSON.stringify(RetornarObjetoPesquisa(_observacaoMDFeManual)));
}

function BuscarCargaMDFeManual() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarCargaMDFeManual, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCargaMDFeManual = new GridView(_pesquisaCargaMDFeManual.Pesquisar.idGrid, "CargaMDFeManual/Pesquisa", _pesquisaCargaMDFeManual, menuOpcoes, null);
    _gridCargaMDFeManual.CarregarGrid();
}

function EditarCargaMDFeManual(cargaMDFeManualGrid) {
    LimparCamposCargaMDFeManual();

    if (cargaMDFeManualGrid.Situacao == "Processando Integração") {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Aguarde processamento do MDFe para editar");
    }
    else {
        _cargaMDFeManual.Codigo.val(cargaMDFeManualGrid.Codigo);
        BuscarCargaMDFeManualPorCodigo();
    }
}

function SetarInformacoesVeiculo(veiculo, reboques) {
    _cargaMDFeManual.Veiculo.codEntity(veiculo.Codigo);

    if (_cargaMDFeManual.ExigeConfirmacaoTracao.val())
        _cargaMDFeManual.Veiculo.val(veiculo.Placa || veiculo.Descricao);
    else
        _cargaMDFeManual.Veiculo.val(veiculo.Descricao);

    if (reboques != null && reboques[0]) {
        var reboque1 = reboques[0];
        _cargaMDFeManual.Reboque.val(reboque1.Descricao);
        _cargaMDFeManual.Reboque.codEntity(reboque1.Codigo);
    }

    if (reboques != null && reboques[1]) {
        var reboque2 = reboques[1];
        _cargaMDFeManual.SegundoReboque.val(reboque2.Descricao);
        _cargaMDFeManual.SegundoReboque.codEntity(reboque2.Codigo);
    }
}

function BuscarCargaMDFeManualPorCodigo(callback) {
    BuscarPorCodigo(_cargaMDFeManual, "CargaMDFeManual/BuscarPorCodigo", function (arg) {
        SetarDadosGeraisCargaMDFeManual(arg);
        _cargaMDFeManual.Empresa.enable(false);

        _cargaMDFeManual.Motorista.basicTable.SetarRegistros(_cargaMDFeManual.Motoristas.val());

        if (_cargaMDFeManual.Situacao.val() == EnumSituacaoMDFeManual.EmDigitacao) {
            SetarEnableCamposKnockout(_cargasMDFeManual, true);
            SetarEnableCamposKnockout(_ctesMDFeManual, true);
            _cargaMDFeManual.Motorista.basicTable.HabilitarOpcoes();
            _cargaMDFeManual.AdicionarMotoristaComissao.visible(true);
            _cargaMDFeManual.AtualizarMotoristaComissao.visible(false);
            _cargaMDFeManual.ExcluirMotoristaComissao.visible(false);
            _cargaMDFeManual.CancelarMotoristaComissao.visible(false);
        } else {
            _cargaMDFeManual.Motorista.basicTable.DesabilitarOpcoes();
            _cargaMDFeManual.AdicionarMotoristaComissao.visible(false);
            _cargaMDFeManual.AtualizarMotoristaComissao.visible(false);
            _cargaMDFeManual.ExcluirMotoristaComissao.visible(false);
            _cargaMDFeManual.CancelarMotoristaComissao.visible(false);
        }

        SetarInformacoesVeiculo(arg.Data.Veiculo, arg.Data.Veiculo.Reboques);
        if (_cargaMDFeManual.ExigeConfirmacaoTracao.val())
            ExibirVeiculosSeparados(arg.Data.Veiculo.Reboques.length);

        if (callback != null)
            callback();
    }, null);
}


function DefinirDadosBancarios(dadosBancarios) {
    _cargaMDFeManualInformacoesBancarias.TipoPagamento.val(dadosBancarios.TipoPagamento);

    _cargaMDFeManualInformacoesBancarias.TipoChavePIX.val(dadosBancarios.TipoChavePIX);
    switch (dadosBancarios.TipoChavePIX) {
        case EnumTipoChavePix.CPFCNPJ: _cargaMDFeManualInformacoesBancarias.ChavePIXCPFCNPJ.val(dadosBancarios.ChavePIX); break;
        case EnumTipoChavePix.Email: _cargaMDFeManualInformacoesBancarias.ChavePIXEmail.val(dadosBancarios.ChavePIX); break;
        case EnumTipoChavePix.Celular: _cargaMDFeManualInformacoesBancarias.ChavePIXCelular.val(dadosBancarios.ChavePIX); break;
        case EnumTipoChavePix.Aleatoria: _cargaMDFeManualInformacoesBancarias.ChavePIXAleatoria.val(dadosBancarios.ChavePIX); break;
        default: break;
    }
    _cargaMDFeManualInformacoesBancarias.CNPJInstituicaoPagamento.val(dadosBancarios.CNPJInstituicaoPagamento);
    _cargaMDFeManualInformacoesBancarias.Conta.val(dadosBancarios.Conta);
    _cargaMDFeManualInformacoesBancarias.Agencia.val(dadosBancarios.Agencia);
}

function SetarDadosGeraisCargaMDFeManual(arg) {
    _cargaMDFeManual.ListaLacres.val(_cargaMDFeManual.Lacres.val());
    RecarregarGridLacres();

    _cargaMDFeManual.ListaCIOT.val(_cargaMDFeManual.CIOT.val());
    RecarregarGridCIOT();

    _cargaMDFeManual.ListaSeguro.val(_cargaMDFeManual.Seguro.val());
    RecarregarGridSeguro();

    _cargaMDFeManual.ListaValePedagio.val(_cargaMDFeManual.ValePedagio.val());
    RecarregarGridValePedagio();

    _cargaMDFeManual.ListaDestinos.val(_cargaMDFeManual.Destinos.val());
    recarregarGridReorder();

    DefinirDadosBancarios(arg.Data);

    AtualizarObservacaoMDFeManual(arg.Data.Observacao);
    AtualizarInformacoesMapaMDFeManual(arg.Data.Percurso);

    if (arg.Data.Empresa.EmpresaPropria)
        DesativarSelecaoCTe();
    else
        RecarregarCTes(arg.Data.CTes);

    RecarregarCargas(arg.Data.Cargas);
    RecarregarNFes(arg.Data.NFes);

    _pesquisaCargaMDFeManual.ExibirFiltros.visibleFade(false);

    _seguroMDFeManual.UsarDadosCTe.val(_cargaMDFeManual.UsarSeguroCTe.val());

    _cargaMDFeManual.Motorista.basicTable.SetarRegistros(_cargaMDFeManual.Motoristas.val());

    if (_cargaMDFeManual.Situacao.val() == EnumSituacaoMDFeManual.EmDigitacao) {
        _crudCargaMDFeManual.Emitir.visible(true);
        _crudCargaMDFeManual.Salvar.visible(true);
        _cargaMDFeManual.Motorista.basicTable.HabilitarOpcoes();
        _cargaMDFeManual.AdicionarMotoristaComissao.visible(true);
        _cargaMDFeManual.AtualizarMotoristaComissao.visible(false);
        _cargaMDFeManual.ExcluirMotoristaComissao.visible(false);
        _cargaMDFeManual.CancelarMotoristaComissao.visible(false);
    } else {
        _crudCargaMDFeManual.Emitir.visible(false);
        _crudCargaMDFeManual.Salvar.visible(false);

        _cargaMDFeManual.AdicionarMotoristaComissao.visible(false);
        _cargaMDFeManual.AtualizarMotoristaComissao.visible(false);
        _cargaMDFeManual.ExcluirMotoristaComissao.visible(false);
        _cargaMDFeManual.CancelarMotoristaComissao.visible(false);

        SetarEnableCamposKnockout(_cargaMDFeManual, false);
        SetarEnableCamposKnockout(_ordemDestino, false);
        SetarEnableCamposKnockout(_ctesMDFeManual, false);
        SetarEnableCamposKnockout(_cargasMDFeManual, false);
        SetarEnableCamposKnockout(_lacresMDFeManual, false);

        SetarEnableCamposKnockout(_ciotMDFeManual, false);
        SetarEnableCamposKnockout(_seguroMDFeManual, false);
        SetarEnableCamposKnockout(_valePedagioMDFeManual, false);
        SetarEnableCamposKnockout(_observacaoMDFeManual, false);

        _cargaMDFeManual.Motorista.basicTable.DesabilitarOpcoes();
    }
    SetarEtapaMDFe();

}

function LimparPropriedadesVeiculo() {
    _cargaMDFeManual.Veiculo.text("*Veículo:");
    _cargaMDFeManual.Reboque.text("*Veículo (Carreta):");
    _cargaMDFeManual.Reboque.visible(false);
    _cargaMDFeManual.Reboque.val("");
    _cargaMDFeManual.Reboque.codEntity(0);
    _cargaMDFeManual.SegundoReboque.visible(false);
    _cargaMDFeManual.SegundoReboque.val("");
    _cargaMDFeManual.SegundoReboque.codEntity(0);
}

function LimparCamposCargaMDFeManual() {
    _crudCargaMDFeManual.Emitir.visible(true);
    _crudCargaMDFeManual.Salvar.visible(true);
    LimparPropriedadesVeiculo();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        SetarEnableCamposKnockout(_cargasMDFeManual, true);
        SetarEnableCamposKnockout(_ctesMDFeManual, true);
    } else {
        SetarEnableCamposKnockout(_cargasMDFeManual, false);
        SetarEnableCamposKnockout(_ctesMDFeManual, false);
    }

    SetarEnableCamposKnockout(_ordemDestino, true);
    SetarEnableCamposKnockout(_cargaMDFeManual, true);
    SetarEnableCamposKnockout(_lacresMDFeManual, true);

    SetarEnableCamposKnockout(_ciotMDFeManual, true);
    SetarEnableCamposKnockout(_seguroMDFeManual, true);
    SetarEnableCamposKnockout(_valePedagioMDFeManual, true);
    SetarEnableCamposKnockout(_observacaoMDFeManual, true);

    _cargaMDFeManual.ListaLacres.val([]);
    _cargaMDFeManual.ListaCIOT.val([]);
    _cargaMDFeManual.ListaSeguro.val([]);
    _cargaMDFeManual.ListaValePedagio.val([]);
    _cargaMDFeManual.ListaDestinos.val([]);

    _cargaMDFeManual.Motorista.basicTable.SetarRegistros([]);
    _cargaMDFeManual.Motorista.basicTable.HabilitarOpcoes();
    _cargaMDFeManual.AdicionarMotoristaComissao.visible(true);
    _cargaMDFeManual.AtualizarMotoristaComissao.visible(false);
    _cargaMDFeManual.ExcluirMotoristaComissao.visible(false);
    _cargaMDFeManual.CancelarMotoristaComissao.visible(false);

    recarregarGridReorder();
    RecarregarCargas();
    RecarregarNFes();

    _mapaPercursoMDFe.LimparMapa();
    LimparCampos(_cargaMDFeManual);
    LimparCampos(_ctesMDFeManual);
    LimparCampos(_lacresMDFeManual);

    LimparCampos(_ciotMDFeManual);
    LimparCampos(_seguroMDFeManual);
    LimparCampos(_valePedagioMDFeManual);
    LimparCampos(_observacaoMDFeManual);

    RecarregarGridLacres();
    RecarregarGridCIOT();
    RecarregarGridSeguro();
    RecarregarGridValePedagio();
    RecarregarCTes();
    HabilitarSelecaoCTe();

    $("#liDestinos").show();

}

function FormatarPercentualChange(e,sender) {
    let valor = _cargaMDFeManual.PercentualExecucao.val();

    // Remove tudo que não for número ou vírgula
    let valorLimpo = valor.replace(/[^\d,]/g, '');

    // Substitui vírgula por ponto e converte
    let valorNumerico = parseFloat(valorLimpo.replace(',', '.'));

    if (isNaN(valorNumerico)) {
        valorNumerico = 0;
    }

    // Formata como 0,00
    let valorFormatado = valorNumerico.toFixed(2).replace('.', ',');

    _cargaMDFeManual.PercentualExecucao.val(valorFormatado);
};

