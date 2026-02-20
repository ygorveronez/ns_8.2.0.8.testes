using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracoes
{
    public class FiltrosPesquisaControleDasIntegracoes
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoIntegradora { get; set; }
        public bool Sitaucao { get; set; }
        public int CodigoMetodo { get; set; }

    }
}
