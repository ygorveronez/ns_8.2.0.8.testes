/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="DadosEmissao.js" />
/// <reference path="Geral.js" />
/// <reference path="Lacre.js" />
/// <reference path="LocaisPrestacao.js" />
/// <reference path="Observacao.js" />
/// <reference path="Passagem.js" />
/// <reference path="Percurso.js" />
/// <reference path="Rota.js" />
/// <reference path="Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Carga.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

//*******MAPEAMENTO*******
var _cargaDadosEmissaoConfiguracao;
var _indiceGlobalPedidoConfiguracaoEmissao;

var _notasFiscaisCompletas = "";
var _notasFiscaisResumidas = "";
var _expandirNotasFiscais = false;

var CargaDadosEmissaoConfiguracao = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.AplicarConfiguracaoEmTodosPedidos = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Cargas.Carga.AoAtualizarEssaConfiguracaoDesejaAplicalaEmTodosPedidosDaCarga, def: true, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.CargaTrocaNota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.CargaParaTrocaDeNotas.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(false) });
    this.FormulaRateio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.FormulaDoRateioDoFrete.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoEmissaoCTeParticipantes = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeParticipantes.Normal), options: EnumTipoEmissaoCTeParticipantes.obterOpcoes(), text: Localization.Resources.Cargas.Carga.ParticipantesDosDocumentos.getFieldDescription(), def: EnumTipoEmissaoCTeParticipantes.Normal, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoRateio = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeDocumentos.NaoInformado), options: EnumTipoEmissaoCTeDocumentos.obterOpcoes(), text: Localization.Resources.Cargas.Carga.RateioDosDocumentos.getFieldDescription(), def: EnumTipoEmissaoCTeDocumentos.NaoInformado, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.Recebedor.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(false) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.Expedidor.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), required: false, visible: ko.observable(false) });
    this.NotasFiscais = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.NotasFiscais.getFieldDescription()), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Destinatario.getFieldDescription()), visible: ko.observable(true) });
    this.TipoOperacaoUtilizaCentroDeCustoOuPEP = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });
    this.TipoOperacaoUtilizaContaRazao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });
    this.QuantidadePaletes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadePaletes.getFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, maxlength: 2, visible: ko.observable(false), enable: ko.observable(true) });
    this.FatorCubagemRateioFormula = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FatorDeCubagem, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, visible: ko.observable(false) });
    this.TipoUsoFatorCubagemRateioFormula = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeUsoDoFatorDeCubagem, val: ko.observable(EnumTipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria), options: EnumTupoUsoFatorCubagem.obterOpcoes(), def: EnumTipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria, issue: 0, visible: ko.observable(false) });

    this.Atualizar = PropertyEntity({ eventClick: alterarDadosEmissaoConfiguracaoClick, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarConfiguracoes, visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoEmissaoCTeParticipantes.val.subscribe(function (novoValor) {
        TipoEmissaoCTeParticipantesChange(novoValor);
    });

    this.ExpandirNotasFiscais = PropertyEntity({ eventClick: AlterarValorNotasFiscais, type: types.event });
    this.VerMais = PropertyEntity({ val: ko.observable(" ...Ver Todas") });
}

//*******EVENTOS*******

function loadCargaDadosEmissaoConfiguracao() {
    _cargaDadosEmissaoConfiguracao = new CargaDadosEmissaoConfiguracao();
    KoBindings(_cargaDadosEmissaoConfiguracao, "tabConfiguracoes_" + _cargaAtual.DadosEmissaoFrete.id);
    $("#tabConfiguracoes_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

    BuscarRateioFormulas(_cargaDadosEmissaoConfiguracao.FormulaRateio, retornoBuscaRateioFormula);
    BuscarClientes(_cargaDadosEmissaoConfiguracao.Expedidor);
    BuscarClientes(_cargaDadosEmissaoConfiguracao.Recebedor);
    BuscarCargaParaTrocaNota(_cargaDadosEmissaoConfiguracao.CargaTrocaNota);

    _cargaDadosEmissaoConfiguracao.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoConfiguracao.Atualizar.enable(_cargaAtual.EtapaFreteEmbarcador.enable());

    $("#" + _cargaDadosEmissaoConfiguracao.AplicarConfiguracaoEmTodosPedidos.id).click(aplicarConfiguracaoEmTodosPedidosClick);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarConfiguracao, _PermissoesPersonalizadasCarga))
        _cargaDadosEmissaoConfiguracao.Atualizar.enable(false);

    _cargaDadosEmissaoConfiguracao.QuantidadePaletes.visible(_cargaAtual.PermiteInformarQuantidadePaletes.val() === true);
}

function aplicarConfiguracaoEmTodosPedidosClick() {
    if (!_cargaDadosEmissaoConfiguracao.AplicarConfiguracaoEmTodosPedidos.val()) {
        carregarDadosPedido(0, () => $("#" + _cargaDadosEmissaoConfiguracao.Pedido.idTab).slideDown(), false, true);
    } else {
        $("#" + _cargaDadosEmissaoConfiguracao.Pedido.idTab).slideUp();
        obterDadosEmissaoGeralCarga();
    }
}
function retornoBuscaRateioFormula(valor) {

    _cargaDadosEmissaoConfiguracao.FormulaRateio.val(valor.Descricao);
    _cargaDadosEmissaoConfiguracao.FormulaRateio.codEntity(valor.Codigo);

    if (valor.ParametroRateioFormula == EnumParametroRateioFormula.PorPesoUtilizandoFatorCubagem) {
        _cargaDadosEmissaoConfiguracao.FatorCubagemRateioFormula.visible(true);
        _cargaDadosEmissaoConfiguracao.TipoUsoFatorCubagemRateioFormula.visible(true);
    }
    else {
        _cargaDadosEmissaoConfiguracao.FatorCubagemRateioFormula.visible(false);
        _cargaDadosEmissaoConfiguracao.TipoUsoFatorCubagemRateioFormula.visible(false);
    }
}

function alterarDadosEmissaoConfiguracaoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAtualizarConfiguracao, function () {
        Salvar(e, "DadosEmissaoConfiguracao/AtualizarConfiguracao", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ConfiguracaoAtualizadaComSucesso);

                    if (e.AplicarConfiguracaoEmTodosPedidos.val()) {
                        for (let i = 0; i < _cargaAtual.Pedidos.val.length; i++)
                            DesmacarPendenciaConfiguracao(i);
                    } else
                        DesmacarPendenciaConfiguracao(_indiceGlobalPedidoConfiguracaoEmissao);

                    BuscarPercursoCargaDadosEmissaoPassagem();
                    if (arg.Data != null)
                        preecherRetornoFrete(_cargaAtual, arg.Data);

                    return;
                }
                return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        });
    });
}

function DesmacarPendenciaConfiguracao(index) {
    const idLi = _cargaDadosEmissaoConfiguracao.Pedido.idTab + "_" + index;
    $("#" + idLi + " a").css("background", "");
    _cargaAtual.Pedidos.val[index].AgInformarRecebedor = false;
}

function TipoEmissaoCTeParticipantesChange(valor) {
    if (valor == EnumTipoEmissaoCTeParticipantes.ComRecebedor) {
        _cargaDadosEmissaoConfiguracao.Recebedor.visible(true);
        _cargaDadosEmissaoConfiguracao.Recebedor.required = true;
    }
    else {
        _cargaDadosEmissaoConfiguracao.Recebedor.visible(false);
        _cargaDadosEmissaoConfiguracao.Recebedor.required = false;
    }

    if (valor == EnumTipoEmissaoCTeParticipantes.ComExpedidor) {
        _cargaDadosEmissaoConfiguracao.Expedidor.visible(true);
        _cargaDadosEmissaoConfiguracao.Expedidor.required = true;
    }
    else {
        _cargaDadosEmissaoConfiguracao.Expedidor.visible(false);
        _cargaDadosEmissaoConfiguracao.Expedidor.required = false;
    }

    if (valor == EnumTipoEmissaoCTeParticipantes.ComExpedidorERecebedor) {
        _cargaDadosEmissaoConfiguracao.Recebedor.visible(true);
        _cargaDadosEmissaoConfiguracao.Expedidor.visible(true);
        _cargaDadosEmissaoConfiguracao.Recebedor.required = true;
        _cargaDadosEmissaoConfiguracao.Expedidor.required = true;
    }
}

function preencherCargaDadosEmissaoConfiguracao(dados) {
    if (_cargaDadosEmissaoConfiguracao != null) {
        PreencherObjetoKnout(_cargaDadosEmissaoConfiguracao, dados);

        visibilidadeDetalhesConfiguracao(dados.Data);
        _cargaDadosEmissaoGeral.Fronteiras.visible(dados.Data.PossuiFronteira);
        _cargaDadosEmissaoGeral.NumeroCargaVincularPreCarga.visible(dados.Data.CargaDePreCarga);
        _cargaDadosEmissaoGeral.NumeroCargaVincularPreCarga.val(dados.Data.NumeroCargaVincularPreCarga);
        _cargaDadosEmissaoGeral.NumeroOrdem.visible(dados.Data.InserirDadosContabeisXCampoXTextCTe);
        _cargaDadosEmissaoGeral.NumeroOrdem.val(dados.Data.NumeroOrdem);
        _cargaDadosEmissaoGeral.CentroResultado.val(dados.Data.CentroResultado.Descricao);
        _cargaDadosEmissaoGeral.CentroResultado.codEntity(dados.Data.CentroResultado.Codigo);
        _cargaDadosEmissaoGeral.ContaContabil.codEntity(dados.Data.ContaContabil.Codigo);
        _cargaDadosEmissaoGeral.ContaContabil.val(dados.Data.ContaContabil.Descricao);
        _cargaDadosEmissaoGeral.ElementoPEP.val(dados.Data.ElementoPEP);

        if (_cargaDadosEmissaoConfiguracao.TipoOperacaoUtilizaCentroDeCustoOuPEP.val()) {
            _cargaDadosEmissaoGeral.ElementoPEP.visible(true);
            _cargaDadosEmissaoGeral.CentroResultado.visible(true);
        }

        if (_cargaDadosEmissaoConfiguracao.TipoOperacaoUtilizaContaRazao.val()) {
            _cargaDadosEmissaoGeral.ContaContabil.visible(true);
            _cargaDadosEmissaoGeral.ContaContabil.required = true;
        }

        _notasFiscaisCompletas = _cargaDadosEmissaoConfiguracao.NotasFiscais.val();
        _notasFiscaisResumidas = _cargaDadosEmissaoConfiguracao.NotasFiscais.val().split(",", 50);
        AlterarValorNotasFiscais();
    }
}

function preencherDadosPedidoConfiguracao(dados) {
    _cargaDadosEmissaoConfiguracao.Carga.val(_cargaAtual.Codigo.val());
    _cargaDadosEmissaoConfiguracao.Pedido.val(dados.Codigo)
    _cargaDadosEmissaoConfiguracao.TipoEmissaoCTeParticipantes.val(dados.TipoEmissaoCTeParticipantes);

    _cargaDadosEmissaoConfiguracao.TipoRateio.val(dados.TipoRateio);

    _cargaDadosEmissaoConfiguracao.FormulaRateio.val(dados.FormulaRateio.Descricao);
    _cargaDadosEmissaoConfiguracao.FormulaRateio.codEntity(dados.FormulaRateio.Codigo);
    _cargaDadosEmissaoConfiguracao.Recebedor.val(ObterDescricaoParticipanteCargaPedido(dados.Recebedor));
    _cargaDadosEmissaoConfiguracao.Recebedor.codEntity(dados.Recebedor.Codigo);
    _cargaDadosEmissaoConfiguracao.Expedidor.val(ObterDescricaoParticipanteCargaPedido(dados.Expedidor));
    _cargaDadosEmissaoConfiguracao.Expedidor.codEntity(dados.Expedidor.Codigo);
    _cargaDadosEmissaoConfiguracao.Destinatario.val(dados.Destinatario);
    _cargaDadosEmissaoConfiguracao.NotasFiscais.val(dados.NotasFiscais);
    _cargaDadosEmissaoConfiguracao.QuantidadePaletes.val(dados.numeroPaletes);

    if (dados.FormulaRateio.ParametroRateioFormula == EnumParametroRateioFormula.PorPesoUtilizandoFatorCubagem) {
        _cargaDadosEmissaoConfiguracao.FatorCubagemRateioFormula.visible(true);
        _cargaDadosEmissaoConfiguracao.TipoUsoFatorCubagemRateioFormula.visible(true);
        _cargaDadosEmissaoConfiguracao.FatorCubagemRateioFormula.val(dados.FatorCubagemRateioFormula);
        _cargaDadosEmissaoConfiguracao.TipoUsoFatorCubagemRateioFormula.val(dados.TipoUsoFatorCubagemRateioFormula);
    }
    else {
        _cargaDadosEmissaoConfiguracao.FatorCubagemRateioFormula.visible(false);
        _cargaDadosEmissaoConfiguracao.TipoUsoFatorCubagemRateioFormula.visible(false);
    }

    TipoEmissaoCTeParticipantesChange(dados.TipoEmissaoCTeParticipantes);

    visibilidadeDetalhesConfiguracao(dados);
}

function ObterDescricaoParticipanteCargaPedido(participante) {
    let descricao = participante.Nome;

    if (!string.IsNullOrWhiteSpace(participante.CPF_CNPJ))
        descricao += " (" + participante.CPF_CNPJ + ")";

    if (participante.Localidade != null && !string.IsNullOrWhiteSpace(participante.Localidade.Descricao))
        descricao += " - " + participante.Localidade.Descricao;

    return descricao;
}

function visibilidadeDetalhesConfiguracao(dados) {
    if (_cargaDadosEmissaoConfiguracao.Destinatario.val() == "")
        _cargaDadosEmissaoConfiguracao.Destinatario.visible(false);
    else
        _cargaDadosEmissaoConfiguracao.Destinatario.visible(true);

    if (_cargaDadosEmissaoConfiguracao.NotasFiscais.val() == "")
        _cargaDadosEmissaoConfiguracao.NotasFiscais.visible(false);
    else
        _cargaDadosEmissaoConfiguracao.NotasFiscais.visible(true);

    _cargaDadosEmissaoConfiguracao.CargaTrocaNota.visible(dados ? dados.TipoOperacaoPermiteTrocaNota : false);
}

function carregarDadosEmissaoConfiguracao(indice) {
    if (indice != _indiceGlobalPedidoConfiguracaoEmissao) {
        BuscarDadosEmissao(indice, function (arg) {
            $("#" + _cargaDadosEmissaoConfiguracao.Pedido.idTab + "_" + _indiceGlobalPedidoConfiguracaoEmissao).removeClass("active");
            $("#" + _cargaDadosEmissaoConfiguracao.Pedido.idTab + "_" + indice).addClass("active");
            _indiceGlobalPedidoConfiguracaoEmissao = indice;
            preencherDadosPedidoConfiguracao(arg);
        });
    }
}

function AlterarValorNotasFiscais() {
    if (_notasFiscaisCompletas.split(",").length <= 50) {
        _cargaDadosEmissaoConfiguracao.VerMais.val("");
        return;
    }
    if (_expandirNotasFiscais) {
        _cargaDadosEmissaoConfiguracao.NotasFiscais.val(_notasFiscaisCompletas);
        _cargaDadosEmissaoConfiguracao.VerMais.val(" - Ver Menos");
        _expandirNotasFiscais = false;
    } else {
        _cargaDadosEmissaoConfiguracao.NotasFiscais.val(_notasFiscaisResumidas);
        _cargaDadosEmissaoConfiguracao.VerMais.val(" ...Ver Todas");
        _expandirNotasFiscais = true;
    }
}
