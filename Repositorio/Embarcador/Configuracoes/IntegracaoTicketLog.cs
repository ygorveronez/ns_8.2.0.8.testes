using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTicketLog : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog>
    {
        public IntegracaoTicketLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog>();
            return consultaIntegracao.FirstOrDefault();
        }
    }
}
