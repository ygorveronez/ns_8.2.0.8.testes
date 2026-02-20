using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.FaturamentoMensal
{

    [CustomAuthorize("FaturamentoMensal/ValidacaoFaturamentoMensal")]
    public class ValidacaoFaturamentoMensalController : BaseController
    {
		#region Construtores

		public ValidacaoFaturamentoMensalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais        

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarEmpresaFaturamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoGrupoFaturamento = 0;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(codigoGrupoFaturamento);
                if (faturamentoMensalGrupo == null)
                    codigoGrupoFaturamento = -1;
                else if (!faturamentoMensalGrupo.FaturamentoAutomatico)
                    return new JsonpResult(false, "Favor selecione um grupo de faturamento que esteja configurado para buscar valores automaticamente.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("CNPJCliente", false);
                grid.AdicionarCabecalho("CodigoFaturamentoMensalCliente", false);
                grid.AdicionarCabecalho("Empresa", "Empresa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dia Faturamento", "DiaFaturamento", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Próximo Vencimento", "ProximoVencimento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Último Vencimento", "UltimoVencimento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor Faturamento", "ValorFaturamento", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Cadastro Faturamento", "CadastroFaturamento", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Plano Mensal", "PlanoMensal", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Qtd. Documentos", "QtdDocumento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Qtd. NF-e", "QtdNFe", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Qtd. NFS-e", "QtdNFSe", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Qtd. Boletos", "QtdBoleto", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Qtd. Títulos", "QtdTitulo", 8, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                propOrdenar = "RazaoSocial";

                //List<Dominio.Entidades.Empresa> listaEmpresa = repEmpresa.ConsultarEmpresaFaturamento(codigoGrupoFaturamento, this.Usuario.Empresa.Codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repEmpresa.ContarListaEmpresaFaturamentoMensal(this.Usuario.Empresa.Codigo, codigoGrupoFaturamento));
                var lista = ConfiguraListaEmpresa(this.Usuario.Empresa.Codigo, unitOfWork, faturamentoMensalGrupo, codigoGrupoFaturamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristasa ativos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarEmpresas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigoGrupoFaturamento = 0;
                int.TryParse(Request.Params("CodigoGrupoFaturamento"), out codigoGrupoFaturamento);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo;
                if (codigoGrupoFaturamento > 0)
                {
                    faturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(codigoGrupoFaturamento);
                }
                else
                {
                    return new JsonpResult(false, "Favor selecione um grupo de faturamento.");
                }

                if (faturamentoMensalGrupo.DiaFatura <= 0 || !faturamentoMensalGrupo.FaturamentoAutomatico || faturamentoMensalGrupo.TipoMovimento == null || faturamentoMensalGrupo.Servico == null)
                    return new JsonpResult(false, "O grupo de faturamento selecionado não está configurado corretamente.");

                SalvarEmpresasFaturamento(unitOfWork, faturamentoMensalGrupo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensalGrupo, null, "Salvou Empresas.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir as novas empresas no faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> GerarRelatorioEmpresas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigoGrupoFaturamento = 0;
                int.TryParse(Request.Params("CodigoGrupoFaturamento"), out codigoGrupoFaturamento);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo;
                if (codigoGrupoFaturamento > 0)
                {
                    faturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(codigoGrupoFaturamento);
                }
                else
                {
                    return new JsonpResult(false, "Favor selecione um grupo de faturamento.");
                }

                if (faturamentoMensalGrupo.DiaFatura <= 0 || !faturamentoMensalGrupo.FaturamentoAutomatico || faturamentoMensalGrupo.TipoMovimento == null || faturamentoMensalGrupo.Servico == null)
                    return new JsonpResult(false, "O grupo de faturamento selecionado não está configurado corretamente.");

                GerarRelatorio(faturamentoMensalGrupo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir as novas empresas no faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        #endregion

        private void SalvarEmpresasFaturamento(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo)
        {
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            List<Dominio.Entidades.Empresa> listaEmpresa = repEmpresa.ConsultarEmpresaFaturamento(faturamentoMensalGrupo.Codigo, this.Usuario.Empresa.Codigo, "", "", 0, 0);
            var lista = ConfiguraListaEmpresa(this.Usuario.Empresa.Codigo, unidadeDeTrabalho, faturamentoMensalGrupo, faturamentoMensalGrupo.Codigo, "", "", 0, 0);

            if (lista.Count > 0)
            {
                foreach (var dynEmpresa in lista)
                {
                    if (dynEmpresa.CodigoFaturamentoMensalCliente == 0)
                    {
                        double cnpjCliente = dynEmpresa.CNPJCliente;
                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjCliente);
                        if (cliente == null)
                        {
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(dynEmpresa.CodigoEmpresa);
                            Dominio.Entidades.Cliente pessoa = Servicos.Embarcador.Pessoa.Pessoa.Converter(empresa, unidadeDeTrabalho);
                            pessoa.Ativo = true;
                            repCliente.Inserir(pessoa);
                            cnpjCliente = pessoa.CPF_CNPJ;
                        }
                        else
                            cnpjCliente = dynEmpresa.CNPJCliente;

                        Servicos.Log.TratarErro(cnpjCliente.ToString());
                        cliente = repCliente.BuscarPorCPFCNPJ(cnpjCliente);

                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamentoMensalCliente = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente
                        {
                            Ativo = true,
                            BoletoConfiguracao = faturamentoMensalGrupo.BoletoConfiguracao,
                            DiaFatura = faturamentoMensalGrupo.DiaFatura,
                            Empresa = faturamentoMensalGrupo.Empresa,
                            FaturamentoMensalGrupo = faturamentoMensalGrupo,
                            Pessoa = cliente,
                            Servico = faturamentoMensalGrupo.Servico,
                            TipoMovimento = faturamentoMensalGrupo.TipoMovimento,
                            TipoObservacaoFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum,
                            ValorAdesao = 0,
                            ValorServicoPrincipal = 0
                        };

                        if (cliente.Localidade.Estado.Sigla == faturamentoMensalGrupo.Empresa.Localidade.Estado.Sigla)
                            faturamentoMensalCliente.NaturezaDaOperacao = faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado;
                        else
                            faturamentoMensalCliente.NaturezaDaOperacao = faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado;

                        repFaturamentoMensalCliente.Inserir(faturamentoMensalCliente, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, faturamentoMensalCliente, null, "Criou por Salvar Clientes.", unidadeDeTrabalho);
                    }
                }
            }
        }

        private dynamic ConfiguraListaEmpresa(int codigoEmpresaPai, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo, int codigoGrupoFaturamento, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);

            Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unitOfWork);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

            IList<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento> listaRelatorio = repEmpresa.ListaEmpresaFaturamentoMensal(codigoEmpresaPai, codigoGrupoFaturamento, propOrdenar, dirOrdena, inicio, limite);

            var lista2 = (from p in listaRelatorio
                          orderby p.Empresa
                          select new
                          {
                              p.CodigoEmpresa,
                              p.CNPJCliente,
                              p.CodigoFaturamentoMensalCliente,
                              p.Empresa,
                              p.DiaFaturamento,
                              ProximoVencimento = p.CodigoFaturamentoMensalCliente > 0 ? servFaturamentoMensal.ProximaDataVencimento(p.CodigoFaturamentoMensalCliente, unitOfWork).HasValue ? servFaturamentoMensal.ProximaDataVencimento(p.CodigoFaturamentoMensalCliente, unitOfWork).Value.ToString("dd/MM/yyyy") : servFaturamentoMensal.ProximaDataVencimento(int.Parse(p.DiaFaturamento)).Value.ToString("dd/MM/yyyy") : servFaturamentoMensal.ProximaDataVencimento(int.Parse(p.DiaFaturamento)).Value.ToString("dd/MM/yyyy"),
                              UltimoVencimento = p.CodigoFaturamentoMensalCliente > 0 ? servFaturamentoMensal.UltimaDataVencimento(p.CodigoFaturamentoMensalCliente, unitOfWork).HasValue ? servFaturamentoMensal.UltimaDataVencimento(p.CodigoFaturamentoMensalCliente, unitOfWork).Value.ToString("dd/MM/yyyy") : "" : "",
                              ValorFaturamento = "",
                              p.CadastroFaturamento,
                              PlanoMensal = "",
                              QtdDocumento = "",
                              QtdNFe = "",
                              QtdNFSe = "",
                              QtdBoleto = "",
                              QtdTitulo = ""
                          }).ToList();

            var lista3 = (from p in lista2
                          orderby p.Empresa
                          select new
                          {
                              p.CodigoEmpresa,
                              p.CNPJCliente,
                              p.CodigoFaturamentoMensalCliente,
                              p.Empresa,
                              p.DiaFaturamento,
                              p.ProximoVencimento,
                              p.UltimoVencimento,
                              ValorFaturamento = !string.IsNullOrWhiteSpace(p.ProximoVencimento) ? servFaturamentoMensal.ValorTotalFaturamentoCliente(p.CodigoFaturamentoMensalCliente, DateTime.Parse(p.ProximoVencimento), unitOfWork, p.CNPJCliente).ToString("n2") : "0,00",
                              p.CadastroFaturamento,
                              PlanoMensal = "",
                              QtdDocumento = "",
                              QtdNFe = !string.IsNullOrWhiteSpace(p.ProximoVencimento) ? repNotaFiscal.QuantidadeNotaFiscal(p.CodigoEmpresa, Dominio.Enumeradores.TipoAmbiente.Producao, DateTime.Parse(p.ProximoVencimento).AddMonths(-1)) : 0,
                              QtdNFSe = !string.IsNullOrWhiteSpace(p.ProximoVencimento) ? repCTe.QuantidadeNotaServico(p.CodigoEmpresa, Dominio.Enumeradores.TipoAmbiente.Producao, DateTime.Parse(p.ProximoVencimento).AddMonths(-1)) : 0,
                              QtdBoleto = !string.IsNullOrWhiteSpace(p.ProximoVencimento) ? repTitulo.QuantidadeBoletosReceber(p.CodigoEmpresa, Dominio.Enumeradores.TipoAmbiente.Producao, DateTime.Parse(p.ProximoVencimento).AddMonths(-1)) : 0,
                              QtdTitulo = !string.IsNullOrWhiteSpace(p.ProximoVencimento) ? repTitulo.QuantidadeTitulosReceber(p.CodigoEmpresa, Dominio.Enumeradores.TipoAmbiente.Producao, DateTime.Parse(p.ProximoVencimento).AddMonths(-1)) : 0
                          }).ToList();

            var lista4 = (from p in lista3
                          orderby p.Empresa
                          select new
                          {
                              p.CodigoEmpresa,
                              p.CNPJCliente,
                              p.CodigoFaturamentoMensalCliente,
                              p.Empresa,
                              p.DiaFaturamento,
                              p.ProximoVencimento,
                              p.UltimoVencimento,
                              p.ValorFaturamento,
                              p.CadastroFaturamento,
                              PlanoMensal = repPlanoEmissaoNFe.BuscarPlanoEmissao(p.QtdTitulo, p.QtdBoleto, p.QtdNFe, p.QtdNFSe, this.Usuario.Empresa?.Codigo) != null ? repPlanoEmissaoNFe.BuscarPlanoEmissao(p.QtdTitulo, p.QtdBoleto, p.QtdNFe, p.QtdNFSe, this.Usuario.Empresa?.Codigo).PlanoEmissaoNFe.Descricao : "",
                              QtdDocumento = (p.QtdTitulo + p.QtdBoleto + p.QtdNFe + p.QtdNFSe),
                              p.QtdNFe,
                              p.QtdNFSe,
                              p.QtdBoleto,
                              p.QtdTitulo
                          }).ToList();

            return lista4;
        }

        private void GerarRelatorio(Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R072_EmpresasFaturamento, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R072_EmpresasFaturamento, TipoServicoMultisoftware, "Relatório de Empresas para o Faturamento", "FaturamentoMensal", "EmpresasFaturamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "CodigoEmpresa", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                Task.Factory.StartNew(() => GerarRelatorioEmpresasFaturamento(faturamentoMensalGrupo, stringConexao, relatorioControleGeracao));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void GerarRelatorioEmpresasFaturamento(Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                List<Dominio.Entidades.Empresa> listaEmpresa = repEmpresa.ConsultarEmpresaFaturamento(faturamentoMensalGrupo.Codigo, this.Usuario.Empresa.Codigo, "", "", 0, 0);
                var lista = ConfiguraListaEmpresa(this.Usuario.Empresa.Codigo, unitOfWork, faturamentoMensalGrupo, faturamentoMensalGrupo.Codigo, "", "", 0, 0);
                List<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento> listaRelatorio = new List<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento>();
                foreach (var dynEmpresa in lista)
                {
                    Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento empresa = new Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento();
                    empresa.CadastroFaturamento = dynEmpresa.CadastroFaturamento;
                    empresa.CNPJCliente = dynEmpresa.CNPJCliente;
                    empresa.CodigoEmpresa = dynEmpresa.CodigoEmpresa;
                    empresa.CodigoFaturamentoMensalCliente = dynEmpresa.CodigoFaturamentoMensalCliente;
                    empresa.DiaFaturamento = dynEmpresa.DiaFaturamento;
                    empresa.Empresa = dynEmpresa.Empresa;
                    empresa.PlanoMensal = dynEmpresa.PlanoMensal;
                    empresa.ProximoVencimento = dynEmpresa.ProximoVencimento;
                    empresa.QtdBoleto = ((int)dynEmpresa.QtdBoleto).ToString();
                    empresa.QtdDocumento = ((int)dynEmpresa.QtdDocumento).ToString();
                    empresa.QtdNFe = ((int)dynEmpresa.QtdNFe).ToString();
                    empresa.QtdNFSe = ((int)dynEmpresa.QtdNFSe).ToString();
                    empresa.QtdTitulo = ((int)dynEmpresa.QtdTitulo).ToString();
                    empresa.UltimoVencimento = dynEmpresa.UltimoVencimento;
                    empresa.ValorFaturamento = dynEmpresa.ValorFaturamento;
                    listaRelatorio.Add(empresa);
                }

                ReportRequest.WithType(ReportType.EmpresasFaturamento)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("ListaRelatorio", listaRelatorio.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CodigoUsuario", Usuario.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
