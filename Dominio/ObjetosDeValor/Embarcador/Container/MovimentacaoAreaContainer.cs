using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public class MovimentacaoAreaContainer
    {
        public bool Armador { get; set; }
        public string Carga { get; set; }
        public int Codigo { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoContainerRetirar{ get; set; }
        public int CodigoTipoContainerCarga { get; set; }
        public bool Coleta { get; set; }
        public bool ColetaEquipamento { get; set; }
        public string DescricaoContainerRetirar { get; set; }
        public bool Fronteira { get; set; }
        public double LocalRetiradaContainer { get; set; }
        public SituacaoEntrega Situacao { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoContainerCarga { get; set; }
        public string Veiculo { get; set; }
        public string NumeroExp { get; set; }
        public double CPFCNPJCliente { get; set; }

        public bool ColetaDeContainer
        {
            get
            {
                return Coleta && ColetaEquipamento && LocalRetiradaContainer > 0;
            }
        }

        public string DescricaoSituacao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }

        public string LocalRetiradaContainerDescricao
        {
            get
            {
                return LocalRetiradaContainer.ToString();
            }
        }

        public TipoCargaEntrega TipoCargaEntrega
        {
            get
            {
                if (Fronteira) return TipoCargaEntrega.Fronteira;
                if (Coleta) return TipoCargaEntrega.Coleta;
                return TipoCargaEntrega.Entrega;
            }
        }

        public string TipoCargaEntregaDescricao
        {
            get
            {
                return TipoCargaEntrega.ObterDescricao();
            }
        }
    }
}
