/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumOcorrenciaGeracaoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoGeracaoRelatorio.js" />

// #region Objetos Globais do Arquivo

var _diaGeracaoRelatorio = Object.freeze([
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
]);

var _diaUteisGeracaoRelatorio = Object.freeze([
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
    { text: "20", value: 20 }
]);

// #endregion Objetos Globais do Arquivo

// #region Classes

var AutomatizarGeracaoRelatorioCadastro = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", enable: ko.observable(true), required: true, maxlength: 200 });
    this.DiaGeracao = PropertyEntity({ text: "*Dia:", val: ko.observable(1), def: 1, options: ko.observable(_diaGeracaoRelatorio), required: true, enable: ko.observable(true) });
    this.HoraGeracao = PropertyEntity({ text: "*Hora:", getType: typesKnockout.time, required: true, enable: ko.observable(true) });
    this.OcorrenciaGeracao = PropertyEntity({ text: "*Ocorrência:", val: ko.observable(EnumOcorrenciaGeracaoRelatorio.Diario), def: EnumOcorrenciaGeracaoRelatorio.Diario, options: EnumOcorrenciaGeracaoRelatorio.obterOpcoes(), required: true, enable: ko.observable(true) });
    this.TipoArquivo = PropertyEntity({ text: "*Tipo do Arquivo:", val: ko.observable(EnumTipoArquivoGeracaoRelatorio.Pdf), def: EnumTipoArquivoGeracaoRelatorio.Pdf, options: EnumTipoArquivoGeracaoRelatorio.obterOpcoes(), required: true, enable: ko.observable(true) });
    this.GerarSegunda = PropertyEntity({ getType: typesKnockout.bool, text: "Segunda-Feira", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarTerca = PropertyEntity({ getType: typesKnockout.bool, text: "Terça-Feira", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarQuarta = PropertyEntity({ getType: typesKnockout.bool, text: "Quarta-Feira", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarQuinta = PropertyEntity({ getType: typesKnockout.bool, text: "Quinta-Feira", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarSexta = PropertyEntity({ getType: typesKnockout.bool, text: "Sexta-Feira", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarSabado = PropertyEntity({ getType: typesKnockout.bool, text: "Sábado", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarDomingo = PropertyEntity({ getType: typesKnockout.bool, text: "Domingo", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.GerarSomenteEmDiaUtil = PropertyEntity({ getType: typesKnockout.bool, text: "Somente em Dia Útil", val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.EnviarPorEmail = PropertyEntity({ getType: typesKnockout.bool, text: "Envio por E-mail", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.Email = PropertyEntity({ text: ko.observable("E-mail:"), textDefault: "E-mail:", getType: typesKnockout.multiplesEmails, required: ko.observable(false), enable: ko.observable(true), maxlength: 1000 });

    this.EnviarParaFTP = PropertyEntity({ getType: typesKnockout.bool, text: "Envio para FTP", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.EnderecoFTP = PropertyEntity({ text: ko.observable("Endereço:"), textDefault: "Endereço:", maxlength: 150, required: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.Usuario = PropertyEntity({ text: ko.observable("Usuário:"), textDefault: "Usuário:", maxlength: 50, required: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.Senha = PropertyEntity({ text: ko.observable("Senha:"), textDefault: "Senha:", maxlength: 50, required: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.Diretorio = PropertyEntity({ text: ko.observable("Diretório:"), textDefault: "Diretório:", maxlength: 150, required: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.Porta = PropertyEntity({ text: ko.observable("Porta:"), textDefault: "Porta:", maxlength: 10, required: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.Passivo = PropertyEntity({ text: "FTP Passivo", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: ko.observable(false), enable: ko.observable(true) });
    this.UtilizarSFTP = PropertyEntity({ text: "SFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: ko.observable(false), enable: ko.observable(true) });
    this.SSL = PropertyEntity({ text: "SSL", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: ko.observable(false), enable: ko.observable(true) });
    this.Nomenclatura = PropertyEntity({ text: "Nomenclatura do arquivo:", enable: ko.observable(true) });
    this.TagAno = PropertyEntity({ eventClick: function (e) { InserirTag(self.Nomenclatura.id, "#Ano#"); }, type: types.event, text: "Ano", visible: ko.observable(true), enable: ko.observable(true) });
    this.TagMes = PropertyEntity({ eventClick: function (e) { InserirTag(self.Nomenclatura.id, "#Mes#"); }, type: types.event, text: "Mês", visible: ko.observable(true), enable: ko.observable(true) });
    this.TagDia = PropertyEntity({ eventClick: function (e) { InserirTag(self.Nomenclatura.id, "#Dia#"); }, type: types.event, text: "Dia", visible: ko.observable(true), enable: ko.observable(true) });
    this.TagHora = PropertyEntity({ eventClick: function (e) { InserirTag(self.Nomenclatura.id, "#Hora#"); }, type: types.event, text: "Hora", visible: ko.observable(true), enable: ko.observable(true) });
    this.TagMinutos = PropertyEntity({ eventClick: function (e) { InserirTag(self.Nomenclatura.id, "#Minutos#"); }, type: types.event, text: "Minutos", visible: ko.observable(true), enable: ko.observable(true) });
    this.TagSegundos = PropertyEntity({ eventClick: function (e) { InserirTag(self.Nomenclatura.id, "#Segundos#"); }, type: types.event, text: "Segundos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Email.required.subscribe(function (novoValor) { self.Email.text((novoValor ? "*" : "") + self.Email.textDefault); });
    this.EnderecoFTP.required.subscribe(function (novoValor) { self.EnderecoFTP.text((novoValor ? "*" : "") + self.EnderecoFTP.textDefault); });
    this.Usuario.required.subscribe(function (novoValor) { self.Usuario.text((novoValor ? "*" : "") + self.Usuario.textDefault); });
    this.Senha.required.subscribe(function (novoValor) { self.Senha.text((novoValor ? "*" : "") + self.Senha.textDefault); });
    this.Diretorio.required.subscribe(function (novoValor) { self.Diretorio.text((novoValor ? "*" : "") + self.Diretorio.textDefault); });
    this.Porta.required.subscribe(function (novoValor) { self.Porta.text((novoValor ? "*" : "") + self.Porta.textDefault); });

    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var AutomatizarGeracaoRelatorioContainer = function () {
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(false), idGrid: guid() });
}

var AutomatizarGeracaoRelatorioPesquisa = function () {
    this.CodigoControleRelatorios = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var AutomatizarGeracaoRelatorio = function (idConteudoRelatorio, relatorioGlobal) {
    this._automatizarGeracaoRelatorioCadastro;
    this._automatizarGeracaoRelatorioContainer;
    this._automatizarGeracaoRelatorioPesquisa;
    this._idCadastro = idConteudoRelatorio + "-modal-automatizar-geracao-relatorio-cadastro";
    this._idContainer = idConteudoRelatorio + "-modal-automatizar-geracao-relatorio";
    this._permitirEdicao = false;
    this._relatorioGlobal = relatorioGlobal;
    this._grid;
    this._urlRelatorio;
    this._modalAutomatizacaoRelatorio;
}

AutomatizarGeracaoRelatorio.prototype = {
    carregar: function (codigoControleRelatorios) {
        var self = this;

        self._urlRelatorio = self._relatorioGlobal.obterUrl();

        self._automatizarGeracaoRelatorioPesquisa = new AutomatizarGeracaoRelatorioPesquisa();
        self._automatizarGeracaoRelatorioPesquisa.CodigoControleRelatorios.val(codigoControleRelatorios);

        self._automatizarGeracaoRelatorioContainer = new AutomatizarGeracaoRelatorioContainer();
        self._automatizarGeracaoRelatorioContainer.Adicionar.eventClick = function () { self._exibirModalAutomatizacaoAdicionar(); };
        KoBindings(self._automatizarGeracaoRelatorioContainer, self._idContainer, false);

        self._automatizarGeracaoRelatorioCadastro = new AutomatizarGeracaoRelatorioCadastro();
        self._automatizarGeracaoRelatorioCadastro.GerarSomenteEmDiaUtil.val.subscribe(function () { self._controlarDiaGeracao(); });
        self._automatizarGeracaoRelatorioCadastro.EnviarPorEmail.val.subscribe(function () { self._controlarCamposEnvioPorEmailHabilitados(); });
        self._automatizarGeracaoRelatorioCadastro.EnviarParaFTP.val.subscribe(function () { self._controlarCamposEnvioParaFTPHabilitados(); });
        self._automatizarGeracaoRelatorioCadastro.Adicionar.eventClick = function () { self._adicionarAutomatizacao(); };
        self._automatizarGeracaoRelatorioCadastro.Atualizar.eventClick = function () { self._atualizarAutomatizacao(); };
        self._automatizarGeracaoRelatorioCadastro.Excluir.eventClick = function () { self._excluirAutomatizacao(); };

        KoBindings(self._automatizarGeracaoRelatorioCadastro, self._idCadastro, false);

        self._carregarGrid();
    },
    exibirModal: function () {
        var self = this;
        var gridPreview = self._relatorioGlobal.obterGridPreview();

        self._permitirEdicao = Boolean(gridPreview) && Boolean(gridPreview.GridViewTable());
        self._automatizarGeracaoRelatorioContainer.Adicionar.visible(self._permitirEdicao);
        self._grid.CarregarGrid();

        Global.abrirModal(self._idContainer);
    },
    _adicionarAutomatizacao: function () {
        var self = this;
        var automatizacaoSalvar = self._obterAutomatizacaoSalvar();

        if (!automatizacaoSalvar)
            return;

        executarReST(self._urlRelatorio + "/AdicionarAutomatizacaoGeracao", automatizacaoSalvar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    self._fecharModalAutomatizacao();
                    self._grid.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    },
    _atualizarAutomatizacao: function () {
        var self = this;
        var automatizacaoSalvar = self._obterAutomatizacaoSalvar();

        if (!automatizacaoSalvar)
            return;

        executarReST(self._urlRelatorio + "/AtualizarAutomatizacaoGeracao", automatizacaoSalvar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    self._fecharModalAutomatizacao();
                    self._grid.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    },
    _carregarGrid: function () {
        var self = this;
        var opcaoDetalhes = { descricao: "Editar", id: guid(), metodo: function (registroSelecionado) { self._exibirModalAutomatizacaoEditar(registroSelecionado); } };
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoDetalhes] };

        self._grid = new GridView(self._automatizarGeracaoRelatorioContainer.Adicionar.idGrid, self._urlRelatorio + "/PesquisaAutomatizacaoGeracao", self._automatizarGeracaoRelatorioPesquisa, menuOpcoes);
    },
    _controlarCamposEnvioParaFTPHabilitados: function () {
        var enviarParaFTP = this._automatizarGeracaoRelatorioCadastro.EnviarParaFTP.val();
        var habilitarEdicao = enviarParaFTP && this._permitirEdicao;

        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.EnderecoFTP, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.Usuario, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.Senha, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.Diretorio, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.Porta, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.Passivo, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.UtilizarSFTP, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.SSL, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.Nomenclatura, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.TagAno, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.TagMes, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.TagDia, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.TagHora, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.TagMinutos, habilitarEdicao);
        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.TagSegundos, habilitarEdicao);

        this._automatizarGeracaoRelatorioCadastro.EnderecoFTP.required(enviarParaFTP);
        this._automatizarGeracaoRelatorioCadastro.Usuario.required(enviarParaFTP);
        this._automatizarGeracaoRelatorioCadastro.Senha.required(enviarParaFTP);
        this._automatizarGeracaoRelatorioCadastro.Diretorio.required(enviarParaFTP);
        this._automatizarGeracaoRelatorioCadastro.Porta.required(enviarParaFTP);
    },
    _controlarCamposEnvioPorEmailHabilitados: function () {
        var enviarPorEmail = this._automatizarGeracaoRelatorioCadastro.EnviarPorEmail.val();
        var habilitarEdicao = enviarPorEmail && this._permitirEdicao;

        SetarEnableCampo(this._automatizarGeracaoRelatorioCadastro.Email, habilitarEdicao);

        this._automatizarGeracaoRelatorioCadastro.Email.required(enviarPorEmail);
    },
    _controlarDiaGeracao: function () {
        if (this._automatizarGeracaoRelatorioCadastro.GerarSomenteEmDiaUtil.val())
            this._automatizarGeracaoRelatorioCadastro.DiaGeracao.options(_diaUteisGeracaoRelatorio);
        else
            this._automatizarGeracaoRelatorioCadastro.DiaGeracao.options(_diaGeracaoRelatorio);
    },
    _excluirAutomatizacao: function () {
        var self = this;

        exibirConfirmacao("Confirmação", "Realmente deseja excluir a automatização de geração do relatório?", function () {
            executarReST(self._urlRelatorio + "/ExcluirAutomatizacaoGeracao", { Codigo: self._automatizarGeracaoRelatorioCadastro.Codigo.val() }, function (retorno) {
                if (retorno.Success) {
                    self._fecharModalAutomatizacao();
                    self._grid.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _exibirModalAutomatizacao: function () {
        var self = this;
        var isEdicao = self._automatizarGeracaoRelatorioCadastro.Codigo.val() > 0;

        self._automatizarGeracaoRelatorioCadastro.Adicionar.visible(!isEdicao && self._permitirEdicao);
        self._automatizarGeracaoRelatorioCadastro.Atualizar.visible(isEdicao && self._permitirEdicao);
        self._automatizarGeracaoRelatorioCadastro.Excluir.visible(isEdicao && self._permitirEdicao);

        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.Descricao, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.DiaGeracao, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.HoraGeracao, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.OcorrenciaGeracao, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.TipoArquivo, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarSegunda, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarTerca, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarQuarta, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarQuinta, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarSexta, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarSabado, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarDomingo, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.GerarSomenteEmDiaUtil, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.EnviarPorEmail, self._permitirEdicao);
        SetarEnableCampo(self._automatizarGeracaoRelatorioCadastro.EnviarParaFTP, self._permitirEdicao);

        self._controlarCamposEnvioPorEmailHabilitados();
        self._controlarCamposEnvioParaFTPHabilitados();

        Global.abrirModal(self._idCadastro);
        $('#' + self._idCadastro).one("hidden.bs.modal", function () {
            LimparCampos(self._automatizarGeracaoRelatorioCadastro);
        });
    },
    _exibirModalAutomatizacaoAdicionar: function () {
        this._exibirModalAutomatizacao();
    },
    _exibirModalAutomatizacaoEditar: function (registroSelecionado) {
        var self = this;

        executarReST(self._urlRelatorio + "/ObterAutomatizacaoGeracaoPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                PreencherObjetoKnout(self._automatizarGeracaoRelatorioCadastro, retorno);
                self._exibirModalAutomatizacao();
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    },
    _fecharModalAutomatizacao: function () {
        var self = this;
        Global.fecharModal(self._idCadastro);
    },
    _obterAutomatizacaoSalvar: function () {
        var self = this;

        if (!ValidarCamposObrigatorios(self._automatizarGeracaoRelatorioCadastro)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
            return undefined;
        }

        if (!ValidarMultiplosEmails(self._automatizarGeracaoRelatorioCadastro.Email.val())) {
            exibirMensagem(tipoMensagem.atencao, "E-mails Inválidos", "Por favor, verifique os e-mails informados!");
            return;
        }

        var gridPreview = self._relatorioGlobal.obterGridPreview();
        var knoutPesquisa = self._relatorioGlobal.obterKnoutPesquisa();
        var knoutRelatorio = self._relatorioGlobal.obterKnoutRelatorio();

        if ((knoutRelatorio.Grid.val() != null) && (gridPreview != null))
            knoutRelatorio.Grid.val(gridPreview.GetGrid());

        $.each(knoutPesquisa, function (i, prop) {
            if (prop.getType == typesKnockout.report) {
                knoutPesquisa[i].val(RetornarObjetoPesquisa(knoutRelatorio));
                return false;
            }
        });

        var automatizacaoSalvar = RetornarObjetoPesquisa(knoutPesquisa);

        automatizacaoSalvar['Automatizacao'] = JSON.stringify(RetornarObjetoPesquisa(self._automatizarGeracaoRelatorioCadastro));

        return automatizacaoSalvar;
    }
}

// #endregion Classes
