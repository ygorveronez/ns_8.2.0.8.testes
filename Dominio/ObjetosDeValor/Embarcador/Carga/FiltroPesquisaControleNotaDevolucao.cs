using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaControleNotaDevolucao
    {
        public string Chave { get; set; }
        public StatusControleNotaDevolucao Status { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroChamado { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public double CnpjCpfDestinatario { get; set; }
    }
}
