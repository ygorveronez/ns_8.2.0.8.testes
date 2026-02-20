using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.ICMS
{
    public class RegraExtensao
    {
        public int Codigo { get; set; }
        public string Extensao { get; set; }
        public TipoPropriedadeVeiculo? TipoPropriedade { get; set; }
        public int CodigoModeloVeicular { get; set; }   

    }
}
