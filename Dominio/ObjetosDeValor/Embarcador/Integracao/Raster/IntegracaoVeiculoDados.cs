using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class IntegracaoVeiculoDados
    {
        public string Placa { get; set; }
        public int? CodIBGECidade { get; set; }
        public string Renavam { get; set; }
        public string Chassi { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string NumeroANTT { get; set; }
        public string NumeroFrota { get; set; }
        public int? CodTipoVeiculo { get; set; }
        public int? CodTipoCarreta { get; set; }
        public int? CodMarca { get; set; }
        public int? CodCor { get; set; }
        public int? AnoFabricacao { get; set; }
        public int? AnoModelo { get; set; }
        public string CNPJProprietario { get; set; }
        public string PossuiRastreador { get; set; }
        public int? TecnoRasPrincipal { get; set; }
        public int? ModelRasPrincipal { get; set; }
        public string TermiRasPrincipal { get; set; }
        public int? TecnoRasSecundario { get; set; }
        public int? ModelRasSecundario { get; set; }
        public string TermiRasSecundario { get; set; }
        public List<IntegracaoVeiculoDadosDispositivo> Dispositivos { get; set; }
    }
}
