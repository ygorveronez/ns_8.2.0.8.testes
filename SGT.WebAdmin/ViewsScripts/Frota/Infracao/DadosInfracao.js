/// <reference path="Infracao.js" />
/// <reference path="../../Consultas/TipoInfracao.js" />
/// <reference path="../../Consultas/CargaCte.js" />
/// <reference path="../../Consultas/Seguradora.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../Enumeradores/EnumTipoInfracaoTransito.js" />
/// <reference path="DadosSinistro.js" />
/// <reference path="AnexoInfracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosInfracao;

/*
 * Declaração das Classes
 */

var DadosInfracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "*Cidade:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true, enable: ko.observable(false), visible: ko.observable(true) });
    this.Local = PropertyEntity({ text: "*Local:", getType: typesKnockout.string, val: ko.observable(""), required: ko.observable(true), maxlength: 100, enable: ko.observable(false), visible: ko.observable(true) });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", text: "Número: ", enable: false, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.NumeroAtuacao = PropertyEntity({ text: "Número da Autuação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 100, enable: ko.observable(false), visible: ko.observable(true) });    
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000, enable: ko.observable(false), visible: ko.observable(true) });    
    this.TipoInfracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true), adicionarRenavamVeiculoObservacao: false });
    this.TipoInfracaoTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Veículo:"), idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });

    this.DataEmissaoInfracao = PropertyEntity({ text: "Data de Emissão: ", getType: typesKnockout.dateTime, required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.DataLimiteIndicacaoCondutor = PropertyEntity({ text: "*Limite para indicação do condutor: ", getType: typesKnockout.dateTime, required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.OrgaoEmissor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Órgão Emissor:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.TipoOcorrenciaInfracao = PropertyEntity({ val: ko.observable(""), options: EnumTipoOcorrenciaInfracao.ObterOpcoes(), def: "", text: "*Tipo de Ocorrência: ", enable: ko.observable(false) });

    this.NaoGerarTituloFinanceiro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.GerarMovimentoFichaMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoMovimentoFichaMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.TipoMovimentoTituloEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false) });

    this.TipoOcorrenciaInfracao.val.subscribe(function () { ChangeTipoOcorrenciaInfracao(); });
    
    this.AtualizarDataAssinaturaMulta = PropertyEntity({ eventClick: atualizardataassinaturaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.DataAssinaturaMulta = PropertyEntity({ text: "*Data Assinatura Motorista: ", getType: typesKnockout.dateTime, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });    

    //Dados Sinistro
    this.DataSinistro = PropertyEntity({ text: "*Data do Sinistro: ", getType: typesKnockout.date, required: false, visible: ko.observable(true) });
    this.DataEmbarque = PropertyEntity({ text: "*Data do Embarque: ", getType: typesKnockout.date, required: false, visible: ko.observable(true) });
    this.NumeroNotaFiscal = PropertyEntity({ val: ko.observable(""), def: "", text: "Nº Nota Fiscal Embarcador: ", enable: false, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.CargaCte = PropertyEntity({ type: types.map, text: "CargaCte:", val: ko.observable("") });
    this.Emitente = PropertyEntity({ type: types.map, text: "Emitente:", val: ko.observable("") });
    this.Destinatario = PropertyEntity({ type: types.map, text: "Destinatario:", val: ko.observable("") });
    this.Segurado = PropertyEntity({ val: ko.observable(""), def: false, getType: typesKnockout.bool, text: "Estava segurado?" });
    this.LimpezaPista = PropertyEntity({ val: ko.observable(""), def: false, getType: typesKnockout.bool, text: "Ocorreu limpeza da pista?" });
    this.Seguradora = PropertyEntity({ type: types.map, text: "Seguradora:", val: ko.observable("") });
    this.Carga = PropertyEntity({ type: types.map, text: "Carga:", val: ko.observable("") });
    this.ProdutoCarga = PropertyEntity({ text: "*Produto Carga:", getType: typesKnockout.string, val: ko.observable(""), required: false, maxlength: 100, visible: ko.observable(true) });
    this.ValorNotaFiscal = PropertyEntity({ text: "Valor Nota Fiscal:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(true) });
    this.ValorEstimadoPrejuizo = PropertyEntity({ text: "Valor Estimado do Prejuizo:", getType: typesKnockout.decimal, val: ko.observable(""), visible: ko.observable(true) });
    this.ClassificacaoSinistro = PropertyEntity({ val: ko.observable(""), options: EnumClassificacaoSinistro.obterOpcoes(), text: "Classificação Sinistro: ", visible: ko.observable(true) });
    this.CausaSinistro = PropertyEntity({ text: "Causa do Sinistro:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, visible: ko.observable(true) });

};

var DadosTabs = function () {
    this.Sinistro = PropertyEntity({ type: types.event, visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ type: types.event, visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadDadosInfracao() {
    _dadosInfracao = new DadosInfracao();
    _dadosTabs = new DadosTabs();
    KoBindings(_dadosInfracao, "knockoutDadosInfracao");
    KoBindings(_dadosTabs, "knockoutTabsDados");

    new BuscarLocalidades(_dadosInfracao.Cidade);
    new BuscarTipoInfracao(_dadosInfracao.TipoInfracao, retornoTipoInfracao);
    //new BuscarVeiculos(_dadosInfracao.Veiculo, retornoVeiculo);
    new BuscarVeiculos(_dadosInfracao.Veiculo, retornoVeiculo, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarClientes(_dadosInfracao.OrgaoEmissor);
    new BuscarFuncionario(_dadosInfracao.Funcionario);
    new BuscarMotoristas(_dadosInfracao.Motorista, null, null, null, null, EnumSituacaoColaborador.Todos, true);

    loadDadosSinistro();
}

/*
 * Declaração das Funções
 */

function retornoVeiculo(data) {
    _dadosInfracao.Veiculo.codEntity(data.Codigo);
    _dadosInfracao.Veiculo.val(data.Descricao);

    if (_dadosInfracao.TipoInfracao.adicionarRenavamVeiculoObservacao) {
        var observacaoAtual = _dadosInfracao.Observacao.val();
        var observacao = "Renavam " + data.Renavam;

        if (string.IsNullOrWhiteSpace(observacaoAtual))
            _dadosInfracao.Observacao.val(observacao);
        else
            _dadosInfracao.Observacao.val(observacaoAtual + " - " + observacao);
    }

    if (_dadosInfracao.Data.val() != "" && _dadosInfracao.Data.val() != null && (_dadosInfracao.Motorista.codEntity() == 0 || _dadosInfracao.Motorista.codEntity == null)) {
        executarReST("Infracao/BuscarMotorista", { Veiculo: data.Codigo, Data: _dadosInfracao.Data.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _dadosInfracao.Motorista.codEntity(retorno.Data.Codigo);
                    _dadosInfracao.Motorista.val(retorno.Data.Nome);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
}

function retornoTipoInfracao(data) {
    if (data.Tipo == EnumTipoInfracaoTransito.Sinistro) {
        _dadosTabs.Anexos.visible(true);
        _dadosTabs.Sinistro.visible(true);
    } else {
        _dadosTabs.Anexos.visible(false);
        _dadosTabs.Sinistro.visible(false);
    }
    _CRUDInfracao.BuscarInfracoes.visible(data.PermitirReplicarInformacao);
    _dadosInfracao.TipoInfracao.codEntity(data.Codigo);
    _dadosInfracao.TipoInfracao.val(data.Descricao);
    _dadosInfracao.TipoInfracao.adicionarRenavamVeiculoObservacao = data.AdicionarRenavamVeiculoObservacao;

    _dadosInfracao.GerarMovimentoFichaMotorista.val(data.GerarMovimentoFichaMotorista);
    _dadosInfracao.NaoGerarTituloFinanceiro.val(data.NaoGerarTituloFinanceiro);

    if (data.CodigoTipoMovimentoTituloEmpresa != null && data.CodigoTipoMovimentoTituloEmpresa != undefined && data.CodigoTipoMovimentoTituloEmpresa > 0) {
        _dadosInfracao.TipoMovimentoTituloEmpresa.codEntity(data.CodigoTipoMovimentoTituloEmpresa);
        _dadosInfracao.TipoMovimentoTituloEmpresa.val(data.TipoMovimentoTituloEmpresa);
    }
    if (data.CodigoTipoMovimentoFichaMotorista != null && data.CodigoTipoMovimentoFichaMotorista != undefined && data.CodigoTipoMovimentoFichaMotorista > 0) {
        _dadosInfracao.TipoMovimentoFichaMotorista.codEntity(data.CodigoTipoMovimentoFichaMotorista);
        _dadosInfracao.TipoMovimentoFichaMotorista.val(data.TipoMovimentoFichaMotorista.Descricao);
    }
    controleCamposProcessamento();
    controleCampoEmpresaInfracao();

    _dadosInfracao.Cidade.required(!data.NaoObrigarInformarCidade);
    _dadosInfracao.Local.required(!data.NaoObrigarInformarLocal);
 
}

function ChangeTipoOcorrenciaInfracao() {
    var ocorrenciaVeiculo = _dadosInfracao.TipoOcorrenciaInfracao.val() === EnumTipoOcorrenciaInfracao.Veiculo;
    var ocorrenciaMotorista = _dadosInfracao.TipoOcorrenciaInfracao.val() === EnumTipoOcorrenciaInfracao.Motorista;
    var ocorrenciaFuncionario = _dadosInfracao.TipoOcorrenciaInfracao.val() === EnumTipoOcorrenciaInfracao.Funcionario;

    if (ocorrenciaMotorista)
        _dadosInfracao.Veiculo.text("Veículo:");
    else
        _dadosInfracao.Veiculo.text("*Veículo:");

    _dadosInfracao.NumeroAtuacao.visible(ocorrenciaFuncionario || ocorrenciaMotorista || ocorrenciaVeiculo);
    _dadosInfracao.DataEmissaoInfracao.visible(ocorrenciaFuncionario || ocorrenciaMotorista || ocorrenciaVeiculo);
    _dadosInfracao.DataLimiteIndicacaoCondutor.visible(ocorrenciaVeiculo);
    _dadosInfracao.OrgaoEmissor.visible(ocorrenciaFuncionario || ocorrenciaMotorista || ocorrenciaVeiculo);
    _dadosInfracao.Motorista.visible(ocorrenciaVeiculo || ocorrenciaMotorista);
    _dadosInfracao.Funcionario.visible(ocorrenciaFuncionario);
    _dadosInfracao.Veiculo.visible(ocorrenciaVeiculo || ocorrenciaMotorista);

    _dadosInfracao.Funcionario.required = ocorrenciaFuncionario;
    _dadosInfracao.Veiculo.required = ocorrenciaVeiculo;
    _dadosInfracao.Motorista.required = ocorrenciaVeiculo || ocorrenciaMotorista;


}

function adicionarDadosInfracao() {
    if (!ValidarCamposObrigatorios(_dadosInfracao)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja gerar a ocorrência?", function () {
        preecherDadosInfracao();
        executarReST("Infracao/AdicionarDados", RetornarObjetoPesquisa(_dadosInfracao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrência gerada com sucesso");

                    _gridInfracao.CarregarGrid();

                    enviarArquivosAnexados(retorno.Data.Codigo);

                    buscarInfracaoPorCodigo(retorno.Data.Codigo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function atualizarClick() {
    if (!ValidarCamposObrigatorios(_dadosInfracao)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja atualizar a ocorrência?", function () {
        preecherDadosInfracao();
        executarReST("Infracao/AtualizarDados", RetornarObjetoPesquisa(_dadosInfracao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrência atualizada com sucesso");

                    _gridInfracao.CarregarGrid();

                    limparCamposInfracao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function atualizardataassinaturaClick() {
    if (!ValidarCamposObrigatorios(_dadosInfracao)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja atualizar a data de assinatura da multa?", function () {        
        executarReST("Infracao/AtualizarDadosAssinaturaMulta", RetornarObjetoPesquisa(_dadosInfracao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Data assinatura multa atualizada com sucesso");

                    _gridInfracao.CarregarGrid();

                    limparCamposInfracao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function controlarCamposDadosInfracaoHabilitados() {
    var habilitarCampo = _infracao.Situacao.val() === EnumSituacaoInfracao.Todas || _infracao.Situacao.val() === EnumSituacaoInfracao.AguardandoProcessamento || _infracao.Situacao.val() === EnumSituacaoInfracao.AguardandoConfirmacao;

    _dadosInfracao.Cidade.enable(habilitarCampo);
    _dadosInfracao.Data.enable(habilitarCampo);
    _dadosInfracao.Local.enable(habilitarCampo);
    _dadosInfracao.NumeroAtuacao.enable(habilitarCampo);
    _dadosInfracao.Observacao.enable(habilitarCampo);
    _dadosInfracao.TipoInfracao.enable(habilitarCampo);
    _dadosInfracao.Veiculo.enable(habilitarCampo);
    _dadosInfracao.Funcionario.enable(habilitarCampo);
    _dadosInfracao.Motorista.enable(habilitarCampo);
    _dadosInfracao.OrgaoEmissor.enable(habilitarCampo);
    _dadosInfracao.TipoOcorrenciaInfracao.enable(habilitarCampo);
    _dadosInfracao.DataLimiteIndicacaoCondutor.enable(habilitarCampo);
    _dadosInfracao.DataEmissaoInfracao.enable(habilitarCampo);

    //sinistro
    _dadosSinistro.DataSinistro.enable(habilitarCampo);
    _dadosSinistro.DataEmbarque.enable(habilitarCampo);
    _dadosSinistro.NumeroNotaFiscal.enable(habilitarCampo);
    _dadosSinistro.Segurado.enable(habilitarCampo);
    _dadosSinistro.LimpezaPista.enable(habilitarCampo);
    _dadosSinistro.CargaCte.enable(habilitarCampo);
    _dadosSinistro.Emitente.enable(habilitarCampo);
    _dadosSinistro.Destinatario.enable(habilitarCampo);
    _dadosSinistro.Seguradora.enable(habilitarCampo);
    _dadosSinistro.Carga.enable(habilitarCampo);
    _dadosSinistro.ProdutoCarga.enable(habilitarCampo);
    _dadosSinistro.ValorNotaFiscal.enable(habilitarCampo);
    _dadosSinistro.ValorEstimadoPrejuizo.enable(habilitarCampo);
    _dadosSinistro.ClassificacaoSinistro.enable(habilitarCampo);
    _dadosSinistro.CausaSinistro.enable(habilitarCampo);

    //data assinatura motorista e observacao
    habilitarAssinatura = _infracao.Situacao.val() === EnumSituacaoInfracao.AguardandoProcessamento ||
                          _infracao.Situacao.val() === EnumSituacaoInfracao.AguardandoAprovacao ||
                          _infracao.Situacao.val() === EnumSituacaoInfracao.SemRegraAprovacao ||
                          _infracao.Situacao.val() === EnumSituacaoInfracao.Finalizada ||
                          _infracao.Situacao.val() === EnumSituacaoInfracao.AguardandoConfirmacao;

    _dadosInfracao.AtualizarDataAssinaturaMulta.visible(habilitarAssinatura);
    _dadosInfracao.DataAssinaturaMulta.enable(habilitarAssinatura);
    
    if (habilitarAssinatura) {
        _dadosInfracao.Observacao.enable(true);        
    } else {
        _dadosInfracao.Observacao.enable(habilitarCampo);
    }
    
}

function limparCamposDadosInfracao() {
    LimparCampos(_dadosInfracao);
    resetarTabs();
}

function preencherDadosInfracao(dadosInfracao) {
    PreencherObjetoKnout(_dadosInfracao, { Data: dadosInfracao });
    const campoRequerido = dadosInfracao.TipoInfracaoRequeridos;

    _dadosInfracao.TipoInfracao.adicionarRenavamVeiculoObservacao = dadosInfracao.TipoInfracao.AdicionarRenavamVeiculoObservacao;

    _dadosInfracao.TipoInfracaoTipo.val(dadosInfracao.TipoInfracaoTipo);
    if (_dadosInfracao.TipoInfracaoTipo.val() == EnumTipoInfracaoTransito.Sinistro) {
        _dadosTabs.Anexos.visible(true);
        _dadosTabs.Sinistro.visible(true);
    }

    controleCamposProcessamento();
    controleCampoEmpresaInfracao();

    _dadosInfracao.Cidade.required(!campoRequerido.NaoObrigarInformarCidade);
    _dadosInfracao.Local.required(!campoRequerido.NaoObrigarInformarLocal);
}

function preencherDadosSinistro(dadosSinistro) {
    PreencherObjetoKnout(_dadosSinistro, { Data: dadosSinistro });
}

function resetarTabs() {
    Global.ResetarAbas();
    _dadosTabs.Anexos.visible(false);
    _dadosTabs.Sinistro.visible(false);
}