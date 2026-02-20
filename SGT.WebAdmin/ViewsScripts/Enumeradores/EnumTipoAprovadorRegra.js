var EnumTipoAprovadorRegraHelper = function () {
    this.Usuario = 0;
    this.Transportador = 1;
    this.Setor = 2;
}

EnumTipoAprovadorRegraHelper.prototype = {
    obterOpcoesPorTransportador: function () {
        return [
            { text: "Transportador", value: this.Transportador },
            { text: "Usuário", value: this.Usuario }
        ];
    },
    obterOpcoesPorSetor: function () {
        return [
            { text: "Setor", value: this.Setor },
            { text: "Usuário", value: this.Usuario }
        ];
    }
}

var EnumTipoAprovadorRegra = Object.freeze(new EnumTipoAprovadorRegraHelper());
