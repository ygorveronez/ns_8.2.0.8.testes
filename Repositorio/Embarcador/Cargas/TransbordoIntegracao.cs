using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas
{
    public class TransbordoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>
    {
        public TransbordoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ContarTransbordoETipoIntegracao(int codigoTransbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();

            var result = from obj in query where obj.Transbordo.Codigo == codigoTransbordo && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> BuscarPorTransbordo(int transbordo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();
            var result = from obj in query where obj.Transbordo.Codigo == transbordo select obj;
            return result.Fetch(o => o.TipoIntegracao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> BuscarPendentesPorTransbordo(int transbordo)
        {
            List<SituacaoIntegracao> integracoesNaoPendentes = new List<SituacaoIntegracao>() { SituacaoIntegracao.Integrado, SituacaoIntegracao.ProblemaIntegracao };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();
            var result = from obj in query where obj.Transbordo.Codigo == transbordo select obj;

            result = result.Where(obj => !integracoesNaoPendentes.Contains(obj.SituacaoIntegracao));

            return result.Fetch(o => o.TipoIntegracao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> BuscarPorTransbordo(int transbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();
            var result = from obj in query where obj.Transbordo.Codigo == transbordo select obj;

            if (situacao.HasValue)
                result = result.Where(obj => obj.SituacaoIntegracao == situacao);

            return result
                .Fetch(o => o.TipoIntegracao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();
            var result = from obj in query where obj.Transbordo.Carga.Codigo == codigoCarga select obj;
            return result.Fetch(o => o.TipoIntegracao).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorTransbordo(int transbordo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();

            var result = from obj in query where obj.Transbordo.Codigo == transbordo select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> Consultar(int codigoTransbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();

            var result = from obj in query select obj;

            if (codigoTransbordo > 0)
                result = result.Where(o => o.Transbordo.Codigo == codigoTransbordo);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).Fetch(obj => obj.TipoIntegracao).ToList();
        }

        public int ContarConsulta(int codigoTransbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();

            var result = from obj in query select obj;

            if (codigoTransbordo > 0)
                result = result.Where(o => o.Transbordo.Codigo == codigoTransbordo);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }
        public Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> BuscarListaIntegracaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao>();

            var result = from obj in query where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao 
                         || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < 3) select obj;

            return result.WithOptions(o => o.SetTimeout(120)).ToList();
        }

    }
}
