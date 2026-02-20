/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="DadosTransporte.js" />
/// <reference path="Tipo.js" />
/// <reference path="Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

var _percentualExecucao,_currentCarga;

function removerMotoristaClick(knoutMotoristas, data) {

    var motoristasGrid = knoutMotoristas.basicTable.BuscarRegistros();
    for (var i = 0; i < motoristasGrid.length; i++) {
        if (data.Codigo == motoristasGrid[i].Codigo) {
            motoristasGrid.splice(i, 1);
            break;
        }
    }
    knoutMotoristas.basicTable.CarregarGrid(motoristasGrid);
}

//*******MÉTODOS*******

function atualizarGridDadosMotorista(knoutCarga, carga) {
    var motoristas = new Array();
    var infoMotoristas = "";

    if (carga.Motoristas != null) {
        $.each(carga.Motoristas, function (i, motorista) {
            motoristas.push({ Codigo: motorista.Codigo, CPF: motorista.CPF, Nome: motorista.Descricao, PercentualExecucao: motorista.PercentualExecucao });
            if (i > 0) {
                infoMotoristas += ", ";
            }
            infoMotoristas += motorista.Descricao;
        });
    }

    knoutCarga.Motorista.motoristas(infoMotoristas);
    knoutCarga.AdicionarMotoristas.basicTable.CarregarGrid(motoristas);
}

function ajusteDePercentualExecucaoClick(knoutCarga) {
    const gridMotoristas = knoutCarga.AdicionarMotoristas.basicTable;
    const dataGrid = gridMotoristas.BuscarRegistros();

    const dados = dataGrid.map(el => ({ ...el, DT_Enable: true}));

    _percentualExecucao.PercentualExecucao.basicTable.CarregarGrid(dados);

    _currentCarga = knoutCarga;   

    Global.abrirModal('divModalPercentualExecucao');
}

var PercentualExecucao = function () {
    this.PercentualExecucao = PropertyEntity({ type: types.listEntity, visible: ko.observable(true), enable: ko.observable(true), idGrid: guid() });
    this.AjustarPercentual = PropertyEntity({ eventClick: atualizarPercentualClick, text: "Atualizar % de Execucão", type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    //this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
}


function atualizarPercentualClick() {
    const dataGrid = _percentualExecucao.PercentualExecucao.basicTable.BuscarRegistros().map(el => {
        delete el.DT_Enable;
        return el;
    });

    var percentualTotal = 0;
    $.each(dataGrid, function (i, motorista) {       
       percentualTotal = percentualTotal + (isNaN(motorista.PercentualExecucao) ? Globalize.parseFloat(motorista.PercentualExecucao) : motorista.PercentualExecucao);
    });

    if (percentualTotal > 100) {
        exibirMensagem(tipoMensagem.aviso, "Percentual", "A somas dos percentuais não pode ser maior que 100%");
        return;
    }

    const gridMotoristas = _currentCarga.AdicionarMotoristas.basicTable;

    executarReST("Carga/AlterarPercentualExecucao", { Carga: _currentCarga.Codigo.val(), Motoristas: JSON.stringify(dataGrid)}, function (e) {
        if (e.Success) {

            gridMotoristas.CarregarGrid(dataGrid);

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Percentual atualizado com sucesso");
            Global.fecharModal("divModalPercentualExecucao");
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
        }
    });
    
}

function loadMotoristas(knoutCarga, carga) {

    _percentualExecucao = new PercentualExecucao();
    KoBindings(_percentualExecucao, "knockoutPercentualExecucao");

    loadGridPercentualExecucao();

    preecherGridDadosMotorista(knoutCarga, carga);
    
}

function loadGridPercentualExecucao() {
    const editablePercentual = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal(),
        maxlength: 6,
    };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.Carga.CPF, width: "10%", className: "text-align-left", orderable: false },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "50%", className: "text-align-left", orderable: false },
        {
            data: "PercentualExecucao", title: "% Execucão", width: "15%", orderable: false, editableCell: editablePercentual
        },
    ];

    const editarColuna = {
        permite: true,
        callback: SalvarRetornoGrid,
        atualizarRow: true
    };

    var gridPercentualExecucao = new BasicDataTable(_percentualExecucao.PercentualExecucao.idGrid, header, null, { column: 1, dir: orderDir.asc }, null, null, null, null, editarColuna)
    _percentualExecucao.PercentualExecucao.basicTable = gridPercentualExecucao;
}

function SalvarRetornoGrid(dataRow) {

}

function preecherGridDadosMotorista(knoutCarga, carga) {

    var motoristas = new Array();
    var infoMotoristas = "";

    if (carga.Motoristas != null) {
        $.each(carga.Motoristas, function (i, motorista) {
            motoristas.push({ Codigo: motorista.Codigo, CPF: motorista.CPF, Nome: motorista.Descricao, PercentualExecucao: motorista.PercentualExecucao });
            if (i > 0) {
                infoMotoristas += ", ";
            }
            infoMotoristas += motorista.Descricao;
        });
    }

    knoutCarga.Motorista.motoristas(infoMotoristas);

    var remover = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: function (datagrid) {
            removerMotoristaClick(knoutCarga.AdicionarMotoristas, datagrid);
        }, icone: ""
    };
    var menuOpcoes = knoutCarga.EtapaInicioTMS.enable() ? { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [remover] } : null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.Carga.CPF, width: "15%", className: "text-align-center", orderable: false },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "50%", className: "text-align-left", orderable: false },
        { data: "PercentualExecucao", title: "% Execucão", width: "20%", className: "text-align-right", orderable: false, visible: _CONFIGURACAO_TMS.UtilizaControlePercentualExecucao },
    ];

    var gridMotoristas = new BasicDataTable(knoutCarga.AdicionarMotoristas.idGrid, header, menuOpcoes);

    gridMotoristas.CarregarGrid(motoristas);

    knoutCarga.AdicionarMotoristas.basicTable = gridMotoristas;

    new BuscarMotoristas(knoutCarga.AdicionarMotoristas, function (m) { AdicionarMotoristaCargaClick(m, knoutCarga, EnumSituacoesCarga.Nova); }, _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? null : _CONFIGURACAO_TMS.FiltrarBuscaVeiculosPorEmpresa ? knoutCarga.Empresa : null, gridMotoristas, true, EnumSituacaoColaborador.Trabalhando);
}

function AdicionarMotoristaCargaClick(motoristas, knoutCarga, etapa) {
    const gridMotoristas = knoutCarga.AdicionarMotoristas.basicTable;

    if (etapa == EnumSituacoesCarga.Nova) {
        if (gridMotoristas != null) {
            const dataGrid = gridMotoristas.BuscarRegistros();
            const juncao = [].concat(dataGrid, motoristas);
            const aux = [];

            for (let i = 0; i < juncao.length; i++) {
                if (aux.filter(o => o.Codigo === juncao[i].Codigo).length == 0)
                    aux.push({ ...juncao[i], PercentualExecucao: null });
            }

            executarReST("Motorista/ValidarLicencaMotoristas", { Motoristas: JSON.stringify(aux), Validar: knoutCarga.ValidarLicencaMotorista.val() }, function (retorno) {
                if (retorno.Data.validar) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.MotoristasSelecionadosComSucesso);

                        gridMotoristas.CarregarGrid(aux);

                        if (_CONFIGURACAO_TMS.AcoplarMotoristaAoVeiculoAoSelecionarNaCarga === true && dataGrid.length == 1 && knoutCarga.CodigoMotoristaVeiculo.val() != dataGrid[0].Codigo) {
                            const motorista = dataGrid[0];

                            exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.IdentificamosQueMotoristaVinculadoAoVeiculoDiferenteDoSelecionado.format(knoutCarga.NomeMotoristaVeiculo.val()) + "<br/><br/>" + Localization.Resources.Cargas.Carga.DesejaVincularMotoristaAoVeiculo.format(motorista.Nome, knoutCarga.Veiculo.val()), function () {
                                executarReST("Veiculo/VincularMotorista", { Veiculo: knoutCarga.Veiculo.codEntity(), Motorista: motorista.Codigo }, function (retorno) {
                                    if (retorno.Success) {
                                        if (retorno.Data) {
                                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.VinculadoAoVeiculoComSucesso.format(motorista.Nome, knoutCarga.Veiculo.val()));

                                            knoutCarga.NomeMotoristaVeiculo.val(motorista.Nome);
                                            knoutCarga.CodigoMotoristaVeiculo.val(motorista.Codigo);
                                        }
                                        else
                                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                                    }
                                    else
                                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                                });
                            });
                        }
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            });
        }
    }
    else if (etapa == EnumSituacoesCarga.AgTransportador) {
        const dataGrid = gridMotoristas.BuscarRegistros();
        const juncao = [].concat(dataGrid, motoristas);
        const aux = [];

        for (let i = 0; i < juncao.length; i++) {
            const novaPlaca = juncao[i].Placa;
            const auxAtualizado = aux.filter(o => o.Placa !== novaPlaca);
            auxAtualizado.push({ ...juncao[i], PercentualExecucao: null });
            aux.length = 0;
            aux.push(...auxAtualizado);
        }

        executarReST("Motorista/ValidarLicencaMotoristas", { Motoristas: JSON.stringify(aux), Validar: knoutCarga.ValidarLicencaMotorista.val(), Etapa: EnumSituacoesCarga.AgTransportador}, function (retorno) {
            if (retorno.Data.validar) {
                if (retorno.Data) {
                    knoutCarga.Motorista.val(motoristas.Descricao);
                    knoutCarga.Motorista.codEntity(motoristas.Codigo);
                    
                    gridMotoristas.CarregarGrid(aux);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.MotoristaSelecionadoComSucesso);
                } else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        });
    }
}