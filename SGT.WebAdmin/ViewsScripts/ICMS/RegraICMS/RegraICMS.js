/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoCliente.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoCTe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraICMS;
var _regraICMS;
var _pesquisaRegraICMS;

var _cst = [
    { text: "Não Informado", value: "" },
    { text: "00 - tributação normal ICMS", value: "00" },
    { text: "20 - tributação com BC reduzida do ICMS", value: "20" },
    { text: "40 - ICMS isenção", value: "40" },
    { text: "41 - ICMS não tributada", value: "41" },
    { text: "51 - ICMS diferido", value: "51" },
    { text: "60 - ICMS cobrado anteriormente por substituição tributária", value: "60" },
    { text: "90 - ICMS outros", value: "91" },
    { text: "90 - ICMS devido à UF de origem da prestação, quando diferente da UF do emitente", value: "90" },
    { text: "Simples Nacional", value: "SN" }
];

var _estados = [
    { text: "Não Selecionado", value: "" },
    { text: "Acre", value: "AC" },
    { text: "Alagoas", value: "AL" },
    { text: "Amapá", value: "AP" },
    { text: "Amazonas", value: "AM" },
    { text: "Bahia", value: "BA" },
    { text: "Ceará", value: "CE" },
    { text: "Distrito Federal", value: "DF" },
    { text: "Espírito Santo", value: "ES" },
    { text: "Goiás", value: "GO" },
    { text: "Maranhão", value: "MA" },
    { text: "Mato Grosso", value: "MT" },
    { text: "Mato Grosso do Sul", value: "MS" },
    { text: "Minas Gerais", value: "MG" },
    { text: "Pará", value: "PA" },
    { text: "Paraíba", value: "PB" },
    { text: "Paraná", value: "PR" },
    { text: "Pernambuco", value: "PE" },
    { text: "Piauí", value: "PI" },
    { text: "Rio de Janeiro", value: "RJ" },
    { text: "Rio Grande do Norte", value: "RN" },
    { text: "Rio Grande do Sul", value: "RS" },
    { text: "Rondônia", value: "RO" },
    { text: "Roraima", value: "RR" },
    { text: "Santa Catarina", value: "SC" },
    { text: "São Paulo", value: "SP" },
    { text: "Sergipe", value: "SE" },
    { text: "Tocantins", value: "TO" },
    { text: "Exterior", value: "EX" }
];

var PesquisaRegraICMS = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, maxlength: 200 });

    this.DataInicio = PropertyEntity({ text: "Filtro Vigência Início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Filtro Vigência Fim: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.UFEmitente = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Emitente: ", required: false });
    this.UFEmitenteDiferente = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Emitente Diferente de: ", required: false, visible: ko.observable(true) });
    this.UFOrigem = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Origem: ", required: false });
    this.UFDestino = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Destino: ", required: false });
    this.UFTomador = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Tomador: ", required: false });

    this.TipoRemetente = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoesSemCategoria(), def: EnumTipoCliente.Pessoa, text: "Tipo Remetente/Expedidor: ", required: false,  });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoesSemCategoria(), def: EnumTipoCliente.Pessoa, text: "Tipo Destinatario/Recebedor: ", required: false, });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoesSemCategoria(), def: EnumTipoCliente.Pessoa, text: "Tipo Tomador: ", required: false,  });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente/Expedidor:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário/Recebedor:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente/Expedidor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário/Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador: "), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraICMS.CarregarGrid();
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

    this.TipoTomador.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _pesquisaRegraICMS.Tomador.visible(true);
            _pesquisaRegraICMS.GrupoTomador.visible(false);
        } else {
            _pesquisaRegraICMS.Tomador.visible(false);
            _pesquisaRegraICMS.GrupoTomador.visible(true);
        }
    });

    this.TipoRemetente.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _pesquisaRegraICMS.Remetente.visible(true);
            _pesquisaRegraICMS.GrupoRemetente.visible(false);
        } else {
            _pesquisaRegraICMS.Remetente.visible(false);
            _pesquisaRegraICMS.GrupoRemetente.visible(true);
        }
    });

    this.TipoDestinatario.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _pesquisaRegraICMS.Destinatario.visible(true);
            _pesquisaRegraICMS.GrupoDestinatario.visible(false);
        } else {
            _pesquisaRegraICMS.Destinatario.visible(false);
            _pesquisaRegraICMS.GrupoDestinatario.visible(true);
        }
    });
};

var RegraICMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, maxlength: 200 });

    this.VigenciaInicio = PropertyEntity({ text: "Vigência Início: ", getType: typesKnockout.date });
    this.VigenciaFim = PropertyEntity({ text: "Vigência Fim: ", dateRangeInit: this.VigenciaInicio, getType: typesKnockout.date });
    this.VigenciaInicio.dateRangeLimit = this.VigenciaFim;
    this.VigenciaFim.dateRangeInit = this.VigenciaInicio;

    this.TipoModal = PropertyEntity({ val: ko.observable(EnumTipoModal.Todos), options: EnumTipoModal.obterOpcoesPesquisa(), def: EnumTipoModal.Todos, text: "Modal: ", required: false });
    this.TipoServico = PropertyEntity({ val: ko.observable(EnumTipoServicoCTe.Todos), options: EnumTipoServicoCTe.obterOpcoesPesquisa(), def: EnumTipoServicoCTe.Todos, text: "Tipo do Serviço: ", required: false });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(EnumTipoPagamento.Todos), options: EnumTipoPagamento.obterOpcoesPesquisa(), def: EnumTipoPagamento.Todos, text: "Tipo do Pagamento: ", required: false });
    this.UFOrigemIgualUFTomador = PropertyEntity({ getType: typesKnockout.bool, text: "UF de origem igual a UF do tomador?", val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.SomenteOptanteSimplesNacional = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Somente para emitente optante do simples nacional?" });
    this.UFEmitente = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Emitente: ", required: false });
    this.UFOrigem = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Origem: ", required: false });
    this.SetorEmpresa = PropertyEntity({ val: ko.observable(""), def: "", text: "Setor: ", required: false });
    this.NumeroProposta = PropertyEntity({ val: ko.observable(""), def: "", text: "Nº Proposta: ", required: false });
    this.EstadoOrigemDiferente = PropertyEntity({ val: ko.observable(false), def: false, text: " Diferente de", issue: 0 });
    this.UFDestino = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Destino: ", required: false });
    this.EstadoDestinoDiferente = PropertyEntity({ val: ko.observable(false), def: false, text: " Diferente de", issue: 0 });
    this.UFTomador = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Tomador: ", required: false });
    this.EstadoTomadorDiferente = PropertyEntity({ val: ko.observable(false), def: false, text: " Diferente de", issue: 0 });
    this.UFEmitenteDiferente = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "Estado Emitente Diferente de: ", required: false, visible: ko.observable(true) });
    this.TipoRemetente = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoesSemCategoria(), def: EnumTipoCliente.Pessoa, text: "Tipo Remetente/Expedidor: ", required: false, visible: ko.observable(true) });
    this.TipoDestinatario = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoesSemCategoria(), def: EnumTipoCliente.Pessoa, text: "Tipo Destinatario/Recebedor: ", required: false,  });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoCliente.Pessoa), type: types.local, options: EnumTipoCliente.obterOpcoesSemCategoria(), def: EnumTipoCliente.Pessoa, text: "Tipo Tomador: ", required: false,  });
    this.RegimeTributarioTomador = PropertyEntity({ val: ko.observable(EnumRegimeTributario.NaoSelecionado), options: EnumRegimeTributario.obterOpcoesNaoSelecionado(), def: EnumRegimeTributario.NaoSelecionado, text: "Regime Tributário do Tomador: ", required: false });
    this.RegimeTributarioTomadorDiferente = PropertyEntity({ val: ko.observable(false), def: false, text: " Diferente de", issue: 0 });

    this.GrupoRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente/Expedidor:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário/Recebedor:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente/Expedidor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário/Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador: "), idBtnSearch: guid(), visible: ko.observable(true) });

    this.AtividadeRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade Remetente/Expedidor:", idBtnSearch: guid() });
    this.AtividadeDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade Destinatário/Recebedor:", idBtnSearch: guid() });
    this.AtividadeTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade Tomador:", idBtnSearch: guid() });
    this.AtividadeTomadorDiferente = PropertyEntity({ val: ko.observable(false), def: false, text: " Diferente de", issue: 0 });

    //this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto Embarcador:", idBtnSearch: guid() });

    this.AdicionarTipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridTipoOperacao = PropertyEntity({ type: types.local });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AdicionarTipoDeCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Carga", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridTipoDeCarga = PropertyEntity({ type: types.local });
    this.TiposDeCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AdicionarProdutoEmbarcador = PropertyEntity({ type: types.event, text: "Adicionar Produto Embarcador", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridProdutoEmbarcador = PropertyEntity({ type: types.local });
    this.ProdutosEmbarcador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PercentualReducaoBC = PropertyEntity({ type: types.map, text: "Percentual Redução BC:", val: ko.observable(""), getType: typesKnockout.decimal, def: "", visible: ko.observable(false) });
    this.PercentualCreditoPresumido = PropertyEntity({ type: types.map, text: "Percentual Crédito Presumido:", val: ko.observable(""), getType: typesKnockout.decimal, def: "", visible: ko.observable(false) });

    this.Aliquota = PropertyEntity({ type: types.map, text: "Aliquota:", configDecimal: { precision: 2, allowZero: true }, val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.decimal, maxlength: 6 });
    this.ComAliquota = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Alíquota", def: false });
    this.ComAliquota.val.subscribe(ComAliquotaSubscribe);
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP:", idBtnSearch: guid() });
    this.DescricaoRegra = PropertyEntity({ text: "*Descrição da Regra (lei): ", issue: 779, required: true, maxlength: 400 });

    this.CST = PropertyEntity({ val: ko.observable(""), options: _cst, def: "", text: "CST:", required: false });
    this.CST.val.subscribe(CSTChange);

    this.IncluirPisConfisNaBC = PropertyEntity({ val: ko.observable(false), def: false, visible: ko.observable(true), text: "Incluir PIS e COFINS na base de Cálculo" });
    this.NaoIncluirPisConfisNaBCParaComplementos = PropertyEntity({ val: ko.observable(false), def: false, text: "Não incluir quando o documento for do tipo complementar" });
    this.NaoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber = PropertyEntity({ val: ko.observable(false), def: false, text: "Não calcular ICMS reduzido para o total da prestação e valores a receber" });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.ImprimeLeiNoCTe = PropertyEntity({ val: ko.observable(false), def: false, text: "Imprimir lei (Descrição da Regra) no CT-e", issue: 780 });
    this.ZerarValorICMS = PropertyEntity({ val: ko.observable(false), def: false, text: "Zerar base de calculo do ICMS" });
    this.DescontarICMSDoValorAReceber = PropertyEntity({ val: ko.observable(false), def: false, text: "Descontar ICMS do Valor a Receber", visible: ko.observable(false) });
    this.NaoReduzirRetencaoICMSDoValorDaPrestacao = PropertyEntity({ val: ko.observable(false), def: false, text: "Não reduzir a retenção do ICMS do Valor da Prestação?", visible: ko.observable(false) });
    this.NaoImprimirImpostosDACTE = PropertyEntity({ val: ko.observable(false), def: false, text: "Não imprimir os impostos no DACTE", issue: 0 });
    this.NaoEnviarImpostoICMSNaEmissaoCte = PropertyEntity({ val: ko.observable(false), def: false, text: "Não enviar impostos ICMS na emissão de CTe", issue: 0 });
    this.NaoIncluirICMSValorFrete = PropertyEntity({ val: ko.observable(false), def: false, text: "Não incluir ICMS no valor do frete", issue: 0 });

    this.TagAliquotaCTe = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#Aliquota#"); }, type: types.event, text: "Aliquota", visible: ko.observable(true) });
    this.TagValorICMSCTe = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#ValorICMS#"); }, type: types.event, text: "Valor ICMS", visible: ko.observable(true) });
    this.TagValorFreteCTe = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#ValorFrete#"); }, type: types.event, text: "Valor Frete", visible: ko.observable(true) });
    this.TagBaseCalculo = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#BaseCalculo#"); }, type: types.event, text: "Base de Cálculo", visible: ko.observable(true) });

    var empresa = "Empresa/Filial";
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        empresa = "Transportador";

    this.TagTransportadora = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#Transportadora#"); }, type: types.event, text: empresa, visible: ko.observable(true) });
    this.TagTomador = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#Tomador#"); }, type: types.event, text: "Tomador", visible: ko.observable(true) });
    this.TagRementente = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#Rementente#"); }, type: types.event, text: "Rementente", visible: ko.observable(true) });
    this.TagUFOrigem = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#UFOrigem#"); }, type: types.event, text: "UF Origem", visible: ko.observable(true) });
    this.TagUFDestino = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#UFDestino#"); }, type: types.event, text: "UF Destino", visible: ko.observable(true) });
    this.TagProduto = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#Produto#"); }, type: types.event, text: "Produto Predominante", visible: ko.observable(true) });
    this.TagPercentualICMSIncluirNoFrete = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#PercentualICMSIncluirNoFrete#"); }, type: types.event, text: "Percentual Inclusão ICMS", visible: ko.observable(true) });
    this.TagValorCreditoPresumido = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#ValorCreditoPresumido#"); }, type: types.event, text: "Valor Crédito Presumido", visible: ko.observable(true) });
    this.TagValorICMSMenosValorCreditoPresumido = PropertyEntity({ eventClick: function (e) { InserirTag(_regraICMS.DescricaoRegra.id, "#ValorICMSMenosValorCreditoPresumido#"); }, type: types.event, text: "Valor ICMS - Valor Crédito Presumido", visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.TipoTomador.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _regraICMS.Tomador.visible(true);
            _regraICMS.GrupoTomador.visible(false);
        } else {
            _regraICMS.Tomador.visible(false);
            _regraICMS.GrupoTomador.visible(true);
        }
    });

    this.TipoRemetente.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _regraICMS.Remetente.visible(true);
            _regraICMS.GrupoRemetente.visible(false);
        } else {
            _regraICMS.Remetente.visible(false);
            _regraICMS.GrupoRemetente.visible(true);
        }
    });

    this.TipoDestinatario.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _regraICMS.Destinatario.visible(true);
            _regraICMS.GrupoDestinatario.visible(false);
        } else {
            _regraICMS.Destinatario.visible(false);
            _regraICMS.GrupoDestinatario.visible(true);
        }
    });

};


//*******EVENTOS*******

function CSTChange(e, sender) {
    exibirPercentualReducao(_regraICMS);
    controlarDescontoICMS(_regraICMS);
}

function ComAliquotaSubscribe(val) {
    if (!val)
        _regraICMS.Aliquota.val("");
    else if (_regraICMS.Aliquota.val() === "")
        _regraICMS.Aliquota.val("0,00");
}

function loadRegraICMS() {

    _regraICMS = new RegraICMS();
    KoBindings(_regraICMS, "knockoutCadastroRegraICMS");

    HeaderAuditoria("RegraICMS", _regraICMS);

    _pesquisaRegraICMS = new PesquisaRegraICMS();
    KoBindings(_pesquisaRegraICMS, "knockoutPesquisaRegraICMS", false, _pesquisaRegraICMS.Pesquisar.id);

    new BuscarClientes(_pesquisaRegraICMS.Remetente);
    new BuscarClientes(_pesquisaRegraICMS.Destinatario);
    new BuscarClientes(_pesquisaRegraICMS.Tomador);
    new BuscarGruposPessoas(_pesquisaRegraICMS.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_pesquisaRegraICMS.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_pesquisaRegraICMS.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarTransportadores(_pesquisaRegraICMS.Empresa);

    new BuscarClientes(_regraICMS.Remetente);
    new BuscarClientes(_regraICMS.Destinatario);
    new BuscarClientes(_regraICMS.Tomador);
    new BuscarGruposPessoas(_regraICMS.GrupoDestinatario, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_regraICMS.GrupoRemetente, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_regraICMS.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarAtividades(_regraICMS.AtividadeDestinatario);
    new BuscarAtividades(_regraICMS.AtividadeRemetente);
    new BuscarAtividades(_regraICMS.AtividadeTomador);
    new BuscarCFOPs(_regraICMS.CFOP);
    //new BuscarProdutos(_regraICMS.ProdutoEmbarcador);
    new BuscarTransportadores(_regraICMS.Empresa, RetornoTransportador);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _regraICMS.Empresa.text("Empresa/Filial: ");
        _pesquisaRegraICMS.Empresa.text("Empresa/Filial: ");
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _regraICMS.Empresa.visible(false);
        _pesquisaRegraICMS.Empresa.visible(false);
        _regraICMS.UFEmitenteDiferente.visible(false);
        _pesquisaRegraICMS.UFEmitenteDiferente.visible(false);
    }

    LoadTiposOperacao();
    LoadTiposDeCarga();
    LoadProdutoEmbarcador();
    buscarRegraICMS();

}

function RetornoTransportador(data) {
    _regraICMS.Empresa.codEntity(data.Codigo);
    _regraICMS.Empresa.val(data.Descricao + " " + data.CNPJ + "");
}

function adicionarClick(e, sender) {

    _regraICMS.TiposOperacao.val(JSON.stringify(_regraICMS.AdicionarTipoOperacao.basicTable.BuscarRegistros()));
    _regraICMS.TiposDeCarga.val(JSON.stringify(_regraICMS.AdicionarTipoDeCarga.basicTable.BuscarRegistros()));
    _regraICMS.ProdutosEmbarcador.val(JSON.stringify(_regraICMS.AdicionarProdutoEmbarcador.basicTable.BuscarRegistros()));

    Salvar(e, "RegraICMS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && _CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoAlteracaoRegraICMS)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado efetuado, aguardando aprovação do Embarcador");
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");

                _gridRegraICMS.CarregarGrid();
                limparCamposRegraICMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {

    _regraICMS.TiposOperacao.val(JSON.stringify(_regraICMS.AdicionarTipoOperacao.basicTable.BuscarRegistros()));
    _regraICMS.TiposDeCarga.val(JSON.stringify(_regraICMS.AdicionarTipoDeCarga.basicTable.BuscarRegistros()));
    _regraICMS.ProdutosEmbarcador.val(JSON.stringify(_regraICMS.AdicionarProdutoEmbarcador.basicTable.BuscarRegistros()));

    Salvar(e, "RegraICMS/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridRegraICMS.CarregarGrid();
                limparCamposRegraICMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a regra de ICMS " + _regraICMS.DescricaoRegra.val() + "?", function () {
        ExcluirPorCodigo(_regraICMS, "RegraICMS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraICMS.CarregarGrid();
                    limparCamposRegraICMS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg)
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRegraICMS();
}

//*******MÉTODOS*******


function buscarRegraICMS() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraICMS, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraICMS = new GridView(_pesquisaRegraICMS.Pesquisar.idGrid, "RegraICMS/Pesquisa", _pesquisaRegraICMS, menuOpcoes);
    _gridRegraICMS.CarregarGrid();
}

function editarRegraICMS(regraICMSGrid) {
    limparCamposRegraICMS();
    _regraICMS.Codigo.val(regraICMSGrid.Codigo);
    BuscarPorCodigo(_regraICMS, "RegraICMS/BuscarPorCodigo", function (arg) {
        _pesquisaRegraICMS.ExibirFiltros.visibleFade(false);
        _regraICMS.Atualizar.visible(true);
        _regraICMS.Cancelar.visible(true);
        _regraICMS.Excluir.visible(true);
        _regraICMS.Adicionar.visible(false);
        if (_regraICMS.GrupoRemetente.codEntity() > 0)
            setarGrupoRemetente(_regraICMS);
        if (_regraICMS.GrupoDestinatario.codEntity() > 0)
            setarGrupoDestinatario(_regraICMS);
        if (_regraICMS.GrupoTomador.codEntity() > 0)
            setarGrupoTomador(_regraICMS);
        if (_regraICMS.Remetente.codEntity() > 0)
            setarRemetente(_regraICMS);
        if (_regraICMS.Destinatario.codEntity() > 0)
            setarDestinatario(_regraICMS);
        if (_regraICMS.Tomador.codEntity() > 0)
            setarTomador(_regraICMS);

        _regraICMS.AdicionarTipoOperacao.basicTable.CarregarGrid(_regraICMS.TiposOperacao.val());
        _regraICMS.AdicionarTipoDeCarga.basicTable.CarregarGrid(_regraICMS.TiposDeCarga.val());
        _regraICMS.AdicionarProdutoEmbarcador.basicTable.CarregarGrid(_regraICMS.ProdutosEmbarcador.val());

        exibirPercentualReducao(_regraICMS);
    }, null);
}

function exibirPercentualReducao(e) {
    var cst = e.CST.val();
    if (cst == "20" || cst == "60" || cst == "90" || cst == "91") {
        e.PercentualReducaoBC.visible(true);
    } else {
        e.PercentualReducaoBC.visible(false);
        e.PercentualReducaoBC.val("");
    }

    if (cst == "60" || cst == "91") {
        e.PercentualCreditoPresumido.visible(true);
    } else {
        e.PercentualCreditoPresumido.visible(false);
        e.PercentualCreditoPresumido.val("");
    }

    if (cst == "40" || cst == "41" || cst == "51" || cst == "SN") {
        e.DescontarICMSDoValorAReceber.visible(false);
        e.DescontarICMSDoValorAReceber.val(false);
        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.visible(false);
        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.val(false);
    }
    else if (cst == "60") {
        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.visible(true);
        e.DescontarICMSDoValorAReceber.visible(_CONFIGURACAO_TMS.UtilizarRegraICMSParaDescontarValorICMS);
    }
    else {
        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.visible(false);
        e.DescontarICMSDoValorAReceber.visible(_CONFIGURACAO_TMS.UtilizarRegraICMSParaDescontarValorICMS);
    }

}

function controlarDescontoICMS(e) {
    var cst = e.CST.val();

    if (cst == "40" || cst == "41" || cst == "51" || cst == "SN") {
        e.DescontarICMSDoValorAReceber.visible(false);
        e.DescontarICMSDoValorAReceber.val(false);

        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.visible(false);
        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.val(false);
    }
    else if (cst == "60") {
        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.visible(true);

        e.DescontarICMSDoValorAReceber.visible(_CONFIGURACAO_TMS.UtilizarRegraICMSParaDescontarValorICMS);
        e.DescontarICMSDoValorAReceber.val(true);
    }
    else {
        e.NaoReduzirRetencaoICMSDoValorDaPrestacao.visible(false);

        e.DescontarICMSDoValorAReceber.visible(_CONFIGURACAO_TMS.UtilizarRegraICMSParaDescontarValorICMS);
        e.DescontarICMSDoValorAReceber.val(false);
    }
}


function setarRemetente(e) {
    e.TipoRemetente.val(1);
    e.GrupoRemetente.val("");
    e.GrupoRemetente.codEntity(0);
    e.GrupoRemetente.visible(false);
    e.Remetente.visible(true);
}
function setarGrupoRemetente(e) {
    e.TipoRemetente.val(2);
    e.Remetente.val("");
    e.Remetente.codEntity(0);
    e.GrupoRemetente.visible(true);
    e.Remetente.visible(false);
}
function setarDestinatario(e) {
    e.TipoDestinatario.val(1);
    e.GrupoDestinatario.val("");
    e.GrupoDestinatario.codEntity(0);
    e.GrupoDestinatario.visible(false);
    e.Destinatario.visible(true);
}
function setarGrupoDestinatario(e) {
    e.TipoDestinatario.val(2);
    e.Destinatario.val("");
    e.Destinatario.codEntity(0);
    e.GrupoDestinatario.visible(true);
    e.Destinatario.visible(false);
}
function setarTomador(e) {
    e.TipoTomador.val(1);
    e.GrupoTomador.val("");
    e.GrupoTomador.codEntity(0);
    e.GrupoTomador.visible(false);
    e.Tomador.visible(true);
}
function setarGrupoTomador(e) {
    e.TipoTomador.val(2);
    e.Tomador.val("");
    e.Tomador.codEntity(0);
    e.GrupoTomador.visible(true);
    e.Tomador.visible(false);
}

function limparCamposRegraICMS() {
    _regraICMS.TipoTomador.val(1);
    _regraICMS.TipoDestinatario.val(1);
    _regraICMS.TipoRemetente.val(1);
    _regraICMS.Atualizar.visible(false);
    _regraICMS.Cancelar.visible(false);
    _regraICMS.Excluir.visible(false);
    _regraICMS.Adicionar.visible(true);

    _regraICMS.AdicionarTipoOperacao.basicTable.CarregarGrid(new Array());
    _regraICMS.AdicionarTipoDeCarga.basicTable.CarregarGrid(new Array());
    _regraICMS.AdicionarProdutoEmbarcador.basicTable.CarregarGrid(new Array());

    LimparCampos(_regraICMS);
}

function DescricaoEstado(sigla) {
    var descricao = "";
    $.each(_estados, function (i, estado) {
        if (estado.value == sigla) {
            descricao = estado.text;
            return false;
        }
    });
    return descricao;
}