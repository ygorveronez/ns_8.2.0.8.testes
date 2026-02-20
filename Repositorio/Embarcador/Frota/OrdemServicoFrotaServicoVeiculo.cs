using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaServicoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>
    {
        public OrdemServicoFrotaServicoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo BuscarUltimoRealizado(int codigoServico, int codigoVeiculo, int codigoEquipamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.Servico.Codigo == codigoServico && o.OrdemServico.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.Finalizada);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.OrdemServico.Veiculo.Codigo == codigoVeiculo);

            if (codigoEquipamento > 0)
                query = query.Where(o => o.OrdemServico.Equipamento.Codigo == codigoEquipamento);

            return query.OrderByDescending(o => o.OrdemServico.DataProgramada).FirstOrDefault();
        }

        public decimal BuscarCustoMedio(int codigoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.Servico.Codigo == codigoServico && o.OrdemServico.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.Finalizada);

            int count = query.Count();

            if (count > 0)
                return (query.Sum(o => (decimal?)o.CustoEstimado) ?? 0m) / count;
            else
                return 0;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota> BuscarTipoManutencaoPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Select(o => o.TipoManutencao).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo BuscarPorOrdemServicoEServico(int codigoOrdemServico, int codigoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico && o.Servico.Codigo == codigoServico);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> Consultar(int codigoOrdemServico, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Count();
        }

        public bool ServicoJaEstaLancado(int codigoServico, int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(o => o.Servico.Codigo == codigoServico && o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Count() > 0;
        }

        #endregion
    }
}
