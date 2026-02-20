let EnumTipoDocumentoPaisTrizyHelper = function () {
    this.Brasil = 0;
    this.Argentina = 1;
    this.Paraguai = 2;
    this.Uruguai = 3;
    this.Bolivia = 4;
    this.Venezuela = 5;
    this.Chile = 6;
    this.Colombia = 7;
    this.CostaRica = 8;
    this.Cuba = 9;
    this.Equador = 10;
    this.ElSalvador = 11;
    this.Guatemala = 12;
    this.Haiti = 13;
    this.Honduras = 14;
    this.Mexico = 15;
    this.Nicaragua = 16;
    this.Panama = 17;
    this.Peru = 18;
    this.RepublicaDominicana = 19;
    this.EstadosUnidosDaAmerica = 20;
    this.Canada = 21;
    this.Estrangeiro = 999;
};

EnumTipoDocumentoPaisTrizyHelper.prototype = {
    obterOpcoes: function () {
        return [
            { value: this.Brasil, text: "Brasil" },
            { value: this.Argentina, text: "Argentina" },
            { value: this.Paraguai, text: "Paraguai" },
            { value: this.Uruguai, text: "Uruguai" },
            { value: this.Bolivia, text: "Bolivia" },
            { value: this.Venezuela, text: "Venezuela" },
            { value: this.Chile, text: "Chile" },
            { value: this.Colombia, text: "Colombia" },
            { value: this.CostaRica, text: "Costa Rica" },
            { value: this.Cuba, text: "Cuba" },
            { value: this.Equador, text: "Equador" },
            { value: this.ElSalvador, text: "El Salvador" },
            { value: this.Guatemala, text: "Guatemala" },
            { value: this.Haiti, text: "Haití" },
            { value: this.Honduras, text: "Honduras" },
            { value: this.Mexico, text: "México" },
            { value: this.Nicaragua, text: "Nicarágua" },
            { value: this.Panama, text: "Panamá" },
            { value: this.Peru, text: "Perú" },
            { value: this.RepublicaDominicana, text: "Republica Dominicana" },
            { value: this.EstadosUnidosDaAmerica, text: "Estados Unidos da América" },
            { value: this.Canada, text: "Canadá" },
            { value: this.Estrangeiro, text: "Estrangeiro" },
        ];
    }
}

let EnumTipoDocumentoPaisTrizy = Object.freeze(new EnumTipoDocumentoPaisTrizyHelper());