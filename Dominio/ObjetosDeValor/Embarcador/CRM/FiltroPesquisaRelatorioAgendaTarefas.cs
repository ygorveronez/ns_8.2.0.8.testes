using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.CRM
{
    public class FiltroPesquisaRelatorioAgendaTarefas
    {
        public string Observacao { get; set; }
        public StatusAgendaTarefa? Status { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoCliente { get; set; }
       
    }
}