using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracoesEntreSistemas: LongRunningProcessBase<IntegracoesEntreSistemas>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ConsultarRotasAngelLira(unitOfWork);
            ConsultarDocumentoTransporteNatura(unitOfWork, _stringConexao);
        }

        public void ConsultarDocumentoTransporteNatura(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
                return;

            Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura svcIntegracaoNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura(unidadeTrabalho);
            svcIntegracaoNatura.ConsultarDT(null, 0, DateTime.Now, DateTime.Now, unidadeTrabalho);
        }

        public void ConsultarFaturaNatura(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura svcIntegracaoNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoFaturaNatura();

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
                return;

            List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarAtivasProducao(0);

            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                svcIntegracaoNatura.ConsultarPreFaturas(null, empresa, 0L, DateTime.Now, DateTime.Now, false, unidadeTrabalho);
            }
        }

        public void ConsultarRotasAngelLira(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira = repIntegracaoAngelLira.Buscar();

            if (integracaoAngelLira?.ObterRotasAutomaticamente ?? false)
                Servicos.Embarcador.Integracao.AngelLira.Rota.ObterRotasAngelLira(unitOfWork);
        }
    }
}