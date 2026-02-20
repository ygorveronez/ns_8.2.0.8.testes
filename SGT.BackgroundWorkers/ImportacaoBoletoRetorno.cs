using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.BackgroundWorkers
{
    public class ImportacaoBoletoRetorno : LongRunningProcessBase<ImportacaoBoletoRetorno>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            BuscarImportacaoRetorno(_codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _clienteMultisoftware.Codigo);
            BuscarXMLEmail(_codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _clienteMultisoftware.Codigo);
        }

        private void BuscarXMLEmail(int codigoEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, int clienteCodigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                List<int> codigosEmpresas = new List<int>();
                codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasIntegracaoEmail();
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null;
                foreach (int codigoEmpresaEmail in codigosEmpresas)
                {
                    Servicos.Log.TratarErro("Inicio empresa " + codigoEmpresaEmail.ToString(), "XMLEmail");
                    Auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    Auditado.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresaEmail);
                    Auditado.Integradora = null;
                    Auditado.IP = "";
                    Auditado.Texto = "";
                    Auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                    Auditado.Usuario = null;

                    Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.VerificarCaixaDeEntrada(codigoEmpresaEmail, _stringConexao, out string msgErro, Auditado, tipoServicoMultisoftware);
                    Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.VerificarEmails(codigoEmpresaEmail, _stringConexao, out string msgErro2, Auditado, tipoServicoMultisoftware);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "XMLEmail");
            }
            finally
            {
                Servicos.Log.TratarErro("Fim BuscarXMLEmail", "XMLEmail");
                unitOfWork.Dispose();
                unitOfWork = null;
                GC.Collect();
            }
        }

        private void BuscarImportacaoRetorno(int codigoEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, int clienteCodigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);
            try
            {
                Repositorio.Embarcador.Financeiro.BoletoRetornoArquivo repBoletoRetornoArquivo = new Repositorio.Embarcador.Financeiro.BoletoRetornoArquivo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoRetorno repBoletoRetorno = new Repositorio.Embarcador.Financeiro.BoletoRetorno(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo> arquivos = repBoletoRetornoArquivo.BuscarRetornoArquivoConcluido();

                unitOfWork.Start();
                foreach (var arquivo in arquivos)
                {
                    IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto> listaRetornoBoleto = repBoletoRetorno.RelatorioRetornoBoleto(0, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, 0, 0, "", "", "", "", 0, 0, false, false, arquivo.Codigo);
                    var lista = (from obj in listaRetornoBoleto
                                 select new
                                 {
                                     obj.Comando,
                                     obj.NossoNumero,
                                     obj.Banco,
                                     DataVencimento = obj.DataVencimento != null && obj.DataVencimento > DateTime.MinValue ? obj.DataVencimento.ToString("dd/MM/yyyy") : string.Empty,
                                     DataOcorrencia = obj.DataOcorrencia != null && obj.DataOcorrencia > DateTime.MinValue ? obj.DataOcorrencia.ToString("dd/MM/yyyy") : string.Empty,
                                     obj.ValorRetorno,
                                     obj.ValorDocumento,
                                     obj.ValorJuros,
                                     obj.ValorOutrasDespesas,
                                     obj.ValorTarifa,
                                     obj.ValorRecebido,
                                     DataCredito = obj.DataCredito != null && obj.DataCredito > DateTime.MinValue ? obj.DataCredito.ToString("dd/MM/yyyy") : string.Empty,
                                     obj.CodigoRejeicao,
                                     DataBaixa = obj.DataBaixa != null && obj.DataBaixa > DateTime.MinValue ? obj.DataBaixa.ToString("dd/MM/yyyy") : string.Empty,
                                     DataImportacao = obj.DataImportacao != null && obj.DataImportacao > DateTime.MinValue ? obj.DataImportacao.ToString("dd/MM/yyyy") : string.Empty,
                                     obj.CodigoTitulo,
                                     VencimentoTitulo = obj.VencimentoTitulo != null && obj.VencimentoTitulo > DateTime.MinValue ? obj.VencimentoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                                     EmissaoTitulo = obj.EmissaoTitulo != null && obj.EmissaoTitulo > DateTime.MinValue ? obj.EmissaoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                                     obj.ValorTitulo,
                                     obj.NossoNumeroTitulo,
                                     obj.Sequencia,
                                     obj.Cliente,
                                     obj.Empresa,
                                     obj.DescricaoComando
                                 }).ToList();
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && lista.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R176_ArquivoRetornoBoleto, tipoServicoMultisoftware);
                        if (relatorio == null)
                            relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R176_ArquivoRetornoBoleto, tipoServicoMultisoftware, "Relatório de Retorno de Boleto", "Financeiros", "BoletoRetornoArquivo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, arquivo.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                        Task.Factory.StartNew(() => GerarRelatorioRetornoBoleto(codigoEmpresa, arquivo.Codigo, stringConexao, relatorioControleGeracao, listaRetornoBoleto));
                    }

                    arquivo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.ComRemessaEBoleto;
                    repBoletoRetornoArquivo.Atualizar(arquivo);
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
                adminUnitOfWork.Dispose();
            }
        }

        private void GerarRelatorioRetornoBoleto(int codigoEmpresa, int codigoArquivoRetorno, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto> listaRetornoBoleto)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            try
            {
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                ReportRequest.WithType(ReportType.BoletoRetornoArquivo)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("codigoArquivoRetorno", codigoArquivoRetorno)
                    .AddExtraData("listaRetornoBoleto", listaRetornoBoleto.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CaminhoLogoDacte", empresaRelatorio.CaminhoLogoDacte)
                    .CallReport();
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
    }
}