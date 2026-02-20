using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public class Filiais
    {
        public string CodigoIntegracao { get; set; }
        public SituacaoFilial Situacao { get; set; }
        public string NCM { get; set; }
        public UsoMaterial UsoMaterial { get; set; }
    }
}
