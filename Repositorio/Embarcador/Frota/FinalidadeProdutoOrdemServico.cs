using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class FinalidadeProdutoOrdemServico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico>
    {
        public FinalidadeProdutoOrdemServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico> Consultar(string descricao, SituacaoAtivoPesquisa? situacao, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));
            
            if (situacao.HasValue)
            {
                if (situacao.Value == SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo);
                else if (situacao.Value == SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(o => !o.Ativo);
            }

            return query.Fetch(o => o.TipoMovimentoUso).OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa? situacao, string propOrdena)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));
            
            if (situacao.HasValue)
            {
                if (situacao.Value == SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo);
                else if (situacao.Value == SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(o => !o.Ativo);
            }

            return query.Count();
        }
    }
}
