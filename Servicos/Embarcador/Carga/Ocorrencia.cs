using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class Ocorrencia : ServicoBase
    {
        #region Construtores

        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador = null;

        public Ocorrencia() : base() { }
        
        public Ocorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            if (_configuracaoEmbarcador != null)
                return _configuracaoEmbarcador;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            return _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        private static Dominio.ObjetosDeValor.CTe.ImpostoISS BuscarRegraISS(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.NFSeManual nFSManual)
        {
            Dominio.ObjetosDeValor.CTe.ImpostoISS regraISS = new Dominio.ObjetosDeValor.CTe.ImpostoISS()
            {
                Aliquota = nFSManual.AliquotaISS,
                BaseCalculo = nFSManual.ValorBaseCalculo,
                PercentualRetencao = nFSManual.PercentualRetencao,
                ValorRetencao = nFSManual.ValorRetido,
                Valor = nFSManual.ValorISS
            };

            return regraISS;
        }

        private bool ComplementoDeveGerarNFsManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Repositorio.UnitOfWork unitOfWork)
        {
            CargaPedido serCargaPedido = new CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

            // Se o tipo da ocorrência está gerando um documento não-fiscal, e está marcado
            // que deve gerar NFS Manual, gerar (#32931)
            if (cargaCTeComplementoInfo.CargaOcorrencia?.TipoOcorrencia.ModeloDocumentoFiscal != null
                && cargaCTeComplementoInfo.CargaOcorrencia?.TipoOcorrencia?.DisponibilizarDocumentosParaNFsManual == true)
            {
                return true;
            }

            bool gerarNFSeParaComplementosTomadorIgualDestinatario = cargaCTeComplementoInfo.CargaOcorrencia?.TipoOcorrencia?.GerarNFSeParaComplementosTomadorIgualDestinatario ?? false;
            bool tomadorIgualDestinatario = cargaCTeComplementoInfo.CargaCTeComplementado?.CTe?.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario;

            if (!gerarNFSeParaComplementosTomadorIgualDestinatario || !tomadorIgualDestinatario)
                return false;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTeComplementoInfo.CargaOcorrencia.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

            TipoEmissaoIntramunicipal tipoOperacaoMunicipal = serCargaPedido.ObterTipoEmissaoIntramunicipalCarga(carga, cargaPedido.Origem, carga.TipoOperacao?.Codigo ?? 0, unitOfWork);

            if (tipoOperacaoMunicipal == TipoEmissaoIntramunicipal.NaoEspecificado)
                tipoOperacaoMunicipal = configuracaoEmbarcador.TipoEmissaoIntramunicipal;

            bool empresaEmiteForaEmbarcador = tipoOperacaoMunicipal == TipoEmissaoIntramunicipal.SempreNFSManual;

            return empresaEmiteForaEmbarcador;
        }

        private static void EnviarEmailAceiteTransportador(int codigoOcorrencia, string stringConexao)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            try
            {

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> complementos = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(cargaOcorrencia.Codigo, false);

                if (complementos.Count > 0)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = complementos.FirstOrDefault().CTe;

                    string subject = " Débito gerada e aguardando o aceite. ";
                    string body = "O débito " + conhecimento.Numero + " gerado no valor de " + conhecimento.ValorAReceber.ToString("n2") + " destinada ao transportador " + conhecimento.Empresa.Descricao + " com o motivo " + cargaOcorrencia.TipoOcorrencia.Descricao + " está aguardando o aceite.";

                    string emails = "";

                    if (conhecimento.Empresa.StatusEmail == "A" && !string.IsNullOrEmpty(conhecimento.Empresa.Email))
                    {
                        emails = conhecimento.Empresa.Email;
                    }

                    string[] splitEmail = emails.Split(';');
                    string email = splitEmail[0];
                    List<string> cc = new List<string>();

                    if (splitEmail.Length > 1)
                    {
                        for (int i = 1; i < splitEmail.Length; i++)
                        {
                            cc.Add(splitEmail[i]);
                        }
                    }

                    if (!Servicos.Email.EnviarEmailAutenticado(email, subject, body, unitOfWork, out string msg, "", null, null, cc.ToArray()))
                    {
                        Servicos.Log.TratarErro("Falha ao enviar notificação do débito gerado ao transportador: " + msg);
                    }
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private void GerarControleEmissaoNFSManual(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.NFS svcNFS = new NFS(unitOfWork);
            bool considerarLocalidadeCarga = false;
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null)
                    continue;

                bool permiteGerar = false;
                bool atualizarFlagPossuiNFSManual = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe;

                if (ComplementoDeveGerarNFsManual(cargaCTeComplementoInfo, unitOfWork))
                {
                    cargaCTe = cargaCTeComplementoInfo.CargaCTeComplementado;
                    permiteGerar = true;
                    atualizarFlagPossuiNFSManual = true;
                }
                else
                {
                    cargaCTe = repCargaCTe.BuscarCargaCTePorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);

                    if (cargaCTe == null)
                        continue;

                    if (cargaOcorrencia.ModeloDocumentoFiscal != null && cargaOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                        permiteGerar = true;
                    else if (cargaOcorrencia.Carga != null && cargaOcorrencia.Carga.TipoOperacao != null && cargaOcorrencia.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                    {
                        if (cargaOcorrencia.Carga.TipoOperacao.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual && cargaOcorrencia.Carga.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal != null)
                            permiteGerar = cargaCTe.CTe.ModeloDocumentoFiscal.Codigo == cargaOcorrencia.Carga.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal.Codigo;
                    }
                    else if (cargaCTe.CTe?.TomadorPagador != null && cargaCTe.CTe.TomadorPagador.Cliente != null)
                    {
                        if (cargaCTe.CTe.TomadorPagador.Cliente.NaoUsarConfiguracaoEmissaoGrupo && cargaCTe.CTe.TomadorPagador.Cliente.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual && cargaCTe.CTe.TomadorPagador.Cliente.ModeloDocumentoFiscalEmissaoMunicipal != null)
                            permiteGerar = cargaCTe.CTe.ModeloDocumentoFiscal.Codigo == cargaCTe.CTe.Tomador.Cliente.ModeloDocumentoFiscalEmissaoMunicipal.Codigo;
                        else if (cargaCTe.CTe.TomadorPagador.Cliente.GrupoPessoas != null && cargaCTe.CTe.TomadorPagador.Cliente.GrupoPessoas.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual && cargaCTe.CTe.TomadorPagador.Cliente.GrupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal != null)
                            permiteGerar = cargaCTe.CTe.ModeloDocumentoFiscal.Codigo == cargaCTe.CTe.TomadorPagador.Cliente.GrupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal.Codigo;
                    }

                }

                if (atualizarFlagPossuiNFSManual)
                {
                    cargaCTe.Carga.DadosSumarizados.PossuiNFSManual = true;
                    cargaCTeComplementoInfo.CargaOcorrencia.PossuiNFSManual = true;
                    cargaCTeComplementoInfo.CargaOcorrencia.NFSManualPendenteGeracao = true;
                    repCargaOcorrencia.Atualizar(cargaCTeComplementoInfo.CargaOcorrencia);
                    repCargaDadosSumarizados.Atualizar(cargaCTe.Carga.DadosSumarizados);
                }

                considerarLocalidadeCarga = cargaCTeComplementoInfo?.CargaCTeComplementado?.LancamentoNFSManual?.DadosNFS?.ConsiderarLocalidadeCarga ?? false;

                if (permiteGerar)
                {
                    if (configuracaoGeral.UtilizarLocalidadeTomadorNFSManual && !considerarLocalidadeCarga)
                        svcNFS.CriarCargaCTePendenteEmissaoManualDeNFS(cargaCTe.Carga, cargaCTe, cargaCTe.CTe.Tomador?.Localidade ?? cargaCTe.CTe.LocalidadeTerminoPrestacao, unitOfWork, cargaOcorrencia);
                    else
                        svcNFS.CriarCargaCTePendenteEmissaoManualDeNFS(cargaCTe.Carga, cargaCTe, cargaCTe.CTe.LocalidadeInicioPrestacao, unitOfWork, cargaOcorrencia);
                }
            }
        }

        private void GerarTitulosGNRE(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool IsCTeComplemento = false)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE repConfiguracaoFinanceiraGNRE = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro repConfiguracaoFinanceiraGNRERegistro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

            if (carga == null)
                return;

            if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS || carga.CargaTransbordo || (carga.GerouTituloGNREAutorizacao && !IsCTeComplemento))
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE configuracaoFinanceiraGNRE = repConfiguracaoFinanceiraGNRE.BuscarPrimeiroRegistro();
            if (configuracaoFinanceiraGNRE == null || !configuracaoFinanceiraGNRE.GerarGNREParaCTesEmitidos || !configuracaoFinanceiraGNRE.GerarGNREAutomaticamente)
                return;

            if (!repCargaCTe.CargaGeraFaturamento(carga.Codigo))
                return;

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> configuracaoFinanceiraGNRERegistros = repConfiguracaoFinanceiraGNRERegistro.BuscarPorConfiguracao(configuracaoFinanceiraGNRE.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);
            List<int> codigosCargaCTes = repCargaCTe.BuscarCodigosPorCargaComplementaresComFaturamentoEQueNaoGeraramTitulosGNRE(carga.Codigo);

            foreach (int codigoCargaCTe in codigosCargaCTes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                if (cargaCTe.GerouTituloGNREAutorizacao && !IsCTeComplemento)
                    continue;

                unidadeTrabalho.Start();

                Servicos.Embarcador.Financeiro.Titulo.GerarTituloGNRE(cargaCTe, cargaPedido, configuracaoFinanceiraGNRERegistros, unidadeTrabalho, tipoServicoMultisoftware, configuracaoFinanceiro);

                cargaCTe.GerouTituloGNREAutorizacao = true;

                repCargaCTe.Atualizar(cargaCTe);

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }

            carga = repCarga.BuscarPorCodigo(carga.Codigo);

            carga.GerouTituloGNREAutorizacao = true;

            repCarga.Atualizar(carga);
        }

        private void GerarControleFaturamentoDocumentos(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTeComplementoInfo.CargaOcorrencia.Carga;
                    if (carga == null)
                        carga = cargaCTeComplementoInfo.CargaOcorrencia.Cargas?.FirstOrDefault();

                    Servicos.Log.TratarErro($"GerarControleFaturamentoDocumentos inserindo documento faturamento - Ocorrencia {cargaOcorrencia?.Codigo ?? 0} - Carga {carga?.Codigo ?? 0} -  CTe {cargaCTeComplementoInfo?.CTe?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(carga, cargaCTeComplementoInfo.CTe, cargaCTeComplementoInfo.CargaOcorrencia, null, null, null, cargaCTeComplementoInfo.ProvisaoPelaNotaFiscal, cargaCTeComplementoInfo.ComplementoFilialEmissora, false, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
                }
            }
        }

        private static void GerarControleFinanceiroCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe != null)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.BuscarTodosPorCTe(cargaCTeComplementoInfo.CTe.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                    {
                        if (cargaCTeComplementoInfo.CTe.Status == "C")
                        {
                            documentoFaturamento.DataCancelamento = cargaCTeComplementoInfo.CTe.DataCancelamento;
                            documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;
                        }
                        else if (cargaCTeComplementoInfo.CTe.Status == "Z")
                        {
                            documentoFaturamento.DataAnulacao = cargaCTeComplementoInfo.CTe.DataAnulacao;
                            documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado;
                        }

                        repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    }
                }
            }
        }

        private void GerarTitulosAutorizacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unidadeTrabalho);

            Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe == null)
                    continue;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaOcorrencia.Carga?.Codigo ?? 0);

                int codigoBoletoConfiguracao = 0;
                bool gerarFaturamentoAVista = false;
                bool geraBoletoAutomaticamente = false;
                bool enviarBoletoPorEmailAutomaticamente = false;
                bool enviarDocumentacaoFaturamentoCTe = false;
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTeComplementoInfo.CTe;

                if (repTituloDocumento.ExisteTituloPendentePorCTe(cte))
                    continue;

                if (!VerificarSeGeraTituloAutomaticamente(cte.TomadorPagador.Cliente, unidadeTrabalho, out gerarFaturamentoAVista, out geraBoletoAutomaticamente, out codigoBoletoConfiguracao, out enviarBoletoPorEmailAutomaticamente, out enviarDocumentacaoFaturamentoCTe, cargaPedido))
                    continue;

                Dominio.Entidades.Cliente tomador = cte.TomadorPagador.Cliente;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = tomador.GrupoPessoas;

                servicoTitulo.GerarTituloPorDocumento(cte, null, grupoPessoas, tomador, configuracaoFinanceiraFatura, tipoServicoMultisoftware, gerarFaturamentoAVista, geraBoletoAutomaticamente, false, codigoBoletoConfiguracao, enviarBoletoPorEmailAutomaticamente, enviarDocumentacaoFaturamentoCTe, Auditado, 0);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia ObterCalculoFreteOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = parametrosCalcularValorOcorrencia.ListaCargaCTe.FirstOrDefault().CTe;
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Remetente.CPF_CNPJ));
            Dominio.Entidades.Cliente destinatario = null;

            if (cte.Destinatario != null)
            {
                if (!string.IsNullOrWhiteSpace(cte.Destinatario.CPF_CNPJ))
                    destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Destinatario.CPF_CNPJ));
                else if (cte.Destinatario.Cliente != null)
                    destinatario = cte.Destinatario.Cliente;
            }

            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente).FirstOrDefault();

            if (tabelaFrete == null)
                throw new ServicoException("Tabela de Frete não localizada.");

            DateTime dataFimAjustada = parametrosCalcularValorOcorrencia.DataFim;
            DateTime dataParametro = cte.DataEmissao.Value;

            if (parametrosCalcularValorOcorrencia.CodigoParametroPeriodo == 0 && !parametrosCalcularValorOcorrencia.PermiteInformarValor && parametrosCalcularValorOcorrencia.CodigoParametroData > 0)
            {
                dataParametro = parametrosCalcularValorOcorrencia.ParametroData;

                if (dataParametro == DateTime.MinValue)
                    throw new ServicoException("Data da chegada da reentrega não informada.");
            }

            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> vigencias = repVigencia.BuscarDataInicial(dataParametro, tabelaFrete.Codigo);

            if (vigencias == null || vigencias.Count == 0)
            {
                if (parametrosCalcularValorOcorrencia.CodigoParametroPeriodo == 0 && parametrosCalcularValorOcorrencia.CodigoParametroData > 0)
                    throw new ServicoException("Tabela com vigência para a data da chegada da reentrega não localizada.");
                else
                    throw new ServicoException("Tabela com vigência para a data de emissão do CT-e não localizada.");
            }

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = null;

            int codigoModeloVeicular = carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null ? carga.Veiculo.ModeloVeicularCarga.Codigo : 0;

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                codigoModeloVeicular = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga.Codigo;

            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unitOfWork);
            bool tabelaEncontrada = false;

            foreach (Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia in vigencias)
            {
                tabelaFreteCliente = repTabelaFreteCliente.BuscarPorTabelaFreteCliente(tabelaFrete.Codigo, null, null, remetente, destinatario, 0, string.Empty, vigencia != null ? vigencia.Codigo : 0, 0, 0, 0, false);

                if (tabelaFreteCliente != null)
                {
                    List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> listaParametroBaseCalculoTabelaFrete = repParametroBaseCalculoTabelaFrete.BuscarPorTabelaFrete(tabelaFreteCliente.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in listaParametroBaseCalculoTabelaFrete)
                    {
                        if (parametro.CodigoObjeto == codigoModeloVeicular)
                        {
                            tabelaEncontrada = true;
                            break;
                        }
                    }
                }

                if (tabelaEncontrada)
                    break;
            }

            if (tabelaFreteCliente == null)
                throw new ServicoException("Tabela de Frete cliente e origem não localizada.");

            decimal valorOcorrencia = 0;
            decimal valorOcorrenciaDestino = 0;
            decimal percentualAcrescimoValor = 0;
            decimal horasOcorrencia = 0;
            decimal horasOcorrenciaDestino = 0;
            double horasPeriodo = 0;
            string observacaoCTe = "";
            string observacaoCTeDestino = "";
            string observacaoOcorrenciaOrigem = "";
            string observacaoOcorrenciaDestino = "";
            bool dividirOcorrencia = false;
            DateTime horaInicioJanela = DateTime.Now;
            DateTime horaFimJanela = DateTime.Now;
            DateTime horaInicioCarga = DateTime.Now;
            DateTime horaFimCarga = DateTime.Now;
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = null;

            if (parametrosCalcularValorOcorrencia.CodigoParametroPeriodo > 0 && parametrosCalcularValorOcorrencia.DataInicio > DateTime.MinValue && parametrosCalcularValorOcorrencia.DataFim > DateTime.MinValue)
            {
                clienteDescarga = repClienteDescarga.BuscarPorOrigemEDestino(remetente.CPF_CNPJ, destinatario.CPF_CNPJ);

                if (clienteDescarga != null && !string.IsNullOrWhiteSpace(clienteDescarga.HoraInicioDescarga) && !string.IsNullOrWhiteSpace(clienteDescarga.HoraLimiteDescarga))
                {
                    horaInicioJanela = new DateTime(parametrosCalcularValorOcorrencia.DataInicio.Year, parametrosCalcularValorOcorrencia.DataInicio.Month, parametrosCalcularValorOcorrencia.DataInicio.Day, int.Parse(clienteDescarga.HoraInicioDescarga.Substring(0, 2)), int.Parse(clienteDescarga.HoraInicioDescarga.Substring(3, 2)), 0);
                    horaInicioCarga = horaInicioJanela;
                    horaFimJanela = new DateTime(horaInicioJanela.Year, horaInicioJanela.Month, horaInicioJanela.Day, int.Parse(clienteDescarga.HoraLimiteDescarga.Substring(0, 2)), int.Parse(clienteDescarga.HoraLimiteDescarga.Substring(3, 2)), 0);

                    if (horaInicioJanela > horaFimJanela && horaInicioJanela.Date == horaFimJanela.Date)
                        horaFimJanela = horaFimJanela.AddDays(1);
                    //Comentado devido a problema nas janelas que iniciavam em um dia e terminavam em outro
                    //if (horaInicioJanela > horaFimJanela && dataInicio.DayOfWeek != DayOfWeek.Sunday)
                    //{
                    //    horaInicioJanela = horaInicioJanela.AddDays(-1);
                    //    horaInicioCarga = horaInicioJanela;
                    //}
                    //else if (horaInicioJanela > horaFimJanela && horaInicioJanela.Date == horaFimJanela.Date)
                    //    horaFimJanela = horaFimJanela.AddDays(1);

                    bool cargaDomingo = false;

                    if (!clienteDescarga.Domingo && horaInicioCarga.DayOfWeek == DayOfWeek.Sunday)
                    {
                        horaInicioCarga = horaInicioCarga.AddDays(1);
                        cargaDomingo = true;
                    }

                    if (horaInicioJanela > horaFimJanela)
                    {
                        if (horaInicioJanela.DayOfWeek == DayOfWeek.Sunday)
                        {
                            horaInicioJanela = horaInicioJanela.AddDays(1);
                            horaFimJanela = horaInicioJanela.AddDays(1);
                            horaInicioCarga = horaInicioJanela;
                        }
                        else
                        {
                            horaInicioJanela = horaInicioJanela.AddDays(-1);
                            horaInicioCarga = horaInicioJanela;
                        }
                    }

                    if (parametrosCalcularValorOcorrencia.DataInicio > horaInicioCarga && parametrosCalcularValorOcorrencia.DataInicio >= horaFimJanela)
                        horaInicioCarga = horaInicioCarga.AddDays(1);
                    else if (parametrosCalcularValorOcorrencia.DataInicio > horaInicioCarga && parametrosCalcularValorOcorrencia.DataInicio < horaFimJanela)
                        horaInicioCarga = parametrosCalcularValorOcorrencia.DataInicio;

                    if (!clienteDescarga.Domingo && horaInicioCarga.DayOfWeek == DayOfWeek.Sunday)
                    {
                        horaInicioCarga = horaInicioCarga.AddDays(1);
                        cargaDomingo = true;
                    }
                    else
                        cargaDomingo = false;

                    if (parametrosCalcularValorOcorrencia.DataFim > horaInicioCarga)
                    {
                        dividirOcorrencia = true;
                        dataFimAjustada = horaInicioCarga;

                        if (cargaDomingo)
                            observacaoOcorrenciaOrigem = "Ocorrência calculada do dia " + parametrosCalcularValorOcorrencia.DataInicio.ToString() + " até as " + horaInicioCarga.ToString() + " (horário de inicio da janela do Destino).";
                        else if (parametrosCalcularValorOcorrencia.DataInicio < horaInicioJanela && horaInicioJanela > horaInicioCarga)
                            observacaoOcorrenciaOrigem = "Ocorrência calculada do dia " + parametrosCalcularValorOcorrencia.DataInicio.ToString() + " até as " + horaInicioJanela.ToString() + " (horário de inicio da janela do Destino).";
                        else if (horaInicioCarga.Hour <= horaInicioJanela.Hour && horaInicioCarga.Date == horaInicioJanela.Date)
                            observacaoOcorrenciaOrigem = "Ocorrência calculada do dia " + parametrosCalcularValorOcorrencia.DataInicio.ToString() + " até as " + horaInicioCarga.ToString() + " (horário de inicio da janela do Destino).";
                        else if (parametrosCalcularValorOcorrencia.DataInicio > horaInicioCarga)
                            observacaoOcorrenciaOrigem = "Ocorrência calculada do dia " + parametrosCalcularValorOcorrencia.DataInicio.ToString() + " até as " + parametrosCalcularValorOcorrencia.DataInicio.ToString() + " (horário de chegada no Destino).";
                        else
                            observacaoOcorrenciaOrigem = "Ocorrência calculada do dia " + parametrosCalcularValorOcorrencia.DataInicio.ToString() + " até as " + horaInicioCarga.ToString() + " (horário de chegada no Destino).";
                    }
                }

                TimeSpan diferenca = dataFimAjustada - parametrosCalcularValorOcorrencia.DataInicio;
                horasPeriodo = diferenca.TotalHours;

                if (horasPeriodo >= (parametrosCalcularValorOcorrencia.HorasSemFranquia + (parametrosCalcularValorOcorrencia.FreeTime ?? 0)))
                    horasOcorrencia = decimal.Parse(horasPeriodo.ToString()) - parametrosCalcularValorOcorrencia.HorasSemFranquia - (parametrosCalcularValorOcorrencia?.FreeTime ?? 0);
            }

            valorOcorrencia = ObterValorOcorrenciaPorTabelaCliente(parametrosCalcularValorOcorrencia, carga, tabelaFreteCliente.Codigo, horasOcorrencia, codigoModeloVeicular, unitOfWork);

            if (dividirOcorrencia)
            {
                TimeSpan diferenca = parametrosCalcularValorOcorrencia.DataFim - horaInicioCarga;
                horasPeriodo = diferenca.TotalHours;
                horasOcorrenciaDestino = decimal.Parse(horasPeriodo.ToString()) - parametrosCalcularValorOcorrencia.HorasSemFranquia - (parametrosCalcularValorOcorrencia?.FreeTime ?? 0);

                if (horasOcorrenciaDestino > 0)
                {
                    valorOcorrenciaDestino = ObterValorOcorrenciaPorTabelaCliente(parametrosCalcularValorOcorrencia, carga, tabelaFreteCliente.Codigo, horasOcorrenciaDestino, codigoModeloVeicular, unitOfWork);
                    observacaoOcorrenciaDestino = "Ocorrência calculada referente ao período das " + horaInicioCarga.ToString() + " até " + parametrosCalcularValorOcorrencia.DataFim.ToString() + " referente a espera no destino.";
                }
            }

            if (valorOcorrencia == 0 && tipoOcorrencia != null && tipoOcorrencia.PermiteInformarValor)
                valorOcorrencia = parametrosCalcularValorOcorrencia.ValorOcorrencia;

            if (tabelaFrete.PercentualCobrancaPadraoTerceiros > 0 && tabelaFrete.EmpresaContratante != null && tabelaFrete.EmpresaContratante.CNPJ == cte.Empresa.CNPJ)
            {
                percentualAcrescimoValor = tabelaFrete.PercentualCobrancaPadraoTerceiros;
                //    if (valorOcorrencia > 0)
                //        observacaoCTe = "Frete Despesa: R$" + String.Format("{0:0.##}", valorOcorrencia, cultura) + " / Tabela de Frete utilizada para o Calculo é a mesma da data da prestação do serviço;";
                //    if (valorOcorrenciaDestino > 0)
                //        observacaoCTeDestino = "Frete Despesa: R$" + String.Format("{0:0.##}", valorOcorrenciaDestino, cultura) + " / Tabela de Frete utilizada para o Calculo é a mesma da data da prestação do serviço;";
            }

            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
            {
                DividirOcorrencia = dividirOcorrencia,
                HorasOcorrencia = horasOcorrencia,
                HorasOcorrenciaDestino = horasOcorrenciaDestino,
                IncluirICMSFrete = tabelaFrete.IncluirICMSValorFrete,
                ObservacaoCTe = observacaoCTe,
                ObservacaoCTeDestino = observacaoCTeDestino,
                ObservacaoOcorrencia = observacaoOcorrenciaOrigem,
                ObservacaoOcorrenciaDestino = observacaoOcorrenciaDestino,
                PercentualAcrescimoValor = percentualAcrescimoValor,
                ValorCalculadoPorTabelaFrete = false,
                ValorOcorrencia = valorOcorrencia,
                ValorOcorrenciaDestino = valorOcorrenciaDestino
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia ObterCalculoFreteOcorrenciaPorTabelaFrete(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = null;

            //Filtrar carga pedidos conforme ctes da ocorrência
            if (parametrosCalcularValorOcorrencia.ListaCargaCTe?.Count > 0)
                cargaPedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarListaCargaPedidoPorCargaCTes(parametrosCalcularValorOcorrencia.ListaCargaCTe.Select(o => o.Codigo).ToList());
            else
                cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unitOfWork);

            StringBuilder mensagemRetorno = new StringBuilder();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = serFrete.ObterTabelasFrete(carga, unitOfWork, tipoServicoMultisoftware, configuracao, ref mensagemRetorno, false, null, false, false, tipoOcorrencia.Codigo, parametrosCalcularValorOcorrencia.LocalFreeTime, parametrosCalcularValorOcorrencia.FronteiraOUParqueamento);

            if (mensagemRetorno.Length > 0)
                throw new ServicoException(mensagemRetorno.ToString());

            if (tabelasFrete.Count == 0)
                throw new ServicoException("Não foi localizada uma tabela de frete compatível com as informações da ocorrência.");

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = serFrete.ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, false, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracao, true);

            if (parametrosCalculoFrete == null)
                throw new ServicoException("Não foi possível obter os parametros para cálculo de frete da carga pois os pedidos da carga não são cálculaveis (exemplo, somente pedidos de pallet)");

            if (tabelaFrete.Tempos.Count() > 0 && (parametrosCalcularValorOcorrencia.DataInicio.Day != parametrosCalcularValorOcorrencia.DataFim.Day || parametrosCalcularValorOcorrencia.DataInicio.Month != parametrosCalcularValorOcorrencia.DataFim.Month || parametrosCalcularValorOcorrencia.DataInicio.Year != parametrosCalcularValorOcorrencia.DataFim.Year))
                throw new ServicoException("A data inicial e final devem estar dentro do mesmo dia, mês e ano.");

            if (tabelaFrete.Horas.Count > 0 && (parametrosCalcularValorOcorrencia.DataInicio == DateTime.MinValue || parametrosCalcularValorOcorrencia.DataFim == DateTime.MinValue))
                throw new ServicoException("É obrigatório informar a inicial e a data final.");

            parametrosCalculoFrete.DataInicialViagem = parametrosCalcularValorOcorrencia.DataInicio;
            parametrosCalculoFrete.DataFinalViagem = parametrosCalcularValorOcorrencia.DataFim;
            parametrosCalculoFrete.Distancia = parametrosCalcularValorOcorrencia.KmInformado;
            parametrosCalculoFrete.NumeroAjudantes = parametrosCalcularValorOcorrencia.QuantidadeAjudantes;

            if (parametrosCalcularValorOcorrencia.Minutos > 0)
            {
                parametrosCalculoFrete.DataInicialViagem = DateTime.Now.Date;
                parametrosCalculoFrete.DataFinalViagem = DateTime.Now.Date.AddMinutes(parametrosCalcularValorOcorrencia.Minutos);
            }

            parametrosCalculoFrete.Minutos = (int)(parametrosCalculoFrete.DataFinalViagem.Value - parametrosCalculoFrete.DataInicialViagem.Value).TotalMinutes;
            parametrosCalculoFrete.DeducaoMinutos = (parametrosCalcularValorOcorrencia.DeducaoHoras + parametrosCalcularValorOcorrencia?.FreeTime ?? 0) * 60;
            Log.TratarErro($"parametrosCalculoFrete.Minutos: {parametrosCalculoFrete.Minutos}; parametrosCalculoFrete.DeducaoMinutos: {parametrosCalculoFrete.DeducaoMinutos}", "GATILHO");

            decimal fracaoHorasOcorrencia = (decimal)parametrosCalculoFrete.TotalMinutos / 60;
            decimal horasOcorrencia = tabelaFrete.TipoArredondamentoHoras == TipoArredondamentoTabelaFrete.ParaCima ? Math.Ceiling(fracaoHorasOcorrencia) :
                tabelaFrete.TipoArredondamentoHoras == TipoArredondamentoTabelaFrete.ParaBaixo ? Math.Floor(fracaoHorasOcorrencia) :
                Math.Round(fracaoHorasOcorrencia, 2, MidpointRounding.AwayFromZero);
            Log.TratarErro($"tabelaFrete: {tabelaFrete.Codigo}; fracaoHorasOcorrencia.Minutos: {fracaoHorasOcorrencia}; horasOcorrencia: {horasOcorrencia}", "GATILHO");

            if (parametrosCalcularValorOcorrencia.ApenasReboque && parametrosCalculoFrete.ModeloVeiculo != null && parametrosCalculoFrete.ModeloVeiculo.NumeroReboques == 0)
                throw new ServicoException("O modelo veicular da carga não é um modelo articulado (" + parametrosCalculoFrete.ModeloVeiculo.Descricao + ").");

            if (parametrosCalcularValorOcorrencia.ApenasReboque)
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaTracaoReboque = repModeloVeicularCarga.BuscarTracaoReboque();

                if (modeloVeicularCargaTracaoReboque != null)
                    parametrosCalculoFrete.ModeloVeiculo = modeloVeicularCargaTracaoReboque;
            }

            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repositorioComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculoFrete, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();

            Log.TratarErro($"tabelaFreteCliente: {tabelaFreteCliente?.Codigo}; horasOcorrencia: {horasOcorrencia}", "GATILHO");

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentesFreteTabelaFrete = new List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

            if (tabelaFreteCliente == null)
            {
                dadosCalculoFrete.FreteCalculado = false;
                dadosCalculoFrete.FreteCalculadoComProblemas = true;
                dadosCalculoFrete.MensagemRetorno = mensagemRetorno.ToString();

                Log.TratarErro($"Problemas ao encontrar tabela frete cliente: {mensagemRetorno.ToString()}", "GATILHO");
            }
            else
            {
                componentesFreteTabelaFrete = repositorioComponenteFreteTabelaFrete.BuscarPorTabelaFreteParaCalculo(tabelaFreteCliente.TabelaFrete.Codigo, parametrosCalculoFrete.Veiculo?.PossuiTagValePedagio ?? false);

                if (componentesFreteTabelaFrete.Exists(x => x.UtilizarCalculoDesseComponenteNaOcorrencia ?? false))
                    dadosCalculoFrete.ValorFrete = carga.ValorFrete;

                if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                    svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosCalculoFrete, parametrosCalculoFrete, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
                else
                    svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosCalculoFrete, parametrosCalculoFrete, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

                dadosCalculoFrete.FreteCalculado = true;
            }


            decimal valorTotalFrete = 0;

            if (!componentesFreteTabelaFrete.Exists(x => x.UtilizarCalculoDesseComponenteNaOcorrencia ?? false))
                valorTotalFrete = dadosCalculoFrete.ValorFrete;

            if (dadosCalculoFrete.Componentes?.Count > 0)
            {
                decimal valorComponente = (from obj in dadosCalculoFrete.Componentes select obj.ValorComponente).Sum();
                valorTotalFrete += parametrosCalcularValorOcorrencia.QuantidadeAjudantes > 0 ? parametrosCalcularValorOcorrencia.QuantidadeAjudantes * valorComponente : valorComponente;
            }

            Log.TratarErro($"Frete calculado: {dadosCalculoFrete.FreteCalculado}", "GATILHO");

            if (!dadosCalculoFrete.FreteCalculado)
                throw new ServicoException(dadosCalculoFrete.MensagemRetorno);

            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
            {
                IncluirICMSFrete = tabelaFrete.IncluirICMSValorFrete,
                ObservacaoCTe = tabelaFrete.IncluirICMSValorFrete ? tabelaFrete.Observacao : "",
                ValorCalculadoPorTabelaFrete = true,
                HorasOcorrencia = horasOcorrencia + parametrosCalcularValorOcorrencia?.FreeTime ?? 0,
                ValorOcorrencia = valorTotalFrete
            };
        }

        private decimal ObterValorOcorrenciaPorTabelaCliente(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoTabelaCliente, decimal horasTotais, int codigoModeloVeicular, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);
            decimal valorTotal = 0;

            List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> listaParametroBaseCalculoTabelaFrete = repParametroBaseCalculoTabelaFrete.BuscarPorTabelaFrete(codigoTabelaCliente);

            foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in listaParametroBaseCalculoTabelaFrete)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParameto = null;

                if (codigoModeloVeicular == 0)
                    codigoModeloVeicular = carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null ? carga.Veiculo.ModeloVeicularCarga.Codigo : 0;

                if (parametro.CodigoObjeto == codigoModeloVeicular)
                {
                    itemParameto = repItemParametroBaseCalculoTabelaFrete.BuscarPorCodigoObjetoETipoItem(0, parametro.Codigo, parametrosCalcularValorOcorrencia.CodigoParametroInteiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ParametrosOcorrencia);

                    if (itemParameto != null)
                        valorTotal = itemParameto.ValorParaCalculo;
                    else
                    {
                        itemParameto = repItemParametroBaseCalculoTabelaFrete.BuscarPorCodigoObjetoETipoItem(0, parametro.Codigo, parametrosCalcularValorOcorrencia.CodigoParametroBooleano, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ParametrosOcorrencia);

                        if (parametrosCalcularValorOcorrencia.ApenasReboque && itemParameto != null)
                        {
                            if (horasTotais > 0)
                                valorTotal = itemParameto.ValorParaCalculo * horasTotais;
                        }
                        else
                        {
                            itemParameto = repItemParametroBaseCalculoTabelaFrete.BuscarPorCodigoObjetoETipoItem(0, parametro.Codigo, parametrosCalcularValorOcorrencia.CodigoParametroPeriodo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ParametrosOcorrencia);

                            if ((itemParameto != null) && (horasTotais > 0))
                                valorTotal = itemParameto.ValorParaCalculo * horasTotais;
                        }
                    }
                }
            }

            return valorTotal;
        }

        private static void PreecherQuantidadesTotais(List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes, ref List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesGlobal)
        {
            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCTe in quantidadesCTes)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeGlobal = (from obj in quantidadesGlobal where obj.Unidade == quantidadeCTe.Unidade select obj).FirstOrDefault();
                if (quantidadeGlobal == null)
                    quantidadesGlobal.Add(quantidadeGlobal);
                else
                    quantidadeGlobal.Quantidade += quantidadeCTe.Quantidade;
            }
        }

        private static void ProcessarIntegracoesOcorrencia(bool integracaoFilialEmissora, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao(unitOfWork);

            Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(ocorrencia, integracaoFilialEmissora, tipoServicoMultisoftware, unitOfWork);
            bool primeiraPassagem = !ocorrencia.IntegrandoFilialEmissora;

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao> ocorrenciasEDIIntegracao = repOcorrenciaEDIIntegracao.BuscarPorOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, integracaoFilialEmissora);
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao> ocorrenciaIntegracao = repOcorrenciaIntegracao.BuscarPorOcorrencia(ocorrencia.Codigo);

            if (ocorrenciasEDIIntegracao.Count() > 0 || (ocorrenciaIntegracao.Count() > 0 && !ocorrencia.IntegrandoFilialEmissora))
            {
                if (integracaoFilialEmissora)
                    ocorrencia.IntegrandoFilialEmissora = true;
                ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao;
            }
            else
            {
                if (integracaoFilialEmissora)
                {
                    ocorrencia.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = true;
                    ocorrencia.GerouTodosDocumentos = false;
                }
                else
                    ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
            }

            if (ocorrenciaIntegracao.Count() > 0 && (!ocorrencia.IntegrandoFilialEmissora || primeiraPassagem))
                ocorrencia.GerandoIntegracoes = true;
        }

        private static void RemoverEGerarMovimentacaoDosCTesEmitidosPorOutroSistema(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctesEmitidosEmOutroSistema = repCargaCTe.BuscarPorOcorrenciaEmitidosEmOutroSistema(ocorrenciaCancelamento.Ocorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in ctesEmitidosEmOutroSistema)
            {
                if (
                    ocorrenciaCancelamento.SituacaoOcorrenciaNoCancelamento == SituacaoOcorrencia.Finalizada ||
                    ocorrenciaCancelamento.SituacaoOcorrenciaNoCancelamento == SituacaoOcorrencia.FalhaIntegracao //não gerou movimentação de autorização, gera as movimentações de autorização antes de gerar as de cancelamento
                )
                {
                    if (!Cancelamento.GerarMovimentoCancelamentoCTe(out string mensagemErro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, stringConexao, true))
                        throw new ServicoException(mensagemErro);
                }

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> integracoes = repOcorrenciaCTeIntegracao.BuscarPorCargaCTe(cargaCTe.Codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao> integracoesCancelamento = repOcorrenciaCTeCancelamentoIntegracao.BuscarPorCargaCTe(cargaCTe.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamentos = repDocumentoFaturamento.BuscarTodosPorCTe(cargaCTe.CTe.Codigo);

                Servicos.Embarcador.Carga.Cancelamento.RemoverDocumentoFaturamentoCancelamentoCarga(documentoFaturamentos, unidadeTrabalho);

                //remove os veículos importados para o CT-e da carga, (CT-e 3.0 não tem veículo aí o sistema usa os da carga ao importar o CT-e para a mesma)
                foreach (Dominio.Entidades.VeiculoCTE veiculoCTe in cargaCTe.CTe.Veiculos)
                    if (repVeiculoCTe.BuscarPorCodigo(veiculoCTe.Codigo) != null)
                        if (veiculoCTe.ImportadoCarga)
                            repVeiculoCTe.Deletar(veiculoCTe);

                if (integracoesCancelamento.Count == 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao in integracoes)
                        repOcorrenciaCTeIntegracao.Deletar(integracao);

                    repCargaCTe.Deletar(cargaCTe);
                }
            }
        }

        private static void ReverterSaldoContratoPrestacaoServico(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaOcorrencia.Carga != null)
            {
                Embarcador.Frete.ContratoPrestacaoServicoSaldo servicoSaldo = new Embarcador.Frete.ContratoPrestacaoServicoSaldo(unitOfWork);

                if (servicoSaldo.IsUtilizaContratoPrestacaoServico())
                {
                    Embarcador.Carga.Carga servicoCarga = new Embarcador.Carga.Carga(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados = new Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados()
                    {
                        CodigoFilial = cargaOcorrencia.Carga.Filial?.Codigo ?? 0,
                        CodigoTransportador = cargaOcorrencia.Carga.Empresa?.Codigo ?? 0,
                        Descricao = $"Saldo referente ao cancelamento da carga {servicoCarga.ObterNumeroCarga(cargaOcorrencia.Carga, unitOfWork)}",
                        TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                        TipoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoContratoPrestacaoServico.Entrada,
                        Valor = cargaOcorrencia.ValorOcorrencia
                    };

                    servicoSaldo.Adicionar(dados);
                }
            }
        }

        private static void VincularPedidoXMLNotaAoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<Dominio.Entidades.DocumentosCTE> documentos = repDocumentosCTE.BuscarPorCTe(cte.Codigo);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            cte.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                if (documento.NumeroModelo == "55")
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChaveECarga(documento.ChaveNFE, carga.Codigo);
                    if (xmlNotaFiscal != null)
                        cte.XMLNotaFiscais.Add(xmlNotaFiscal);
                }
                else
                {
                    int numero = 0;
                    int.TryParse(documento.Numero, out numero);
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorNumeroECarga(numero, carga.Codigo);
                    if (xmlNotaFiscal != null)
                        cte.XMLNotaFiscais.Add(xmlNotaFiscal);
                }
            }

            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;//todo: rever isso, pois por padrão sempre considera que o CT-e importado tem o valor do icms incluso no frete, isso afeta possiveis ctes de complemento.
            repCTe.Atualizar(cte);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint ObterWayPoint(Dominio.Entidades.Localidade localidade)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
            {
                Lat = localidade.Latitude.ToString().ToDouble(),
                Lng = localidade.Longitude.ToString().ToDouble()
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repositorioCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> cargasEntregaNfeDevolucao = repositorioCargaEntregaNFeDevolucao.BuscarPorCargaEntrega(parametrosCalcularValorOcorrencia.CargaEntrega.Codigo);

            decimal pesoTotais = 0;

            IEnumerable<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> listaCargaEntregaNotasDevolvidas = parametrosCalcularValorOcorrencia.CargaEntregaNotaFiscals.Where(x => x.DevolucaoParcial || x.DevolucaoTotal);

            IEnumerable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCte = parametrosCalcularValorOcorrencia.ListaCargaCTe
                .Where(x => x.CTe.XMLNotaFiscais
                    .Any(nf => listaCargaEntregaNotasDevolvidas
                        .Select(ce => ce.Numero)
                        .Contains(nf.Numero)));

            decimal pesoTotal = listaCargaCte.Sum(o => o.CTe.Peso);

            if (!parametrosCalcularValorOcorrencia.DevolucaoParcial)
                pesoTotais = pesoTotal;
            else
            {
                List<int> numerosDevolucao = cargasEntregaNfeDevolucao
                    .Select(obj => obj.XMLNotaFiscal.Numero)
                    .ToList();

                List<int> numerosNotasFiltradas = listaCargaEntregaNotasDevolvidas
                    .Where(x => !numerosDevolucao.Contains(x.Numero))
                    .Select(x => x.Numero)
                    .ToList();

                if (numerosNotasFiltradas.Count > 0)
                    pesoTotais = repositorioCargaEntregaNotaFiscal.BuscarPesoPorCargaEntregaNumerosNotas(
                        parametrosCalcularValorOcorrencia.CargaEntrega.Codigo,
                        numerosNotasFiltradas);

                pesoTotais += cargasEntregaNfeDevolucao.Sum(x => x.PesoDevolvido);
            }

            decimal valorCtes = listaCargaCte.Sum(o => o.CTe.ValorAReceber);

            if (pesoTotal == 0 || valorCtes == 0)
                throw new ServicoException("Não há peso nos CTEs para cálculo do valor da ocorrência.");

            decimal valorFrete = (valorCtes / pesoTotal) * pesoTotais * (tipoOcorrencia.PercentualDoFrete / 100);

            parametrosCalcularValorOcorrencia.ListaCargaCTe = listaCargaCte.ToList();

            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
            {
                ValorCalculadoPorTabelaFrete = false,
                ValorOcorrencia = valorFrete
            };
        }

        private static void AvancarEtapaGestaoDevolucao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao svcGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, auditado, clienteMultisoftware);

            svcGestaoDevolucao.AvancarEtapaGestaoDevolucao(new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork).BuscarPorCodigo(ocorrencia.GestaoDevolucao.Codigo), "Finalizado ao finalizar atendimento");
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public static void AvancarEtapaOcorrenciaPosEmissao(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.AceiteDebito repAceiteDebito = new Repositorio.Embarcador.Ocorrencias.AceiteDebito(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            bool possuiCTeComplementar = repCargaCTeComplementoInfo.ExisteCargaCTeComplementoInfoPorOcorrencia(ocorrencia.Codigo);

            if (possuiCTeComplementar)
            {
                bool requerAceiteTransportador = !configuracaoTMS.NaoExigeAceiteTransportadorParaNFDebito ? (ocorrencia.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito) : false;
                Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito aceite = repAceiteDebito.BuscarPorOcorrencia(ocorrencia.Codigo);

                if (aceite == null && requerAceiteTransportador)
                {
                    aceite = new Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito()
                    {
                        DataCriacao = DateTime.Now,
                        DataRetorno = null,
                        Observacao = "",
                        Ocorrencia = ocorrencia,
                        Situacao = SituacaoAceiteDebito.AgAceite,
                        Usuario = null
                    };

                    repAceiteDebito.Inserir(aceite);
                }

                if (aceite != null && aceite.Situacao == SituacaoAceiteDebito.AgAceite)
                {
                    ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgAceiteTransportador;
                    if (ocorrencia.NotificarDebitosAtivos)
                    {
                        int codigoOcorrencia = ocorrencia.Codigo;
                        string stringConexao = unitOfWork.StringConexao;
                        Task t = Task.Factory.StartNew(() => { EnviarEmailAceiteTransportador(codigoOcorrencia, stringConexao); });
                    }
                }
                else if (aceite != null && aceite.Situacao == SituacaoAceiteDebito.Rejeitado)
                    ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.DebitoRejeitadoTransportador;
                else
                {
                    if (ocorrencia.TipoOcorrencia == null || !ocorrencia.TipoOcorrencia.NaoGerarIntegracao)
                        ProcessarIntegracoesOcorrencia(false, ocorrencia, tipoServicoMultisoftware, unitOfWork);
                    else
                        ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.Finalizada;
                }
            }
            else
            {
                var tiposIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
                {
                    TipoIntegracao.Riachuelo,
                    TipoIntegracao.Electrolux
                };

                if (ocorrencia.TipoOcorrencia != null && repOcorrenciaTipoIntegracao.PossuiIntegracaoPorTipoOcorrenciaETipoIntegracao(ocorrencia.TipoOcorrencia.Codigo, tiposIntegracao))
                    ProcessarIntegracoesOcorrencia(false, ocorrencia, tipoServicoMultisoftware, unitOfWork);
                else
                    ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.Finalizada;
            }

            ocorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
            if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Finalizada)
            {
                Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(ocorrencia, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrencia(ocorrencia.Codigo);
                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                {

                    servicoChamado.GerarIntegracoes(chamado, unitOfWork, Auditado, tipoServicoMultisoftware);

                    if (chamado.Situacao != SituacaoChamado.AgIntegracao && chamado.Situacao == SituacaoChamado.LiberadaOcorrencia)
                    {
                        chamado.Situacao = SituacaoChamado.Finalizado;

                        repChamado.Atualizar(chamado);
                    }
                }

                if (ocorrencia.GestaoDevolucao != null)
                    AvancarEtapaGestaoDevolucao(unitOfWork, Auditado, ocorrencia, clienteMultisoftware);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia CalcularValorOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigo(parametrosCalcularValorOcorrencia.CodigoTipoOcorrencia);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(parametrosCalcularValorOcorrencia.CodigoCarga);

            if (parametrosCalcularValorOcorrencia.ListaCargaCTe == null || parametrosCalcularValorOcorrencia.ListaCargaCTe.Count == 0)
                throw new ServicoException("CT-e da carga não localizado/selecionado para cálculo do valor.");

            if (parametrosCalcularValorOcorrencia.ListaCargaCTe.Exists(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
                throw new ServicoException("Não é possível selecionar uma NFS para gerar a ocorrência.");

            if (tipoOcorrencia?.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao ?? false)
                return EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao(parametrosCalcularValorOcorrencia, tipoOcorrencia, unitOfWork);

            if (tipoOcorrencia.OcorrenciaPorPercentualDoFrete && tipoOcorrencia.PercentualDoFrete > 0 && tipoOcorrencia.CalcularValorCTEComplementarPeloValorCTESemImposto)
            {
                //Valor do Frete + Componentes
                decimal valorCtesSemImposto = parametrosCalcularValorOcorrencia.ListaCargaCTe.Sum(o => o.CTe.ValorPrestacaoServico);
                decimal valorOcorrencia = valorCtesSemImposto * (tipoOcorrencia.PercentualDoFrete / 100);

                return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
                {
                    ValorCalculadoPorTabelaFrete = false,
                    ValorOcorrencia = valorOcorrencia
                };
            }

            if (tipoOcorrencia.OcorrenciaPorPercentualDoFrete && tipoOcorrencia.PercentualDoFrete > 0 && tipoOcorrencia.TipoInclusaoImpostoComplemento != TipoInclusaoImpostoComplemento.NuncaIncluir)
            {
                decimal valorCtes = parametrosCalcularValorOcorrencia.ListaCargaCTe.Sum(o => o.CTe.ValorAReceber);
                decimal valorOcorrencia = valorCtes * (tipoOcorrencia.PercentualDoFrete / 100);

                return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
                {
                    ValorCalculadoPorTabelaFrete = false,
                    ValorOcorrencia = valorOcorrencia
                };
            }

            if (tipoOcorrencia.TipoInclusaoImpostoComplemento == TipoInclusaoImpostoComplemento.NuncaIncluir)
            {
                decimal valorLiquidoSemInclusaoImposto = parametrosCalcularValorOcorrencia.ListaCargaCTe.Sum(o => o.Carga.ValorFreteLiquido);
                decimal valorOcorrencia = 0m;

                if (tipoOcorrencia.OcorrenciaPorPercentualDoFrete)
                    valorOcorrencia = valorLiquidoSemInclusaoImposto * (tipoOcorrencia.PercentualDoFrete / 100);
                else
                    valorOcorrencia = valorLiquidoSemInclusaoImposto;

                return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
                {
                    ValorCalculadoPorTabelaFrete = false,
                    ValorOcorrencia = valorOcorrencia
                };
            }

            if (tipoOcorrencia.GerarOcorrenciaComMesmoValorCTesAnteriores)
            {
                decimal valorCtes = parametrosCalcularValorOcorrencia.ListaCargaCTe.Sum(o => o.CTe.ValorFrete);

                return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
                {
                    ValorCalculadoPorTabelaFrete = false,
                    ValorOcorrencia = Math.Round(valorCtes, 2)
                };
            }

            if (tipoOcorrencia.TipoOcorrenciaControleEntrega && !tipoOcorrencia.PermiteInformarValor && !tipoOcorrencia.CalculaValorPorTabelaFrete)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
                {
                    ValorCalculadoPorTabelaFrete = false,
                    ValorOcorrencia = 0
                };
            }

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && !tipoOcorrencia.CalculaValorPorTabelaFrete)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia()
                {
                    ValorCalculadoPorTabelaFrete = false,
                    ValorOcorrencia = 0
                };
            }
            else
            {
                if (!tipoOcorrencia.CalculaValorPorTabelaFrete)
                    return ObterCalculoFreteOcorrencia(parametrosCalcularValorOcorrencia, carga, tipoOcorrencia, unitOfWork);

                return ObterCalculoFreteOcorrenciaPorTabelaFrete(parametrosCalcularValorOcorrencia, carga, tipoOcorrencia, unitOfWork, configuracaoEmbarcador, tipoServicoMultisoftware);
            }
        }

        public void CancelarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cancelamentoOcorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);

            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);
            Servicos.Embarcador.Credito.SolicitacaoCredito serSolicitacaoCredito = new Credito.SolicitacaoCredito(unitOfWork);
            Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Credito.CreditoMovimentacao(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = cancelamentoOcorrencia.Ocorrencia;

            if (cargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada)
            {
                if (cargaOcorrencia.SolicitacaoCredito != null)
                    serSolicitacaoCredito.ExtornarSolicitacaoCredito(cargaOcorrencia.SolicitacaoCredito, unitOfWork, tipoServicoMultisoftware);

                cancelamentoOcorrencia.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.Cancelada;
                cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada;
                cargaOcorrencia.SituacaoOcorrenciaNoCancelamento = cancelamentoOcorrencia.SituacaoOcorrenciaNoCancelamento;
                repCargaOcorrencia.Atualizar(cargaOcorrencia);

                List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorOcorrencia(cargaOcorrencia.Codigo);
                serCreditoMovimentacao.ExtornarCreditos(creditosUtilizadosDestino, tipoServicoMultisoftware, unitOfWork);
                RemoverCTeOcorrenciaEliberarCTeImportadoGerados(cargaOcorrencia, unitOfWork);

                Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(cargaOcorrencia.Carga, unitOfWork, StringConexao, tipoServicoMultisoftware, false);
                if (cargaOcorrencia.EmiteComplementoFilialEmissora)
                    Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(cargaOcorrencia.Carga, unitOfWork, StringConexao, tipoServicoMultisoftware, true);

                ReverterSaldoContratoPrestacaoServico(cargaOcorrencia, unitOfWork);

                if (cancelamentoOcorrencia.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento)
                    GerarMovimentosCancelamentoOcorrencia(cargaOcorrencia, unitOfWork, tipoServicoMultisoftware);
                else
                    GerarMovimentosAnulacaoOcorrencia(cargaOcorrencia, unitOfWork, tipoServicoMultisoftware);

                GerarControleFinanceiroCancelamentoOcorrencia(cargaOcorrencia, unitOfWork, tipoServicoMultisoftware);
            }

            if (cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada && cancelamentoOcorrencia.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.Cancelada)
                cancelamentoOcorrencia.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.Cancelada;

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrencia(cargaOcorrencia.Codigo);
            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
            {
                chamado.Situacao = SituacaoChamado.LiberadaOcorrencia;
                repChamado.Atualizar(chamado);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;
                if (cargaEntrega != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);
                    if (cargaEvento != null)
                    {
                        servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                    }
                }

            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(cargaOcorrencia.Codigo, false);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTesComplementoInfo in cargaCTesComplementaresInfo)
            {
                if (cargaCTesComplementoInfo.CTe != null)
                {
                    cargaCTesComplementoInfo.ComplementoIntegradoEmbarcador = false;
                    //todo: remover rateios por produtos do CTe complementar aqui
                    repCargaCTeComplementoInfo.Atualizar(cargaCTesComplementoInfo);
                }
            }

            if (cancelamentoOcorrencia.Ocorrencia.Carga != null)
                Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ValidarOcorrenciaPendente(cancelamentoOcorrencia.Ocorrencia.Carga, unitOfWork);

            Servicos.Embarcador.Escrituracao.CancelamentoProvisao.CancelarProvisaoDocumentosOcorrencia(cargaOcorrencia, unitOfWork);
            Servicos.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento.AdicionarDocumentoParaEscrituracao(cargaOcorrencia, unitOfWork);

            repOcorrenciaCancelamento.Atualizar(cancelamentoOcorrencia);
        }

        public void GerarMovimentosAnulacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(StringConexao);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete repCargaNFSComponentes = new Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modeloNFSe = repModeloDocumento.BuscarPorModelo("39");

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe != null && cargaCTeComplementoInfo.CTe.Status == "Z")
                {
                    DateTime dataMovimentacao = cargaCTeComplementoInfo.CTe.DataAnulacao != null ? cargaCTeComplementoInfo.CTe.DataAnulacao.Value : cargaCTeComplementoInfo.CTe.DataEmissao.Value;
                    string observacaoMovimentacao = "Movimento gerado à partir da anulação do " + cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.Abreviacao + " " + cargaCTeComplementoInfo.CTe.Numero + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero + " da ocorrência " + cargaOcorrencia.NumeroOcorrencia.ToString() + ".";

                    svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoAnulacao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorAReceber, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaImpostos)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoAnulacao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorICMS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: ICMS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaPIS)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoPISAnulacao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorPIS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorCOFINS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaIR)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoIRAnulacao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorIR, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCSLL)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLAnulacao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorCSLL, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCTeComplementoInfo.CTe.CargaCTes)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentesFrete = repCargaCTeComponentes.BuscarPorCargaCTeQueGeraMovimentacao(cargaCte.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componente in componentesFrete)
                            if (componente.ComponenteFrete.GerarMovimentoAutomatico)
                                svcMovimento.GerarMovimentacao(componente.ComponenteFrete.TipoMovimentoAnulacao, dataMovimentacao, componente.ValorComponente, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componente.ComponenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);
                    }
                }
            }
        }

        public void GerarMovimentosCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(StringConexao);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete repCargaNFSComponentes = new Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modeloNFSe = repModeloDocumento.BuscarPorModelo("39");

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe != null && cargaCTeComplementoInfo.CTe.Status == "C")
                {
                    DateTime dataMovimentacao = cargaCTeComplementoInfo.CTe.DataCancelamento != null ? cargaCTeComplementoInfo.CTe.DataCancelamento.Value : cargaCTeComplementoInfo.CTe.DataEmissao.Value;
                    string observacaoMovimentacao = "Movimento gerado à partir do cancelamento do " + cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.Abreviacao + " " + cargaCTeComplementoInfo.CTe.Numero + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero + " da ocorrência " + cargaOcorrencia.NumeroOcorrencia.ToString() + ".";

                    svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoCancelamento, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorAReceber, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaImpostos)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoCancelamento, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorICMS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: ICMS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaPIS)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoPISCancelamento, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorPIS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorCOFINS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaIR)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoIRCancelamento, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorIR, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCSLL)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLCancelamento, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorCSLL, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCTeComplementoInfo.CTe.CargaCTes)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentesFrete = repCargaCTeComponentes.BuscarPorCargaCTeQueGeraMovimentacao(cargaCte.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componente in componentesFrete)
                            if (componente.ComponenteFrete.GerarMovimentoAutomatico)
                                svcMovimento.GerarMovimentacao(componente.ComponenteFrete.TipoMovimentoCancelamento, dataMovimentacao, componente.ValorComponente, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componente.ComponenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento);
                    }
                }
            }
        }

        public void GerarMovimentosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(StringConexao);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete repCargaNFSComponentes = new Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modeloNFSe = repModeloDocumento.BuscarPorModelo("39");

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe != null && cargaCTeComplementoInfo.CTe.Status == "A")
                {
                    DateTime dataMovimentacao = cargaCTeComplementoInfo.CTe.DataAutorizacao != null ? cargaCTeComplementoInfo.CTe.DataAutorizacao.Value : cargaCTeComplementoInfo.CTe.DataEmissao.Value;
                    string observacaoMovimentacao = "Movimento gerado à partir do " + cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.Abreviacao + " " + cargaCTeComplementoInfo.CTe.Numero + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero + " da ocorrência " + cargaOcorrencia.NumeroOcorrencia.ToString() + ".";

                    svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoEmissao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorAReceber, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaImpostos)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoEmissao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorICMS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: ICMS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaPIS)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoPISEmissao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorPIS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSEmissao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorCOFINS, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaIR)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoIREmissao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorIR, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCSLL)
                        svcMovimento.GerarMovimentacao(cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLEmissao, dataMovimentacao, cargaCTeComplementoInfo.CTe.ValorCSLL, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> lstCargaCTe = repCargaCTe.BuscarPorCodigoCTe(cargaCTeComplementoInfo.CTe.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in lstCargaCTe)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentesFrete = repCargaCTeComponentes.BuscarPorCargaCTeQueGeraMovimentacao(cargaCte.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componente in componentesFrete)
                            if (componente.ComponenteFrete.GerarMovimentoAutomatico)
                                svcMovimento.GerarMovimentacao(componente.ComponenteFrete.TipoMovimentoEmissao, dataMovimentacao, componente.ValorComponente, cargaCTeComplementoInfo.CTe.Numero.ToString() + "-" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componente.ComponenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);
                    }

                }
                else if (cargaCTeComplementoInfo.CargaNFS != null && cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                {
                    DateTime dataMovimentacao = cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.DataEmissao;
                    string observacaoMovimentacao = "Movimento gerado à partir do " + modeloNFSe.Abreviacao + " " + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero + " da ocorrência " + cargaOcorrencia.NumeroOcorrencia.ToString() + ".";

                    svcMovimento.GerarMovimentacao(modeloNFSe.TipoMovimentoEmissao, dataMovimentacao, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.ValorServicos, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero, observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (modeloNFSe.DiferenciarMovimentosParaImpostos)
                        svcMovimento.GerarMovimentacao(modeloNFSe.TipoMovimentoImpostoEmissao, dataMovimentacao, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.ValorISS, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero, observacaoMovimentacao + " Imposto: ISS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (modeloNFSe.DiferenciarMovimentosParaPIS)
                        svcMovimento.GerarMovimentacao(modeloNFSe.TipoMovimentoPISEmissao, dataMovimentacao, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.ValorPIS, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero, observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (modeloNFSe.DiferenciarMovimentosParaCOFINS)
                        svcMovimento.GerarMovimentacao(modeloNFSe.TipoMovimentoCOFINSEmissao, dataMovimentacao, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.ValorCOFINS, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero, observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (modeloNFSe.DiferenciarMovimentosParaIR)
                        svcMovimento.GerarMovimentacao(modeloNFSe.TipoMovimentoIREmissao, dataMovimentacao, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.ValorIR, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero, observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    if (modeloNFSe.DiferenciarMovimentosParaCSLL)
                        svcMovimento.GerarMovimentacao(modeloNFSe.TipoMovimentoCSLLEmissao, dataMovimentacao, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.ValorCSLL, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero, observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete> componentesFrete = repCargaNFSComponentes.BuscarPorCargaNFSQueGeraMovimentacao(cargaCTeComplementoInfo.CargaNFS.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete componente in componentesFrete)
                        if (componente.ComponenteFrete.GerarMovimentoAutomatico)
                            svcMovimento.GerarMovimentacao(componente.ComponenteFrete.TipoMovimentoEmissao, dataMovimentacao, componente.ValorComponente, cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Numero + "-" + cargaCTeComplementoInfo.CargaNFS.NotaFiscalServico.NFSe.Serie.Numero, observacaoMovimentacao + " Componente: " + componente.ComponenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTeComplementoInfo.CTe.TomadorPagador?.Cliente, cargaCTeComplementoInfo.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTeComplementoInfo.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao);
                }
            }
        }

        public static void GerarOcorrenciaAutomaticaTabelaFreteMinima(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga.TabelaFrete == null || !carga.TabelaFrete.EmitirOcorrenciaAutomatica)
                return;

            bool transacaoIniciada = unitOfWork.IsActiveTransaction();
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                string ctes = string.Join(",", from cte in carga.CargaCTes select cte.CTe.Numero);

                var cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
                {
                    Carga = carga,
                    DataAlteracao = DateTime.Now,
                    DataOcorrencia = DateTime.Now,
                    DataFinalizacaoEmissaoOcorrencia = DateTime.Now,
                    NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                    Observacao = $"Ocorrência gerada automaticamente tabela de frete mínima. CTEs {ctes}. Valor a receber {carga?.ValorAReceberCTes?.ToString("N2"): 0,00} ",
                    SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes,
                    ValorOcorrencia = 0,
                    ValorOcorrenciaOriginal = 0,
                    ObservacaoCTe = string.Empty,
                    CTeEmitidoNoEmbarcador = false,
                    TipoOcorrencia = carga?.TabelaFrete?.TipoOcorrenciaTabelaMinima,
                    ComponenteFrete = carga?.TabelaFrete?.TipoOcorrenciaTabelaMinima?.ComponenteFrete
                };

                if (!transacaoIniciada)
                    unitOfWork.Start();

                repOcorrencia.Inserir(cargaOcorrencia);

                foreach (var cargaCTE in carga.CargaCTes)
                {

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento
                    {
                        CargaCTe = cargaCTE,
                        CargaOcorrencia = cargaOcorrencia,
                    };

                    repCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);
                }

                if (!transacaoIniciada)
                    unitOfWork.CommitChanges();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (!transacaoIniciada)
                    unitOfWork.Rollback();
            }
        }

        public static void GerarObjetoCTeNFSeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.NFSeManual dadosNFSeManual, Dominio.Enumeradores.TipoCTE tipo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);
            Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
            Servicos.Embarcador.Carga.CTe serCte = new CTe(unitOfWork);
            Servicos.Embarcador.Carga.RateioNFSManual serRateioNFManual = new RateioNFSManual();
            Servicos.Embarcador.Carga.CTeComplementar serCTeComplementar = new CTeComplementar(unitOfWork);
            // Hub
            Hubs.Ocorrencia svcHubOcorrencia = new Hubs.Ocorrencia();


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            List<string> rotas = new List<string>();
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = null;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = dadosNFSeManual.ModeloDocumentoFiscal;
            int tipoEnvio = 0;
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componenteFreteDinamico = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe
            {
                Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
            };

            Dominio.Entidades.Empresa empresa = cargaCteComplementarInfo.CargaOcorrencia.ObterEmitenteOcorrencia();
            cte.Emitente = Servicos.Empresa.ObterEmpresaCTE(empresa);
            unitOfWork.Start();

            if (empresa == null)
                throw new Exception("Nenhuma empresa para emissão.");

            if (cargaCteComplementarInfo.CargaCTeComplementado != null)
                cteComplementado = cargaCteComplementarInfo.CargaCTeComplementado.CTe;
            else if (cargaCteComplementarInfo.CargaOcorrencia.Cargas != null && cargaCteComplementarInfo.CargaOcorrencia.Cargas.Count > 0)
            {
                var cteComLocalidaDoTomador = from o in cargaCteComplementarInfo.CargaOcorrencia.Cargas.FirstOrDefault().CargaCTes
                                              where o.CTe != null && o.CargaCTeComplementoInfo == null
                                              select o.CTe;
                cteComplementado = cteComLocalidaDoTomador.FirstOrDefault();
            }

            //if (cteComplementado == null)
            //    throw new Exception("Nenhum documento anterior encontrado.");

            decimal peso = 1;
            int volumes = 1;
            if (cteComplementado != null)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cteComplementado.XMLNotaFiscais)
                {
                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(xmlNotaFiscal, empresa.TipoAmbiente, configuracaoEmbarcador);
                    cte.Documentos.Add(docNF);
                }

                peso = (from q in cteComplementado.QuantidadesCarga where q.UnidadeMedida == "02" select q.Quantidade).Sum();
                volumes = (int)(from q in cteComplementado.QuantidadesCarga where q.UnidadeMedida == "04" select q.Quantidade).Sum();
            }

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>()
            {
                cargaCteComplementarInfo.CargaOcorrencia.Carga
            };
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = cteComplementado != null ? ctesAnteriores = serCTeComplementar.ObterCTesAnteriores(cteComplementado) : null;

            cte.TipoCTe = tipo;
            cte.ISS = BuscarRegraISS(dadosNFSeManual);
            cte.Serie = dadosNFSeManual.Serie;
            cte.ValorAReceber = dadosNFSeManual.ValorFrete;
            cte.ValorFrete = dadosNFSeManual.ValorFrete;
            cte.ValorTotalPrestacaoServico = dadosNFSeManual.ValorFrete;
            cte.ValorTotalMercadoria = cteComplementado != null ? cteComplementado.ValorTotalMercadoria : 0;
            cte.ProdutoPredominante = cteComplementado != null ? cteComplementado.ProdutoPredominante : "DIVERSOS";

            cte.TipoCTeIntegracao = cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.TipoCTeIntegracao;
            cte.TipoConhecimentoProceda = cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.TipoConhecimentoProceda;

            cte.CIOT = cteComplementado != null ? cteComplementado.CIOT : string.Empty;
            cte.indicadorIETomador = cteComplementado == null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS :
                                     cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS :
                                     cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento :
                                     cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte :
                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
            cte.indicadorGlobalizado = cteComplementado != null ? cteComplementado.IndicadorGlobalizado : Dominio.Enumeradores.OpcaoSimNao.Sim;

            Dominio.Enumeradores.TipoPagamento tipoPagamento = cteComplementado != null ? cteComplementado.TipoPagamento : Dominio.Enumeradores.TipoPagamento.Pago;
            Dominio.Entidades.Cliente tomador = cteComplementado != null ? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.TomadorPagador.CPF_CNPJ)) : cargaCteComplementarInfo.CargaOcorrencia.Tomador;
            Dominio.Enumeradores.TipoTomador tipoTomador = cteComplementado != null ? cteComplementado.TipoTomador : Dominio.Enumeradores.TipoTomador.Remetente;

            Dominio.Entidades.Cliente remetente = cteComplementado != null ? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Remetente.CPF_CNPJ)) : cargaCteComplementarInfo.CargaOcorrencia.Tomador;
            Dominio.Entidades.Cliente destinatario = cteComplementado != null ? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Destinatario.CPF_CNPJ)) : cargaCteComplementarInfo.CargaOcorrencia.Tomador;
            Dominio.Entidades.Cliente expedidor = null;
            Dominio.Entidades.Cliente recebedor = null;

            if (cteComplementado != null && cteComplementado.Expedidor != null)
                expedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Expedidor.CPF_CNPJ));

            if (cteComplementado != null && cteComplementado.Recebedor != null)
                recebedor = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Recebedor.CPF_CNPJ));

            cte.ChaveCTESubstituicaoComplementar = cteComplementado != null ? cteComplementado.Chave : string.Empty;

            Dominio.Entidades.Localidade destino = cteComplementado != null ? cteComplementado.LocalidadeTerminoPrestacao : cargaCteComplementarInfo.CargaOcorrencia.Tomador.Localidade;
            Dominio.Entidades.Localidade origem = destino;

            //if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel.HasValue)
            //{
            //    if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel.Value != cteComplementado.TipoTomador)
            //    {
            //        if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Outros)
            //            tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
            //        else if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Remetente)
            //            tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            //        else
            //            tipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;

            //        if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
            //        {
            //            tomador = remetente;
            //            tipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            //        }
            //        else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
            //        {
            //            tomador = destinatario;
            //            tipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            //        }
            //        else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            //        {
            //            tomador = cargaCteComplementarInfo.CargaOcorrencia.Tomador;
            //            tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

            //            if (modeloDocumentoFiscal.Numero == "39")
            //                destino = tomador.Localidade;
            //        }

            //        cte.ChaveCTESubstituicaoComplementar = string.Empty;

            //        if (cte.Documentos.Count == 0)
            //        {
            //            Dominio.ObjetosDeValor.CTe.Documento documento = new Dominio.ObjetosDeValor.CTe.Documento
            //            {
            //                Tipo = Dominio.Enumeradores.TipoDocumentoCTe.Outros,
            //                Descricao = "COMPLEMENTO",
            //                Numero = cteComplementado.Numero.ToString(),
            //                ModeloDocumentoFiscal = "99",
            //                DataEmissao = cteComplementado.DataEmissao.Value.ToString("dd/MM/yyyy"),
            //                Valor = 0
            //            };
            //            cte.Documentos.Add(documento);
            //        }
            //    }
            //}

            cte.Remetente = serCliente.ObterClienteCTE(remetente, null);
            cte.Destinatario = serCliente.ObterClienteCTE(destinatario, null);
            if (cte.Remetente != null)
                cte.CodigoIBGECidadeInicioPrestacao = cte.Remetente.CodigoIBGECidade;
            if (cte.Destinatario != null)
                cte.CodigoIBGECidadeTerminoPrestacao = cte.Destinatario.CodigoIBGECidade;

            if (cte.ValorFrete > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesNFe = serCte.RetornaQuantidades(peso, volumes, 0m, 0m, 0m);
                PreecherQuantidadesTotais(quantidades, ref quantidades);

                //List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTes = serCte.GerarCTe(cargaCteComplementarInfo, cargas, cargaCteComplementarInfo.CargaOcorrencia.ObtemEmitenteOcorrencia(), cte, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, null, null, tipoPagamento, tipoTomador, quantidades, componenteFreteDinamico, dadosNFSeManual.Observacao, pedido, rotas, seguros, regraICMS, regraISS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, true, 0, unitOfWork);
                //Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = cargasCTes.FirstOrDefault().CTe;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCTe.GerarCTePorObjeto(cte, 0, unitOfWork, "1", tipoEnvio, "A", modeloDocumentoFiscal, dadosNFSeManual.Numero, tipoServicoMultisoftware);

                cargaCteComplementarInfo.CTe = cteIntegrado;
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCargaCTeComplementoInfo(cargaCteComplementarInfo.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                {
                    cargaCTe.CTe = cteIntegrado;
                    VincularPedidoXMLNotaAoCTe(cteIntegrado, cargaCTe.Carga, unitOfWork);

                    repCargaCTe.Atualizar(cargaCTe);
                }

                repCargaCTeComplementoInfo.Atualizar(cargaCteComplementarInfo);

                unitOfWork.CommitChanges();
            }
            else
            {
                unitOfWork.CommitChanges();
            }
        }

        public static void RefazerComplementacaoValorFreteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool componenteFilialEmissora)
        {
            if (carga == null || carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                return;

            Servicos.Embarcador.Carga.RateioFrete serCargaFreteRateio = new RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia, componenteFilialEmissora);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repCargaOcorrencia.BuscarOcorrenciasComplementoValorFreteCarga(carga.Codigo);

            if (cargaComponentesFrete.Count > 0 || ocorrencias.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete in cargaComponentesFrete)
                    repCargaComponentesFrete.Deletar(cargaComponenteFrete);

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
                {
                    serComponetesFrete.AdicionarComponenteFreteCargaUnicoPorTipo(carga, ocorrencia.ComponenteFrete, ocorrencia.ValorOcorrencia, 0m, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, ocorrencia.ComponenteFrete.TipoComponenteFrete, null, true, false, null, tipoServicoMultisoftware, ocorrencia.Usuario, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia, false);
                    if (ocorrencia.EmiteComplementoFilialEmissora)
                        serComponetesFrete.AdicionarComponenteFreteCargaUnicoPorTipo(carga, ocorrencia.ComponenteFrete, ocorrencia.ValorOcorrencia, 0m, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, ocorrencia.ComponenteFrete.TipoComponenteFrete, null, true, false, null, tipoServicoMultisoftware, ocorrencia.Usuario, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia, false);
                }

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                serCargaFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, false, unitOfWork, tipoServicoMultisoftware);

                if (carga.EmpresaFilialEmissora != null)
                    serCargaFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, true, unitOfWork, tipoServicoMultisoftware);

                repCarga.Atualizar(carga);
            }
        }

        public void RemoverCTeOcorrenciaEliberarCTeImportadoGerados(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(cargaOcorrencia.Codigo);
            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento in cargaOcorrenciaDocumentos)
            {
                if (cargaOcorrenciaDocumento.OcorrenciaDeCTe != null)
                {
                    repOcorrenciaDeCTe.Deletar(cargaOcorrenciaDocumento.OcorrenciaDeCTe);
                    cargaOcorrenciaDocumento.OcorrenciaDeCTe = null;
                    repCargaOcorrenciaDocumento.Atualizar(cargaOcorrenciaDocumento);
                }

                if (cargaOcorrenciaDocumento.CTeImportado != null && cargaOcorrenciaDocumento.CTeImportado.Status == "A")//se um CT-e foi importado e a ocorrência foi cancelada e o CT-e não, o mesmo é liberado para que possa ser utilizado em outra carga.
                {
                    cargaOcorrenciaDocumento.CTeImportado.CTeSemCarga = true;
                    repCTe.Atualizar(cargaOcorrenciaDocumento.CTeImportado);
                }
            }
        }

        public bool VerificarSeOcorrenciaPermiteCancelamento(out string mensagem, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

            if (repOcorrenciaCancelamento.ExistePorOcorrencia(ocorrencia.Codigo))
            {
                mensagem = "Já existe um cancelamento registrado para esta ocorrência.";
                return false;
            }

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[] situacoesPermitemCancelamento = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[] {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Todas,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgConfirmacaoUso,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.SemRegraAprovacao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAutorizacaoEmissao,
                //Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgEmissaoCTeComplementar,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.FalhaIntegracao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.SemRegraEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.DebitoRejeitadoTransportador,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAceiteTransportador
            };

            if (situacoesPermitemCancelamento.Contains(ocorrencia.SituacaoOcorrencia) ||
               (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar && ocorrencia.AgImportacaoCTe))
            {
                //if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                //{
                //    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrencia.Codigo);
                //    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTesComplementoInfo in cargaCTesComplementaresInfo)
                //    {
                //        if (cargaCTesComplementoInfo.CTe != null && cargaCTesComplementoInfo.CTe.CargaCTes.Any(obj => obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe))
                //        {
                //            if (cargaCTesComplementoInfo.CTe.Status == "A" && cargaCTesComplementoInfo.CTe.DataRetornoSefaz < DateTime.Now.AddDays(-7) && cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57")
                //            {
                //                mensagem = "Não é possível cancelar CT-e(s) que foram emitidos a mais de 7 dias.";
                //                return false;
                //            }
                //        }
                //    }
                //}

                if (repCargaDocumentoParaEmissaoNFSManual.ExistemEmitidosPorOcorrencia(ocorrencia.Codigo))
                {
                    mensagem = "Já existe NFS Manual emitida para esta ocorrência, não sendo possível realizar o cancelamento.";
                    return false;
                }
            }
            else
            {
                mensagem = "Não é possível cancelar a ocorrência na atual situação (" + ocorrencia.DescricaoSituacao + ").";
                return false;
            }

            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repDocumentoProvisao.BuscarDocumentoOcorrenciaEmFechamento(ocorrencia.Codigo);
            if (documentoProvisao != null)
            {
                mensagem = "Antes de cancelar a ocorrencia é necessário finalizar a provisão de seus documento, a ocorrência está sendo provisionada na provisão de número " + documentoProvisao.Provisao.Numero + ".";
                return false;
            }

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarDocumentoOcorrenciaEmFechamento(ocorrencia.Codigo);
            if (documentoFaturamento != null)
            {
                mensagem = "Antes de cancelar a ocorrencia é necessário finalizar a pagamento, a carga está sendo finalizada no pagamento de número " + documentoFaturamento.Pagamento.Numero + ".";
                return false;
            }

            if (repDocumentoFaturamento.ExisteDocumentoProvisionadoPorOcorrencia(ocorrencia.Codigo))
            {
                mensagem = "Não é possível cancelar a ocorrência pois seus documentos já foram pagos.";
                return false;
            }

            mensagem = null;
            return true;
        }

        public string SolicitarCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cancelamentoOcorrencia, Repositorio.UnitOfWork unitOfWork, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            string retorno = "";
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[] situacoesPermitemCancelamento = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[] {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgConfirmacaoUso,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.SemRegraAprovacao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAutorizacaoEmissao,
                //Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgEmissaoCTeComplementar,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.FalhaIntegracao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.SemRegraEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.DebitoRejeitadoTransportador,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAceiteTransportador
            };

            if (situacoesPermitemCancelamento.Contains(cancelamentoOcorrencia.Ocorrencia.SituacaoOcorrencia) ||
               (cancelamentoOcorrencia.Ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar && cancelamentoOcorrencia.Ocorrencia.AgImportacaoCTe))
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cancelamentoOcorrencia.Ocorrencia.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTesComplementoInfo in cargaCTesComplementaresInfo)
                    {
                        if (cargaCTesComplementoInfo.CTe != null && cargaCTesComplementoInfo.CTe.CargaCTes.Any(obj => obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe))
                        {
                            if (cargaCTesComplementoInfo.CTe.Status == "A" && cargaCTesComplementoInfo.CTe.DataRetornoSefaz < DateTime.Now.AddDays(-7) && cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57")
                            {
                                retorno = "Não é possível cancelar CT-e(s) que foram emitidos a mais de 7 dias.";
                                break;
                            }
                        }
                    }
                }
                if (repCargaDocumentoParaEmissaoNFSManual.ExistemEmitidosPorOcorrencia(cancelamentoOcorrencia.Ocorrencia.Codigo))
                    retorno = "Já existe NFS Manual emitida para esta ocorrência, não sendo possível realizar o cancelamento.";
            }
            else
                retorno = "Não é possível cancelar a ocorrência na atual situação (" + cancelamentoOcorrencia.Ocorrencia.DescricaoSituacao + ").";

            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repDocumentoProvisao.BuscarDocumentoOcorrenciaEmFechamento(cancelamentoOcorrencia.Ocorrencia.Codigo);
            if (documentoProvisao != null)
                retorno = "Antes de cancelar a ocorrencia é necessário finalizar a provisão de seus documento, a ocorrência está sendo provisionada na provisão de número " + documentoProvisao.Provisao.Numero + ".";

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarDocumentoOcorrenciaEmFechamento(cancelamentoOcorrencia.Ocorrencia.Codigo);
            if (documentoFaturamento != null)
                retorno = "Antes de cancelar a ocorrencia é necessário finalizar a pagamento, a carga está sendo finalizada no pagamento de número " + documentoFaturamento.Pagamento.Numero + ".";

            bool liquidada = repDocumentoFaturamento.ExisteDocumentoProvisionadoPorOcorrencia(cancelamentoOcorrencia.Ocorrencia.Codigo);
            if (liquidada)
                retorno = "Não é possível cancelar a ocorrência pois seus documentos já foram pagos.";

            return retorno;
        }

        public string ValidarEmissaoComplementosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool emiteCTes, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, string webServiceOracle = "", bool avancarEtapaOcorrenciaPosEmissao = true, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware = null)
        {
            string retorno = "";
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Financeiro.ConciliacaoTransportador serConciliacaoTransportador = new Servicos.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
            RateioProduto serRateioProduto = new Servicos.Embarcador.Carga.RateioProduto(unitOfWork);

            bool cteFilialEmissora = false;
            if (cargaOcorrencia.EmiteComplementoFilialEmissora && !cargaOcorrencia.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                cteFilialEmissora = true;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo, cteFilialEmissora);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            bool emitiuTodos = true;
            bool rejeicao = false;
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> ctesParaEmissao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe == null && cargaCTeComplementoInfo.CargaNFS == null && cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualGerado == null) // Se ainda não foi processado
                {
                    // Regra antiga que englobava NFS. Agora, não emitiremos mais complementos de NFs, apenas de CargaDocumentoParaEmissaoNFSManual diretamente.
                    if (cargaCTeComplementoInfo.CargaCTeComplementado != null && cargaCTeComplementoInfo.CargaCTeComplementado.CTe?.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = repCargaNFeParaEmissaoNFSManual.BuscarPorOcorrenciaECargaCTe(cargaOcorrencia.Codigo, cargaCTeComplementoInfo.CargaCTeComplementado.Codigo);
                        if (cargaDocumentoParaEmissaoNFSManual != null)
                            continue;
                    }

                    if (cargaCTeComplementoInfo.CargaCTeComplementado?.CTe?.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && (cargaOcorrencia.Carga?.Empresa?.EmiteNFSeOcorrenciaForaEmbarcador ?? false))
                        continue;

                    emitiuTodos = false;
                    ctesParaEmissao.Add(cargaCTeComplementoInfo);
                }
                else if (cargaCTeComplementoInfo.CTe != null)
                {
                    if (cargaCTeComplementoInfo.CTe.Status != "A")
                        emitiuTodos = false;

                    if (cargaCTeComplementoInfo.CTe.Status == "E" && cargaCTeComplementoInfo.CTe.CodigoCTeIntegrador == 0 && (cargaCTeComplementoInfo.CTe.SistemaEmissor ?? TipoEmissorDocumento.Integrador) == TipoEmissorDocumento.Integrador)
                    {
                        cargaCTeComplementoInfo.CTe.Status = "R";
                        cargaCTeComplementoInfo.CTe.MensagemRetornoSefaz = "888 - Falha ao conectar com o Sefaz.";
                        repCTe.Atualizar(cargaCTeComplementoInfo.CTe);
                    }

                    if (cargaCTeComplementoInfo.CTe.Status == "R" || cargaCTeComplementoInfo.CTe.Status == "D" || cargaCTeComplementoInfo.CTe.Status == "I" || cargaCTeComplementoInfo.CTe.Status == "P" || cargaCTeComplementoInfo.CTe.Status == "S")
                    {
                        rejeicao = true;
                        break;
                    }
                }
            }

            unitOfWork.Start();

            if (emitiuTodos)
            {
                //flega para dividir o avança da ocorrencia da geração dos documentos 
                cargaOcorrencia.GerouTodosDocumentos = true;

                if (avancarEtapaOcorrenciaPosEmissao)
                {
                    Hubs.Ocorrencia hubOcorrencia = new Hubs.Ocorrencia();
                    if (!cargaOcorrencia.EmiteComplementoFilialEmissora || cargaOcorrencia.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaOcorrencia.Carga;
                        GerarMovimentosOcorrencia(cargaOcorrencia, unitOfWork, tipoServicoMultisoftware);
                        GerarTitulosAutorizacaoOcorrencia(cargaOcorrencia, cargaCTesComplementoInfo, unitOfWork, tipoServicoMultisoftware, Auditado);

                        Servicos.Log.TratarErro($"ValidarEmissaoComplementosOcorrencia inserindo documento faturamento - Ocorrencia {cargaOcorrencia?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                        GerarControleFaturamentoDocumentos(cargaOcorrencia, cargaCTesComplementoInfo, unitOfWork, tipoServicoMultisoftware, configuracao);

                        GerarControleEmissaoNFSManual(cargaOcorrencia, cargaCTesComplementoInfo, unitOfWork, tipoServicoMultisoftware); // Removido em 02/07/2021
                        GerarTitulosGNRE(ref carga, cargaCTesComplementoInfo, unitOfWork, configuracaoFinanceiro, tipoServicoMultisoftware, true);

                        if (!configuracao.NaoRatearValorFreteProtudos && cargaOcorrencia.Carga != null)
                            serRateioProduto.RatearFretePorProduto(cargaOcorrencia, tipoServicoMultisoftware, unitOfWork);

                        Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(cargaOcorrencia, tipoServicoMultisoftware, unitOfWork);

                        AvancarEtapaOcorrenciaPosEmissao(ref cargaOcorrencia, Auditado, tipoServicoMultisoftware, unitOfWork, clienteMultisoftware);

                        repCargaOcorrencia.Atualizar(cargaOcorrencia);

                        if (cargaOcorrencia.ComplementoValorFreteCarga)
                        {
                            RefazerComplementacaoValorFreteCarga(cargaOcorrencia.Carga, unitOfWork, StringConexao, tipoServicoMultisoftware, false);
                            if (cargaOcorrencia.EmiteComplementoFilialEmissora)
                                RefazerComplementacaoValorFreteCarga(cargaOcorrencia.Carga, unitOfWork, StringConexao, tipoServicoMultisoftware, true);
                        }

                        if (cargaOcorrencia.Carga != null)
                            Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ValidarOcorrenciaPendente(cargaOcorrencia.Carga, unitOfWork);

                        repCargaOcorrencia.Atualizar(cargaOcorrencia);

                        hubOcorrencia.InformarOcorrenciaAtualizada(cargaOcorrencia.Codigo);
                    }
                    else
                    {
                        Servicos.Log.TratarErro($"ValidarEmissaoComplementosOcorrencia inserindo documento faturamento - Ocorrencia {cargaOcorrencia?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                        GerarControleFaturamentoDocumentos(cargaOcorrencia, cargaCTesComplementoInfo, unitOfWork, tipoServicoMultisoftware, configuracao);

                        ProcessarIntegracoesOcorrencia(true, cargaOcorrencia, tipoServicoMultisoftware, unitOfWork);

                        repCargaOcorrencia.Atualizar(cargaOcorrencia);

                        hubOcorrencia.InformarOcorrenciaAtualizada(cargaOcorrencia.Codigo);
                    }

                    if (cargaOcorrencia.Carga != null)
                        serConciliacaoTransportador.AdicionarCargaEmConciliacaoTransportador(cargaOcorrencia.Carga);
                }
                else
                    repCargaOcorrencia.Atualizar(cargaOcorrencia);
            }
            else
            {
                if (cargaOcorrencia.NaoGerarDocumento)
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao;
                else if (!rejeicao)
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;
                else
                {
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao;

                    if (cargaOcorrencia.Usuario != null)
                    {
                        Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Notificacao.Notificacao(unitOfWork.StringConexao, null, tipoServicoMultisoftware, "");
                        serNotificaocao.GerarNotificacao(cargaOcorrencia.Usuario, cargaOcorrencia.Codigo, "Ocorrencias/Ocorrencia", string.Format(Localization.Resources.Ocorrencias.Ocorrencia.RejeicaoEmissaoComplementosOcorrencia, cargaOcorrencia.NumeroOcorrencia), IconesNotificacao.falha, TipoNotificacao.alerta, tipoServicoMultisoftware, unitOfWork);
                    }
                }
                repCargaOcorrencia.Atualizar(cargaOcorrencia);
            }

            unitOfWork.CommitChanges();

            if (ctesParaEmissao.Count > 0 && emiteCTes)
            {
                Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new CTeComplementar(unitOfWork);
                try
                {
                    serCargaCTeComplementar.EmitirDocumentoComplementar(ctesParaEmissao, unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware, webServiceOracle);
                }
                catch (ArgumentNullException excecao)
                {
                    cargaOcorrencia.SituacaoOcorrencia = SituacaoOcorrencia.PendenciaEmissao;
                    cargaOcorrencia.MotivoRejeicao = excecao.ParamName;
                    repCargaOcorrencia.Atualizar(cargaOcorrencia);
                }
                catch (ServicoException excecao)
                {
                    cargaOcorrencia.SituacaoOcorrencia = SituacaoOcorrencia.PendenciaEmissao;
                    cargaOcorrencia.MotivoRejeicao = excecao.Message;
                    repCargaOcorrencia.Atualizar(cargaOcorrencia);
                }
                catch
                {
                    throw;
                }
            }
            return retorno;
        }

        public void ValidarEnviarEmissaoComplementosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;
            repCargaOcorrencia.Atualizar(cargaOcorrencia);
        }

        public void VerificarCancelamentoCTesComplementaresOcorrencia(int codigoCancelamentoOcorrencia, Repositorio.UnitOfWork unitOfWork, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoOcorrenciaCancelamento servicoIntegracaoOcorrenciaCancelamento = new Servicos.Embarcador.Integracao.IntegracaoOcorrenciaCancelamento(unitOfWork);
            Hubs.Ocorrencia servicoNotificacaoOcorrencia = new Hubs.Ocorrencia();
            bool transacaoAberta = false;
            bool notificarFrontend = false;

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cancelamentoOcorrencia = repOcorrenciaCancelamento.BuscarPorCodigo(codigoCancelamentoOcorrencia);

            try
            {

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(cancelamentoOcorrencia.Ocorrencia.Codigo, false);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamento = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
                bool todosCancelados = true;

                if (cancelamentoOcorrencia.EnviouCTesParaCancelamento)
                {
                    if (cargaCTesComplementaresInfo.Any(o =>
                        o.CTe.CargaCTes.Any(c => c.SistemaEmissor == SistemaEmissor.MultiCTe) &&
                        (o.CTe.ModeloDocumentoFiscal.Numero == "57" || o.CTe.ModeloDocumentoFiscal.Numero == "39") &&
                        (o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.EmCancelamento || o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.EmInutilizacao)
                    ))
                        todosCancelados = false;
                    else if (!cancelamentoOcorrencia.LiberarCancelamentoComCTeNaoInutilizado)
                    {
                        if (cargaCTesComplementaresInfo.Any(o => o.CTe.CargaCTes.Any(c => c.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && (o.CTe.ModeloDocumentoFiscal.Numero == "57" || o.CTe.ModeloDocumentoFiscal.Numero == "39") && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Anulado && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada)))
                            throw new ServicoException("Não foi possível cancelar/inutilizar todos os CT-es.");
                    }
                    else
                    {
                        bool possuiCteNãoCancelado = cargaCTesComplementaresInfo.Any(o =>
                            o.CTe.CargaCTes.Any(c =>
                                (c.SistemaEmissor == SistemaEmissor.MultiCTe) &&
                                (o.CTe.ModeloDocumentoFiscal.Numero == "57" || o.CTe.ModeloDocumentoFiscal.Numero == "39") &&
                                (o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Cancelada) &&
                                (o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Inutilizada) &&
                                (o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Anulado) &&
                                (o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Denegada) &&
                                (o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Rejeitada || (o.CTe.MensagemStatus == null || !o.CTe.MensagemStatus.PermiteLiberarSemInutilizacao))
                            )
                        );

                        if (possuiCteNãoCancelado)
                            throw new ServicoException("Não foi possível cancelar/inutilizar todos os CT-es.");
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTesComplementoInfo in cargaCTesComplementaresInfo)
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamentoCTe = repTituloDocumento.BuscarTitulosEmAbertoPorCTe(cargaCTesComplementoInfo.CTe.Codigo);

                        if (titulosParaCancelamentoCTe.Count > 0)
                            titulosParaCancelamento.AddRange(titulosParaCancelamentoCTe);
                    }
                }
                else
                {
                    Servicos.CTe serCTE = new Servicos.CTe(unitOfWork);
                    Servicos.NFSe serNFSe = new Servicos.NFSe(unitOfWork);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTesComplementoInfo in cargaCTesComplementaresInfo)
                    {
                        List<int> numerosFaturas = repFaturaDocumento.BuscarNumeroFaturaPorCTe(cargaCTesComplementoInfo.CTe.Codigo);

                        if (numerosFaturas.Count > 0)
                            throw new ServicoException($"O CT-e ({cargaCTesComplementoInfo.CTe.Numero}) está vinculado à(s) fatura(s) nº {string.Join(", ", numerosFaturas)}, não sendo possível realizar o cancelamento/anulação.");

                        List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorCTe(cargaCTesComplementoInfo.CTe.Codigo);

                        if (numerosTitulos.Count > 0)
                            throw new ServicoException($"O CT-e ({cargaCTesComplementoInfo.CTe.Numero}) está vinculado ao(s) título(s) nº {string.Join(", ", numerosTitulos)}, não sendo possível realizar o cancelamento/anulação.");

                        List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorCarga(cargaCTesComplementoInfo.CTe.Codigo);

                        if (nossoNumeroBoletoTitulos.Count > 0)
                            throw new ServicoException($"O CT-e ({cargaCTesComplementoInfo.CTe.Numero}) está vinculado a boleto(s) no(s) título(s) nº {string.Join(", ", nossoNumeroBoletoTitulos)}, não sendo possível realizar o cancelamento/anulação.");

                        if (cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57" || cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "39")
                        {
                            if (cargaCTesComplementoInfo.CTe.Status == "A")
                            {
                                if (cancelamentoOcorrencia.Tipo == TipoCancelamentoOcorrencia.Cancelamento)
                                {
                                    if (cargaCTesComplementoInfo.CTe.CargaCTes.Any(obj => obj.SistemaEmissor == SistemaEmissor.MultiCTe))
                                    {
                                        if (cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57" && cancelamentoOcorrencia.Ocorrencia.Carga != null)
                                        {
                                            if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cargaCTesComplementoInfo.CTe.SistemaEmissor).CancelarCte(cargaCTesComplementoInfo.CTe.Codigo, cargaCTesComplementoInfo.CTe.Empresa.Codigo, cancelamentoOcorrencia.MotivoCancelamento, unitOfWork))
                                                throw new ServicoException($"Não foi possível cancelar o CT-e {cargaCTesComplementoInfo.CTe.Numero}.");

                                            servicoNotificacaoOcorrencia.InformarCancelamentoDocumentoAtualizado(cancelamentoOcorrencia.Codigo);
                                        }
                                        else if (cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "39")
                                        {
                                            if (!serNFSe.CancelarNFSe(cargaCTesComplementoInfo.CTe.Codigo, unitOfWork))
                                                throw new ServicoException($"Não foi possível cancelar a NFS-e {cargaCTesComplementoInfo.CTe.Numero}.");

                                            servicoNotificacaoOcorrencia.InformarCancelamentoDocumentoAtualizado(cancelamentoOcorrencia.Codigo);
                                        }
                                    }
                                    else if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
                                    {
                                        if (cancelamentoOcorrencia.Ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgConfirmacaoUso)//quando o usuário não aprova uma ocorrencia gerada nos CT-e importados, deve permitir o cancelamento da ocorrencia e liberar os CT-es importados para outra carga, sem cancelar os CT-e, já que os mesmos devem ser cancelados sempre no Embarcador.
                                            throw new ServicoException("Não é possível cancelar CT-es foram emitidos e importados de outro sistema emissor.");
                                    }
                                }
                                else if (cancelamentoOcorrencia.Tipo == TipoCancelamentoOcorrencia.Anulacao)
                                {
                                    if (cargaCTesComplementoInfo.CTe.CargaCTes.Any(obj => obj.SistemaEmissor == SistemaEmissor.MultiCTe))
                                    {
                                        if ((cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57" || cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "39") && cancelamentoOcorrencia.Ocorrencia.Carga != null)
                                        {
                                            cargaCTesComplementoInfo.CTe.Status = "Z";
                                            cargaCTesComplementoInfo.CTe.DataRetornoSefaz = DateTime.Now;
                                            cargaCTesComplementoInfo.CTe.DataAnulacao = DateTime.Now;
                                            cargaCTesComplementoInfo.CTe.ObservacaoCancelamento = cancelamentoOcorrencia.MotivoCancelamento;

                                            repConhecimentoDeTransporteEletronico.Atualizar(cargaCTesComplementoInfo.CTe);
                                            servicoNotificacaoOcorrencia.InformarCancelamentoDocumentoAtualizado(cancelamentoOcorrencia.Codigo);
                                        }
                                    }
                                    else if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
                                    {
                                        if (cancelamentoOcorrencia.Ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgConfirmacaoUso)//quando o usuário não aprova uma ocorrencia gerada nos CT-e importados, deve permitir o cancelamento da ocorrencia e liberar os CT-es importados para outra carga, sem cancelar os CT-e, já que os mesmos devem ser cancelados sempre no Embarcador.
                                            throw new ServicoException("Não é possível anular CT-es foram emitidos e importados de outro sistema emissor.");
                                    }
                                }
                            }
                            else if (cargaCTesComplementoInfo.CTe.Status == "R" && cancelamentoOcorrencia.Ocorrencia.Carga != null)
                            {
                                if (cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57")
                                {
                                    if (!cancelamentoOcorrencia.LiberarCancelamentoComCTeNaoInutilizado)
                                    {
                                        if (!serCTE.Inutilizar(cargaCTesComplementoInfo.CTe.Codigo, cargaCTesComplementoInfo.CTe.Empresa.Codigo, cancelamentoOcorrencia.MotivoCancelamento, tipoServicoMultisoftware, unitOfWork))
                                            throw new ServicoException($"Não foi possível inutilizar o CT-e {cargaCTesComplementoInfo.CTe.Numero}.");

                                        servicoNotificacaoOcorrencia.InformarCancelamentoDocumentoAtualizado(cancelamentoOcorrencia.Codigo);
                                    }
                                }
                                else if (cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "39")
                                {
                                    cargaCTesComplementoInfo.CTe.Status = "I";
                                    cargaCTesComplementoInfo.CTe.ObservacaoCancelamento = cancelamentoOcorrencia.MotivoCancelamento;

                                    repConhecimentoDeTransporteEletronico.Atualizar(cargaCTesComplementoInfo.CTe);
                                    servicoNotificacaoOcorrencia.InformarCancelamentoDocumentoAtualizado(cancelamentoOcorrencia.Codigo);
                                }
                            }
                        }
                        else
                        {
                            DateTime dataCancelamento = DateTime.Now;

                            cargaCTesComplementoInfo.CTe.Status = "C";
                            cargaCTesComplementoInfo.CTe.DataRetornoSefaz = dataCancelamento;
                            cargaCTesComplementoInfo.CTe.DataCancelamento = dataCancelamento;

                            repConhecimentoDeTransporteEletronico.Atualizar(cargaCTesComplementoInfo.CTe);
                        }
                    }
                }

                unitOfWork.Start();

                transacaoAberta = true;

                if (todosCancelados && cancelamentoOcorrencia.EnviouCTesParaCancelamento)
                {
                    bool possuiIntegracao = servicoIntegracaoOcorrenciaCancelamento.AdicionarIntegracoesCancelamento(cancelamentoOcorrencia);
                    Financeiro.Titulo.GerarCancelamentoAutomaticoTitulosEmAberto(titulosParaCancelamento, $"Cancelamento do título gerado automaticamente à partir do cancelamento da ocorrência {cancelamentoOcorrencia.Ocorrencia.NumeroOcorrencia}.", tipoServicoMultisoftware, unitOfWork);
                    RemoverEGerarMovimentacaoDosCTesEmitidosPorOutroSistema(cancelamentoOcorrencia, unitOfWork, tipoServicoMultisoftware, StringConexao);
                    CancelarOcorrencia(cancelamentoOcorrencia, unitOfWork, tipoServicoMultisoftware, StringConexao);

                    if (possuiIntegracao && (cancelamentoOcorrencia.Situacao == SituacaoCancelamentoOcorrencia.Cancelada))
                        cancelamentoOcorrencia.Situacao = SituacaoCancelamentoOcorrencia.AguardandoIntegracao;

                    notificarFrontend = true;
                }
                else
                    cancelamentoOcorrencia.EnviouCTesParaCancelamento = true;
            }
            catch (ServicoException excecao)
            {
                cancelamentoOcorrencia.MensagemRejeicaoCancelamento = excecao.Message;
                cancelamentoOcorrencia.Situacao = SituacaoCancelamentoOcorrencia.RejeicaoCancelamento;
                cancelamentoOcorrencia.EnviouCTesParaCancelamento = false;

                notificarFrontend = true;
            }

            repOcorrenciaCancelamento.Atualizar(cancelamentoOcorrencia);

            if (transacaoAberta)
                unitOfWork.CommitChanges();

            if (notificarFrontend)
                servicoNotificacaoOcorrencia.InformarCancelamentoAtualizado(cancelamentoOcorrencia.Codigo);
        }

        public static bool VerificarSeGeraTituloAutomaticamente(Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unidadeTrabalho, out bool gerarFaturamentoAVista, out bool geraBoletoAutomaticamente, out int codigoBoletoConfiguracao, out bool enviarBoletoPorEmailAutomaticamente, out bool enviarDocumentacaoFaturamentoCTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            codigoBoletoConfiguracao = 0;
            enviarBoletoPorEmailAutomaticamente = false;
            enviarDocumentacaoFaturamentoCTe = false;
            gerarFaturamentoAVista = false;
            geraBoletoAutomaticamente = false;

            if (cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.PedidoTipoPagamento != null && cargaPedido.Pedido.PedidoTipoPagamento.FormaPagamento.HasValue)
            {
                if (cargaPedido.Pedido.PedidoTipoPagamento.FormaPagamento.Value == FormaPagamento.Avista)
                {
                    gerarFaturamentoAVista = true;
                    return true;
                }
                else if (cargaPedido.Pedido.PedidoTipoPagamento.FormaPagamento.Value == FormaPagamento.GerarTituloAutomaticamente)
                    return true;

            }

            if (tomador != null)
            {
                if (cargaPedido.Carga.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false && cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura != null)
                {
                    geraBoletoAutomaticamente = cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.GerarBoletoAutomaticamente ?? false;
                    enviarBoletoPorEmailAutomaticamente = cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.EnviarBoletoPorEmailAutomaticamente ?? false;
                    enviarDocumentacaoFaturamentoCTe = cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.EnviarDocumentacaoFaturamentoCTe ?? false;
                    codigoBoletoConfiguracao = cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.BoletoConfiguracao?.Codigo ?? 0;
                    if (cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.GerarFaturamentoAVista ?? false)
                    {
                        gerarFaturamentoAVista = true;
                        return true;
                    }
                    else
                        return cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.GerarTituloAutomaticamente ?? false;
                }
                else if (tomador.NaoUsarConfiguracaoFaturaGrupo || tomador.GrupoPessoas == null)
                {
                    geraBoletoAutomaticamente = tomador.GerarBoletoAutomaticamente;
                    enviarBoletoPorEmailAutomaticamente = tomador.EnviarBoletoPorEmailAutomaticamente;
                    enviarDocumentacaoFaturamentoCTe = tomador.EnviarDocumentacaoFaturamentoCTe;
                    codigoBoletoConfiguracao = tomador.BoletoConfiguracao?.Codigo ?? 0;
                    if (tomador.GerarFaturamentoAVista)
                    {
                        gerarFaturamentoAVista = true;
                        return true;
                    }
                    else
                        return tomador.GerarTituloAutomaticamente;
                }
                else
                {
                    geraBoletoAutomaticamente = tomador.GrupoPessoas.GerarBoletoAutomaticamente;
                    enviarBoletoPorEmailAutomaticamente = tomador.GrupoPessoas.EnviarBoletoPorEmailAutomaticamente;
                    enviarDocumentacaoFaturamentoCTe = tomador.GrupoPessoas.EnviarDocumentacaoFaturamentoCTe;
                    codigoBoletoConfiguracao = tomador.GrupoPessoas.BoletoConfiguracao?.Codigo ?? 0;
                    if (tomador.GrupoPessoas.GerarFaturamentoAVista)
                    {
                        gerarFaturamentoAVista = true;
                        return true;
                    }
                    else
                        return tomador.GrupoPessoas.GerarTituloAutomaticamente;
                }
            }

            return false;
        }

        public int CalcularDistanciaCTEsOcorrencia(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> CargaCTes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao config = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork).Buscar();
            Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(config.ServidorRouteOSM);
            decimal distanciaTotal = 0m;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in CargaCTes)
            {
                if (cargaCTe.CTe.Distancia > 0)
                {
                    distanciaTotal += cargaCTe.CTe.Distancia;
                    continue;
                }

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint origem;
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint destino;

                if (cargaCTe.CTe?.Remetente?.Cliente != null && !string.IsNullOrEmpty(cargaCTe.CTe.Remetente.Cliente.Latitude) && cargaCTe.CTe?.Destinatario?.Cliente != null && !string.IsNullOrEmpty(cargaCTe.CTe.Destinatario.Cliente.Latitude))
                {
                    origem = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(cargaCTe.CTe.Remetente.Cliente.Latitude, cargaCTe.CTe.Remetente.Cliente.Longitude);
                    destino = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(cargaCTe.CTe.Destinatario.Cliente.Latitude, cargaCTe.CTe.Destinatario.Cliente.Longitude);
                }
                else
                {
                    origem = ObterWayPoint(cargaCTe.CTe.LocalidadeInicioPrestacao);
                    destino = ObterWayPoint(cargaCTe.CTe.LocalidadeTerminoPrestacao);
                }


                if ((origem.Lat == 0 && origem.Lng == 0) || (destino.Lat == 0 && destino.Lng == 0))
                    continue;

                try
                {
                    rota.Add(origem);
                    rota.Add(destino);
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao percurso = rota.Roteirizar();

                    cargaCTe.CTe.Distancia = percurso.Distancia;
                    distanciaTotal += percurso.Distancia;

                    repCTe.Atualizar(cargaCTe.CTe);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    rota.Clear();
                }
            }

            return (int)distanciaTotal;
        }

        public static void GerarOcorrenciaAutomaticaSemTabelaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrenciaSemTabelaFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool transacaoIniciada = unitOfWork.IsActiveTransaction();
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            try
            {
                bool existeOcorrencia = repOcorrencia.ContemOcorrenciaCargaTipoOcorrencia(carga.Codigo, tipoOcorrenciaSemTabelaFrete.Codigo);

                if (!existeOcorrencia)
                {
                    var cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
                    {
                        Carga = carga,
                        DataAlteracao = DateTime.Now,
                        DataOcorrencia = DateTime.Now,
                        DataFinalizacaoEmissaoOcorrencia = DateTime.Now,
                        NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                        Observacao = "Ocorrência gerada automaticamente para carga sem tabela de frete.",
                        SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao,
                        ValorOcorrencia = 0,
                        ValorOcorrenciaOriginal = 0,
                        ObservacaoCTe = string.Empty,
                        CTeEmitidoNoEmbarcador = false,
                        TipoOcorrencia = tipoOcorrenciaSemTabelaFrete,
                        ComponenteFrete = tipoOcorrenciaSemTabelaFrete.ComponenteFrete
                    };

                    if (!transacaoIniciada)
                        unitOfWork.Start();
                    repOcorrencia.Inserir(cargaOcorrencia);

                    List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaFiltradaEmissao = Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia.VerificarRegrasAutorizacaoOcorrencia(cargaOcorrencia, EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia, unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao>();

                    if (listaFiltradaEmissao.Count() > 0)
                        Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia.CriarRegrasAutorizacao(listaFiltradaEmissao, cargaOcorrencia, null, out notificoes, tipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork);

                    if (!transacaoIniciada)
                        unitOfWork.CommitChanges();
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (!transacaoIniciada)
                    unitOfWork.Rollback();
            }
        }

        #endregion Métodos Públicos
    }
}

