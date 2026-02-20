using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Escrituracao
{
    public class ProvisaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>
    {
        public ProvisaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao> BuscarPorProvisao(int codigoProvisao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao);

            return consultaIntegracoes.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao BuscarPorProvisaoETipoIntegracao(int codigoProvisao, int codigoTipoIntegracao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao && obj.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return consultaIntegracoes.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao BuscarPorCarga(int protocoloCarga)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>()
                .Where(obj => obj.Provisao.Carga.Codigo == protocoloCarga);

            return consultaIntegracoes
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public int ContarPorProvisao(int codigoProvisao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao);

            return consultaIntegracoes.Count();
        }

        public int ContarPorProvisaoESituacao(int codigoProvisao, SituacaoIntegracao situacao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao && obj.SituacaoIntegracao == situacao);

            return consultaIntegracoes.Count();
        }

        public int ContarPorProvisaoESituacoes(int codigoProvisao, List<SituacaoIntegracao> situacoes)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>()
                .Where(obj => obj.Provisao.Codigo == codigoProvisao && situacoes.Contains(obj.SituacaoIntegracao));

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, int limiteRegistros)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>()
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
                .ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao> _Consultar(int codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoProvisao > 0)
                result = result.Where(obj => obj.Provisao.Codigo == codigoProvisao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao> Consultar(int codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoProvisao, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoProvisao, SituacaoIntegracao? situacao)
        {
            var result = _Consultar(codigoProvisao, situacao);

            return result.Count();
        }
    }
}
