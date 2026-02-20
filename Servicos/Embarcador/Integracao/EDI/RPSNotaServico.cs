using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class RPSNotaServico
    {
        #region Propriedades Globais

        public static bool ThreadExecutada = false;
        public static bool Sucesso = false;
        public static Dominio.Entidades.LayoutEDI LayoutEDI = null;
        public static Dominio.ObjetosDeValor.EDI.RPS.RetornoNotaServico NOTFIS = null;
        public static Stream ArquivoEDI = null;

        #endregion

        public Dominio.ObjetosDeValor.EDI.RPS.NotaServico ConverterRPSNotaServico(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Empresa empresaEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(lancamentoNFSManual.Transportador?.Codigo ?? 0);

            Dominio.ObjetosDeValor.EDI.RPS.NotaServico notaServico = new Dominio.ObjetosDeValor.EDI.RPS.NotaServico();
            //Dominio.ObjetosDeValor.EDI.RPS.Cabecalho cabecalho = new Dominio.ObjetosDeValor.EDI.RPS.Cabecalho();            
            Dominio.ObjetosDeValor.EDI.RPS.Rodape rodape = new Dominio.ObjetosDeValor.EDI.RPS.Rodape();

            PreencherCabecalho(ref notaServico, lancamentoNFSManual);

            Dominio.ObjetosDeValor.EDI.RPS.Detalhe detalhe = new Dominio.ObjetosDeValor.EDI.RPS.Detalhe();
            PreencherDetalhe(ref detalhe, lancamentoNFSManual, transportadorConfiguracaoNFSe);
            notaServico.Detalhes = new List<Dominio.ObjetosDeValor.EDI.RPS.Detalhe>();
            notaServico.Detalhes.Add(detalhe);

            PreencherRodape(ref rodape, notaServico);
            notaServico.Rodape = rodape;

            return notaServico;
        }

        public Dominio.ObjetosDeValor.EDI.RPS.NotaServico ConverterRPSNotaServico(List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentoNFSManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(lancamentoNFSManual.FirstOrDefault().Transportador?.Codigo ?? 0);

            Dominio.ObjetosDeValor.EDI.RPS.NotaServico notaServico = new Dominio.ObjetosDeValor.EDI.RPS.NotaServico();
            Dominio.ObjetosDeValor.EDI.RPS.Rodape rodape = new Dominio.ObjetosDeValor.EDI.RPS.Rodape();

            PreencherCabecalho(ref notaServico, lancamentoNFSManual.FirstOrDefault());
            notaServico.Detalhes = new List<Dominio.ObjetosDeValor.EDI.RPS.Detalhe>();

            foreach (var nfsManual in lancamentoNFSManual)
            {
                Dominio.ObjetosDeValor.EDI.RPS.Detalhe detalhe = new Dominio.ObjetosDeValor.EDI.RPS.Detalhe();
                PreencherDetalhe(ref detalhe, nfsManual, transportadorConfiguracaoNFSe);
                notaServico.Detalhes.Add(detalhe);
            }

            PreencherRodape(ref rodape, notaServico);
            notaServico.Rodape = rodape;

            return notaServico;
        }

        private void PreencherCabecalho(ref Dominio.ObjetosDeValor.EDI.RPS.NotaServico cabecalho, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            cabecalho.InscricaoContribuinte = lancamentoNFSManual.Transportador?.InscricaoMunicipal;
            cabecalho.IdentificacaoRemessa = DateTime.Now.ToString("yyyMMddHHmmss");
        }

        private void PreencherDetalhe(ref Dominio.ObjetosDeValor.EDI.RPS.Detalhe detalhe, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe)
        {
            detalhe.SerieRPS = transportadorConfiguracaoNFSe?.SerieRPS ?? string.Empty;
            detalhe.SerieNFe = lancamentoNFSManual.DadosNFS?.Serie?.Numero.ToString() ?? "";
            if (lancamentoNFSManual.DadosNFS?.NumeroRPS > 0)
                detalhe.NumeroRPS = lancamentoNFSManual.DadosNFS?.NumeroRPS.ToString() ?? "";
            else
                detalhe.NumeroRPS = lancamentoNFSManual.DadosNFS?.Numero.ToString() ?? "";
            detalhe.DataRPS = lancamentoNFSManual.DadosNFS != null && lancamentoNFSManual.DadosNFS.DataEmissao.HasValue ? lancamentoNFSManual.DadosNFS.DataEmissao.Value : DateTime.Now.Date;
            detalhe.HoraRPS = lancamentoNFSManual.DadosNFS != null && lancamentoNFSManual.DadosNFS.DataEmissao.HasValue ? lancamentoNFSManual.DadosNFS.DataEmissao.Value.TimeOfDay : DateTime.Now.TimeOfDay;
            detalhe.LocalServicoPrestado = "1";
            detalhe.EnderecoServicoPrestado = lancamentoNFSManual.Transportador.Endereco;
            detalhe.NumeroServicoPrestado = lancamentoNFSManual.Transportador.Numero;
            detalhe.ComplementoServicoPrestado = lancamentoNFSManual.Transportador.Complemento;
            detalhe.BairroServicoPrestado = lancamentoNFSManual.Transportador.Bairro;
            detalhe.CidadeServicoPrestado = lancamentoNFSManual.Transportador.Localidade.Descricao;
            detalhe.UFServicoPrestado = lancamentoNFSManual.Transportador.Localidade.Estado.Sigla;
            detalhe.CEPServicoPrestado = Utilidades.String.OnlyNumbers(lancamentoNFSManual.Transportador.CEP);
            detalhe.QuantidadeServicoPrestado = 1;
            detalhe.ValorServicoPrestado = lancamentoNFSManual.DadosNFS?.ValorFrete ?? 0;
            detalhe.ValorTotalRetencoes = lancamentoNFSManual.DadosNFS?.ValorRetido ?? 0;
            detalhe.IndicadorCPFCNPJTomador = lancamentoNFSManual.Tomador.Tipo == "F" ? "1" : "2";
            detalhe.CPFCNPJTomador = lancamentoNFSManual.Tomador.CPF_CNPJ_SemFormato;
            detalhe.NomeTomador = lancamentoNFSManual.Tomador.Nome;
            detalhe.EnderecoTomador = lancamentoNFSManual.Tomador.Endereco;
            detalhe.NumeroTomador = lancamentoNFSManual.Tomador.Numero;
            detalhe.ComplementoTomador = lancamentoNFSManual.Tomador.Complemento;
            detalhe.BairroTomador = lancamentoNFSManual.Tomador.Bairro;
            detalhe.CidadeTomador = lancamentoNFSManual.Tomador.Localidade.Descricao;
            detalhe.UFTomador = lancamentoNFSManual.Tomador.Localidade.Estado.Sigla;
            detalhe.CEPTomador = Utilidades.String.OnlyNumbers(lancamentoNFSManual.Tomador.CEP);

            string email = !string.IsNullOrWhiteSpace(lancamentoNFSManual.Tomador.Email) ? lancamentoNFSManual.Tomador.Email : "";
            string[] splitEmail = email.Split(';');
            string emailTomador = "";
            if (lancamentoNFSManual.Transportador?.QuantidadeMaximaEmailRPS > 0 && splitEmail.Length > lancamentoNFSManual.Transportador.QuantidadeMaximaEmailRPS)
                emailTomador = string.Join("|", splitEmail, 0, lancamentoNFSManual.Transportador.QuantidadeMaximaEmailRPS);
            else
                emailTomador = string.Join("|", splitEmail);

            detalhe.EmailTomador = emailTomador;
            detalhe.DiscriminacaoServico = lancamentoNFSManual.DadosNFS?.Observacoes ?? "";
            detalhe.Fatura = "";
            detalhe.ValorFatura = 0;
            detalhe.FormaPagamento = "";

            detalhe.Valores = new List<Dominio.ObjetosDeValor.EDI.RPS.Valore>();
            if (lancamentoNFSManual.DadosNFS != null && lancamentoNFSManual.DadosNFS.ValorIR > 0)
            {
                detalhe.Valores.Add(new Dominio.ObjetosDeValor.EDI.RPS.Valore()
                {
                    CodigoOutrosValores = "01",
                    Valor = lancamentoNFSManual.DadosNFS.ValorIR
                });
            }
            if (lancamentoNFSManual.DadosNFS != null && lancamentoNFSManual.DadosNFS.ValorPIS > 0)
            {
                detalhe.Valores.Add(new Dominio.ObjetosDeValor.EDI.RPS.Valore()
                {
                    CodigoOutrosValores = "02",
                    Valor = lancamentoNFSManual.DadosNFS.ValorPIS
                });
            }
            if (lancamentoNFSManual.DadosNFS != null && lancamentoNFSManual.DadosNFS.ValorCOFINS > 0)
            {
                detalhe.Valores.Add(new Dominio.ObjetosDeValor.EDI.RPS.Valore()
                {
                    CodigoOutrosValores = "03",
                    Valor = lancamentoNFSManual.DadosNFS.ValorCOFINS
                });
            }
            if (lancamentoNFSManual.DadosNFS != null && lancamentoNFSManual.DadosNFS.ValorCSLL > 0)
            {
                detalhe.Valores.Add(new Dominio.ObjetosDeValor.EDI.RPS.Valore()
                {
                    CodigoOutrosValores = "04",
                    Valor = lancamentoNFSManual.DadosNFS.ValorCSLL
                });
            }

        }

        private void PreencherRodape(ref Dominio.ObjetosDeValor.EDI.RPS.Rodape rodape, Dominio.ObjetosDeValor.EDI.RPS.NotaServico notaServico)
        {
            rodape.NumeroLinhas = notaServico.Detalhes.Count();
            rodape.NumeroLinhas += notaServico.Detalhes.Sum(o => o.Valores.Count());
            rodape.NumeroLinhas += 2;
            rodape.ValorTotalServicos = notaServico.Detalhes.Sum(o => o.ValorServicoPrestado);
            rodape.ValorTotalValores = notaServico.Detalhes.Sum(o => o.Valores.Sum(p => p.Valor));
        }

        public string ProcessarRetornoRPSNotaServico(Repositorio.UnitOfWork unitOfWork, System.IO.Stream Stream, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, string stringConexao)
        {
            Stream.Position = 0;

            ThreadExecutada = false;
            NOTFIS = null;
            Sucesso = false;
            LayoutEDI = layoutEDI;
            ArquivoEDI = Stream;

            int executionCount = 0;

            Thread thread = new Thread(GerarEDIRps, 80000000);

            thread.Start();

            while (!ThreadExecutada)
            {
                executionCount++;

                if (executionCount == 20)
                {
                    thread.Abort();
                    return "Ocorreu uma falha ao ler o NOTFIS. Tempo de execução muito longo.";
                }

                System.Threading.Thread.Sleep(500);

                if (ThreadExecutada)
                {
                    if (Sucesso)
                        return ProcessarRetornoNotaServicoNotasFiscaisNOTFIS(unitOfWork, NOTFIS, layoutEDI, auditado, tipoServicoMultisoftware, usuario, stringConexao);
                    else
                        return "Ocorreu uma falha ao ler o NOTFIS.";
                }
            }

            return "Não foi possível ler o NOTFIS, tente novamente.";
        }

        private void GerarEDIRps()
        {
            try
            {
                Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(null, LayoutEDI, ArquivoEDI, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);

                NOTFIS = serLeituraEDI.LerRetornoNotaServico();

                Sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            ThreadExecutada = true;
        }

        private string ProcessarRetornoNotaServicoNotasFiscaisNOTFIS(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.EDI.RPS.RetornoNotaServico NOTFIS, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, string stringConexao)
        {
            if (NOTFIS != null && NOTFIS.Detalhes != null && NOTFIS.Detalhes.Count > 0)
            {
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                foreach (var retornoNFs in NOTFIS.Detalhes)
                {
                    int numero = 0, serie = 0, numeroRPS = 0;
                    int.TryParse(retornoNFs.NumeroNFe, out numero);
                    int.TryParse(retornoNFs.SerieNFe, out serie);
                    int.TryParse(retornoNFs.NumeroRPS, out numeroRPS);

                    double cnpjCPF = 0;
                    double.TryParse(retornoNFs.CPFCNPJTomador, out cnpjCPF);
                    string status = retornoNFs.SituacaoNFe;
                    string codigoAutencidade = retornoNFs.CodigoAutencidade;
                    if (numero > 0 && cnpjCPF > 0 && status == "A" && !string.IsNullOrWhiteSpace(codigoAutencidade))
                    {
                        Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorNumeroTomadorSituacao(numero, cnpjCPF, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota);
                        if (nfsManual == null && numeroRPS > 0)
                            nfsManual = repLancamentoNFSManual.BuscarPorNumeroRPSTomadorSituacao(numeroRPS, cnpjCPF, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota);
                        // Valida
                        if (nfsManual != null)
                        {
                            // Busca as regras
                            bool possuiRegras = Servicos.Embarcador.NFSe.NFSManual.VerificarRegrasAutorizacaoNFS(nfsManual, tipoServicoMultisoftware, unitOfWork);
                            if (!possuiRegras)
                                nfsManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.SemRegra;
                            else
                            {
                                nfsManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgAprovacao;
                                Servicos.Embarcador.NFSe.NFSManual.VerificarSituacaoNFS(nfsManual, unitOfWork, tipoServicoMultisoftware, stringConexao, usuario);
                            }

                            List<Dominio.Entidades.NFSe> nfsMultiCTe = (from o in nfsManual.Documentos where o.DocumentosNFSe != null select o.DocumentosNFSe.NFSe).Distinct().ToList();
                            foreach (Dominio.Entidades.NFSe nota in nfsMultiCTe)
                            {
                                nota.Status = Dominio.Enumeradores.StatusNFSe.AgAprovacaoNFSeManual;
                                repNFSe.Atualizar(nota);
                            }

                            // Persiste dados
                            Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFSManual = repDadosNFSManual.BuscarPorCodigo(nfsManual.DadosNFS?.Codigo ?? 0);
                            if (dadosNFSManual != null)
                            {
                                Dominio.Entidades.EmpresaSerie empresaSerie = repEmpresaSerie.BuscarPorSerie(nfsManual.Transportador.Codigo, serie, Dominio.Enumeradores.TipoSerie.NFSe);
                                if (empresaSerie == null && serie > 0)
                                {
                                    empresaSerie = new Dominio.Entidades.EmpresaSerie()
                                    {
                                        Empresa = nfsManual.Transportador,
                                        Numero = serie,
                                        Status = "A",
                                        Tipo = Dominio.Enumeradores.TipoSerie.NFSe,
                                    };
                                    repEmpresaSerie.Inserir(empresaSerie);
                                }

                                dadosNFSManual.Numero = numero;
                                if (empresaSerie != null)
                                    dadosNFSManual.Serie = empresaSerie;
                                dadosNFSManual.CodigoAutenticidade = codigoAutencidade;
                                repDadosNFSManual.Atualizar(dadosNFSManual);
                            }
                            repLancamentoNFSManual.Atualizar(nfsManual);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, nfsManual, null, "Solicitou Emissão da NFS-e via importação de EDI da prefeitura.", unitOfWork);
                            // Integracao com SignalR
                            svcNFSManual.InformarLancamentoNFSManualAtualizada(nfsManual.Codigo);
                        }
                    }
                }
                return "";
            }
            else
                return "Não foi possível ler o arquivo enviado, tente novamente.";
        }
    }
}
