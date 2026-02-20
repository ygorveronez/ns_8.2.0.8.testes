using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoMarfrig : LongRunningProcessBase<IntegracaoMarfrig>
    {

        public Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            _configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            CriarIntegracaoTitulos(unitOfWork, _tipoServicoMultisoftware);

            IntegrarTitulos(unitOfWork, _tipoServicoMultisoftware);
        }

        private void CriarIntegracaoTitulos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig servicoMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig(unitOfWork);

            if (_configuracaoIntegracao != null && _configuracaoIntegracao.PossuiIntegracaoMarfrig && !string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLMarfrig) && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Servicos.Log.TratarErro($"Integração Marfrig CRIACAO titulos Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ");

                servicoMarfrig.CriarIntegracaoCteTitulosReceber(unitOfWork, _configuracaoIntegracao);

                Servicos.Log.TratarErro($"Integração Marfrig CRIACAO titulos Finalizada e Inciando CONSULTA | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ");
            }

        }

        private void IntegrarTitulos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig servicoMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig(unitOfWork);

            if (_configuracaoIntegracao != null && _configuracaoIntegracao.PossuiIntegracaoMarfrig && !string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLMarfrig) && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber repIntegracaoMarfrigCteTitulo = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(unitOfWork);

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber> ListaIntegracaoMarfigCteTituloReceberAProcessar = repIntegracaoMarfrigCteTitulo.BuscarAguardandoIntegracao(_configuracaoIntegracao.DataCorteConsultaIntegracaoTituloMarfrig);

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber> ListaFiltrada = servicoMarfrig.ObterListaFiltradaCTeAConsultar(ListaIntegracaoMarfigCteTituloReceberAProcessar);

                foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracao in ListaFiltrada)
                {
                    try
                    {
                        unitOfWork.Start();

                        if (integracao != null)
                            servicoMarfrig.IntegrarCTeTituloReceber(integracao, false, null);

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception excecao)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(excecao);
                    }
                }

                Servicos.Log.TratarErro($"Integração Marfrig CONSULTA Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ");
            }

        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                return new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig);
        }

    }
}