var EnumFormaRecebimentoMotoristaAcertoHelper = function () {
    this.NadaFazer = 0;
    this.CriarTitulo = 1;
    this.DescontarFicha = 2;
};

EnumFormaRecebimentoMotoristaAcertoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nada a fazer (padrão)", value: this.NadaFazer },
            { text: "Criar título a receber", value: this.CriarTitulo },
            { text: "Descontar ficha motorista", value: this.DescontarFicha }
        ];
    }
};

var EnumFormaRecebimentoMotoristaAcerto = Object.freeze(new EnumFormaRecebimentoMotoristaAcertoHelper());