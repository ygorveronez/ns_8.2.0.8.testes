var EnumEstadoHelper = function () {
    this.Todos = "";
    this.Acre = "AC";
    this.Alagoas = "AL";
    this.Amapa = "AP";
    this.Amazonas = "AM";
    this.Bahia = "BA";
    this.Ceara = "CE";
    this.DistritoFederal = "DF";
    this.EspiritoSanto = "ES";
    this.Exportacao = "EX";
    this.Goias = "GO";
    this.Maranhao = "MA";
    this.MatoGrosso = "MT";
    this.MatoGrossoDoSul = "MS";
    this.MinasGerais = "MG";
    this.Para = "PA";
    this.Paraiba = "PB";
    this.Parana = "PR";
    this.Pernambuco = "PE";
    this.Piaui = "PI";
    this.RioDeJaneiro = "RJ";
    this.RioGrandeDoNorte = "RN";
    this.RioGrandeDoSul = "RS";
    this.Rondonia = "RO";
    this.Roraima = "RR";
    this.SantaCatarina = "SC";
    this.SaoPaulo = "SP";
    this.Sergipe = "SE";
    this.Tocantins = "TO";
};

EnumEstadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Acre", value: this.Acre },
            { text: "Alagoas", value: this.Alagoas },
            { text: "Amapá", value: this.Amapa },
            { text: "Amazonas", value: this.Amazonas },
            { text: "Bahia", value: this.Bahia },
            { text: "Ceará", value: this.Ceara },
            { text: "Distrito Federal", value: this.DistritoFederal },
            { text: "Espírito Santo", value: this.EspiritoSanto },
            { text: "Exportação", value: this.Exportacao },
            { text: "Goiás", value: this.Goias },
            { text: "Maranhão", value: this.Maranhao },
            { text: "Mato Grosso", value: this.MatoGrosso },
            { text: "Mato Grosso do Sul", value: this.MatoGrossoDoSul },
            { text: "Minas Gerais", value: this.MinasGerais },
            { text: "Pará", value: this.Para },
            { text: "Paraíba", value: this.Paraiba },
            { text: "Paraná", value: this.Parana },
            { text: "Pernambuco", value: this.Pernambuco },
            { text: "Piauí", value: this.Piaui },
            { text: "Rio de Janeiro", value: this.RioDeJaneiro },
            { text: "Rio Grande do Norte", value: this.RioGrandeDoNorte },
            { text: "Rio Grande do Sul", value: this.RioGrandeDoSul },
            { text: "Rondônia", value: this.Rondonia },
            { text: "Roraima", value: this.Roraima },
            { text: "Santa Catarina", value: this.SantaCatarina },
            { text: "São Paulo", value: this.SaoPaulo },
            { text: "Sergipe", value: this.Sergipe },
            { text: "Tocantins", value: this.Tocantins }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastro: function () {
        return [{ text: "Selecione", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesSemExterior: function () {
        return [
            { text: "Acre", value: this.Acre },
            { text: "Alagoas", value: this.Alagoas },
            { text: "Amapá", value: this.Amapa },
            { text: "Amazonas", value: this.Amazonas },
            { text: "Bahia", value: this.Bahia },
            { text: "Ceará", value: this.Ceara },
            { text: "Distrito Federal", value: this.DistritoFederal },
            { text: "Espírito Santo", value: this.EspiritoSanto },
            { text: "Goiás", value: this.Goias },
            { text: "Maranhão", value: this.Maranhao },
            { text: "Mato Grosso", value: this.MatoGrosso },
            { text: "Mato Grosso do Sul", value: this.MatoGrossoDoSul },
            { text: "Minas Gerais", value: this.MinasGerais },
            { text: "Pará", value: this.Para },
            { text: "Paraíba", value: this.Paraiba },
            { text: "Paraná", value: this.Parana },
            { text: "Pernambuco", value: this.Pernambuco },
            { text: "Piauí", value: this.Piaui },
            { text: "Rio de Janeiro", value: this.RioDeJaneiro },
            { text: "Rio Grande do Norte", value: this.RioGrandeDoNorte },
            { text: "Rio Grande do Sul", value: this.RioGrandeDoSul },
            { text: "Rondônia", value: this.Rondonia },
            { text: "Roraima", value: this.Roraima },
            { text: "Santa Catarina", value: this.SantaCatarina },
            { text: "São Paulo", value: this.SaoPaulo },
            { text: "Sergipe", value: this.Sergipe },
            { text: "Tocantins", value: this.Tocantins }
        ];
    },
    obterOpcoesPesquisaSemExterior: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoesSemExterior());
    },
    obterOpcoesComExterior: function () {
        return [
            { text: "Acre", value: this.Acre },
            { text: "Alagoas", value: this.Alagoas },
            { text: "Amapá", value: this.Amapa },
            { text: "Amazonas", value: this.Amazonas },
            { text: "Bahia", value: this.Bahia },
            { text: "Ceará", value: this.Ceara },
            { text: "Distrito Federal", value: this.DistritoFederal },
            { text: "Espírito Santo", value: this.EspiritoSanto },
            { text: "Goiás", value: this.Goias },
            { text: "Maranhão", value: this.Maranhao },
            { text: "Mato Grosso", value: this.MatoGrosso },
            { text: "Mato Grosso do Sul", value: this.MatoGrossoDoSul },
            { text: "Minas Gerais", value: this.MinasGerais },
            { text: "Pará", value: this.Para },
            { text: "Paraíba", value: this.Paraiba },
            { text: "Paraná", value: this.Parana },
            { text: "Pernambuco", value: this.Pernambuco },
            { text: "Piauí", value: this.Piaui },
            { text: "Rio de Janeiro", value: this.RioDeJaneiro },
            { text: "Rio Grande do Norte", value: this.RioGrandeDoNorte },
            { text: "Rio Grande do Sul", value: this.RioGrandeDoSul },
            { text: "Rondônia", value: this.Rondonia },
            { text: "Roraima", value: this.Roraima },
            { text: "Santa Catarina", value: this.SantaCatarina },
            { text: "São Paulo", value: this.SaoPaulo },
            { text: "Sergipe", value: this.Sergipe },
            { text: "Tocantins", value: this.Tocantins },
            { text: "Exterior", value: this.Exportacao }
        ];
    },
    obterOpcoesPesquisaComExterior: function () {
        return [{ text: Localization.Resources.Enumeradores.Estado.NaoSelecionado, value: this.Todos }].concat(this.obterOpcoesComExterior());
    }
};

var EnumEstado = Object.freeze(new EnumEstadoHelper());