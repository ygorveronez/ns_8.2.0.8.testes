using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoEntregas
{
    public class LocalizacaoView
    {
        public bool ExibirMapa { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PolilinhaPlanejada { get; set; }

        public string PolilinhaRealizada { get; set; }

        public List<EntregaLocalizacaoView> Entregas { get; set; }
    }
}
