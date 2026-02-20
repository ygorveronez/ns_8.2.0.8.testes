using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class MotivoRejeicaoColeta
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public int CodigoTipoOperacaoColeta { get; set; }
        public List<MotivoRejeicaoColetaProduto> Produtos { get; set; }
    }
}
