/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosCartaCorrecao;
var _gridAnexosCartaCorrecao;
var ControleDocumento;

var AnexosCartaCorrecao = function () {

	//-- Adicionar arquivo
	this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
	this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, type: types.map, getType: typesKnockout.string, maxlength: 150 });
	this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Arquivo, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
	this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
	this.Arquivo.val.subscribe(function (novoValor) {
		var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
		_anexosCartaCorrecao.NomeArquivo.val(nomeArquivo);
	});

	this.CartaCorrecaoAnexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
	this.CartaCorrecaoAnexos.val.subscribe(function () {
		RenderizarGridAnexosCartaCorrecao();
	});

	this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoCartaCorrecaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosCartaCorrecao() {
	_anexosCartaCorrecao = new AnexosCartaCorrecao();
	KoBindings(_anexosCartaCorrecao, "knockoutAnexosCartaCorrecao");

	var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoCartaCorrecaoClick, icone: "", visibilidade: visibleDownloadAnexoCartaCorrecao };
	var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoCartaCorrecaoClick, icone: "", visibilidade: visibleRemoverAnexoCartaCorrecao };

	var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [download, remover] };

	var header = [
		{ data: "Codigo", visible: false },
		{ data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%", className: "text-align-left" },
		{ data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
	];

	var linhasPorPaginas = 5;
	_gridAnexosCartaCorrecao = new BasicDataTable(_anexosCartaCorrecao.CartaCorrecaoAnexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
	_gridAnexosCartaCorrecao.CarregarGrid([]);
}

function downloadAnexoCartaCorrecaoClick(dataRow) {
	var data = { Codigo: dataRow.Codigo };
	executarDownload("CartaCorrecaoAnexo/DownloadAnexo", data);
}

function removerAnexoCartaCorrecaoClick(dataRow) {
	var listaAnexos = GetAnexosCartaCorrecao();
	RemoverAnexoCartaCorrecao(dataRow, listaAnexos, _anexosCartaCorrecao, "CartaCorrecaoAnexo/ExcluirAnexo");
}

function abrirAnexosCartaCorrecaoClick(registroSelecionado) {
	
	_anexosCartaCorrecao.CartaCorrecaoAnexos.visible(visibleCartaCorrecaoAnexos(registroSelecionado));
	executarReST("ControleDocumento/BuscarAnexos", { Codigo: registroSelecionado.Codigo }, function (retorno) {
		if (retorno.Success) {
			if (retorno.Data) {
				_anexosCartaCorrecao.CartaCorrecaoAnexos.val(retorno.Data.Anexos);
				ControleDocumento = parseInt(registroSelecionado.Codigo);
				Global.abrirModal('divModalAnexosCartaCorrecao');
			}
			else
				exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
		}
		else
			exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
	});
}

function adicionarAnexoCartaCorrecaoClick() {
	var file = document.getElementById(_anexosCartaCorrecao.Arquivo.id);

	if (file.files.length == 0)
		return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

	var anexo = {
		Codigo: guid(),
		Descricao: _anexosCartaCorrecao.Descricao.val(),
		NomeArquivo: _anexosCartaCorrecao.NomeArquivo.val(),
		Arquivo: file.files[0]
	};

	EnviarAnexoCartaCorrecao(anexo);

	LimparCampos(_anexosCartaCorrecao);
	_anexosCartaCorrecao.Arquivo.val('');

	file.value = null;
}

//*******MÉTODOS*******

function GetAnexosCartaCorrecao() {
	return _anexosCartaCorrecao.CartaCorrecaoAnexos.val().slice();
}


function RenderizarGridAnexosCartaCorrecao() {
	var anexos = GetAnexosCartaCorrecao();

	_gridAnexosCartaCorrecao.CarregarGrid(anexos);
}

//function EnviarArquivosAnexadosCartaCorrecao() {
//    var anexos = GetAnexos();

//    if (anexos.length > 0) {
//        var dados = {
//            Codigo: codigoCartaCorrecao,
//        }

//        CriaEEnviaFormData(anexos, dados);
//    }
//}

function RemoverAnexoCartaCorrecao(dataRow, anexosCartaCorrecao, knout, url) {
	var RemoveDaGridCartaCorrecao = function () {
		anexosCartaCorrecao.forEach(function (anexo, i) {
			if (dataRow.Codigo == anexo.Codigo) {
				anexosCartaCorrecao.splice(i, 1);
			}
		});

		knout.CartaCorrecaoAnexos.val(anexosCartaCorrecao);
	}

	executarReST(url, dataRow, function (arg) {
		if (arg.Success) {
			if (arg.Data) {
				RemoveDaGridCartaCorrecao();
				exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
			} else {
				exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
			}
		} else {
			exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
		}
	});
}

function CriaEEnviaFormDataCartaCorrecao(anexos, dados) {
	var formData = new FormData();
	anexos.forEach(function (anexo) {
		if (isNaN(anexo.Codigo)) {
			formData.append("Arquivo", anexo.Arquivo);
			formData.append("Descricao", anexo.Descricao);
		}
	});

	enviarArquivo("CartaCorrecaoAnexo/AnexarArquivos?callback=?", dados, formData, function (arg) {
		if (arg.Success) {
			if (arg.Data != false) {
				_anexosCartaCorrecao.CartaCorrecaoAnexos.val(arg.Data.Anexos);
				exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
			} else {
				exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, arg.Msg);
			}
		} else {
			exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
		}
	});
}

function EnviarAnexoCartaCorrecao(anexo) {
	var anexos = [anexo];
	var dados = {
		Codigo: ControleDocumento,
	}

	CriaEEnviaFormDataCartaCorrecao(anexos, dados);
}

function limparCartaCorrecaoAnexos() {
	LimparCampos(_anexosCartaCorrecao);
	_anexosCartaCorrecao.CartaCorrecaoAnexos.val(_anexosCartaCorrecao.CartaCorrecaoAnexos.def);
}

function visibleDownloadAnexoCartaCorrecao(dataRow) {
	return !isNaN(dataRow.Codigo);
}

function visibleRemoverAnexoCartaCorrecao(dataRow) {
	return dataRow.CodigoIrregularidade == 0;
}


function visibleCartaCorrecaoAnexos(registro) {
	var opcoesCCe = [EnumGatilhoIrregularidade.CSTICMS, EnumGatilhoIrregularidade.NFeVinculadaAoFrete];
	return (opcoesCCe.indexOf(parseInt(registro.CodigoIrregularidade)) >= 0);
}