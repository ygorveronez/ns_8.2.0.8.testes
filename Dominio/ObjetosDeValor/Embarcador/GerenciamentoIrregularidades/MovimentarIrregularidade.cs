using Dominio.Entidades.Embarcador.Documentos;
using Dominio.Entidades.Embarcador.GerenciamentoIrregularidades;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades
{
    public class MovimentarIrregularidade
    {
        public ControleDocumento ControleDocumento { get; set; }
        public string Observacao { get; set; }
        public Irregularidade Irregularidade { get; set; }
        public int Sequencia { get; set; } 
        public ServicoResponsavel Responsavel { get; set; } 
    }
}
