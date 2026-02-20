using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class EmpresaIntegracao : RepositorioBase<Dominio.Entidades.EmpresaIntegracao>
    {
        public EmpresaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.EmpresaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasSemIntegracao()
        {
            var consultaEmpresaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>();
            var consultaEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                .Where(o => o.Status == "A");

            consultaEmpresa = consultaEmpresa.Where(o => !consultaEmpresaIntegracao.Where(i => i.Empresa.Codigo == o.Codigo).Any());

            return consultaEmpresa
                .OrderBy(o => o.Codigo)
#if DEBUG
                .Skip(0)
                .Take(10)
#endif
                .ToList();
        }

        public List<Dominio.Entidades.EmpresaIntegracao> BuscarEmpresasParaNovaIntegracao(int intervaloConsultaSintegraEmMeses)
        {
            DateTime dataLimiteProximaConsulta = DateTime.Now.Date.AddMonths(-intervaloConsultaSintegraEmMeses);

            var consultaEmpresaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>()
                .Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado && o.Empresa.Status == "A" &&
                (o.Empresa.DataUltimaConsultaSintegra.Value <= dataLimiteProximaConsulta || o.Empresa.DataProximaConsultaSintegra.Value <= DateTime.Now.Date));

            return consultaEmpresaIntegracao.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.EmpresaIntegracao> BuscarIntegracaoPendente()
        {
            DateTime dataLimiteProximaTentativa = DateTime.Now.AddMinutes(-5d);
            int numeroTentativasLimite = 3;

            var consultaEmpresaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>()
                .Where(o => o.Empresa.Status == "A" &&
                    (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                    (
                        o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < numeroTentativasLimite &&
                        o.DataIntegracao <= dataLimiteProximaTentativa
                    ))
                );

            return consultaEmpresaIntegracao.OrderBy(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.EmpresaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarArquivoHistoricoPorCodigo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            var result = from obj in query where obj.Codigo == codigoArquivo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EmpresaIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportadorIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            consulta = consulta
                .Fetch(o => o.TipoIntegracao);

            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportadorIntegracao filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public List<Dominio.Entidades.EmpresaIntegracao> ConsultarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>()
                .Where(o => o.SituacaoIntegracao == situacaoIntegracao);

            return query.ToList();
        }

        public int ContarConsultaPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>()
                .Where(o => o.SituacaoIntegracao == situacaoIntegracao);

            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.EmpresaIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportadorIntegracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntegracao>();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao.Date >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao.Date <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.AtualizouCadastro.HasValue)
            {
                if (filtrosPesquisa.AtualizouCadastro.Value)
                    query = query.Where(o => !o.Empresa.OptanteSimplesNacional && o.Empresa.OptanteSimplesNacional == o.SimplesNacional && o.AtualizouCadastro);
                else
                    query = query.Where(o => o.Empresa.OptanteSimplesNacional && o.Empresa.OptanteSimplesNacional == o.SimplesNacional && o.AtualizouCadastro);
            }

            return query;
        }

        #endregion
    }
}
