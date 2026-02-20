var EnumConfiguracaoTaxaDescargaTipoHelper = function () {
	this.Todas = 0;
	this.AjudantesPorQuantidade = 1;
};

EnumConfiguracaoTaxaDescargaTipoHelper.prototype = {
	obterOpcoes: function () {
		return [
			{ text: "Ajudantes por quantidade", value: this.AjudantesPorQuantidade },
		];
	},
	obterOpcoesPesquisa: function () {
		return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
	},

	obterDescricao: function (valor) {
		switch (valor) {
			case this.AjudantesPorQuantidade: return "Ajudantes por quantidade";
			default: return "";
		}
	},
};

var EnumConfiguracaoTaxaDescargaTipo = Object.freeze(new EnumConfiguracaoTaxaDescargaTipoHelper());