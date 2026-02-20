/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
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
/// <reference path="../../Enumeradores/EnumTipoObservacaoFaturamentoMensal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFaturamentoMensalGrupo;
var _faturamentoMensalGrupo;
var _pesquisaFaturamentoMensalGrupo;

var _tipoObservacao = [{ text: "Usar em BOLETO", value: EnumTipoObservacaoFaturamentoMensal.Boleto },
    { text: "Nenhum", value: EnumTipoObservacaoFaturamentoMensal.Nenhum },
    { text: "Usar em NF", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscal },
    { text: "Usar em NF e BOLETO", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto }];

var PesquisaFaturamentoMensalGrupo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFaturamentoMensalGrupo.CarregarGrid();
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
}

var FaturamentoMensalGrupo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoObservacao = PropertyEntity({ val: ko.observable(EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto), options: _tipoObservacao, text: "Tipo Observacao: ", def: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto, required: false, enable: ko.observable(true) });    
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, maxlength: 500 });
    this.FaturamentoAutomatico = PropertyEntity({ val: ko.observable(false), text: "Calcular faturamento mensal a partir dos planos cadastrados?", def: false, getType: typesKnockout.bool, visible: ko.observable(true), eventChange: FaturamentoAutomaticoChange });

    this.DiaFatura = PropertyEntity({ text: "*Dia da Fatura: ", required: ko.observable(false), getType: typesKnockout.int });
    this.Servico = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Serviço Principal:", idBtnSearch: guid(), val: ko.observable("") });
    this.NaturezaDaOperacaoDentroEstado = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Natureza da Operação Dentro do Estado:", idBtnSearch: guid(), val: ko.observable("") });
    this.NaturezaDaOperacaoForaEstado = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Natureza da Operação Fora do Estado:", idBtnSearch: guid(), val: ko.observable("") });
    this.TipoMovimento = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Tipo de Movimento:", idBtnSearch: guid(), val: ko.observable("") });
    this.BoletoConfiguracao = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "Configuração do Boleto:", idBtnSearch: guid(), val: ko.observable("") });
    this.ObservacaoAdesao = PropertyEntity({ text: "Observação do faturamento automático:", required: false, maxlength: 500 });    

    this.TagNomeGrupo = PropertyEntity({ eventClick: function (e) { InserirTag(_faturamentoMensalGrupo.ObservacaoAdesao.id, "#NomeGrupo"); }, type: types.event, text: "Nome do Grupo" });
    this.TagQtdTotalDocumentos = PropertyEntity({ eventClick: function (e) { InserirTag(_faturamentoMensalGrupo.ObservacaoAdesao.id, "#QtdTotalDocumentos"); }, type: types.event, text: "Qtd Total Documentos" });
    this.TagQtdNFe = PropertyEntity({ eventClick: function (e) { InserirTag(_faturamentoMensalGrupo.ObservacaoAdesao.id, "#QtdNFe"); }, type: types.event, text: "Qtd NF-e" });
    this.TagQtdNFSe = PropertyEntity({ eventClick: function (e) { InserirTag(_faturamentoMensalGrupo.ObservacaoAdesao.id, "#QtdNFSe"); }, type: types.event, text: "Qtd NFS-e" });
    this.TagQtdBoleto = PropertyEntity({ eventClick: function (e) { InserirTag(_faturamentoMensalGrupo.ObservacaoAdesao.id, "#QtdBoleto"); }, type: types.event, text: "Qtd. Boletos" });
    this.TagQtdTitulo = PropertyEntity({ eventClick: function (e) { InserirTag(_faturamentoMensalGrupo.ObservacaoAdesao.id, "#QtdTitulo"); }, type: types.event, text: "Qtd. Títulos" });
    this.TagMesAnoPeriodo = PropertyEntity({ eventClick: function (e) { InserirTag(_faturamentoMensalGrupo.ObservacaoAdesao.id, "#MesAnoPeriodo"); }, type: types.event, text: "Período da fatura" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadFaturamentoMensalGrupo() {

    _pesquisaFaturamentoMensalGrupo = new PesquisaFaturamentoMensalGrupo();
    KoBindings(_pesquisaFaturamentoMensalGrupo, "knockoutPesquisaFaturamentoMensalGrupo", false, _pesquisaFaturamentoMensalGrupo.Pesquisar.id);

    _faturamentoMensalGrupo = new FaturamentoMensalGrupo();
    KoBindings(_faturamentoMensalGrupo, "knockoutCadastroFaturamentoMensalGrupo");

    HeaderAuditoria("FaturamentoMensalGrupo", _faturamentoMensalGrupo);

    new BuscarServicoTMS(_faturamentoMensalGrupo.Servico);
    new BuscarNaturezasOperacoesNotaFiscal(_faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado, null, null, null, null, null, null, "S");
    new BuscarNaturezasOperacoesNotaFiscal(_faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado, null, null, null, null, null, null, "N");
    new BuscarTipoMovimento(_faturamentoMensalGrupo.TipoMovimento);
    new BuscarBoletoConfiguracao(_faturamentoMensalGrupo.BoletoConfiguracao, RetornoBoletoConfiguracao);

    buscarFaturamentoMensalGrupo();
}

function RetornoBoletoConfiguracao(data) {
    _faturamentoMensalGrupo.BoletoConfiguracao.codEntity(data.Codigo);
    _faturamentoMensalGrupo.BoletoConfiguracao.val(data.DescricaoBanco);
}

function adicionarClick(e, sender) {
    Salvar(_faturamentoMensalGrupo, "FaturamentoMensalGrupo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridFaturamentoMensalGrupo.CarregarGrid();
                limparCamposFaturamentoMensalGrupo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    Salvar(_faturamentoMensalGrupo, "FaturamentoMensalGrupo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridFaturamentoMensalGrupo.CarregarGrid();
                limparCamposFaturamentoMensalGrupo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o grupo " + _faturamentoMensalGrupo.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_faturamentoMensalGrupo, "FaturamentoMensalGrupo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridFaturamentoMensalGrupo.CarregarGrid();
                    limparCamposFaturamentoMensalGrupo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposFaturamentoMensalGrupo();
}

//*******MÉTODOS*******

function FaturamentoAutomaticoChange(e, sender) {

    if (e.FaturamentoAutomatico.val() != true)
        $("#divFaturamentoAutomatico").removeClass("d-none");
    else
        $("#divFaturamentoAutomatico").addClass("d-none");
    
    _faturamentoMensalGrupo.DiaFatura.required(!e.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.Servico.required(!e.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado.required(!e.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado.required(!e.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.TipoMovimento.required(!e.FaturamentoAutomatico.val());
}


function editarFaturamentoMensalGrupo(faturamentoMensalGrupoGrid) {
    limparCamposFaturamentoMensalGrupo();
    _faturamentoMensalGrupo.Codigo.val(faturamentoMensalGrupoGrid.Codigo);
    BuscarPorCodigo(_faturamentoMensalGrupo, "FaturamentoMensalGrupo/BuscarPorCodigo", function (arg) {
        _pesquisaFaturamentoMensalGrupo.ExibirFiltros.visibleFade(false);
        _faturamentoMensalGrupo.Atualizar.visible(true);
        _faturamentoMensalGrupo.Cancelar.visible(true);
        _faturamentoMensalGrupo.Excluir.visible(true);
        _faturamentoMensalGrupo.Adicionar.visible(false);
        ValidarCamposFaturamentoAutomatico();
    }, null);
}

function ValidarCamposFaturamentoAutomatico() {
    if (_faturamentoMensalGrupo.FaturamentoAutomatico.val() == true)
        $("#divFaturamentoAutomatico").removeClass("d-none");
    else
        $("#divFaturamentoAutomatico").addClass("d-none");

    _faturamentoMensalGrupo.DiaFatura.required(_faturamentoMensalGrupo.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.Servico.required(_faturamentoMensalGrupo.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado.required(_faturamentoMensalGrupo.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado.required(_faturamentoMensalGrupo.FaturamentoAutomatico.val());
    _faturamentoMensalGrupo.TipoMovimento.required(_faturamentoMensalGrupo.FaturamentoAutomatico.val());
}


function buscarFaturamentoMensalGrupo() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFaturamentoMensalGrupo, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFaturamentoMensalGrupo = new GridView(_pesquisaFaturamentoMensalGrupo.Pesquisar.idGrid, "FaturamentoMensalGrupo/Pesquisa", _pesquisaFaturamentoMensalGrupo, menuOpcoes, null);
    _gridFaturamentoMensalGrupo.CarregarGrid();
}


function limparCamposFaturamentoMensalGrupo() {
    _faturamentoMensalGrupo.Atualizar.visible(false);
    _faturamentoMensalGrupo.Cancelar.visible(false);
    _faturamentoMensalGrupo.Excluir.visible(false);
    _faturamentoMensalGrupo.Adicionar.visible(true);
    LimparCampos(_faturamentoMensalGrupo);
    ValidarCamposFaturamentoAutomatico();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}