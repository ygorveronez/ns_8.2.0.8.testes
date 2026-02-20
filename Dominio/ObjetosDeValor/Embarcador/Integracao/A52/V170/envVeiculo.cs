using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envVeiculo
    {
        public string placa { get; set; }

        public string frota { get; set; }

        public int? tipo { get; set; }

        public int? tipoCarreta { get; set; }

        public int? vinculo { get; set; }

        public int? limiteVelocidade { get; set; }

        public bool ativo { get; set; }

        public List<envVeiculoEquipamento> equipamentos { get; set; }

        public int? capacidadeKg { get; set; }

        public int? capacidadeM3 { get; set; }

        public int? capacidadePallet { get; set; }

        public int? idTipoEmblema { get; set; }
    }

    public class envVeiculoEquipamento
    {
        public int? id { get; set; }

        public string codigo { get; set; }

        public int? idTecnologia { get; set; }

        public bool recebe_posicao { get; set; }

        public bool recebe_macro { get; set; }

        public bool recebe_evento { get; set; }

        public bool recebe_mensagem { get; set; }
    }
}