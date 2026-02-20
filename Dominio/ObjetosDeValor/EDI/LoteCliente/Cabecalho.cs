using System;

namespace Dominio.ObjetosDeValor.EDI.LoteCliente
{
    public class Cabecalho
    {
        public DateTime DataGeracao { get; set; }
        public int Sequencia { get; set; }
        public string NomeArquivo { get; set; }
    }
}
