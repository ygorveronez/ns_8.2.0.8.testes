using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCustoViagem : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem>
    {

        public CentroCustoViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem> _Consultar(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo == true);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Ativo == false);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem> Consultar(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoIntegracao, descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(codigoIntegracao, descricao, status);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem BuscarPorCodigoIntegracao(string codigoIntegracao, bool somenteAtivos = false)
        {
            var consultaCentroCustoDeViagem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (somenteAtivos)
                consultaCentroCustoDeViagem = consultaCentroCustoDeViagem.Where(o => o.Ativo);

            return consultaCentroCustoDeViagem.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem BuscarPorDescricao(string descricao, bool somenteAtivos = false)
        {
            var consultaCentroCustoDeViagem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem>()
                .Where(o => o.Descricao.Equals(descricao));

            if (somenteAtivos)
                consultaCentroCustoDeViagem = consultaCentroCustoDeViagem.Where(o => o.Ativo);

            return consultaCentroCustoDeViagem.FirstOrDefault();
        }

    }
}

