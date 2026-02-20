var CartaCorrecaoCTe = function () {
    var self = this;

    self.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    self.CTe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    self.NumeroCTe = PropertyEntity({ text: "CT-e:" });
    self.Remetente = PropertyEntity({ text: "Remetente:" });
    self.Destinatario = PropertyEntity({ text: "Destinatário:" });
    self.Tomador = PropertyEntity({ text: "Tomador:" });
    self.Origem = PropertyEntity({ text: "Origem:" });
    self.Destino = PropertyEntity({ text: "Destino:" });
    self.ValorReceber = PropertyEntity({ text: "Valor a Receber:" });

    self.Pesquisar = PropertyEntity({ idGrid: guid(), eventClick: self._pesquisarClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    self.Adicionar = PropertyEntity({ eventClick: self._adicionarClick, type: types.event, text: "Nova Carta de Correção", visible: ko.observable(true), enable: ko.observable(true) });

    self.CodigoMDFe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    self.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });
    self.PesquisarMDFe = PropertyEntity({ eventClick: self._carregarIntegracoesMDFeManual, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    self.ReenviarTodos = PropertyEntity({ eventClick: self._reenviarTodasIntegracoesClick, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
    self.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    self.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    self.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    self.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    self.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    self.ObterTotais = PropertyEntity({ eventClick: self._carregarTotaisIntegracaoMDFeManual, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });

    self._gridIntegracoesCartaCorrecao = null;
    self._gridHistoricoIntegracaoCartaCorrecao = null;
    self._grid = null;
    self._html = null;
    self._id = guid();
};

CartaCorrecaoCTe.prototype = {
    _pesquisarClick: function () {
        this._grid.CarregarGrid();
    },
    _adicionarClick: function () {
        var self = this;

        if (self.CartaCorrecao == null)
            self.CartaCorrecao = new CartaCorrecao(self).Load(self.CTe.val(), 0);
        else
            self.CartaCorrecao.Load(self.CTe.val(), 0);
    },
    _sincronizarCCeClick: function (e) {
        var self = this;

        if (e.Status === EnumStatusCCe.Enviado) {
            var data = { Codigo: e.Codigo };
            executarReST("CartaCorrecao/SincronizarDocumentoEmProcessamento", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        self._pesquisarClick();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    },
    _editarClick: function (data) {
        var self = this;

        if (self.CartaCorrecao == null)
            self.CartaCorrecao = new CartaCorrecao(self).Load(self.CTe.val(), data.Codigo);
        else
            self.CartaCorrecao.Load(self.CTe.val(), data.Codigo);
    },
    _loadGrid: function () {
        var self = this;

        if (self._grid == null) { 
            let sincronizarDocumento = { descricao: "Sincronizar Documento", id: guid(), evento: "onclick", metodo: function (data) { self._sincronizarCCeClick(data); }, tamanho: "20", icone: "", visibilidade: function (data) { return self._visibilidadeSincronizarDocumento(data); } };
            var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { self._editarClick(data); }, tamanho: "20", icone: "" };
            var downloadPDF = { descricao: "Download PDF", id: guid(), evento: "onclick", metodo: function (data) { self._downloadPDFClick(data); }, tamanho: "20", icone: "", visibilidade: function (data) { return self._visibilidadeDownload(data); } };
            var downloadXML = { descricao: "Download XML", id: guid(), evento: "onclick", metodo: function (data) { self._downloadXMLClick(data); }, tamanho: "20", icone: "", visibilidade: function (data) { return self._visibilidadeDownload(data); } };
            var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [sincronizarDocumento, editar, downloadPDF, downloadXML], tamanho: 7 };

            this._grid = new GridView(self.Pesquisar.idGrid, "CartaCorrecao/Pesquisa", self, menuOpcoes, { column: 2, dir: orderDir.desc }, null, null);
        }
    },
    _loadGridCartaCorrecaoManualIntegracoes: function () {
        var self = this;

        if (!self._gridIntegracoesCartaCorrecao) {
            var linhasPorPaginas = 5;
            var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Integrar, id: guid(), metodo: self._integrarClick, icone: "" };
            var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: self._exibirHistoricoIntegracoesClick, icone: "" };
            var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] };

            this._gridIntegracoesCartaCorrecao = new GridView(self.PesquisarMDFe.idGrid, "CargaCartaCorrecaoIntegracao/PesquisaCartaCorrecaoIntegracoes", self, menuOpcoes, null, linhasPorPaginas);
            this._gridIntegracoesCartaCorrecao.CarregarGrid();
        }
    },
    _downloadPDFClick: function (data) {
        executarDownload("CartaCorrecao/DownloadPDF", { Codigo: data.Codigo });
    },
    _downloadXMLClick: function (data) {
        executarDownload("CartaCorrecao/DownloadXML", { Codigo: data.Codigo });
    },
    _visibilidadeDownload: function (data) {
        return data.Status === EnumStatusCCe.Autorizado;
    },
    _visibilidadeSincronizarDocumento: function (data) {
        return data.HabilitarSincronizarDocumento === true;
    },
    _loadHTML: function () {
        var self = this;
        var promiseHTML = new promise.Promise();
        if (string.IsNullOrWhiteSpace(self._html)) {
            $.get("Content/Static/CTe/CartaCorrecaoCTe.html?dyn=" + guid(), function (data) {
                self._html = data;
                promiseHTML.done();
            });
        } else {
            promiseHTML.done();
        }
        return promiseHTML;
    },
    _vincularHTML: function () {
        var self = this;

        if (!$("#" + self._id).length) {
            var html = self._html.replace(/#divModalCartaCorrecaoCTe/g, self._id);

            $('#js-page-content').append(html);
        }
    },
    _abrirModal: function () {
        Global.abrirModal(this._id);
    },
    _bind: function () {
        var self = this;

        LocalizeCurrentPage();
        self._vincularHTML();
        KoBindings(self, self._id);
        self._loadGrid();
    },
    _carregarDadosGerais: function () {
        var self = this;
        var promiseHTML = new promise.Promise();

        BuscarPorCodigo(self, "CartaCorrecao/ObterDetalhesCTe", function (r) {
            if (r.Success) {
                if (r.Data) {
                    promiseHTML.done();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Atenção", r.Msg);
            }
        });

        return promiseHTML;
    },
    _carregarTotaisIntegracaoMDFeManual: function () {
        var self = this;

        executarReST("CargaCartaCorrecaoIntegracao/ObterTotaisIntegracoes", { Codigo: self.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                self.TotalGeral.val(retorno.Data.TotalGeral);
                self.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
                self.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
                self.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
                self.TotalIntegrado.val(retorno.Data.TotalIntegrado);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    },
    _reenviarTodasIntegracoesClick: function () {
        var self = this;

        executarReST("CargaCartaCorrecaoIntegracao/ReenviarTodos", { Codigo: self.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    self._carregarIntegracoesMDFeManual;
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    },
    _carregarIntegracoesMDFeManual: function () {
        var self = this;

        self._gridIntegracoesCartaCorrecao.CarregarGrid();

        self._carregarTotaisIntegracaoMDFeManual();
    },
    _integrarClick: function (registroSelecionado) {
        var self = this;

        executarReST("CargaCartaCorrecaoIntegracao/Reenviar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    self._carregarIntegracoesMDFeManual;
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    },
    _exibirHistoricoIntegracoesClick(integracao) {
        var self = this;

        self._BuscarHistoricoIntegracao(integracao);
        Global.abrirModal('divModalHistoricoIntegracao');
    },
    _BuscarHistoricoIntegracao(integracao) {
        var self = this;

        self.CodigoMDFe.val(integracao.Codigo);

        if (self._gridHistoricoIntegracaoCartaCorrecao == null) {
            var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: self._DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };
            var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [download] };

            this._gridHistoricoIntegracaoCartaCorrecao = new GridView("tblHistoricoIntegracao", "CargaCartaCorrecaoIntegracao/ConsultarHistoricoIntegracao", self, menuOpcoes, { column: 1, dir: orderDir.desc });
            this._gridHistoricoIntegracaoCartaCorrecao.CarregarGrid();
        }
    },
    _DownloadArquivosHistoricoIntegracaoClick: function (historicoConsulta) {
        executarDownload("CargaCartaCorrecaoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
    },
    _VisibilidadeAbaIntegracao: function () {
        executarReST("CargaCartaCorrecaoIntegracao/ObterConfiguracoesIntegracaoIntercab", null, function (retorno) {
            if (retorno.Data.AtivarIntegracaoCartaCorrecao)
                $("#liTabIntegracoes").removeClass("d-none");
        });
    },
    Load: function (codigoCTe) {
        var self = this;
        self.Codigo.val(codigoCTe);
        self._carregarDadosGerais().then(function () {
            self._loadHTML().then(function () {
                self._bind();
                self._loadGridCartaCorrecaoManualIntegracoes();
                self._VisibilidadeAbaIntegracao();
                LocalizeCurrentPage();
                self._grid.CarregarGrid().then(function () {
                    self._abrirModal();
                });
            });
        });
    }
};