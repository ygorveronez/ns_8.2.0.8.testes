using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class Adiantamento : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Adiantamento>
    {
        public Adiantamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.Adiantamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Adiantamento>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.Adiantamento> Consultar(int codigoMotorista, decimal valorInicial, decimal valorFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Adiantamento>();

            var result = from obj in query select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (valorInicial > 0)
                result = result.Where(obj => obj.Valor >= valorInicial);

            if (valorFinal > 0)
                result = result.Where(obj => obj.Valor <= valorFinal);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento.Todos)
                result = result.Where(o => o.Situacao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.Motorista)
                .ToList();

        }

        public int ContarConsultar(int codigoMotorista, decimal valorInicial, decimal valorFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Adiantamento>();

            var result = from obj in query select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (valorInicial > 0)
                result = result.Where(obj => obj.Valor >= valorInicial);

            if (valorFinal > 0)
                result = result.Where(obj => obj.Valor <= valorFinal);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento.Todos)
                result = result.Where(o => o.Situacao == situacao);

            return result.Count();

        }
    }
}
