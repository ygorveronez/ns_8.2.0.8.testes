/// <reference path="NFSe.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />

var ListaServico = function (nfse) {

    var instancia = this;

    this.ItensNFSe = PropertyEntity({ type: types.local, id: guid() });

    this.Load = function () {
        nfse.Servicos = new Array();

        KoBindings(instancia, nfse.IdKnockoutListaServico);

        var editarItem = { descricao: "Editar", id: guid(), metodo: instancia.Editar, icone: "" };
        var excluirItem = { descricao: "Excluir", id: guid(), metodo: instancia.Excluir, icone: "" };

        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [editarItem, excluirItem] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoServico", visible: false },
            { data: "Descricao", title: "Serviço", width: "30%" },
            { data: "CodigoLocalidade", visible: false },
            { data: "DescricaoLocalidade", title: "Localidade", width: "23%" },
            { data: "Qtd", title: "Quantidade", width: "8%" },
            { data: "ValorUnitario", title: "Valor Unit.", width: "10%" },
            { data: "Deducao", visible: false },
            { data: "DescontoIncondicional", visible: false },
            { data: "DescontoCondicional", visible: false },
            { data: "ValorTotal", title: "Total", width: "10%" },
            { data: "BCISS", visible: false },
            { data: "AliquotaISS", visible: false },
            { data: "ValorISS", title: "Valor ISS", width: "8%" },
            { data: "CodigoExigibilidade", visible: false },
            { data: "Discriminacao", visible: false },
            { data: "CSTPIS", visible: false },
            { data: "CSTCOFINS", visible: false },
            { data: "BasePIS", visible: false },
            { data: "BaseCOFINS", visible: false },
            { data: "AliquotaPIS", visible: false },
            { data: "AliquotaCOFINS", visible: false },
            { data: "ValorPIS", visible: false },
            { data: "ValorCOFINS", visible: false }
        ];

        nfse.GridServico = new BasicDataTable(instancia.ItensNFSe.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarListaServico = function () {
        DesabilitarCamposInstanciasNFSe(instancia);
        nfse.GridServico.CarregarGrid(nfse.Servicos, false);
    };

    this.HabilitarListaServico = function () {
        HabilitarCamposInstanciasNFSe(instancia);
        nfse.GridServico.CarregarGrid(nfse.Servicos, false);
    };

    this.Excluir = function (produtoServico) {

        if (nfse.Servico.Codigo.val() > 0) {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
            return;
        }
        if (nfse.Servico.ServicoItem.val() != "") {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
            return;
        }

        exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + produtoServico.Descricao + "?", function () {
            for (var i = 0; i < nfse.Servicos.length; i++) {
                if (produtoServico.Codigo == nfse.Servicos[i].Codigo) {

                    instancia.RemoverValorItemValor(produtoServico);

                    nfse.Servicos.splice(i, 1);
                    break;
                }
            }
            instancia.RecarregarGrid();
        });
    };

    this.Editar = function (produtoServico) {

        if (nfse.Servico.Codigo.val() > 0) {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
            return
        }
        if (nfse.Servico.ServicoItem.val() != "") {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar o mesmo.");
            return
        }

        LimparCampos(nfse.Servico);
        LimparCampos(nfse.ListaServico);

        instancia.RemoverValorItemValor(produtoServico);

        nfse.Servico.Codigo.val(produtoServico.Codigo);
        nfse.Servico.ServicoItem.codEntity(produtoServico.CodigoServico);
        nfse.Servico.ServicoItem.val(produtoServico.Descricao);
        nfse.Servico.LocalidadeServico.codEntity(produtoServico.CodigoLocalidade);
        nfse.Servico.LocalidadeServico.val(produtoServico.DescricaoLocalidade);
        nfse.Servico.Quantidade.val(Globalize.format(produtoServico.Qtd, "n4"));
        nfse.Servico.ValorUnitario.val(Globalize.format(produtoServico.ValorUnitario, "n5"));
        nfse.Servico.ValorTotalItem.val(Globalize.format(produtoServico.ValorTotal, "n2"));
        nfse.Servico.Discriminacao.val(produtoServico.Discriminacao);

        nfse.Servico.BaseISS.val(Globalize.format(produtoServico.BCISS, "n2"));
        nfse.Servico.AliquotaISS.val(Globalize.format(produtoServico.AliquotaISS, "n2"));
        nfse.Servico.ValorISS.val(Globalize.format(produtoServico.ValorISS, "n2"));
        nfse.Servico.Deducao.val(Globalize.format(produtoServico.Deducao, "n2"));
        nfse.Servico.DescontoIncondicional.val(Globalize.format(produtoServico.DescontoIncondicional, "n2"));
        nfse.Servico.DescontoCondicional.val(Globalize.format(produtoServico.DescontoCondicional, "n2"));
        nfse.Servico.Exigibilidade.val(produtoServico.CodigoExigibilidade);

        nfse.Servico.CSTPIS.val(produtoServico.CSTPIS);
        nfse.Servico.CSTCOFINS.val(produtoServico.CSTCOFINS);
        nfse.Servico.BasePIS.val(Globalize.format(produtoServico.BasePIS, "n2"));
        nfse.Servico.BaseCOFINS.val(Globalize.format(produtoServico.BaseCOFINS, "n2"));
        nfse.Servico.AliquotaPIS.val(Globalize.format(produtoServico.AliquotaPIS, "n4"));
        nfse.Servico.AliquotaCOFINS.val(Globalize.format(produtoServico.AliquotaCOFINS, "n4"));
        nfse.Servico.ValorPIS.val(Globalize.format(produtoServico.ValorPIS, "n2"));
        nfse.Servico.ValorCOFINS.val(Globalize.format(produtoServico.ValorCOFINS, "n2"));

        // NOVOS CAMPOS IBS/CBS (itens)
        nfse.Servico.NBS.val(produtoServico.NBS || "");
        nfse.Servico.CodigoIndicadorOperacao.val(produtoServico.CodigoIndicadorOperacao || "");
        nfse.Servico.CSTIBSCBS.val(produtoServico.CSTIBSCBS || "");
        nfse.Servico.ClassificacaoTributariaIBSCBS.val(produtoServico.ClassificacaoTributariaIBSCBS || "");

        nfse.Servico.BaseCalculoIBSCBS.val(Globalize.format(produtoServico.BaseCalculoIBSCBS || 0, "n2"));

        nfse.Servico.AliquotaIBSEstadual.val(Globalize.format(produtoServico.AliquotaIBSEstadual || 0, "n2"));
        nfse.Servico.PercentualReducaoIBSEstadual.val(Globalize.format(produtoServico.PercentualReducaoIBSEstadual || 0, "n3"));
        nfse.Servico.ValorIBSEstadual.val(Globalize.format(produtoServico.ValorIBSEstadual || 0, "n3"));
        nfse.Servico.ValorIBSEstadualBruto.val(Globalize.format(produtoServico.ValorIBSEstadualBruto || 0, "n3"));

        nfse.Servico.AliquotaIBSMunicipal.val(Globalize.format(produtoServico.AliquotaIBSMunicipal || 0, "n2"));
        nfse.Servico.PercentualReducaoIBSMunicipal.val(Globalize.format(produtoServico.PercentualReducaoIBSMunicipal || 0, "n3"));
        nfse.Servico.ValorIBSMunicipal.val(Globalize.format(produtoServico.ValorIBSMunicipal || 0, "n3"));
        nfse.Servico.ValorIBSMunicipalBruto.val(Globalize.format(produtoServico.ValorIBSMunicipalBruto || 0, "n3"));

        nfse.Servico.AliquotaCBS.val(Globalize.format(produtoServico.AliquotaCBS || 0, "n2"));
        nfse.Servico.PercentualReducaoCBS.val(Globalize.format(produtoServico.PercentualReducaoCBS || 0, "n3"));
        nfse.Servico.ValorCBS.val(Globalize.format(produtoServico.ValorCBS || 0, "n3"));
        nfse.Servico.ValorCBSBruto.val(Globalize.format(produtoServico.ValorCBSBruto || 0, "n3"));

        nfse.Servico.AliquotaEfetivaIBSEstadual.val(Globalize.format(produtoServico.AliquotaEfetivaIBSEstadual || 0, "n2"));
        nfse.Servico.AliquotaEfetivaIBSMunicipal.val(Globalize.format(produtoServico.AliquotaEfetivaIBSMunicipal || 0, "n2"));
        nfse.Servico.AliquotaEfetivaCBS.val(Globalize.format(produtoServico.AliquotaEfetivaCBS || 0, "n2"));

        $("#tabItensNFSe_" + nfse.NFSe.IdModal.val() + " a:eq(0)").tab("show");
        $("#" + nfse.Servico.id).focus();
    };

    this.RemoverValorItemValor = function (produtoServico) {
        for (var i = 0; i < nfse.Servicos.length; i++) {
            if (produtoServico.Codigo == nfse.Servicos[i].Codigo) {

                var baseISSItem = Globalize.parseFloat(produtoServico.BCISS);
                var baseISSTotal = Globalize.parseFloat(nfse.Valor.BaseISS.val());
                var baseISS = baseISSTotal - baseISSItem;
                nfse.Valor.BaseISS.val(Globalize.format(baseISS, "n2"));

                var aliquotaISSItem = Globalize.parseFloat(produtoServico.AliquotaISS);
                /*var aliquotaISSTotal = Globalize.parseFloat(nfse.Valor.AliquotaISS.val());
                var aliquotaISS = aliquotaISSTotal - aliquotaISSItem;
                nfse.Valor.AliquotaISS.val(Globalize.format(aliquotaISS, "n2"));*/

                var valorISSItem = Globalize.parseFloat(produtoServico.ValorISS);
                var valorISSTotal = Globalize.parseFloat(nfse.Valor.ValorISS.val());
                var valorISS = valorISSTotal - valorISSItem;
                nfse.Valor.ValorISS.val(Globalize.format(valorISS, "n2"));

                var baseDeducaoItem = Globalize.parseFloat(produtoServico.Deducao);
                var baseDeducaoTotal = Globalize.parseFloat(nfse.Valor.BaseDeducao.val());
                var baseDeducao = baseDeducaoTotal - baseDeducaoItem;
                nfse.Valor.BaseDeducao.val(Globalize.format(baseDeducao, "n2"));

                var valorDescontoIncondicionalItem = Globalize.parseFloat(produtoServico.DescontoIncondicional);
                var valorDescontoIncondicionalTotal = Globalize.parseFloat(nfse.Valor.ValorDescontoIncondicional.val());
                var valorDescontoIncondicional = valorDescontoIncondicionalTotal - valorDescontoIncondicionalItem;
                nfse.Valor.ValorDescontoIncondicional.val(Globalize.format(valorDescontoIncondicional, "n2"));

                var valorDescontoCondicionalItem = Globalize.parseFloat(produtoServico.DescontoCondicional);
                var valorDescontoCondicionalTotal = Globalize.parseFloat(nfse.Valor.ValorDescontoCondicional.val());
                var valorDescontoCondicional = valorDescontoCondicionalTotal - valorDescontoCondicionalItem;
                nfse.Valor.ValorDescontoCondicional.val(Globalize.format(valorDescontoCondicional, "n2"));

                var valorTotalItem = Globalize.parseFloat(produtoServico.ValorTotal);
                var valorTotalServicosTotal = Globalize.parseFloat(nfse.Valor.ValorTotalServicos.val());
                var valorTotalServicos = valorTotalServicosTotal - valorTotalItem;
                nfse.Valor.ValorTotalServicos.val(Globalize.format(valorTotalServicos, "n2"));

                var valorISSRetido = 0;
                if (nfse.Valor.RetencaoISS.val() === EnumSimNao.Sim) {
                    var valorISSRetidoTotal = Globalize.parseFloat(nfse.Valor.ValorRetencaoISS.val());
                    var valorRetencaoCalculadoItem = baseISSItem * (aliquotaISSItem / 100);
                    valorISSRetido = valorISSRetidoTotal - valorRetencaoCalculadoItem;
                    if (valorISSRetido < 0)
                        valorISSRetido = 0;
                    nfse.Valor.ValorRetencaoISS.val(Globalize.format(valorISSRetido, "n2"));
                }

                var valorliquidoTotal = valorTotalServicos - valorISSRetido;
                nfse.Valor.ValorTotalLiquido.val(Globalize.format(valorliquidoTotal, "n2"));
                nfse.Valor.RetencaoISS.enable(false);

                var basePis = Globalize.parseFloat(produtoServico.BasePIS);
                var basePisTotal = Globalize.parseFloat(nfse.Valor.BasePIS.val());
                var novaBasePis = basePisTotal - basePis;
                if (novaBasePis < 0) 
                    novaBasePis = 0

                nfse.Valor.BasePIS.val(Globalize.format(novaBasePis, "n2"));

                var baseCofins = Globalize.parseFloat(produtoServico.BaseCOFINS);
                var baseCofinsTotal = Globalize.parseFloat(nfse.Valor.BaseCOFINS.val());
                var novaBaseCofins = baseCofinsTotal - baseCofins;
                if (novaBaseCofins < 0) 
                    novaBaseCofins = 0

                nfse.Valor.BaseCOFINS.val(Globalize.format(novaBaseCofins, "n2"));
                nfse.Valor.AliquotaPIS.val(Globalize.format(produtoServico.AliquotaPIS, "n4"));
                nfse.Valor.AliquotaCOFINS.val(Globalize.format(produtoServico.AliquotaCOFINS, "n4"));

                var aliquotaPIS = Globalize.parseFloat(produtoServico.AliquotaPIS) / 100;
                var valorPIS = novaBasePis * aliquotaPIS;
                if (valorPIS < 0)
                    valorPIS = 0;

                nfse.Valor.ValorPIS.val(Globalize.format(valorPIS, "n2"));

                var aliquotaCOFINS = Globalize.parseFloat(produtoServico.AliquotaCOFINS) / 100;
                var valorCOFINS = novaBaseCofins * aliquotaCOFINS;
                if (valorCOFINS < 0)
                    valorCOFINS = 0;
                     
                nfse.Valor.ValorCOFINS.val(Globalize.format(valorCOFINS, "n2"));

                break;
            }
        }
    };

    this.RecarregarGrid = function () {
        nfse.GridServico.CarregarGrid(nfse.Servicos);
    };
}