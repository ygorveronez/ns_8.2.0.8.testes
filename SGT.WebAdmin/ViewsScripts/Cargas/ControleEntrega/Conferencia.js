/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="Entrega.js" />

var _observacaoConferenciaProduto;
var _observacaoConferenciaSobra;
var _conferenciaProduto;

var ObservacaoConferenciaProduto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Observacao = PropertyEntity({ type: typesKnockout.string, text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 2000 });

    this.Confirmar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
}

var ObservacaoConferenciaSobra = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Observacao = PropertyEntity({ type: typesKnockout.string, text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 2000 });
    this.QuantidadeConferencia = PropertyEntity({ type: typesKnockout.int, text: "Quantidade conferida:", val: ko.observable(""), def: "" });
    this.QuantidadeSobra = PropertyEntity({ type: typesKnockout.int, text: "Quantidade sobra:", val: ko.observable(""), def: "", enable: false});

    this.Confirmar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
}

function loadConferenciaProduto() {
    _observacaoConferenciaProduto = new ObservacaoConferenciaProduto();
    KoBindings(_observacaoConferenciaProduto, "modalObservacaoConferenciaProduto");

    _observacaoConferenciaSobra = new ObservacaoConferenciaSobra();
    KoBindings(_observacaoConferenciaSobra, "modalObservacaoConferenciaSobra");

    var modalObservacaoConferenciaProduto = $("#modalObservacaoConferenciaProduto");
    modalObservacaoConferenciaProduto.on("hidden.bs.modal", function () { LimparCampos(_observacaoConferenciaProduto) });

    var modalObservacaoConferenciaSobra = $("#modalObservacaoConferenciaSobra");
    modalObservacaoConferenciaSobra.on("hidden.bs.modal", function () { LimparCampos(_observacaoConferenciaSobra) });

    _conferenciaProduto = ConferenciaProdutos;
}


var ConferenciaProdutos = {
    _gridsConferenciaProduto: [],
    CarregarConferenciaProdutos: function (data) {
        var self = this;
        if (data.EnumSituacao == EnumSituacaoEntrega.Entregue) {
            $("#liConferenciaProdutos").show();
            _entrega.ConfirmarConferenciaProdutos.visible(false);
            _observacaoConferenciaProduto.Confirmar.visible(false);
        }
        else
            $("#liConferenciaProdutos").hide();

        executarReST("ControleEntregaConferencia/ObterDadosConferenciaEntrega", { Codigo: _entrega.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    self._gridsConferenciaProduto = [];
                    self.CarregarGridConferenciaProdutos(r.Data);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso.Falha, r.Msg);
            }
        });

    },
    CarregarGridConferenciaProdutos: function (data) {
        var self = this;
        if (data.ProdutosConfernecia)
            _entrega.ConferenciaProdutos(data.ProdutosConfernecia);
        else
            _entrega.ConferenciaProdutos([]);

        if (_entrega.ConferenciaProdutos().length > 0) {

            var configuracaoEdicaoColuna = {
                permite: true,
                atualizarRow: false,
                callback: function (rowData, row, head, callbackTabPress, table) {
                    self.AtualizarGridConferenciaProduto(rowData, row, head, callbackTabPress, table);
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

            var header = [
                { data: "Codigo", visible: false },
                { data: "Observacao", visible: false },
                { data: "CodigoProduto", title: "Código Produto", width: "20%", className: "text-align-left" },
                { data: "Descricao", title: "Produto", width: "40%", className: "text-align-left" },
                { data: "Quantidade", title: "Quantidade", width: "20%", className: "text-align-right" },
                { data: "QuantidadeConferencia", title: "Quantidade conferida", width: "20%", className: "text-align-right", editableCell: configuracaoEdicaoCelula },
            ];

            let observacaoConferencia = {
                descricao: Localization.Resources.Gerais.Geral.Observacao,
                id: guid(),
                evento: "onclick",
                visibilidade: true,
                metodo: function (dataRow, row, table) {
                    self.ObservacaoProdutoConferencia(dataRow, row, table);
                }
            };

            let opcoes = {
                tipo: TypeOptionMenu.list,
                descricao: Localization.Resources.Gerais.Geral.Opcoes,
                opcoes: [observacaoConferencia],
                tamanho: 5
            };

            var configExportacao = {
                url: "ControleEntregaConferencia/ExportarExcel",
                btnText: "Exportar Excel",
                funcaoObterParametros: function () {
                    return { Codigo: _entrega.Codigo.val() };
                }
            }

            _entrega.ConferenciaProdutos().forEach(function (notaFiscal) {
                _gridConferenciaProduto = new BasicDataTable("grid-conferencia-" + notaFiscal.Codigo, header, opcoes, null, null, 5, null, null, configuracaoEdicaoColuna, null, null, null, configExportacao);

                for (var i = 0; i < notaFiscal.Produtos.length; i++)
                    notaFiscal.Produtos[i].DT_Enable = _entrega.EnumSituacao.val() != EnumSituacaoEntrega.Entregue;

                _gridConferenciaProduto.CarregarGrid(notaFiscal.Produtos);

                self._gridsConferenciaProduto.push(_gridConferenciaProduto);
            });

        }
    },
    ObservacaoProdutoConferencia: function (data, row, table) {
        var self = this;
        _observacaoConferenciaProduto.Codigo.val(data.Codigo);
        _observacaoConferenciaProduto.Observacao.val(data.Observacao);
        _observacaoConferenciaProduto.Confirmar.eventClick = function () { self.SalvarObservacaoClick(data, row, table); };

        Global.abrirModal("modalObservacaoConferenciaProduto");
    },
    SalvarObservacaoClick: function (rowData, row, table) {
        var self = this;
        var data = {
            CodigoCargaEntregaProduto: _observacaoConferenciaProduto.Codigo.val(),
            Observacao: _observacaoConferenciaProduto.Observacao.val()
        };

        executarReST("ControleEntregaConferencia/SalvarObservacaoProdutoConferencia", data, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Observação gravada com sucesso!");

                    rowData.Observacao = data.Observacao;
                    AtualizarDataRow(table, row, rowData, null);

                    Global.fecharModal("modalObservacaoConferenciaProduto");
                } else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        });
    },
    AtualizarGridConferenciaProduto: function (rowData, row, head, callbackTabPress, table) {
        var quantidade = Globalize.parseFloat(rowData.Quantidade);
        var quantidadeConferencia = Globalize.parseFloat(rowData.QuantidadeConferencia);
        var corFonte = "";

        if (quantidadeConferencia > quantidade)
            quantidadeConferencia = quantidade;

        if (quantidadeConferencia == quantidade)
            corFonte = "#00cc00";
        else if (quantidadeConferencia > 0)
            corFonte = "#e08506";

        rowData.QuantidadeConferencia = Globalize.format(quantidadeConferencia, "n" + _CONFIGURACAO_TMS.NumeroCasasDecimaisQuantidadeProduto);
        rowData.DT_FontColor = corFonte;

        AtualizarDataRow(table, row, rowData, callbackTabPress);
    },
    SalvarConferenciaProdutos: function (callback) {
        var self = this;
        var data = [];

        for (var i = 0; i < self._gridsConferenciaProduto.length; i++) {
            data = data.concat(self._gridsConferenciaProduto[i].BuscarRegistros());
        }

        executarReST("ControleEntregaConferencia/SalvarConferenciaProdutos", { Produtos: JSON.stringify(data) }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Conferência gravada com sucesso!");

                    if (callback != null)
                        callback()
                } else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        });
    }
};

function realizarConferencia() {
    _entrega.ConfirmarEntrega.visible(false);
    _entrega.ConfirmarConferenciaProdutos.visible(true);
    _observacaoConferenciaProduto.Confirmar.visible(true);
    $("#liConferenciaProdutos").show();
    Global.ExibirAba("tab-controle-entrega-conferencia-produtos");
}

function confirmarConferenciaProdutos(e) {
    _conferenciaProduto.SalvarConferenciaProdutos(function () {
        confirmacaoEntrega(e);
    });
}

function CarregarGridSobras(dados) {
    if (dados.ExibirSobras) {
        _entrega.Sobras.visible(true)

        let conferencia = {
            descricao: "Conferência",
            id: guid(),
            evento: "onclick",
            visibilidade: true,
            metodo: function (dataRow, row, table) {
                ObservacaoSobraConferencia(dataRow, row, table);
            }
        };

        let opcoes = {
            tipo: TypeOptionMenu.list,
            descricao: Localization.Resources.Gerais.Geral.Opcoes,
            opcoes: [conferencia],
            tamanho: 5
        };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Observacao", visible: false },
            { data: "CodigoSobra", title: "Código", width: "40%", className: "text-align-left" },
            { data: "QuantidadeSobra", title: "Quantidade", width: "25%", className: "text-align-left" },
            { data: "QuantidadeConferencia", title: "Quantidade conferida", width: "20%", className: "text-align-left" },
        ];

        _gridSobras = new BasicDataTable(_entrega.Sobras.idGrid, header, opcoes);
        
        _gridSobras.CarregarGrid(dados.Sobras);
    }
}

function ObservacaoSobraConferencia(data, row, table) {
    _observacaoConferenciaSobra.Codigo.val(data.Codigo);
    _observacaoConferenciaSobra.Observacao.val(data.Observacao);
    _observacaoConferenciaSobra.QuantidadeConferencia.val(data.QuantidadeConferencia);
    _observacaoConferenciaSobra.QuantidadeSobra.val(data.QuantidadeSobra);

    _observacaoConferenciaSobra.Confirmar.eventClick = function () { SalvarConferenciaSobras(data, row, table); };

    Global.abrirModal("modalObservacaoConferenciaSobra");
}

function SalvarConferenciaSobras(rowData, row, table) {
    var data = {
        CodigoGestaoOcorrenciaSobra: _observacaoConferenciaSobra.Codigo.val(),
        Observacao: _observacaoConferenciaSobra.Observacao.val(),
        QuantidadeConferencia: _observacaoConferenciaSobra.QuantidadeConferencia.val()
    };

    executarReST("GestaoOcorrencia/SalvarDadosSobraConferencia", data, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Dados Conferencia gravados com sucesso!");

                rowData.Observacao = data.Observacao;
                rowData.QuantidadeConferencia = data.QuantidadeConferencia;
                AtualizarDataRow(table, row, rowData, null);

                Global.fecharModal("modalObservacaoConferenciaSobra");
            } else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    });
}