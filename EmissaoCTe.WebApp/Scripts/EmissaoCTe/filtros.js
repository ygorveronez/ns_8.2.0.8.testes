var EnumFiltroAverbacaoCTe = {
    Averbados: 1,
    NaoAverbados: 2
};

$(document).ready(function () {
    var today = new Date();
    var yesterday = new Date(today);
    var tomorrow = new Date(today);
    yesterday.setDate(today.getDate() - 1);
    tomorrow.setDate(today.getDate() + 1);

    $("#txtDataEmissaoInicialCTeFiltro").val(Globalize.format(yesterday, "dd/MM/yyyy"));
    $("#txtDataEmissaoFinalCTeFiltro").val(Globalize.format(tomorrow, "dd/MM/yyyy"));

    $("#txtNumeroInicialCTeFiltro").mask("9?999999999");
    $("#txtNumeroFinalCTeFiltro").mask("9?999999999");
    $("#txtNumeroNF").mask("9?999999999");
    $("#txtPlacaCTeFiltro").mask("*******");

    FormatarCampoDate("txtDataEmissaoInicialCTeFiltro");
    FormatarCampoDate("txtDataEmissaoFinalCTeFiltro");

    CarregarConsultadeClientes("btnBuscarRemetenteCTeFiltro", "btnBuscarRemetenteCTeFiltro", RetornoConsultaRemetenteFiltro, true, false);
    CarregarConsultadeClientes("btnBuscarDestinatarioCTeFiltro", "btnBuscarDestinatarioCTeFiltro", RetornoConsultaDestinatarioFiltro, true, false);

    AtualizarGridCTes();

    $("#txtRemetenteCTeFiltro").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                LimparCamposRemetenteFiltro();
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtCPFCNPJRemetenteFiltro").focusout(function () {
        BuscarRemetenteFiltro();
    });

    $("#txtDestinatarioCTeFiltro").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                LimparCamposDestinatarioFiltro();
            } else {
                e.preventDefault();
            }
        }
    });

    $("#selStatusCTeFiltro").change(function () {
        var status = $("#selStatusCTeFiltro").val();

        if (status == "S" || status == "R" || status == "F")
            $("#btnEmitirTodosCTes").show();
        else
            $("#btnEmitirTodosCTes").hide();
    });
    $("#selAverbacaoCTe").change(function () {
        var statusAverbacao = $("#selAverbacaoCTe").val();

        if (statusAverbacao == EnumFiltroAverbacaoCTe.NaoAverbados)
            $("#btnReenviarAverbacaoCTes").show();
        else
            $("#btnReenviarAverbacaoCTes").hide();
    });
    
    $("#txtCPFCNPJDestinatarioFiltro").focusout(function () {
        BuscarDestinatarioFiltro();
    });

    $("#btnConsultarCTe").click(function () {
        AtualizarGridCTes();
    });
    
    $("#btnEmitirTodosCTes").click(function () {
        EmitirTodosCTes();
    });

    $("#btnReenviarAverbacaoCTes").click(function () {
        ReenviarAverbacaoCTes();
    });
});

function DadosFiltro() {
    return {
        inicioRegistros: 0,
        fimRegistros: 20,
        NumeroInicial: $("#txtNumeroInicialCTeFiltro").val(),
        NumeroFinal: $("#txtNumeroFinalCTeFiltro").val(),
        Remetente: $("#hddRemetenteFiltro").val().replace(/[^0-9]/g, ''),
        Destinatario: $("#hddDestinatarioFiltro").val().replace(/[^0-9]/g, ''),
        DataEmissaoInicial: $("#txtDataEmissaoInicialCTeFiltro").val(),
        DataEmissaoFinal: $("#txtDataEmissaoFinalCTeFiltro").val(),
        Placa: $("#txtPlacaCTeFiltro").val(),
        Motorista: $("#txtMotoristaCTeFiltro").val(),
        Status: $("#selStatusCTeFiltro").val(),
        Finalidade: $("#selFinalidadeCTeFiltro").val(),
        TipoOcorrencia: $("#selTipoOcorrencia").val(),
        Serie: $("#ddlSerieFiltro").val(),
        NumeroNF: $("#txtNumeroNF").val(),
        Contem: $("#chkContem")[0].checked,
        AverbacaoCTe: $("#selAverbacaoCTe").val()
    };
}

function ReenviarAverbacaoCTes() {
    var dados = DadosFiltro();

    executarRest("/ConhecimentoDeTransporteEletronico/ReenviarAverbacaoCTes?callback=?", dados, function (r) {
        if (r.Sucesso) ExibirMensagemSucesso("Averbações reenviadas com sucesso.", "Sucesso!");
        else ExibirMensagemErro(r.Erro, "Erro!");
        AtualizarGridCTes();
    });
}

function EmitirTodosCTes() {
    var _limitededias = 2;
    var dados = DadosFiltro();

    if (dados.DataEmissaoInicial == "" || dados.DataEmissaoFinal == "")
        return ExibirMensagemAlerta("Data de início e fim são obrigatórias.", "Atenção");
    
    var invertePadrao = function(data){
        // Converte de data pt pra es
        data = data.split('/');
        data = [data[1], data[0], data[2]].join('/');

        // retorna instancia de data
        return new Date(data);
    }

    var dataInicial = invertePadrao(dados.DataEmissaoInicial);
    var dataFinal = invertePadrao(dados.DataEmissaoFinal);
    var diferencaData = dataFinal.getTime() - dataInicial.getTime();
    
    // Remove ms
    diferencaData = diferencaData / 1000;

    // Remove minutos
    diferencaData = diferencaData / 60;

    // Remove horas
    diferencaData = diferencaData / 60;

    // Remove horas
    diferencaData = diferencaData / 24;

    // Diferenca de data maior que _limitededias dias
    if (diferencaData > _limitededias)
        return ExibirMensagemAlerta("Não é possível emitir CT-es com mais de " + _limitededias + " dia" + (_limitededias>1?"s":"") + " de diferença.", "Atenção");
    
    jConfirm("Deseja realmente emitir todos os CT-es filtrados?", "Atenção", function (ret) {
        if (ret) {
            executarRest("/ConhecimentoDeTransporteEletronico/EmitirTodos?callback=?", dados, function (r) {
                if (r.Sucesso) ExibirMensagemSucesso("CT-es emitidos com sucesso.", "Sucesso!");
                else ExibirMensagemErro(r.Erro, "Erro!");
                AtualizarGridCTes();
            });
        }
    });
}
function AtualizarGridCTes() {
    var dados = DadosFiltro();

    var opcoes = new Array();
    opcoes.push({ Descricao: "Baixar DACTE", Evento: DownloadDacte });
    opcoes.push({ Descricao: "Baixar XML", Evento: DownloadXML });
    opcoes.push({ Descricao: "Baixar XML Cancelamento", Evento: DownloadXMLCancelamento });
    opcoes.push({ Descricao: "Editar", Evento: EditarCTe });
    opcoes.push({ Descricao: "Emitir", Evento: EmitirCTe_Menu });
    opcoes.push({ Descricao: "Duplicar", Evento: DuplicarCTe });
    opcoes.push({ Descricao: "Cancelar CT-e", Evento: AbrirTelaCancelamentoCTe });
    opcoes.push({ Descricao: "Inutilizar CT-e", Evento: AbrirTelaInutilizacaoCTe });
    opcoes.push({ Descricao: "MDF-e", Evento: AbrirTelaConsultaMDFe });
    opcoes.push({ Descricao: "CT-e Anulação", Evento: EmitirCTeAnulacao });
    opcoes.push({ Descricao: "CT-e Complementar", Evento: EmitirCTeComplementar });
    opcoes.push({ Descricao: "CT-e Substituição", Evento: EmitirCTeSubstituicao });
    opcoes.push({ Descricao: "Reenviar e-mails", Evento: EnviarDacteEXMLParaEmailsCadastrados });
    opcoes.push({ Descricao: "Enviar para um e-mail", Evento: AbrirTelaEnvioEmail });
    opcoes.push({ Descricao: "Ocorrências do CT-e", Evento: BuscarOcorrenciasDoCTe });
    opcoes.push({ Descricao: "Averbações do CT-e", Evento: ConsultarAverbacaoCTe });
    opcoes.push({ Descricao: "Integrações do CT-e", Evento: ConsultarIntegracaoRetorno });
    opcoes.push({ Descricao: "Emitir em Contingência", Evento: EmitirCTeContingencia });
    opcoes.push({ Descricao: "Carta de Correção", Evento: EmitirCCe });
    opcoes.push({ Descricao: "Baixar Pré-CT-e", Evento: DownloadPreCTe });
    opcoes.push({ Descricao: "Retornos Sefaz", Evento: RetornosSefaz });

    CriarGridView("/ConhecimentoDeTransporteEletronico/Consultar?callback=?", dados, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", opcoes, [0, 1, 2], null, [2, 3, 4, 5, 6, 7, 8, 10, 12],20);

    jQuery("#btnConsultarCTe").blur();
}

function BuscarRemetenteFiltro() {
    if ($("#hddRemetenteFiltro").val() != $("#txtCPFCNPJRemetenteFiltro").val()) {
        var cpfCnpj = $("#txtCPFCNPJRemetenteFiltro").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("#hddRemetenteFiltro").val(r.Objeto.CPF_CNPJ);
                            $("#txtCPFCNPJRemetenteFiltro").val(r.Objeto.CPF_CNPJ);
                            $("#txtRemetenteCTeFiltro").val(r.Objeto.Nome);
                        } else {
                            LimparCamposRemetenteFiltro();
                            jAlert("Remetente não encontrado.", "Atenção");
                        }
                    } else {
                        LimparCamposRemetenteFiltro();
                        jAlert(r.Erro, "Erro");
                    }
                });
            } else {
                LimparCamposRemetenteFiltro();
                jAlert("O CPF/CNPJ digitado é inválido!", "Atenção");
            }
        } else {
            LimparCamposRemetenteFiltro();
        }
    }
}

function LimparCamposRemetenteFiltro() {
    $("#hddRemetenteFiltro").val("");
    $("#txtCPFCNPJRemetenteFiltro").val("");
    $("#txtRemetenteCTeFiltro").val("");
}

function RetornoConsultaRemetenteFiltro(cliente) {
    $("#hddRemetenteFiltro").val(cliente.CPFCNPJ);
    $("#txtRemetenteCTeFiltro").val(cliente.Nome);
    $("#txtCPFCNPJRemetenteFiltro").val(cliente.CPFCNPJ);
}

function BuscarDestinatarioFiltro() {
    if ($("#txtCPFCNPJDestinatarioFiltro").val() != $("#hddDestinatarioFiltro").val()) {
        var cpfCnpj = $("#txtCPFCNPJDestinatarioFiltro").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("#hddDestinatarioFiltro").val(r.Objeto.CPF_CNPJ);
                            $("#txtCPFCNPJDestinatarioFiltro").val(r.Objeto.CPF_CNPJ);
                            $("#txtDestinatarioCTeFiltro").val(r.Objeto.Nome);
                        } else {
                            LimparCamposDestinatarioFiltro();
                            jAlert("Destinatário não encontrado.", "Atenção");
                        }
                    } else {
                        LimparCamposDestinatarioFiltro();
                        jAlert(r.Erro, "Erro");
                    }
                });
            } else {
                LimparCamposDestinatarioFiltro();
                jAlert("O CPF/CNPJ digitado é inválido!", "Atenção");
            }
        } else {
            LimparCamposDestinatarioFiltro();
        }
    }
}

function LimparCamposDestinatarioFiltro() {
    $("#hddDestinatarioFiltro").val("");
    $("#txtCPFCNPJDestinatarioFiltro").val("");
    $("#txtDestinatarioCTeFiltro").val("");
}

function RetornoConsultaDestinatarioFiltro(cliente) {
    $("#hddDestinatarioFiltro").val(cliente.CPFCNPJ);
    $("#txtDestinatarioCTeFiltro").val(cliente.Nome);
    $("#txtCPFCNPJDestinatarioFiltro").val(cliente.CPFCNPJ);
}