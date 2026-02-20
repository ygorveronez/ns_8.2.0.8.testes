using Dominio.ObjetosDeValor.CTe;

namespace Dominio.ObjetosDeValor.NFSe
{
    public class Item
    {
        public Servico Servico { get; set; }

        public int CodigoIBGECidade { get; set; }

        public int CodigoIBGECidadeIncidencia { get; set; }

        public int CodigoPaisPrestacaoServico { get; set; }

        public bool ServicoPrestadoNoPais { get; set; }

        public decimal ValorServico { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal ValorDescontoCondicionado { get; set; }

        public decimal ValorDescontoIncondicionado { get; set; }

        public decimal ValorDeducoes { get; set; }

        public decimal BaseCalculoISS { get; set; }

        public decimal AliquotaISS { get; set; }

        public decimal ValorISS { get; set; }

        public string Discriminacao { get; set; }

        public int ExigibilidadeISS { get; set; }

        public bool ISSInclusoValorTotal { get; set; }

        public IBSCBS IBSCBS { get; set; }
    }
}
