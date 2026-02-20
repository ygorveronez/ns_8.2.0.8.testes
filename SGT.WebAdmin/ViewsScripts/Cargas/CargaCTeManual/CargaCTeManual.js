/// <reference path="./../../Enumeradores/EnumTipoOperacaoCargaCTeManual.js" />
/// <reference path="./../../Enumeradores/EnumTipoNaoEnviarParaMercante.js" />
/// <reference path="./../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../CTe/CTe/CTe.js" />
/// <reference path="ImportarXMLCTe.js" />
/// <reference path="Integracoes.js" />
/// <reference path="Anulacao.js" />
/// <reference path="AnulacaoGerencial.js" />
/// <reference path="EtapasCargaCTeManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaCarga, _gridCargas, _carga, _cargaCTe, _gridCargaCTe, _codigosVeiculosCarga, _codigosMotoristasCarga, _confirmacaoDocumentosCarga, _PermissoesPersonalizadas, _naoEnviarParaMercante;
var _integracaoCTeManual = false;

var _situacaoTipoOperacao = [
    { value: EnumTipoOperacaoCargaCTeManual.NovaCarga, text: "Nova Carga" },
    { value: EnumTipoOperacaoCargaCTeManual.ManutencaoCarga, text: "Manutenção de Carga" }
];

var PesquisaCarga = function () {
    this.Codigo = PropertyEntity({ text: "Número da Carga:", visible: ko.observable(false) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido no Embarcador:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: "Número do CT-e:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int });
    this.NumeroBooking = PropertyEntity({ text: "Booking:", val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.NumeroOS = PropertyEntity({ text: "Número da OS:", val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.NumeroControle = PropertyEntity({ text: "Número de Controle:", val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), issue: 69, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", issue: 145, idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: false });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular de Carga:", issue: 44, required: true, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", issue: 53, required: true, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Clientes:", issue: 58, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.PermiteCTeManual), def: EnumSituacoesCarga.PermiteCTeManual, text: "*Situação da Carga: ", required: true });
    this.TipoOperacaoCargaCTeManual = PropertyEntity({ text: "*Tipo de Operação:", options: _situacaoTipoOperacao, val: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? EnumTipoOperacaoCargaCTeManual.ManutencaoCarga : EnumTipoOperacaoCargaCTeManual.NovaCarga), def: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? EnumTipoOperacaoCargaCTeManual.ManutencaoCarga : EnumTipoOperacaoCargaCTeManual.NovaCarga, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.SomenteTerceiros = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Somente Terceiros?", visible: ko.observable(false) });
    this.ExibirCargasNaoFechadas = PropertyEntity({ type: types.bool, val: ko.observable(true), def: true });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        this.Filial.visible(false);
        this.GrupoPessoa.visible(true);
        this.Empresa.text("Empresa/Filial:");
    }

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargas.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.TipoOperacaoCargaCTeManual.val.subscribe(function (novoValor) {
        _gridCargas.CarregarGrid();
    });
};

var CancelamentoCTe = function () {
    this.Observacao = PropertyEntity({ type: types.map, text: "*Justificativa: ", required: true });
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.SituacaoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.ConfirmarCancelamento = PropertyEntity({ eventClick: confirmarConfirmarCancelamentoClick, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });

    this.Observacao.val.subscribe(function (novoValor) {
        const regex = new RegExp(_CONFIGURACAO_TMS.ReplaceMotivoRegexPattern, "gi");

        // Remove os caracteres não permitidos
        this.Observacao.val(novoValor.replace(regex, ""));
    }.bind(this));
};

var NaoEnviarParaMercante = function () {
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.JustificativaMercante = PropertyEntity({ text: "*Justificativa Mercante:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });

    this.ConfirmarNaoEnviarParaMercante = PropertyEntity({ eventClick: confirmarNaoEnviarParaMercanteClick, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
};

var ConfirmacaoDocumentosCarga = function () {
    this.NaoGerarMDFe = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true, text: "Gerar o MDF-e desta carga manualmente?" });

    this.Confirmar = PropertyEntity({ eventClick: confirmarCTesClick, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: FecharTelaConfirmacaoDocumentosCarga, type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

var Carga = function () {
    this.Descricao = PropertyEntity({ type: types.map });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("Transportador:"), idBtnSearch: guid(), enable: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Motorista:", issue: 145, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Tipo de Carga:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Centro de Resultado:", visible: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Modelo Veicular de Carga:", issue: 44, idBtnSearch: guid() });
    this.Peso = PropertyEntity({ type: types.map, codEntity: ko.observable(""), required: false, text: "Peso:", idGrid: guid() });
    this.SituacaoCarga = PropertyEntity({ type: types.map, required: false, idBtnSearch: guid() });

    //Botões
    this.BuscarNovamenteCTe = PropertyEntity({ eventClick: buscarNovamenteCTeClick, type: types.event, text: "Atualizar / Buscar CT-e", visible: ko.observable(true) });
    this.ImportarEmbarcador = PropertyEntity({ eventClick: ImportarEmbarcadorClick, type: types.event, text: "Vincular CT-e Embarcador", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CTeManual_VincularCTeEmbarcador, _PermissoesPersonalizadas)) });
    this.InserirNovoCTe = PropertyEntity({ eventClick: adicionarNovoCTeClick, type: types.event, text: "Novo CT-e", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Importar = PropertyEntity({ type: types.event, text: "Importar", icon: "fal fa-upload", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.ImportarNFeSiteSEFAZ = PropertyEntity({ eventClick: ImportarNFeSiteSEFAZClick, type: types.event, text: "Importar de Chave", visible: ko.observable(false) });
    this.ArquivoNFe = PropertyEntity({ type: types.file, eventChange: function () { }, codEntity: ko.observable(0), text: "XML de NF-e", icon: "fal fa-file-code", accept: ".xml", val: ko.observable(""), visible: ko.observable(true) });
    this.ArquivoCTe = PropertyEntity({ type: types.file, eventChange: function () { }, codEntity: ko.observable(0), text: "XML de CT-e", icon: "fal fa-file-code", accept: ".xml", val: ko.observable(""), visible: ko.observable(true) });

    this.CTe = PropertyEntity({ type: types.local, idGrid: guid() });

    this.ConfirmarCTe = PropertyEntity({ eventClick: AbrirTelaConfirmacaoDocumentosCarga, type: types.event, text: "Confirmar CT-e(s)", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        this.Empresa.text("Empresa/Filial:");
    }
};

var CargaCTe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.Status = PropertyEntity();
};

//*******EVENTOS*******

function adicionarNovoCTeClick(e, sender) {
    abrirModalCTe(0);
}

function editarCTeClick(e, sender) {
    abrirModalCTe(parseInt(e.CodigoCTE));
}

function duplicarCTeClick(e, sender) {
    abrirModalCTe(parseInt(e.CodigoCTE), null, true);
}

function naoEnviarMercanteClick(e, sender) {
    _naoEnviarParaMercante = new NaoEnviarParaMercante();
    _naoEnviarParaMercante.CodigoCTe.val(e.CodigoCTE);
    KoBindings(_naoEnviarParaMercante, "knoutNaoEnviarParaMercante");

    BuscarJustificativaMercante(_naoEnviarParaMercante.JustificativaMercante);

    Global.abrirModal('divModalNaoEnviarParaMercantea');
}

function abrirModalCTe(codigoCTe, documentos, duplicar, ctesParaSubcontratacao) {
    const permissoesTotal = [EnumPermissoesEdicaoCTe.total];

    const instanciaEmissao = new EmissaoCTe(codigoCTe, function () {

        instanciaEmissao.CRUDCTe.Emitir.visible(true);
        instanciaEmissao.CRUDCTe.Salvar.visible(false);

        instanciaEmissao.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instanciaEmissao);
        };

        instanciaEmissao.CRUDCTe.Emitir.eventClick = function () {
            if (instanciaEmissao.Validar() === true) {
                var objetoCTe = ObterObjetoCTe(instanciaEmissao);

                if (verificarPermissaoEmissaoCTeZerado(instanciaEmissao))
                    EmitirCTe(objetoCTe, instanciaEmissao);
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Não possui permissão para gerar um documento com valor a receber zerado");
            }
        };

        instanciaEmissao.TotalServico.IncluirICMSFrete.val(true);
        instanciaEmissao.Componente.IncluirBaseCalculoICMS.def = true;
        instanciaEmissao.Componente.IncluirBaseCalculoICMS.val(true);

        if (_carga.Empresa.codEntity() > 0)
            instanciaEmissao.ObterInformacoesEmpresa(_carga.Empresa.codEntity());

        instanciaEmissao.ObterInformacoesModalRodoviario(_codigosVeiculosCarga, _codigosMotoristasCarga);

        if (_carga.Veiculo.codEntity() > 0) {
            instanciaEmissao.Veiculo.Veiculo.codEntity(_carga.Veiculo.codEntity());
            instanciaEmissao.Veiculo.Veiculo.val(_carga.Veiculo.val());
            instanciaEmissao.Veiculo.AdicionarVeiculo();
        }

        if (documentos != null) {

            let peso = 0;

            for (let i = 0; i < documentos.length; i++) {
                const doc = documentos[i];

                instanciaEmissao.Documento.TipoDocumento.val(EnumTipoDocumentoCTe.NFeNotaFiscalEletronica);
                instanciaEmissao.Documento.Numero.val(doc.Numero);
                instanciaEmissao.Documento.Chave.val(doc.Chave);
                instanciaEmissao.Documento.DataEmissao.val(doc.DataEmissao);
                instanciaEmissao.Documento.ValorNotaFiscal.val(Globalize.format(doc.ValorTotal, "n2"));
                instanciaEmissao.Documento.Peso.val(Globalize.format(doc.Peso, "n2"));
                instanciaEmissao.Documento.CFOP.val(doc.CFOP);
                instanciaEmissao.Documento.NCMPredominante.val(doc.NCMPredominante);

                instanciaEmissao.Documento.AdicionarDocumento();

                peso += doc.Peso;
            }

            const documento = documentos[0];

            if (documento.Remetente.CPFCNPJ != null && documento.Remetente.CPFCNPJ != "")
                instanciaEmissao.Remetente.CPFCNPJ.val(documento.Remetente.CPFCNPJ);
            else
                instanciaEmissao.Remetente.CPFCNPJ.val(documento.Remetente);

            instanciaEmissao.Remetente.BuscarDadosPorCPFCNPJ();

            if (documento.Destinatario !== null) {
                if (documento.Destinatario.CPFCNPJ != null && documento.Destinatario.CPFCNPJ != "")
                    instanciaEmissao.Destinatario.CPFCNPJ.val(documento.Destinatario.CPFCNPJ);
                else
                    instanciaEmissao.Destinatario.CPFCNPJ.val(documento.Destinatario);
                instanciaEmissao.Destinatario.BuscarDadosPorCPFCNPJ();
            } else if (documento.DestinatarioExportacao != null) {
                instanciaEmissao.Destinatario.ParticipanteExterior.val(true);
                instanciaEmissao.Destinatario.RazaoSocial.val(documento.DestinatarioExportacao.RazaoSocial);
                instanciaEmissao.Destinatario.Endereco.val(documento.DestinatarioExportacao.Endereco);
                instanciaEmissao.Destinatario.Numero.val(documento.DestinatarioExportacao.Numero);
                instanciaEmissao.Destinatario.Bairro.val(documento.DestinatarioExportacao.Bairro);
                instanciaEmissao.Destinatario.Complemento.val(documento.DestinatarioExportacao.Complemento);
                instanciaEmissao.Destinatario.Pais.val(documento.DestinatarioExportacao.DescricaoPais);
                instanciaEmissao.Destinatario.Pais.codEntity(documento.DestinatarioExportacao.CodigoPais);
                instanciaEmissao.Destinatario.EmailGeral.val(documento.DestinatarioExportacao.Emails);
                instanciaEmissao.Destinatario.LocalidadeExterior.val(documento.DestinatarioExportacao.Cidade);
            }

            if (documento.QuantidadesCarga != null) {
                for (let a = 0; a < documento.QuantidadesCarga.length; a++) {
                    const quantidadeCarga = documento.QuantidadesCarga[a];

                    instanciaEmissao.QuantidadeCarga.Quantidade.val(Globalize.format(quantidadeCarga.Quantidade, "n4"));
                    instanciaEmissao.QuantidadeCarga.UnidadeMedida.val(quantidadeCarga.UnidadeMedida.Descricao);
                    instanciaEmissao.QuantidadeCarga.UnidadeMedida.codEntity(quantidadeCarga.UnidadeMedida.Codigo);
                    instanciaEmissao.QuantidadeCarga.AdicionarQuantidadeCarga();
                }
            }

            if (documento.FormaPagamento == 0)
                instanciaEmissao.CTe.TipoPagamento.val(EnumTipoPagamento.Pago);
            else if (documento.FormaPagamento == 1)
                instanciaEmissao.CTe.TipoPagamento.val(EnumTipoPagamento.A_Pagar);
            else
                instanciaEmissao.CTe.TipoPagamento.val(EnumTipoPagamento.Outros);
        }
        else if (ctesParaSubcontratacao != null) {
            SetarInformacoesCTesParaSubcontratacao(instanciaEmissao, ctesParaSubcontratacao);
        }

        const dadosCarga = { Carga: _carga.Codigo.val() };
        executarReST("CargaCTeManual/RetornarDadosPadraoCarga", dadosCarga, function (arg) {
            if (arg.Success) {
                if (arg.Data != false && (instanciaEmissao.CTe.Tipo === EnumTipoCTe.Normal || codigoCTe === 0)) {
                    instanciaEmissao.CTe.Empresa.codEntity(arg.Data.CodigoEmpresa);
                    instanciaEmissao.CTe.Empresa.val(arg.Data.DescricaoEmpresa);

                    if (_CONFIGURACAO_TMS.NaoPreencherSerieCTeManual === false && _CONFIGURACAO_TMS.NaoUtilizarSerieCargaCTeManual === false) {
                        instanciaEmissao.CTe.Serie.codEntity(arg.Data.CodigoSerie);
                        instanciaEmissao.CTe.Serie.val(arg.Data.DescricaoSerie);
                    }

                    instanciaEmissao.CTe.TipoTomador.val(arg.Data.TipoTomador);

                    instanciaEmissao.CTe.LocalidadeEmissao.codEntity(arg.Data.CodigoLocalidadeEmissao);
                    instanciaEmissao.CTe.LocalidadeEmissao.val(arg.Data.DescricaoLocalidadeEmissao);

                    instanciaEmissao.CTe.TipoModal.val(arg.Data.TipoModal);

                    instanciaEmissao.CTe.CFOP.codEntity(arg.Data.CodigoCFOP);
                    instanciaEmissao.CTe.CFOP.val(arg.Data.DescricaoCFOP);

                    instanciaEmissao.InformacaoModal.PortoOrigem.codEntity(arg.Data.CodigoPortoOrigem);
                    instanciaEmissao.InformacaoModal.PortoOrigem.val(arg.Data.DescricaoPortoOrigem);

                    instanciaEmissao.InformacaoModal.PortoPassagemUm.codEntity(arg.Data.CodigoPortoPassagemUm);
                    instanciaEmissao.InformacaoModal.PortoPassagemUm.val(arg.Data.DescricaoPortoPassagemUm);

                    instanciaEmissao.InformacaoModal.PortoPassagemDois.codEntity(arg.Data.CodigoPortoPassagemDois);
                    instanciaEmissao.InformacaoModal.PortoPassagemDois.val(arg.Data.DescricaoPortoPassagemDois);

                    instanciaEmissao.InformacaoModal.PortoPassagemTres.codEntity(arg.Data.CodigoPortoPassagemTres);
                    instanciaEmissao.InformacaoModal.PortoPassagemTres.val(arg.Data.DescricaoPortoPassagemTres);

                    instanciaEmissao.InformacaoModal.PortoPassagemQuatro.codEntity(arg.Data.CodigoPortoPassagemQuatro);
                    instanciaEmissao.InformacaoModal.PortoPassagemQuatro.val(arg.Data.DescricaoPortoPassagemQuatro);

                    instanciaEmissao.InformacaoModal.PortoPassagemCinco.codEntity(arg.Data.CodigoPortoPassagemCinco);
                    instanciaEmissao.InformacaoModal.PortoPassagemCinco.val(arg.Data.DescricaoPortoPassagemCinco);

                    instanciaEmissao.InformacaoModal.PortoDestino.codEntity(arg.Data.CodigoPortoDestino);
                    instanciaEmissao.InformacaoModal.PortoDestino.val(arg.Data.DescricaoPortoDestino);

                    instanciaEmissao.InformacaoModal.TerminalOrigem.codEntity(arg.Data.CodigoTerminalOrigem);
                    instanciaEmissao.InformacaoModal.TerminalOrigem.val(arg.Data.DescricaoTerminalOrigem);

                    instanciaEmissao.InformacaoModal.TerminalDestino.codEntity(arg.Data.CodigoTerminalDestino);
                    instanciaEmissao.InformacaoModal.TerminalDestino.val(arg.Data.DescricaoTerminalDestino);

                    instanciaEmissao.InformacaoModal.Viagem.codEntity(arg.Data.CodigoViagem);
                    instanciaEmissao.InformacaoModal.Viagem.val(arg.Data.DescricaoViagem);

                    instanciaEmissao.InformacaoModal.NumeroBooking.val(arg.Data.NumeroBooking);
                    instanciaEmissao.InformacaoModal.DescricaoCarrier.val(arg.Data.DescricaoCarrier);
                    instanciaEmissao.InformacaoModal.TipoPropostaFeeder.val(arg.Data.TipoPropostaFeeder);

                    instanciaEmissao.TotalServico.ICMS.val(arg.Data.ICMS);
                    instanciaEmissao.TotalServico.AliquotaICMS.val(arg.Data.AliquotaICMS);

                    instanciaEmissao.InformacaoCarga.ProdutoPredominante.val(arg.Data.ProdutoPredominante);

                    instanciaEmissao.TotalServico.IncluirICMSFrete.val(arg.Data.IncluirICMSFrete);
                    instanciaEmissao.TotalServico.PercentualICMSIncluir.val(arg.Data.PercentualICMSIncluir);

                    if (arg.Data.Seguro != null && arg.Data.Seguro.length > 0) {
                        for (let a = 0; a < arg.Data.Seguro.length; a++) {
                            const seguro = arg.Data.Seguro[a];

                            instanciaEmissao.Seguro.Responsavel.val(seguro.CodigoResponsavel);
                            instanciaEmissao.Seguro.CNPJSeguradora.val(seguro.CNPJSeguradora);
                            instanciaEmissao.Seguro.ConsultarCNPJSeguradora.codEntity(seguro.CNPJSeguradora);
                            instanciaEmissao.Seguro.Seguradora.val(seguro.Seguradora);
                            instanciaEmissao.Seguro.NumeroApolice.val(seguro.NumeroApolice);
                            instanciaEmissao.Seguro.NumeroAverbacao.val(seguro.NumeroAverbacao);
                            instanciaEmissao.Seguro.Valor.val(seguro.Valor);

                            instanciaEmissao.Seguro.AdicionarSeguro();
                        }
                    }

                    if (arg.Data.Recebedor != null && arg.Data.Recebedor != undefined)
                        PreencherObjetoKnout(instanciaEmissao.Recebedor, { Data: arg.Data.Recebedor });

                    if (arg.Data.Expedidor != null && arg.Data.Expedidor != undefined)
                        PreencherObjetoKnout(instanciaEmissao.Expedidor, { Data: arg.Data.Expedidor });

                    if (arg.Data.Tomador != null && arg.Data.Tomador != undefined)
                        PreencherObjetoKnout(instanciaEmissao.Tomador, { Data: arg.Data.Tomador });

                    let cnpjTomador = 0;
                    if (instanciaEmissao.CTe.TipoTomador.val() == EnumTipoTomador.Destinatario)
                        cnpjTomador = instanciaEmissao.Destinatario.CPFCNPJ.val();
                    else if (instanciaEmissao.CTe.TipoTomador.val() == EnumTipoTomador.Expedidor)
                        cnpjTomador = instanciaEmissao.Expedidor.CPFCNPJ.val();
                    else if (instanciaEmissao.CTe.TipoTomador.val() == EnumTipoTomador.Recebedor)
                        cnpjTomador = instanciaEmissao.Recebedor.CPFCNPJ.val();
                    else if (instanciaEmissao.CTe.TipoTomador.val() == EnumTipoTomador.Remetente)
                        cnpjTomador = instanciaEmissao.Remetente.CPFCNPJ.val();
                    else if (instanciaEmissao.CTe.TipoTomador.val() == EnumTipoTomador.Outros)
                        cnpjTomador = instanciaEmissao.Tomador.CPFCNPJ.val();

                    if (arg.Data.RNTRC != null && arg.Data.RNTRC != "") {
                        instanciaEmissao.Rodoviario.RNTRC.val(arg.Data.RNTRC)
                    }

                    //Desabilitado por solicitação do Kelvin em 27/01 chamado 557
                    //var dadosCarga = { Carga: _carga.Codigo.val(), Empresa: instanciaEmissao.CTe.Empresa.codEntity(), Emitente: instanciaEmissao.Remetente.CPFCNPJ.val(), Destinatario: instanciaEmissao.Destinatario.CPFCNPJ.val(), Tomador: cnpjTomador, ValorFrete: instanciaEmissao.TotalServico.ValorFrete.val() };
                    //executarReST("CargaCTeManual/BuscarRegraICMSCTeManual", dadosCarga, function (arg) {
                    //    if (arg.Success) {
                    //        if (arg.Data != false) {
                    //            instanciaEmissao.TotalServico.ICMS.val(arg.Data.CST);
                    //            instanciaEmissao.TotalServico.BaseCalculoICMS.val(arg.Data.ValorBaseCalculoICMS);
                    //            instanciaEmissao.TotalServico.AliquotaICMS.val(arg.Data.Aliquota);
                    //            instanciaEmissao.TotalServico.ValorICMS.val(arg.Data.ValorICMS);
                    //            instanciaEmissao.TotalServico.PercentualReducaoBaseCalculoICMS.val(arg.Data.PercentualReducaoBC);
                    //            instanciaEmissao.TotalServico.ExibirNaDACTE.val(arg.Data.NaoImprimirImpostosDACTE);

                    //            instanciaEmissao.CTe.CFOP.codEntity(arg.Data.CFOP);
                    //            instanciaEmissao.CTe.CFOP.val(arg.Data.DescricaoCFOP);

                    //            var strObservacaoAnterior = instanciaEmissao.ObservacaoGeral.ObservacaoGeral.val();
                    //            if (arg.Data.ObservacaoCTe != undefined && arg.Data.ObservacaoCTe != null)
                    //                strObservacaoAnterior = strObservacaoAnterior + " " + arg.Data.ObservacaoCTe;
                    //            instanciaEmissao.ObservacaoGeral.ObservacaoGeral.val(strObservacaoAnterior);
                    //        }
                    //    } else {
                    //        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    //    }
                    //});
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
        //Preencher dados padrões
    }, permissoesTotal, null, duplicar);
}

function CarregarRegraICMSCTeManual(codigoCarga, codigoEmpresa, emitente, destinatario, tomador, valorFrete) {
    const dadosCarga = { Carga: codigoCarga, Empresa: codigoEmpresa, Emitente: emitente, Destinatario: destinatario, Tomador: tomador, ValorFrete: valorFrete };
    executarReST("CargaCTeManual/BuscarRegraICMSCTeManual", dadosCarga, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                //instanciaEmissao.TotalServico.IncluirICMSFrete.val(arg.Data.IncluirICMSBC);
                //instanciaEmissao.TotalServico.PercentualICMSIncluir.val(arg.Data.PercentualInclusaoBC);
                instanciaEmissao.TotalServico.ICMS.val(arg.Data.CST);
                instanciaEmissao.TotalServico.BaseCalculoICMS.val(arg.Data.ValorBaseCalculoICMS);
                instanciaEmissao.TotalServico.AliquotaICMS.val(arg.Data.Aliquota);
                instanciaEmissao.TotalServico.ValorICMS.val(arg.Data.ValorICMS);
                instanciaEmissao.TotalServico.PercentualReducaoBaseCalculoICMS.val(arg.Data.PercentualReducaoBC);
                instanciaEmissao.TotalServico.ExibirNaDACTE.val(arg.Data.NaoImprimirImpostosDACTE);

                instanciaEmissao.CTe.CFOP.codEntity(arg.Data.CFOP);
                instanciaEmissao.CTe.CFOP.val(arg.Data.DescricaoCFOP);

                let strObservacaoAnterior = instanciaEmissao.ObservacaoGeral.ObservacaoGeral.val();
                if (arg.Data.ObservacaoCTe != undefined && arg.Data.ObservacaoCTe != null)
                    strObservacaoAnterior = strObservacaoAnterior + " " + arg.Data.ObservacaoCTe;
                instanciaEmissao.ObservacaoGeral.ObservacaoGeral.val(strObservacaoAnterior);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function confirmarCTesClick(e, sender) {
    const dados = { Carga: _cargaCTe.Carga.val(), NaoGerarMDFe: _confirmacaoDocumentosCarga.NaoGerarMDFe.val() };
    exibirConfirmacao("Confirmação", "Você realmente confirmar esses CT-es para a carga?", function () {
        executarReST("CargaCTeManual/ConfirmarEnvioCTes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    FecharTelaConfirmacaoDocumentosCarga();
                    LimparCampos(_pesquisaCarga);
                    LimparCamposCTeManual();
                    buscarCargas();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-es Confirmados com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    });
}

function EmitirCTe(cte, instancia) {
    const dados = { CTe: cte, Carga: _cargaCTe.Carga.val() };
    executarReST("CargaCTeManual/EmitirCTeManualmente", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                instancia.FecharModal();
                _gridCargaCTe.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e emitido com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function emitirCTeClick(e) {
    if (e.SituacaoCTe == EnumStatusCTe.REJEICAO) {
        const data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/EmitirNovamente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCargaCTe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite emissão", "A atual situação do ct-e (" + e.Status + ")  não permite que ele seja emitido novamente");
    }
}

function inutilizarCTeClick(e, knout) {
    if (e.SituacaoCTe == EnumStatusCTe.REJEICAO) {
        const knoutJustificativa = new CancelamentoCTe();
        knoutJustificativa.CodigoCTe.val(e.CodigoCTE);
        knoutJustificativa.SituacaoCTe.val(e.SituacaoCTe);
        KoBindings(knoutJustificativa, "knoutSolicitarCancelamentoCarga");
        Global.abrirModal('divModalSolicitarCancelamentoCarga');
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite a inutilização", "A atual situação do ct-e (" + e.Status + ")  não permite que ele seja inutilizado");
    }
}

function cancelarCTeClick(e, knout) {
    if (e.SituacaoCTe == EnumStatusCTe.AUTORIZADO) {
        const knoutJustificativa = new CancelamentoCTe();
        knoutJustificativa.CodigoCTe.val(e.CodigoCTE);
        knoutJustificativa.SituacaoCTe.val(e.SituacaoCTe);
        KoBindings(knoutJustificativa, "knoutSolicitarCancelamentoCarga");
        Global.abrirModal('divModalSolicitarCancelamentoCarga');
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite o cancelamento", "A atual situação do ct-e (" + e.Status + ")  não permite que ele seja cancelado");
    }
}

function confirmarNaoEnviarParaMercanteClick(e, sender) {
    if (ValidarCamposObrigatorios(e)) {
        const url = "CargaCTe/NaoEnviarParaMercante";
        const data = { CodigoCTe: e.CodigoCTe.val(), JustificativaMercante: e.JustificativaMercante.codEntity() };
        executarReST(url, data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCargaCTe.CarregarGrid();
                    Global.fecharModal('divModalNaoEnviarParaMercantea');
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alteração realizada com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "É obrigatório informar uma justificativa.");
    }
}

function confirmarConfirmarCancelamentoClick(e, sender) {

    if (!ValidarCamposObrigatorios(e))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "É obrigatório informar uma justificativa.");

    if (e.Observacao.val().length < 20)
        return exibirMensagem(tipoMensagem.atencao, "Dados Obrigatórios", "É obrigatório que a justificativa tenha ao menos 20 caracteres.");

    const url = e.SituacaoCTe.val() == EnumStatusCTe.REJEICAO ? "CargaCTe/InutilizarCTe" : "CargaCTe/CancelarCTe";
    const data = { CodigoCTe: e.CodigoCTe.val(), Justificativa: e.Observacao.val(), CodigoCarga: _carga.Codigo.val() };
    executarReST(url, data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaCTe.CarregarGrid();
                Global.fecharModal('divModalSolicitarCancelamentoCarga');
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de cancelamento enviada com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function loadCargaCTeManual() {

    _pesquisaCarga = new PesquisaCarga();
    _carga = new Carga();
    _confirmacaoDocumentosCarga = new ConfirmacaoDocumentosCarga();

    KoBindings(_pesquisaCarga, "knockoutPesquisaCargaCTeManual", false, _pesquisaCarga.Pesquisar.id);
    KoBindings(_carga, "knockoutCadastroCargaCTeManual");
    KoBindings(_confirmacaoDocumentosCarga, "divModalConfirmacaoDocumentosCarga");

    BuscarTransportadores(_pesquisaCarga.Empresa, null, null, true);
    BuscarVeiculos(_pesquisaCarga.Veiculo);
    BuscarMotoristas(_pesquisaCarga.Motorista);
    BuscarTiposdeCarga(_pesquisaCarga.TipoCarga);
    BuscarModelosVeicularesCarga(_pesquisaCarga.ModeloVeicularCarga);
    BuscarFilial(_pesquisaCarga.Filial);
    BuscarGruposPessoas(_pesquisaCarga.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    BuscarClientes(_pesquisaCarga.Remetente);
    BuscarClientes(_pesquisaCarga.Destinatario);
    BuscarOperador(_pesquisaCarga.Operador);
    BuscarLocalidades(_pesquisaCarga.Origem, "Buscar Cidade de Origem", "Cidades de Origem");
    BuscarLocalidades(_pesquisaCarga.Destino, "Buscar Cidade de Destino", "Cidades de Destino");

    _carga.ArquivoNFe.file = document.getElementById(_carga.ArquivoNFe.idFile);
    _carga.ArquivoCTe.file = document.getElementById(_carga.ArquivoCTe.idFile);

    buscarCargas();
    SetarCamposMultiModal();

    LoadImportarXMLNFe();
    LoadImportarXMLCTe();
    LoadImportarNFeSiteSEFAZ();
    LoadAnulacaoCTe();
    LoadAnulacaoGerencialCTe();
    loadCTeManualIntegracoes();
    loadEtapaCargaCTeManual();
    configurarIntegracaoCTeManual();
    LoadImportarCteEmbarcador();
}

function buscarNovamenteCTeClick(e, sender) {
    _gridCargaCTe.CarregarGrid();
}

function cancelarClick(e, sender) {
    LimparCampos(_pesquisaCarga);
    LimparCamposCTeManual();
    buscarCargas();
}

function detalhesCTeClick(e, sender) {
    const codigo = parseInt(e.CodigoCTE);
    const permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    const instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            const objetoCTe = ObterObjetoCTe(instancia);
            SalvarCTe(objetoCTe, e.Codigo, instancia);
        };
    }, permissoesSomenteLeituraCTe);
}

function selecionarCargaClick(e) {
    Global.ResetarAbas();
    _carga.Codigo.val(e.Codigo);

    const data = { Carga: e.Codigo };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            let placasComModelo;

            _codigosVeiculosCarga = new Array();
            _codigosMotoristasCarga = new Array();

            if (_pesquisaCarga.TipoOperacaoCargaCTeManual.val() == EnumTipoOperacaoCargaCTeManual.NovaCarga) {
                _carga.ConfirmarCTe.enable(true);
                _carga.ConfirmarCTe.visible(true);
            } else {
                _carga.ConfirmarCTe.enable(false);
                _carga.ConfirmarCTe.visible(false);
            }

            const carga = arg.Data;
            _carga.Descricao.val("Carga " + carga.CodigoCargaEmbarcador + " : " + carga.OrigemDestinos);

            _carga.Empresa.val(carga.Empresa != null ? carga.Empresa.Descricao : "");
            _carga.Empresa.codEntity(carga.Empresa != null ? carga.Empresa.Codigo : 0);

            let infoMotoristas = "";
            $.each(carga.Motoristas, function (i, motorista) {
                _codigosMotoristasCarga.push(motorista.Codigo);

                if (i > 0)
                    infoMotoristas += ", ";

                infoMotoristas += motorista.Descricao;
            });
            _carga.Motorista.val(infoMotoristas);

            if (carga.Veiculo != null) {
                _codigosVeiculosCarga.push(carga.Veiculo.Codigo);

                let placas = carga.Veiculo.Descricao;
                placasComModelo = carga.Veiculo.Descricao;

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                    if (carga.Veiculo.ModeloVeicularCarga != "") {
                        placasComModelo += " (" + carga.Veiculo.ModeloVeicularCarga + ")";
                    }
                }

                if (carga.Veiculo.VeiculosVinculados != null) {
                    $.each(carga.Veiculo.VeiculosVinculados, function (i, veiculoVinculado) {
                        _codigosVeiculosCarga.push(veiculoVinculado.Codigo);

                        placas += ", " + veiculoVinculado.Descricao;
                        placasComModelo += ", " + veiculoVinculado.Descricao;

                        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                            if (veiculoVinculado.ModeloVeicularCarga != "") {
                                placas += " (" + veiculoVinculado.ModeloVeicularCarga + ")";
                                placasComModelo += " (" + veiculoVinculado.ModeloVeicularCarga + ")";
                            }
                        }
                    });
                }
            }

            _carga.Peso.val(Globalize.format(carga.PesoTotal, "n2"));
            _carga.Veiculo.val(placasComModelo);
            _carga.TipoCarga.val(carga.TipoCarga != null ? carga.TipoCarga.Descricao : "");
            _carga.ModeloVeicularCarga.val(carga.ModeloVeicularCarga != null ? carga.ModeloVeicularCarga.Descricao : "");
            _carga.CentroResultado.val(carga.CentroDeResultado != null ? carga.CentroDeResultado.Descricao : "");
            _carga.CentroResultado.codEntity(carga.CentroDeResultado != null ? carga.CentroDeResultado.Codigo : "");
            _carga.SituacaoCarga.val(carga.DescricaoSituacaoCarga);
            buscarCTEs();
            recarregarCTeManualIntegracoes();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

    executarReST("CargaCTeManual/ObterConfiguracaoGeralIntegracao", data, function (arg) {
        if (arg.Success) {
            if ((arg.Data.UsaFluxoSubstituicaoFaseada && arg.Data.IntegrarCTeSubstituto) || arg.Data.PossuiConfiguracaoIntegracao) {
                _etapaCargaCTeManual.ExibirEtapas.visible(true);

                executarReST("CargaCTeManualIntegracao/ObterStatusEtapas", data, function (arg) {
                    if (arg.Success) {
                        if (arg.Data.SituacaoIntegracao == EnumSituacaoIntegracao.AgIntegracao) {
                            Etapa1Liberada();
                        }
                        if (arg.Data.SituacaoIntegracao == EnumSituacaoIntegracao.AgRetorno) {
                            Etapa2Liberada();
                        }
                        if (arg.Data.SituacaoIntegracao == EnumSituacaoIntegracao.Integrado) {
                            Etapa2Aprovada();
                        }
                        if (arg.Data.SituacaoIntegracao == EnumSituacaoIntegracao.ProblemaIntegracao) {
                            Etapa2Reprovada();
                        }
                    }
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function AbrirTelaConfirmacaoDocumentosCarga() {
    LimparCampos(_confirmacaoDocumentosCarga);
    Global.abrirModal("divModalConfirmacaoDocumentosCarga");
}

function FecharTelaConfirmacaoDocumentosCarga() {
    LimparCampos(_confirmacaoDocumentosCarga);
    Global.fecharModal('divModalConfirmacaoDocumentosCarga');
}

function buscarCargas() {
    const editar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: selecionarCargaClick, tamanho: "15", icone: "" };
    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCargas = new GridView(_pesquisaCarga.Pesquisar.idGrid, "Carga/PesquisaCargasNaGrid", _pesquisaCarga, menuOpcoes, null, null, function (e) {
        _pesquisaCarga.BuscaAvancada.visibleFade(false);
    }, null, null, null, 10);
    _gridCargas.CarregarGrid();
}

function buscarCTEs() {

    _cargaCTe = new CargaCTe();
    _cargaCTe.Carga.val(_carga.Codigo.val());

    const baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    const baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    const retornoSefaz = { descricao: "Mensagem Sefaz", id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    const editar = { descricao: "Editar", id: guid(), metodo: editarCTeClick, icone: "", visibilidade: VisibilidadeOpcaoEditar };
    const cancelar = { descricao: "Cancelar o CT-e", id: guid(), metodo: cancelarCTeClick, icone: "", visibilidade: VisibilidadeAutorizado };
    const inutilizar = { descricao: "Inutilizar o CT-e", id: guid(), metodo: inutilizarCTeClick, icone: "", visibilidade: VisibilidadeRejeicao };
    const visualizar = { descricao: "Detalhes", id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDetalhes };
    const emitir = {
        descricao: "Emitir", id: guid(), metodo: function (datagrid) {
            emitirCTeClick(datagrid);
        }, icone: "", visibilidade: VisibilidadeRejeicao
    };
    const anularGerencial = {
        descricao: "Anular Gerenc. CT-e", id: guid(), metodo: function (datagrid) {
            AbrirTelaAnulacaoGerencialCTe(datagrid);
        }, icone: "", visibilidade: VisibilidadeAutorizado
    };
    const substituir = {
        descricao: "Substituir o CT-e", id: guid(), metodo: function (datagrid) {
            AbrirTelaSubstituicaoCTe(datagrid);
        }, icone: "", visibilidade: VisibilidadeAutorizadoSubstituicao
    };
    const duplicar = { descricao: "Duplicar", id: guid(), metodo: duplicarCTeClick, icone: "" };
    const naoEnviarMercante = { descricao: "Não enviar para o Mercante", id: guid(), metodo: naoEnviarMercanteClick, icone: "" };
    const auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ConhecimentoDeTransporteEletronico", "CodigoCTE"), visibilidade: VisibilidadeOpcaoAuditoria };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML, retornoSefaz, visualizar, editar, emitir, auditar] };

    menuOpcoes.opcoes = menuOpcoes.opcoes.concat([duplicar]);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        menuOpcoes.opcoes = menuOpcoes.opcoes.concat([naoEnviarMercante]);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CTeManual_PermiteCancelarCTe, _PermissoesPersonalizadas))
        menuOpcoes.opcoes = menuOpcoes.opcoes.concat([cancelar, inutilizar, substituir])

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CTeManual_PermiteAnulacaoGerencialCTe, _PermissoesPersonalizadas))
        menuOpcoes.opcoes = menuOpcoes.opcoes.concat([anularGerencial]);

    _gridCargaCTe = new GridView(_carga.CTe.idGrid, "CargaCTe/ConsultarCargaCTe", _cargaCTe, menuOpcoes, null);
    _gridCargaCTe.CarregarGrid(function () {
        $("#wid-id-4").show();

        _pesquisaCarga.ExibirFiltros.visibleFade(false);
    });
}

function LimparCamposCTeManual() {
    LimparCampos(_carga);
    Global.ResetarAbas();
    $("#wid-id-4").hide();
    DesabilitarTodasEtapas();

    _pesquisaCarga.ExibirFiltros.visibleFade(true);
}

function baixarDacteClick(e) {
    const data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function baixarXMLCTeClick(e) {
    const data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal('divModalRetornoSefaz');
}

function VisibilidadeMensagemSefaz(data) {
    return data.RetornoSefaz != "";
}

function VisibilidadeRejeicao(data) {
    return data.SituacaoCTe == EnumStatusCTe.REJEICAO && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador;
}

function VisibilidadeOpcaoDownload(data) {
    return data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.INUTILIZADO;
}

function VisibilidadeOpcaoDetalhes(data) {
    return (data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.INUTILIZADO) && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador;
}

function VisibilidadeAutorizado(data) {
    return data.SituacaoCTe == EnumStatusCTe.AUTORIZADO && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador;
}

function VisibilidadeAutorizadoSubstituicao(data) {
    return (data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS);
}

function VisibilidadeOpcaoAnular(data) {
    return data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO;
}

function VisibilidadeOpcaoEditar(data) {
    return (data.SituacaoCTe == EnumStatusCTe.EMDIGITACAO || data.SituacaoCTe == EnumStatusCTe.REJEICAO) && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS;
}

function SetarCamposMultiModal() {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _pesquisaCarga.NumeroBooking.visible(true);
        _pesquisaCarga.NumeroControle.visible(true);
        _pesquisaCarga.NumeroOS.visible(true);
    }
}

function configurarIntegracaoCTeManual() {
    executarReST("ConfiguracaoIntercab/ConsultarIntegracaoCTeManual", {}, function (r) {
        if (r.Success && r.Data) {
            _integracaoCTeManual = true;
        }
    });
}

function verificarPermissaoEmissaoCTeZerado(instanciaCTe) {
    if (instanciaCTe.TotalServico.ValorReceber.val() == "0,00") {
        return VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CTeManual_PermiteRealizarCTeManualComValorZerado, _PermissoesPersonalizadas);
    }

    return true;
}