using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class PendenciasControleDocumento
    {
        public GatilhoIrregularidade Gatilho { get; set; }
        public int Quantidade { get; set; }
        public int CodigoSetor { get; set; }
    }
}
