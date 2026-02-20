using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class retVeiculo : envVeiculo
    {
        public int? id { get; set; }

        public int? idEmpresa { get; set; }

        public int? idGrupoEmpresa { get; set; }
    }
}