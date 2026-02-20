using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.NFSe
{
    public class NFSAutomatica
    {
        #region Métodos Públicos
        public static void GerarNFSAutomatica(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();
                Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                List<int> empresas = repEmpresa.BuscarEmpresasConfiguradasEmissaoNFSAutomatica();

                if (empresas == null || empresas.Count <= 0)
                    return;

                for (int i = 0; i < empresas.Count; i++)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(empresas[i]);

                    bool isFinalMes = false;
                    if (empresa.PeriodicidadeEmissaoNFSManual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal && DateTime.Now.IsLastDayOfMonth())
                        isFinalMes = empresa.DiaMesEmissaoNFSManual == 28 || empresa.DiaMesEmissaoNFSManual == 29 || empresa.DiaMesEmissaoNFSManual == 30 || empresa.DiaMesEmissaoNFSManual == 31;

                    if (empresa.PeriodicidadeEmissaoNFSManual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Diario
                        || (empresa.PeriodicidadeEmissaoNFSManual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal && (int)DateTime.Now.DayOfWeek == ((int)empresa.DiaSemanaEmissaoNFSManual - 1))
                        || (empresa.PeriodicidadeEmissaoNFSManual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal && (DateTime.Now.Day == empresa.DiaMesEmissaoNFSManual || isFinalMes))
                        )
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual()
                        {
                            CodigoTransportador = empresas[i]
                        };
                        Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                        {
                            InicioRegistros = 0,
                            LimiteRegistros = 0
                        };
                        int countRegistro = repositorioCargaDocumentoParaEmissaoNFSManual.ContarConsultaSelecaoNFSManual(filtrosPesquisa);

                        if (countRegistro > 0)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaDocumentos = repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarSelecaoNFSManual(filtrosPesquisa, parametrosConsulta);
                            List<double> cnpjTomadores = listaDocumentos.Select(c => c.Tomador.CPF_CNPJ).Distinct().ToList();

                            foreach (var cnpjTomador in cnpjTomadores)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = listaDocumentos.Where(c => c.Tomador.CPF_CNPJ == cnpjTomador).ToList();
                                if (documentos != null && documentos.Count > 0)
                                {
                                    unitOfWork.Start();

                                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                                    Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);

                                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();
                                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                                    if (configuracaoCargaEmissaoDocumento?.NaoPermitirNFSComMultiplosCentrosResultado ?? false)
                                    {
                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = documentos.Where(o => o.PedidoXMLNotaFiscal != null && o.PedidoCTeParaSubContratacao == null).Select(o => o.PedidoXMLNotaFiscal.CargaPedido).Distinct().ToList();
                                        cargaPedidos.AddRange(documentos.Where(o => o.PedidoCTeParaSubContratacao != null && o.PedidoXMLNotaFiscal == null && o.CargaCTe == null).Select(o => o.PedidoCTeParaSubContratacao.CargaPedido).Distinct().ToList());
                                        cargaPedidos.AddRange(documentos.Where(o => o.CargaCTe != null && o.PedidoXMLNotaFiscal == null && o.PedidoCTeParaSubContratacao == null).Select(o => o.CargaCTe.NotasFiscais.Select(p => p.PedidoXMLNotaFiscal.CargaPedido)).SelectMany(o => o).Distinct().ToList());
                                    }

                                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual referencia = documentos[0];
                                    Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = new Dominio.Entidades.Embarcador.NFS.DadosNFSManual()
                                    {
                                        ValorFrete = documentos.Sum(obj => obj.ValorFrete),
                                        NumeroRPS = repDadosNFSManual.BuscarProximoNumeroRPS(),
                                        ValorTotalMoeda = documentos.Sum(obj => obj.ValorTotalMoeda ?? 0m),
                                        Moeda = MoedaCotacaoBancoCentral.Real
                                    };

                                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                    {
                                        if (configuracaoTMS.PadraoInclusaiISSDesmarcado)
                                            dadosNFS.IncluirISSBC = false;
                                        else
                                            dadosNFS.IncluirISSBC = true;
                                        if (documentos != null && documentos.Count > 0)
                                        {
                                            if (configuracaoTMS.PossuiWMS)
                                                dadosNFS.Observacoes = "Minutas " + string.Join(" ", (from o in documentos select o.Numero.ToString("D")).ToList());
                                            else
                                                dadosNFS.Observacoes = "DESCARGA DE MERCADORIA MIN.: " + string.Join(", ", (from o in documentos select o.Numero.ToString("D")).ToList());
                                        }
                                    }

                                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual()
                                    {
                                        Transportador = referencia.Carga.Empresa,
                                        Situacao = SituacaoLancamentoNFSManual.DadosNota,
                                        Tomador = referencia.Tomador,
                                        Filial = referencia.Carga.Filial,
                                        TipoOperacao = referencia.Carga.TipoOperacao,
                                        FechamentoFrete = referencia.FechamentoFrete,
                                        DadosNFS = dadosNFS,
                                        LocalidadePrestacao = referencia.LocalidadePrestacao,
                                        NFSResidual = referencia.DocResidual,
                                        Usuario = referencia.Carga.Operador,
                                        DataCriacao = DateTime.Now,
                                        CargasMultiCTe = (from o in documentos where o.DocumentosNFSe != null select o).Count() > 0,
                                        CodigoServico = string.Empty,
                                        NFSEmitidaAutomaticamente = true
                                    };

                                    //consultar aqui os dados de aliquota                                    
                                    Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, nfsManual.Tomador.Localidade?.Estado?.Sigla ?? "", nfsManual.Tomador.GrupoPessoas?.Codigo ?? 0, nfsManual.Tomador.Localidade?.Codigo ?? 0);

                                    if (configuracaoNFSe == null)
                                        configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, nfsManual.Tomador.Localidade?.Estado?.Sigla ?? "", 0, 0);
                                    if (configuracaoNFSe == null)
                                        configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, "", nfsManual.Tomador.GrupoPessoas?.Codigo ?? 0, 0);
                                    if (configuracaoNFSe == null)
                                        configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, "", 0, nfsManual.Tomador.Localidade?.Codigo ?? 0);
                                    if (configuracaoNFSe == null)
                                        configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, "", 0, 0);
                                    if (configuracaoNFSe == null)
                                        configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, 0, "", 0, 0);

                                    if (configuracaoNFSe != null)
                                    {
                                        Dominio.Entidades.EmpresaSerie empresaSerie = repEmpresaSerie.BuscarPorEmpresaTipo(nfsManual.Transportador.Codigo, Dominio.Enumeradores.TipoSerie.NFSe);

                                        dadosNFS.Observacoes += " " + (configuracaoNFSe?.ServicoNFSe?.Descricao ?? string.Empty);
                                        dadosNFS.AliquotaISS = configuracaoNFSe.AliquotaISS;
                                        dadosNFS.PercentualRetencao = configuracaoNFSe.RetencaoISS;
                                        dadosNFS.IncluirISSBC = configuracaoNFSe.IncluirISSBaseCalculo;
                                        dadosNFS.DataEmissao = DateTime.Now;
                                        dadosNFS.Serie = configuracaoNFSe.SerieNFSe != null ? configuracaoNFSe.SerieNFSe : empresaSerie;

                                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                            dadosNFS.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFS);
                                        else
                                        {
                                            dadosNFS.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);
                                            dadosNFS.Numero = repDadosNFSManual.BuscarProximoNumero(dadosNFS.Serie.Codigo, dadosNFS.ModeloDocumentoFiscal.Codigo, nfsManual.Transportador.Codigo);
                                        }

                                        dadosNFS.ValorRetido = 0m;
                                        dadosNFS.ValorCOFINS = 0m;
                                        dadosNFS.ValorCSLL = 0m;
                                        dadosNFS.ValorPIS = 0m;
                                        dadosNFS.ValorIR = 0m;
                                        nfsManual.CodigoServico = (configuracaoNFSe?.ServicoNFSe?.CodigoTributacao ?? string.Empty);
                                    }
                                    else
                                        dadosNFS.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFS);

                                    if (!string.IsNullOrWhiteSpace(dadosNFS.Observacoes))
                                        dadosNFS.Observacoes = dadosNFS.Observacoes.Trim();

                                    // Persiste dados
                                    repDadosNFSManual.Inserir(dadosNFS);
                                    repLancamentoNFSManual.Inserir(nfsManual, auditado);

                                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento in documentos)
                                    {
                                        documento.LancamentoNFSManual = nfsManual;
                                        repCargaDocumentoParaEmissaoNFSManual.Atualizar(documento);
                                        Servicos.Auditoria.Auditoria.Auditar(auditado, documento, null, "Criou uma NFS-e Automática com o documento.", unitOfWork);
                                    }

                                    AdicionarDescontos(empresas[i], nfsManual, configuracaoEmbarcador, unitOfWork);
                                    Servicos.Embarcador.NFSe.NFSManual.CalcularValores(dadosNFS, unitOfWork);
                                    Servicos.Embarcador.NFSe.NFSManual.CalcularISS(dadosNFS);
                                    decimal baseCalculoIBSCBS = dadosNFS.ValorFrete;

                                    if (dadosNFS.IncluirISSBC)
                                        baseCalculoIBSCBS += dadosNFS.ValorISS;

                                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                                    {
                                        BaseCalculo = baseCalculoIBSCBS,
                                        ValoAbaterBaseCalculo = dadosNFS.ValorISS,
                                        CodigoLocalidade = nfsManual.Tomador.Localidade.Codigo,
                                        SiglaUF = nfsManual.Tomador.Localidade.Estado.Sigla,
                                        CodigoTipoOperacao = 0,
                                        Empresa = empresa
                                    });

                                    dadosNFS.NBS = configuracaoNFSe?.ServicoNFSe?.NBS ?? "";
                                    dadosNFS.IndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
                                    dadosNFS.CSTIBSCBS = impostoIBSCBS.CST;
                                    dadosNFS.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
                                    dadosNFS.BaseCalculoIBSCBS = baseCalculoIBSCBS;
                                    dadosNFS.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                                    dadosNFS.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                                    dadosNFS.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                                    dadosNFS.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                                    dadosNFS.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                                    dadosNFS.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                                    dadosNFS.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                                    dadosNFS.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                                    dadosNFS.ValorCBS = impostoIBSCBS.ValorCBS;

                                    repDadosNFSManual.Atualizar(dadosNFS);

                                    EmitirNFSe(nfsManual.Codigo, unitOfWork, auditado, tipoServicoMultisoftware);

                                    // Integracao com SignalR
                                    svcNFSManual.InformarLancamentoNFSManualAtualizada(nfsManual.Codigo);

                                    unitOfWork.CommitChanges();
                                }
                            }
                        }
                    }

                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }

        #endregion

        #region Métodos Privados

        private static void AdicionarDescontos(int codigoEmpresa, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            //descontos residuais só podem ser aplicados em notas com valores residuais positivos.
            if (!lancamentoNFSManual.NFSResidual)
                return;

            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual()
            {
                CodigoTransportador = codigoEmpresa
            };
            Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioLancamentoNFSManualDesconto.BuscarCargasParaLancamentoNFSManualDesconto(filtrosPesquisa);
            decimal ValorFreteBruto = lancamentoNFSManual.DadosNFS.ValorFrete;
            decimal valorDescontos = 0m;

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                decimal desconto = -(valorDescontos + carga.ValorFreteResidual);
                if (desconto <= lancamentoNFSManual.DadosNFS.ValorFrete)//se o valor do desconto for superior ao valor da nota não adiciona na nota, será utilizado em outra nota.
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto lancamentoNFSManualDesconto = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto()
                    {
                        Carga = carga,
                        LancamentoNFSManual = lancamentoNFSManual
                    };

                    repositorioLancamentoNFSManualDesconto.Inserir(lancamentoNFSManualDesconto);
                    valorDescontos += carga.ValorFreteResidual;
                }
            }

            valorDescontos = -valorDescontos;

            lancamentoNFSManual.DadosNFS.ValorFrete -= valorDescontos;
            lancamentoNFSManual.DadosNFS.ValorDescontos = valorDescontos;

            if (lancamentoNFSManual.DadosNFS.ValorFrete <= 0)
                lancamentoNFSManual.DadosNFS.ValorFrete = 1m;
        }

        private static void EmitirNFSe(int codigoNFs, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

            Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigoNFs);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = repCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentoNFsManual(codigoNFs);

            // Valida
            if (nfsManual == null)
                return;

            nfsManual.MensagemRetornoNFSAutomaticamente = string.Empty;

            if (nfsManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota)
                nfsManual.MensagemRetornoNFSAutomaticamente += "A situação do lançamento não permite essa operação. ";

            // Valida entidade
            if (!ValidaEntidade(nfsManual.DadosNFS, out string erro, tipoServicoMultisoftware))
                nfsManual.MensagemRetornoNFSAutomaticamente += erro + " ";

            if (nfsManual.DadosNFS == null || nfsManual.DadosNFS.Serie == null || nfsManual.DadosNFS.ModeloDocumentoFiscal == null || nfsManual.Transportador == null)
                nfsManual.MensagemRetornoNFSAutomaticamente += "NFS não possui todos os campos informados. ";

            if (repLancamentoNFSManual.ExisteNFSHabilitadaComMesmoNumero(nfsManual.DadosNFS.Numero, nfsManual.DadosNFS.Serie.Numero, nfsManual.DadosNFS.ModeloDocumentoFiscal.Codigo, nfsManual.Transportador.Codigo))
                nfsManual.MensagemRetornoNFSAutomaticamente += "Existe uma NFS habilitada com esta mesma numeração, não sendo possível realizar a emissão da mesma. ";

            if (string.IsNullOrWhiteSpace(nfsManual.MensagemRetornoNFSAutomaticamente))
            {
                nfsManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgEmissao;
                List<Dominio.Entidades.NFSe> nfsMultiCTe = (from o in documentos where o.DocumentosNFSe != null select o.DocumentosNFSe.NFSe).Distinct().ToList();

                foreach (Dominio.Entidades.NFSe nota in nfsMultiCTe)
                {
                    if (nota != null)
                    {
                        nota.Status = Dominio.Enumeradores.StatusNFSe.AgAprovacaoNFSeManual;
                        repNFSe.Atualizar(nota);
                    }
                }
            }

            // Persiste dados
            repLancamentoNFSManual.Atualizar(nfsManual);
            Servicos.Auditoria.Auditoria.Auditar(auditado, nfsManual, null, "Solicitou Emissão da NFS-e.", unitOfWork);
            // Integracao com SignalR
            svcNFSManual.InformarLancamentoNFSManualAtualizada(nfsManual.Codigo);
        }

        private static bool ValidaEntidade(Dominio.Entidades.Embarcador.NFS.DadosNFSManual dados, out string msgErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            //if (dados.Numero == 0)
            //{
            //    msgErro = "Número é obrigatório.";
            //    return false;
            //}

            if (dados.Serie == null)
            {
                msgErro = "Série é obrigatório.";
                return false;
            }

            if (dados.ValorFrete == 0)
            {
                msgErro = "Valor Prestação do Serviço é obrigatório.";
                return false;
            }

            if (dados.AliquotaISS == 0 && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (!dados.Moeda.HasValue || dados.Moeda == MoedaCotacaoBancoCentral.Real))
            {
                msgErro = "Aliquota ISS é obrigatório.";
                return false;
            }

            /*if (dados.ValorISS == 0)
            {
                msgErro = "Valor ISS é obrigatório.";
                return false;
            }

            if (dados.ValorBaseCalculo == 0)
            {
                msgErro = "Base de Cálculo é obrigatório.";
                return false;
            }*/

            //if (dados.PercentualRetencao == 0)
            //{
            //    msgErro = "Percentual de Retenção é obrigatório.";
            //    return false;
            //}

            return true;
        }

        #endregion
    }
}
