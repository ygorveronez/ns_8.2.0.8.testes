using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteValorPadrao : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao>
    {
        public ContratoFreteValorPadrao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> BuscarAtivos(double cpfCnpjTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao>();

            query = query.Where(o => o.Ativo);

            if (cpfCnpjTerceiro > 0D)
                query = query.Where(o => o.TransportadorTerceiro == null || o.TransportadorTerceiro.CPF_CNPJ == cpfCnpjTerceiro);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> query = ObterQueryConsulta(descricao, status);
            
            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> query = ObterQueryConsulta(descricao, status); ;

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> ObterQueryConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query;
        }
    }
}
