using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaServicoVeiculoTempoExecucao : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao>
    {
        public OrdemServicoFrotaServicoVeiculoTempoExecucao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao BuscarTempoExecucaoEmAberto(int codigoMecanico, int codigoServico, int codigoOrdemServico, int codigoManutencao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico && o.Mecanico.Codigo == codigoMecanico && o.Servico.Codigo == codigoServico && (o.HoraFim == null || o.TempoExecutado == 0));

            if (codigoManutencao > 0)
                query = query.Where(o => o.Manutencao.Codigo == codigoManutencao || o.Manutencao == null);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao> Consultar(int codigoOrdemServico, int codigoServico, int codigoManutencao, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico && o.Servico.Codigo == codigoServico);

            if (codigoManutencao > 0)
                query = query.Where(o => o.Manutencao.Codigo == codigoManutencao || o.Manutencao == null);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoOrdemServico, int codigoServico, int codigoManutencao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico && o.Servico.Codigo == codigoServico);

            if (codigoManutencao > 0)
                query = query.Where(o => o.Manutencao.Codigo == codigoManutencao || o.Manutencao == null);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao> BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.ToList();
        }

        #endregion
    }
}
