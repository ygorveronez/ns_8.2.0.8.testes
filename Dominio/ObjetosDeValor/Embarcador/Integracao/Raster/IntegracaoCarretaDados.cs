using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class IntegracaoCarretaDados
    {
        public string Placa { get; set; }
        public int CodIBGECidade { get; set; }
        public string Cidade { get; set; }
        public string Renavam { get; set; }
        public string Chassi { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string NumeroANTT { get; set; }
        public string NumeroFrota { get; set; }
        public int CodTipoCarreta { get; set; }
        public int CodMarca { get; set; }
        public int CodCor { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string CNPJProprietario { get; set; }
        public string PossuiRastreador { get; set; }
        public int TecnologiaRastreador { get; set; }
        public int ModeloRastreador { get; set; }
        public string TerminalRastreador { get; set; }
        public List<IntegracaoCarretaDadosDispositivo> Dispositivos { get; set; }
    }
}
