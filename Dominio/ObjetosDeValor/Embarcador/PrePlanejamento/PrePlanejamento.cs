using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.PrePlanejamento
{
    public class PrePlanejamento
    {
        public string NomeGrupo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Observacao{ get; set; }
        public List<Veiculo> Placas { get; set; }
        public List<Meta> Metas { get; set; }
        public Vigencia Vigencia { get; set; }
    }
}
