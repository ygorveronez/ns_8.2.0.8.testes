using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCancelamentoCargaCTeIntegracaoDados : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>
    {
        public CargaCancelamentoCargaCTeIntegracaoDados(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                                (
                                    obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao
                                    && obj.NumeroTentativas < numeroTentativas
                                    && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                )
                            )
                            && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public bool PossuiIntegracoesPendentesPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();

            var result = query.Where(obj => obj.CargaCancelamento.Codigo == codigoCargaCancelamento &&
                obj.TipoIntegracao.Ativo &&
                (obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao));

            return result.Any();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados> Consultar(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();
            var result = from obj in query select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigo && o.SituacaoIntegracao == situacao);
            else
                result = result.Where(o => o.CargaCancelamento.Codigo == codigo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBusca(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();
            var result = from obj in query select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigo && o.SituacaoIntegracao == situacao);
            else
                result = result.Where(o => o.CargaCancelamento.Codigo == codigo);

            return result.Count();
        }

        public int ContarPorCargaCancelamento(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();

            query = query.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento && o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados> BuscarIntegracoesPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();

            return query.Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados BuscarPorCodigo(int codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();

            return query.Where(o => o.Codigo == codigoIntegracao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();

            return query.Where(o => o.ArquivosTransacao.Any(x => x.Codigo == codigoArquivo)).FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados> BuscarIntegracoesPorCargaCancelamentoCte(int codigoCargaCancelamento, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados>();
            var result = query.Where(obj => obj.CargaCancelamento.Codigo == codigoCargaCancelamento && obj.TipoIntegracao.Codigo == codigoTipoIntegracao);
            return result.ToList();
        }

    }
}
