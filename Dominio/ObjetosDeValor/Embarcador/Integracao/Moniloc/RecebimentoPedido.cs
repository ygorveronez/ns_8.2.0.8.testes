using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc
{
    public class RecebimentoPedido
    {
        //Tipo Registro 01
        public string CodigoPlanta { get; set; }
        public string CodigoFornecedor { get; set; }
        public string LocalEntrega { get; set; }
        public string Reservado { get; set; }

        //Tipo Registro 2
        public List<DadosRecebimentoPedidoProduto> Produtos { get; set; }

        //Tipo Registro 3
        public List<DadosRecebimentoPedidoDeposito> Depositos { get; set; }

        //Tipo Registro 4
        public List<DadosRecebimentoPedidoColeta> Coletas { get; set; }
    }
}
