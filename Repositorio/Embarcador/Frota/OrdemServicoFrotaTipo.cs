using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaTipo : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>
    {
        public OrdemServicoFrotaTipo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>();

            query = query.Where(o => o.Descricao.Contains(descricao));

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo> BuscarPorCodigo(List<long> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoEmpresa, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);
            
            return ObterLista(query, propriedadeOrdenar, dirOrdena, inicio, limite);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.Count();
        }
    }
}
