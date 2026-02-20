using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoAgendamentoColetaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoAgendamentoColetaEntrega>
    {
        public ConfiguracaoTipoOperacaoAgendamentoColetaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoAgendamentoColetaEntrega Buscar()
        {
            var consultaCD = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoAgendamentoColetaEntrega>();

            return consultaCD.FirstOrDefault();
        }

    }
}
