using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SGT.BackgroundWorkers.Utils;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 6000)]
    public class MDFeManual : LongRunningProcessBase<MDFeManual>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManual serIntegracaoCargaMDFeManual = new Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManual(unitOfWork, _tipoServicoMultisoftware);
            Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManualCancelamento serIntegracaoCargaMDFeManualCancelamento = new Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManualCancelamento(unitOfWork, _tipoServicoMultisoftware);

            servicoCarga.ProcessarCargaParaAvancoAutomaticoEtapaFrete(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _webServiceConsultaCTe, _auditado);
            servicoCarga.ProcessarCargaParaAvancoAutomaticoDocumentosFiscais(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            VerificarMDFeManualEmEmissao(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
            VerificarMDFeManualEmCancelamento(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
            VerificarMDFeManualProcessandoIntegracao(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);

            Servicos.Embarcador.Integracao.MultiEmbarcador.Veiculo.IntegrarVeiculosPendentesIntegracao(unitOfWork);
            Servicos.Embarcador.Veiculo.Veiculo.IntegrarVeiculosPendentesIntegracaoTrocaMotorista(unitOfWork);
            Servicos.Embarcador.Carga.Carga.VincularNotasFiscaisParciaisDosPedidos(_tipoServicoMultisoftware, unitOfWork, _auditado);

            serIntegracaoCargaMDFeManual.VerificarIntegracoesPendentes();
            serIntegracaoCargaMDFeManualCancelamento.VerificarIntegracoesPendentes();
        }

        private void VerificarMDFeManualProcessandoIntegracao(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                List<int> cargaMDFesEmEmissao = repCargaMDFeManual.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.ProcessandoIntegracao, 0, 5);

                for (var i = 0; i < cargaMDFesEmEmissao.Count(); i++)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(cargaMDFesEmEmissao[i]);
                    if (cargaMDFeManual.Motoristas == null || cargaMDFeManual.Motoristas.Count == 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(cargaMDFeManual.Cargas.FirstOrDefault().Codigo);

                        if (cargaMotoristas != null && cargaMotoristas.Count > 0)
                        {
                            if (cargaMDFeManual.Motoristas == null)
                                cargaMDFeManual.Motoristas = new List<Dominio.Entidades.Usuario>();
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
                                cargaMDFeManual.Motoristas.Add(cargaMotorista.Motorista);
                        }
                    }

                    if (cargaMDFeManual.Veiculo == null)
                    {
                        if (cargaMDFeManual.Cargas.FirstOrDefault().Veiculo != null)
                        {
                            cargaMDFeManual.Veiculo = cargaMDFeManual.Cargas.FirstOrDefault().Veiculo;

                            if (cargaMDFeManual.Veiculo.TipoVeiculo == "1")
                            {
                                if (cargaMDFeManual.Veiculo.VeiculosTracao != null && cargaMDFeManual.Veiculo.VeiculosTracao.Count > 0)
                                {
                                    Dominio.Entidades.Veiculo tracao = (from obj in cargaMDFeManual.Veiculo.VeiculosTracao where obj.Ativo select obj).FirstOrDefault();

                                    if (tracao != null)
                                        cargaMDFeManual.Veiculo = tracao;
                                }
                            }
                        }
                    }

                    if (cargaMDFeManual.Reboques == null || cargaMDFeManual.Reboques.Count == 0)
                    {
                        if (cargaMDFeManual.Cargas.FirstOrDefault().VeiculosVinculados != null && cargaMDFeManual.Cargas.FirstOrDefault().VeiculosVinculados.Count > 0)
                        {
                            if (cargaMDFeManual.Reboques == null)
                                cargaMDFeManual.Reboques = new List<Dominio.Entidades.Veiculo>();

                            foreach (Dominio.Entidades.Veiculo veiculoVinculado in cargaMDFeManual.Cargas.FirstOrDefault().VeiculosVinculados)
                            {
                                if (cargaMDFeManual.Veiculo == null || veiculoVinculado.Placa != cargaMDFeManual.Veiculo.Placa)
                                    cargaMDFeManual.Reboques.Add(veiculoVinculado);
                            }
                        }

                    }

                    SalvarDestinos(ref cargaMDFeManual, configuracao, tipoServicoMultisoftware, unitOfWork);

                    if (cargaMDFeManual.Motoristas == null || cargaMDFeManual.Motoristas.Count <= 0)
                    {
                        cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao;
                        cargaMDFeManual.RetornoProcessamento = "MDFe manual sem motorista.";
                        repCargaMDFeManual.Atualizar(cargaMDFeManual);
                        unitOfWork.CommitChanges();
                        return;
                    }

                    if (cargaMDFeManual.Veiculo == null)
                    {
                        cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao;
                        cargaMDFeManual.RetornoProcessamento = "MDFe manual sem veículo.";
                        repCargaMDFeManual.Atualizar(cargaMDFeManual);
                        unitOfWork.CommitChanges();
                        return;
                    }

                    if (cargaMDFeManual.Reboques == null || cargaMDFeManual.Reboques.Count == 0)
                    {
                        if (cargaMDFeManual.Reboques == null)
                            cargaMDFeManual.Reboques = new List<Dominio.Entidades.Veiculo>();

                        foreach (Dominio.Entidades.Veiculo veiculoVinculado in cargaMDFeManual.Veiculo.VeiculosVinculados)
                            cargaMDFeManual.Reboques.Add(veiculoVinculado);
                    }

                    cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao;
                    repCargaMDFeManual.Atualizar(cargaMDFeManual);

                    unitOfWork.CommitChanges();

                    string mensagemErro = string.Empty;
                    GerarMDFe(out mensagemErro, cargaMDFeManual, tipoServicoMultisoftware, webServiceConsultaCTe, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao;
                        cargaMDFeManual.RetornoProcessamento = mensagemErro.Left(1000);
                        repCargaMDFeManual.Atualizar(cargaMDFeManual);
                    }

                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarMDFeManualEmEmissao(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);
                Servicos.Embarcador.Hubs.MDFeManual svcHubMDFeManual = new Servicos.Embarcador.Hubs.MDFeManual();
                Servicos.Embarcador.Hubs.MDFeAquaviario svcHubMDFeAquaviario = new Servicos.Embarcador.Hubs.MDFeAquaviario();
                Servicos.Embarcador.Carga.CargaMDFeManual serCargasMDFeManual = new Servicos.Embarcador.Carga.CargaMDFeManual();


                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> cargaMDFesEmEmissao = repCargaMDFeManual.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao, 0, 20);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual in cargaMDFesEmEmissao)
                {
                    if (cargaMDFeManual.MDFeManualMDFes.Any(obj => obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao))
                    {
                        cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Rejeicao;
                        repCargaMDFeManual.Atualizar(cargaMDFeManual);
                        if (cargaMDFeManual.TipoModalMDFe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario)
                            svcHubMDFeAquaviario.InformarCargaMDFeAquaviarioAtualizado(cargaMDFeManual.Codigo);
                        else
                            svcHubMDFeManual.InformarCargaMDFeManualAtualizado(cargaMDFeManual.Codigo);
                    }

                    if (cargaMDFeManual.MDFeManualMDFes.All(obj => obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado))
                    {
                        serCargasMDFeManual.VincularMDfesACarga(cargaMDFeManual, unitOfWork);
                        if (cargaMDFeManual.TipoModalMDFe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario)
                            svcHubMDFeAquaviario.InformarCargaMDFeAquaviarioAtualizado(cargaMDFeManual.Codigo);
                        else
                            svcHubMDFeManual.InformarCargaMDFeManualAtualizado(cargaMDFeManual.Codigo);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarMDFeManualEmCancelamento(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Servicos.Embarcador.Hubs.MDFeManual svcHubMDFeManual = new Servicos.Embarcador.Hubs.MDFeManual();

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> cargaMDFesEmCancelamento = repCargaMDFeManualCancelamento.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.EmCancelamento, 0, 20);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento in cargaMDFesEmCancelamento)
                {
                    if (cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes.Any(obj => obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado))
                    {
                        cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.CancelamentoRejeitado;
                        cargaMDFeManualCancelamento.MotivoRejeicaoCancelamento = "Não foi possível cancelar os MDF-es, por favor, tente novamente.";
                        repCargaMDFeManualCancelamento.Atualizar(cargaMDFeManualCancelamento);

                        svcHubMDFeManual.InformarCargaMDFeManualAtualizadoCancelamento(cargaMDFeManualCancelamento.Codigo);
                    }

                    if (cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes.All(obj => obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao))
                    {
                        unitOfWork.Start();
                        cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.AgIntegracao;
                        cargaMDFeManualCancelamento.GerandoIntegracoes = true;
                        cargaMDFeManualCancelamento.MotivoRejeicaoCancelamento = "";
                        repCargaMDFeManualCancelamento.Atualizar(cargaMDFeManualCancelamento);
                        cargaMDFeManualCancelamento.CargaMDFeManual.SituacaoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.AgIntegracao;
                        repCargaMDFeManual.Atualizar(cargaMDFeManualCancelamento.CargaMDFeManual);

                        if (cargaMDFeManualCancelamento.CargaMDFeManual.MDFeEnviadoComSucessoPelaIntegracao == true && cargaMDFeManualCancelamento.CargaMDFeManual.MDFeRecebidoDeIntegracao != true && cargaMDFeManualCancelamento.RecebidoPorIntegracao != true && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                        {
                            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab);
                            if (tipoIntegracao != null)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao cargaMDFeAquaviarioManual = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao()
                                {
                                    CargaMDFeManualCancelamento = cargaMDFeManualCancelamento,
                                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                                    TipoIntegracao = tipoIntegracao,
                                    DataIntegracao = DateTime.Now,
                                    NumeroTentativas = 0,
                                    ProblemaIntegracao = ""
                                };
                                repCargaMDFeManualCancelamentoIntegracao.Inserir(cargaMDFeAquaviarioManual);
                            }
                        }

                        serCargaDadosSumarizados.AtualizarDadosMDFeAquaviario(cargaMDFeManualCancelamento.CargaMDFeManual.Codigo, unitOfWork);
                        serCargaDadosSumarizados.ConsultarMDFeAquaviarioJaGeradoPorMDFe(cargaMDFeManualCancelamento.CargaMDFeManual.Codigo, unitOfWork);

                        unitOfWork.CommitChanges();
                        svcHubMDFeManual.InformarCargaMDFeManualAtualizadoCancelamento(cargaMDFeManualCancelamento.Codigo);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void SalvarDestinos(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaMDFeManual.Cargas.ToList())
            {
                int empresa = carga.Empresa.Codigo;
                if (carga.EmpresaFilialEmissora != null && carga.EmiteMDFeFilialEmissora)
                    empresa = carga.EmpresaFilialEmissora.Codigo;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = null;
                if (!configuracao.AgruparCargaAutomaticamente)
                    cargaCTEs = repCargaCTe.BuscarPorCargaParaMDFe(carga.Codigo, empresa, tipoServicoMultisoftware, true, false, true, true);
                else
                    cargaCTEs = repCargaCTe.BuscarPorCargaOrigemParaMDFe(carga.Codigo, empresa, tipoServicoMultisoftware, true, false, true, true);

                ctes.AddRange((from obj in cargaCTEs select obj.CTe).ToList());
            }

            List<Dominio.Entidades.Localidade> localidadesDestino = (from obj in ctes select obj.LocalidadeTerminoPrestacao).Distinct().ToList();

            foreach (Dominio.Entidades.Localidade localidade in localidadesDestino)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino destino = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino();

                destino.CargaMDFeManual = cargaMDFeManual;
                destino.Localidade = localidade;
                destino.Ordem = 0;

                repCargaMDFeManualDestino.Inserir(destino);
            }
        }

        private bool GerarMDFe(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualLacre repLacre = new Repositorio.Embarcador.Cargas.CargaMDFeManualLacre(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);
            Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            string retorno = svcMDFe.EmitirMDFe(cargaMDFeManual, configuracaoTMS, tipoServicoMultisoftware, webServiceConsultaCTe, unidadeTrabalho);

            if (string.IsNullOrWhiteSpace(retorno))
            {
                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                repCargaMDFeManual.Atualizar(cargaMDFeManual);
                erro = string.Empty;
                return true;
            }
            else
            {
                erro = retorno;
                return false;
            }
        }
    }
}