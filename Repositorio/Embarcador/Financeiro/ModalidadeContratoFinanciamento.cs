using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Financeiro
{
    public class ModalidadeContratoFinanciamento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento>
    {
        public ModalidadeContratoFinanciamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public List<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento> BuscarPorCodigos(List<long> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

         
            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);


            return query.Count();
        }
    }
}

