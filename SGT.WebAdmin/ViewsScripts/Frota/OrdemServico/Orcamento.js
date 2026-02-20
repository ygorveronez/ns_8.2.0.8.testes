////*******MAPEAMENTO KNOUCKOUT*******

var _orcamentoOrdemServico, _servicoOrcamentoOrdemServico, _resumoOrcamentoOrdemServico, _produtoOrcamentoServicoOrdemServico, _gridProdutoOrcamentoServicoOrdemServico;

var OrcamentoOrdemServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Servicos = ko.observableArray();
};

var AbaOrcamentoOrdemServico = function (dados) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(dados.Codigo) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(dados.Codigo), val: ko.observable(dados.Descricao) });

    this.Visualizar = PropertyEntity({ eventClick: VisualizarOrcamentoClick, type: types.event });
};

var ResumoOrcamentoOrdemServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ValorTotalProdutos = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor em Produtos:" });
    this.ValorTotalMaoObra = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor da Mão de Obra:" });
    this.ValorTotalOrcado = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor Total Orçado:" });
    this.ValorTotalPreAprovado = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor Total Pré Aprovado:" });
    this.ValorTotalListaProdutosDosServicos = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor Total Produtos dos Serviços:" });

    this.Parcelas = PropertyEntity({ text: "Parcelas:", val: ko.observable(""), def: "", getType: typesKnockout.int, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", enable: ko.observable(true) });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarOrcamentoClick, type: types.event, text: "Salvar", icon: "fal fa-save", idGrid: guid(), visible: ko.observable(false) });
};

var ServicoOrcamentoOrdemServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Manutencao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ValorProdutos = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor em Produtos:", val: ko.observable("0,00"), def: "0,00", enable: ko.observable(true) });
    this.ValorMaoObra = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor em Mão de Obra:", val: ko.observable("0,00"), def: "0,00", enable: ko.observable(true) });
    this.OrcadoPor = PropertyEntity({ text: "Orçado por:", val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", enable: ko.observable(true) });

    this.VisualizarImagem = PropertyEntity({ eventClick: VisualizarImagemOrcamentoServicoClick, type: types.event, text: "Vizualizar", icon: "fal fa-fw fa-eye", visible: ko.observable(true) });
    this.DownloadImagem = PropertyEntity({ eventClick: DownloadImagemOrcamentoServicoClick, type: types.event, text: "Download", icon: "fal fa-fw fa-download", visible: ko.observable(true) });
    this.AnexarImagem = PropertyEntity({ type: types.file, val: ko.observable(""), text: IsMobile() ? "Anexar Imagem" : "Anexar uma Imagem ao Orçamento", icon: "fal fa-camera", visible: ko.observable(false) });
    this.RemoverImagem = PropertyEntity({ eventClick: RemoverImagemOrcamentoServicoClick, type: types.event, text: "Remover", icon: "fal fa-fw fa-close", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarOrcamentoServicoClick, type: types.event, text: "Salvar Dados", icon: "fal fa-save", visible: ko.observable(true) });
    this.AdicionarAnexoOS = PropertyEntity({ eventClick: AdicionarAnexoOSClick, type: types.event, text: "Adicionar Anexos Odem Serviço", visible: ko.observable(true) });

    this.AdicionarProduto = PropertyEntity({ eventClick: AdicionarProdutoOrcamentoServicoClick, type: types.event, text: "Adicionar Produto", icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(true) });
    this.TotalizadorProdutos = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Totalizador Produtos:" });
};

var ProdutoOrcamentoServicoOrdemServico = function () {
    var self = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.OrcamentoServico = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor:", val: ko.observable(""), def: "", required: true });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, text: "Quantidade:", val: ko.observable(""), def: "", required: true });
    this.ValorTotal = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Total:", val: ko.observable(""), def: "", required: true });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarProdutoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarProdutoClick, type: types.event, text: "Atualizar", icon: "fal fa-save", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirProdutoClick, type: types.event, text: "Excluir", icon: "fal fa-close", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarProdutoClick, type: types.event, text: "Cancelar", icon: "fal fa-ban", visible: ko.observable(true) });

    ko.computed(function () {
        var quantidade = parseFloat(self.Quantidade.val());
        var valor = parseFloat(self.Valor.val().replace(',', '.'));
        if (!isNaN(quantidade) && !isNaN(valor)) {
            self.ValorTotal.val((quantidade * valor).toFixed(2));
        } else {
            self.ValorTotal.val("");
        }
    });

};

////*******EVENTOS*******

function LoadOrcamentoOrdemServico() {

    _orcamentoOrdemServico = new OrcamentoOrdemServico();
    KoBindings(_orcamentoOrdemServico, "knockoutAbasOrdemServico");

    _servicoOrcamentoOrdemServico = new ServicoOrcamentoOrdemServico();
    KoBindings(_servicoOrcamentoOrdemServico, "tabOrcamentoServicoOrdemServico");

    _resumoOrcamentoOrdemServico = new ResumoOrcamentoOrdemServico();
    KoBindings(_resumoOrcamentoOrdemServico, "knockoutResumoOrcamentoOrdemServico");

    _produtoOrcamentoServicoOrdemServico = new ProdutoOrcamentoServicoOrdemServico();
    KoBindings(_produtoOrcamentoServicoOrdemServico, "knockoutProduto");

    new BuscarProdutoTMS(_produtoOrcamentoServicoOrdemServico.Produto);

    $("#" + _servicoOrcamentoOrdemServico.AnexarImagem.id).on("change", function () {
        AnexarImagemOrcamentoServico(this.files);
    });

    LoadAnexoArquivoOS();
}

function VisualizarOrcamentoClick(e, sender) {
    _servicoOrcamentoOrdemServico.Codigo.val(e.Codigo.val());
    BuscarPorCodigo(_servicoOrcamentoOrdemServico, "OrcamentoOrdemServico/BuscarServicoPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridProdutoOrcamentoServicoOrdemServico.CarregarGrid();

                if (r.Data.PossuiImagem) {
                    _servicoOrcamentoOrdemServico.AnexarImagem.visible(false);
                    _servicoOrcamentoOrdemServico.RemoverImagem.visible(true);
                    _servicoOrcamentoOrdemServico.VisualizarImagem.visible(true);
                    _servicoOrcamentoOrdemServico.DownloadImagem.visible(true);
                } else {
                    _servicoOrcamentoOrdemServico.AnexarImagem.visible(true);
                    _servicoOrcamentoOrdemServico.VisualizarImagem.visible(false);
                    _servicoOrcamentoOrdemServico.DownloadImagem.visible(false);
                    _servicoOrcamentoOrdemServico.RemoverImagem.visible(false);
                }

                if (_ordemServico.Situacao.val() != EnumSituacaoOrdemServicoFrota.AgAutorizacao) {
                    _servicoOrcamentoOrdemServico.AnexarImagem.visible(false);
                    _servicoOrcamentoOrdemServico.RemoverImagem.visible(false);
                }
            } else {
                $("#tabOrcamentoServicoOrdemServico").removeClass("active show");
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            $("#tabOrcamentoServicoOrdemServico").removeClass("active show");
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AnexarImagemOrcamentoServico(files) {

    var formData = new FormData();

    formData.append("upload0", files[0]);

    enviarArquivo("OrcamentoOrdemServico/AnexarImagemOrcamento", { Codigo: _servicoOrcamentoOrdemServico.Codigo.val() }, formData, function (r) {
        if (r.Success) {
            if (r.Data) {
                _servicoOrcamentoOrdemServico.AnexarImagem.visible(false);
                _servicoOrcamentoOrdemServico.VisualizarImagem.visible(true);
                _servicoOrcamentoOrdemServico.DownloadImagem.visible(true);
                _servicoOrcamentoOrdemServico.RemoverImagem.visible(true);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });

    _servicoOrcamentoOrdemServico.AnexarImagem.val("");
}

function RemoverImagemOrcamentoServicoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover a imagem do orçamento?", function () {
        executarReST("OrcamentoOrdemServico/RemoverImagemOrcamento", { Codigo: _servicoOrcamentoOrdemServico.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _servicoOrcamentoOrdemServico.AnexarImagem.visible(true);
                    _servicoOrcamentoOrdemServico.VisualizarImagem.visible(false);
                    _servicoOrcamentoOrdemServico.DownloadImagem.visible(false);
                    _servicoOrcamentoOrdemServico.RemoverImagem.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function VisualizarImagemOrcamentoServicoClick() {
    executarReST("OrcamentoOrdemServico/ObterVisualizacaoImagemOrcamento", { Codigo: _servicoOrcamentoOrdemServico.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.abrirModal("modalVisualizarImagemOrcamento");
                $("#imgVisualizacaoOrcamento").attr("src", r.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function DownloadImagemOrcamentoServicoClick() {
    executarDownload("OrcamentoOrdemServico/DownloadImagemOrcamento", { Codigo: _servicoOrcamentoOrdemServico.Codigo.val() });
}

function AtualizarOrcamentoServicoClick(e, sender) {
    Salvar(_servicoOrcamentoOrdemServico, "OrcamentoOrdemServico/AtualizarServico", function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_resumoOrcamentoOrdemServico, r);
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados salvos com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarOrcamentoClick(e, sender) {
    Salvar(_resumoOrcamentoOrdemServico, "OrcamentoOrdemServico/AtualizarOrcamento", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados salvos com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AdicionarProdutoOrcamentoServicoClick() {
    LimparCamposProdutoOrcamentoServico();
    _produtoOrcamentoServicoOrdemServico.OrcamentoServico.val(_servicoOrcamentoOrdemServico.Codigo.val());
    Global.abrirModal("knockoutProduto");
}

function AdicionarProdutoClick(e, sender) {
    Salvar(_produtoOrcamentoServicoOrdemServico, "OrcamentoOrdemServico/AdicionarProduto", function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal('knockoutProduto');
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Produto adicionado com sucesso!");
                _gridProdutoOrcamentoServicoOrdemServico.CarregarGrid();
                LimparCamposProdutoOrcamentoServico();
                _servicoOrcamentoOrdemServico.TotalizadorProdutos.val(r.Data.TotalizadorProdutos);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarProdutoClick(e, sender) {
    Salvar(_produtoOrcamentoServicoOrdemServico, "OrcamentoOrdemServico/AtualizarProduto", function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal('knockoutProduto');
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Produto atualizado com sucesso!");
                _gridProdutoOrcamentoServicoOrdemServico.CarregarGrid();
                LimparCamposProdutoOrcamentoServico();
                _servicoOrcamentoOrdemServico.TotalizadorProdutos.val(r.Data.TotalizadorProdutos);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirProdutoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o produto " + _produtoOrcamentoServicoOrdemServico.Produto.val() + "?", function () {
        ExcluirPorCodigo(_produtoOrcamentoServicoOrdemServico, "OrcamentoOrdemServico/ExcluirProduto", function (r) {
            if (r.Success) {
                if (r.Data) {
                    Global.fecharModal('knockoutProduto');
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Produto excluído com sucesso!");
                    _gridProdutoOrcamentoServicoOrdemServico.CarregarGrid();
                    LimparCamposProdutoOrcamentoServico();
                    _servicoOrcamentoOrdemServico.TotalizadorProdutos.val(r.Data.TotalizadorProdutos);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarProdutoClick(e, sender) {
    Global.fecharModal('knockoutProduto');
    LimparCamposProdutoOrcamentoServico();
}

function EditarProdutoClick(produtoGrid) {
    LimparCamposProdutoOrcamentoServico();
    _produtoOrcamentoServicoOrdemServico.Codigo.val(produtoGrid.Codigo);
    BuscarPorCodigo(_produtoOrcamentoServicoOrdemServico, "OrcamentoOrdemServico/BuscarProdutoPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _produtoOrcamentoServicoOrdemServico.Adicionar.visible(false);
                _produtoOrcamentoServicoOrdemServico.Atualizar.visible(true);
                _produtoOrcamentoServicoOrdemServico.Excluir.visible(true);
                Global.abrirModal("knockoutProduto");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

////*******METODOS*******

function CarregarGridProdutosOrcamento(permiteAlterar) {

    if (permiteAlterar == null)
        permiteAlterar = true;

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarProdutoClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    if (_ordemServico.Situacao.val() != EnumSituacaoOrdemServicoFrota.AgAutorizacao || !permiteAlterar)
        menuOpcoes = null;

    _gridProdutoOrcamentoServicoOrdemServico = new GridView(_servicoOrcamentoOrdemServico.AdicionarProduto.idGrid, "OrcamentoOrdemServico/ConsultarProdutos", _servicoOrcamentoOrdemServico, menuOpcoes, { column: 0, dir: orderDir.desc }, 5);
}

function BuscarServicosOrcamento() {
    LimparCamposOrcamentoOrdemServico();

    executarReST("OrcamentoOrdemServico/ObterDetalhesGeraisOrcamento", { Codigo: _ordemServico.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {

                PreencherObjetoKnout(_resumoOrcamentoOrdemServico, { Data: r.Data.Orcamento });

                _orcamentoOrdemServico.Servicos.removeAll();

                for (var i = 0; i < r.Data.Servicos.length; i++)
                    _orcamentoOrdemServico.Servicos.push(new AbaOrcamentoOrdemServico(r.Data.Servicos[i]));

                if (_ordemServico.Situacao.val() == EnumSituacaoOrdemServicoFrota.Finalizada) {
                    SetarEnableCamposKnockout(_servicoOrcamentoOrdemServico, false);
                    SetarEnableCamposKnockout(_resumoOrcamentoOrdemServico, false);
                }
                else if (_ordemServico.Situacao.val() == EnumSituacaoOrdemServicoFrota.EmManutencao) {
                    SetarEnableCamposKnockout(_servicoOrcamentoOrdemServico, false);
                    SetarEnableCamposKnockout(_resumoOrcamentoOrdemServico, false);
                }
                else {
                    SetarEnableCamposKnockout(_servicoOrcamentoOrdemServico, true);
                    SetarEnableCamposKnockout(_resumoOrcamentoOrdemServico, true);
                }

                if (r.Data.Servicos.length > 0) {
                    $("#" + _orcamentoOrdemServico.Servicos()[0].Codigo.id).trigger("click");
                    $("#divGeralServicosOrdemServico").show();
                    $("#divOrcamentoSemServicos").hide();
                } else {
                    $("#divGeralServicosOrdemServico").hide();
                    $("#divOrcamentoSemServicos").show();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarValorProdutoOrcamentoServicoOrdemServico(calculaValorUnitario) {
    var quantidade = Globalize.parseFloat(_produtoOrcamentoServicoOrdemServico.Quantidade.val());
    var valor = Globalize.parseFloat(_produtoOrcamentoServicoOrdemServico.Valor.val());
    var valorTotal = Globalize.parseFloat(_produtoOrcamentoServicoOrdemServico.ValorTotal.val());

    if (quantidade > 0) {
        if (calculaValorUnitario && valorTotal > 0) {
            var valorNovo = valorTotal / quantidade;
            _produtoOrcamentoServicoOrdemServico.Valor.val(Globalize.format(valorNovo, "n2"));
        } else if (valor > 0) {
            var valorNovo = quantidade * valor;
            _produtoOrcamentoServicoOrdemServico.ValorTotal.val(Globalize.format(valorNovo, "n2"));
        }
    }
}

function LimparCamposProdutoOrcamentoServico() {
    LimparCampos(_produtoOrcamentoServicoOrdemServico);
    _produtoOrcamentoServicoOrdemServico.Adicionar.visible(true);
    _produtoOrcamentoServicoOrdemServico.Atualizar.visible(false);
    _produtoOrcamentoServicoOrdemServico.Excluir.visible(false);
}

function BloquearAlteracaoOrcamento() {
    SetarEnableCamposKnockout(_resumoOrcamentoOrdemServico, false);
    SetarEnableCamposKnockout(_servicoOrcamentoOrdemServico, false);
    CarregarGridProdutosOrcamento(false);

    _resumoOrcamentoOrdemServico.Atualizar.visible(false);
    _servicoOrcamentoOrdemServico.AdicionarProduto.visible(false);
    _servicoOrcamentoOrdemServico.Atualizar.visible(false);
    _servicoOrcamentoOrdemServico.RemoverImagem.visible(false);
    _servicoOrcamentoOrdemServico.AnexarImagem.visible(false);
}

function LimparCamposOrcamentoOrdemServico() {
    SetarEnableCamposKnockout(_resumoOrcamentoOrdemServico, true);
    SetarEnableCamposKnockout(_servicoOrcamentoOrdemServico, true);

    CarregarGridProdutosOrcamento();

    LimparCampos(_resumoOrcamentoOrdemServico);
    LimparCampos(_servicoOrcamentoOrdemServico);
    LimparCampos(_orcamentoOrdemServico);

    _orcamentoOrdemServico.Servicos.removeAll();

    _resumoOrcamentoOrdemServico.Atualizar.visible(true);
    _servicoOrcamentoOrdemServico.AdicionarProduto.visible(true);
    _servicoOrcamentoOrdemServico.Atualizar.visible(true);
    _servicoOrcamentoOrdemServico.RemoverImagem.visible(true);
    _servicoOrcamentoOrdemServico.AnexarImagem.visible(true);
}