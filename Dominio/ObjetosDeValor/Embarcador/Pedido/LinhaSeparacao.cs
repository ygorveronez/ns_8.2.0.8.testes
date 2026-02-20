
namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class LinhaSeparacao
    {
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public bool Roteiriza { get; set; }
        public int NivelPrioridade { get; set; }
        public Filial.Filial Filial { get; set; }
    }
}
