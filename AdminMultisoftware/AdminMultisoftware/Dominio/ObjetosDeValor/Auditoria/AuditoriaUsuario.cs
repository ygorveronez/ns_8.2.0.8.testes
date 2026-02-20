using System;

namespace AdminMultisoftware.Dominio.ObjetosDeValor.Auditoria
{
    public class AuditoriaUsuario
    {
        public Int64 Codigo { get; set; }
        public string Usuario { get; set; }
        public string Menu { get; set; }
        public string Acao { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public string Propriedade { get; set; }
        public string ValorAntigo { get; set; }
        public string ValorNovo { get; set; }

    }
}