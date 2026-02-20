using System;

namespace Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario
{
    public class FiltroPesquisaCheckListUsuario
    {
        public DateTime DataPreenchimentoInicial { get; set; }
        public DateTime DataPreenchimentoFinal { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoTipoGROT { get; set; }
    }
}
