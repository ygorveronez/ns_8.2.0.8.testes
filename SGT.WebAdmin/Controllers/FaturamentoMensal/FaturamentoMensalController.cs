using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.FaturamentoMensal
{
    [CustomAuthorize("FaturamentosMensais/FaturamentoMensal")]
    public class FaturamentoMensalController : BaseController
    {
        #region Construtores

        public FaturamentoMensalController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoGrupoFaturamento = 0, codigoServico = 0;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("Servico"), out codigoServico);

                DateTime dataVencimento;
                DateTime.TryParse(Request.Params("DataVencimento"), out dataVencimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status;
                Enum.TryParse(Request.Params("Status"), out status);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Grupos Faturamento", "GruposFaturamento", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataProcessamento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Finalização", "DataFinalizacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaStatusFaturamentoMensal", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaStatusFaturamentoMensal")
                    propOrdenar = "StatusFaturamentoMensal";

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> listaFaturamentoMensal = repFaturamentoMensal.Consultar(cnpjPessoa, codigoGrupoFaturamento, codigoServico, dataVencimento, status, this.Usuario.Empresa.Codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturamentoMensal.ContarConsulta(cnpjPessoa, codigoGrupoFaturamento, codigoServico, dataVencimento, status, this.Usuario.Empresa.Codigo));

                var lista = (from p in listaFaturamentoMensal
                             select new
                             {
                                 p.Codigo,
                                 p.GruposFaturamento,
                                 DataProcessamento = p.DataProcessamento.Value.ToString("dd/MM/yyyy"),
                                 DataFinalizacao = p.DataFinalizacao != null && p.DataFinalizacao.HasValue ? p.DataFinalizacao.Value.ToString("dd/MM/yyyy") : "",
                                 p.DescricaStatusFaturamentoMensal
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);

                if (faturamentoMensal.StatusFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Cancelado)
                    return new JsonpResult(false, "Faturamento cancelado, não é mais permitido editar.");

                var dynEquipamento = new
                {
                    faturamentoMensal.Codigo,
                    faturamentoMensal.StatusFaturamentoMensal
                };

                return new JsonpResult(dynEquipamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaFaturamentoMensal()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisaFaturamentoMensal(unidadeTrabalho);
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
        public async Task<IActionResult> ExportarPesquisaFaturamentoMensal()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisaFaturamentoMensal(unidadeTrabalho, true);
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
        public async Task<IActionResult> BuscarTodasEmpresas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoGrupoFaturamento = 0, codigoServico = 0, codigoConfiguracaoBanco = 0;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("ConfiguracaoBanco"), out codigoConfiguracaoBanco);

                Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente> listaFaturamentoMensalCliente = repFaturamentoMensalCliente.Consulta(false, null, this.Usuario.Empresa.Codigo, -1, codigoConfiguracaoBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, codigoGrupoFaturamento, cnpjPessoa, 0, codigoServico, "", "", 0, 0);

                var lista = (from p in listaFaturamentoMensalCliente
                             select new
                             {
                                 Codigo = p.Codigo,
                                 CodigoFaturamentoCliente = p.Codigo,
                                 CodigoFaturamentoClienteServico = 0,
                                 CodigoConfiguracaoBanco = p.BoletoConfiguracao != null ? p.BoletoConfiguracao.Codigo : 0,
                                 CodigoTitulo = 0,
                                 CodigoNotaFiscal = 0,
                                 CodigoNotaFiscalServico = 0,
                                 Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Iniciada,
                                 Pessoa = p.Pessoa.Nome,
                                 GrupoFaturamento = p.FaturamentoMensalGrupo.Descricao,
                                 Banco = p.BoletoConfiguracao != null ? p.BoletoConfiguracao.DescricaoBanco : "",
                                 DiaFaturamento = p.DiaFatura.ToString("n0"),
                                 DataVencimento = servFaturamentoMensal.ProximaDataVencimento(p.Codigo, unitOfWork).Value.ToString("dd/MM/yyyy"),
                                 NumeroTitulo = "",
                                 NumeroBoleto = "",
                                 NumeroNota = "",
                                 StatusNotaFiscal = "",
                                 Valor = servFaturamentoMensal.ValorTotalFaturamentoCliente(p.Codigo, servFaturamentoMensal.ProximaDataVencimento(p.Codigo, unitOfWork).Value, unitOfWork).ToString("n2"),
                                 Observacao = ""
                             }).ToList();

                return new JsonpResult(lista);

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarFaturamentoSelecionado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo > 0)
                {
                    Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                    List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> listaFaturamentoMensal = repFaturamentoMensal.ConsultarPorCodigoFaturamento(codigo);

                    var lista = (from p in listaFaturamentoMensal
                                 select new
                                 {
                                     Codigo = p.FaturamentoMensalCliente.Codigo,
                                     CodigoFaturamentoCliente = p.FaturamentoMensalCliente.Codigo,
                                     CodigoFaturamentoClienteServico = p.Codigo,
                                     CodigoConfiguracaoBanco = p.FaturamentoMensalCliente.BoletoConfiguracao != null ? p.FaturamentoMensalCliente.BoletoConfiguracao.Codigo : 0,
                                     CodigoTitulo = p.Titulo != null ? p.Titulo.Codigo : 0,
                                     CodigoNotaFiscal = p.NotaFiscal != null ? p.NotaFiscal.Codigo : 0,
                                     CodigoNotaFiscalServico = p.NotaFiscalServico != null ? p.NotaFiscalServico.Codigo : 0,
                                     Status = p.FaturamentoMensal.StatusFaturamentoMensal,
                                     Pessoa = p.FaturamentoMensalCliente.Pessoa.Nome,
                                     GrupoFaturamento = p.FaturamentoMensalCliente.FaturamentoMensalGrupo.Descricao,
                                     Banco = p.FaturamentoMensalCliente.BoletoConfiguracao != null ? p.FaturamentoMensalCliente.BoletoConfiguracao.DescricaoBanco : "",
                                     DiaFaturamento = p.FaturamentoMensalCliente.DiaFatura.ToString("n0"),
                                     DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                     NumeroTitulo = p.Titulo != null ? p.Titulo.Codigo.ToString("n0") : "",
                                     NumeroBoleto = p.Titulo != null ? p.Titulo.NossoNumero : "",
                                     NumeroNota = p.NotaFiscal != null ? p.NotaFiscal.Numero.ToString("n0") : "",
                                     NumeroNotaServico = p.NotaFiscalServico != null ? p.NotaFiscalServico.Numero.ToString("n0") : "",
                                     StatusNotaFiscal = p.NotaFiscal != null ? p.NotaFiscal.DescricaoStatus : "",
                                     Valor = p.ValorFatura.ToString("n2"),
                                     Observacao = p.ObservacaoFatura
                                 }).ToList();

                    return new JsonpResult(lista);
                }
                else
                    return new JsonpResult(false, "Favor inicie um faturamento mensal.");

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

        public async Task<IActionResult> IniciarFaturamentoMensal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoGrupoFaturamento = 0, codigoServico = 0, codigoConfiguracaoBanco = 0;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("ConfiguracaoBanco"), out codigoConfiguracaoBanco);
                bool selecionarTodos = false;
                bool.TryParse(Request.Params("SelecionarTodos"), out selecionarTodos);

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                {
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                    repFaturamentoMensalClienteServico.DeletarPorFaturamento(codigo);
                }
                else
                {
                    faturamentoMensal = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal();
                    faturamentoMensal.DataProcessamento = DateTime.Now;
                    faturamentoMensal.Empresa = this.Usuario.Empresa;
                }
                faturamentoMensal.Usuario = this.Usuario;
                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Iniciada;
                if (codigo > 0)
                    repFaturamentoMensal.Atualizar(faturamentoMensal);
                else
                    repFaturamentoMensal.Inserir(faturamentoMensal);

                SalvarFaturamentoClienteServico(unitOfWork, faturamentoMensal, selecionarTodos, cnpjPessoa, codigoGrupoFaturamento, codigoServico, codigoConfiguracaoBanco);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Iniciou o faturamento mensal.", unitOfWork);
                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = faturamentoMensal.Codigo
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar o faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> GerarDocumentosFaturamentoMensal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                bool gerarDocumentosFaturamentoMensal = false;
                bool.TryParse(Request.Params("NaoGerarDocumento"), out gerarDocumentosFaturamentoMensal);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                {
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                }
                else
                {
                    return new JsonpResult(false, "Por favor inicie um faturamento mensal.");
                }

                if (gerarDocumentosFaturamentoMensal)
                {
                    GerarTituloFaturamentoMensalTodosFaturamento(unitOfWork, faturamentoMensal);
                }
                else
                {
                    GerarTituloFaturamentoMensal(unitOfWork, faturamentoMensal);
                    string msgRetorno = string.Empty;
                    GerarDocumentosFaturamentoMensal(unitOfWork, faturamentoMensal, ref msgRetorno);
                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(msgRetorno);
                        return new JsonpResult(false, msgRetorno);
                    }
                }

                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.GeradoDocumentos;
                repFaturamentoMensal.Atualizar(faturamentoMensal);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Gerou Documentos do Faturamento.", unitOfWork);

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = faturamentoMensal.Codigo
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha na geração de documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> AutorizarDocumentosFaturamentoMensal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Nenhum faturamento selecionado.");

                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.AguardandoAutorizacaoDocumento;
                repFaturamentoMensal.Atualizar(faturamentoMensal);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Autorizou Documentos do Faturamento.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao autorizar os documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> IniciarGeracaoBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Nenhum faturamento selecionado.");

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> faturamentos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(codigo);
                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    if (faturamentos[i].Titulo == null)
                        return new JsonpResult(false, "Existe faturamento(s) sem geração do título.");
                    if (faturamentos[i].NotaFiscal != null)
                        if (faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Emitido || faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.AguardandoAssinar || faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.EmProcessamento)
                            return new JsonpResult(false, "Existe nota(s) fiscal(is) não autorizadas.");
                    //if (faturamentos[i].NotaFiscalServico != null)
                    //if (faturamentos[i].NotaFiscalServico.Status != "A")
                    //return new JsonpResult(false, "Existe nota(s) fiscal(is) de serviço não autorizadas.");
                }

                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.DocumentosAutorizados;
                repFaturamentoMensal.Atualizar(faturamentoMensal);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Iniciou Geração de Boletos.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha iniciar o faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> GerarBoletoFaturamentoMensal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Nenhum faturamento selecionado.");

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> faturamentos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(codigo);
                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    if (faturamentos[i].Titulo != null && faturamentos[i].Titulo.BoletoConfiguracao != null && string.IsNullOrEmpty(faturamentos[i].Titulo.NossoNumero))
                    {
                        faturamentos[i].Titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;
                        repTitulo.Atualizar(faturamentos[i].Titulo);

                        Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
                        servTitulo.IntegrarEmitido(faturamentos[i].Titulo, unitOfWork);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Gerou Boleto do Faturamento.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha iniciar o faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> IniciarEnvioEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Nenhum faturamento selecionado.");

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> faturamentos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(codigo);
                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    if (faturamentos[i].Titulo == null)
                        return new JsonpResult(false, "Existe faturamento(s) sem geração do título.");
                    if (faturamentos[i].Titulo != null && faturamentos[i].Titulo.BoletoConfiguracao != null && string.IsNullOrWhiteSpace(faturamentos[i].Titulo.NossoNumero))
                        return new JsonpResult(false, "Existe título(s) sem nosso número de boleto.");
                    if (faturamentos[i].NotaFiscal != null)
                        if (faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Emitido || faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.AguardandoAssinar || faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.EmProcessamento)
                            return new JsonpResult(false, "Existe nota(s) fiscal(is) não autorizadas.");
                    //if (faturamentos[i].NotaFiscalServico != null)
                    //if (faturamentos[i].NotaFiscalServico.Status != "A")
                    //return new JsonpResult(false, "Existe nota(s) fiscal(is) de serviço não autorizadas.");
                }

                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.GeradoBoletos;
                repFaturamentoMensal.Atualizar(faturamentoMensal);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Iniciou Envio dos E-mails.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha iniciar o faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> EnviarEmailFaturamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(this.Usuario.Empresa.Codigo);
                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Nenhum faturamento selecionado.");

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> faturamentos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(codigo);
                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    if (faturamentos[i].Titulo == null)
                        return new JsonpResult(false, "Existe faturamento(s) sem geração do título.");
                    if (faturamentos[i].Titulo != null && faturamentos[i].Titulo.BoletoConfiguracao != null && string.IsNullOrWhiteSpace(faturamentos[i].Titulo.NossoNumero))
                        return new JsonpResult(false, "Existe título(s) sem nosso número de boleto.");
                    if (faturamentos[i].NotaFiscal != null)
                        if (faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Emitido || faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.AguardandoAssinar || faturamentos[i].NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.EmProcessamento)
                            return new JsonpResult(false, "Existe nota(s) fiscal(is) não autorizadas.");
                    //if (faturamentos[i].NotaFiscalServico != null)
                    //if (faturamentos[i].NotaFiscalServico.Status != "A")
                    //return new JsonpResult(false, "Existe nota(s) fiscal(is) de serviço não autorizadas.");
                }

                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.AguardandoEnvioEmail;
                repFaturamentoMensal.Atualizar(faturamentoMensal);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Enviou E-mail do Faturamento.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar e-mail do faturamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> CanelarFaturamentoMensal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Nenhum faturamento selecionado.");

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> faturamentos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(codigo);
                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    if (faturamentos[i].Titulo != null)
                        return new JsonpResult(false, "Existe faturamento(s) com títulos gerados, impossível de cancelar o mesmo.");
                }

                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Cancelado;
                faturamentoMensal.DataFinalizacao = DateTime.Now.Date;
                repFaturamentoMensal.Atualizar(faturamentoMensal);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Cancelou o Faturamento.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha iniciar o faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> FinalizarFaturamentoMensal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal;
                if (codigo > 0)
                    faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);
                else
                    return new JsonpResult(false, "Nenhum faturamento selecionado.");

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> faturamentos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(codigo);
                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    if (faturamentos[i].NotaFiscal != null && faturamentos[i].NotaFiscal.Status != Dominio.Enumeradores.StatusNFe.Autorizado)
                    {
                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoDeletar = repFaturamentoMensalClienteServico.BuscarPorCodigo(faturamentos[i].Codigo);
                        repFaturamentoMensalClienteServico.Deletar(faturamentoDeletar);
                    }
                    if (faturamentos[i].NotaFiscalServico != null && faturamentos[i].NotaFiscalServico.Status != "A")
                    {
                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoDeletar = repFaturamentoMensalClienteServico.BuscarPorCodigo(faturamentos[i].Codigo);
                        repFaturamentoMensalClienteServico.Deletar(faturamentoDeletar);
                    }
                }
                faturamentoMensal.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado;
                faturamentoMensal.DataFinalizacao = DateTime.Now.Date;
                repFaturamentoMensal.Atualizar(faturamentoMensal);

                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> relatoriosEmExecucao = repRelatorioControleGeracao.BuscarRelatoriosEmExecucao(this.Usuario.Codigo);
                foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioEmExecucao in relatoriosEmExecucao)
                {
                    relatorioEmExecucao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado;
                    repRelatorioControleGeracao.Atualizar(relatorioEmExecucao);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensal, null, "Finalizou o Faturamento.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha iniciar o faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarDadosFaturamentoMensalCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamentoMensalCliente = repFaturamentoMensalCliente.BuscarPorCodigo(codigo);
                Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                DateTime? dataUltimaFatura, dataProximaFatura;
                dataUltimaFatura = servFaturamentoMensal.UltimaDataVencimento(codigo, unitOfWork);
                dataProximaFatura = servFaturamentoMensal.ProximaDataVencimento(codigo, unitOfWork);

                var dynFaturamentoCliente = new
                {
                    faturamentoMensalCliente.Codigo,
                    faturamentoMensalCliente.Ativo,
                    TipoNota = faturamentoMensalCliente.TipoNotaFiscal,
                    faturamentoMensalCliente.DiaFatura,
                    ValorTotal = servFaturamentoMensal.ValorTotalFaturamentoCliente(faturamentoMensalCliente.Codigo, dataProximaFatura, unitOfWork),
                    DataUltimaFatura = dataUltimaFatura != null && dataUltimaFatura.HasValue ? dataUltimaFatura.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataProximaFatura = dataProximaFatura != null && dataProximaFatura.HasValue ? dataProximaFatura.Value.ToString("dd/MM/yyyy") : string.Empty,
                    faturamentoMensalCliente.ValorServicoPrincipal,
                    faturamentoMensalCliente.ValorAdesao,
                    TipoObservacao = faturamentoMensalCliente.TipoObservacaoFaturamentoMensal,
                    faturamentoMensalCliente.Observacao,
                    DataContrato = faturamentoMensalCliente.DataContrato != null && faturamentoMensalCliente.DataContrato.HasValue ? faturamentoMensalCliente.DataContrato.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataLancamento = faturamentoMensalCliente.DataLancamento != null && faturamentoMensalCliente.DataLancamento.HasValue ? faturamentoMensalCliente.DataLancamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataLancamentoAte = faturamentoMensalCliente.DataLancamentoAte != null && faturamentoMensalCliente.DataLancamentoAte.HasValue ? faturamentoMensalCliente.DataLancamentoAte.Value.ToString("dd/MM/yyyy") : string.Empty,
                    faturamentoMensalCliente.NumeroPedidoCompra,
                    faturamentoMensalCliente.NumeroPedidoItemCompra,
                    GrupoFaturamento = new
                    {
                        Codigo = faturamentoMensalCliente.FaturamentoMensalGrupo != null ? faturamentoMensalCliente.FaturamentoMensalGrupo.Codigo : 0,
                        Descricao = faturamentoMensalCliente.FaturamentoMensalGrupo != null ? faturamentoMensalCliente.FaturamentoMensalGrupo.Descricao : ""
                    },
                    Pessoa = new
                    {
                        Codigo = faturamentoMensalCliente.Pessoa != null ? faturamentoMensalCliente.Pessoa.Codigo : 0,
                        Descricao = faturamentoMensalCliente.Pessoa != null ? faturamentoMensalCliente.Pessoa.Nome : ""
                    },
                    ServicoPrincipal = new
                    {
                        Codigo = faturamentoMensalCliente.Servico != null ? faturamentoMensalCliente.Servico.Codigo : 0,
                        Descricao = faturamentoMensalCliente.Servico != null ? faturamentoMensalCliente.Servico.Descricao : ""
                    },
                    NaturezaOperacao = new
                    {
                        Codigo = faturamentoMensalCliente.NaturezaDaOperacao != null ? faturamentoMensalCliente.NaturezaDaOperacao.Codigo : 0,
                        Descricao = faturamentoMensalCliente.NaturezaDaOperacao != null ? faturamentoMensalCliente.NaturezaDaOperacao.Descricao : ""
                    },
                    TipoMovimento = new
                    {
                        Codigo = faturamentoMensalCliente.TipoMovimento != null ? faturamentoMensalCliente.TipoMovimento.Codigo : 0,
                        Descricao = faturamentoMensalCliente.TipoMovimento != null ? faturamentoMensalCliente.TipoMovimento.Descricao : ""
                    },
                    BoletoConfiguracao = new
                    {
                        Codigo = faturamentoMensalCliente.BoletoConfiguracao != null ? faturamentoMensalCliente.BoletoConfiguracao.Codigo : 0,
                        Descricao = faturamentoMensalCliente.BoletoConfiguracao != null ? faturamentoMensalCliente.BoletoConfiguracao.DescricaoBanco : ""
                    },
                    ServicosExtras = faturamentoMensalCliente.ServicosExtras != null && faturamentoMensalCliente.ServicosExtras.Count > 0 ? (from obj in faturamentoMensalCliente.ServicosExtras
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 obj.Codigo,
                                                                                                                                                 CodigoServico = obj.Servico.Codigo,
                                                                                                                                                 Descricao = obj.Servico.Descricao,
                                                                                                                                                 Quantidade = obj.Quantidade.ToString("n2"),
                                                                                                                                                 ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                                                                                                                                 ValorTotalServicoExtra = obj.ValorTotal.ToString("n2"),
                                                                                                                                                 DataLancamentoServico = obj.DataLancamento != null && obj.DataLancamento.HasValue ? obj.DataLancamento.Value.ToString("dd/MM/yyyy") : "",
                                                                                                                                                 DataLancamentoServicoAte = obj.DataLancamentoAte != null && obj.DataLancamentoAte.HasValue ? obj.DataLancamentoAte.Value.ToString("dd/MM/yyyy") : "",
                                                                                                                                                 TipoObservacaoServicoExtra = obj.TipoObservacaoFaturamentoMensal,
                                                                                                                                                 ObservacaoServicoExtra = obj.Observacao,
                                                                                                                                                 NumeroPedidoCompraExtra = obj.NumeroPedidoCompra,
                                                                                                                                                 NumeroPedidoItemCompraExtra = obj.NumeroPedidoItemCompra,
                                                                                                                                                 Historico = obj.Historico
                                                                                                                                             }).ToList() : null
                };
                return new JsonpResult(dynFaturamentoCliente);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o Faturamento Mensal de Cliente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> TodosBoletosGeradosFaturamentoMensal()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unidadeTrabalho);

                if (codigo > 0)
                {
                    int total = repFaturamentoMensal.ContarConsultarPorCodigoFaturamento(codigo);
                    int totalBoletosGerados = repFaturamentoMensal.ContarBoletosGeradosPorCodigoFaturamento(codigo);

                    if (total == totalBoletosGerados)
                        return new JsonpResult(true);
                    else
                        return new JsonpResult(false);
                }
                else
                    return new JsonpResult(false);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesFaturamentosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int codigoGrupoFaturamento = 0, codigoServico = 0, codigoConfiguracaoBanco = 0;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("Servico"), out codigoServico);

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<int> codigosFaturamentos = Request.GetListParam<int>("ListaFaturamentos");

                int.TryParse(Request.Params("ConfiguracaoBanco"), out codigoConfiguracaoBanco);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente> listaFaturamentoMensalCliente = repFaturamentoMensalCliente.Consulta(selecionarTodos, codigosFaturamentos, this.Usuario.Empresa.Codigo, -1, codigoConfiguracaoBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, codigoGrupoFaturamento, cnpjPessoa, 0, codigoServico, "", "", 0, 0);

                var lista = (from p in listaFaturamentoMensalCliente
                             select new
                             {
                                 ValorTotal = servFaturamentoMensal.ValorTotalFaturamentoCliente(p.Codigo, servFaturamentoMensal.ProximaDataVencimento(p.Codigo, unitOfWork).Value, unitOfWork)
                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes dos títulos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private IActionResult ObterGridPesquisaFaturamentoMensal(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            double cnpjPessoa = 0;
            double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

            int codigo = 0;
            int.TryParse(Request.Params("Codigo"), out codigo);

            int codigoGrupoFaturamento = 0, codigoServico = 0, codigoConfiguracaoBanco = 0;
            int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
            int.TryParse(Request.Params("Servico"), out codigoServico);
            int.TryParse(Request.Params("ConfiguracaoBanco"), out codigoConfiguracaoBanco);

            Models.Grid.EditableCell editableValorLiquido = null;
            Models.Grid.EditableCell editableValorString = null;
            Models.Grid.EditableCell editableValorDate = null;
            editableValorLiquido = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9);
            editableValorString = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 500);
            editableValorDate = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aData);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFaturamentoMensal etapa;
            Enum.TryParse(Request.Params("Etapa"), out etapa);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoFaturamentoCliente", false);
            grid.AdicionarCabecalho("CodigoFaturamentoClienteServico", false);
            grid.AdicionarCabecalho("CodigoConfiguracaoBanco", false);
            grid.AdicionarCabecalho("CodigoTitulo", false);
            grid.AdicionarCabecalho("CodigoNotaFiscal", false);
            grid.AdicionarCabecalho("CodigoNotaFiscalServico", false);
            grid.AdicionarCabecalho("Status", false);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Grupo Faturamento", "GrupoFaturamento", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Banco", "Banco", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Dia Faturamento", "DiaFaturamento", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 8, Models.Grid.Align.center, false);

            if (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFaturamentoMensal.Etapa1)
            {
                grid.AdicionarCabecalho("NumeroTitulo", false);
                grid.AdicionarCabecalho("NumeroBoleto", false);
                grid.AdicionarCabecalho("NumeroNota", false);
                grid.AdicionarCabecalho("StatusNotaFiscal", false);
                grid.AdicionarCabecalho("RetornoNotaFiscal", false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false, false, false, false, true, editableValorLiquido);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.center, false, false, false, false, true, editableValorString);
            }
            else
            {
                grid.AdicionarCabecalho("Nº Título", "NumeroTitulo", 8, Models.Grid.Align.center, false);
                if (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFaturamentoMensal.Etapa2)
                    grid.AdicionarCabecalho("NumeroBoleto", false);
                else
                    grid.AdicionarCabecalho("Nº Boleto", "NumeroBoleto", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Nº NF-e", "NumeroNota", 6, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Nº NFS-e", "NumeroNotaServico", 6, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Status NF", "StatusNotaFiscal", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno NF", "RetornoNotaFiscal", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false, false, false, false, true, editableValorLiquido);
                grid.AdicionarCabecalho("Observacao", false);
            }

            string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

            if (codigo > 0)
            {
                propOrdenar = "FaturamentoMensalCliente.Pessoa.Nome";
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> listaFaturamentoMensal = repFaturamentoMensal.ConsultarPorCodigoFaturamento(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturamentoMensal.ContarConsultarPorCodigoFaturamento(codigo));

                var lista = (from p in listaFaturamentoMensal
                             select new
                             {
                                 Codigo = p.FaturamentoMensalCliente.Codigo,
                                 CodigoFaturamentoCliente = p.FaturamentoMensalCliente.Codigo,
                                 CodigoFaturamentoClienteServico = p.Codigo,
                                 CodigoConfiguracaoBanco = p.FaturamentoMensalCliente.BoletoConfiguracao != null ? p.FaturamentoMensalCliente.BoletoConfiguracao.Codigo : 0,
                                 CodigoTitulo = p.Titulo != null ? p.Titulo.Codigo : 0,
                                 CodigoNotaFiscal = p.NotaFiscal != null ? p.NotaFiscal.Codigo : 0,
                                 CodigoNotaFiscalServico = p.NotaFiscalServico != null ? p.NotaFiscalServico.Codigo : 0,
                                 Status = p.FaturamentoMensal.StatusFaturamentoMensal,
                                 Pessoa = p.FaturamentoMensalCliente.Pessoa.Descricao,
                                 GrupoFaturamento = p.FaturamentoMensalCliente.FaturamentoMensalGrupo.Descricao,
                                 Banco = p.FaturamentoMensalCliente.BoletoConfiguracao != null ? p.FaturamentoMensalCliente.BoletoConfiguracao.DescricaoBanco : "",
                                 DiaFaturamento = p.FaturamentoMensalCliente.DiaFatura.ToString("n0"),
                                 DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                 NumeroTitulo = p.Titulo != null ? p.Titulo.Codigo.ToString("n0") : "",
                                 NumeroBoleto = p.Titulo != null ? p.Titulo.NossoNumero : "",
                                 NumeroNota = p.NotaFiscal != null ? p.NotaFiscal.Numero.ToString("n0") : "",
                                 NumeroNotaServico = p.NotaFiscalServico != null ? p.NotaFiscalServico.Numero.ToString("n0") : "",
                                 StatusNotaFiscal = p.NotaFiscal != null ? p.NotaFiscal.DescricaoStatus : p.NotaFiscalServico != null ? p.NotaFiscalServico.DescricaoStatus : "",
                                 RetornoNotaFiscal = p.NotaFiscal != null ? p.NotaFiscal.UltimoStatusSEFAZ : p.NotaFiscalServico != null ? p.NotaFiscalServico.MensagemRetornoSefaz : "",
                                 Valor = p.ValorFatura.ToString("n2"),
                                 Observacao = p.ObservacaoFatura
                             }).ToList();

                grid.AdicionaRows(lista);
            }
            else
            {
                if (propOrdenar == "Pessoa")
                    propOrdenar = "Pessoa.Nome";
                else if (propOrdenar == "GrupoFaturamento")
                    propOrdenar = "FaturamentoMensalGrupo.Descricao";
                else if (propOrdenar == "Banco")
                    propOrdenar = "BoletoConfiguracao.DescricaoBanco";

                Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente> listaFaturamentoMensalCliente = repFaturamentoMensalCliente.Consulta(false, null, this.Usuario.Empresa.Codigo, -1, codigoConfiguracaoBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, codigoGrupoFaturamento, cnpjPessoa, 0, codigoServico, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturamentoMensalCliente.ContaConsulta(this.Usuario.Empresa.Codigo, -1, codigoConfiguracaoBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, codigoGrupoFaturamento, cnpjPessoa, 0, codigoServico));

                var lista = (from p in listaFaturamentoMensalCliente
                             select new
                             {
                                 Codigo = p.Codigo,
                                 CodigoFaturamentoCliente = p.Codigo,
                                 CodigoFaturamentoClienteServico = 0,
                                 CodigoConfiguracaoBanco = p.BoletoConfiguracao != null ? p.BoletoConfiguracao.Codigo : 0,
                                 CodigoTitulo = 0,
                                 CodigoNotaFiscal = 0,
                                 CodigoNotaFiscalServico = 0,
                                 Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Iniciada,
                                 Pessoa = p.Pessoa.Descricao,
                                 GrupoFaturamento = p.FaturamentoMensalGrupo.Descricao,
                                 Banco = p.BoletoConfiguracao != null ? p.BoletoConfiguracao.DescricaoBanco : "",
                                 DiaFaturamento = p.DiaFatura.ToString("n0"),
                                 DataVencimento = servFaturamentoMensal.ProximaDataVencimento(p.Codigo, unitOfWork).Value.ToString("dd/MM/yyyy"),
                                 NumeroTitulo = "",
                                 NumeroBoleto = "",
                                 NumeroNota = "",
                                 NumeroNotaServico = "",
                                 StatusNotaFiscal = "",
                                 RetornoNotaFiscal = "",
                                 Valor = servFaturamentoMensal.ValorTotalFaturamentoCliente(p.Codigo, servFaturamentoMensal.ProximaDataVencimento(p.Codigo, unitOfWork).Value, unitOfWork).ToString("n2"),
                                 Observacao = ""
                             }).ToList();

                grid.AdicionaRows(lista);
            }

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
            {
                return new JsonpResult(grid);
            }
        }

        private void SalvarFaturamentoClienteServico(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal, bool selecionarTodos, double cnpjPessoa, int codigoGrupoFaturamento, int codigoServico, int codigoConfiguracaoBanco)
        {
            Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repNFSe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente> listaFaturamentoMensalClienteOriginal = null;
            if (selecionarTodos)
            {
                listaFaturamentoMensalClienteOriginal = repFaturamentoMensalCliente.Consulta(false, null, this.Usuario.Empresa.Codigo, -1, codigoConfiguracaoBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, codigoGrupoFaturamento, cnpjPessoa, 0, codigoServico, "", "", 0, 0);
            }
            List<int> codigosSelecionados = new List<int>();
            List<int> codigosNaoSelecionados = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaNaoSelecionados")))
            {
                dynamic listaNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaNaoSelecionados"));
                if (listaNaoSelecionados != null)
                {
                    foreach (var naoSelecionado in listaNaoSelecionados)
                    {
                        int codigoFaturamentoCliente = int.Parse((string)naoSelecionado.CodigoFaturamentoCliente);
                        codigosNaoSelecionados.Add(codigoFaturamentoCliente);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaFaturamento")))
            {
                dynamic listaFaturamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaFaturamento"));
                if (listaFaturamento != null)
                {
                    foreach (var faturamento in listaFaturamento)
                    {
                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico();
                        int codigoNotaFiscal = 0, codigoTitulo = 0, codigoNotaFiscalServico = 0;
                        int.TryParse((string)faturamento.CodigoNotaFiscal, out codigoNotaFiscal);
                        int.TryParse((string)faturamento.CodigoNotaFiscalServico, out codigoNotaFiscalServico);
                        int.TryParse((string)faturamento.CodigoTitulo, out codigoTitulo);
                        int codigoFaturamentoCliente = int.Parse((string)faturamento.CodigoFaturamentoCliente);

                        faturamentoClienteServico.DataVencimento = DateTime.Parse((string)faturamento.DataVencimento);
                        faturamentoClienteServico.FaturamentoMensal = faturamentoMensal;
                        faturamentoClienteServico.FaturamentoMensalCliente = repFaturamentoMensalCliente.BuscarPorCodigo(codigoFaturamentoCliente);
                        if (codigoNotaFiscal > 0)
                            faturamentoClienteServico.NotaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNotaFiscal);
                        else
                            faturamentoClienteServico.NotaFiscal = null;
                        faturamentoClienteServico.ObservacaoFatura = (string)faturamento.Observacao;
                        faturamentoClienteServico.SemDocumento = false;
                        if (codigoTitulo > 0)
                            faturamentoClienteServico.Titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
                        else
                            faturamentoClienteServico.Titulo = null;
                        faturamentoClienteServico.ValorFatura = decimal.Parse((string)faturamento.Valor);
                        if (codigoNotaFiscalServico > 0)
                            faturamentoClienteServico.NotaFiscalServico = repNFSe.BuscarPorCodigo(codigoNotaFiscalServico);
                        else
                            faturamentoClienteServico.NotaFiscalServico = null;

                        repFaturamentoMensalClienteServico.Inserir(faturamentoClienteServico);
                        codigosSelecionados.Add(codigoFaturamentoCliente);
                    }
                }
            }
            if (codigosSelecionados != null && codigosSelecionados.Count > 0 && selecionarTodos && listaFaturamentoMensalClienteOriginal != null && listaFaturamentoMensalClienteOriginal.Count > 0 && listaFaturamentoMensalClienteOriginal.Count > codigosSelecionados.Count)
            {
                foreach (var faturamentoCliente in listaFaturamentoMensalClienteOriginal)
                {
                    if (!codigosSelecionados.Contains(faturamentoCliente.Codigo) && !codigosNaoSelecionados.Contains(faturamentoCliente.Codigo))
                    {
                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico();

                        faturamentoClienteServico.DataVencimento = servFaturamentoMensal.ProximaDataVencimento(faturamentoCliente.Codigo, unidadeDeTrabalho).Value;
                        faturamentoClienteServico.FaturamentoMensal = faturamentoMensal;
                        faturamentoClienteServico.FaturamentoMensalCliente = faturamentoCliente;
                        faturamentoClienteServico.NotaFiscal = null;
                        faturamentoClienteServico.ObservacaoFatura = string.Empty;
                        faturamentoClienteServico.SemDocumento = false;
                        faturamentoClienteServico.ValorFatura = servFaturamentoMensal.ValorTotalFaturamentoCliente(faturamentoCliente.Codigo, faturamentoClienteServico.DataVencimento, unidadeDeTrabalho);

                        repFaturamentoMensalClienteServico.Inserir(faturamentoClienteServico);
                    }
                }
            }
        }

        private void GerarDocumentosFaturamentoMensal(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal, ref string msgRetorno)
        {
            msgRetorno = string.Empty;
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unidadeDeTrabalho);

            if (!string.IsNullOrWhiteSpace(Request.Params("ListaDocumentosSelecionados")))
            {
                dynamic listaFaturamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaDocumentosSelecionados"));
                if (listaFaturamento != null)
                {
                    foreach (var faturamento in listaFaturamento)
                    {
                        int codigoNotaFiscal = 0, codigoFaturamentoClienteServico = 0, codigoTitulo = 0, codigoNotaFiscalServico = 0;
                        int.TryParse((string)faturamento.CodigoNotaFiscal, out codigoNotaFiscal);
                        int.TryParse((string)faturamento.CodigoNotaFiscalServico, out codigoNotaFiscalServico);
                        int.TryParse((string)faturamento.CodigoTitulo, out codigoTitulo);
                        int.TryParse((string)faturamento.CodigoFaturamentoClienteServico, out codigoFaturamentoClienteServico);
                        if (codigoNotaFiscal == 0 && codigoTitulo == 0 && codigoNotaFiscalServico == 0)
                        {
                            if (codigoFaturamentoClienteServico > 0)
                            {
                                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarPorCodigo(codigoFaturamentoClienteServico);
                                if (faturamentoClienteServico != null && faturamentoClienteServico.NotaFiscal == null && faturamentoClienteServico.Titulo == null && faturamentoClienteServico.NotaFiscalServico == null)
                                {
                                    decimal valorOriginalFatura = servFaturamentoMensal.ValorTotalFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo, faturamentoClienteServico.DataVencimento.Value, unidadeDeTrabalho);
                                    decimal valorOriginalAdesao = 0;
                                    decimal valorPlano = 0;
                                    string observacaoAdesao = "";
                                    string observacaoPlanoMensalNFe = "";
                                    string observacaoPlanoMensalValorNFe = "";
                                    string observacaoPlanoMensalTitulo = "";
                                    string observacaoPlanoMensalValorTitulo = "";

                                    if (faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.FaturamentoAutomatico)
                                    {
                                        Dominio.Entidades.Empresa empresaCliente = repEmpresa.BuscarPorCNPJ(faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.CPF_CNPJ_SemFormato);
                                        int codigoEmpresaCliente = 0;
                                        if (empresaCliente != null)
                                            codigoEmpresaCliente = empresaCliente.Codigo;

                                        int qtdTitulo = repTitulo.QuantidadeTitulosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdBoleto = repTitulo.QuantidadeBoletosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdNFSe = repCTe.QuantidadeNotaServico(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdNotaFiscal = repNotaFiscal.QuantidadeNotaFiscal(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdTotalDocumentos = qtdBoleto + qtdNFSe + qtdNotaFiscal + qtdTitulo;
                                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico ultimoFaturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo);
                                        Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor planoEmissao = repPlanoEmissaoNFe.BuscarPlanoEmissao(qtdTitulo, qtdBoleto, qtdNotaFiscal, qtdNFSe, empresaCliente?.EmpresaPai?.Codigo);
                                        if (planoEmissao != null)
                                        {
                                            if (planoEmissao.PlanoEmissaoNFe.ValorAdesao > 0 && ultimoFaturamentoClienteServico == null)
                                            {
                                                valorOriginalAdesao = planoEmissao.PlanoEmissaoNFe.ValorAdesao;
                                                valorPlano = planoEmissao.Valor;
                                                observacaoAdesao = "FATURAMENTO DE ADESAO AO SISTEMA";
                                            }
                                            else
                                            {
                                                valorPlano = planoEmissao.Valor;
                                                valorOriginalAdesao = 0;
                                            }

                                            observacaoAdesao += " " + faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.ObservacaoAdesao;
                                            observacaoAdesao = observacaoAdesao.Replace("#NomeGrupo", faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Descricao);
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdTotalDocumentos", qtdTotalDocumentos.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdNFe", qtdNotaFiscal.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdNFSe", qtdNFSe.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdBoleto", qtdBoleto.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdTitulo", qtdTitulo.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#MesAnoPeriodo", faturamentoClienteServico.DataVencimento.Value.AddMonths(-1).ToString("MM/yyyy"));

                                            if (!string.IsNullOrWhiteSpace(planoEmissao.Observacao) && planoEmissao.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                                            {
                                                observacaoPlanoMensalNFe = planoEmissao.Observacao;
                                                observacaoPlanoMensalTitulo = planoEmissao.Observacao;
                                            }
                                            else if (!string.IsNullOrWhiteSpace(planoEmissao.Observacao) && planoEmissao.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal)
                                            {
                                                observacaoPlanoMensalNFe = planoEmissao.Observacao;
                                            }
                                            else if (!string.IsNullOrWhiteSpace(planoEmissao.Observacao) && planoEmissao.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto)
                                            {
                                                observacaoPlanoMensalTitulo = planoEmissao.Observacao;
                                            }

                                            if (!string.IsNullOrWhiteSpace(planoEmissao.PlanoEmissaoNFe.Observacao) && planoEmissao.PlanoEmissaoNFe.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                                            {
                                                observacaoPlanoMensalValorNFe = planoEmissao.Observacao;
                                                observacaoPlanoMensalValorTitulo = planoEmissao.Observacao;
                                            }
                                            else if (!string.IsNullOrWhiteSpace(planoEmissao.PlanoEmissaoNFe.Observacao) && planoEmissao.PlanoEmissaoNFe.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal)
                                            {
                                                observacaoPlanoMensalValorNFe = planoEmissao.Observacao;
                                            }
                                            else if (!string.IsNullOrWhiteSpace(planoEmissao.PlanoEmissaoNFe.Observacao) && planoEmissao.PlanoEmissaoNFe.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto)
                                            {
                                                observacaoPlanoMensalValorTitulo = planoEmissao.Observacao;
                                            }
                                        }
                                        else
                                            valorOriginalAdesao = 0;
                                    }

                                    if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao > 0 && string.IsNullOrWhiteSpace(observacaoAdesao))
                                    {
                                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico ultimoFaturamento = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo);
                                        if (ultimoFaturamento == null)
                                        {
                                            if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao == faturamentoClienteServico.ValorFatura)
                                            {
                                                observacaoAdesao = "FATURAMENTO DE ADESAO AO SISTEMA";
                                            }
                                        }
                                    }

                                    if (faturamentoClienteServico.FaturamentoMensalCliente.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNota.NFe)
                                        SalvarNotaFiscalEletronica(unidadeDeTrabalho, faturamentoClienteServico, valorOriginalFatura, valorOriginalAdesao, observacaoAdesao, observacaoPlanoMensalNFe, observacaoPlanoMensalValorNFe, valorPlano, observacaoPlanoMensalTitulo, observacaoPlanoMensalValorTitulo);
                                    else
                                    {
                                        bool notasGeradas = SalvarNotaFiscalServico(unidadeDeTrabalho, faturamentoClienteServico, valorOriginalFatura, valorOriginalAdesao, observacaoAdesao, observacaoPlanoMensalNFe, observacaoPlanoMensalValorNFe, valorPlano, observacaoPlanoMensalTitulo, observacaoPlanoMensalValorTitulo, ref msgRetorno);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GerarTituloFaturamentoMensal(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal)
        {
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

            if (!string.IsNullOrWhiteSpace(Request.Params("ListaDocumentosNaoSelecionados")))
            {
                dynamic listaFaturamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaDocumentosNaoSelecionados"));
                if (listaFaturamento != null)
                {
                    foreach (var faturamento in listaFaturamento)
                    {
                        int codigoTitulo = 0, codigoFaturamentoClienteServico = 0;
                        int.TryParse((string)faturamento.CodigoTitulo, out codigoTitulo);
                        int.TryParse((string)faturamento.CodigoFaturamentoClienteServico, out codigoFaturamentoClienteServico);
                        if (codigoTitulo == 0)
                        {
                            if (codigoFaturamentoClienteServico > 0)
                            {
                                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarPorCodigo(codigoFaturamentoClienteServico);
                                if (faturamentoClienteServico != null)
                                {

                                    string observacaoAdesao = "";
                                    string observacaoPlanoMensalTitulo = "";
                                    string observacaoPlanoMensalValorTitulo = "";

                                    if (faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.FaturamentoAutomatico)
                                    {
                                        Dominio.Entidades.Empresa empresaCliente = repEmpresa.BuscarPorCNPJ(faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.CPF_CNPJ_SemFormato);
                                        int codigoEmpresaCliente = 0;
                                        if (empresaCliente != null)
                                            codigoEmpresaCliente = empresaCliente.Codigo;

                                        int qtdTitulo = repTitulo.QuantidadeTitulosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdBoleto = repTitulo.QuantidadeBoletosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdNFSe = repCTe.QuantidadeNotaServico(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdNotaFiscal = repNotaFiscal.QuantidadeNotaFiscal(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                        int qtdTotalDocumentos = qtdBoleto + qtdNFSe + qtdNotaFiscal + qtdTitulo;

                                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico ultimoFaturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo);
                                        Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor planoEmissao = repPlanoEmissaoNFe.BuscarPlanoEmissao(qtdTitulo, qtdBoleto, qtdNotaFiscal, qtdNFSe, empresaCliente?.EmpresaPai?.Codigo);
                                        if (planoEmissao != null)
                                        {
                                            if (planoEmissao.PlanoEmissaoNFe.ValorAdesao > 0 && ultimoFaturamentoClienteServico == null)
                                            {
                                                observacaoAdesao = "FATURAMENTO DE ADESAO AO SISTEMA";
                                            }

                                            observacaoAdesao += " " + faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.ObservacaoAdesao;
                                            observacaoAdesao = observacaoAdesao.Replace("#NomeGrupo", faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Descricao);
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdTotalDocumentos", qtdTotalDocumentos.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdNFe", qtdNotaFiscal.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdNFSe", qtdNFSe.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdBoleto", qtdBoleto.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#QtdTitulo", qtdTitulo.ToString("n0"));
                                            observacaoAdesao = observacaoAdesao.Replace("#MesAnoPeriodo", faturamentoClienteServico.DataVencimento.Value.AddMonths(-1).ToString("MM/yyyy"));

                                            if (!string.IsNullOrWhiteSpace(planoEmissao.Observacao) && (planoEmissao.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto || planoEmissao.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto))
                                            {
                                                observacaoPlanoMensalTitulo = planoEmissao.Observacao;
                                            }

                                            if (!string.IsNullOrWhiteSpace(planoEmissao.PlanoEmissaoNFe.Observacao) && (planoEmissao.PlanoEmissaoNFe.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto || planoEmissao.PlanoEmissaoNFe.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto))
                                            {
                                                observacaoPlanoMensalValorTitulo = planoEmissao.PlanoEmissaoNFe.Observacao;
                                            }
                                        }
                                    }

                                    if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao > 0 && string.IsNullOrWhiteSpace(observacaoAdesao))
                                    {
                                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico ultimoFaturamento = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo);
                                        if (ultimoFaturamento == null)
                                        {
                                            if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao == faturamentoClienteServico.ValorFatura)
                                            {
                                                observacaoAdesao = "FATURAMENTO DE ADESAO AO SISTEMA";
                                            }
                                        }
                                    }

                                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                                    titulo.Acrescimo = 0;
                                    titulo.BoletoConfiguracao = faturamentoClienteServico.FaturamentoMensalCliente.BoletoConfiguracao;
                                    if (titulo.BoletoConfiguracao != null)
                                        titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;
                                    titulo.DataAutorizacao = null;
                                    titulo.DataCancelamento = null;
                                    titulo.DataEmissao = DateTime.Now.Date;
                                    titulo.DataLiquidacao = null;
                                    titulo.DataBaseLiquidacao = null;
                                    titulo.DataVencimento = faturamentoClienteServico.DataVencimento;
                                    titulo.DataProgramacaoPagamento = faturamentoClienteServico.DataVencimento;
                                    titulo.Desconto = 0;
                                    titulo.Empresa = this.Usuario.Empresa;
                                    titulo.Historico = "TÍTULO GERADO DA FATURAMENTO MENSAL";
                                    titulo.Observacao = faturamentoClienteServico.ObservacaoFatura;

                                    titulo.DataLancamento = DateTime.Now;
                                    titulo.Usuario = this.Usuario;

                                    if (faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto || faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                                    {
                                        if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.Observacao))
                                            titulo.Observacao += " " + faturamentoClienteServico.FaturamentoMensalCliente.Observacao;
                                    }

                                    if (!string.IsNullOrWhiteSpace(observacaoAdesao))
                                        titulo.Observacao += " " + observacaoAdesao;
                                    if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalTitulo))
                                        titulo.Observacao += " " + observacaoPlanoMensalTitulo;
                                    if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalValorTitulo))
                                        titulo.Observacao += " " + observacaoPlanoMensalValorTitulo;

                                    titulo.Pessoa = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa;
                                    titulo.GrupoPessoas = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.GrupoPessoas;
                                    titulo.Sequencia = 1;
                                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                                    titulo.DataAlteracao = DateTime.Now;
                                    titulo.TipoMovimento = faturamentoClienteServico.FaturamentoMensalCliente.TipoMovimento;
                                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                                    titulo.ValorOriginal = faturamentoClienteServico.ValorFatura;
                                    titulo.ValorPago = 0;
                                    titulo.ValorPendente = faturamentoClienteServico.ValorFatura;
                                    titulo.ValorTituloOriginal = faturamentoClienteServico.ValorFatura;

                                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                        titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                                    if (!string.IsNullOrWhiteSpace(titulo.Observacao) && titulo.Observacao.Length >= 300)
                                        titulo.Observacao = titulo.Observacao.Substring(0, 299);

                                    if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
                                    {
                                        Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                                        servTitulo.IntegrarEmitido(titulo, unidadeDeTrabalho);
                                    }

                                    repTitulo.Inserir(titulo);

                                    servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO DE FATURAMENTO MENSAL " + faturamentoClienteServico.ObservacaoFatura, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo);

                                    faturamentoClienteServico.Titulo = titulo;
                                    repFaturamentoMensalClienteServico.Atualizar(faturamentoClienteServico);

                                    if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
                                    {
                                        Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                                        servTitulo.IntegrarEmitido(titulo, unidadeDeTrabalho);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GerarTituloFaturamentoMensalTodosFaturamento(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal faturamentoMensal)
        {
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

            List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> listaFaturamento = repFaturamentoMensalClienteServico.BuscarPorFaturamento(faturamentoMensal.Codigo);

            if (listaFaturamento != null && listaFaturamento.Count() > 0)
            {
                foreach (var faturamento in listaFaturamento)
                {
                    int codigoTitulo = 0;
                    if (faturamento.Titulo != null)
                        codigoTitulo = faturamento.Titulo.Codigo;

                    int codigoFaturamentoClienteServico = 0;
                    if (faturamento != null)
                        codigoFaturamentoClienteServico = faturamento.Codigo;

                    if (codigoTitulo == 0)
                    {
                        if (codigoFaturamentoClienteServico > 0)
                        {
                            Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarPorCodigo(codigoFaturamentoClienteServico);
                            if (faturamentoClienteServico != null)
                            {

                                string observacaoAdesao = "";
                                string observacaoPlanoMensalTitulo = "";
                                string observacaoPlanoMensalValorTitulo = "";

                                if (faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.FaturamentoAutomatico)
                                {
                                    Dominio.Entidades.Empresa empresaCliente = repEmpresa.BuscarPorCNPJ(faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.CPF_CNPJ_SemFormato);
                                    int codigoEmpresaCliente = 0;
                                    if (empresaCliente != null)
                                        codigoEmpresaCliente = empresaCliente.Codigo;

                                    int qtdTitulo = repTitulo.QuantidadeTitulosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                    int qtdBoleto = repTitulo.QuantidadeBoletosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                    int qtdNFSe = repCTe.QuantidadeNotaServico(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                    int qtdNotaFiscal = repNotaFiscal.QuantidadeNotaFiscal(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, faturamentoClienteServico.DataVencimento.Value.AddMonths(-1));
                                    int qtdTotalDocumentos = qtdBoleto + qtdNFSe + qtdNotaFiscal + qtdTitulo;

                                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico ultimoFaturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo);
                                    Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor planoEmissao = repPlanoEmissaoNFe.BuscarPlanoEmissao(qtdTitulo, qtdBoleto, qtdNotaFiscal, qtdNFSe, empresaCliente?.EmpresaPai?.Codigo);
                                    if (planoEmissao != null)
                                    {
                                        if (planoEmissao.PlanoEmissaoNFe.ValorAdesao > 0 && ultimoFaturamentoClienteServico == null)
                                        {
                                            observacaoAdesao = "FATURAMENTO DE ADESAO AO SISTEMA";
                                        }

                                        observacaoAdesao += " " + faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.ObservacaoAdesao;
                                        observacaoAdesao = observacaoAdesao.Replace("#NomeGrupo", faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Descricao);
                                        observacaoAdesao = observacaoAdesao.Replace("#QtdTotalDocumentos", qtdTotalDocumentos.ToString("n0"));
                                        observacaoAdesao = observacaoAdesao.Replace("#QtdNFe", qtdNotaFiscal.ToString("n0"));
                                        observacaoAdesao = observacaoAdesao.Replace("#QtdNFSe", qtdNFSe.ToString("n0"));
                                        observacaoAdesao = observacaoAdesao.Replace("#QtdBoleto", qtdBoleto.ToString("n0"));
                                        observacaoAdesao = observacaoAdesao.Replace("#QtdTitulo", qtdTitulo.ToString("n0"));
                                        observacaoAdesao = observacaoAdesao.Replace("#MesAnoPeriodo", faturamentoClienteServico.DataVencimento.Value.AddMonths(-1).ToString("MM/yyyy"));

                                        if (!string.IsNullOrWhiteSpace(planoEmissao.Observacao) && (planoEmissao.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto || planoEmissao.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto))
                                        {
                                            observacaoPlanoMensalTitulo = planoEmissao.Observacao;
                                        }

                                        if (!string.IsNullOrWhiteSpace(planoEmissao.PlanoEmissaoNFe.Observacao) && (planoEmissao.PlanoEmissaoNFe.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto || planoEmissao.PlanoEmissaoNFe.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto))
                                        {
                                            observacaoPlanoMensalValorTitulo = planoEmissao.PlanoEmissaoNFe.Observacao;
                                        }
                                    }
                                }

                                if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao > 0 && string.IsNullOrWhiteSpace(observacaoAdesao))
                                {
                                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico ultimoFaturamento = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo);
                                    if (ultimoFaturamento == null)
                                    {
                                        if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao == faturamentoClienteServico.ValorFatura)
                                        {
                                            observacaoAdesao = "FATURAMENTO DE ADESAO AO SISTEMA";
                                        }
                                    }
                                }

                                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                                titulo.Acrescimo = 0;
                                titulo.BoletoConfiguracao = faturamentoClienteServico.FaturamentoMensalCliente.BoletoConfiguracao;
                                if (titulo.BoletoConfiguracao != null)
                                    titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;
                                titulo.DataAutorizacao = null;
                                titulo.DataCancelamento = null;
                                titulo.DataEmissao = DateTime.Now.Date;
                                titulo.DataLiquidacao = null;
                                titulo.DataBaseLiquidacao = null;
                                titulo.DataVencimento = faturamentoClienteServico.DataVencimento;
                                titulo.DataProgramacaoPagamento = faturamentoClienteServico.DataVencimento;
                                titulo.Desconto = 0;
                                titulo.Empresa = this.Usuario.Empresa;
                                titulo.Historico = "TÍTULO GERADO DA FATURAMENTO MENSAL";
                                titulo.Observacao = faturamentoClienteServico.ObservacaoFatura;

                                titulo.Usuario = this.Usuario;
                                titulo.DataLancamento = DateTime.Now;

                                if (faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto || faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                                {
                                    if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.Observacao))
                                        titulo.Observacao += " " + faturamentoClienteServico.FaturamentoMensalCliente.Observacao;
                                }

                                if (!string.IsNullOrWhiteSpace(observacaoAdesao))
                                    titulo.Observacao += " " + observacaoAdesao;
                                if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalTitulo))
                                    titulo.Observacao += " " + observacaoPlanoMensalTitulo;
                                if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalValorTitulo))
                                    titulo.Observacao += " " + observacaoPlanoMensalValorTitulo;

                                titulo.Pessoa = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa;
                                titulo.GrupoPessoas = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.GrupoPessoas;
                                titulo.Sequencia = 1;
                                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                                titulo.DataAlteracao = DateTime.Now;
                                titulo.TipoMovimento = faturamentoClienteServico.FaturamentoMensalCliente.TipoMovimento;
                                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                                titulo.ValorOriginal = faturamentoClienteServico.ValorFatura;
                                titulo.ValorPago = 0;
                                titulo.ValorPendente = faturamentoClienteServico.ValorFatura;
                                titulo.ValorTituloOriginal = faturamentoClienteServico.ValorFatura;

                                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                    titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                                if (!string.IsNullOrWhiteSpace(titulo.Observacao) && titulo.Observacao.Length >= 300)
                                    titulo.Observacao = titulo.Observacao.Substring(0, 299);

                                if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
                                {
                                    Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                                    servTitulo.IntegrarEmitido(titulo, unidadeDeTrabalho);
                                }

                                repTitulo.Inserir(titulo);

                                servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO DE FATURAMENTO MENSAL " + faturamentoClienteServico.ObservacaoFatura, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo);

                                faturamentoClienteServico.Titulo = titulo;
                                repFaturamentoMensalClienteServico.Atualizar(faturamentoClienteServico);

                                if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
                                {
                                    Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                                    servTitulo.IntegrarEmitido(titulo, unidadeDeTrabalho);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SalvarNotaFiscalEletronica(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico, decimal valorOriginalFatura, decimal valorOriginalAdesao, string observacaoAdesao, string observacaoPlanoMensalNFe, string observacaoPlanoMensalValorNFe, decimal valorPlano, string observacaoPlanoMensalTitulo, string observacaoPlanoMensalValorTitulo)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(unidadeDeTrabalho);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico repFaturamentoMensalServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();
            notaFiscal.Atividade = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.Atividade;
            notaFiscal.BCCOFINS = 0;
            notaFiscal.BCDeducao = 0;
            notaFiscal.BCICMS = 0;
            notaFiscal.BCICMSST = 0;
            notaFiscal.BCISSQN = 0;
            notaFiscal.BCPIS = 0;
            notaFiscal.Cliente = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa;
            notaFiscal.DataEmissao = DateTime.Now.Date;
            notaFiscal.DataPrestacaoServico = DateTime.Now.Date;
            notaFiscal.DataSaida = DateTime.Now.Date;
            notaFiscal.Empresa = this.Usuario.Empresa;
            notaFiscal.EmpresaSerie = repEmpresaSerie.BuscarPorEmpresaTipo(this.Usuario.Empresa.Codigo, Dominio.Enumeradores.TipoSerie.NFe);
            notaFiscal.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Normal;
            notaFiscal.ICMSDesonerado = 0;
            notaFiscal.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Outros;
            notaFiscal.IndicadorIntermediador = Dominio.Enumeradores.IndicadorIntermediadorNFe.SemIntermediador;
            notaFiscal.LocalidadePrestacaoServico = this.Usuario.Empresa.Localidade;
            notaFiscal.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("55");
            notaFiscal.ModeloNotaFiscal = "55";
            notaFiscal.NaturezaDaOperacao = faturamentoClienteServico.FaturamentoMensalCliente.NaturezaDaOperacao;
            notaFiscal.Numero = repNotaFiscal.BuscarUltimoNumero(notaFiscal.Empresa.Codigo, notaFiscal.EmpresaSerie.Numero, this.Usuario.Empresa.TipoAmbiente, "55") + 1;

            int proximoNumeroSerie = repEmpresaSerie.BuscarProximoNumeroDocumentoPorSerie(notaFiscal.Empresa.Codigo, notaFiscal.EmpresaSerie.Numero, Dominio.Enumeradores.TipoSerie.NFe);
            if (notaFiscal.Numero < proximoNumeroSerie)
                notaFiscal.Numero = proximoNumeroSerie;

            notaFiscal.ObservacaoNota = faturamentoClienteServico.ObservacaoFatura;
            if (faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal || faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
            {
                if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.Observacao))
                    notaFiscal.ObservacaoNota += " " + faturamentoClienteServico.FaturamentoMensalCliente.Observacao;
            }
            if (faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal || faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
            {
                if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao))
                    notaFiscal.ObservacaoNota += " " + faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao;
            }

            if (!string.IsNullOrWhiteSpace(observacaoAdesao))
                notaFiscal.ObservacaoNota += " " + observacaoAdesao;
            if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalNFe))
                notaFiscal.ObservacaoNota += " " + observacaoPlanoMensalNFe;
            if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalValorNFe))
                notaFiscal.ObservacaoNota += " " + observacaoPlanoMensalValorNFe;

            notaFiscal.Status = Dominio.Enumeradores.StatusNFe.Emitido;
            notaFiscal.TipoAmbiente = this.Empresa.TipoAmbiente;
            notaFiscal.TipoEmissao = Dominio.Enumeradores.TipoEmissaoNFe.Saida;
            notaFiscal.TipoFrete = Dominio.Enumeradores.ModalidadeFrete.SemFrete;
            notaFiscal.ValorCOFINS = 0;
            notaFiscal.ValorDesconto = 0;
            notaFiscal.ValorDescontoCondicional = 0;
            notaFiscal.ValorDescontoIncondicional = 0;
            notaFiscal.ValorFCP = 0;
            notaFiscal.ValorFrete = 0;
            notaFiscal.ValorICMS = 0;
            notaFiscal.ValorICMSDestino = 0;
            notaFiscal.ValorICMSRemetente = 0;
            notaFiscal.ValorICMSST = 0;
            notaFiscal.ValorII = 0;
            notaFiscal.ValorImpostoIBPT = 0;
            notaFiscal.ValorIPI = 0;
            notaFiscal.ValorISSQN = 0;
            notaFiscal.ValorOutrasDespesas = 0;
            notaFiscal.ValorOutrasRetencoes = 0;
            notaFiscal.ValorPIS = 0;
            notaFiscal.ValorProdutos = 0;
            notaFiscal.ValorRetencaoISS = 0;
            notaFiscal.ValorSeguro = 0;
            notaFiscal.ValorServicos = faturamentoClienteServico.ValorFatura;
            notaFiscal.ValorTotalNota = faturamentoClienteServico.ValorFatura;
            notaFiscal.ValorTroco = 0;

            repNotaFiscal.Inserir(notaFiscal);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela parcela = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela();
            parcela.Acrescimo = 0;
            parcela.DataEmissao = DateTime.Now.Date;
            parcela.DataVencimento = faturamentoClienteServico.DataVencimento;
            parcela.Desconto = 0;
            parcela.NotaFiscal = notaFiscal;
            parcela.Sequencia = 1;
            parcela.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto;
            parcela.Valor = faturamentoClienteServico.ValorFatura;

            repNotaFiscalParcela.Inserir(parcela);

            string observacaoNotaFiscal = "";
            string observacaoBoleto = "";

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item = null;
            List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico> listaServicoExtra = repFaturamentoMensalServico.BuscarServicosExtrasFaturamento(faturamentoClienteServico.FaturamentoMensalCliente.Codigo, faturamentoClienteServico.DataVencimento.Value);
            if (valorOriginalFatura == faturamentoClienteServico.ValorFatura && listaServicoExtra?.Count > 0)
            {
                for (int i = 0; i < listaServicoExtra.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(listaServicoExtra[i].Observacao))
                    {
                        if (listaServicoExtra[i].TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                        {
                            observacaoNotaFiscal += " " + listaServicoExtra[i].Observacao;
                            observacaoBoleto += " " + listaServicoExtra[i].Observacao;
                        }
                        else if (listaServicoExtra[i].TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto)
                        {
                            observacaoBoleto += " " + listaServicoExtra[i].Observacao;
                        }
                        else if (listaServicoExtra[i].TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal)
                        {
                            observacaoNotaFiscal += " " + listaServicoExtra[i].Observacao;
                        }
                    }
                    item = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos();
                    item.AliquotaCOFINS = 0;
                    item.AliquotaFCP = 0;
                    item.AliquotaICMS = 0;
                    item.AliquotaICMSDestino = 0;
                    item.AliquotaICMSInterno = 0;
                    item.AliquotaICMSOperacao = 0;
                    item.AliquotaICMSSimples = 0;
                    item.AliquotaICMSST = 0;
                    item.AliquotaICMSSTInterestadual = 0;
                    item.AliquotaIPI = 0;
                    if (!this.Usuario.Empresa.OptanteSimplesNacional)
                        item.AliquotaISS = listaServicoExtra[i].Servico.AliquotaISS;
                    else
                        item.AliquotaISS = 0;
                    item.AliquotaPIS = 0;
                    item.BaseII = 0;
                    if (!this.Usuario.Empresa.OptanteSimplesNacional)
                        item.BaseISS = listaServicoExtra[i].ValorTotal;
                    else
                        item.BaseISS = 0;
                    item.BCCOFINS = 0;
                    item.BCDeducao = 0;
                    item.BCICMS = 0;
                    item.BCICMSDestino = 0;
                    item.BCICMSST = 0;
                    item.BCIPI = 0;
                    item.BCPIS = 0;
                    if (this.Usuario.Empresa.Localidade.Estado.Sigla != faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.Localidade.Estado.Sigla)
                        item.CFOP = listaServicoExtra[i].Servico.CFOPVendaForaEstado;
                    else
                        item.CFOP = listaServicoExtra[i].Servico.CFOPVendaDentroEstado;
                    item.CodigoItem = listaServicoExtra[i].Servico.Codigo.ToString();
                    item.CSTCOFINS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
                    item.CSTICMS = null;
                    item.CSTIPI = null;
                    item.CSTPIS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
                    item.DescontoCondicional = 0;
                    item.DescontoIncondicional = 0;
                    item.IncentivoFiscal = false;
                    item.MVAICMSST = 0;
                    item.NotaFiscal = notaFiscal;
                    item.OutrasRetencoes = 0;
                    item.PercentualPartilha = 0;
                    item.Quantidade = listaServicoExtra[i].Quantidade;
                    item.ReducaoBCCOFINS = 0;
                    item.ReducaoBCICMS = 0;
                    item.ReducaoBCICMSST = 0;
                    item.ReducaoBCIPI = 0;
                    item.ReducaoBCPIS = 0;
                    item.RetencaoISS = 0;
                    item.Servico = listaServicoExtra[i].Servico;
                    item.DescricaoItem = listaServicoExtra[i].Servico.Descricao;
                    item.ValorCOFINS = 0;
                    item.ValorDesconto = 0;
                    item.ValorDespesaII = 0;
                    item.ValorFCP = 0;
                    item.ValorFrete = 0;
                    item.ValorFreteMarinho = 0;
                    item.ValorICMS = 0;
                    item.ValorICMSDesonerado = 0;
                    item.ValorICMSDestino = 0;
                    item.ValorICMSDiferido = 0;
                    item.ValorICMSOperacao = 0;
                    item.ValorICMSRemetente = 0;
                    item.ValorICMSSimples = 0;
                    item.ValorICMSST = 0;
                    item.ValorII = 0;
                    item.ValorImpostoIBPT = 0;
                    item.ValorIOFII = 0;
                    item.ValorIPI = 0;
                    if (!this.Usuario.Empresa.OptanteSimplesNacional && item.BaseISS > 0 && item.AliquotaISS > 0)
                        item.ValorISS = ((item.AliquotaISS / 100) * item.BaseISS);
                    else
                        item.ValorISS = 0;
                    item.ValorOutrasDespesas = 0;
                    item.ValorPIS = 0;
                    item.ValorSeguro = 0;
                    item.ValorTotal = listaServicoExtra[i].ValorTotal;
                    item.ValorUnitario = listaServicoExtra[i].ValorUnitario;
                    item.NumeroItemOrdemCompra = listaServicoExtra[i].NumeroPedidoItemCompra;
                    item.NumeroOrdemCompra = listaServicoExtra[i].NumeroPedidoCompra;

                    repNotaFiscalProdutos.Inserir(item);
                }
            }

            if (!string.IsNullOrWhiteSpace(observacaoNotaFiscal))
            {
                notaFiscal.ObservacaoNota += " " + observacaoNotaFiscal;
                repNotaFiscal.Atualizar(notaFiscal);
            }

            bool servicoPrincipalVigente = false;
            if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue))
                servicoPrincipalVigente = true;
            else if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.Value.Date <= DateTime.Now.Date) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.Value.Date >= DateTime.Now.Date))
                servicoPrincipalVigente = true;
            else if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.Value.Date <= DateTime.Now.Date) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue))
                servicoPrincipalVigente = true;
            else if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.Value.Date >= DateTime.Now.Date))
                servicoPrincipalVigente = true;

            decimal valorServicoPrincipal = 0;
            if (valorOriginalAdesao > 0 && valorPlano > 0 && valorOriginalFatura == faturamentoClienteServico.ValorFatura)
            {
                valorServicoPrincipal = valorPlano + valorOriginalAdesao;
                servicoPrincipalVigente = true;
            }
            else if (valorPlano > 0 && valorOriginalFatura == faturamentoClienteServico.ValorFatura)
            {
                valorServicoPrincipal = valorPlano;
                servicoPrincipalVigente = true;
            }
            else if (faturamentoClienteServico.FaturamentoMensalCliente.ValorServicoPrincipal != faturamentoClienteServico.ValorFatura && valorOriginalFatura == faturamentoClienteServico.ValorFatura && servicoPrincipalVigente)
                valorServicoPrincipal = faturamentoClienteServico.FaturamentoMensalCliente.ValorServicoPrincipal;
            else
            {
                valorServicoPrincipal = faturamentoClienteServico.ValorFatura;
                servicoPrincipalVigente = true;
            }

            if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao > 0)
            {
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServicoAnterio = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo, faturamentoClienteServico.Codigo);
                if (faturamentoClienteServicoAnterio == null)
                {
                    valorServicoPrincipal = valorServicoPrincipal + faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao;
                    servicoPrincipalVigente = true;
                }
            }

            if (servicoPrincipalVigente)
            {
                item = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos();
                item.AliquotaCOFINS = 0;
                item.AliquotaFCP = 0;
                item.AliquotaICMS = 0;
                item.AliquotaICMSDestino = 0;
                item.AliquotaICMSInterno = 0;
                item.AliquotaICMSOperacao = 0;
                item.AliquotaICMSSimples = 0;
                item.AliquotaICMSST = 0;
                item.AliquotaICMSSTInterestadual = 0;
                item.AliquotaIPI = 0;
                if (!this.Usuario.Empresa.OptanteSimplesNacional)
                    item.AliquotaISS = faturamentoClienteServico.FaturamentoMensalCliente.Servico.AliquotaISS;
                else
                    item.AliquotaISS = 0;
                item.AliquotaPIS = 0;
                item.BaseII = 0;
                if (!this.Usuario.Empresa.OptanteSimplesNacional)
                {
                    item.BaseISS = valorServicoPrincipal;
                }
                else
                    item.BaseISS = 0;
                item.BCCOFINS = 0;
                item.BCDeducao = 0;
                item.BCICMS = 0;
                item.BCICMSDestino = 0;
                item.BCICMSST = 0;
                item.BCIPI = 0;
                item.BCPIS = 0;
                if (this.Usuario.Empresa.Localidade.Estado.Sigla != faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.Localidade.Estado.Sigla)
                    item.CFOP = faturamentoClienteServico.FaturamentoMensalCliente.Servico.CFOPVendaForaEstado;
                else
                    item.CFOP = faturamentoClienteServico.FaturamentoMensalCliente.Servico.CFOPVendaDentroEstado;
                item.CodigoItem = faturamentoClienteServico.FaturamentoMensalCliente.Servico.Codigo.ToString();
                item.CSTCOFINS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
                item.CSTICMS = null;
                item.CSTIPI = null;
                item.CSTPIS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
                item.DescontoCondicional = 0;
                item.DescontoIncondicional = 0;
                item.IncentivoFiscal = false;
                item.MVAICMSST = 0;
                item.NotaFiscal = notaFiscal;
                item.OutrasRetencoes = 0;
                item.PercentualPartilha = 0;
                item.Quantidade = 1;
                item.ReducaoBCCOFINS = 0;
                item.ReducaoBCICMS = 0;
                item.ReducaoBCICMSST = 0;
                item.ReducaoBCIPI = 0;
                item.ReducaoBCPIS = 0;
                item.RetencaoISS = 0;
                item.Servico = faturamentoClienteServico.FaturamentoMensalCliente.Servico;
                item.DescricaoItem = faturamentoClienteServico.FaturamentoMensalCliente.Servico.Descricao;
                item.ValorCOFINS = 0;
                item.ValorDesconto = 0;
                item.ValorDespesaII = 0;
                item.ValorFCP = 0;
                item.ValorFrete = 0;
                item.ValorFreteMarinho = 0;
                item.ValorICMS = 0;
                item.ValorICMSDesonerado = 0;
                item.ValorICMSDestino = 0;
                item.ValorICMSDiferido = 0;
                item.ValorICMSOperacao = 0;
                item.ValorICMSRemetente = 0;
                item.ValorICMSSimples = 0;
                item.ValorICMSST = 0;
                item.ValorII = 0;
                item.ValorImpostoIBPT = 0;
                item.ValorIOFII = 0;
                item.ValorIPI = 0;
                if (!this.Usuario.Empresa.OptanteSimplesNacional && item.BaseISS > 0 && item.AliquotaISS > 0)
                    item.ValorISS = ((item.AliquotaISS / 100) * item.BaseISS);
                else
                    item.ValorISS = 0;
                item.ValorOutrasDespesas = 0;
                item.ValorPIS = 0;
                item.ValorSeguro = 0;
                item.ValorTotal = valorServicoPrincipal;
                item.ValorUnitario = valorServicoPrincipal;
                item.NumeroItemOrdemCompra = faturamentoClienteServico.FaturamentoMensalCliente.NumeroPedidoItemCompra;
                item.NumeroOrdemCompra = faturamentoClienteServico.FaturamentoMensalCliente.NumeroPedidoCompra;

                repNotaFiscalProdutos.Inserir(item);
            }

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
            titulo.Acrescimo = 0;
            titulo.BoletoConfiguracao = faturamentoClienteServico.FaturamentoMensalCliente.BoletoConfiguracao;
            if (titulo.BoletoConfiguracao != null)
                titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;
            titulo.DataAutorizacao = null;
            titulo.DataCancelamento = null;
            titulo.DataEmissao = DateTime.Now.Date;
            titulo.DataLiquidacao = null;
            titulo.DataBaseLiquidacao = null;
            titulo.DataVencimento = faturamentoClienteServico.DataVencimento;
            titulo.DataProgramacaoPagamento = faturamentoClienteServico.DataVencimento;
            titulo.Desconto = 0;
            titulo.Empresa = this.Usuario.Empresa;
            titulo.Historico = "TÍTULO GERADO DO FATURAMENTO MENSAL";
            titulo.Observacao = faturamentoClienteServico.ObservacaoFatura;

            titulo.Usuario = this.Usuario;
            titulo.DataLancamento = DateTime.Now;

            if (faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto || faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
            {
                if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.Observacao))
                    titulo.Observacao += " " + faturamentoClienteServico.FaturamentoMensalCliente.Observacao;
            }
            if (faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto || faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
            {
                if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao))
                    titulo.Observacao += " " + faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao;
            }

            if (!string.IsNullOrWhiteSpace(observacaoBoleto))
                titulo.Observacao += " " + observacaoBoleto;
            if (!string.IsNullOrWhiteSpace(observacaoAdesao))
                titulo.Observacao += " " + observacaoAdesao;
            if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalTitulo))
                titulo.Observacao += " " + observacaoPlanoMensalTitulo;
            if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalValorTitulo))
                titulo.Observacao += " " + observacaoPlanoMensalValorTitulo;

            titulo.Pessoa = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa;
            titulo.GrupoPessoas = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa.GrupoPessoas;
            titulo.Sequencia = 1;
            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.TipoMovimento = faturamentoClienteServico.FaturamentoMensalCliente.TipoMovimento;
            titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
            titulo.ValorOriginal = faturamentoClienteServico.ValorFatura;
            titulo.ValorPago = 0;
            titulo.NotaFiscal = notaFiscal;
            titulo.ValorPendente = faturamentoClienteServico.ValorFatura;
            titulo.ValorTituloOriginal = faturamentoClienteServico.ValorFatura;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

            if (!string.IsNullOrWhiteSpace(titulo.Observacao) && titulo.Observacao.Length >= 300)
                titulo.Observacao = titulo.Observacao.Substring(0, 299);

            if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
            {
                Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                servTitulo.IntegrarEmitido(titulo, unidadeDeTrabalho);
            }

            repTitulo.Inserir(titulo);

            servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO DE FATURAMENTO MENSAL " + faturamentoClienteServico.ObservacaoFatura, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo);

            faturamentoClienteServico.NotaFiscal = notaFiscal;
            faturamentoClienteServico.NotaFiscalServico = null;
            faturamentoClienteServico.Titulo = titulo;
            repFaturamentoMensalClienteServico.Atualizar(faturamentoClienteServico);

            if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
            {
                Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                servTitulo.IntegrarEmitido(titulo, unidadeDeTrabalho);
            }
        }

        private bool SalvarNotaFiscalServico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico, decimal valorOriginalFatura, decimal valorOriginalAdesao, string observacaoAdesao, string observacaoPlanoMensalNFe, string observacaoPlanoMensalValorNFe, decimal valorPlano, string observacaoPlanoMensalTitulo, string observacaoPlanoMensalValorTitulo, ref string retorno)
        {
            retorno = string.Empty;
            try
            {
                Servicos.Cliente serCliente = new Servicos.Cliente(_conexao.StringConexao);
                Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico repFaturamentoMensalServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.NaturezaNFSe repNaturezaDaOperacao = new Repositorio.NaturezaNFSe(unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.CTe.CTeParcela repParcelaNFSe = new Repositorio.Embarcador.CTe.CTeParcela(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                Dominio.Entidades.Cliente remetente = faturamentoClienteServico.FaturamentoMensalCliente.Pessoa;
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModelo.BuscarPorModelo("39");
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRementente = null;

                int numeroDocumento = 0, codigoCTe = 0;
                int codigoEmpresa = this.Usuario.Empresa.Codigo;
                cte.CFOP = 5353;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(codigoEmpresa);
                Dominio.Entidades.Localidade localidadePrestacao = this.Usuario.Empresa.Localidade;

                cte.Remetente = serCliente.ObterClienteCTE(remetente, enderecoRementente);
                cte.Destinatario = serCliente.ObterClienteCTE(remetente, enderecoRementente);
                cte.Emitente = Servicos.Empresa.ObterEmpresaCTE(repEmpresa.BuscarPorCodigo(codigoEmpresa));
                cte.CodigoIBGECidadeInicioPrestacao = localidadePrestacao.CodigoIBGE;
                cte.CodigoIBGECidadeTerminoPrestacao = localidadePrestacao.CodigoIBGE;

                DateTime dataEmissao = DateTime.Now;
                cte.DataEmissao = dataEmissao.ToString("dd/MM/yyyy HH:mm:ss");

                Dominio.Entidades.Cliente cliTomador = null;
                cliTomador = remetente;

                cte.Serie = repSerie.BuscarPorEmpresaTipo(this.Usuario.Empresa.Codigo, Dominio.Enumeradores.TipoSerie.NFSe).Numero;
                cte.SerieRPS = transportadorConfiguracaoNFSe != null ? transportadorConfiguracaoNFSe.SerieRPS : string.Empty;

                cte.NaturezaNFSe = new Dominio.ObjetosDeValor.CTe.NaturezaNFSe();
                if (faturamentoClienteServico.FaturamentoMensalCliente == null || faturamentoClienteServico.FaturamentoMensalCliente.NaturezaDaOperacao == null || faturamentoClienteServico.FaturamentoMensalCliente.NaturezaDaOperacao.NaturezaNFSe == null)
                {
                    retorno = "Natureza da operação não configurada corretamente";
                    return false;
                }

                cte.NaturezaNFSe.CodigoInterno = faturamentoClienteServico.FaturamentoMensalCliente.NaturezaDaOperacao.NaturezaNFSe.Codigo;
                cte.ISS = new Dominio.ObjetosDeValor.CTe.ImpostoISS();
                cte.ItensNFSe = new List<Dominio.ObjetosDeValor.CTe.ItemNFSe>();

                bool issRetido = false;

                cte.ValorDescontoIncondicionado = 0;
                cte.ValorDescontoCondicionado = 0;
                cte.ValorDeducoes = 0;
                cte.ValorOutrasRetencoes = 0;

                cte.ObservacoesGerais = faturamentoClienteServico.ObservacaoFatura;
                if (faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal || faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                {
                    if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.Observacao))
                        cte.ObservacoesGerais += " " + faturamentoClienteServico.FaturamentoMensalCliente.Observacao;
                }
                if (faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal || faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                {
                    if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao))
                        cte.ObservacoesGerais += " " + faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao;
                }

                if (!string.IsNullOrWhiteSpace(observacaoAdesao))
                    cte.ObservacoesGerais += " " + observacaoAdesao;
                if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalNFe))
                    cte.ObservacoesGerais += " " + observacaoPlanoMensalNFe;
                if (!string.IsNullOrWhiteSpace(observacaoPlanoMensalValorNFe))
                    cte.ObservacoesGerais += " " + observacaoPlanoMensalValorNFe;

                Dominio.Entidades.Localidade localidadeItem = faturamentoClienteServico.FaturamentoMensalCliente.Servico.Localidade;

                bool servicoPrincipalVigente = false;
                if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue))
                    servicoPrincipalVigente = true;
                else if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.Value.Date <= DateTime.Now.Date) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.Value.Date >= DateTime.Now.Date))
                    servicoPrincipalVigente = true;
                else if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.Value.Date <= DateTime.Now.Date) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue))
                    servicoPrincipalVigente = true;
                else if ((faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento == null || !faturamentoClienteServico.FaturamentoMensalCliente.DataLancamento.HasValue) && (faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.HasValue && faturamentoClienteServico.FaturamentoMensalCliente.DataLancamentoAte.Value.Date >= DateTime.Now.Date))
                    servicoPrincipalVigente = true;

                decimal valorServicoPrincipal = 0;
                if (valorOriginalAdesao > 0 && valorPlano > 0 && valorOriginalFatura == faturamentoClienteServico.ValorFatura)
                {
                    valorServicoPrincipal = valorPlano + valorOriginalAdesao;
                    servicoPrincipalVigente = true;
                }
                else if (valorPlano > 0 && valorOriginalFatura == faturamentoClienteServico.ValorFatura)
                {
                    valorServicoPrincipal = valorPlano;
                    servicoPrincipalVigente = true;
                }
                else if (faturamentoClienteServico.FaturamentoMensalCliente.ValorServicoPrincipal != faturamentoClienteServico.ValorFatura && valorOriginalFatura == faturamentoClienteServico.ValorFatura && servicoPrincipalVigente)
                    valorServicoPrincipal = faturamentoClienteServico.FaturamentoMensalCliente.ValorServicoPrincipal;
                else
                {
                    valorServicoPrincipal = faturamentoClienteServico.ValorFatura;
                    servicoPrincipalVigente = true;
                }

                if (faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao > 0)
                {
                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServicoAnterio = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(faturamentoClienteServico.FaturamentoMensalCliente.Codigo, faturamentoClienteServico.Codigo);
                    if (faturamentoClienteServicoAnterio == null)
                    {
                        valorServicoPrincipal = valorServicoPrincipal + faturamentoClienteServico.FaturamentoMensalCliente.ValorAdesao;
                        servicoPrincipalVigente = true;
                    }
                }

                if (servicoPrincipalVigente)
                {
                    Dominio.ObjetosDeValor.CTe.ItemNFSe item = new Dominio.ObjetosDeValor.CTe.ItemNFSe();

                    item.ValorDescontoIncondicionado = 0;
                    item.ValorDescontoCondicionado = 0;
                    item.ValorDeducoes = 0;
                    item.AliquotaISS = faturamentoClienteServico.FaturamentoMensalCliente.Servico.AliquotaISS;
                    item.BaseCalculoISS = valorServicoPrincipal;
                    item.CodigoIBGECidade = localidadeItem.CodigoIBGE;
                    item.CodigoIBGECidadeIncidencia = localidadeItem.CodigoIBGE;
                    item.CodigoPaisPrestacaoServico = localidadeItem.Pais.Codigo;
                    item.Discriminacao = string.Empty;
                    item.ExigibilidadeISS = 1; //Exigível
                    item.ISSInclusoValorTotal = false;
                    item.Quantidade = 1;
                    item.Servico = new Dominio.ObjetosDeValor.CTe.ServicoNFSe();
                    if (faturamentoClienteServico.FaturamentoMensalCliente.Servico == null || faturamentoClienteServico.FaturamentoMensalCliente.Servico.ServicoNFSe == null)
                    {
                        retorno = "Serviço principal selecionado não configurada corretamente";
                        return false;
                    }
                    item.Servico.CodigoInterno = faturamentoClienteServico.FaturamentoMensalCliente.Servico.ServicoNFSe.Codigo;
                    item.ServicoPrestadoNoPais = true;
                    item.ValorServico = valorServicoPrincipal;
                    item.ValorISS = ((item.AliquotaISS / 100) * item.BaseCalculoISS);
                    item.ValorTotal = valorServicoPrincipal;

                    decimal baseCalculoIBSCBS = valorServicoPrincipal;

                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                    {
                        BaseCalculo = baseCalculoIBSCBS,
                        ValoAbaterBaseCalculo = item.ValorISS,
                        CodigoLocalidade = localidadeItem.Codigo,
                        SiglaUF = localidadeItem.Estado.Sigla,
                        CodigoTipoOperacao = 0,
                        Empresa = this.Usuario.Empresa
                    });


                    if (impostoIBSCBS != null)
                    {
                        item.IBSCBS.NBS = faturamentoClienteServico.FaturamentoMensalCliente.Servico.ServicoNFSe?.NBS ?? "";
                        item.IBSCBS.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
                        item.IBSCBS.CST = impostoIBSCBS.CST;
                        item.IBSCBS.ClassificacaoTributaria = impostoIBSCBS.ClassificacaoTributaria;
                        item.IBSCBS.BaseCalculo = baseCalculoIBSCBS;
                        item.IBSCBS.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                        item.IBSCBS.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                        item.IBSCBS.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                        item.IBSCBS.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                        item.IBSCBS.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                        item.IBSCBS.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                        item.IBSCBS.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                        item.IBSCBS.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                        item.IBSCBS.ValorCBS = impostoIBSCBS.ValorCBS;
                    }

                    cte.ItensNFSe.Add(item);
                }

                decimal valorTotalItens = valorServicoPrincipal;
                decimal valorISS = ((faturamentoClienteServico.FaturamentoMensalCliente.Servico.AliquotaISS / 100) * valorTotalItens);

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico> listaServicoExtra = repFaturamentoMensalServico.BuscarServicosExtrasFaturamento(faturamentoClienteServico.FaturamentoMensalCliente.Codigo, faturamentoClienteServico.DataVencimento.Value);
                if (valorOriginalFatura == faturamentoClienteServico.ValorFatura && listaServicoExtra?.Count > 0)
                {
                    for (int i = 0; i < listaServicoExtra.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico servicoExtra = listaServicoExtra[i];
                        if (servicoExtra.Servico == null || servicoExtra.Servico.ServicoNFSe == null)
                        {
                            retorno = "Serviço extra selecionado não configurada corretamente";
                            return false;
                        }

                        Dominio.ObjetosDeValor.CTe.ItemNFSe itemExtra = new Dominio.ObjetosDeValor.CTe.ItemNFSe();
                        itemExtra.ValorDescontoIncondicionado = 0;
                        itemExtra.ValorDescontoCondicionado = 0;
                        itemExtra.ValorDeducoes = 0;
                        itemExtra.AliquotaISS = faturamentoClienteServico.FaturamentoMensalCliente.Servico.AliquotaISS;
                        itemExtra.BaseCalculoISS = servicoExtra.ValorTotal;
                        itemExtra.CodigoIBGECidade = localidadeItem.CodigoIBGE;
                        itemExtra.CodigoIBGECidadeIncidencia = localidadeItem.CodigoIBGE;
                        itemExtra.CodigoPaisPrestacaoServico = localidadeItem.Pais.Codigo;
                        itemExtra.Discriminacao = string.Empty;
                        itemExtra.ExigibilidadeISS = 1; //Exigível
                        itemExtra.ISSInclusoValorTotal = false;
                        itemExtra.Quantidade = 1;
                        itemExtra.Servico = new Dominio.ObjetosDeValor.CTe.ServicoNFSe();
                        itemExtra.Servico.CodigoInterno = servicoExtra.Servico.ServicoNFSe.Codigo;
                        itemExtra.ServicoPrestadoNoPais = true;
                        itemExtra.ValorServico = servicoExtra.ValorTotal;
                        itemExtra.ValorISS = ((itemExtra.AliquotaISS / 100) * itemExtra.BaseCalculoISS);
                        itemExtra.ValorTotal = servicoExtra.ValorTotal;

                        valorTotalItens += itemExtra.ValorTotal;
                        valorISS += itemExtra.ValorISS;

                        decimal baseCalculoIBSCBSExtra = valorTotalItens;

                        Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBSExtra = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                        {
                            BaseCalculo = baseCalculoIBSCBSExtra,
                            ValoAbaterBaseCalculo = valorISS,
                            CodigoLocalidade = localidadeItem.Codigo,
                            SiglaUF = localidadeItem.Estado.Sigla,
                            CodigoTipoOperacao = 0,
                            Empresa = this.Usuario.Empresa
                        });

                        if (impostoIBSCBSExtra != null)
                        {
                            itemExtra.IBSCBS = new Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS();
                            itemExtra.IBSCBS.NBS = faturamentoClienteServico.FaturamentoMensalCliente.Servico.ServicoNFSe?.NBS ?? "";
                            itemExtra.IBSCBS.CodigoIndicadorOperacao = impostoIBSCBSExtra.CodigoIndicadorOperacao;
                            itemExtra.IBSCBS.CST = impostoIBSCBSExtra.CST;
                            itemExtra.IBSCBS.ClassificacaoTributaria = impostoIBSCBSExtra.ClassificacaoTributaria;
                            itemExtra.IBSCBS.BaseCalculo = impostoIBSCBSExtra.BaseCalculo;
                            itemExtra.IBSCBS.AliquotaIBSEstadual = impostoIBSCBSExtra.AliquotaIBSEstadual;
                            itemExtra.IBSCBS.PercentualReducaoIBSEstadual = impostoIBSCBSExtra.PercentualReducaoIBSEstadual;
                            itemExtra.IBSCBS.ValorIBSEstadual = impostoIBSCBSExtra.ValorIBSEstadual;
                            itemExtra.IBSCBS.AliquotaIBSMunicipal = impostoIBSCBSExtra.AliquotaIBSMunicipal;
                            itemExtra.IBSCBS.PercentualReducaoIBSMunicipal = impostoIBSCBSExtra.PercentualReducaoIBSMunicipal;
                            itemExtra.IBSCBS.ValorIBSMunicipal = impostoIBSCBSExtra.ValorIBSMunicipal;
                            itemExtra.IBSCBS.AliquotaCBS = impostoIBSCBSExtra.AliquotaCBS;
                            itemExtra.IBSCBS.PercentualReducaoCBS = impostoIBSCBSExtra.PercentualReducaoCBS;
                            itemExtra.IBSCBS.ValorCBS = impostoIBSCBSExtra.ValorCBS;
                        }

                        cte.ItensNFSe.Add(itemExtra);
                    }
                }


                cte.ISS.BaseCalculo = valorTotalItens;
                cte.ISS.Aliquota = faturamentoClienteServico.FaturamentoMensalCliente.Servico.AliquotaISS;
                cte.ISS.Valor = ((faturamentoClienteServico.FaturamentoMensalCliente.Servico.AliquotaISS / 100) * valorTotalItens);
                cte.ISS.ValorRetencao = 0;
                decimal baseCalculoTotalIBSCBS = valorTotalItens;

                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoTotalIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                {
                    BaseCalculo = baseCalculoTotalIBSCBS,
                    ValoAbaterBaseCalculo = valorISS,
                    CodigoLocalidade = localidadeItem.Codigo,
                    SiglaUF = localidadeItem.Estado.Sigla,
                    CodigoTipoOperacao = 0,
                    Empresa = this.Usuario.Empresa
                });

                if (impostoTotalIBSCBS != null)
                {
                    cte.IBSCBS = new Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS();

                    cte.IBSCBS.NBS = faturamentoClienteServico.FaturamentoMensalCliente.Servico.ServicoNFSe?.NBS ?? "";
                    cte.IBSCBS.CodigoIndicadorOperacao = impostoTotalIBSCBS.CodigoIndicadorOperacao;
                    cte.IBSCBS.CST = impostoTotalIBSCBS.CST;
                    cte.IBSCBS.ClassificacaoTributaria = impostoTotalIBSCBS.ClassificacaoTributaria;
                    cte.IBSCBS.BaseCalculo = impostoTotalIBSCBS.BaseCalculo;
                    cte.IBSCBS.AliquotaIBSEstadual = impostoTotalIBSCBS.AliquotaIBSEstadual;
                    cte.IBSCBS.PercentualReducaoIBSEstadual = impostoTotalIBSCBS.PercentualReducaoIBSEstadual;
                    cte.IBSCBS.ValorIBSEstadual = impostoTotalIBSCBS.ValorIBSEstadual;
                    cte.IBSCBS.AliquotaIBSMunicipal = impostoTotalIBSCBS.AliquotaIBSMunicipal;
                    cte.IBSCBS.PercentualReducaoIBSMunicipal = impostoTotalIBSCBS.PercentualReducaoIBSMunicipal;
                    cte.IBSCBS.ValorIBSMunicipal = impostoTotalIBSCBS.ValorIBSMunicipal;
                    cte.IBSCBS.AliquotaCBS = impostoTotalIBSCBS.AliquotaCBS;
                    cte.IBSCBS.PercentualReducaoCBS = impostoTotalIBSCBS.PercentualReducaoCBS;
                    cte.IBSCBS.ValorCBS = impostoTotalIBSCBS.ValorCBS;
                }

                cte.ValorTotalPrestacaoServico = valorTotalItens;
                cte.ValorAReceber = valorTotalItens;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCte.GerarCTePorObjeto(cte, codigoCTe, unitOfWork, "1", 0, "S", modeloDocumentoFiscal, numeroDocumento, TipoServicoMultisoftware);

                cteIntegrado.ValorCSLL = 0;
                cteIntegrado.ValorIR = 0;
                cteIntegrado.ValorINSS = 0;
                cteIntegrado.ValorCOFINS = 0;
                cteIntegrado.ValorPIS = 0;
                cteIntegrado.ISSRetido = issRetido;

                repCTe.Atualizar(cteIntegrado);

                Dominio.Entidades.Embarcador.CTe.CTeParcela parcelaNFSe = new Dominio.Entidades.Embarcador.CTe.CTeParcela();
                parcelaNFSe.Sequencia = 1;
                parcelaNFSe.DataEmissao = DateTime.Now;
                parcelaNFSe.DataVencimento = faturamentoClienteServico.DataVencimento;
                parcelaNFSe.Valor = valorTotalItens;
                parcelaNFSe.ConhecimentoDeTransporteEletronico = cteIntegrado;

                repParcelaNFSe.Inserir(parcelaNFSe);

                //O título e a movimentação serão gerados pela thread GeracaoTituloNFSe

                faturamentoClienteServico.NotaFiscal = null;
                faturamentoClienteServico.NotaFiscalServico = cteIntegrado;
                repFaturamentoMensalClienteServico.Atualizar(faturamentoClienteServico);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno = ex.Message;
                throw;
            }
        }

        #endregion
    }
}
