using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracoes
{
    public class FiltroPesquisaIntegracaoHUBOfertas
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoIntegracao? Situacao { get; set; }
        public TipoEnvioHUBOfertas? TipoEnvioHUBOfertas { get; set; }
        public int CodigoTransportador { get; set; }
        public int Carga { get; set; }
    }
}
