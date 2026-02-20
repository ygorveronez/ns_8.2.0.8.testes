using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class CheckListVistoria
    {
        public int Codigo { get; set; }

        public int CodigoTipoChecklist { get; set; }

        public int DiasVencimento { get; set; }

        public string Placa { get; set; }

        public DateTime DataProgramada { get; set; }

        public string DescricaoTipoCheckList { get; set; }

        public int KmAtual { get; set; }
    }
}
