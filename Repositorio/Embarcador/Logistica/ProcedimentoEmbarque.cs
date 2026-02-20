using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Logistica
{
    public class ProcedimentoEmbarque : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque>
    {
        public ProcedimentoEmbarque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.TipoOperacao)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque>();

            var result = from obj in query where obj.Ativo select obj;

            return result
                 .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque> Consultar(int filial, int tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = Consulta(filial, tipoOperacao, status);

            if (maximoRegistros > 0)
                query = query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros);
            else
                query = query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            return query
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.TipoOperacao)
                .ToList();

        }

        public int ContarConsulta(int filial, int tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = Consulta(filial, tipoOperacao, status);

            return query.Count();
        }

        public bool VerificarExistencia(Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque procedimentoEmbarque)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque>()
                .Any(o => o.Filial == procedimentoEmbarque.Filial && o.TipoOperacao == procedimentoEmbarque.TipoOperacao && o.Codigo != procedimentoEmbarque.Codigo);

            return query;

        }

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque> Consulta(int filial, int tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque>();

            if (filial > 0)
                query = query.Where(o => o.Filial.Codigo == filial);

            if (tipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao.Codigo == tipoOperacao);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query;
        }
    }
}
