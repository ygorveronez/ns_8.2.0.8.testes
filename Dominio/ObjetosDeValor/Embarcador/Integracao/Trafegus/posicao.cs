using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus
{
    public class Posicao
    {
        public long IdPosicao { get; set; }
        public string Placa { get; set; }
        public string SequenciaTerminal { get; set; }
        public string NumeroTerminal { get; set; }
        public string Tecnologia { get; set; }
        public object IdViagem { get; set; }
        public DateTime DataBordo { get; set; }
        public DateTime? DataTecnologia { get; set; }
        public DateTime DataCadastro { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DescricaoSistema { get; set; }
        public string DescricaoTecnologia { get; set; }
        public string Ignicao { get; set; }
        public string Odometro { get; set; }
        public int? Velocidade { get; set; }
        public string RPM { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public double? Distancia { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? PrevisaoInicio { get; set; }
        public DateTime? PrevisaoFim { get; set; }
        public double? ValorCarga { get; set; }
        public string Motorista { get; set; }
        public string Proprietario { get; set; }
        public string Frota { get; set; }
        public string Carreta { get; set; }
        public string Vinculo { get; set; }
    }

    public class PosicaoTrafegus
    {
        public List<Posicao> Posicao { get; set; }
    }


}
