using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaAbastecimento
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int Veiculo { get; set; }
        public int TipoVeiculo { get; set; }
        public int Produto { get; set; }
        public int Equipamento { get; set; }
        public int Motorista { get; set; }
        public double ClientePosto { get; set; }
        public int Quilometragem { get; set; }
        public int Horimetro { get; set; }
        public TipoAbastecimento TipoAbastecimento { get; set; }
        public string Situacao { get; set; }
        public string Documento { get; set; }
        public string Placa { get; set; }
        public int NumeroDocumentoInicial { get; set; }
        public int NumeroDocumentoFinal { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoAbastecimentoIgnorar { get; set; }
        public int CodigoCentroResultado { get; set; }
        public List<int> CodigosEmpresa { get; set; }
        public int CodigoUsuarioLogado { get; set; }
    }
}
