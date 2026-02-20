function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guid() {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

//Consultas de Localidades
function CarregarConsultaDeLocalidades(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: 'txtNomeOuCodigoLocalidade' }, { DescricaoBusca: 'IBGE', Descricao: 'Código IBGE', Id: ('txtCodigoIBGE' + guid()) }], "/Localidade/Consulta?callback=?", "Consulta de Cidades", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consuta de Atividades
function CarregarConsultaDeAtividades(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtNomeOuCodigoAtividade' + guid()) }], "/Atividade/Consulta?callback=?", "Consulta de Atividades", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [], fecharConsulta, usarKeyDown);
}

//Consulta de Notas Fiscais Eletronicas
function CarregarConsultaDeNotasFiscaisEletronicas(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NumeroNota', Descricao: 'Número', Id: ('txtNumeroXMLNotaFiscalEletronica' + guid()), Tipo: 'number' },
                                                     { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: ('txtDataEmissaoXMLNotaFiscalEletronica' + guid()), Tipo: 'date' },
                                                     { DescricaoBusca: 'CPF_CNPJ_Emitente', Descricao: 'CPF/CNPJ Emitente', Id: ('txtCPF_CNPJEmitenteXMLNotaFiscalEletronica' + guid()), Tipo: 'cpf_cnpj' }], "/XMLNotaFiscalEletronica/Consultar?callback=?", "Consulta de Notas Fiscais Eletrônicas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2, 3, 4], fecharConsulta, usarKeyDown);
}

//Consulta de Observações
function CarregarConsultaDeObservacoesPorTipo(idTxtBuscar, idBtnBuscar, tipoObservacao, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoObservacao' + guid()) }], "/Observacao/ConsultarTodos?callback=?&Tipo=" + tipoObservacao, "Consulta de Observações", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Observações
function CarregarConsultaDeObservacoes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoObservacao' + guid()) }], "/Observacao/ConsultarTodos?callback=?", "Consulta de Observações", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Naturezas das Operações
function CarregarConsultaDeNaturezasDasOperacoes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoNaturezaDaOperacao' + guid()) }], "/NaturezaDaOperacao/Consultar?callback=?", "Consulta de Naturezas das Operações", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [], fecharConsulta, usarKeyDown);
}

//Consulta de CFOPs
function CarregarConsultaDeCFOPs(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'CFOP', Descricao: 'CFOP', Id: ('txtCodigoCFOP' + guid()) }, { DescricaoBusca: 'NaturezaDaOperacao', Descricao: 'Natureza da Operação', Id: ('txtNaturezaDaOperacao' + guid()) }], "/CFOP/Consultar?callback=?", "Consulta de CFOPs", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Usuários
function CarregarConsultaDeUsuarios(idTxtBuscar, idBtnBuscar, codigoEmpresa, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome:', Id: ('txtNomeUsuario' + guid()) },
                                                     { DescricaoBusca: 'CPFCNPJ', Descricao: 'CPF_CNPJ', Id: ('txtCPFCNPJ' + guid()) },
                                                     { DescricaoBusca: 'Login', Descricao: 'Login', Id: ('txtLogin' + guid()) }], "/Usuario/ConsultarPorEmpresa?callback=?&CodigoEmpresa=" + codigoEmpresa, "Consulta de Usuários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Administradores
function CarregarConsultaDeUsuariosAdmin(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome:', Id: ('txtNomeUsuario' + guid()) },
                                                     { DescricaoBusca: 'CPFCNPJ', Descricao: 'CPF_CNPJ', Id: ('txtCPFCNPJ' + guid()) }], "/Usuario/ConsultarAdministradores?callback=?", "Consulta de Usuários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

function CarregarConsultaDeUsuariosAdminAtivos(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome:', Id: ('txtNomeUsuario' + guid()) },
    { DescricaoBusca: 'CPFCNPJ', Descricao: 'CPF_CNPJ', Id: ('txtCPFCNPJ' + guid()) }], "/Usuario/ConsultarAdministradoresAtivos?callback=?", "Consulta de Usuários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Empresas Com Configuracao
function CarregarConsultaDeEmpresasComConfiguracao(idTxtBuscar, idBtnBuscar, callbackSelecionar, callbackConfigurar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Razão Social/Nome Fantasia', Id: ('txtRazaoSocialNomeFantasia' + guid()) }], "/Empresa/Consultar?callback=?", "Consulta de Empresas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }, { Descricao: "Configurar", Evento: callbackConfigurar }], [0,1], fecharConsulta, usarKeyDown);
}

//Consulta de Empresas
function CarregarConsultaDeEmpresas(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Razão Social/Nome Fantasia', Id: ('txtRazaoSocialNomeFantasia' + guid()) },
                                                     { DescricaoBusca: 'CNPJ', Descricao: 'CNPJ', Id: ('txtCNPJ' + guid()) }], "/Empresa/Consultar?callback=?&Status=" + status, "Consulta de Empresas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0,1], fecharConsulta, usarKeyDown);
}

//Consulta de Atendimentos
function CarregarConsultaDeAtendimentos(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'NomeEmpresa', Descricao: 'Empresa', Id: ('txtEmpresa' + guid()) }, 
        { DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) },
        { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Tipo: 'date', Id: ('DataInicial' + guid()) },
        { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Tipo: 'date', Id: ('DataFinal' + guid()) },
        { DescricaoBusca: 'Status', Descricao: 'Situação', Id: ('txtStatus' + guid()) }], "/Atendimento/Consultar?callback=?&Status=" + status, "Consulta de Atendimentos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Paginas
function CarregarConsultaDePaginas(idTxtBuscar, idBtnBuscar, ambienteAdmin, ambienteEmissao, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) },{ DescricaoBusca: 'Menu', Descricao: 'Menu', Id: ('txtMenu' + guid()) }], "/Pagina/Consultar?callback=?&AmbienteAdmin=" + ambienteAdmin + "&AmbienteEmissao=" + ambienteEmissao, "Consulta de Formulários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1, 2, 3], fecharConsulta, usarKeyDown);
}

//Consulta de Menus
function CarregarConsultaDeMenus(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [], "/Pagina/ConsultarMenus?callback=?", "Consulta de Menus", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de MDFes Encerradas
function CarregarConsultaDeMDFeEncerrada(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeDaEmpresa', Descricao: 'Nome da Empresa', Id: ('txtNomeDaEmpresa' + guid()) },
                                                     { DescricaoBusca: 'CNPJ', Descricao: 'CNPJ', Id: ('txtCNPJ' + guid()) },
                                                     { DescricaoBusca: 'ChaveMDFe', Descricao: 'Chave MDFe', Id: ('txtChaveMDFe' + guid()) }], "/EncerramentoManualMDFe/Consultar?callback=?", "Consulta de MDFes Encerradas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Veículos
function CarregarConsultaDeVeiculos(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Placa', Descricao: 'Placa', Id: ('txtPlacaVeiculo' + guid()) },
                                                     { DescricaoBusca: 'Renavam', Descricao: 'Renavam', Id: ('txtRenavamVeiculo' + guid()) }], "/Veiculo/Consultar?callback=?", "Consulta de Veículos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Clientes
function CarregarConsultadeClientes(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown, tipo) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Razão Social', Id: ('txtNomeRazaoCliente' + guid()) },
                                                     { DescricaoBusca: 'CPFCNPJ', Descricao: 'CNPJ', Id: ('txtCNPJ' + guid()) }], "/Cliente/Consultar?callback=?&Tipo=" + tipo, "Consulta de Clientes", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de CT-es
function CarregarConsultaDeCTes(idTxtBuscar, idBtnBuscar, tipo, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Numero', Descricao: 'Número', Id: ('txtNumero' + guid()) },
                                                     { DescricaoBusca: 'DataEmissao', Descricao: 'Data Emissão', Id: ('txtDataEmissao' + guid()), Tipo: 'date' }], "/ConhecimentoDeTransporteEletronico/ConsultarPorTipo?callback=?&TipoCTe=" + tipo, "Consulta de CT-e's", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Séries
function CarregarConsultaDeSeries(idTxtBuscar, idBtnBuscar, status, codigoEmpresa, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Serie', Descricao: 'Série', Id: ('txtSerie' + guid()) }], "/EmpresaSerie/ConsultarPorEmpresa?callback=?&Status=" + status + "&CodigoEmpresa=" + codigoEmpresa, "Consulta de Séries", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Planos de Emissão
function CarregarConsultaDePlanosDeEmissao(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricaoPlanoEmissaoCTe' + guid()) }], "/PlanoEmissaoCTe/Consultar?callback=?&Status=" + status, "Consulta de Planos de Emissão de CT-e", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Despesas Adicionais da Empresa
function CarregarConsultaDeDespesasAdicionaisDaEmpresa(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeEmpresa', Descricao: 'Nome da Empresa', Id: ('txtNomeEmpresa' + guid()) },
                                                     { DescricaoBusca: 'DescricaoDespesa', Descricao: 'Descrição', Id: ('txtDescricaoDespesa' + guid()) },
                                                     { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: ('txtDataInicial' + guid()), Tipo: 'date' },
                                                     { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: ('txtDataFinal' + guid()), Tipo: 'date' }], "/DespesaAdicionalEmpresa/Consultar?callback=?&Status=" + status, "Consulta de Planos de Emissão de CT-e", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Mensagens de Aviso
function CarregarConsultaDeMensagensDeAviso(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Titulo', Descricao: 'Título', Id: ('txtTitulo' + guid()) }, { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Id: ('txtDataInicial' + guid()), Tipo: 'date' }, { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Id: ('txtDataFinal' + guid()), Tipo: 'date' }], "/MensagemDeAviso/Consultar?callback=?", "Consulta de Mensagens de Aviso", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Campo da Carta de Correção Eletrônica
function CarregarConsultaDeCamposDaCartaDeCorrecaoEletronica(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }, { DescricaoBusca: 'GrupoCampo', Descricao: 'Grupo do Campo', Id: ('txtGrupoDoCampo' + guid()) }, { DescricaoBusca: 'NomeCampo', Descricao: 'Nome do Campo', Id: ('txtNomeCampo' + guid()) }], "/CampoDaCartaDeCorrecaoEletronica/Consultar?callback=?", "Consulta de Campos da Carta de Correção Eletrônica", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Configuração Emissão Email
function CarregarConsultaDeConfiguracaoEmissaoEmail(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Razão Social', Id: ('txtRazaoSocialNomeFantasia' + guid()) },
                                                     { DescricaoBusca: 'CNPJ', Descricao: 'CNPJ', CNPJ: ('txtCNPJ' + guid()) }], "/ConfiguracaoEmissaoEmail/Consultar?callback=?&Status=" + status, "Consulta de Configuração Emissão E-mail", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Email
function CarregarConsultaDeEmail(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Email', Descricao: 'E-mail', Id: ('txtEmail' + guid()) }], "/ConfiguracaoEmissaoEmail/ConsultarEmail?callback=?", "Consulta de E-mail", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Contrato da Empresa
function CarregarConsultaDeContratosDaEmpresa(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'NomeEmpresa', Descricao: 'Nome da Empresa', Id: ('txtNomeEmpresa' + guid()) }], "/EmpresaContrato/Consultar?callback=?", "Consulta de Contratos", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Contrato de CamposNCM
function CarregarConsultaCamposNCM(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/NCM/Consultar?callback=?", "Consulta de Campos NCM", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Impressoras
function CarregarConsultaImpressoras(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Unidade', Descricao: 'Unidade', Id: ('txtUnidade' + guid()) }, { DescricaoBusca: 'Impressora', Descricao: 'Impressora', Id: ('txtImpressora' + guid()) }], "/Impressoras/Consultar?callback=?", "Consulta Impressoras", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

function CarregarConsultaDeAjuda(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/Ajuda/Consultar?callback=?", "Consulta Ajudas", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

function CarregarConsultaDeFilial(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }], "/Filial/Consultar?callback=?", "Consulta Filial", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}


function CarregarConsultaDeTipoAtendimento(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }],
        "/Atendimento/ConsultarTipoAtendimento?callback=?&Status=A", "Consulta de Tipo", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

function CarregarConsultaDeTipoAtendimentoEmissao(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) }],
        "/AtendimentoEmissao/ConsultarTipoAtendimentoEmissao?callback=?&Status=A", "Consulta de Tipo", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Atendimentos
function CarregarConsultaDeAtendimentosEmissao(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'NomeEmpresa', Descricao: 'Empresa', Id: ('txtEmpresa' + guid()) },
        { DescricaoBusca: 'Descricao', Descricao: 'Descrição', Id: ('txtDescricao' + guid()) },
        { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Tipo: 'date', Id: ('DataInicial' + guid()) },
        { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Tipo: 'date', Id: ('DataFinal' + guid()) },
        { DescricaoBusca: 'Status', Descricao: 'Situação', Id: ('txtStatus' + guid()) }], "/AtendimentoEmissao/ConsultarEmissao?callback=?&Status=" + status, "Consulta de Atendimentos Emissão", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Mensagens de Aviso
function CarregarConsultaDeRecado(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'Titulo', Descricao: 'Título', Id: ('txtTitulo' + guid()) },
        { DescricaoBusca: 'DataLancamento', Descricao: 'Data', Id: ('txtDataLancamento' + guid()), Tipo: 'date' },
        { DescricaoBusca: 'NomeUsuario', Descricao: 'Nome Usuário', Id: ('txtNomeUsuario' + guid()) },], "/Recado/Consultar?callback=?", "Consulta de Mensagens de Aviso", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0, 1], fecharConsulta, usarKeyDown);
}

//Consulta de Historico Sefaz
function CarregarConsultaDeHistoricoSefaz(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'DataInicial', Descricao: 'Data Inicial', Tipo: 'date', Id: ('DataInicial' + guid()) },
        { DescricaoBusca: 'DataFinal', Descricao: 'Data Final', Tipo: 'date', Id: ('DataFinal' + guid()) },
        { DescricaoBusca: 'Sefaz', Descricao: 'Sefaz', Id: ('txtSefaz' + guid()) }], "/SefazHistorico/Consultar?callback=?&Status=" + status, "Consulta de Histórico Sefaz", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Estado Contingência
function CarregarConsultaDeContingenciaEstado(idTxtBuscar, idBtnBuscar, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [
        { DescricaoBusca: 'Nome', Descricao: 'Nome', Id: ('txtNome' + guid()) }], "/Estado/Consultar?callback=?", "Consulta de Campos da Carta de Correção Eletrônica", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}