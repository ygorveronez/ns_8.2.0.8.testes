using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class RegrasPagamentoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>
    {
        public RegrasPagamentoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> BuscarRegraPorEmpresa(int codigoEmpresa, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.Empresa.Codigo == codigoEmpresa) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.Empresa.Codigo != codigoEmpresa)
                         select obj.RegrasPagamentoMotorista;

            result = result.Where(o => o.RegraPorEmpresa == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> BuscarRegraPorTipoPagamento(int codigoTipoPagamento, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.PagamentoMotoristaTipo.Codigo == codigoTipoPagamento) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.PagamentoMotoristaTipo.Codigo != codigoTipoPagamento)
                         select obj.RegrasPagamentoMotorista;

            result = result.Where(o => o.RegraPorEmpresa == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> BuscarRegraPorValor(decimal valorOcorrencia, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.Valor == valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.Valor != valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && obj.Valor <= valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && obj.Valor < valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && obj.Valor >= valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && obj.Valor > valorOcorrencia)
                         select obj.RegrasPagamentoMotorista;

            result = result.Where(o => o.RegraPorValor == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> Consultar(string descricao, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }
    }
}