/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioChamadoDevolucao, _gridChamadoDevolucao, _pesquisaChamadoDevolucao, _CRUDRelatorioDevolucao, _CRUDFiltrosRelatorioDevolucao;

var PesquisaChamadoDevolucao = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.SituacaoChamado = PropertyEntity({ val: ko.observable(EnumSituacaoChamado.Todas), options: EnumSituacaoChamado.obterOpcoesPesquisa(), def: EnumSituacaoChamado.Todas, text: "Situação: ", visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsável:", issue: 210, idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.Motivo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Representante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Representante:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoasCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas do Cliente:", idBtnSearch: guid() });
    this.GrupoPessoasTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas do Tomador:", idBtnSearch: guid() });
    this.GrupoPessoasDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas do Destinatário:", idBtnSearch: guid() });
    this.FilialVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial de Venda:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoasResponsavel = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas Responsável:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.ClienteResponsavel = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cliente Responsável:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.NF = PropertyEntity({ text: "Nota Fiscal: ", getType: typesKnockout.int });
    this.CTE = PropertyEntity({ text: "Número CT-e: ", getType: typesKnockout.int });
    this.Placa = PropertyEntity({ text: "Placa: ", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Carga = PropertyEntity({ text: "Carga: " });
    this.GerouOcorrencia = PropertyEntity({ text: "Somente atendimentos que resultaram em uma Ocorrência", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.SomenteAtendimentoEstornado = PropertyEntity({ text: "Somente atendimentos estornados", val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.DataCriacaoInicio = PropertyEntity({ text: "Data Criação Início: ", getType: typesKnockout.date });
    this.DataCriacaoFim = PropertyEntity({ text: "Data Criação Fim: ", getType: typesKnockout.date });
    this.DataFinalizacaoInicio = PropertyEntity({ text: "Data Finalização Início: ", getType: typesKnockout.date });
    this.DataFinalizacaoFim = PropertyEntity({ text: "Data Finalização Fim: ", getType: typesKnockout.date });
    this.DataCriacaoInicio.dateRangeLimit = this.DataCriacaoFim;
    this.DataCriacaoFim.dateRangeInit = this.DataCriacaoInicio;
    this.DataFinalizacaoInicio.dateRangeLimit = this.DataFinalizacaoFim;
    this.DataFinalizacaoFim.dateRangeInit = this.DataFinalizacaoInicio;

    this.DataInicialChegadaDiaria = PropertyEntity({ text: "Data Ini. Chegada Diária: ", getType: typesKnockout.date });
    this.DataFinalChegadaDiaria = PropertyEntity({ text: "Data Fin. Chegada Diária: ", getType: typesKnockout.date });
    this.DataInicialSaidaDiaria = PropertyEntity({ text: "Data Ini. Saída Diária: ", getType: typesKnockout.date });
    this.DataFinalSaidaDiaria = PropertyEntity({ text: "Data Fin. Saída Diária: ", getType: typesKnockout.date });
    this.DataInicialChegadaDiaria.dateRangeLimit = this.DataFinalChegadaDiaria;
    this.DataFinalChegadaDiaria.dateRangeInit = this.DataInicialChegadaDiaria;
    this.DataInicialSaidaDiaria.dateRangeLimit = this.DataFinalSaidaDiaria;
    this.DataFinalSaidaDiaria.dateRangeInit = this.DataInicialSaidaDiaria;

    this.PossuiAnexoNFSe = PropertyEntity({ text: "Possui Anexo NFs-e?", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorioDevolucao = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridChamadoDevolucao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaChamadoDevolucao.Visible.visibleFade()) {
                _pesquisaChamadoDevolucao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaChamadoDevolucao.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorioDevolucao = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadChamadoDevolucao() {
    _pesquisaChamadoDevolucao = new PesquisaChamadoDevolucao();
    _CRUDRelatorioDevolucao = new CRUDRelatorioDevolucao();
    _CRUDFiltrosRelatorioDevolucao = new CRUDFiltrosRelatorioDevolucao();

    _gridChamadoDevolucao = new GridView(_CRUDFiltrosRelatorioDevolucao.Preview.idGrid, "Relatorios/ChamadoDevolucao/Pesquisa", _pesquisaChamadoDevolucao);
    _gridChamadoDevolucao.SetPermitirEdicaoColunas(true);
    _gridChamadoDevolucao.SetQuantidadeLinhasPorPagina(10);

    _relatorioChamadoDevolucao= new RelatorioGlobal("Relatorios/ChamadoDevolucao/BuscarDadosRelatorio", _gridChamadoDevolucao, function () {
        _relatorioChamadoDevolucao.loadRelatorio(function () {
            KoBindings(_pesquisaChamadoDevolucao, "knockoutPesquisaChamadoDevolucao", false);
            KoBindings(_CRUDRelatorioDevolucao, "knockoutCRUDPesquisaChamadoDevolucao", false);
            KoBindings(_CRUDFiltrosRelatorioDevolucao, "knockoutCRUDFiltrosPesquisaChamadoDevolucao", false);

            new BuscarTransportadores(_pesquisaChamadoDevolucao.Transportador);
            new BuscarFilial(_pesquisaChamadoDevolucao.Filial);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
                new BuscarFuncionario(_pesquisaChamadoDevolucao.Responsavel);
            else
                new BuscarOperador(_pesquisaChamadoDevolucao.Responsavel);
            new BuscarMotivoChamado(_pesquisaChamadoDevolucao.Motivo);
            new BuscarClientes(_pesquisaChamadoDevolucao.Cliente);
            new BuscarClientes(_pesquisaChamadoDevolucao.Tomador);
            new BuscarClientes(_pesquisaChamadoDevolucao.Destinatario);
            new BuscarMotoristas(_pesquisaChamadoDevolucao.Motorista);
            new BuscarRepresentante(_pesquisaChamadoDevolucao.Representante);
            new BuscarGruposPessoas(_pesquisaChamadoDevolucao.GrupoPessoasCliente);
            new BuscarGruposPessoas(_pesquisaChamadoDevolucao.GrupoPessoasTomador);
            new BuscarGruposPessoas(_pesquisaChamadoDevolucao.GrupoPessoasDestinatario);
            new BuscarFilial(_pesquisaChamadoDevolucao.FilialVenda);
            new BuscarClientes(_pesquisaChamadoDevolucao.ClienteResponsavel);
            new BuscarGruposPessoas(_pesquisaChamadoDevolucao.GrupoPessoasResponsavel);
            new BuscarVeiculos(_pesquisaChamadoDevolucao.Veiculo);

            SetarLayoutPorTipoServico();

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaChamadoDevolucao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioChamadoDevolucao.gerarRelatorio("Relatorios/ChamadoDevolucao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioChamadoDevolucao.gerarRelatorio("Relatorios/ChamadoDevolucao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function SetarLayoutPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaChamadoDevolucao.Filial.visible(false);
        _pesquisaChamadoDevolucao.FilialVenda.visible(false);
        _pesquisaChamadoDevolucao.Representante.visible(false);
        _pesquisaChamadoDevolucao.Transportador.text("Empresa/Filial:");
        _pesquisaChamadoDevolucao.Responsavel.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _pesquisaChamadoDevolucao.ClienteResponsavel.visible(true);
        _pesquisaChamadoDevolucao.GrupoPessoasResponsavel.visible(true);
        _pesquisaChamadoDevolucao.Veiculo.visible(true);
        _pesquisaChamadoDevolucao.Placa.visible(false);
    }

    if (_CONFIGURACAO_TMS.ExigirClienteResponsavelPeloAtendimento)
        _pesquisaChamadoDevolucao.ClienteResponsavel.visible(true);
}