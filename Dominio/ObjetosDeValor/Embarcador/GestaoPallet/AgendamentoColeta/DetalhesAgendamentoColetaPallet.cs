namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta
{
    public class DetalhesAgendamentoColetaPallet
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string StatusAgendamento{ get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColetaPallet Status { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public int QuantidadePallets { get; set; }

        public string Filial { get; set; }

        public string Solicitante { get; set; }

        public string Transportador { get; set; }

        public string Cliente { get; set; }

        public string Veiculo { get; set; }

        public string Motorista { get; set; }

        public string DataOrdem { get; set; }

        public string DataCarregamento { get; set; }

        public int NumeroOrdem { get; set; }

        #endregion Propriedades
    }
}