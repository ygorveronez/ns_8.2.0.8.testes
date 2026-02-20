using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>
    {
        public LoteEscrituracaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LoteEscrituracaoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao BuscarPorCodigo(int codigo)
        {
            var consultaLoteEscrituracaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>()
                .Where(o => o.Codigo == codigo);

            return consultaLoteEscrituracaoIntegracao.FirstOrDefault();
        }


        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>> BuscarIntegracoesPendentesAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();

            var result = from obj in query
                         where obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                               //!obj.LoteEscrituracao.GerandoIntegracoes &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                 obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result
                .Fetch(obj => obj.LoteEscrituracao)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> BuscarPorLoteEscrituracao(int LoteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();
            var resut = from obj in query where obj.LoteEscrituracao.Codigo == LoteEscrituracao select obj;
            return resut.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorLoteEscrituracao(int codigoLoteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();

            var result = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public int ContarPorLoteEscrituracaoETipoIntegracao(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();

            var resut = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao BuscarPorLoteEscrituracaoETipoIntegracao(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();

            var resut = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> Consultar(int codigoLoteEscrituracao, SituacaoIntegracao? situacao, TipoIntegracao? tipo)
        {
            return Consultar(codigoLoteEscrituracao, situacao, tipo, parametrosConsulta: null);
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> Consultar(int codigoLoteEscrituracao, SituacaoIntegracao? situacao, TipoIntegracao? tipo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaEscrituracaoIntegracao = ConsultarLoteIntegracao(codigoLoteEscrituracao, situacao, tipo);

            consultaEscrituracaoIntegracao = consultaEscrituracaoIntegracao
                .Fetch(o => o.LoteEscrituracao)
                .Fetch(o => o.TipoIntegracao);

            return ObterLista(consultaEscrituracaoIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(int codigoLoteEscrituracao, SituacaoIntegracao? situacao, TipoIntegracao? tipo)
        {
            var consultaEscrituracaoIntegracao = ConsultarLoteIntegracao(codigoLoteEscrituracao, situacao, tipo);

            return consultaEscrituracaoIntegracao.Count();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> ConsultarLoteIntegracao(int codigoLoteEscrituracao, SituacaoIntegracao? situacao, TipoIntegracao? tipo)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();

            if (codigoLoteEscrituracao > 0)
                consultaNFSManualCTeIntegracao = consultaNFSManualCTeIntegracao.Where(o => o.LoteEscrituracao.Codigo == codigoLoteEscrituracao);

            if (situacao.HasValue)
                consultaNFSManualCTeIntegracao = consultaNFSManualCTeIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                consultaNFSManualCTeIntegracao = consultaNFSManualCTeIntegracao.Where(o => o.TipoIntegracao.Tipo == tipo);

            return consultaNFSManualCTeIntegracao;
        }


        public int ContarPorLoteEscrituracao(int codigoLoteEscrituracao, SituacaoIntegracao situacao)
        {
            return ContarLoteEscrituracao(codigoLoteEscrituracao, new SituacaoIntegracao[] { situacao });
        }

        public int ContarLoteEscrituracao(int codigoLoteEscrituracao, SituacaoIntegracao[] situacoes)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>()
                .Where(o => o.LoteEscrituracao.Codigo == codigoLoteEscrituracao && situacoes.Contains(o.SituacaoIntegracao));

            return consultaNFSManualCTeIntegracao.Count();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaLoteEscrituracaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaLoteEscrituracaoIntegracao.FirstOrDefault();
        }

        public int ContarPorLoteEscrituracaoESituacaoDiff(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();

            var result = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

    }
}
