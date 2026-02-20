using Dominio.Entidades;
using Dominio.Entidades.Embarcador.Operacional;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Importacao
{
    public class ParametrosImportacao
    {
        public string Guid { get; set; }
        
        public List<DadosLinha> Linhas { get; set; }

        public string Nome { get; set; }

        public OperadorLogistica OperadorLogistica { get; set; }

        public Usuario Usuario { get; set; }
    }
}
