using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteIntegrarAlteracaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao>
    {
        #region Construtores

        public TabelaFreteIntegrarAlteracaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao> ConsultarPorTabelaFrete(int codigoTabelaFrete, SituacaoIntegracao? situacao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao>()
                .Where(integracao => integracao.TabelaFreteIntegrarAlteracao.TabelaFrete.Codigo == codigoTabelaFrete);

            if (situacao.HasValue)
                consultaIntegracao = consultaIntegracao.Where(integracao => integracao.SituacaoIntegracao == situacao.Value);

            return consultaIntegracao;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao>()
                .Where(integracao =>
                    (
                        integracao.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                        (integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && integracao.NumeroTentativas < numeroTentativas && integracao.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                    ) &&
                    integracao.TipoIntegracao.Ativo
                );

            return consultaIntegracao.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao> ConsultarPorTabelaFrete(int codigoTabelaFrete, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIntegracao = ConsultarPorTabelaFrete(codigoTabelaFrete, situacao);

            return ObterLista(consultaIntegracao, parametrosConsulta);
        }

        public int ContarPorSituacao(int codigoTabelaFreteIntegrarAlteracao, SituacaoIntegracao situacao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao>()
                .Where(integracao => integracao.TabelaFreteIntegrarAlteracao.Codigo == codigoTabelaFreteIntegrarAlteracao && integracao.SituacaoIntegracao == situacao);

            return consultaIntegracao.Count();
        }

        public int ContarConsultaPorTabelaFrete(int codigoTabelaFrete, SituacaoIntegracao? situacao)
        {
            var consultaIntegracao = ConsultarPorTabelaFrete(codigoTabelaFrete, situacao);

            return consultaIntegracao.Count();
        }

        public bool ExistePorTipoIntegracao(int codigoTabelaFreteIntegrarAlteracao, int codigoTipoIntegracao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao>()
                .Where(integracao => integracao.TabelaFreteIntegrarAlteracao.Codigo == codigoTabelaFreteIntegrarAlteracao && integracao.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return consultaIntegracao.Count() > 0;
        }

        #endregion Métodos Públicos
    }
}
