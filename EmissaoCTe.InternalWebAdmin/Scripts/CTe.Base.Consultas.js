function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guid() {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

//Consulta de Empresas
function CarregarConsultaDeEmpresasGerenciadoras(idTxtBuscar, idBtnBuscar, status, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Razão Social/Nome Fantasia', Id: ('txtRazaoSocialNomeFantasia' + guid()) }], "/Empresa/ConsultarEmpresasGerenciadoras?callback=?&Status=" + status, "Consulta de Empresas Gerenciadoras", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}

//Consulta de Usuários
function CarregarConsultaDeUsuarios(idTxtBuscar, idBtnBuscar, codigoEmpresa, tipoAcesso, callbackSelecionar, fecharConsulta, usarKeyDown) {
    CTe_Consulta_Carregar(idTxtBuscar, idBtnBuscar, [{ DescricaoBusca: 'Nome', Descricao: 'Nome:', Id: ('txtNomeUsuario' + guid()) }], "/Usuario/ConsultarPorEmpresa?callback=?&CodigoEmpresa=" + codigoEmpresa + "&TipoAcesso=" + tipoAcesso, "Consulta de Usuários", [{ Descricao: "Selecionar", Evento: callbackSelecionar }], [0], fecharConsulta, usarKeyDown);
}