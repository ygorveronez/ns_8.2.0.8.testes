function RetornoConsultaRemetenteFiltro(cliente) {
    RetornosConsultas.RemetenteFiltro = cliente.CPFCNPJ;
    $("#txtRemetenteCTeFiltro").val(cliente.Nome);
    $("#txtCPFCNPJRemetenteFiltro").val(cliente.CPFCNPJ);
}

function RetornoConsultaDestinatarioFiltro(cliente) {
    RetornosConsultas.DestinatarioFiltro = cliente.CPFCNPJ;
    $("#hddDestinatarioFiltro").val(cliente.CPFCNPJ);
    $("#txtDestinatarioCTeFiltro").val(cliente.Nome);
    $("#txtCPFCNPJDestinatarioFiltro").val(cliente.CPFCNPJ);
}

function LimparCamposRemetenteDestinatarioFiltro(tipo) {
    RetornosConsultas[tipo + "Filtro"] = "";
    $("#txtCPFCNPJ" + tipo + "Filtro").val("");
    $("#txt" + tipo + "CTeFiltro").val("");
}

function BuscarRemetenteDestinatarioFiltro($this, tipo) {
    if (RetornosConsultas[tipo + "Filtro"] != $("#txtCPFCNPJ" + tipo + "Filtro").val()) {
        var cpfCnpj = $("#txtCPFCNPJRemetenteFiltro").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            RetornosConsultas[tipo + "Filtro"] = r.Objeto.CPF_CNPJ;
                            $("#txtCPFCNPJ" + tipo + "Filtro").val(r.Objeto.CPF_CNPJ);
                            $("#txt" + tipo + "CTeFiltro").val(r.Objeto.Nome);
                        } else {
                            LimparCamposRemetenteFiltro();
                            jAlert(tipo + " não encontrado.", "Atenção");
                        }
                    } else {
                        LimparCamposRemetenteDestinatarioFiltro(tipo);
                        jAlert(r.Erro, "Erro");
                    }
                });
            } else {
                LimparCamposRemetenteDestinatarioFiltro(tipo);
                jAlert("O CPF/CNPJ digitado é inválido!", "Atenção");
            }
        } else {
            LimparCamposRemetenteDestinatarioFiltro(tipo);
        }
    }
}