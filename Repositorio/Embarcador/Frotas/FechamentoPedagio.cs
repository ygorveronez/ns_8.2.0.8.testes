using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frotas
{
    public class FechamentoPedagio : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio>
    {

        public FechamentoPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> ConsultarPorFechamento(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var result = from obj in query where obj.FechamentoPedagio.Codigo == codigo select obj;

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos)
                result = result.Where(o => o.SituacaoPedagio == situacao);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultarPorFechamento(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var result = from obj in query where obj.FechamentoPedagio.Codigo == codigo select obj;

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos)
                result = result.Where(o => o.SituacaoPedagio == situacao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio> ConsultarFechamentos(int codigoVeiculo, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio>();
            var result = from obj in query select obj;

            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataInicio >= dataInicio && o.DataInicio < dataInicio.AddDays(1));

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataFim >= dataFim && o.DataFim < dataFim.AddDays(1));

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Todas)
                result = result.Where(o => o.Situacao == situacao);

            if (codigoVeiculo > 0)
            {
                var queryPedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
                var resultPedagio = from obj in queryPedagio where obj.FechamentoPedagio != null && obj.Veiculo.Codigo == codigoVeiculo select obj.FechamentoPedagio.Codigo;

                result = result.Where(o => resultPedagio.Contains(o.Codigo));
            }

            result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultarFechamentos(int codigoVeiculo, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio>();
            var result = from obj in query select obj;

            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataInicio >= dataInicio && o.DataInicio < dataInicio.AddDays(1));

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataFim >= dataFim && o.DataFim < dataFim.AddDays(1));

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Todas)
                result = result.Where(o => o.Situacao == situacao);

            if (codigoVeiculo > 0)
            {
                var queryPedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
                var resultPedagio = from obj in queryPedagio where obj.FechamentoPedagio != null && obj.Veiculo.Codigo == codigoVeiculo select obj.FechamentoPedagio.Codigo;

                result = result.Where(o => resultPedagio.Contains(o.Codigo));
            }

            return result.Count();
        }
    }
}
