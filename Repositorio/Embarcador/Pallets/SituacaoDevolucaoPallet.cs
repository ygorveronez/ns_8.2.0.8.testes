using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pallets
{
    public class SituacaoDevolucaoPallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet>
    {
        public SituacaoDevolucaoPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet>();

            query = query.Where(o => o.Ativo);

            return query.OrderBy(o => o.ValorUnitario).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> BuscarAtivosPorSituacaoPalletAvariado()
        {
            var situacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet>()
                .Where(situacao => situacao.Ativo && situacao.SituacaoPalletAvariado)
                .OrderBy(o => o.ValorUnitario)
                .ToList();

            return situacoes;
        }
    }
}
