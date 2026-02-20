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
    public class ProvisaoEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>
    {
        public ProvisaoEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ProvisaoEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> Consultar(int codigoProvisao, SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoProvisao > 0)
                result = result.Where(obj => obj.Provisao.Codigo == codigoProvisao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();
        }

        public int ContarConsulta(int codigoProvisao, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoProvisao > 0)
                result = result.Where(obj => obj.Provisao.Codigo == codigoProvisao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao BuscarUltimoPorProvisao(int codigoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query where obj.Provisao.Codigo == codigoProvisao orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> BuscarPorProvisao(int codigoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query where obj.Provisao.Codigo == codigoProvisao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> BuscarPorProvisao(List<int> codigosProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query where codigosProvisao.Contains(obj.Provisao.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao BuscarPorProvisaoELayout(int codigoProvisao, int codigoLayoutEDI)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao && obj.LayoutEDI.Codigo == codigoLayoutEDI);

            return consultaIntegracoes.FirstOrDefault();
        }

        public int ContarPorProvisao(int codigoProvisao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao);

            return consultaIntegracoes.Count();
        }

        public int ContarPorProvisaoESituacao(int codigoProvisao, SituacaoIntegracao situacao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao && obj.SituacaoIntegracao == situacao);

            return consultaIntegracoes.Count();
        }

        public int ContarPorProvisaoESituacoes(int codigoProvisao, List<SituacaoIntegracao> situacoes)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao && situacoes.Contains(obj.SituacaoIntegracao));

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> BuscarPorProvisao(int codigoProvisao, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query where obj.Provisao.Codigo == codigoProvisao select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>> BuscarIntegracoesPendentesAsync(int numeroTentativas, double minutosACadaTentativa, int limiteRegistros, CancellationToken cancellationToken)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>()
                .Where(obj =>
                    (
                        obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                        (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                    ) &&
                    obj.TipoIntegracao.Ativo
                );

            return consultaIntegracoes
                .Fetch(o => o.Provisao)
                .OrderBy(o => o.NumeroTentativas).ThenBy(o => o.Codigo)
                .Take(limiteRegistros)
                .ToListAsync(cancellationToken);
        }

        public List<TipoIntegracao> BuscarTipoIntegracaoPorProvisao(int codigoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query where obj.Provisao.Codigo == codigoProvisao select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao BuscarPorCarga(int protocoloCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao>();

            var result = from obj in query where obj.Provisao.Carga.Codigo == protocoloCarga orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

    }
}
