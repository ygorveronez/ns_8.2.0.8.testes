using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorAverbacao : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>
    {
        public TransportadorAverbacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TransportadorAverbacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> BuscarPorTransportador(int transportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.TipoOperacao == null &&

                            obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date &&
                            obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date &&
                            obj.ApoliceSeguro.Ativa == true
                         select obj;

            return result.Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ThenFetch(obj => obj.ClienteSeguradora)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarApolicePorTransportador(int transportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.TipoOperacao == null &&
                            obj.ApoliceSeguro != null &&

                            obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date &&
                            obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date &&
                            obj.ApoliceSeguro.Ativa
                         select obj;

            return result.Select(obj => obj.ApoliceSeguro).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>> BuscarPorTransportadorAsync(int transportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.TipoOperacao == null &&

                            obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date &&
                            obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date &&
                            obj.ApoliceSeguro.Ativa
                         select obj;

            return result.Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ThenFetch(obj => obj.ClienteSeguradora)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> BuscarPorTipoOperacao(int transportador, int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.TipoOperacao.Codigo == tipoOperacao &&

                            obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date &&
                            obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date &&
                            obj.ApoliceSeguro.Ativa == true
                         select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>> BuscarPorTipoOperacaoAsync(int transportador, int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.TipoOperacao.Codigo == tipoOperacao &&

                            obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date &&
                            obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date &&
                            obj.ApoliceSeguro.Ativa
                         select obj;

            return result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> BuscarPorTodosTransportador(int transportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date &&
                            obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date &&
                            obj.ApoliceSeguro.Ativa == true
                         select obj;

            return result.Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ThenFetch(obj => obj.ClienteSeguradora)
                .ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>> BuscarPorTodosTransportadorAsync(int transportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date &&
                            obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date &&
                            obj.ApoliceSeguro.Ativa == true
                         select obj;

            return await result.Fetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ThenFetch(obj => obj.ClienteSeguradora)
                .ToListAsync();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> _Consultar(int transportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador
                         select obj;

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> Consultar(int transportador, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(transportador);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(int transportador)
        {
            var result = _Consultar(transportador);

            return result.Count();
        }

        public bool ValidaDuplicidade(int transportador, int tipoOperacao, int apoliceSeguro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

            var result = from obj in query
                         where
                            obj.Empresa.Codigo == transportador &&
                            obj.ApoliceSeguro.Codigo == apoliceSeguro
                         select obj;

            if (tipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao.Codigo == tipoOperacao);

            return result.Count() == 0;
        }
    }
}
