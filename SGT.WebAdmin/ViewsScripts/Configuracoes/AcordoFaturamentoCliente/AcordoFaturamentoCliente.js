/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumTipoPrazoFaturamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridAcordoFaturamentoCliente;
var _acordoFaturamentoCliente;
var _pesquisaAcordoFaturamentoCliente;
var _PermissoesPersonalizadas;

var _gridEmailsCabotagem;
var _gridEmailsLongoCurso;
var _gridEmailsCustoExtra;

var _diaMesAcordoFaturamentoCliente = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 },
    { text: "11", value: 11 },
    { text: "12", value: 12 },
    { text: "13", value: 13 },
    { text: "14", value: 14 },
    { text: "15", value: 15 },
    { text: "16", value: 16 },
    { text: "17", value: 17 },
    { text: "18", value: 18 },
    { text: "19", value: 19 },
    { text: "20", value: 20 },
    { text: "21", value: 21 },
    { text: "22", value: 22 },
    { text: "23", value: 23 },
    { text: "24", value: 24 },
    { text: "25", value: 25 },
    { text: "26", value: 26 },
    { text: "27", value: 27 },
    { text: "28", value: 28 },
    { text: "29", value: 29 },
    { text: "30", value: 30 },
    { text: "31", value: 31 }
];

var EmailCabotagemMap = function () {
    this.CodigoEmailCabotagem = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.PessoaCabotagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailCabotagem = PropertyEntity({ type: types.map, val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.text });
};

var EmailLongoCursoMap = function () {
    this.CodigoEmailLongoCurso = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.PessoaLongoCurso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailLongoCurso = PropertyEntity({ type: types.map, val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.text });
};

var EmailCustoExtraMap = function () {
    this.CodigoEmailCustoExtra = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.PessoaCustoExtra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailCustoExtra = PropertyEntity({ type: types.map, val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.text });
};

var PesquisaAcordoFaturamentoCliente = function () {
    this.TipoPessoa = PropertyEntity({ text: "Tipo Pessoa:", val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pessoa:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        _pesquisaAcordoFaturamentoCliente.Pessoa.visible(false);
        _pesquisaAcordoFaturamentoCliente.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaAcordoFaturamentoCliente.Pessoa);
        LimparCampoEntity(_pesquisaAcordoFaturamentoCliente.GrupoPessoa);

        if (novoValor === EnumTipoPessoaGrupo.Pessoa)
            _pesquisaAcordoFaturamentoCliente.Pessoa.visible(true);
        else
            _pesquisaAcordoFaturamentoCliente.GrupoPessoa.visible(true);
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAcordoFaturamentoCliente.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var AcordoFaturamentoCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoPessoa = PropertyEntity({ text: "Tipo Pessoa:", val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.FaturamentoPermissaoExclusiva = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Faturamento será realizado apenas com permissão exclusiva?", def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.NaoEnviarEmailFaturaAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não enviar por e-mail os dados da fatura de forma automática?", def: false, enable: ko.observable(true), visible: ko.observable(false) });


    this.TipoPessoa.val.subscribe(function () {
        tipoPessoaAcordoFaturamentoClienteChange();
    });

    //Cabotagem
    this.FaturamentoPermissaoExclusivaCabotagem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Faturamento será realizado apenas com permissão exclusiva?", def: false, enable: ko.observable(true) });
    this.CabotagemDiasSemanaFatura = PropertyEntity({ val: ko.observable([]), options: EnumDiaSemana.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple, text: "Dia da Semana:", enable: ko.observable(true) });
    this.CabotagemDiasMesFatura = PropertyEntity({ val: ko.observable([]), options: _diaMesAcordoFaturamentoCliente, def: [], getType: typesKnockout.selectMultiple, text: "Dia do Mês:", enable: ko.observable(true) });
    this.CabotagemGerarFaturamentoAVista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gerar faturamento a vista por CT-e?", def: false, enable: ko.observable(true) });
    this.CabotagemTipoPrazoFaturamento = PropertyEntity({ val: ko.observable(EnumTipoPrazoFaturamento.DataFatura), options: EnumTipoPrazoFaturamento.obterOpcoes(), def: EnumTipoPrazoFaturamento.DataFatura, text: "Tipo prazo faturamento:", enable: ko.observable(true) });
    this.CabotagemDiasDePrazoFatura = PropertyEntity({ text: "Dias de prazo do faturamento:", maxlength: 2, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });
    this.CabotagemEmail = PropertyEntity({ text: "E-mail padrão para o grupo econômico:", getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.CabotagemNaoEnviarEmailFaturaAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não enviar por e-mail os dados da fatura de forma automática?", def: false, enable: ko.observable(true) });

    this.EmailsCabotagem = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GridEmailsCabotagem = PropertyEntity({ type: types.local });
    this.CodigoEmailCabotagem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PessoaCabotagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailCabotagem = PropertyEntity({ text: "*E-mail:", getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.SalvarEmailCabotagem = PropertyEntity({ eventClick: SalvarEmailCabotagemClick, type: types.event, text: "Salvar E-mail" });

    //LongoCurso
    this.FaturamentoPermissaoExclusivaLongoCurso = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Faturamento será realizado apenas com permissão exclusiva?", def: false, enable: ko.observable(true) });
    this.LongoCursoDiasSemanaFatura = PropertyEntity({ val: ko.observable([]), options: EnumDiaSemana.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple, text: "Dia da Semana:", enable: ko.observable(true) });
    this.LongoCursoDiasMesFatura = PropertyEntity({ val: ko.observable([]), options: _diaMesAcordoFaturamentoCliente, def: [], getType: typesKnockout.selectMultiple, text: "Dia do Mês:", enable: ko.observable(true) });
    this.LongoCursoGerarFaturamentoAVista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gerar faturamento a vista por CT-e?", def: false, enable: ko.observable(true) });
    this.LongoCursoTipoPrazoFaturamento = PropertyEntity({ val: ko.observable(EnumTipoPrazoFaturamento.DataFatura), options: EnumTipoPrazoFaturamento.obterOpcoes(), def: EnumTipoPrazoFaturamento.DataFatura, text: "Tipo prazo faturamento:", enable: ko.observable(true) });
    this.LongoCursoDiasDePrazoFatura = PropertyEntity({ text: "Dias de prazo do faturamento:", maxlength: 2, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });
    this.LongoCursoEmail = PropertyEntity({ text: "E-mail padrão para o grupo econômico::", getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.LongoCursoNaoEnviarEmailFaturaAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não enviar por e-mail os dados da fatura de forma automática?", def: false, enable: ko.observable(true) });

    this.EmailsLongoCurso = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GridEmailsLongoCurso = PropertyEntity({ type: types.local });
    this.CodigoEmailLongoCurso = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PessoaLongoCurso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailLongoCurso = PropertyEntity({ text: "*E-mail:", getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.SalvarEmailLongoCurso = PropertyEntity({ eventClick: SalvarEmailLongoCursoClick, type: types.event, text: "Salvar E-mail" });

    //CustoExtra
    this.FaturamentoPermissaoExclusivaCustoExtra = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Faturamento será realizado apenas com permissão exclusiva?", def: false, enable: ko.observable(true) });
    this.CustoExtraDiasSemanaFatura = PropertyEntity({ val: ko.observable([]), options: EnumDiaSemana.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple, text: "Dia da Semana:", enable: ko.observable(true) });
    this.CustoExtraDiasMesFatura = PropertyEntity({ val: ko.observable([]), options: _diaMesAcordoFaturamentoCliente, def: [], getType: typesKnockout.selectMultiple, text: "Dia do Mês:", enable: ko.observable(true) });
    this.CustoExtraGerarFaturamentoAVista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gerar faturamento a vista por CT-e?", def: false, enable: ko.observable(true) });
    this.CustoExtraTipoPrazoFaturamento = PropertyEntity({ val: ko.observable(EnumTipoPrazoFaturamento.DataFatura), options: EnumTipoPrazoFaturamento.obterOpcoes(), def: EnumTipoPrazoFaturamento.DataFatura, text: "Tipo prazo faturamento:", enable: ko.observable(true) });
    this.CustoExtraDiasDePrazoFatura = PropertyEntity({ text: "Dias de prazo do faturamento:", maxlength: 2, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });
    this.CustoExtraEmail = PropertyEntity({ text: "E-mail padrão para o grupo econômico::", getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.CustoExtraNaoEnviarEmailFaturaAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não enviar por e-mail os dados da fatura de forma automática?", def: false, enable: ko.observable(true) });

    //TakeOrPay
    this.ConsiderarParametrosDeFreteCabotagem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Considerar os parâmetros de Frete Cabotagem?", def: false, enable: ko.observable(true) });
    this.TakeOrPayDiasDePrazoFatura = PropertyEntity({ text: "Dias de prazo do faturamento Embarque Certo/No Show:", maxlength: 2, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });
    this.DiasPrazoFaturamentoDnD = PropertyEntity({ text: "Dias de prazo do faturamento DnD:", maxlength: 3, getType: typesKnockout.int, val: ko.observable("30"), enable: ko.observable(false), def: 30 });
    this.DiasPrazoVencimentoNotaDebito = PropertyEntity({ text: "Dias do prazo de Vencimento para Nota de Débito:", maxlength: 3, getType: typesKnockout.int, val: ko.observable("30"), enable: ko.observable(false), def: 30 });

    this.EmailsCustoExtra = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GridEmailsCustoExtra = PropertyEntity({ type: types.local });
    this.CodigoEmailCustoExtra = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PessoaCustoExtra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailCustoExtra = PropertyEntity({ text: "*E-mail:", getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.SalvarEmailCustoExtra = PropertyEntity({ eventClick: SalvarEmailCustoExtraClick, type: types.event, text: "Salvar E-mail" });
};

var CRUDAcordoFaturamentoCliente = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadAcordoFaturamentoCliente() {
    _acordoFaturamentoCliente = new AcordoFaturamentoCliente();
    KoBindings(_acordoFaturamentoCliente, "knockoutCadastroAcordoFaturamentoCliente");

    HeaderAuditoria("AcordoFaturamentoCliente", _acordoFaturamentoCliente);

    _crudAcordoFaturamentoCliente = new CRUDAcordoFaturamentoCliente();
    KoBindings(_crudAcordoFaturamentoCliente, "knockoutCRUDAcordoFaturamentoCliente");

    _pesquisaAcordoFaturamentoCliente = new PesquisaAcordoFaturamentoCliente();
    KoBindings(_pesquisaAcordoFaturamentoCliente, "knockoutPesquisaAcordoFaturamentoCliente", false, _pesquisaAcordoFaturamentoCliente.Pesquisar.id);

    new BuscarClientes(_pesquisaAcordoFaturamentoCliente.Pessoa);
    new BuscarGruposPessoas(_pesquisaAcordoFaturamentoCliente.GrupoPessoa);

    new BuscarClientes(_acordoFaturamentoCliente.Pessoa);
    new BuscarGruposPessoas(_acordoFaturamentoCliente.GrupoPessoa);

    new BuscarClientes(_acordoFaturamentoCliente.PessoaCabotagem, null, null, null, _acordoFaturamentoCliente.GrupoPessoa);
    new BuscarClientes(_acordoFaturamentoCliente.PessoaLongoCurso, null, null, null, _acordoFaturamentoCliente.GrupoPessoa);
    new BuscarClientes(_acordoFaturamentoCliente.PessoaCustoExtra, null, null, null, _acordoFaturamentoCliente.GrupoPessoa);

    buscarAcordoFaturamentoCliente();
    ValidarPermissaoPersonalizada();

    LoadEmailCabotagem();
    LoadEmailLongoCurso();
    LoadEmailCustoExtra();
}

function LoadEmailCabotagem() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirEmailCabotagemClick, tamanho: 10 }] };
    var header = [
        { data: "CodigoEmailCabotagem", visible: false },
        { data: "CodigoPessoa", visible: false },
        { data: "Pessoa", title: "Cliente", width: "40%" },
        { data: "Email", title: "E-mail(s)", width: "40%" }
    ];
    _gridEmailsCabotagem = new BasicDataTable(_acordoFaturamentoCliente.GridEmailsCabotagem.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    RecarregarGridEmailCabotagem();
}

function RecarregarGridEmailCabotagem() {
    var data = new Array();

    $.each(_acordoFaturamentoCliente.EmailsCabotagem.list, function (i, tabela) {
        var tabelaGrid = new Object();

        tabelaGrid.CodigoEmailCabotagem = tabela.CodigoEmailCabotagem.val;
        tabelaGrid.CodigoPessoa = tabela.PessoaCabotagem.codEntity;
        tabelaGrid.Pessoa = tabela.PessoaCabotagem.val;
        tabelaGrid.Email = tabela.EmailCabotagem.val;

        data.push(tabelaGrid);
    });

    _gridEmailsCabotagem.CarregarGrid(data);
}


function SalvarEmailCabotagemClick(e, sender) {
    if (_acordoFaturamentoCliente.CabotagemEmail.val() != undefined && _acordoFaturamentoCliente.CabotagemEmail.val() != null && _acordoFaturamentoCliente.CabotagemEmail.val() != "") {
        exibirMensagem(tipoMensagem.atencao, "E-mail Padrão", "Já foi informado um e-mail padrão para a cabotagem!");
        return;
    }
    var valido = true;
    if (valido && (_acordoFaturamentoCliente.PessoaCabotagem.codEntity() == undefined || _acordoFaturamentoCliente.PessoaCabotagem.codEntity() == null || _acordoFaturamentoCliente.PessoaCabotagem.codEntity() == 0))
        valido = false;
    if (valido && (_acordoFaturamentoCliente.EmailCabotagem.val() == undefined || _acordoFaturamentoCliente.EmailCabotagem.val() == null || _acordoFaturamentoCliente.EmailCabotagem.val() == ""))
        valido = false;
    if (valido && !ValidarMultiplosEmails(_acordoFaturamentoCliente.EmailCabotagem.val()))
        valido = false;
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios e e-mail valido!");
        return;
    }


    _acordoFaturamentoCliente.CodigoEmailCabotagem.val(guid());

    var obj = new EmailCabotagemMap();

    obj.CodigoEmailCabotagem.val = 0;
    obj.PessoaCabotagem.val = _acordoFaturamentoCliente.PessoaCabotagem.val();
    obj.PessoaCabotagem.codEntity = _acordoFaturamentoCliente.PessoaCabotagem.codEntity();
    obj.EmailCabotagem.val = _acordoFaturamentoCliente.EmailCabotagem.val();

    _acordoFaturamentoCliente.EmailsCabotagem.list.push(obj);

    LimparEmailCabotagem();
}

function ExcluirEmailCabotagemClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o e-mail informado?", function () {
        for (var i = 0; i < _acordoFaturamentoCliente.EmailsCabotagem.list.length; i++) {
            if (e.CodigoEmailCabotagem === _acordoFaturamentoCliente.EmailsCabotagem.list[i].CodigoEmailCabotagem.val) {
                _acordoFaturamentoCliente.EmailsCabotagem.list.splice(i, 1);
                break;
            }
        }

        LimparEmailCabotagem();
    });
}


function LoadEmailLongoCurso() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirEmailLongoCursoClick, tamanho: 10 }] };
    var header = [
        { data: "CodigoEmailLongoCurso", visible: false },
        { data: "CodigoPessoa", visible: false },
        { data: "Pessoa", title: "Cliente", width: "40%" },
        { data: "Email", title: "E-mail(s)", width: "40%" }
    ];
    _gridEmailsLongoCurso = new BasicDataTable(_acordoFaturamentoCliente.GridEmailsLongoCurso.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    RecarregarGridEmailLongoCurso();
}

function RecarregarGridEmailLongoCurso() {
    var data = new Array();

    $.each(_acordoFaturamentoCliente.EmailsLongoCurso.list, function (i, tabela) {
        var tabelaGrid = new Object();

        tabelaGrid.CodigoEmailLongoCurso = tabela.CodigoEmailLongoCurso.val;
        tabelaGrid.CodigoPessoa = tabela.PessoaLongoCurso.codEntity;
        tabelaGrid.Pessoa = tabela.PessoaLongoCurso.val;
        tabelaGrid.Email = tabela.EmailLongoCurso.val;

        data.push(tabelaGrid);
    });

    _gridEmailsLongoCurso.CarregarGrid(data);
}


function SalvarEmailLongoCursoClick(e, sender) {
    if (_acordoFaturamentoCliente.LongoCursoEmail.val() != undefined && _acordoFaturamentoCliente.LongoCursoEmail.val() != null && _acordoFaturamentoCliente.LongoCursoEmail.val() != "") {
        exibirMensagem(tipoMensagem.atencao, "E-mail Padrão", "Já foi informado um e-mail padrão para o longo curso!");
        return;
    }

    var valido = true;
    if (valido && (_acordoFaturamentoCliente.PessoaLongoCurso.codEntity() == undefined || _acordoFaturamentoCliente.PessoaLongoCurso.codEntity() == null || _acordoFaturamentoCliente.PessoaLongoCurso.codEntity() == 0))
        valido = false;
    if (valido && (_acordoFaturamentoCliente.EmailLongoCurso.val() == undefined || _acordoFaturamentoCliente.EmailLongoCurso.val() == null || _acordoFaturamentoCliente.EmailLongoCurso.val() == ""))
        valido = false;
    if (valido && !ValidarMultiplosEmails(_acordoFaturamentoCliente.EmailLongoCurso.val()))
        valido = false;
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios e e-mail valido!");
        return;
    }

    _acordoFaturamentoCliente.CodigoEmailLongoCurso.val(guid());

    var obj = new EmailLongoCursoMap();

    obj.CodigoEmailLongoCurso.val = 0;
    obj.PessoaLongoCurso.val = _acordoFaturamentoCliente.PessoaLongoCurso.val();
    obj.PessoaLongoCurso.codEntity = _acordoFaturamentoCliente.PessoaLongoCurso.codEntity();
    obj.EmailLongoCurso.val = _acordoFaturamentoCliente.EmailLongoCurso.val();

    _acordoFaturamentoCliente.EmailsLongoCurso.list.push(obj);

    LimparEmailLongoCurso();
}

function ExcluirEmailLongoCursoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o e-mail informado?", function () {
        for (var i = 0; i < _acordoFaturamentoCliente.EmailsLongoCurso.list.length; i++) {
            if (e.CodigoEmailLongoCurso === _acordoFaturamentoCliente.EmailsLongoCurso.list[i].CodigoEmailLongoCurso.val) {
                _acordoFaturamentoCliente.EmailsLongoCurso.list.splice(i, 1);
                break;
            }
        }

        LimparEmailLongoCurso();
    });
}


function LoadEmailCustoExtra() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirEmailCustoExtraClick, tamanho: 10 }] };
    var header = [
        { data: "CodigoEmailCustoExtra", visible: false },
        { data: "CodigoPessoa", visible: false },
        { data: "Pessoa", title: "Cliente", width: "40%" },
        { data: "Email", title: "E-mail(s)", width: "40%" }
    ];
    _gridEmailsCustoExtra = new BasicDataTable(_acordoFaturamentoCliente.GridEmailsCustoExtra.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    RecarregarGridEmailCustoExtra();
}

function RecarregarGridEmailCustoExtra() {
    var data = new Array();

    $.each(_acordoFaturamentoCliente.EmailsCustoExtra.list, function (i, tabela) {
        var tabelaGrid = new Object();

        tabelaGrid.CodigoEmailCustoExtra = tabela.CodigoEmailCustoExtra.val;
        tabelaGrid.CodigoPessoa = tabela.PessoaCustoExtra.codEntity;
        tabelaGrid.Pessoa = tabela.PessoaCustoExtra.val;
        tabelaGrid.Email = tabela.EmailCustoExtra.val;

        data.push(tabelaGrid);
    });

    _gridEmailsCustoExtra.CarregarGrid(data);
}


function SalvarEmailCustoExtraClick(e, sender) {
    if (_acordoFaturamentoCliente.CustoExtraEmail.val() != undefined && _acordoFaturamentoCliente.CustoExtraEmail.val() != null && _acordoFaturamentoCliente.CustoExtraEmail.val() != "") {
        exibirMensagem(tipoMensagem.atencao, "E-mail Padrão", "Já foi informado um e-mail padrão para o custo extra!");
        return;
    }

    var valido = true;
    if (valido && (_acordoFaturamentoCliente.PessoaCustoExtra.codEntity() == undefined || _acordoFaturamentoCliente.PessoaCustoExtra.codEntity() == null || _acordoFaturamentoCliente.PessoaCustoExtra.codEntity() == 0))
        valido = false;
    if (valido && (_acordoFaturamentoCliente.EmailCustoExtra.val() == undefined || _acordoFaturamentoCliente.EmailCustoExtra.val() == null || _acordoFaturamentoCliente.EmailCustoExtra.val() == ""))
        valido = false;
    if (valido && !ValidarMultiplosEmails(_acordoFaturamentoCliente.EmailCustoExtra.val()))
        valido = false;
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios e e-mail valido!");
        return;
    }

    _acordoFaturamentoCliente.CodigoEmailCustoExtra.val(guid());

    var obj = new EmailCustoExtraMap();

    obj.CodigoEmailCustoExtra.val = 0;
    obj.PessoaCustoExtra.val = _acordoFaturamentoCliente.PessoaCustoExtra.val();
    obj.PessoaCustoExtra.codEntity = _acordoFaturamentoCliente.PessoaCustoExtra.codEntity();
    obj.EmailCustoExtra.val = _acordoFaturamentoCliente.EmailCustoExtra.val();

    _acordoFaturamentoCliente.EmailsCustoExtra.list.push(obj);

    LimparEmailCustoExtra();
}

function ExcluirEmailCustoExtraClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o e-mail informado?", function () {
        for (var i = 0; i < _acordoFaturamentoCliente.EmailsCustoExtra.list.length; i++) {
            if (e.CodigoEmailCustoExtra === _acordoFaturamentoCliente.EmailsCustoExtra.list[i].CodigoEmailCustoExtra.val) {
                _acordoFaturamentoCliente.EmailsCustoExtra.list.splice(i, 1);
                break;
            }
        }

        LimparEmailCustoExtra();
    });
}

function adicionarClick(e, sender) {
    Salvar(_acordoFaturamentoCliente, "AcordoFaturamentoCliente/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridAcordoFaturamentoCliente.CarregarGrid();
                limparCamposAcordoFaturamentoCliente();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_acordoFaturamentoCliente, "AcordoFaturamentoCliente/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAcordoFaturamentoCliente.CarregarGrid();
                limparCamposAcordoFaturamentoCliente();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Acordo de Faturamento Cliente?", excluido() 
    );
}

function excluido() {   
    ExcluirPorCodigo(_acordoFaturamentoCliente, "AcordoFaturamentoCliente/ExcluirPorCodigo", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
            _gridAcordoFaturamentoCliente.CarregarGrid();
            limparCamposAcordoFaturamentoCliente();

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function cancelarClick(e) {
    limparCamposAcordoFaturamentoCliente();
}

function tipoPessoaAcordoFaturamentoClienteChange() {
    if (_acordoFaturamentoCliente.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        _acordoFaturamentoCliente.GrupoPessoa.visible(false);
        _acordoFaturamentoCliente.GrupoPessoa.required(false);
        _acordoFaturamentoCliente.Pessoa.visible(true);
        _acordoFaturamentoCliente.Pessoa.required(true);
        LimparCampoEntity(_acordoFaturamentoCliente.GrupoPessoa);
    }
    else {
        _acordoFaturamentoCliente.GrupoPessoa.visible(true);
        _acordoFaturamentoCliente.GrupoPessoa.required(true);
        _acordoFaturamentoCliente.Pessoa.visible(false);
        _acordoFaturamentoCliente.Pessoa.required(false);
        LimparCampoEntity(_acordoFaturamentoCliente.Pessoa);
    }
}

//*******MÉTODOS*******


function buscarAcordoFaturamentoCliente() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAcordoFaturamentoCliente, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAcordoFaturamentoCliente = new GridView(_pesquisaAcordoFaturamentoCliente.Pesquisar.idGrid, "AcordoFaturamentoCliente/Pesquisa", _pesquisaAcordoFaturamentoCliente, menuOpcoes, null);
    _gridAcordoFaturamentoCliente.CarregarGrid();
}

function editarAcordoFaturamentoCliente(acordoFaturamentoClienteGrid) {
    limparCamposAcordoFaturamentoCliente();
    _acordoFaturamentoCliente.Codigo.val(acordoFaturamentoClienteGrid.Codigo);
    BuscarPorCodigo(_acordoFaturamentoCliente, "AcordoFaturamentoCliente/BuscarPorCodigo", function (arg) {
        _pesquisaAcordoFaturamentoCliente.ExibirFiltros.visibleFade(false);
        _crudAcordoFaturamentoCliente.Atualizar.visible(true);
        _crudAcordoFaturamentoCliente.Cancelar.visible(true);
        _crudAcordoFaturamentoCliente.Excluir.visible(true);
        _crudAcordoFaturamentoCliente.Adicionar.visible(false);

        tipoPessoaAcordoFaturamentoClienteChange();
        ValidarPermissaoPersonalizada();
        LimparEmailCabotagem();
        LimparEmailLongoCurso();
        LimparEmailCustoExtra();
    }, null);
}

function limparCamposAcordoFaturamentoCliente() {
    _crudAcordoFaturamentoCliente.Atualizar.visible(false);
    _crudAcordoFaturamentoCliente.Cancelar.visible(false);
    _crudAcordoFaturamentoCliente.Excluir.visible(false);
    _crudAcordoFaturamentoCliente.Adicionar.visible(true);
    LimparCampos(_acordoFaturamentoCliente);

    tipoPessoaAcordoFaturamentoClienteChange();
    ValidarPermissaoPersonalizada();

    LimparEmailCabotagem();
    LimparEmailLongoCurso();
    LimparEmailCustoExtra();
    resetarTabs();
}

function LimparEmailCabotagem() {
    LimparCampoEntity(_acordoFaturamentoCliente.PessoaCabotagem);
    _acordoFaturamentoCliente.EmailCabotagem.val("");
    _acordoFaturamentoCliente.CodigoEmailCabotagem.val("");
    RecarregarGridEmailCabotagem();
}

function LimparEmailLongoCurso() {
    LimparCampoEntity(_acordoFaturamentoCliente.PessoaLongoCurso);
    _acordoFaturamentoCliente.EmailLongoCurso.val("");
    _acordoFaturamentoCliente.CodigoEmailLongoCurso.val("");
    RecarregarGridEmailLongoCurso();
}

function LimparEmailCustoExtra() {
    LimparCampoEntity(_acordoFaturamentoCliente.PessoaCustoExtra);
    _acordoFaturamentoCliente.EmailCustoExtra.val("");
    _acordoFaturamentoCliente.CodigoEmailCustoExtra.val("");
    RecarregarGridEmailCustoExtra();
}

function resetarTabs() {
    $("#tabCabotagem a:eq(0)").tab("show");
    $("#tabLongoCurso a:eq(0)").tab("show");
    $("#tabCustoExtra a:eq(0)").tab("show");
}

function ValidarPermissaoPersonalizada() {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.AcordoFaturamento_PermiteAlterarApenasEmail, _PermissoesPersonalizadas)) {
        SetarEnableCamposKnockout(_acordoFaturamentoCliente, false);
        _acordoFaturamentoCliente.CabotagemEmail.enable(true);
        _acordoFaturamentoCliente.LongoCursoEmail.enable(true);
        _acordoFaturamentoCliente.CustoExtraEmail.enable(true);
    }
}