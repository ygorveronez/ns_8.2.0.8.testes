/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
var _abertura;
var _analise;

var ControleEntregaDevolucaoNotaFiscal = function (dadosNotaFiscal) {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Codigo), def: dadosNotaFiscal.Codigo, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Numero) });
    this.Serie = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Serie) });
    this.DataEmissao = PropertyEntity({ val: ko.observable(dadosNotaFiscal.DataEmissao) });
    this.Volume = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Volume) });
    this.Valor = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Valor) });
    this.Peso = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Peso) });
    this.Filial = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Filial) });
    this.Chave = PropertyEntity({ val: ko.observable(dadosNotaFiscal.Chave) });

    this.DevolucaoParcial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(dadosNotaFiscal.DevolucaoParcial) });
    this.DevolucaoTotal = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(dadosNotaFiscal.DevolucaoTotal) });
    this.PossuiProdutos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.GridProdutos;
    this.ProdutosProdutos = [];
    this.ItensDevolucao = [];
    this.GridDevolucao;
    this.ProcessarItensDevolucao = PropertyEntity({ eventClick: ko.observable(null) });

    this.CodigoProduto = PropertyEntity({ text: "Código do produto:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DescricaoProduto = PropertyEntity({ text: "Descrição do produto:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.ExibirApenasProdutosComQuantidadeMaiorQueZero = PropertyEntity({ text: "Exibir apenas produtos com quantidade maior que 0", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.MotivoDaDevolucao = PropertyEntity({ type: types.entity, codEntity: ko.observable(dadosNotaFiscal.MotivoDaDevolucao.Codigo), val: ko.observable(dadosNotaFiscal.MotivoDaDevolucao.Descricao), text: ko.observable("Motivo da devolução"), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.SituacaoNotaFiscal = PropertyEntity({ val: ko.observable(dadosNotaFiscal.SituacaoNotaFiscal) });

    this.QuantidadeDevolvida = PropertyEntity({ text: "Quantidade já Devolvida:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.QuantidadeDevolucao = PropertyEntity({ text: "Quantidade Devolução:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorDevolucao = PropertyEntity({ text: "Valor Devolução:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.NFDevolucao = PropertyEntity({ text: "NF Devolução:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.DevolucaoParcial.val.subscribe(function (novoValor) {
        const $alertCapsule = $("#alert-capsule-" + self.Codigo.val());

        if (novoValor) {
            self.DevolucaoTotal.val(false);
            self.MotivoDaDevolucao.visible(true);

            if (novoValor === true) {
                $alertCapsule.removeClass('dont-display');
            } else {
                $alertCapsule.addClass('dont-display');
            }
        }
    });

    this.DevolucaoTotal.val.subscribe(function (novoValor) {
        const $alertCapsule = $("#alert-capsule-" + self.Codigo.val());

        if (novoValor) {
            self.DevolucaoParcial.val(false);
            self.MotivoDaDevolucao.visible(false);

            if (novoValor === true) {
                $alertCapsule.removeClass('dont-display');
            } else {
                $alertCapsule.addClass('dont-display');
            }
        }

        self.ProcessarItensDevolucao.eventClick(novoValor, self);
    });

    this.ExibirApenasProdutosComQuantidadeMaiorQueZero.val.subscribe(function (novoValor) {
        self.ExibirApenasProdutosComQuantidadeMaiorQueZero.eventClick(self);
    });

    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, visible: ko.observable(true) });
    this.AudicaoNotaFiscal = PropertyEntity({ type: types.event, eventClick: AbrirModalAudicaoNotaFiscal, visible: ko.observable(_CONFIGURACAO_TMS.PermiteAuditar) });
    this.Limpar = PropertyEntity({ text: "Limpar Filtros", type: types.event, visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ text: "Salvar", type: types.event, visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ text: "Adicionar", type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.LimparTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Limpar Todos os Registros", visible: ko.observable(true), enable: ko.observable(true) });
    this.DetalhesNotaFiscal = PropertyEntity({ text: "Detalhes da Nota Fiscal", visible: ko.observable(true) });
}

var ControleEntregaDevolucao = function () {
    this.DevolucaoPorPeso = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEdicao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.PossuiProdutos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirOpacaoObservacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarInformacaoLote = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarDataCritica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.NotasFiscais = ko.observableArray(new Array());
    this.GridProdutos;
    this.GridDevolucao;
}

var ControleEntregaDevolucaoContainer = function (idContainer) {
    this._controleEntregaDevolucao;
    this._observacaoProdutoDevolucao;
    this._idContainer = idContainer;

    // Promise para saber quando o html foi carregado
    this._carregado = new Promise(function (resolve, reject) {
        this._resolveCarregado = resolve;
    }.bind(this));

    this._inicializar();
}

var ObservacaoProdutoDevolucao = function () {
    var self = this;
    this.ObservacaoProduto = PropertyEntity({ getType: typesKnockout.string, text: "Observação:", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), maxlength: 1000 });

    this.SalvarObservacao = PropertyEntity({ type: types.event, text: "Salvar Observação", visible: ko.observable(true) });
}

var CargaEntregaProdutoChamado = function () {
    var self = this;

    this.MotivoDaDevolucao = PropertyEntity({
        type: types.entity,
        codEntity: ko.observable(0),
        text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.MotidoDaDevolucao.getFieldDescription()),
        idBtnSearch: guid(),
        required: ko.observable(false),
        visible: ko.observable(true),
        enable: ko.observable(true),
        requiredClass: ko.observable("")
    });
}


var AbrirModalAudicaoNotaFiscal = function (e) {
    var data = { Codigo: e.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("CargaEntregaNotaFiscal");

    closureAuditoria(data);
}

ControleEntregaDevolucaoContainer.prototype = {
    atualizarStatusDevolucaoTotalNotas: function (status) {
        status = Boolean(status);

        if (!this._controleEntregaDevolucao.PermitirEdicao.val())
            return;

        if (this._controleEntregaDevolucao.NotasFiscais().length > 0) {
            var notasFiscais = this._controleEntregaDevolucao.NotasFiscais.slice();
           
            for (var i = 0; i < notasFiscais.length; i++) {
                var notaFiscal = notasFiscais[i];

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS && status === true) {
                    notaFiscal.DevolucaoTotal.val(status);
                    notaFiscal.DevolucaoParcial.val(!status);
                } else {
                    notaFiscal.DevolucaoTotal.val(notasFiscais[i].DevolucaoTotal.val());
                    notaFiscal.DevolucaoParcial.val(notasFiscais[i].DevolucaoParcial.val());
                }

                this._atualizarProdutosDevolucaoTotalNotas(status, notaFiscal);
            }
            this._atualizarProdutosDevolucaoTotal(status, null);
        }
    },
    controlarEdicaoHabilitada: function (permitirEdicao) {
        permitirEdicao = Boolean(permitirEdicao);

        if (this._controleEntregaDevolucao.PermitirEdicao.val() == permitirEdicao)
            return;

        this._controleEntregaDevolucao.PermitirEdicao.val(permitirEdicao);

        if (this._controleEntregaDevolucao.NotasFiscais().length > 0) {
            var notasFiscais = this._controleEntregaDevolucao.NotasFiscais.slice();

            for (var i = 0; i < notasFiscais.length; i++) {
                var notaFiscal = notasFiscais[i];
                var notaFiscalProdutos = notaFiscal.GridProdutos.BuscarRegistros().slice();

                this._carregarGridProduto(notaFiscal.GridProdutos, notaFiscalProdutos);
            }
        }

        if (this._controleEntregaDevolucao.GridProdutos) {
            var produtos = this._controleEntregaDevolucao.GridProdutos.BuscarRegistros().slice();

            this._carregarGridProduto(this._controleEntregaDevolucao.GridProdutos, produtos);
        }
    },
    limpar: function () {
        var notasFiscais = this._controleEntregaDevolucao.NotasFiscais.slice();

        for (var i = 0; i < notasFiscais.length; i++) {
            notasFiscais[i].GridProdutos.Destroy();
            notasFiscais[i].GridProdutos = undefined;
            notasFiscais[i].GridDevolucao.Destroy();
            notasFiscais[i].GridDevolucao = undefined;
        }

        this._controleEntregaDevolucao.NotasFiscais.removeAll();
        this._controleEntregaDevolucao.DevolucaoPorPeso.val(false);
        this._controleEntregaDevolucao.ObrigarInformacaoLote.val(false);
        this._controleEntregaDevolucao.ObrigarDataCritica.val(false);
        this._controleEntregaDevolucao.PermitirEdicao.val(false);
        this._controleEntregaDevolucao.PossuiProdutos.val(false);
        this._controleEntregaDevolucao.ExibirOpacaoObservacao.val(false);

        if (this._controleEntregaDevolucao.GridProdutos) {
            this._controleEntregaDevolucao.GridProdutos.Destroy();
            this._controleEntregaDevolucao.GridProdutos = undefined;
            this._controleEntregaDevolucao.GridDevolucao.Destroy();
            this._controleEntregaDevolucao.GridDevolucao = undefined;
        }

        $("#ul-" + this._idContainer + " a:first").tab("show");
    },
    obter: function () {
        var self = this;

        return JSON.stringify({
            NotasFiscais: self._obterNotasFiscais(),
            Produtos: self._obterProdutos(),
            Devolucao: self._obterDevolucao()
        });
    },
    preencher: function (codigoCargaEntrega, permitirEdicao, codigoChamado, filtroCodigoNotasFiscais, callback, modalRejeicaoControleEntrega = false) {
        var self = this;

        self.limpar();

        if (isNaN(codigoCargaEntrega) || (codigoCargaEntrega <= 0))
            return;

        var codigoMotivoChamado = 0;
        if (_abertura != null && _abertura.MotivoChamado.codEntity() > 0)
            codigoMotivoChamado = _abertura.MotivoChamado.codEntity();

        executarReST("ControleEntregaDevolucao/ObterDadosDevolucaoEntrega", { Codigo: codigoCargaEntrega, CodigoMotivoChamado: codigoMotivoChamado, CodigoChamado: codigoChamado, ModalRejeicaoControleEntrega: modalRejeicaoControleEntrega }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    self._controleEntregaDevolucao.DevolucaoPorPeso.val(retorno.Data.DevolucaoPorPeso);
                    self._controleEntregaDevolucao.ObrigarInformacaoLote.val(retorno.Data.ObrigarInformacaoLote);
                    self._controleEntregaDevolucao.ObrigarDataCritica.val(retorno.Data.ObrigarDataCritica);
                    self._controleEntregaDevolucao.PermitirEdicao.val(Boolean(permitirEdicao));
                    self._controleEntregaDevolucao.ExibirOpacaoObservacao.val(retorno.Data.ExigeConferenciaProdutosConfirmarEntrega);

                    for (var i = 0; i < retorno.Data.NotasFiscais.length; i++) {
                        var notaFiscal = retorno.Data.NotasFiscais[i];
                        if (filtroCodigoNotasFiscais && !filtroCodigoNotasFiscais.find(nota => nota.Numero == notaFiscal.Dados.Numero)) {
                            continue;
                        }
                        var knoutDevolucaoNotaFiscal = new ControleEntregaDevolucaoNotaFiscal(notaFiscal.Dados);

                        self._controleEntregaDevolucao.NotasFiscais.push(knoutDevolucaoNotaFiscal);

                        new BuscarMotivosDevolucaoEntrega(knoutDevolucaoNotaFiscal.MotivoDaDevolucao);

                        knoutDevolucaoNotaFiscal.PossuiProdutos.val(notaFiscal.Produtos.length > 0);
                        knoutDevolucaoNotaFiscal.GridProdutos = self._criarGridProduto("grid-" + self._idContainer + "-nota-fiscal-produto-" + notaFiscal.Dados.Codigo);
                        knoutDevolucaoNotaFiscal.GridDevolucao = self._criarGridDevolucao("grid-" + self._idContainer + "-nota-fiscal-devolucao-" + notaFiscal.Dados.Codigo);
                        knoutDevolucaoNotaFiscal.Produtos = notaFiscal.Produtos;
                        knoutDevolucaoNotaFiscal.ItensDevolucao = notaFiscal.ItensDevolucao;
                        knoutDevolucaoNotaFiscal.Adicionar.enable(permitirEdicao);

                        SetarGetType(knoutDevolucaoNotaFiscal.NFDevolucao, knoutDevolucaoNotaFiscal.NFDevolucao.getType)
                        SetarGetType(knoutDevolucaoNotaFiscal.ValorDevolucao, knoutDevolucaoNotaFiscal.ValorDevolucao.getType)
                        SetarGetType(knoutDevolucaoNotaFiscal.QuantidadeDevolucao, knoutDevolucaoNotaFiscal.QuantidadeDevolucao.getType)
                        SetarGetType(knoutDevolucaoNotaFiscal.QuantidadeDevolvida, knoutDevolucaoNotaFiscal.QuantidadeDevolvida.getType)

                        self._carregarGridProduto(knoutDevolucaoNotaFiscal.GridProdutos, notaFiscal.Produtos);
                        self._carregarGridDevolucao(knoutDevolucaoNotaFiscal.GridDevolucao, notaFiscal.ItensDevolucao);

                        if (!permitirEdicao)
                            knoutDevolucaoNotaFiscal.GridDevolucao.DesabilitarOpcoes();
                        else
                            knoutDevolucaoNotaFiscal.GridDevolucao.HabilitarOpcoes();

                        if (knoutDevolucaoNotaFiscal.DevolucaoParcial.val() == 1 && knoutDevolucaoNotaFiscal.PossuiProdutos.val())
                            knoutDevolucaoNotaFiscal.MotivoDaDevolucao.visible(true);
                        else
                            knoutDevolucaoNotaFiscal.MotivoDaDevolucao.visible(false);

                        knoutDevolucaoNotaFiscal.Pesquisar.eventClick = function (e) {
                            self._filtrarGridProdutos(e);
                        };
                        knoutDevolucaoNotaFiscal.ExibirApenasProdutosComQuantidadeMaiorQueZero.eventClick = function (e) {
                            self._filtrarGridProdutos(e);
                        };
                        knoutDevolucaoNotaFiscal.Adicionar.eventClick = function (e) {
                            self._adicionarDevolucao(e);
                        };

                        knoutDevolucaoNotaFiscal.Limpar.eventClick = function (e) {
                            self._limparFiltroGridProdutos(e);
                        };
                        knoutDevolucaoNotaFiscal.LimparTodos.eventClick = function (e) {
                            self._limparTodosDadosGridProdutos(e);
                        };
                        knoutDevolucaoNotaFiscal.ProcessarItensDevolucao.eventClick = function (e, v) {
                            self._atualizarProdutosDevolucaoTotalNotas(e, v);
                        };

                        knoutDevolucaoNotaFiscal.Salvar.eventClick = function (e) {
                            self._salvarCamposModificadosGridNotasFiscais(e);
                        };
                        knoutDevolucaoNotaFiscal.Salvar.visible(_analise?.Codigo?.val() > 0);

                        knoutDevolucaoNotaFiscal.DetalhesNotaFiscal.eventClick = function (e) {
                            self._visualizarDetalhesNotaFiscal(e);
                        };

                        if (_analise?.Codigo?.val() > 0) {
                            knoutDevolucaoNotaFiscal.ExibirApenasProdutosComQuantidadeMaiorQueZero.val(true);
                            self._filtrarGridProdutos(knoutDevolucaoNotaFiscal);
                        }
                    }

                    self._controleEntregaDevolucao.PossuiProdutos.val(retorno.Data.Produtos.length > 0);
                    self._controleEntregaDevolucao.GridProdutos = self._criarGridProduto("grid-" + self._idContainer + "-produto");
                    self._controleEntregaDevolucao.GridDevolucao = self._criarGridDevolucao("grid-" + self._idContainer + "-devolucao");

                    self._carregarGridProduto(self._controleEntregaDevolucao.GridProdutos, retorno.Data.Produtos);
                    self._carregarGridDevolucao(self._controleEntregaDevolucao.GridDevolucao, retorno.Data.ItensDevolucao);

                    if (callback)
                        callback();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    },
    _atualizarGridProduto: function (rowData, row, head, callbackTabPress, table) {
        var pesoTotal = Globalize.parseFloat(rowData.PesoTotal);
        var quantidade = Globalize.parseFloat(rowData.Quantidade);
        var quantidadeDevolucao = Globalize.parseFloat(rowData.QuantidadeDevolucao);
        var quantidadeDevolvida = Globalize.parseFloat(rowData.QuantidadeDevolvida);
        var quantidadeComparar = this._controleEntregaDevolucao.DevolucaoPorPeso.val() ? pesoTotal : quantidade - quantidadeDevolvida;
        var corFonte = "";

        if (quantidadeDevolucao > quantidadeComparar)
            quantidadeDevolucao = quantidadeComparar;

        if (quantidadeDevolucao == quantidadeComparar)
            corFonte = "#ff0000";
        else if (quantidadeDevolucao > 0)
            corFonte = "#e08506";

        rowData.QuantidadeDevolucao = Globalize.format(quantidadeDevolucao, "n" + _CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto);
        rowData.DT_FontColor = corFonte;

        AtualizarDataRow(table, row, rowData, callbackTabPress);
    },
    _carregarGridProduto: function (gridProdutos, produtos) {
        for (var i = 0; i < produtos.length; i++)
            produtos[i].DT_Enable = this._controleEntregaDevolucao.PermitirEdicao.val();

        gridProdutos.CarregarGrid(produtos);
    },
    _carregarGridDevolucao: function (gridDevolucao, itens) {

        for (var i = 0; i < itens.length; i++)
            itens[i].DT_Enable = this._controleEntregaDevolucao.PermitirEdicao.val();

        gridDevolucao.CarregarGrid(itens);
    },
    _carregarHtml: function (callback) {
        var self = this;

        $.get("Content/Static/Carga/ControleEntrega/ControleEntregaDevolucao.html?dyn=" + guid(), function (htmlControleEntregaDevolucao) {
            var $containerControleEntregaDevolucao = $("#" + self._idContainer);

            if ($containerControleEntregaDevolucao.length == 0) {
                console.error("Esqueçeu da div #" + self._idContainer + "!!!!");
                return;
            }

            $containerControleEntregaDevolucao.html(htmlControleEntregaDevolucao.replace(/#idControleEntregaDevolucao#/g, self._idContainer));
            callback();

            // Promise reolvida para indicar quando o html foi carregado
            self._resolveCarregado();
        });
    },
    _carregarHtmlModais: function (callback) {
        var self = this;

        $.get("Content/Static/Carga/ControleEntrega/ControleEntregaDevolucaoModais.html?dyn=" + guid(), function (htmlControleEntregaDevolucaoModais) {
            var $divModais = $("#modais-devolucao-produto");

            if ($divModais.length == 0)
                $("main").append('<div id="modais-devolucao-produto"></div>');

            $("#modais-devolucao-produto").append(htmlControleEntregaDevolucaoModais.replace(/#idControleEntregaDevolucao#/g, self._idContainer));

            callback();
        });
    },
    _criarGridProduto: function (idGrid) {
        var self = this;
        var quantidadePorPagina = 5;
        var configuracaoEdicaoColuna = {
            permite: true,
            atualizarRow: false,
            callback: function (rowData, row, head, callbackTabPress, table) {
                self._atualizarGridProduto(rowData, row, head, callbackTabPress, table);
            }
        };

        var configuracaoEdicaoCelula = {
            editable: true,
            type: EnumTipoColunaEditavelGrid.decimal,
            mask: "",
            maxlength: 10 + _CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto,
            numberMask: ConfigDecimal({ precision: _CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto }),
            allowZero: false,
            precision: 0,
            thousands: ".",
            type: 1
        };

        var configuracaoEdicaoCelulaString = {
            editable: true,
            type: EnumTipoColunaEditavelGrid.string,
            numberMask: null
        };

        var configuracaoEdicaoCelulaDate = {
            editable: true,
            type: EnumTipoColunaEditavelGrid.data
        };

        var configuracaoEdicaoCelulaInt = {
            editable: true,
            type: EnumTipoColunaEditavelGrid.int,
            numberMask: ConfigInt()
        };

        var header = [];

        header.push({ data: "Codigo", visible: false });
        header.push({ data: "Observacao", visible: false });
        header.push({ data: "CodigoProduto", title: "Código Produto", width: "20%", className: "text-align-left" });
        header.push({ data: "Descricao", title: "Produto", width: "50%", className: "text-align-left" });

        if (self._controleEntregaDevolucao.DevolucaoPorPeso.val()) {
            header.push({ data: "PesoTotal", title: "Peso Total", width: "20%", className: "text-align-right" });
            header.push({ data: "QuantidadeDevolvida", title: "Peso Devolvido", width: "30%", className: "text-align-right" });
            header.push({ data: "QuantidadeDevolucao", title: "Peso Devolução", width: "30%", className: "text-align-right", editableCell: configuracaoEdicaoCelula });
        }
        else {
            header.push({ data: "Quantidade", title: "Quantidade", width: "20%", className: "text-align-right" });
            header.push({ data: "QuantidadeDevolvida", title: "Quantidade Devolvida", width: "20%", className: "text-align-right" });
            header.push({ data: "QuantidadeDevolucao", title: "Quantidade Devolução", width: "20%", className: "text-align-right", editableCell: configuracaoEdicaoCelula });
        }
        if (self._controleEntregaDevolucao.ObrigarDataCritica.val()) {
            header.push({ data: "DataCritica", title: "Data Crítica", width: "20%", className: "text-align-right", editableCell: configuracaoEdicaoCelulaDate });
        }
        if (self._controleEntregaDevolucao.ObrigarInformacaoLote.val()) {
            header.push({ data: "Lote", title: "Lote", width: "20%", className: "text-align-right", editableCell: configuracaoEdicaoCelulaString });
        }
        if (_CONFIGURACAO_TMS.CalcularValorDasDevolucoes) {
            header.push({ data: "ValorDevolucao", title: "Valor Devolução", width: "20%", className: "text-align-right" });
        }
        else {
            header.push({ data: "ValorDevolucao", title: "Valor Devolução", width: "20%", className: "text-align-right", editableCell: configuracaoEdicaoCelula });
            header.push({ data: "NFDevolucao", title: "NF Devolução", width: "20%", className: "text-align-right", editableCell: configuracaoEdicaoCelulaInt });
            header.push({ data: "MotivoDaDevolucao", title: "Motivo da Devolução", width: "20%", className: "text-align-right" });
            header.push({ data: "CodigoMotivoDaDevolucao", visible: false });
        }

        let opcoes;
        let informarObservacaoProduto = {
            descricao: Localization.Resources.Gerais.Geral.Observacao,
            id: guid(),
            evento: "onclick",
            visibilidade: true,
            metodo: function (data, row, table) {
                self._informarObservacaoClick(data, row, table);
            }
        };

        let informarMotivoDaDevolucao = {
            descricao: "Motivo da Devolução",
            id: guid(),
            evento: "onclick",
            visibilidade: true,
            metodo: function (data, row, table) {
                const cargaEntregaProdutoChamado = new CargaEntregaProdutoChamado();
                const buscarMotivoDaDevolucao = new BuscarMotivosDevolucaoEntrega(cargaEntregaProdutoChamado.MotivoDaDevolucao, null, null, (m) => {
                    data.CodigoMotivoDaDevolucao = m.Codigo;
                    data.MotivoDaDevolucao = m.Descricao;

                    self._atualizarGridProduto(data, row, null, null, table);
                }); 

                buscarMotivoDaDevolucao.openModal();
            }
        };

        if (self._controleEntregaDevolucao.ExibirOpacaoObservacao.val())
            opcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [informarObservacaoProduto, informarMotivoDaDevolucao], tamanho: 5 }
        else
            opcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [informarMotivoDaDevolucao], tamanho: 5 }

        return new BasicDataTable(idGrid, header, opcoes, null, null, quantidadePorPagina, null, null, configuracaoEdicaoColuna);
    },
    _filtrarGridProdutos: function (knout, limpaQuantidadeTodos = false) {
        let zero = '0,00';
        if (_CONFIGURACAO_TMS)
            zero = `0${_CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto > 0 ? ',' : ''}${'0'.repeat(_CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto)}`;

        let produtos = knout.Produtos;
        let filtroCodigoProduto = (p) => { return p.CodigoProduto == knout.CodigoProduto.val(); };
        let filtroQuantidadeProduto = (p) => { return string.ParseFloat(p.QuantidadeDevolucao) > 0; };
        let filtroDescricaoProduto = (p) => { return p.Descricao.toLowerCase().includes(knout.DescricaoProduto.val().toLowerCase()); };

        if (knout.CodigoProduto.val() != "" && knout.CodigoProduto.val() != undefined && knout.CodigoProduto.val() != null)
            produtos = produtos.filter(filtroCodigoProduto);

        if (knout.DescricaoProduto.val() != "" && knout.DescricaoProduto.val() != undefined && knout.DescricaoProduto.val() != null)
            produtos = produtos.filter(filtroDescricaoProduto);

        if (knout.ExibirApenasProdutosComQuantidadeMaiorQueZero.val())
            produtos = produtos.filter(filtroQuantidadeProduto);

        if (limpaQuantidadeTodos) {
            produtos.forEach(produto => {
                produto.QuantidadeDevolucao = zero;
            });
            knout.MotivoDaDevolucao.val("");
            knout.MotivoDaDevolucao.codEntity(0);
        }

        produtos.forEach(produto => {
            produto.DT_Enable = this._controleEntregaDevolucao.PermitirEdicao.val();
        });

        knout.GridProdutos.CarregarGrid(produtos);
    },
    _adicionarDevolucao: function (knout) {
        var self = this;
        let data = {
            Codigo: knout.Codigo.val(),
            QuantidadeDevolucao: knout.QuantidadeDevolucao.val(),
            ValorDevolucao: knout.ValorDevolucao.val(),
            NFDevolucao: knout.NFDevolucao.val()
        }
        executarReST("ControleEntregaDevolucao/AdicionarItemDevolucao", data, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Adicionado com sucesso!");
                    var notaFiscal = self._controleEntregaDevolucao.NotasFiscais().filter((obj) => obj.Codigo.val() == data.Codigo)[0];
                    notaFiscal.GridDevolucao.CarregarGrid(r.Data);
                    LimparCampo(knout.QuantidadeDevolucao);
                    LimparCampo(knout.ValorDevolucao);
                    LimparCampo(knout.NFDevolucao);
                } else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        });
    },
    _criarGridDevolucao: function (idGrid) {
        var self = this;
        var quantidadePorPagina = 5;

        var header = [];

        header.push({ data: "Codigo", title: "Código", visible: false });
        header.push({ data: "CodigoNotaFiscal", title: "Código", visible: false });
        header.push({ data: "QuantidadeDevolvida", title: "Quantidade Devolvida", width: "33%", className: "text-align-left" });
        header.push({ data: "QuantidadeDevolucao", title: "Quantidade Devolução", width: "33%", className: "text-align-left" });
        header.push({ data: "ValorDevolucao", title: "Valor Devolução", width: "33%", className: "text-align-left" });
        header.push({ data: "NFDevolucao", title: "NF Devolução", width: "20%", className: "text-align-left" });
        var excluir = {
            descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data, row, table) {
                var dados = { Codigo: data.Codigo }
                executarReST("ControleEntregaDevolucao/RemoverItemDevolucao", dados, function (r) {
                    if (r.Success) {
                        if (r.Data) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Removido com sucesso");
                            var notaFiscal = self._controleEntregaDevolucao.NotasFiscais().filter((obj) => obj.Codigo.val() == data.CodigoNotaFiscal)[0];
                            notaFiscal.ItensDevolucao = r.Data;
                            notaFiscal.GridDevolucao.CarregarGrid(notaFiscal.ItensDevolucao);
                        } else
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    } else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                });
            }, tamanho: "10", icone: ""
        };
        var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };


        return new BasicDataTable(idGrid, header, menuOpcoes, null, null, quantidadePorPagina);
    },
    _inicializar: function () {
        var self = this;

        self._carregarHtml(function () {
            self._controleEntregaDevolucao = new ControleEntregaDevolucao();
            KoBindings(self._controleEntregaDevolucao, "knockout-" + self._idContainer);
        });

        self._carregarHtmlModais(function () {
            self._observacaoProdutoDevolucao = new ObservacaoProdutoDevolucao();
            KoBindings(self._observacaoProdutoDevolucao, "knockoutObservacaoProdutoDevolucao-" + self._idContainer);
        });
    },
    _limparFiltroGridProdutos: function (knout) {
        knout.CodigoProduto.val("");
        knout.DescricaoProduto.val("");
        this._filtrarGridProdutos(knout);
    },
    _limparTodosDadosGridProdutos: function (knout) {
        knout.CodigoProduto.val("");
        knout.DescricaoProduto.val("");
        this._filtrarGridProdutos(knout, true);
    },
    _salvarCamposModificadosGridNotasFiscais: function (knout) {
        if (_chamado.Situacao.val() != EnumSituacaoChamado.Aberto && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Os produtos só podem ser editados durante a etapa de análise.");
            return;
        }

        if (_analise.Codigo.val() > 0) {
            executarReST("ChamadoAnalise/SalvarProdutosNotasFiscaisAnaliseDevolucao", { Produtos: JSON.stringify(knout.Produtos), DevolucaoParcial: knout.DevolucaoParcial.val(), Chamado: _analise.Codigo.val(), NotaFiscal: knout.Codigo.val(), MotivoDaDevolucao: knout.MotivoDaDevolucao.codEntity() }, function (r) {
                if (r.Success) {
                    if (r.Data)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, r.Data.Teste);
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                } else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            });
        }
    },
    _obterNotasFiscais: function () {
        const notasFiscais = this._controleEntregaDevolucao.NotasFiscais.slice();
        let notasFiscaisRetornar = [];

        for (const notaFiscal of notasFiscais) {
            const bkpExibirApenasProdutosComQuantidadeMaiorQueZero = notaFiscal.ExibirApenasProdutosComQuantidadeMaiorQueZero.val();
            notaFiscal.ExibirApenasProdutosComQuantidadeMaiorQueZero.val(false);

            let notaFiscalRetornar = {
                Codigo: notaFiscal.Codigo.val(),
                DevolucaoParcial: notaFiscal.DevolucaoParcial.val(),
                DevolucaoTotal: notaFiscal.DevolucaoTotal.val(),
                MotivoDevolucaoEntrega: notaFiscal.MotivoDaDevolucao.codEntity(),
                Numero: notaFiscal.Numero.val(),
                SituacaoNotaFiscal: notaFiscal.SituacaoNotaFiscal.val(),
                Produtos: []
            }

            const produtosOriginais = notaFiscal.Produtos;
            const notaFiscalProdutos = notaFiscal.GridProdutos.BuscarRegistros().slice();

            for (const notaFiscalProduto of notaFiscalProdutos) {
                let produtoOriginal = produtosOriginais.find(p => p.Codigo == notaFiscalProduto.Codigo);
                notaFiscalRetornar.Produtos.push($.extend(produtoOriginal, {
                    Codigo: notaFiscalProduto.Codigo,
                    QuantidadeDevolucao: notaFiscalProduto.QuantidadeDevolucao,
                    Lote: notaFiscalProduto.Lote,
                    DataCritica: notaFiscalProduto.DataCritica,
                    ValorDevolucao: notaFiscalProduto.ValorDevolucao,
                    NFDevolucao: notaFiscalProduto.NFDevolucao,
                    CodigoMotivoDaDevolucao: notaFiscalProduto.CodigoMotivoDaDevolucao,
                }));
            }

            notasFiscaisRetornar.push(notaFiscalRetornar);
            notaFiscal.ExibirApenasProdutosComQuantidadeMaiorQueZero.val(bkpExibirApenasProdutosComQuantidadeMaiorQueZero);
        }

        return notasFiscaisRetornar;
    },
    _obterProdutos: function () {
        if (!Boolean(this._controleEntregaDevolucao.GridProdutos))
            return [];

        const produtos = this._controleEntregaDevolucao.GridProdutos.BuscarRegistros().slice();
        let produtosRetornar = [];
        for (const produto of produtos) {
            produtosRetornar.push({
                Codigo: produto.Codigo,
                QuantidadeDevolucao: produto.QuantidadeDevolucao,
                Lote: produto.Lote,
                DataCritica: produto.DataCritica,
                ValorDevolucao: produto.ValorDevolucao,
                ValorTotal: produto.ValorTotal,
                NFDevolucao: produto.NFDevolucao,
                CodigoMotivoDaDevolucao: produto.CodigoMotivoDaDevolucao,
            });
        }

        return produtosRetornar;
    },
    _obterDevolucao: function () {
        if (!Boolean(this._controleEntregaDevolucao.GridDevolucao))
            return [];

        var itens = this._controleEntregaDevolucao.GridDevolucao.BuscarRegistros().slice();
        var itensRetornar = [];

        for (var i = 0; i < itens.length; i++) {
            var item = itens[i];
            itensRetornar.push({
                Codigo: item.Codigo,
                CodigoNotaFiscal: item.CargaEntregaNotaFiscal != null ? item.CargaEntregaNotaFiscal.Codigo : item.CodigoNotaFiscal,
                QuantidadeDevolucao: item.QuantidadeDevolucao,
                QuantidadeDevolvida: item.QuantidadeDevolvida,
                ValorDevolucao: item.ValorDevolucao,
                ValorTotal: item.ValorTotal,
                NFDevolucao: item.NFDevolucao,
            })
        }

        return itensRetornar;
    },
    _informarObservacaoClick: function (data, row, table) {
        var self = this;
        var idContainer = self._idContainer;
        self._observacaoProdutoDevolucao.ObservacaoProduto.val(data.Observacao);
        self._observacaoProdutoDevolucao.SalvarObservacao.visible(true);
        self._observacaoProdutoDevolucao.SalvarObservacao.eventClick = function (e) {
            var self = this;

            executarReST("ControleEntregaDevolucao/SalvarObservacaoProdutoDevolucao", { CodigoCargaEntregaProduto: data.Codigo, Observacao: self.ObservacaoProduto.val() }, function (r) {
                if (r.Success) {
                    if (r.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Observação gravada com sucesso!");

                        data.Observacao = self.ObservacaoProduto.val();
                        AtualizarDataRow(table, row, data, null);

                        Global.fecharModal("modal-observacao-devolucao-produto-" + idContainer);
                    } else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                } else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            });
        };

        Global.abrirModal("modal-observacao-devolucao-produto-" + idContainer);
    },
    _atualizarProdutosDevolucaoTotal: function (devolucaoTotal, notaFiscal) {

        if (!this._controleEntregaDevolucao.PermitirEdicao.val()) return;

        let produtos = this._controleEntregaDevolucao.GridProdutos.BuscarRegistros().slice();

        if (!produtos || produtos.length == 0) return;

        if (devolucaoTotal) {
            produtos.forEach(produto => produto.QuantidadeDevolucao = (string.ParseFloat(produto.Quantidade) - string.ParseFloat(produto.QuantidadeDevolvida)).toFixed(2));
        }

        this._carregarGridProduto(this._controleEntregaDevolucao.GridProdutos, produtos);
    },
    _atualizarProdutosDevolucaoTotalNotas: function (devolucaoTotal, notaFiscal) {

        if (!this._controleEntregaDevolucao.PermitirEdicao.val()) return;


        if (!notaFiscal.Produtos || notaFiscal.Produtos.length == 0) return;

        if (devolucaoTotal) {
            notaFiscal.Produtos.forEach(produto => {
                const quantidadeDevolucao = parseFloat(produto.Quantidade) - parseFloat(produto.QuantidadeDevolvida);
                const valorTotal = parseFloat(produto.ValorTotal);

                produto.QuantidadeDevolucao = quantidadeDevolucao.toFixed(2);
                produto.ValorDevolucao = (quantidadeDevolucao * valorTotal).toFixed(2);

            });
        }
    },
    _validarDadosPreenchidosDevolucao: function () {
        let valido = true;

        //Validação dos produtos por nota.
        let notasFiscais = this._obterNotasFiscais();
        for (const notaFiscal of notasFiscais) {
            if (notaFiscal.DevolucaoTotal) continue;
            let notaValida = false;
            for (const produto of notaFiscal.Produtos) {
                if (string.ParseFloat(produto.QuantidadeDevolucao) > 0)
                    notaValida = true;
            }
            if (!notaValida) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, "A nota fiscal de número " + notaFiscal.Numero + " deve conter pelo menos um produto com quantidade a ser devolvida.");
                return false;
            }
        }

        //Validação da grid de produtos sem nota.
        let produtoValido = false;
        let produtosDevolucao = this._obterDevolucao();
        for (const produtoDevolucao of produtosDevolucao) {
            if (string.ParseFloat(produtoDevolucao.QuantidadeDevolvida) > 0)
                produtoValido = true;
        }
        if (produtosDevolucao.length > 0 && !produtoValido) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Lista de Produtos: Deve conter pelo menos um produto com quantidade devolvida.");
            return false;
        }

        return valido;
    },
    _visualizarDetalhesNotaFiscal: function (e) {
        const dadosNotaFiscal = {
            Codigo: e.Codigo.val(),
            Numero: e.Numero.val(),
            Serie: e.Serie.val(),
            DataEmissao: e.DataEmissao.val(),
            Volume: e.Volume.val(),
            Valor: e.Valor.val(),
            Peso: e.Peso.val(),
            Filial: e.Filial.val(),
            Chave: e.Chave.val(),
        };
        if (typeof loadDetalhesNotaFiscal === 'function')
            loadDetalhesNotaFiscal(dadosNotaFiscal);
    }
};
