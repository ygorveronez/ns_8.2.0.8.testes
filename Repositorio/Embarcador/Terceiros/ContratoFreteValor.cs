using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteValor : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>
    {
        public ContratoFreteValor(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ContratoFreteValor(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> Consultar(int codigoContratoFrete, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete);
            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoContratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete);
            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> ConsultarPorCarga(int codigoCarga, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Carga.Codigo == codigoCarga);
            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsultaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Carga.Codigo == codigoCarga);
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> BuscarPorContratoFrete(int codigoContratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete);
            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>> BuscarPorContratoFreteAsync(int codigoContratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete);
            return query.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor BuscarPorContratoFreteJustificativaEValor(int codigoContratoFrete, int codigoJustificativa, decimal valor)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();

            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete && o.Justificativa.Codigo == codigoJustificativa && o.Valor == valor);

            return query.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public decimal BuscarValorPorContratoFrete(int codigoContratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete aplicacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete && o.TipoJustificativa == tipo && o.AplicacaoValor == aplicacao);
            return query.Sum(o => (decimal?)o.Valor) ?? 0m;
        }

        public decimal BuscarValorPorTerceiroPorPeriodo(int codigoContratoFreteDiff, double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] aplicacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();

            query = query.Where(o => o.ContratoFrete.Codigo != codigoContratoFreteDiff && o.ContratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado && o.ContratoFrete.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro && o.ContratoFrete.DataEmissaoContrato >= dataInicial && o.ContratoFrete.DataEmissaoContrato < dataFinal && o.TipoJustificativa == tipo && aplicacao.Contains(o.AplicacaoValor));

            return query.Sum(o => (decimal?)o.Valor) ?? 0m;
        }
        public decimal BuscarValorPorRaizCNPJPorPeriodo(int codigoContratoFreteDiff, string cnpjEmpresa, double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete[] aplicacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();

            query = query.Where(o => o.ContratoFrete.Codigo != codigoContratoFreteDiff && o.ContratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado && o.ContratoFrete.Carga.Empresa.CNPJ.Substring(0, 8) == cnpjEmpresa.Substring(0, 8) && o.ContratoFrete.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro && o.ContratoFrete.DataEmissaoContrato >= dataInicial && o.ContratoFrete.DataEmissaoContrato < dataFinal && o.TipoJustificativa == tipo && aplicacao.Contains(o.AplicacaoValor));

            return query.Sum(o => (decimal?)o.Valor) ?? 0m;
        }
        public decimal BuscarValorPorContratoFrete(int codigoContratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete && o.TipoJustificativa == tipo);
            return query.Sum(o => (decimal?)o.Valor) ?? 0m;
        }

        public bool ExistePorJustificativa(int codigoContratoFrete, int codigoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();

            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete && o.Justificativa.Codigo == codigoJustificativa);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> BuscarGeradosAutomaticamentePorContratoFrete(int codigoContratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor>();
            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete && o.GeradoAutomaticamente);
            return query.ToList();
        }
    }
}
