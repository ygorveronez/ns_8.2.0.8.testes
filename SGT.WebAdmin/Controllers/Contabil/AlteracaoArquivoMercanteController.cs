using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/AlteracaoArquivoMercante", "Cargas/Carga")]
    public class AlteracaoArquivoMercanteController : BaseController
    {
        #region Construtores

        public AlteracaoArquivoMercanteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarConhecimentosConferencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string numeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo");
                string numeroManifesto = Request.GetStringParam("NumeroManifesto");
                string numeroCE = Request.GetStringParam("NumeroCE");

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                if (cte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaCTe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro na carga.");

                var filtro = new Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante()
                {
                    CodigoCTe = codigo
                };

                Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante dadoCTe = repCTe.BuscarPorCTe(filtro);

                if (dadoCTe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    dadoCTe.Codigo,
                    dadoCTe.Viagem,
                    dadoCTe.TerminalOrigem,
                    dadoCTe.TerminalDestino,
                    dadoCTe.NumeroBooking,
                    dadoCTe.NumeroControle,
                    dadoCTe.Container,
                    dadoCTe.NumeroCTe,
                    cte.NumeroManifesto,
                    NumeroCE = cte.NumeroCEMercante,
                    cte.NumeroManifestoTransbordo,
                    dadoCTe.PossuiTransbordo,
                    dadoCTe.StatusCTe,
                    dadoCTe.NavioTransbordo,
                    dadoCTe.PortoTransbordo,
                    TodosCTesComManifesto = !repCargaCTe.TodosCTesNaoContemManifesto(cargaCTe.Carga.Codigo),
                    TodosCTesComMercante = !repCargaCTe.TodosCTesNaoContemMercante(cargaCTe.Carga.Codigo)
                };


                if (!string.IsNullOrWhiteSpace(numeroManifestoTransbordo) && numeroManifestoTransbordo.Trim().Length != 13 && cte.NumeroManifestoTransbordo != numeroManifestoTransbordo)
                    return new JsonpResult(retorno, false, "O número de Manifesto deve conter 13 caracteres.");

                if (!string.IsNullOrWhiteSpace(numeroManifesto) && numeroManifesto.Trim().Length != 13 && cte.NumeroManifesto != numeroManifesto)
                    return new JsonpResult(retorno, false, "O número de Manifesto deve conter 13 caracteres.");

                if (!string.IsNullOrWhiteSpace(numeroCE) && numeroCE.Trim().Length != 15 && cte.NumeroCEMercante != numeroCE)
                    return new JsonpResult(retorno, false, "O número de CE deve conter 15 caracteres.");

                if (integracaoIntercab?.AtivarIntegracaoMercante ?? false)
                {
                    if (repCargaCTe.CargaRecebidaPorIntegracao(cte.Codigo))
                        return new JsonpResult(retorno, false, "Não é possível realizar esta operação para CT-e recebido pela integração.");
                }

                if (cargaCTe.Carga.CargaRecebidaDeIntegracao != true && (numeroManifestoTransbordo != cte.NumeroManifestoTransbordo || numeroManifesto != cte.NumeroManifesto || numeroCE != cte.NumeroCEMercante) && (integracaoIntercab?.AtivarIntegracaoMercante ?? false))
                    CriarRegistroIntegracaoParaDadosCTEModificados(unitOfWork, cte);

                if (!string.IsNullOrWhiteSpace(numeroManifestoTransbordo))
                    cte.NumeroManifestoTransbordo = numeroManifestoTransbordo;
                cte.NumeroManifesto = numeroManifesto;
                cte.NumeroCEMercante = numeroCE;

                repCTe.Atualizar(cte, Auditado);

                serCargaDadosSumarizados.AtualizarDadosMercanteManifesto(cte.Codigo, unitOfWork);

                retorno = new
                {
                    dadoCTe.Codigo,
                    dadoCTe.Viagem,
                    dadoCTe.TerminalOrigem,
                    dadoCTe.TerminalDestino,
                    dadoCTe.NumeroBooking,
                    dadoCTe.NumeroControle,
                    dadoCTe.Container,
                    dadoCTe.NumeroCTe,
                    cte.NumeroManifesto,
                    NumeroCE = cte.NumeroCEMercante,
                    cte.NumeroManifestoTransbordo,
                    dadoCTe.PossuiTransbordo,
                    dadoCTe.StatusCTe,
                    dadoCTe.NavioTransbordo,
                    dadoCTe.PortoTransbordo,
                    TodosCTesComManifesto = !repCargaCTe.TodosCTesNaoContemManifesto(cargaCTe.Carga.Codigo),
                    TodosCTesComMercante = !repCargaCTe.TodosCTesNaoContemMercante(cargaCTe.Carga.Codigo)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o cte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarConhecimentosInformacaoManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoViagem = Request.GetIntParam("Viagem");
                string numeroControle = Request.GetStringParam("NumeroControle");
                string numeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo");
                string numeroManifesto = Request.GetStringParam("NumeroManifesto");
                string numeroCE = Request.GetStringParam("NumeroCE");

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.ConsultaParaArquivoMercante(codigoTerminalOrigem, codigoViagem, numeroControle);

                if (!string.IsNullOrWhiteSpace(numeroManifestoTransbordo) && numeroManifestoTransbordo.Trim().Length != 13)
                    return new JsonpResult(false, true, "O número de Manifesto deve conter 13 caracteres.");

                if (!string.IsNullOrWhiteSpace(numeroManifesto) && numeroManifesto.Trim().Length != 13)
                    return new JsonpResult(false, true, "O número de Manifesto deve conter 13 caracteres.");

                if (!string.IsNullOrWhiteSpace(numeroCE) && numeroCE.Trim().Length != 15)
                    return new JsonpResult(false, true, "O número de CE deve conter 15 caracteres.");

                if (ctes.Count == 0)
                    return new JsonpResult(false, true, "Nenhum registro foi encontrado para alteração.");

                unitOfWork.Start();

                int count = 0;
                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    if (integracaoIntercab?.AtivarIntegracaoMercante ?? false)
                    {
                        if (repCargaCTe.CargaRecebidaPorIntegracao(cte.Codigo))
                            return new JsonpResult(false, true, "Não é possível realizar esta operação para CT-e recebido pela integração.");
                    }

                    cte.Initialize();

                    if (cte.CTeImportadoEmbarcador != true && (numeroManifestoTransbordo != cte.NumeroManifestoTransbordo || numeroManifesto != cte.NumeroManifesto || numeroCE != cte.NumeroCEMercante) && (integracaoIntercab?.AtivarIntegracaoMercante ?? false))
                        CriarRegistroIntegracaoParaDadosCTEModificados(unitOfWork, cte);

                    if (!string.IsNullOrWhiteSpace(numeroManifestoTransbordo))
                        cte.NumeroManifestoTransbordo = numeroManifestoTransbordo;
                    cte.NumeroManifesto = numeroManifesto;
                    cte.NumeroCEMercante = numeroCE;

                    repCTe.Atualizar(cte, Auditado);

                    serCargaDadosSumarizados.AtualizarDadosMercanteManifesto(cte.Codigo, unitOfWork);

                    count++;
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, count);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os ctes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarConhecimentosReplicarNumero()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoViagem = Request.GetIntParam("Viagem");
                string numeroControleAntigo = Request.GetStringParam("NumeroControleAntigo");
                string numeroControleNovo = Request.GetStringParam("NumeroControleNovo");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo = repCTe.ConsultaParaArquivoMercanteUnica(codigoTerminalOrigem, codigoViagem, numeroControleAntigo);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo = repCTe.ConsultaParaArquivoMercanteUnica(codigoTerminalOrigem, codigoViagem, numeroControleNovo);

                if (cteAntigo == null)
                    return new JsonpResult(false, true, "Nenhum registro para o CT-e antigo foi encontrado.");

                if (cteNovo == null)
                    return new JsonpResult(false, true, "Nenhum registro para o CT-e novo foi encontrado.");

                if (integracaoIntercab?.AtivarIntegracaoMercante ?? false)
                {
                    if (repCargaCTe.CargaRecebidaPorIntegracao(cteNovo.Codigo))
                        return new JsonpResult(false, true, "Não é possível realizar esta operação para CT-e recebido pela integração.");
                }

                unitOfWork.Start();

                cteNovo.Initialize();

                if (cteAntigo.CTeImportadoEmbarcador != true && (cteAntigo.NumeroManifestoTransbordo != cteAntigo.NumeroManifestoTransbordo || cteAntigo.NumeroManifesto != cteAntigo.NumeroManifesto || cteAntigo.NumeroCEMercante != cteAntigo.NumeroCEMercante) && (integracaoIntercab?.AtivarIntegracaoMercante ?? false))
                    CriarRegistroIntegracaoParaDadosCTEModificados(unitOfWork, cteAntigo);

                if (!string.IsNullOrWhiteSpace(cteAntigo.NumeroManifestoTransbordo))
                    cteNovo.NumeroManifestoTransbordo = cteAntigo.NumeroManifestoTransbordo;
                cteNovo.NumeroCEMercante = cteAntigo.NumeroCEMercante;
                cteNovo.NumeroManifesto = cteAntigo.NumeroManifesto;
                repCTe.Atualizar(cteNovo, Auditado);

                serCargaDadosSumarizados.AtualizarDadosMercanteManifesto(cteNovo.Codigo, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, 1);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os ctes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoMercante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int empresaSelecionada = 0;
                int.TryParse(Request.Params("Empresa"), out empresaSelecionada);
                int codigoPedidoNavioViagem = 0;
                int.TryParse(Request.Params("PedidoNavioViagem"), out codigoPedidoNavioViagem);

                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

                int tipoCargaPerigosa = Request.GetIntParam("TipoCargaPerigosa");
                int tipoTransbordo = Request.GetIntParam("TipoTransbordo");

                bool comConhecimentosCancelados = Request.GetBoolParam("ComConhecimentosCancelados");

                bool manifestoPorBalsa = Request.GetBoolParam("ManifestoPorBalsa");
                int codigoBalsa = Request.GetIntParam("Balsa");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);

                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.ASCII);

                Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = repPorto.BuscarPorCodigo(codigoPortoOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = repTerminal.BuscarPorCodigo(codigoTerminalOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = repTerminal.BuscarPorCodigo(codigoTerminalDestino);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(empresaSelecionada);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoNavioViagem);
                Dominio.Entidades.Embarcador.Pedidos.Navio balsa = null;

                if (manifestoPorBalsa)
                    balsa = repNavio.BuscarPorCodigo(codigoBalsa);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> terminais = repTerminal.BuscarTodosCadastros(codigoTerminalDestino);

                foreach (var terminal in terminais)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoNavioCTe = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoTerminal = null;

                    if (tipoTransbordo == 0 || tipoTransbordo == 2)
                    {
                        cargaCTes = repCargaCTe.ConsultarAquaviarioMercante(codigoPedidoNavioViagem, terminalOrigem.Codigo, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, comConhecimentosCancelados, tipoTransbordo == 2, balsa?.Codigo ?? 0);
                        cargaCTesTransbordo = repCargaCTe.ConsultarAquaviarioMercanteTransbordo(codigoPedidoNavioViagem, terminalOrigem.Codigo, 0, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminal.Codigo, 0, comConhecimentosCancelados, tipoTransbordo == 2, codigoBalsa: balsa?.Codigo ?? 0);
                    }
                    else
                        cargaCTesTransbordoNavioCTe = repCargaCTe.ConsultarAquaviarioMercanteTransbordo(0, 0, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminalOrigem.Codigo, codigoPedidoNavioViagem, comConhecimentosCancelados, false, codigoBalsa: balsa?.Codigo ?? 0);

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                    if (cargaCTes != null && cargaCTes.Count > 0)
                        ctes.AddRange(cargaCTes.Select(o => o.CTe).Distinct().ToList());
                    if (cargaCTesTransbordo != null && cargaCTesTransbordo.Count > 0)
                        ctes.AddRange(cargaCTesTransbordo.Select(o => o.CTe).Distinct().ToList());
                    if (cargaCTesTransbordoNavioCTe != null && cargaCTesTransbordoNavioCTe.Count > 0)
                        ctes.AddRange(cargaCTesTransbordoNavioCTe.Select(o => o.CTe).Distinct().ToList());
                    if (cargaCTesTransbordoTerminal != null && cargaCTesTransbordoTerminal.Count > 0)
                        ctes.AddRange(cargaCTesTransbordoTerminal.Select(o => o.CTe).Distinct().ToList());

                    if (ctes != null && ctes.Count > 0)
                        ctes = ctes.Distinct().ToList();

                    if (tipoTransbordo == 0 || tipoTransbordo == 2)
                        ProcessarDadosArquivoMercante(unitOfWork, ref x, ctes, empresa, portoOrigem, terminal.Porto, viagem, terminalOrigem, terminal, false, balsa);
                    else
                    {
                        int qtdConhecimentos = ctes.Where(c => c.NumeroManifesto != null && c.NumeroManifesto != "").Distinct().Count();
                        List<string> numerosManifestos = ctes.Select(c => c.NumeroManifesto).Distinct().ToList();
                        bool gerarM4 = true;
                        if (numerosManifestos != null && numerosManifestos.Count > 0)
                        {
                            foreach (var numeroManifesto in numerosManifestos)
                            {
                                if (!string.IsNullOrWhiteSpace(numeroManifesto))
                                {
                                    ProcessarDadosArquivoBaldeacao(unitOfWork, ref x, ctes, empresa, portoOrigem, terminal.Porto, viagem, terminalOrigem, terminal, numeroManifesto, gerarM4, qtdConhecimentos, balsa);
                                    gerarM4 = false;
                                }
                            }
                        }
                    }
                }
                x.Flush();

                if (arquivoINPUT.Length == 0)
                    return new JsonpResult(false, false, "Não foram encontrados dados com os filtros informados");

                Random random = new Random();
                return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("H6DDUSIC.", random.Next(100, 999).ToString()));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Mercante.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarArquivoMercante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
                if (files.Count > 0)
                {
                    //unitOfWork.Start();

                    string msgRetorno = "";
                    Servicos.DTO.CustomFile file = files[0];
                    StreamReader streamReader = new StreamReader(file.InputStream);

                    string caminhoArquivo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Mercante" });
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    
                    string arquivoProcessar = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidArquivo + System.IO.Path.GetExtension(file.FileName));
                    file.SaveAs(arquivoProcessar);

                    if (!ProcessarRetornoArquivoMercante(out msgRetorno, streamReader, unitOfWork, Auditado, file.FileName, arquivoProcessar))
                    {
                        //streamReader.DiscardBufferedData();
                        if (!ProcessarRetornoArquivoMercanteTransbordo(out msgRetorno, streamReader, unitOfWork, Auditado, file.FileName, arquivoProcessar))
                            return new JsonpResult(false, msgRetorno);
                    }

                    //unitOfWork.CommitChanges();

                    return new JsonpResult(true, "Importação do retorno do mercante foi realizada com sucesso.");
                }
                else
                {
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
                }
            }
            catch (Exception ex)
            {
                //unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo do mercante. Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarDocumentacaoMercantePendente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int empresaSelecionada = 0;
                int.TryParse(Request.Params("Empresa"), out empresaSelecionada);
                int codigoPedidoNavioViagem = 0;
                int.TryParse(Request.Params("PedidoNavioViagem"), out codigoPedidoNavioViagem);

                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

                int tipoCargaPerigosa = Request.GetIntParam("TipoCargaPerigosa");
                int tipoTransbordo = Request.GetIntParam("TipoTransbordo");

                bool comConhecimentosCancelados = Request.GetBoolParam("ComConhecimentosCancelados");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = repPorto.BuscarPorCodigo(codigoPortoOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = repTerminal.BuscarPorCodigo(codigoTerminalOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = repTerminal.BuscarPorCodigo(codigoTerminalDestino);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(empresaSelecionada);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoNavioViagem);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> terminais = repTerminal.BuscarTodosCadastros(codigoTerminalDestino);

                string retornoBookings = "";
                foreach (var terminal in terminais)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctes = null;
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasTransbordo = null;
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasTransbordoNavioCTe = null;
                    if (tipoTransbordo == 0 || tipoTransbordo == 2)
                    {
                        cargas = repCargaCTe.ConsultarAquaviarioMercantePendenteAutorizacao(codigoPedidoNavioViagem, terminalOrigem.Codigo, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2);
                        cargasTransbordo = repCargaCTe.ConsultarAquaviarioMercanteTransbordoPendenteAutorizacao(codigoPedidoNavioViagem, terminalOrigem.Codigo, 0, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminal.Codigo, 0);
                    }
                    else
                    {
                        cargasTransbordoNavioCTe = repCargaCTe.ConsultarAquaviarioMercanteTransbordoPendenteAutorizacao(0, 0, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminalOrigem.Codigo, codigoPedidoNavioViagem);
                        ctes = repCargaCTe.ConsultarCTesSemNumeroManifesto(0, 0, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminalOrigem.Codigo, codigoPedidoNavioViagem, comConhecimentosCancelados);
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPendentes = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                    if (cargas != null && cargas.Count > 0)
                        cargasPendentes.AddRange(cargas.Distinct().ToList());
                    if (cargasTransbordo != null && cargasTransbordo.Count > 0)
                        cargas.AddRange(cargasTransbordo.Distinct().ToList());
                    if (cargasTransbordoNavioCTe != null && cargasTransbordoNavioCTe.Count > 0)
                        cargas.AddRange(cargasTransbordoNavioCTe.Distinct().ToList());

                    if (cargas != null && cargas.Count > 0)
                    {
                        cargas = cargas.Distinct().ToList();
                        foreach (var carga in cargas)
                        {
                            retornoBookings += "Carga: " + carga.CodigoCargaEmbarcador + " -> Carga pendêntes de emissão para esta geração do arquivo Mercante, favor verifique no relatório de cargas/conhecimento.<br/>";
                        }
                    }
                    if (ctes != null && ctes.Count > 0)
                    {
                        ctes = ctes.Distinct().ToList();
                        foreach (var cte in ctes)
                        {
                            retornoBookings += "CT-e: " + cte.CTe.NumeroControle + " POL: " + cte.CTe.PortoOrigem?.Descricao + " POD: " + cte.CTe.PortoDestino?.Descricao + " VVD: " + cte.CTe.Viagem?.Descricao + " -> Não possui o número do manifesto importado anteriormente.<br/>";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(retornoBookings))
                    return new JsonpResult(false, true, retornoBookings);
                else
                    return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Mercante.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaImportacaoArquivoMercante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string tipoArquivo = Request.GetStringParam("TipoArquivo");
                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoPortoDestino = Request.GetIntParam("PortoDestino");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("StatusArquivo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("N/V/D", "PedidoViagemNavio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Porto Destino", "PortoDestino", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Arquivo", "NomeArquivo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Absorção", "DataAbsorcao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Status", "StatusRetorno", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Arquivo", "TipoArquivo", 10, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unitOfWork);

                List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante> listaArquivo = repArquivoMercante.Consulta(codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoArquivo, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repArquivoMercante.ContarConsultar(codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoArquivo, situacao));

                var lista = (from p in listaArquivo
                             select new
                             {
                                 p.Codigo,
                                 PedidoViagemNavio = p.PedidoViagemNavio?.Descricao ?? "",
                                 PortoOrigem = p.PortoOrigem?.Descricao ?? "",
                                 PortoDestino = p.PortoDestino?.Descricao ?? "",
                                 p.NomeArquivo,
                                 DataAbsorcao = p.DataAbsorcao.HasValue ? p.DataAbsorcao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 p.StatusRetorno,
                                 p.TipoArquivo
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.ArquivoMercante arquivoMercante = repArquivoMercante.BuscarPorCodigo(codigo);

                if ((arquivoMercante == null) || string.IsNullOrWhiteSpace(arquivoMercante.CaminhoArquivo) || !Utilidades.IO.FileStorageService.Storage.Exists(arquivoMercante.CaminhoArquivo))
                    return new JsonpResult(false, true, "Arquivo não encontrado.");

                byte[] xmlBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoMercante.CaminhoArquivo);

                return Arquivo(xmlBinario, "text/txt", $"{arquivoMercante.NomeArquivo}");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string emailEnvio = Request.GetStringParam("EmailEnvio");
                if (string.IsNullOrWhiteSpace(emailEnvio))
                    return new JsonpResult(false, "Favor informe o e-mail para envio.");

                List<long> codigos = new List<long>();
                List<string> emails = new List<string>();
                codigos = RetornaCodigosArquivos(unitOfWork);

                if (codigos == null || codigos.Count == 0)
                    return new JsonpResult(false, "Nenhum arquivo selecionado, favor verifique os filtros realizados.");

                Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");

                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();

                foreach (var codigo in codigos)
                {
                    Dominio.Entidades.Embarcador.Documentos.ArquivoMercante arquivoMercante = repArquivoMercante.BuscarPorCodigo(codigo);
                    if (arquivoMercante != null && !string.IsNullOrWhiteSpace(arquivoMercante.CaminhoArquivo) && Utilidades.IO.FileStorageService.Storage.Exists(arquivoMercante.CaminhoArquivo))
                        attachments.Add(new System.Net.Mail.Attachment(arquivoMercante.CaminhoArquivo));
                }

                emails.AddRange(emailEnvio.Split(';').ToList());

                if (emails == null || emails.Count == 0)
                    return new JsonpResult(false, "Nenhum e-mail informado.");

                if (attachments == null || attachments.Count == 0)
                    return new JsonpResult(false, "Nenhum arquivo selecionado, favor verifique os filtros realizados.");

                string assunto = "Envio Arquivo Mercante";
                string mensagem = "Segue em anexo o(s) arquivo(s) mercante solicitado";

                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, null, emails.ToArray(), assunto, mensagem, email.Smtp, out string mensagemErro, email.DisplayEmail,
                   attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, 0);

                if (sucesso)
                    return new JsonpResult(true, "Sucesso");
                else
                    return new JsonpResult(false, "Problemas no envio do e-mail: " + mensagemErro);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha processar o envio de e-mail dos arquivos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadLoteArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<long> codigos = new List<long>();
                List<string> emails = new List<string>();
                codigos = RetornaCodigosArquivos(unitOfWork);

                if (codigos == null || codigos.Count == 0)
                    return new JsonpResult(false, "Nenhum arquivo selecionado, favor verifique os filtros realizados.");

                Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unitOfWork);

                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

                foreach (var codigo in codigos)
                {
                    Dominio.Entidades.Embarcador.Documentos.ArquivoMercante arquivoMercante = repArquivoMercante.BuscarPorCodigo(codigo);
                    if (arquivoMercante != null && !string.IsNullOrWhiteSpace(arquivoMercante.CaminhoArquivo) && Utilidades.IO.FileStorageService.Storage.Exists(arquivoMercante.CaminhoArquivo))
                        conteudoCompactar.Add(arquivoMercante.NomeArquivo, Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoMercante.CaminhoArquivo));
                }

                if (conteudoCompactar == null || conteudoCompactar.Count == 0)
                    return new JsonpResult(false, "Nenhum arquivo selecionado, favor verifique os filtros realizados.");

                string caminhoArquivo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Mercante" });
                string guidXML = (Guid.NewGuid().ToString());
                MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                string caminhoArquivoCompactado = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidXML + ".rar");
                using (Stream fs = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoArquivoCompactado))
                {
                    arquivoCompactado.CopyTo(fs);
                    fs.Flush();
                }

                byte[] xmlBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivoCompactado);

                return Arquivo(xmlBinario, "text/xml", $"ArquivoCompactado.rar");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar lote do arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.ArquivoMercanteErro repArquivoMercanteErro = new Repositorio.Embarcador.Documentos.ArquivoMercanteErro(unitOfWork);
                List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro> erros = repArquivoMercanteErro.BuscarPorArquivo(codigo);

                if (erros == null || erros.Count == 0)
                {
                    var retorno = new
                    {
                        Retorno = "Nenhum erro localizado"
                    };
                    return new JsonpResult(retorno, true, "Sucesso");
                }
                else
                {
                    string strErro = "";
                    foreach (var erro in erros)
                        strErro += erro.NumeroControle + ": " + (!string.IsNullOrWhiteSpace(erro.CodigoErro) ? erro.CodigoErro : "OK") + " " + erro.DescricaoErro + "<br/>";
                    var retorno = new
                    {
                        Retorno = strErro
                    };
                    return new JsonpResult(retorno, true, "Sucesso");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os detalhes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        #endregion

        #region Métodos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho, bool exportacao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.EditableCell editableNumeroManifesto = new Models.Grid.EditableCell(TipoColunaGrid.aString, 200);
            Models.Grid.EditableCell editableNumeroCE = new Models.Grid.EditableCell(TipoColunaGrid.aString, 200);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Viagem", "Viagem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Terminal Destino", "TerminalDestino", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Número Booking", "NumeroBooking", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Número Controle", "NumeroControle", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Container", "Container", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Balsa", "Balsa", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Número CTe", "NumeroCTe", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Número Manifesto", "NumeroManifesto", 10, Models.Grid.Align.left, true, false, false, false, true, editableNumeroManifesto);
            grid.AdicionarCabecalho("Número CE", "NumeroCE", 10, Models.Grid.Align.left, true, false, false, false, true, editableNumeroCE);
            grid.AdicionarCabecalho("Nº Man. Transbordo", "NumeroManifestoTransbordo", 10, Models.Grid.Align.left, true, false, false, false, true, editableNumeroManifesto);
            grid.AdicionarCabecalho("Navio Transbordo", "NavioTransbordo", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Possui Transbordo", "PossuiTransbordo", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status", "StatusCTe", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Porto Transbordo", "PortoTransbordo", 5, Models.Grid.Align.left, false);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            int countCTes = repCTe.ContarConsultaConhecimentosAlteracaoArquivoMercante(filtrosPesquisa);

            IList<Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante> listaCTes = repCTe.ConsultarConhecimentosAlteracaoArquivoMercante(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(countCTes);

            var retorno = (from obj in listaCTes
                           select new
                           {
                               obj.Codigo,
                               obj.Viagem,
                               obj.TerminalOrigem,
                               obj.TerminalDestino,
                               obj.NumeroBooking,
                               obj.NumeroControle,
                               obj.Container,
                               obj.Balsa,
                               obj.NumeroCTe,
                               obj.NumeroManifesto,
                               obj.NumeroCE,
                               obj.NumeroManifestoTransbordo,
                               obj.PossuiTransbordo,
                               obj.StatusCTe,
                               obj.NavioTransbordo,
                               obj.PortoTransbordo
                           }).ToList();

            grid.AdicionaRows(retorno);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante()
            {
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                CodigoContainer = Request.GetIntParam("Container"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroCE = Request.GetStringParam("NumeroCE"),
                NumeroManifesto = Request.GetStringParam("NumeroManifesto"),
                NumeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo"),
                PossuiTransbordo = Request.GetEnumParam<OpcaoSimNaoPesquisa>("PossuiTransbordo"),
                Manifesto = Request.GetEnumParam<GeradoPendente>("Manifesto"),
                ManifestoTransbordo = Request.GetEnumParam<GeradoPendente>("ManifestoTransbordo"),
                CE = Request.GetEnumParam<GeradoPendente>("CE"),
                CodigoNavioTransbordo = Request.GetIntParam("NavioTransbordo"),
                StatusCTe = Request.GetListParam<string>("Status"),
                TipoCTe = Request.GetListEnumParam<TipoCTE>("TipoCTe"),
                CodigoPortoTransbordo = Request.GetIntParam("PortoTransbordo"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoBalsa = Request.GetIntParam("Balsa")
            };
        }

        private void ProcessarDadosArquivoBaldeacao(Repositorio.UnitOfWork unitOfWork, ref StreamWriter x, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino, Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino, string numeroManifesto, bool gerarM4, int qtdConhecimentos, Dominio.Entidades.Embarcador.Pedidos.Navio balsa)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            ctes = ctes.Where(c => c.NumeroManifesto == numeroManifesto).ToList();

            if (ctes.Count() > 0)
            {
                if (gerarM4)
                {
                    string linhaM4 = "M4"; //TIPO
                    linhaM4 += "I3";//IND-FUNÇÃO-CARGA
                    linhaM4 += Utilidades.String.Right(numeroManifesto.PadRight(13, ' '), 13);//NRO-MANIFESTO
                    linhaM4 += (qtdConhecimentos.ToString("D")).PadLeft(4, '0');//QTDE-CE
                    linhaM4 += "0".PadLeft(4, '0');//QTDE-CONT-VZ
                    linhaM4 += empresa.CNPJ_SemFormato;//COD-EMP-NAV
                    linhaM4 += empresa.CNPJ_SemFormato;//COD-AGEN-NAV
                    linhaM4 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-ENC-MANIF
                    linhaM4 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-DESCARGA

                    if (!string.IsNullOrWhiteSpace(portoOrigem.CodigoMercante))
                        linhaM4 += Utilidades.String.Right((portoOrigem.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-ORIG
                    else
                        linhaM4 += Utilidades.String.Right(("BR" + portoOrigem.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-ORIG                

                    if (!string.IsNullOrWhiteSpace(portoDestino.CodigoMercante))
                        linhaM4 += Utilidades.String.Right((portoDestino.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-DEST
                    else
                        linhaM4 += Utilidades.String.Right(("BR" + portoDestino.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-DEST

                    linhaM4 += (viagem.NumeroViagem.ToString("D").PadLeft(3, '0') + DirecaoViagemMultimodalHelper.ObterAbreviacao(viagem.DirecaoViagemMultimodal)).PadRight(10, ' ');//NR-VIAGEM
                    linhaM4 += Utilidades.String.Right((balsa is null ? viagem.Navio.CodigoIMO : balsa?.CodigoIMO)?.PadRight(10, ' '), 10);//COD-IMO

                    if (!string.IsNullOrWhiteSpace(terminalOrigem.CodigoMercante))
                        linhaM4 += Utilidades.String.Right(terminalOrigem.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-CARR
                    else
                        linhaM4 += Utilidades.String.Right(terminalOrigem.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-CARR

                    linhaM4 += " ".PadLeft(32, ' ');//espaços

                    if (!string.IsNullOrWhiteSpace(terminalDestino.CodigoMercante))
                        linhaM4 += Utilidades.String.Right(terminalDestino.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-DESC
                    else
                        linhaM4 += Utilidades.String.Right(terminalDestino.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-DESC

                    x.WriteLine(linhaM4);
                }

                foreach (var cte in ctes)
                {
                    string linhaC4 = "C4";
                    linhaC4 += Utilidades.String.Right(cte.NumeroCEMercante?.PadRight(18, ' '), 18);//NUMERO-CE

                    if (!string.IsNullOrWhiteSpace(terminalOrigem?.CodigoMercante))
                        linhaC4 += (terminalOrigem?.CodigoMercante ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC4 += (terminalOrigem?.CodigoTerminal ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG

                    if (!string.IsNullOrWhiteSpace(terminalDestino?.CodigoMercante))
                        linhaC4 += (terminalDestino?.CodigoMercante ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC4 += (terminalDestino?.CodigoTerminal ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG

                    linhaC4 += " ".PadLeft(14, ' ');//cte.ValorFrete.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-FRETE-TOTAL-BALDEACAO
                    linhaC4 += "X".PadLeft(3, ' ');

                    x.WriteLine(linhaC4);
                }
            }
        }

        private void ProcessarDadosArquivoMercante(Repositorio.UnitOfWork unitOfWork, ref StreamWriter x, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino, Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino, bool transbordo, Dominio.Entidades.Embarcador.Pedidos.Navio balsa)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.InformacaoCargaCTE informacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Servicos.Cliente servicoCliente = new Servicos.Cliente(unitOfWork.StringConexao);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if (ctes.Count() > 0)
            {
                string linhaM3 = "M3"; //TIPO
                linhaM3 += (ctes.Count().ToString("D")).PadLeft(4, '0');//QTDE-CE
                linhaM3 += "0".PadLeft(4, '0');//QTDE-CONT-VZ
                linhaM3 += empresa.CNPJ_SemFormato;//COD-EMP-NAV
                linhaM3 += empresa.CNPJ_SemFormato;//COD-AGEN-NAV
                linhaM3 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-ENC-MANIF
                linhaM3 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-DESCARGA

                if (!string.IsNullOrWhiteSpace(portoOrigem.CodigoMercante))
                    linhaM3 += Utilidades.String.Right((portoOrigem.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-ORIG
                else
                    linhaM3 += Utilidades.String.Right(("BR" + portoOrigem.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-ORIG

                if (!string.IsNullOrWhiteSpace(portoDestino.CodigoMercante))
                    linhaM3 += Utilidades.String.Right((portoDestino.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-DEST
                else
                    linhaM3 += Utilidades.String.Right(("BR" + portoDestino.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-DEST
                linhaM3 += (viagem.NumeroViagem.ToString("D").PadLeft(3, '0') + DirecaoViagemMultimodalHelper.ObterAbreviacao(viagem.DirecaoViagemMultimodal)).PadRight(10, ' ');//NR-VIAGEM
                linhaM3 += Utilidades.String.Right((balsa is null ? viagem.Navio.CodigoIMO : balsa.CodigoIMO)?.PadRight(10, ' '), 10);//COD-IMO

                if (!string.IsNullOrWhiteSpace(terminalOrigem.CodigoMercante))
                    linhaM3 += Utilidades.String.Right(terminalOrigem.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-CARR
                else
                    linhaM3 += Utilidades.String.Right(terminalOrigem.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-CARR

                linhaM3 += " ".PadLeft(32, ' ');//espaços

                if (!string.IsNullOrWhiteSpace(terminalDestino.CodigoMercante))
                    linhaM3 += Utilidades.String.Right(terminalDestino.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-DESC
                else
                    linhaM3 += Utilidades.String.Right(terminalDestino.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-DESC

                x.WriteLine(linhaM3);

                foreach (var cte in ctes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repCargaCTe.BuscarPedidoPorCTe(cte.Codigo);
                    Dominio.Entidades.ParticipanteCTe tomador = null;
                    Dominio.Entidades.ParticipanteCTe remetente = null;
                    if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                    {
                        if (cte.DocumentosTransporteAnterior != null && cte.DocumentosTransporteAnterior.Count > 0)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior = repCTe.BuscarPorChave(cte.DocumentosTransporteAnterior.FirstOrDefault().Chave);
                            if (cteAnterior != null)
                            {
                                tomador = cteAnterior.TomadorPagador;
                                if (!string.IsNullOrWhiteSpace(cte.NumeroControle) && cte.NumeroControle.StartsWith("SVM"))
                                    remetente = cte.TomadorPagador;
                                else
                                    remetente = cteAnterior.Remetente;
                            }
                            else
                            {
                                tomador = cte.TomadorPagador;
                                remetente = cte.Remetente;
                            }
                        }
                        else
                        {
                            tomador = cte.TomadorPagador;
                            remetente = cte.Remetente;
                        }
                    }
                    else
                    {
                        tomador = cte.TomadorPagador;
                        remetente = cte.Remetente;
                    }

                    if (tomador == null)
                        tomador = cte.TomadorPagador;

                    if (tomador != null && tomador.Cliente != null && tomador.Cliente.GrupoPessoas != null && cte.PortoOrigem != null && cte.PortoDestino != null && tomador.Cliente.GrupoPessoas.AdicionarDespachanteComoConsignatario && tomador.Cliente.GrupoPessoas.Despachante != null && cte.PortoDestino.AtivarDespachanteComoConsignatario && cte.PortoOrigem.AtivarDespachanteComoConsignatario)
                        tomador = servicoCliente.ConverterClienteParaParticipanteCTe(tomador.Cliente.GrupoPessoas.Despachante);
                    else if (configuracaoFinanceiro != null && configuracaoFinanceiro.Despachante != null && cte.PortoOrigem != null && cte.PortoDestino != null && (cte.PortoDestino.AtivarDespachanteComoConsignatario || cte.PortoOrigem.AtivarDespachanteComoConsignatario))
                        tomador = servicoCliente.ConverterClienteParaParticipanteCTe(configuracaoFinanceiro.Despachante);

                    string linhaC3 = "C3";
                    linhaC3 += (cte.XMLNotaFiscais.Count().ToString("D").PadLeft(4, '0'));//QTDE-NF
                    linhaC3 += (cte.Containers.Count().ToString("D").PadLeft(4, '0'));//QTDE-NF
                    linhaC3 += Utilidades.String.Right(cte.NumeroControle.PadRight(18, ' '), 18);//NUMERO-CE
                    linhaC3 += "N";//ND-BL-A-ORDEM
                    linhaC3 += (tomador?.Cliente?.Tipo == "E") ? "S" : "N";//IND-CONSIG-ESTR

                    linhaC3 += Utilidades.String.Right(Utilidades.String.RemoveAllSpecialCharacters((tomador.Nome + ", " + tomador.Numero + " " + tomador.Endereco + " " + (tomador.Localidade?.DescricaoCidadeEstado ?? tomador.Cidade) + " " + tomador.Bairro)).PadRight(253, ' '), 253);//IDENT-CONSIGNAT
                    //linhaC3 += Utilidades.String.Right((cte.TomadorPagador.Nome + ", " + cte.TomadorPagador.Numero + " " + cte.TomadorPagador.Endereco + " " + (cte.TomadorPagador.Localidade?.DescricaoCidadeEstado ?? cte.TomadorPagador.Cidade) + " " + cte.TomadorPagador.Bairro).PadRight(253, ' '), 253);//IDENT-CONSIGNAT

                    linhaC3 += " ".PadRight(30, ' ');//NR-PASS-ESTR
                    linhaC3 += Utilidades.String.Right(tomador.Cliente?.Tipo == "E" ? tomador.Nome.PadRight(55, ' ') : " ".PadRight(55, ' '), 55);//NOM-CONSIG-ESTR
                    linhaC3 += (!string.IsNullOrWhiteSpace(tomador.CPF_CNPJ) ? tomador.CPF_CNPJ_SemFormato : "").PadRight(14, ' ');//CNPJ-CPF-CONSIG
                    linhaC3 += (!string.IsNullOrWhiteSpace(remetente.CPF_CNPJ) ? remetente.CPF_CNPJ_SemFormato : "").PadRight(14, ' ');//CNPJ / CPF-EMBARC

                    linhaC3 += Utilidades.String.Right(Utilidades.String.RemoveAllSpecialCharacters((remetente.Nome + ", " + remetente.Numero + " " + remetente.Endereco + " " + (remetente.Localidade?.DescricaoCidadeEstado ?? "") + " " + remetente.Bairro)).PadRight(253, ' '), 253);//IDENT-EMBARC
                    linhaC3 += cte.DataEmissao.Value.ToString("yyyyMMdd");//DT-EMIS-CONHEC
                                                                          //linhaC3 += Utilidades.String.Right(((cte.Containers != null ? cte.Containers.Count().ToString("D") : "1") + " Container de " + (cte.Containers != null && cte.Containers.Count > 0 ? (cte.Containers.FirstOrDefault().Container?.ContainerTipo?.Descricao ?? "") : " ") + " p s dizendo conter" + cte.Volumes.ToString("D") + " Volumes de " + cte.ProdutoPredominanteCTe).PadRight(506, ' '), 506);//DESCR-MERC
                    linhaC3 += Utilidades.String.Right(cte.ProdutoPredominanteCTe.PadRight(506, ' '), 506);//DESCR-MERC
                    linhaC3 += Utilidades.String.Right(Utilidades.String.RemoveAllSpecialCharacters(cte.ObservacoesGerais).PadRight(253, ' '), 253);//OBSERVACOES
                    linhaC3 += cte.PesoCubado.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(13, '0');//CUBAGEM-M3

                    if (!string.IsNullOrWhiteSpace(cte.PortoOrigem?.CodigoMercante))
                        linhaC3 += (((cte.PortoOrigem?.CodigoMercante ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM
                    else
                        linhaC3 += (("BR" + (cte.PortoOrigem?.CodigoIATA ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM

                    if (!string.IsNullOrWhiteSpace(cte.PortoDestino?.CodigoMercante))
                        linhaC3 += (((cte.PortoDestino?.CodigoMercante ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM
                    else
                        linhaC3 += (("BR" + (cte.PortoDestino?.CodigoIATA ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM

                    linhaC3 += cte.ValorPrestacaoServico.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-FRETE-BASICO
                    linhaC3 += "790";//CD-MOEDA-FRETE
                    linhaC3 += "P";//CD-RECOLH-FRETE
                    linhaC3 += "HH";//MODAL-FRETE
                    linhaC3 += "N";//CATEGORIA-CARGA
                    linhaC3 += cte.LocalidadeTerminoPrestacao.Estado.Sigla;//UF-DEST-CARGA

                    if (!string.IsNullOrWhiteSpace(terminalOrigem?.CodigoMercante))
                        linhaC3 += (terminalOrigem?.CodigoMercante ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC3 += (terminalOrigem?.CodigoTerminal ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG

                    if (!string.IsNullOrWhiteSpace(terminalDestino?.CodigoMercante))
                        linhaC3 += (terminalDestino?.CodigoMercante ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC3 += (terminalDestino?.CodigoTerminal ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG

                    linhaC3 += "N";//ND-BL-SERVICO
                    linhaC3 += "0".PadLeft(15, '0');//NUMERO-CE-MERCANTEORIGINAL
                    linhaC3 += " ".PadLeft(1657, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS

                    x.WriteLine(linhaC3);
                    List<string> ncms = new List<string>();
                    foreach (var nota in cte.XMLNotaFiscais)
                    {
                        string linhaN3 = "N3";
                        linhaN3 += Utilidades.String.Right(nota.Numero.ToString("D").PadRight(15, ' '), 15);//NR-NOTA-FISCAL

                        string serie = nota.SerieOuSerieDaChave;
                        if (string.IsNullOrWhiteSpace(serie))
                            serie = nota.Serie;
                        if (string.IsNullOrWhiteSpace(serie))
                            serie = "1";

                        linhaN3 += Utilidades.String.Right(serie.PadRight(10, ' '), 10);//NR-SERIE-NFISCAL
                        linhaN3 += nota.DataEmissao.ToString("yyyyMMdd");//DT-EMISSAO-NF
                        linhaN3 += (nota.Volumes > 0 ? nota.Volumes : 1m).ToString("n3").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-FRETE-BASICO
                        linhaN3 += nota.Emitente.CPF_CNPJ_SemFormato.PadRight(14, ' ');//CNPJ-EMIT-NF
                        linhaN3 += Utilidades.String.Right((string.IsNullOrWhiteSpace(nota.Emitente.IE_RG) ? "" : nota.Emitente.IE_RG).PadRight(14, ' '), 14);//INSC-EST-EMIT-NF

                        if (!string.IsNullOrWhiteSpace(nota.NCM) && !ncms.Contains(nota.NCM))
                            ncms.Add(nota.NCM.Trim());

                        x.WriteLine(linhaN3);
                    }

                    int sequencialContainer = 1;
                    foreach (var container in cte.Containers)
                    {
                        string taraContainer = repCTe.BuscarTaraContainer(container.Container.Codigo, cte.Codigo);
                        taraContainer = Utilidades.String.OnlyNumbers(taraContainer);

                        decimal tara = 0;
                        decimal.TryParse(taraContainer, out tara);

                        if (tara <= 0)
                            tara = container.Container?.Tara ?? 0;
                        if (tara <= 0)
                            tara = 2000;

                        decimal pesoContainer = 1;
                        decimal pesoCubado = 1;

                        pesoContainer = repCTe.BuscarPesoNotasConhecimento(container.Codigo, cte.Codigo);

                        if (pesoContainer <= 0)
                            pesoContainer = repCTe.BuscarPesoBrutoContainer(container.Codigo, cte.Codigo);

                        if (pesoContainer <= 0)
                            pesoContainer = 1;

                        pesoCubado = repCTe.BuscarPesoCubicoContainer(container.Codigo);
                        if (pesoCubado <= 0)
                            pesoCubado = 1;


                        string linhaI3 = "I3";
                        linhaI3 += "1";//TIPO-ÍTEM-CARGA
                        linhaI3 += sequencialContainer.ToString("D").PadLeft(4, '0');//R-ITEM
                        linhaI3 += pesoContainer.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(12, '0');//PESO-BRUTO-KG

                        //40 = 40G0
                        //20 = 22G0
                        if (container.Container.ContainerTipo == null)
                            linhaI3 += Utilidades.String.Right(("45G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("40HC"))
                            linhaI3 += Utilidades.String.Right(("45G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("40DC"))
                            linhaI3 += Utilidades.String.Right(("42G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("20TK"))
                            linhaI3 += Utilidades.String.Right(("22T0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("20DC"))
                            linhaI3 += Utilidades.String.Right(("22G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("20HC"))
                            linhaI3 += Utilidades.String.Right(("22G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("20OT"))
                            linhaI3 += Utilidades.String.Right(("22U1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("40OT"))
                            linhaI3 += Utilidades.String.Right(("42U1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("20FR"))
                            linhaI3 += Utilidades.String.Right(("22P1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("40FR"))
                            linhaI3 += Utilidades.String.Right(("42P1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("40FH"))
                            linhaI3 += Utilidades.String.Right(("42P1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("20RF"))
                            linhaI3 += Utilidades.String.Right(("22R1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("40RH"))
                            linhaI3 += Utilidades.String.Right(("45R1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Equals("40RF") || container.Container.ContainerTipo.Descricao.Equals("40RA") || container.Container.ContainerTipo.Descricao.Equals("40RE") || container.Container.ContainerTipo.Descricao.Equals("40RX") || container.Container.ContainerTipo.Descricao.Equals("40RC") || container.Container.ContainerTipo.Descricao.Equals("40HR"))
                            linhaI3 += Utilidades.String.Right(("42R1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Contains("40OH"))
                            linhaI3 += Utilidades.String.Right(("42U1").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Contains("40"))
                            linhaI3 += Utilidades.String.Right(("45G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Contains("20TK"))
                            linhaI3 += Utilidades.String.Right(("22T0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Contains("20"))
                            linhaI3 += Utilidades.String.Right(("22G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else
                            linhaI3 += Utilidades.String.Right((container.Container?.ContainerTipo?.Descricao ?? "").PadRight(4, ' '), 4);//TIPO-CONTEINER

                        linhaI3 += Utilidades.String.Right((container.Container?.Numero ?? "").PadRight(11, ' '), 11);//NR-CONTEINER
                        linhaI3 += tara.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(9, '0');//PB- TR-CONT
                        linhaI3 += "S";//N-PARCIAL-CONT
                        linhaI3 += " ".PadRight(2, ' ');//TIPO-EMBAL-SOLTA
                        linhaI3 += "0".PadLeft(7, '0');//QT-IT-CARG-SOLTA
                        linhaI3 += " ".PadRight(2, ' ');//CD-TIPO-GRANEL
                        linhaI3 += " ".PadRight(253, ' ');//TX-DESCR-GRANEL
                        linhaI3 += " ".PadRight(30, ' ');//NR-CHASSI-VEIC
                        linhaI3 += " ".PadRight(55, ' ');//NOME-MARCA
                        linhaI3 += " ".PadRight(55, ' ');//NOM-CONTRAMARC

                        linhaI3 += Utilidades.String.Right((pedido?.IMOUnidade ?? " ").PadLeft(6, ' '), 6);//" ".PadRight(6, ' ');//CD-MERC-PERIGO
                        linhaI3 += Utilidades.String.Right((pedido?.IMOClasse ?? " ").PadLeft(4, ' '), 4);//" ".PadRight(4, ' ');//CLASS-MERC-PERIG

                        linhaI3 += pesoCubado.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-CUBAGEM-M3
                        linhaI3 += !string.IsNullOrWhiteSpace(container.Lacre1) ? Utilidades.String.Right(container.Lacre1?.PadRight(15, ' '), 15) : " ".PadRight(15, ' ');//LACRE1
                        linhaI3 += !string.IsNullOrWhiteSpace(container.Lacre2) ? Utilidades.String.Right(container.Lacre2?.PadRight(15, ' '), 15) : " ".PadRight(15, ' ');//LACRE2
                        linhaI3 += !string.IsNullOrWhiteSpace(container.Lacre3) ? Utilidades.String.Right(container.Lacre3?.PadRight(15, ' '), 15) : " ".PadRight(15, ' ');//LACRE3
                        linhaI3 += " ".PadRight(15, ' ');//LACRE4
                        if (ncms.Count == 0)
                        {
                            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repCTe.BuscarProdutoEmbarcador(cte.Codigo);
                            if (produtoEmbarcador != null && !string.IsNullOrWhiteSpace(produtoEmbarcador.CodigoNCM))
                                ncms.Add(produtoEmbarcador.CodigoNCM);
                            else
                                ncms.Add("2710");
                        }

                        int qtdNCMs = 1;
                        foreach (var ncm in ncms)
                        {
                            if (qtdNCMs <= 191)
                            {
                                linhaI3 += Utilidades.String.Right(ncm.PadRight(8, ' '), 8).Replace("2016", "4415");//LACRE4                            
                            }
                            qtdNCMs++;
                        }

                        x.WriteLine(linhaI3);

                        sequencialContainer++;
                    }
                }
            }
        }

        private bool ProcessarRetornoArquivoMercanteTransbordo(out string msgRetorno, StreamReader streamReader, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string nomeArquivo, string arquivoProcessar)
        {
            msgRetorno = "";

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unitOfWork);
            Repositorio.Embarcador.Documentos.ArquivoMercanteErro repArquivoMercanteErro = new Repositorio.Embarcador.Documentos.ArquivoMercanteErro(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);

            int linha = 0;
            var cellValue = "";
            string numeroManifestoTransbordo = "";
            string numeroCEMertante = "";

            string siglaPorto = "";
            string codigoMercantePorto = "";
            string codigoIMONavio = "";
            string direcaoNavio = "";
            string numeroViagem = "";
            string portoDestino = "";

            string codigoErroGeral = "";
            string codigoErro = "";

            Dominio.Entidades.Embarcador.Documentos.ArquivoMercante arquivoMercante = null;

            //streamReader.
            streamReader.DiscardBufferedData();
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

            while ((cellValue = streamReader.ReadLine()) != null)
            {
                if (linha == 0)
                {
                    if (cellValue.Substring(0, 2).Trim() != "M4")
                    {
                        msgRetorno = "O layout do arquivo não se encontra homologado!";
                        return false;
                    }
                    else
                    {
                        siglaPorto = cellValue.Substring(1168, 3).Trim();
                        codigoMercantePorto = cellValue.Substring(1166, 5).Trim();
                        codigoIMONavio = cellValue.Substring(1190, 8).Trim();

                        portoDestino = cellValue.Substring(1171, 5).Trim();

                        direcaoNavio = cellValue.Substring(1200, 8).Trim();
                        direcaoNavio = Regex.Replace(direcaoNavio, @"[\d-]", string.Empty);
                        numeroViagem = cellValue.Substring(1200, 8).Trim().ObterSomenteNumeros();

                        arquivoMercante = repArquivoMercante.BuscarPorNavioViagemDirecao(codigoIMONavio, numeroViagem, direcaoNavio, "M4");
                        if (arquivoMercante == null)
                            arquivoMercante = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercante();
                        else
                        {
                            arquivoMercante.Initialize();
                            repArquivoMercanteErro.DeletarPorArquivoMercante(arquivoMercante.Codigo);
                        }

                        arquivoMercante.CodigoIMO = codigoIMONavio;
                        arquivoMercante.CodigoPortoOrigem = codigoMercantePorto;
                        arquivoMercante.CodigoPortoDestino = portoDestino;
                        arquivoMercante.DirecaoViagem = direcaoNavio;
                        arquivoMercante.NumeroViagem = numeroViagem;
                        arquivoMercante.TipoArquivo = "M4";
                        arquivoMercante.PortoOrigem = repPorto.BuscarPorCodigoMercante(codigoMercantePorto);
                        arquivoMercante.PortoDestino = repPorto.BuscarPorCodigoMercante(portoDestino);
                        arquivoMercante.PedidoViagemNavio = repViagem.BuscarPorCodigoImo(codigoIMONavio, direcaoNavio == "N" ? DirecaoViagemMultimodal.Norte : direcaoNavio == "S" ? DirecaoViagemMultimodal.Sul : direcaoNavio == "L" ? DirecaoViagemMultimodal.Leste : direcaoNavio == "O" ? DirecaoViagemMultimodal.Oeste : DirecaoViagemMultimodal.Norte, numeroViagem.ToInt());
                        arquivoMercante.CaminhoArquivo = arquivoProcessar;
                        arquivoMercante.Usuario = this.Usuario;
                        arquivoMercante.StatusRetorno = "OK";
                        arquivoMercante.Integrado = true;
                        arquivoMercante.NomeArquivo = nomeArquivo;
                        arquivoMercante.DataAbsorcao = DateTime.Now;

                        if (arquivoMercante.Codigo > 0)
                            repArquivoMercante.Atualizar(arquivoMercante, Auditado);
                        else
                            repArquivoMercante.Inserir(arquivoMercante, Auditado);
                    }
                }
                if (cellValue.Substring(0, 2).Trim() == "C4")
                {
                    numeroManifestoTransbordo = cellValue.Substring(5, 13).Trim();
                    numeroCEMertante = cellValue.Substring(20, 15).Trim();

                    codigoErroGeral = cellValue.Substring(3, 3).Trim();

                    if (!string.IsNullOrWhiteSpace(codigoErroGeral) && codigoErroGeral != "000")
                    {
                        arquivoMercante.StatusRetorno = "Arquivo recusado, verifique os detalhes para analise do retorno.";
                        arquivoMercante.Integrado = false;
                        repArquivoMercante.Atualizar(arquivoMercante);
                        int muliplo = 0;
                        for (int a = 0; a < 191; a++)
                        {
                            codigoErro = cellValue.Substring(36 + muliplo, 3).Trim();

                            if (!string.IsNullOrWhiteSpace(codigoErro))
                            {
                                Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro erroDetalhe = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro();
                                erroDetalhe.ArquivoMercante = arquivoMercante;
                                erroDetalhe.CodigoGeralErro = codigoErroGeral;
                                erroDetalhe.NumeroControle = numeroCEMertante;
                                erroDetalhe.CodigoErro = codigoErro;
                                repArquivoMercanteErro.Inserir(erroDetalhe);
                            }
                            muliplo += 6;
                        }
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro erro = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro();
                        erro.ArquivoMercante = arquivoMercante;
                        erro.CodigoGeralErro = codigoErroGeral;
                        erro.NumeroControle = numeroCEMertante;
                        erro.CodigoErro = "";
                        repArquivoMercanteErro.Inserir(erro);
                    }

                    if (!string.IsNullOrWhiteSpace(numeroCEMertante))
                    {
                        List<int> ctes = repCTe.BuscarPorNumeroCEMercante(numeroCEMertante);
                        foreach (var codigo in ctes)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);
                            if (cte != null && !string.IsNullOrWhiteSpace(numeroManifestoTransbordo))
                            {
                                cte.Initialize();
                                cte.NumeroManifestoTransbordo = numeroManifestoTransbordo;

                                repCTe.Atualizar(cte, Auditado);
                            }
                            unitOfWork.FlushAndClear();
                        }
                    }

                }
                linha++;
            }
            if (arquivoMercante != null && arquivoMercante.Codigo > 0)
                EnviarNotificacaoProcessamentoArquivoMercante(arquivoMercante.Codigo, unitOfWork);
            return true;
        }

        private bool ProcessarRetornoArquivoMercante(out string msgRetorno, StreamReader streamReader, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string nomeArquivo, string arquivoProcessar)
        {
            msgRetorno = "";
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unitOfWork);
            Repositorio.Embarcador.Documentos.ArquivoMercanteErro repArquivoMercanteErro = new Repositorio.Embarcador.Documentos.ArquivoMercanteErro(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            int linha = 0;
            var cellValue = "";
            string numeroManifesto = "";
            string numeroCEMertante = "";
            string numeroControle = "";
            string siglaPorto = "";
            string codigoMercantePorto = "";
            string codigoIMONavio = "";
            string direcaoNavio = "";
            string numeroViagem = "";
            string portoDestino = "";

            string codigoErroGeral = "";
            string codigoErro = "";

            Dominio.Entidades.Embarcador.Documentos.ArquivoMercante arquivoMercante = null;

            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

            while ((cellValue = streamReader.ReadLine()) != null)
            {
                if (linha == 0)
                {
                    if (cellValue.Substring(0, 2).Trim() != "M3")
                    {
                        msgRetorno = "O layout do arquivo não se encontra homologado!";
                        return false;
                    }
                    {
                        siglaPorto = cellValue.Substring(1168, 3).Trim();
                        codigoMercantePorto = cellValue.Substring(1166, 5).Trim();
                        codigoIMONavio = cellValue.Substring(1190, 8).Trim();

                        portoDestino = cellValue.Substring(1171, 5).Trim();

                        direcaoNavio = cellValue.Substring(1200, 8).Trim();
                        direcaoNavio = Regex.Replace(direcaoNavio, @"[\d-]", string.Empty);
                        numeroViagem = cellValue.Substring(1200, 8).Trim().ObterSomenteNumeros();

                        arquivoMercante = repArquivoMercante.BuscarPorNavioViagemDirecao(codigoIMONavio, numeroViagem, direcaoNavio, "M3");
                        if (arquivoMercante == null)
                            arquivoMercante = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercante();
                        else
                        {
                            repArquivoMercanteErro.DeletarPorArquivoMercante(arquivoMercante.Codigo);
                            arquivoMercante.Initialize();
                        }

                        arquivoMercante.CodigoIMO = codigoIMONavio;
                        arquivoMercante.CodigoPortoOrigem = codigoMercantePorto;
                        arquivoMercante.CodigoPortoDestino = portoDestino;
                        arquivoMercante.DirecaoViagem = direcaoNavio;
                        arquivoMercante.NumeroViagem = numeroViagem;
                        arquivoMercante.TipoArquivo = "M3";
                        arquivoMercante.PortoOrigem = repPorto.BuscarPorCodigoMercante(codigoMercantePorto);
                        arquivoMercante.PortoDestino = repPorto.BuscarPorCodigoMercante(portoDestino);
                        arquivoMercante.PedidoViagemNavio = repViagem.BuscarPorCodigoImo(codigoIMONavio, direcaoNavio == "N" ? DirecaoViagemMultimodal.Norte : direcaoNavio == "S" ? DirecaoViagemMultimodal.Sul : direcaoNavio == "L" ? DirecaoViagemMultimodal.Leste : direcaoNavio == "O" ? DirecaoViagemMultimodal.Oeste : DirecaoViagemMultimodal.Norte, numeroViagem.ToInt());
                        arquivoMercante.CaminhoArquivo = arquivoProcessar;
                        arquivoMercante.Usuario = this.Usuario;
                        arquivoMercante.StatusRetorno = "OK";
                        arquivoMercante.Integrado = true;
                        arquivoMercante.NomeArquivo = nomeArquivo;
                        arquivoMercante.DataAbsorcao = DateTime.Now;

                        if (arquivoMercante.Codigo > 0)
                            repArquivoMercante.Atualizar(arquivoMercante, Auditado);
                        else
                            repArquivoMercante.Inserir(arquivoMercante, Auditado);
                    }
                }
                if (cellValue.Substring(0, 2).Trim() == "I3")
                {
                    numeroManifesto = cellValue.Substring(10, 13).Trim();
                    numeroCEMertante = cellValue.Substring(25, 15).Trim();
                    numeroControle = cellValue.Substring(1186, 18).Trim();
                    codigoErroGeral = cellValue.Substring(7, 3).Trim();

                    if (!string.IsNullOrWhiteSpace(codigoErroGeral) && codigoErroGeral != "000")
                    {
                        arquivoMercante.StatusRetorno = "Arquivo recusado, verifique os detalhes para analise do retorno.";
                        arquivoMercante.Integrado = false;
                        repArquivoMercante.Atualizar(arquivoMercante);
                        int muliplo = 0;
                        for (int a = 0; a < 191; a++)
                        {
                            codigoErro = cellValue.Substring(40 + muliplo, 3).Trim();

                            if (!string.IsNullOrWhiteSpace(codigoErro))
                            {
                                Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro erroDetalhe = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro();
                                erroDetalhe.ArquivoMercante = arquivoMercante;
                                erroDetalhe.CodigoGeralErro = codigoErroGeral;
                                erroDetalhe.NumeroControle = numeroControle;
                                erroDetalhe.CodigoErro = codigoErro;
                                repArquivoMercanteErro.Inserir(erroDetalhe);
                            }
                            muliplo += 6;
                        }

                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro erro = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro();
                        erro.ArquivoMercante = arquivoMercante;
                        erro.CodigoGeralErro = codigoErroGeral;
                        erro.NumeroControle = numeroControle;
                        erro.CodigoErro = "";
                        repArquivoMercanteErro.Inserir(erro);
                    }


                    if (!string.IsNullOrWhiteSpace(numeroControle))
                    {
                        List<int> ctes = repCTe.BuscarPorNumeroControle(numeroControle);
                        foreach (var codigo in ctes)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                            if ((!string.IsNullOrWhiteSpace(siglaPorto) || !string.IsNullOrWhiteSpace(codigoMercantePorto)) && !string.IsNullOrWhiteSpace(codigoIMONavio) && cte.PortoOrigem != null && cte.Navio != null)
                            {
                                if (cte.Navio != null && !string.IsNullOrWhiteSpace(cte.Navio.CodigoIMO) && cte.Navio.CodigoIMO != codigoIMONavio)
                                    cte = null;
                                if (cte != null && cte.PortoOrigem != null && cte.PortoOrigem.CodigoMercante != codigoMercantePorto)
                                    cte = null;
                                //if (cte != null && cte.PortoOrigem != null && !string.IsNullOrWhiteSpace(cte.PortoOrigem.CodigoIATA) && cte.PortoOrigem.CodigoIATA != siglaPorto)
                                //    cte = null;
                            }

                            if (cte != null && !string.IsNullOrWhiteSpace(numeroManifesto))
                            {
                                cte.Initialize();

                                if (cte.CTeImportadoEmbarcador != true && (numeroManifesto != cte.NumeroManifesto || numeroCEMertante != cte.NumeroCEMercante) && (integracaoIntercab?.AtivarIntegracaoMercante ?? false))
                                    CriarRegistroIntegracaoParaDadosCTEModificados(unitOfWork, cte);

                                cte.NumeroManifesto = numeroManifesto;
                                cte.NumeroCEMercante = numeroCEMertante;

                                repCTe.Atualizar(cte, Auditado);

                                serCargaDadosSumarizados.AtualizarDadosMercanteManifesto(cte.Codigo, unitOfWork);
                            }
                            unitOfWork.FlushAndClear();
                        }
                    }

                }
                linha++;
            }
            if (arquivoMercante != null && arquivoMercante.Codigo > 0)
                EnviarNotificacaoProcessamentoArquivoMercante(arquivoMercante.Codigo, unitOfWork);
            return true;
        }

        private List<long> RetornaCodigosArquivos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<long> listaCodigos = new List<long>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaArquivos")))
            {
                dynamic listaCTes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaArquivos"));
                if (listaCTes != null)
                {
                    foreach (var cte in listaCTes)
                    {
                        listaCodigos.Add(int.Parse((string)cte.Codigo));
                    }
                }
                else
                    listaCodigos = RetornaTodosCodigosArquivos(unidadeDeTrabalho);
            }
            else
            {
                listaCodigos = RetornaTodosCodigosArquivos(unidadeDeTrabalho);
            }
            return listaCodigos;
        }

        private List<long> RetornaTodosCodigosArquivos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            string tipoArquivo = Request.GetStringParam("TipoArquivo");
            int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
            int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
            int codigoPortoDestino = Request.GetIntParam("PortoDestino");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("StatusArquivo");

            Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unidadeDeTrabalho);
            List<long> codigosCTes = new List<long>();

            codigosCTes = repArquivoMercante.ConsultarCodigos(codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoArquivo, situacao);

            return codigosCTes.Distinct().ToList();
        }

        private void EnviarNotificacaoProcessamentoArquivoMercante(long codigoAquivo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                List<string> emails = new List<string>();
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unidadeDeTrabalho.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unidadeDeTrabalho);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);
                Repositorio.Embarcador.Documentos.ArquivoMercanteErro repArquivoMercanteErro = new Repositorio.Embarcador.Documentos.ArquivoMercanteErro(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.ArquivoMercante arquivoMercante = repArquivoMercante.BuscarPorCodigo(codigoAquivo);

                string result = string.Empty;
                if (arquivoMercante.Integrado)
                    result = "(SUCESSO)";
                else
                    result = "(FALHA)";
                string assunto = string.Format(Localization.Resources.Contabils.ArquivoMercante.AlteracaoArquivoMercante.ArquivoMercanteImportado, result);

                string mensagem = "Segue em anexo o arquivo mercante importado com ";
                if (arquivoMercante.Integrado)
                    mensagem += " SUCESSO<br/><br/>";
                else
                    mensagem += " FALHA<br/><br/>";

                string strErro = "";
                List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteErro> erros = repArquivoMercanteErro.BuscarPorArquivo(codigoAquivo);
                if (erros != null && erros.Count > 0)
                {
                    foreach (var erro in erros)
                        strErro += erro.NumeroControle + ": " + (!string.IsNullOrWhiteSpace(erro.CodigoErro) ? erro.CodigoErro : "OK") + " " + erro.DescricaoErro + "<br/>";
                }
                mensagem += strErro;

                serNotificacao.GerarNotificacao(arquivoMercante.Usuario, (int)arquivoMercante.Codigo, "Contabils/AlteracaoArquivoMercante", assunto, arquivoMercante.Integrado ? IconesNotificacao.sucesso : IconesNotificacao.falha, TipoNotificacao.alerta, TipoServicoMultisoftware, unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                if (email == null)
                    return;

                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();

                if (arquivoMercante != null && !string.IsNullOrWhiteSpace(arquivoMercante.CaminhoArquivo) && Utilidades.IO.FileStorageService.Storage.Exists(arquivoMercante.CaminhoArquivo))
                    attachments.Add(new System.Net.Mail.Attachment(arquivoMercante.CaminhoArquivo));

                if (arquivoMercante == null || arquivoMercante.Usuario == null || string.IsNullOrWhiteSpace(arquivoMercante.Usuario.Email))
                    return;

                emails.AddRange(arquivoMercante.Usuario.Email.Split(';').ToList());

                if (emails == null || emails.Count == 0)
                    return;

                if (attachments == null || attachments.Count == 0)
                    return;

                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, null, emails.ToArray(), assunto, mensagem, email.Smtp, out string mensagemErro, email.DisplayEmail,
                   attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho, 0);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return;
            }
        }


        private void CriarRegistroIntegracaoParaDadosCTEModificados(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao repArquivoMercanteIntegracao = new Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reptipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao arquivoMercanteIntegracao = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao()
            {
                ConhecimentoDeTransporteEletronico = cte,
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = reptipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab),
                ProblemaIntegracao = ""

            };
            repArquivoMercanteIntegracao.Inserir(arquivoMercanteIntegracao);

            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

            if (integracaoEMP?.AtivarIntegracaoCEMercanteEMP == true)
            {
                arquivoMercanteIntegracao = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao()
                {
                    ConhecimentoDeTransporteEletronico = cte,
                    DataIntegracao = DateTime.Now,
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = reptipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP),
                    ProblemaIntegracao = ""
                };
                repArquivoMercanteIntegracao.Inserir(arquivoMercanteIntegracao);
            }
            if (integracaoEMP?.AtivarIntegracaoNFTPEMP == true)
            {
                arquivoMercanteIntegracao = new Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao()
                {
                    ConhecimentoDeTransporteEletronico = cte,
                    DataIntegracao = DateTime.Now,
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = reptipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP),
                    ProblemaIntegracao = ""
                };
                repArquivoMercanteIntegracao.Inserir(arquivoMercanteIntegracao);
            }
        }

        #endregion
    }
}
