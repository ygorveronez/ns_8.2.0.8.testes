using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public sealed class FechamentoPontuacaoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador>
    {
        #region Construtores

        public FechamentoPontuacaoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> Consultar(int codigoFechamentoPontuacao)
        {
            var consultaFechamentoPontuacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador>()
                .Where(o => o.FechamentoPontuacao.Codigo == codigoFechamentoPontuacao);

            return consultaFechamentoPontuacaoTransportador;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador BuscarPorFechamentoPontuacaoETransportador(int codigoFechamentoPontuacao, int codigoTransportador)
        {
            var consultaFechamentoPontuacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador>()
                .Where(o =>
                    o.FechamentoPontuacao.Codigo == codigoFechamentoPontuacao &&
                    o.Transportador.Codigo == codigoTransportador
                );

            return consultaFechamentoPontuacaoTransportador.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> Consultar(int codigoFechamentoPontuacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFechamentoPontuacaoTransportador = Consultar(codigoFechamentoPontuacao);

            return ObterLista(consultaFechamentoPontuacaoTransportador, parametrosConsulta);
        }

        public int ContarConsulta(int codigoFechamentoPontuacao)
        {
            var consultaFechamentoPontuacaoTransportador = Consultar(codigoFechamentoPontuacao);

            return consultaFechamentoPontuacaoTransportador.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> BuscarPorTransportadores(List<int> codigosTransportadores)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador>()
                .Where(obj => codigosTransportadores.Contains(obj.Transportador.Codigo));
            
            return query
                .Fetch(obj => obj.FechamentoPontuacao)
                .ToList();
        }

        #endregion
    }
}
