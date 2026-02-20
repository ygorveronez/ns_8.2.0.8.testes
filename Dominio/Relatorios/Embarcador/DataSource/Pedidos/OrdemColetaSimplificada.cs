using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class OrdemColetaSimplificada
    {
        public int CodigoPedido { get; set; }

        public string Veiculo { get; set; }
        public string VeiculoRenavam { get; set; }
        public string VeiculoRastreador { get; set; }
        public string VeiculoTipoRastreador { get; set; }
        public string VeiculoMarca { get; set; }
        public string VeiculoModelo { get; set; }
        public string VeiculoAnoFabricacao { get; set; }
        public string VeiculoAnoModelo { get; set; }
        public string VeiculoChassi { get; set; }
        public string VeiculoTara { get; set; }

        public string Reboque { get; set; }
        public string ReboqueRenavam { get; set; }
        public string ReboqueMarca { get; set; }
        public string ReboqueModelo { get; set; }
        public string ReboqueAnoFabricacao { get; set; }
        public string ReboqueAnoModelo { get; set; }
        public string ReboqueChassi { get; set; }
        public string ReboqueTara { get; set; }

        public string Remetente { get; set; }
        public string RemetenteCidade { get; set; }

        public string Destinatario { get; set; }
        public string DestinatarioCidade { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }
        private DateTime DataColeta { get; set; }
        public string Observacao { get; set; }

        #region Propriedades com Regras

        public string DataColetaFormatada
        {
            get { return DataColeta != DateTime.MinValue ? DataColeta.ToDateTimeString() : string.Empty; }
        }

        #endregion
    }
}
