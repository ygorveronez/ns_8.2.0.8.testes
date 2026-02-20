using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoTransportadorChecklist
    {
        #region Propriedades

        public int CodigoChecklist { get; set; }
        public string DataChecklist { get; set; }
        public GrupoProdutoChecklist GrupoProduto { get; set; }
        public EnumRegimeLimpeza RegimeLimpeza { get; set; }
        public OrdemCargaChecklist OrdemCargaChecklist { get; set; }
        public string Placa { get; set; }
        public int CodigoVeiculo { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        #endregion Propriedades com Regras
    }
}
