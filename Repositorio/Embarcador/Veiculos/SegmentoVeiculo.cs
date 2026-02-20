using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Veiculos
{
    public class SegmentoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>
    {
        public SegmentoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> BuscarPorCodigo(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.ToList();
        }

        public List<string> BuscarDescricoesPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> Consultar(string descricao, SituacaoAtivoPesquisa situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo == true);
            else if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => o.Ativo == false);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo == true);
            else if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => o.Ativo == false);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            var result = from obj in query where obj.Ativo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo BuscarPorDescricao(string descricaoSegmento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();

            var result = from obj in query where obj.Descricao.Equals(descricaoSegmento) select obj;

            return result.FirstOrDefault();
        }

        #endregion
    }
}
