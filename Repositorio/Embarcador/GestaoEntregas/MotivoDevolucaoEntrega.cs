using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public class MotivoDevolucaoEntrega : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega>
    {

        public MotivoDevolucaoEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.MotivoChamado)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega>();
            var result = from obj in query where obj.Ativo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> BuscarPocMotivoChamado(int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega>();
            var result = from obj in query where obj.Ativo && obj.MotivoChamado.Codigo == codigoMotivoChamado select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                result = result.Where(obj => obj.Descricao.Contains(descricao));
            }

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Ativo);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Fetch(obj => obj.MotivoChamado).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }
    }
}
