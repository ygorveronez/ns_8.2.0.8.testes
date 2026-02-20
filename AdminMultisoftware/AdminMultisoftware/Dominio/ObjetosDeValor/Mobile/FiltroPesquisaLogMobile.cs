using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.ObjetosDeValor.Mobile
{
    public class FiltroPesquisaLogMobile
    {
        public int Motorista { get; set; }
        public int IdClienteMultisoftware { get; set; }
        public DateTime? InicioDataRegistroApp { get; set; }
        public DateTime? FimDataRegistroApp { get; set; }
        public DateTime? InicioDataCriacao { get; set; }
        public DateTime? FimDataCriacao { get; set; }
        public string Mensagem { get; set; }
        public bool? Erro { get; set; }
    }
}
