using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class GerarCargaPorMDFe : LongRunningProcessBase<GerarCargaPorMDFe>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarMDFesPendentesCarga(unitOfWork.StringConexao, unitOfWork);
        }

        private void VerificarMDFesPendentesCarga(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unitOfWork);
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);

            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            int quantidadePendentes = repIntegracaoMDFe.ContarPendentesIntegracaoMDFe();
            Servicos.Log.GravarInfo($"MDFes pendentes de geração de carga: {quantidadePendentes}", "GerarCargaMDFes");

            if (quantidadePendentes > 0)
            {
                List<int> listaIntegracaoMDFe = repIntegracaoMDFe.BuscarPendentesIntegracaoMDFe(50);

                for (var i = 0; i < listaIntegracaoMDFe.Count; i++)
                {
                    Dominio.Entidades.IntegracaoMDFe integracaoMDFe = repIntegracaoMDFe.BuscarPorCodigo(listaIntegracaoMDFe[i]);

                    if (integracaoMDFe != null)
                    {
                        try
                        {
                            unitOfWork.Start();

                            List<int> listaIntegracaoCTe = repDocumentoMDFe.BuscarCodigosDeCTesPorMDFe(integracaoMDFe.MDFe.Codigo);
                            string numeroCargaSequencial = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();
                            int codigoCargaGerada = 0;

                            for (var j = 0; j < listaIntegracaoCTe.Count; j++)
                            {
                                Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPorCTe(listaIntegracaoCTe[j]);
                                if (integracaoCTe != null)
                                {
                                    try
                                    {
                                        string tipoVeiculo = "";
                                        try
                                        {
                                            if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO VEICULO:"))
                                            {
                                                int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO VEICULO:");
                                                int posicaoFim = posicao + 13 + 8;
                                                if (posicao > -1 && posicaoFim > -1)
                                                {
                                                    int inicio = posicao + 13;
                                                    int tamanho = posicaoFim - (posicao + 13);
                                                    tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(inicio, tamanho).Replace(" ", "");
                                                }
                                            }
                                            else if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO: VEICULO"))
                                            {
                                                int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO: VEICULO");
                                                int posicaoFim = posicao + 13 + 8;
                                                if (posicao > -1 && posicaoFim > -1)
                                                    tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(posicao + 13, posicaoFim - (posicao + 13)).Replace(" ", "");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("Problema ao gerar definir tipo de veiculo: " + ex.Message, "GerarCargaMDFes");
                                        }

                                        Servicos.Log.TratarErro("Gerando carga CTe: " + integracaoCTe.CTe.Codigo, "GerarCargaMDFes");
                                        int codigoCarga = servicoCTe.GerarCargaCTe(integracaoCTe.CTe.Codigo, "Todas", numeroCargaSequencial, tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, stringConexao, unitOfWork);

                                        if (codigoCarga > 0)
                                        {
                                            codigoCargaGerada = codigoCarga;

                                            integracaoCTe.NumeroDaCarga = numeroCargaSequencial.ToInt();
                                            integracaoCTe.GerouCargaEmbarcador = true;
                                            repIntegracaoCTe.Atualizar(integracaoCTe);

                                            integracaoMDFe.GerouCargaEmbarcador = true;
                                            repIntegracaoMDFe.Atualizar(integracaoMDFe);
                                        }
                                        else
                                        {
                                            Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo : " + integracaoCTe.CTe.Codigo, "GerarCargaMDFes");

                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex.Message, "GerarCargaMDFes");

                                        throw;
                                    }
                                }
                            }

                            if (codigoCargaGerada > 0)
                                FinalizarGeracaoCarga(integracaoMDFe, codigoCargaGerada, unitOfWork);

                            unitOfWork.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao gerar carga MDFe codigo " + integracaoMDFe.MDFe.Codigo + ": " + ex.Message, "GerarCargaMDFes");
                            unitOfWork.Rollback();
                        }
                    }

                    unitOfWork.FlushAndClear();
                }
            }
        }

        private void FinalizarGeracaoCarga(Dominio.Entidades.IntegracaoMDFe integracaoMDFe, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);
            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
            if (carga == null)
            {
                Servicos.Log.TratarErro($"Carga: {integracaoMDFe.NumeroDaCarga} não encontrada.", "GerarCargaMDFes");
                return;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(carga.Codigo);

            VincularAverbacaoCTeCarga(cargaCTes, cargaPedidos.FirstOrDefault(), unitOfWork);

            servicoCTe.AtualizarValoresCarga(carga, unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            servicoCarga.FecharCarga(carga, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null);

            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacaoPorCTe(carga, cargaPedidos, cargaCTes, unitOfWork, configuracaoPedido);

            Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

            servicoMDFe.AdicionarMDFeNaCarga(integracaoMDFe.MDFe, carga, unitOfWork);
            VincularAverbacaoMDFeCarga(integracaoMDFe.MDFe.Codigo, cargaPedidos.FirstOrDefault(), unitOfWork);

            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;

            if (carga.FreteDeTerceiro && carga.Terceiro == null && carga.Veiculo != null && carga.Veiculo.Proprietario != null)
                carga.Terceiro = carga.Veiculo.Proprietario;
            else if (carga.ProvedorOS != null)
                carga.Terceiro = carga.ProvedorOS;

            carga.CargaFechada = true;
            Servicos.Log.TratarErro("17 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

            carga.DataInicioCalculoFrete = null;
            carga.CalculandoFrete = false;
            carga.CalcularFreteSemEstornarComplemento = false;
            carga.EmitindoCTes = false;
            carga.PossuiPendencia = false;
            carga.AgImportacaoCTe = false;
            repositorioCarga.Atualizar(carga);

            Servicos.Log.TratarErro($"Carga: {integracaoMDFe.NumeroDaCarga} finalizada com sucesso.", "GerarCargaMDFes");
        }

        private void VincularAverbacaoCTeCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedido == null || cargaCTes.Count <= 0)
                return;

            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.AverbacaoCTe repositorioAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

            List<int> codigoCTes = cargaCTes.Select(cte => cte.Codigo).ToList();

            List<Dominio.Entidades.AverbacaoCTe> averbacoesCte = repositorioAverbacaoCTe.BuscarPorCodigoCTes(codigoCTes);

            foreach (Dominio.Entidades.AverbacaoCTe averbacaoCTe in averbacoesCte)
            {
                if (averbacaoCTe.ApoliceSeguroAverbacao.CargaPedido != null)
                {
                    averbacaoCTe.ApoliceSeguroAverbacao.CargaPedido = cargaPedido;
                    repositorioApoliceSeguroAverbacao.Atualizar(averbacaoCTe.ApoliceSeguroAverbacao);
                }
            }
        }

        private void VincularAverbacaoMDFeCarga(int codigoMDFe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedido == null)
                return;

            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.AverbacaoMDFe repositorioAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            List<Dominio.Entidades.AverbacaoMDFe> averbacoesMDFe = repositorioAverbacaoMDFe.BuscarPorCodigoMDFe(codigoMDFe);
            
            foreach (Dominio.Entidades.AverbacaoMDFe averbacaoMDFe in averbacoesMDFe)
            {
                if (averbacaoMDFe.Carga != null)
                {
                    averbacaoMDFe.Carga = cargaPedido.Carga;
                    averbacaoMDFe.ApoliceSeguroAverbacao.CargaPedido = cargaPedido;
                    repositorioApoliceSeguroAverbacao.Atualizar(averbacaoMDFe.ApoliceSeguroAverbacao);
                }
            }
        }

        #endregion Métodos Protegidos
    }
}