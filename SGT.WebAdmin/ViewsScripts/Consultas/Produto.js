/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoAbastecimento.js" />


var BuscarProdutos = function (knout, callbackRetorno, knoutGrupoPessoa, knoutPessoa, knoutGrupoProduto, possuiMenu, mostrarFiltrosPesquisa, basicGrid, knoutFilial, knoutClienteBase, produtos) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
        this.CodigoBarrasEAN = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Consultas.Produto.EAN.getFieldDescription() });
        this.GrupoPessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.GrupoProduto = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Produto.GrupoProduto.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Produto.Filial.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.ClienteBase = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Produto.Cliente.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Ativo = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(1), visible: false });
        this.Produtos = PropertyEntity({ val: ko.observableArray([]), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutGrupoPessoa != null || knoutGrupoProduto != null || knoutPessoa != null || knoutFilial != null || knoutClienteBase != null || knoutClienteBase != null || produtos != null) {
        funcaoParametroDinamico = function () {
            if (knoutGrupoPessoa != null) {
                knoutOpcoes.GrupoPessoa.codEntity(knoutGrupoPessoa.codEntity());
                knoutOpcoes.GrupoPessoa.val(knoutGrupoPessoa.val());
            }
            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }
            if (knoutGrupoProduto != null) {
                knoutOpcoes.GrupoProduto.codEntity(knoutGrupoProduto.codEntity());
                knoutOpcoes.GrupoProduto.val(knoutGrupoProduto.val());
            }
            if (knoutFilial != null) {
                knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
                knoutOpcoes.Filial.val(knoutFilial.val());
            }
            if (knoutClienteBase != null) {
                knoutOpcoes.ClienteBase.codEntity(knoutClienteBase.codEntity());
                knoutOpcoes.ClienteBase.val(knoutClienteBase.val());
            }
            if (produtos != null) {
                knoutOpcoes.Produtos.val(produtos);
            }
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, null, mostrarFiltrosPesquisa);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e, knout);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ProdutoEmbarcador/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ProdutoEmbarcador/Pesquisa", knoutOpcoes, (possuiMenu === false ? null : divBusca.OpcaoPadrao(callback)), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    return { ModalBusca: divBusca, Grid: GridConsulta };
}

var BuscarTipoCombustiveis = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Codigo = PropertyEntity({ col: 4, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaTipoCombustivel", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarNCMS = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarNCMs, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.NCMs, type: types.local });
        this.Descricao = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Produto.NCM.getFieldDescription(), maxlength: 8 });
        this.CodigoNCM = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 200 });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaNCMS", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarProdutoTMS = function (knout, callbackRetorno, knoutTipoAbastecimento, knoutNaturezaOperacao, knoutOrdemCompra, somenteComEstoque, permitirAdicionarProduto) {

    var idDiv = guid();
    var GridConsulta;

    if (permitirAdicionarProduto == null)
        permitirAdicionarProduto = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });

        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), visible: !IsMobile() });
        this.CodigoBarrasEAN = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Consultas.Produto.EAN.getFieldDescription(), visible: !IsMobile() });
        this.CodigoNCM = PropertyEntity({ col: 2, maxlength: 8, text: Localization.Resources.Consultas.Produto.NCM.getFieldDescription(), visible: !IsMobile() });
        this.Ativo = PropertyEntity({ col: 2, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), options: _statusPesquisa, def: 1, val: ko.observable(1), visible: !IsMobile() });

        this.TipoAbastecimento = PropertyEntity({ col: 12, visible: false });
        this.NaturezaOperacao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.OrdemCompra = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.SomenteComEstoque = PropertyEntity({ getType: typesKnockout.bool, visible: false, val: ko.observable(false), def: ko.observable(false) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
        this.AdicionarProduto = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Produto.AdicionarNovoProduto, visible: permitirAdicionarProduto, cssClass: "btn btn-default", icon: " " });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutTipoAbastecimento != null || knoutNaturezaOperacao != null || knoutOrdemCompra != null || somenteComEstoque != null) {
        funcaoParametroDinamico = function () {
            if (knoutTipoAbastecimento != null) {
                knoutOpcoes.TipoAbastecimento.val(knoutTipoAbastecimento.val());
            }
            if (knoutNaturezaOperacao != null) {
                knoutOpcoes.NaturezaOperacao.codEntity(knoutNaturezaOperacao.codEntity());
                knoutOpcoes.NaturezaOperacao.val(knoutNaturezaOperacao.val());
            }
            if (knoutOrdemCompra != null) {
                knoutOpcoes.OrdemCompra.codEntity(knoutOrdemCompra.codEntity());
                knoutOpcoes.OrdemCompra.val(knoutOrdemCompra.val());
            }
            if (somenteComEstoque != null) {
                knoutOpcoes.SomenteComEstoque.val(somenteComEstoque)
            }
        };
    }

    if (permitirAdicionarProduto) {

        var fnAbrirModalAdicionarProduto = function (e, knoutCadastro, modalCadastro, produto) {

            LimparCampos(produto);

            produto.Descricao.val(e.Descricao.val());
            produto.CodigoBarrasEAN.val(e.CodigoBarrasEAN.val());
            produto.CodigoNCM.val(e.CodigoNCM.val());
                        
            Global.abrirModal(modalCadastro);
        };

        var fnCodigoNCMExit = function (produto) {
            if ($("#" + produto.CodigoNCM.id).val() != "") {
                produto.CodigoNCM.codEntity($("#" + produto.CodigoNCM.id).val());
            }
        };

        var knoutCadastro = idDiv + "knockoutCadastroProduto";
        var modalCadastro = idDiv + "divModalCadastrarProduto";
        $.get("Content/Static/Consultas/Cadastros/Produto.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroProduto/g, knoutCadastro).replace(/#divModalCadastrarProduto/g, modalCadastro);
            $('#js-page-content').append(html);
            var produto = new knockoutCadastroProduto();

            produto.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            produto.Adicionar.eventClick = function (e) {
                if (produto.CodigoNCM.val() == "") {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o NCM do produto!");
                    produto.CodigoNCM.requiredClass("form-control is-invalid");
                    return;
                }

                Salvar(e, "Produto/AdicionarProdutoSimplificado", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            };

            produto.CodigoNCM.fnFocusOut = function (produto) {
                fnCodigoNCMExit(produto);
            };

            KoBindings(produto, knoutCadastro, false);

            new BuscarNCMS(produto.CodigoNCM, function (e) {
                produto.CodigoNCM.val(e.Descricao);
                produto.CodigoNCM.codEntity(e.Descricao);
            });

            knoutOpcoes.AdicionarProduto.eventClick = function (e) {
                fnAbrirModalAdicionarProduto(e, knoutCadastro, modalCadastro, produto);
            };
        });
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            if (!(/^\d+$/.test(knout.val()))) {
                knoutOpcoes.CodigoProdutoEmbarcador.val("");
                knoutOpcoes.Descricao.val(knout.val());
            }
            else if (/^\d+$/.test(knout.val())) {
                knoutOpcoes.Descricao.val("");
                knoutOpcoes.CodigoProdutoEmbarcador.val(knout.val());
            }
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};

var BuscarProdutoTMSComEstoque = function (knout, callbackRetorno, knoutNaturezaOperacao, permitirAdicionarProduto) {

    var idDiv = guid();
    var GridConsulta;

    if (permitirAdicionarProduto == null)
        permitirAdicionarProduto = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });

        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), visible: !IsMobile() });
        this.CodigoBarrasEAN = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Consultas.Produto.EAN.getFieldDescription(), visible: !IsMobile() });
        this.CodigoNCM = PropertyEntity({ col: 2, maxlength: 8, text: Localization.Resources.Consultas.Produto.NCM.getFieldDescription(), visible: !IsMobile() });

        this.NaturezaOperacao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
        this.AdicionarProduto = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Produto.AdicionarNovoProduto, visible: permitirAdicionarProduto, cssClass: "btn btn-default", icon: " " });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutNaturezaOperacao != null) {
        funcaoParametroDinamico = function () {
            if (knoutNaturezaOperacao != null) {
                knoutOpcoes.NaturezaOperacao.codEntity(knoutNaturezaOperacao.codEntity());
                knoutOpcoes.NaturezaOperacao.val(knoutNaturezaOperacao.val());
            }
        };
    }

    if (permitirAdicionarProduto) {

        var fnAbrirModalAdicionarProduto = function (e, knoutCadastro, modalCadastro, produto) {

            LimparCampos(produto);

            produto.Descricao.val(e.Descricao.val());
            produto.CodigoBarrasEAN.val(e.CodigoBarrasEAN.val());
            produto.CodigoNCM.val(e.CodigoNCM.val());
                        
            Global.abrirModal(modalCadastro);
        };

        var fnCodigoNCMExit = function (produto) {
            if ($("#" + produto.CodigoNCM.id).val() != "") {
                produto.CodigoNCM.codEntity($("#" + produto.CodigoNCM.id).val());
            }
        };

        var knoutCadastro = idDiv + "knockoutCadastroProduto";
        var modalCadastro = idDiv + "divModalCadastrarProduto";
        $.get("Content/Static/Consultas/Cadastros/Produto.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroProduto/g, knoutCadastro).replace(/#divModalCadastrarProduto/g, modalCadastro);
            $('#js-page-content').append(html);
            var produto = new knockoutCadastroProduto();

            produto.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            produto.Adicionar.eventClick = function (e) {
                produto.CodigoNCM.requiredClass("form-control");
                if (produto.CodigoNCM.val() == "") {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o NCM do produto!");
                    produto.CodigoNCM.requiredClass("form-control is-invalid");
                    return;
                }

                Salvar(e, "Produto/AdicionarProdutoSimplificado", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            };

            produto.CodigoNCM.fnFocusOut = function (produto) {
                fnCodigoNCMExit(produto);
            };

            KoBindings(produto, knoutCadastro, false);

            new BuscarNCMS(produto.CodigoNCM, function (e) {
                produto.CodigoNCM.val(e.Descricao);
                produto.CodigoNCM.codEntity(e.Descricao);
            });

            knoutOpcoes.AdicionarProduto.eventClick = function (e) {
                fnAbrirModalAdicionarProduto(e, knoutCadastro, modalCadastro, produto);
            };
        });
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaComEstoque", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            if (!(/^\d+$/.test(knout.val()))) {
                knoutOpcoes.CodigoProdutoEmbarcador.val("");
                knoutOpcoes.Descricao.val(knout.val());
            }
            else if (/^\d+$/.test(knout.val())) {
                knoutOpcoes.Descricao.val("");
                knoutOpcoes.CodigoProdutoEmbarcador.val(knout.val());
            }
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};

var BuscarProdutoTMSComEstoqueAgrupado = function (knout, callbackRetorno, knoutNaturezaOperacao, permitirAdicionarProduto) {

    var idDiv = guid();
    var GridConsulta;

    if (permitirAdicionarProduto == null)
        permitirAdicionarProduto = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });

        this.Descricao = PropertyEntity({ col: 4, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Codigo = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), visible: !IsMobile() });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 2, maxlength: 50, text: "Cód. Produto", visible: !IsMobile() });
        this.CodigoBarrasEAN = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Consultas.Produto.EAN.getFieldDescription(), visible: !IsMobile() });
        this.CodigoNCM = PropertyEntity({ col: 2, maxlength: 8, text: Localization.Resources.Consultas.Produto.NCM.getFieldDescription(), visible: !IsMobile() });

        this.NaturezaOperacao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
        this.AdicionarProduto = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Produto.AdicionarNovoProduto, visible: permitirAdicionarProduto, cssClass: "btn btn-default", icon: " " });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutNaturezaOperacao != null) {
        funcaoParametroDinamico = function () {
            if (knoutNaturezaOperacao != null) {
                knoutOpcoes.NaturezaOperacao.codEntity(knoutNaturezaOperacao.codEntity());
                knoutOpcoes.NaturezaOperacao.val(knoutNaturezaOperacao.val());
            }
        };
    }

    if (permitirAdicionarProduto) {

        var fnAbrirModalAdicionarProduto = function (e, knoutCadastro, modalCadastro, produto) {

            LimparCampos(produto);

            produto.Descricao.val(e.Descricao.val());
            produto.CodigoBarrasEAN.val(e.CodigoBarrasEAN.val());
            produto.CodigoNCM.val(e.CodigoNCM.val());

            Global.abrirModal(modalCadastro);
        };

        var fnCodigoNCMExit = function (produto) {
            if ($("#" + produto.CodigoNCM.id).val() != "") {
                produto.CodigoNCM.codEntity($("#" + produto.CodigoNCM.id).val());
            }
        };

        var knoutCadastro = idDiv + "knockoutCadastroProduto";
        var modalCadastro = idDiv + "divModalCadastrarProduto";
        $.get("Content/Static/Consultas/Cadastros/Produto.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroProduto/g, knoutCadastro).replace(/#divModalCadastrarProduto/g, modalCadastro);
            $('#js-page-content').append(html);
            var produto = new knockoutCadastroProduto();

            produto.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            produto.Adicionar.eventClick = function (e) {
                produto.CodigoNCM.requiredClass("form-control");
                if (produto.CodigoNCM.val() == "") {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o NCM do produto!");
                    produto.CodigoNCM.requiredClass("form-control is-invalid");
                    return;
                }

                Salvar(e, "Produto/AdicionarProdutoSimplificado", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            };

            produto.CodigoNCM.fnFocusOut = function (produto) {
                fnCodigoNCMExit(produto);
            };

            KoBindings(produto, knoutCadastro, false);

            new BuscarNCMS(produto.CodigoNCM, function (e) {
                produto.CodigoNCM.val(e.Descricao);
                produto.CodigoNCM.codEntity(e.Descricao);
            });

            knoutOpcoes.AdicionarProduto.eventClick = function (e) {
                fnAbrirModalAdicionarProduto(e, knoutCadastro, modalCadastro, produto);
            };
        });
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaComEstoqueAgrupado", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            if (!(/^\d+$/.test(knout.val()))) {
                knoutOpcoes.CodigoProdutoEmbarcador.val("");
                knoutOpcoes.Descricao.val(knout.val());
            }
            else if (/^\d+$/.test(knout.val())) {
                knoutOpcoes.Descricao.val("");
                knoutOpcoes.CodigoProdutoEmbarcador.val(knout.val());
            }
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};

var BuscarProdutoEstoque = function (knout, callbackRetorno, knoutFilial, permitirAdicionarProduto) {
    var idDiv = guid();
    var GridConsulta;

    if (permitirAdicionarProduto == null)
        permitirAdicionarProduto = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });

        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
        this.CodigoBarrasEAN = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Consultas.Produto.EAN.getFieldDescription() });
        this.Filial = PropertyEntity({ col: 12, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
        this.AdicionarProduto = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Produto.AdicionarNovoProduto, visible: permitirAdicionarProduto, cssClass: "btn btn-default", icon: " " });
    }
    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutFilial != null) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.Filial.val(knoutFilial.codEntity());
        }
    }

    if (permitirAdicionarProduto) {

        var fnAbrirModalAdicionarProduto = function (e, knoutCadastro, modalCadastro, produto) {

            LimparCampos(produto);

            produto.Descricao.val(e.Descricao.val());
            produto.CodigoBarrasEAN.val(e.CodigoBarrasEAN.val());
                        
            Global.abrirModal(modalCadastro);
        };

        var fnCodigoNCMExit = function (produto) {
            if ($("#" + produto.CodigoNCM.id).val() != "") {
                produto.CodigoNCM.codEntity($("#" + produto.CodigoNCM.id).val());
            }
        };

        var knoutCadastro = idDiv + "knockoutCadastroProduto";
        var modalCadastro = idDiv + "divModalCadastrarProduto";
        $.get("Content/Static/Consultas/Cadastros/Produto.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroProduto/g, knoutCadastro).replace(/#divModalCadastrarProduto/g, modalCadastro);
            $('#js-page-content').append(html);
            var produto = new knockoutCadastroProduto();

            produto.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            produto.Adicionar.eventClick = function (e) {
                if (produto.CodigoNCM.val() == "") {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o NCM do produto!");
                    produto.CodigoNCM.requiredClass("form-control is-invalid");
                    return;
                }

                Salvar(e, "Produto/AdicionarProdutoSimplificado", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            };

            produto.CodigoNCM.fnFocusOut = function (produto) {
                fnCodigoNCMExit(produto);
            };

            KoBindings(produto, knoutCadastro, false);

            new BuscarNCMS(produto.CodigoNCM, function (e) {
                produto.CodigoNCM.val(e.Descricao);
                produto.CodigoNCM.codEntity(e.Descricao);
            });

            knoutOpcoes.AdicionarProduto.eventClick = function (e) {
                fnAbrirModalAdicionarProduto(e, knoutCadastro, modalCadastro, produto);
            };
        });
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaProdutoEstoque", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            if (!(/^\d+$/.test(knout.val()))) {
                knoutOpcoes.CodigoProdutoEmbarcador.val("");
                knoutOpcoes.Descricao.val(knout.val());
            }
            else if (/^\d+$/.test(knout.val())) {
                knoutOpcoes.Descricao.val("");
                knoutOpcoes.CodigoProdutoEmbarcador.val(knout.val());
            }
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}

var BuscarProdutoTMSEstoqueMinimo = function (knout, callbackRetorno, knoutFilial, basicGrid) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var _situacaoEstoqueMinimo = [
        { text: Localization.Resources.Consultas.Produto.ComEstoqueMinimo, value: true },
        { text: Localization.Resources.Consultas.Produto.SemEstoqueMinimo, value: false }
    ];

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutosEstoqueMinimo, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
        this.CodigoBarrasEAN = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Consultas.Produto.EAN.getFieldDescription() });
        this.GrupoProdutoTMS = PropertyEntity({ col: 4, maxlength: 50, type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto", idBtnSearch: guid(), visible: ko.observable(true) });
        this.SomenteComEstoque = PropertyEntity({ col: 3, maxlength: 50, val: ko.observable(true), options: _situacaoEstoqueMinimo, def: true, text: Localization.Resources.Consultas.Produto.SomenteEstoqueMinimo.getFieldDescription() });
        this.Filial = PropertyEntity({ col: 12, visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }
    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutFilial != null) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.Filial.val(knoutFilial.codEntity());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {
        new BuscarGruposProdutosTMS(knoutOpcoes.GrupoProdutoTMS)
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaEstoqueMinimo", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaEstoqueMinimo", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}

var BuscarTipoCombustiveisTMS = function (knout, callbackRetorno, knoutSomenteAbastecimentos) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });

        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 200 });
        this.Codigo = PropertyEntity({ col: 4, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), maxlength: 11, getType: typesKnockout.int });
        this.TipoAbastecimento = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Produto.TipoAbastecimento.getFieldDescription(), val: ko.observable(EnumTipoAbastecimento.Todos), options: EnumTipoAbastecimento.obterOpcoesPesquisa(), def: EnumTipoAbastecimento.Todos });

        this.SomenteAbastecimentos = PropertyEntity({ col: 12, val: ko.observable(false), visible: false, getType: typesKnockout.bool });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutSomenteAbastecimentos !== null && knoutSomenteAbastecimentos !== undefined) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.SomenteAbastecimentos.val(knoutSomenteAbastecimentos);
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaTipoCombustivelTMS", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCESTS = function (knout, callbackRetorno, knoutCodigoNCM) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarCESTs, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.CESTs, type: types.local });
        this.CodigoCEST = PropertyEntity({ col: 3, maxlength: 7, text: Localization.Resources.Consultas.Produto.CEST.getFieldDescription() });
        this.CodigoNCM = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Produto.NCM.getFieldDescription(), maxlength: 8 });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 200 });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutCodigoNCM != null) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.CodigoNCM.val(knoutCodigoNCM.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaCESTS", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCEST.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarProdutoAvaria = function (knout, callbackRetorno, knoutCarga) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 4, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametroDinamico = null;

    if (knoutCarga != null) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }
    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ProdutoAvaria/ProdutoSolicitacaoAvaria", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarProdutoTMSTabelaPreco = function (knout, callbackRetorno, knoutPessoa, somenteComEstoque) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Produto.PesquisarProdutos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Produto.Produtos, type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Codigo = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
        this.CodigoBarrasEAN = PropertyEntity({ col: 2, maxlength: 50, text: Localization.Resources.Consultas.Produto.EAN.getFieldDescription() });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.SomenteComEstoque = PropertyEntity({ getType: typesKnockout.bool, visible: false, val: ko.observable(false), def: ko.observable(false) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutPessoa != null || somenteComEstoque != null) {
        funcaoParametroDinamico = function () {
            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }
            if (somenteComEstoque != null) {
                knoutOpcoes.SomenteComEstoque.val(somenteComEstoque)
            }
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Produto/PesquisaComTabelaPrecoProduto", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var knockoutCadastroProduto = function () {

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.CodigoProduto = PropertyEntity({ text: "Cód. Produto: ", required: false, maxlength: 50 });
    this.CodigoBarrasEAN = PropertyEntity({ text: "Cód. Barras EAN: ", required: false, maxlength: 14 });
    this.CodigoNCM = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*NCM:"), idBtnSearch: guid(), enable: ko.observable(true), issue: 139 });
    this.UnidadeMedida = PropertyEntity({ val: ko.observable(EnumUnidadeMedida.Quilograma), options: EnumUnidadeMedida.obterOpcoes(), text: "*Unidade de Medida: ", def: EnumUnidadeMedida.Quilograma, issue: 88 });
    this.CategoriaProduto = PropertyEntity({ val: ko.observable(EnumCategoriaProduto.MercadoriaRevenda), options: EnumCategoriaProduto.obterOpcoesComNumero(), def: EnumCategoriaProduto.MercadoriaRevenda, text: "*Tipo Item: " });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    var $this = this;
};