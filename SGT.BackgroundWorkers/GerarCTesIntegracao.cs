using Dominio.Excecoes.Embarcador;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class GerarCTesIntegracao : LongRunningProcessBase<GerarCTesIntegracao>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            await VerificarCTesPendentes(unitOfWork);
        }

        private async Task VerificarCTesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.GerarCTesIntegracao);

            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);

            List<(int Codigo, int EmpresaCodigo)> codigosComEmpresa = servicoOrquestradorFila.Ordenar(max => repIntegracaoCTe.BuscarIntegracaoCTesPendentesCodigos(max));

            List<Task> tarefas = codigosComEmpresa
                .GroupBy(x => x.EmpresaCodigo)
                .Select(grupo => Task.Run(async () =>
                {
                    foreach (var item in grupo)
                    {
                        CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
                        await ProcessarCteAsync(item.Codigo, cancellationToken);
                    }
                }))
                .ToList();

            await Task.WhenAll(tarefas);
        }

        private async Task ProcessarCteAsync(int codigoIntegracaoCTe, CancellationToken cancellationToken)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.GerarCTesIntegracao);

                Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);
                Repositorio.IntegracaoCTe repositorioIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork, cancellationToken);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

                Dominio.Entidades.IntegracaoCTe integracaoCTe = await repositorioIntegracaoCTe.BuscarPorCodigoAsync(codigoIntegracaoCTe, false);
                integracaoCTe.Tentativas++;
                await repositorioIntegracaoCTe.AtualizarAsync(integracaoCTe);

                Servicos.Log.GravarInfo($"Iniciou geração do CTe {integracaoCTe.CTe.Codigo}", "GerarCTeIntegracao");

                try
                {
                    if (integracaoCTe.CTe.Status == "P")
                    {
                        Dominio.ObjetosDeValor.CTe.CTe documentoCTe = null;
                        Dominio.ObjetosDeValor.CTe.CTeNFSe documentoCTeNFSe = null;
                        try
                        {
                            documentoCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTe>(integracaoCTe.Arquivo);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao converter objeto CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex, "GerarCTeIntegracao");

                            documentoCTeNFSe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTeNFSe>(integracaoCTe.Arquivo);
                        }

                        if (documentoCTe == null && documentoCTeNFSe != null)
                            documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documentoCTeNFSe);

                        await unitOfWork.StartAsync(cancellationToken);

                        //Gambiara feita para a BRF, isso deve ser removido quando a BRF passar a informar o NCM do produto predominante.
                        if (!Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().AmbienteProducao && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().IdentificacaoAmbiente == "BRF" && string.IsNullOrWhiteSpace(documentoCTe.ProdutoPredominanteNCM))
                        {
                            documentoCTe.ProdutoPredominanteNCM = "02011000";
                        }

                        Dominio.Entidades.Empresa empresa = await repositorioEmpresa.BuscarPorCNPJAsync(documentoCTe.Emitente.CNPJ);

                        AjustarDadosCTesInvalidos(documentoCTe);

                        ObterDadosSeguroCTe(documentoCTe, empresa, unitOfWork);

                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorObjeto(documentoCTe, integracaoCTe.CTe.Codigo, unitOfWork, "1", 0, "E", null, 0, null, empresa);

                        await unitOfWork.CommitChangesAsync(cancellationToken);

                        if (cte.Status == "E")
                        {
                            if (!servicoCTe.Emitir(ref cte, unitOfWork))
                                throw new ServicoException("O CT-e " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.");

                            if (!servicoCTe.AdicionarCTeNaFilaDeConsulta(cte, unitOfWork))
                                throw new ServicoException("O CT-e " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.");

                            integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                            await repositorioIntegracaoCTe.AtualizarAsync(integracaoCTe);
                        }
                    }
                    else
                    {
                        integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                        await repositorioIntegracaoCTe.AtualizarAsync(integracaoCTe);
                    }

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(integracaoCTe.Codigo);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao gerar CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex, "GerarCTeIntegracao");
                    await unitOfWork.RollbackAsync(cancellationToken);

                    servicoOrquestradorFila.RegistroComFalha(integracaoCTe.Codigo, ex.Message);
                }

                Servicos.Log.GravarInfo($"Finalizou geração do CTe {integracaoCTe.CTe.Codigo}", "GerarCTeIntegracao");
            }
        }

        private void AjustarDadosCTesInvalidos(Dominio.ObjetosDeValor.CTe.CTe documentoCTe)
        {
            if ((documentoCTe.Destinatario?.Exportacao ?? false) && (documentoCTe.Recebedor?.Exportacao ?? false) && documentoCTe.Recebedor.CPFCNPJ == "99999999999999" && string.IsNullOrWhiteSpace(documentoCTe.Recebedor.RazaoSocial))
                documentoCTe.Recebedor = null;
        }

        private void ObterDadosSeguroCTe(Dominio.ObjetosDeValor.CTe.CTe documentoCTe, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            if (documentoCTe.Seguros?.Count > 0)
                return;

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesTransportador = new Servicos.Averbacao(unitOfWork).ObterApolicesSeguroTransportador(empresa, unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro = new Servicos.Embarcador.Carga.CTe(unitOfWork).ConverterApolicesSeguroEmSeguroCTe(apolicesTransportador, documentoCTe.TipoTomador, documentoCTe.ValorTotalMercadoria);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Seguro apolice in apolicesSeguro)
            {
                Dominio.ObjetosDeValor.CTe.Seguro seguro = new Dominio.ObjetosDeValor.CTe.Seguro()
                {
                    CNPJSeguradora = apolice.CNPJSeguradora,
                    NomeSeguradora = apolice.Seguradora,
                    NumeroAverbacao = apolice.Averbacao,
                    Tipo = apolice.ResponsavelSeguro,
                    NumeroApolice = apolice.Apolice,
                    Valor = apolice.Valor
                };

                if (documentoCTe.Seguros == null)
                    documentoCTe.Seguros = new List<Dominio.ObjetosDeValor.CTe.Seguro>();

                documentoCTe.Seguros.Add(seguro);
            }
        }

        #endregion Métodos Protegidos
    }
}