using System.Collections.Generic;

namespace Repositorio
{
    public class Atendimento : RepositorioBase<Dominio.Entidades.Embarcador.Atendimento.Atendimento>
    {
        public Atendimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioAtendimento> InstanceDS()
        {
            return new List<Dominio.ObjetosDeValor.Relatorios.RelatorioAtendimento>();
        }
    }
}
