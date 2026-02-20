using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class GestaoPatio
    {
        public Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao Carga { get; set; }

        public List<EtapaDescricao> Etapas { get; set; }

        public int IndiceEtapaAtual { get; set; }

        public bool EtapaAtualLiberada { get; set; }
    }
}
