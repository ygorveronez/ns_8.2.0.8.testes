
namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class FiltroPesquisaClienteBuscaAutomatica
    {
        public Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
        public int CodigoFilial { get; set; }
        public double? CodigoCliente { get; set; }
        public double? CodigoRemetente { get; set; }
        public double? CodigoDestinatario { get; set; }
        public int? CodigoLocalidadeOrigem { get; set; }
    }

}
