using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "BuscarDetalhesDevolucao", "BuscarSituacoes", "DownloadComprovanteEntrega" }, "Pallets/Devolucao")]
    public class DevolucaoController : BaseController
    {
		#region Construtores

		public DevolucaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoPallet filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoPallet()
                {
                    DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                    DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                    DataBaixaInicial = Request.GetDateTimeParam("DataBaixaInicial"),
                    DataBaixaFinal = Request.GetDateTimeParam("DataBaixaFinal"),
                    NumeroCarga = Request.GetStringParam("Carga"),
                    NumeroNotaFiscal = Request.GetIntParam("NotaFiscal"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoMotorista = Request.GetIntParam("Motorista"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    NumeroDevolucao = Request.GetIntParam("NumeroDevolucao"),
                    CodigosTransportador = Request.GetListParam<int>("Transportador"),
                    Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet>("Situacao"),
                    CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                    CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                    CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                    CodigoTomador = Request.GetDoubleParam("Tomador"),
                    NaoExibirSemNotaFiscal = configuracaoPaletes?.NaoExibirDevolucaoPaletesSemNotaFiscal ?? false,
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    filtrosPesquisa.CodigosTransportador = new List<int>() { Empresa.Codigo };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("CodigoPallet", false);


                grid.AdicionarCabecalho("Nº Devolução", "NumeroDevolucao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("N° Pedido", "NumeroPedido", 10, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 10, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Nº Pallets", "Pallets", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Entregues", "PalletsEntregues", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data de Transporte", "DataTransporte", 10, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, false);
                }
                else
                    grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 12, Models.Grid.Align.left, true);


                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeTrabalho);
                Repositorio.Embarcador.Pallets.ValePallet repositorioPalet = new Repositorio.Embarcador.Pallets.ValePallet (unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> listaDevolucao = repDevolucao.Consultar(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repDevolucao.ContarConsulta(filtrosPesquisa));

                grid.AdicionaRows((
                    from p in listaDevolucao
                    select new
                    {
                        p.Codigo,
                        p.Situacao,
                        NumeroDevolucao = p.NumeroDevolucao > 0 ? p.NumeroDevolucao.ToString() : "",
                        Carga = p.CargaPedido?.Carga.CodigoCargaEmbarcador ?? "Sem carga",
                        NotaFiscal = p.XMLNotaFiscal?.Numero,
                        NumeroPedido = p.CargaPedido?.Pedido.NumeroPedidoEmbarcador,
                        Remetente = p.CargaPedido?.Pedido.Remetente?.Descricao,
                        Pallets = p.QuantidadePallets,
                        PalletsEntregues = p.Situacoes?.Where(o => o.AcresceSaldo).Sum(o => o.Quantidade) ?? 0,
                        DataTransporte = p.DataTransporte?.ToString("dd/MM/yyyy"),
                        Transportador = p.Transportador.RazaoSocial,
                        Motorista = p.CargaPedido?.Carga.RetornarMotoristas ?? string.Empty,
                        Veiculo = p.CargaPedido?.Carga.RetornarPlacas ?? string.Empty,
                        p.DescricaoSituacao,
                        CodigoPallet = repositorioPalet?.BuscarPorCodigoDevolucao(p.Codigo)?.Codigo ?? 0
                    }).ToList()
                );

                return new JsonpResult(grid);
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
        public async Task<IActionResult> PesquisaDevolucaoSemValePallet()
        {
            var unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigoTransportador = Request.GetIntParam("Transportador");
                var dataEmissaoInicial = Request.GetNullableDateTimeParam("DataEmissaoInicial");
                var dataEmissaoFinal = Request.GetNullableDateTimeParam("DataEmissaoFinal");
                var numeroCarga = Request.Params("Carga");
                var numeroNotaFiscal = Request.GetIntParam("NotaFiscal");
                var cargaOuNotaFiscal = Request.Params("CargaOuNotaFiscal");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoTransportador = Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Pallets", "Pallets", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data de Transporte", "DataTransporte", 10, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
                else
                    grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var repositorioDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeTrabalho);
                var listaDevolucao = repositorioDevolucao.ConsultarDevolucaoSemValePallet(numeroNotaFiscal, numeroCarga, cargaOuNotaFiscal, codigoTransportador, dataEmissaoInicial, dataEmissaoFinal, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorioDevolucao.ContarConsultaDevolucaoSemValePallet(numeroNotaFiscal, numeroCarga, cargaOuNotaFiscal, codigoTransportador, dataEmissaoInicial, dataEmissaoFinal));

                grid.AdicionaRows((
                    from devolucao in listaDevolucao
                    select new
                    {
                        devolucao.Codigo,
                        devolucao.Descricao,
                        Carga = devolucao.CargaPedido?.Carga.CodigoCargaEmbarcador ?? "Sem carga",
                        NotaFiscal = devolucao.XMLNotaFiscal?.Numero,
                        Pallets = devolucao.QuantidadePallets,
                        DataTransporte = devolucao.CargaPedido?.Carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? devolucao.XMLNotaFiscal?.DataEmissao.ToString("dd/MM/yyyy"),
                        Transportador = devolucao.Transportador.RazaoSocial,
                        Motorista = devolucao.CargaPedido?.Carga.RetornarMotoristas ?? string.Empty
                    }
                ).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception escecao)
            {
                Servicos.Log.TratarErro(escecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repDevolucao.BuscarPorCodigo(codigo);

                if (devolucao == null)
                    return new JsonpResult(false, true, "Devolução de pallets não encontrada.");

                Repositorio.Embarcador.Pallets.DevolucaoPalletAnexo repositorioDevolucaoPalletAnexo = new Repositorio.Embarcador.Pallets.DevolucaoPalletAnexo(unidadeTrabalho);
                
                List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletAnexo> anexos = repositorioDevolucaoPalletAnexo.BuscarPorDevolucao(devolucao.Codigo);
                var quantidadePalletsValePallet = ObterQuantidadePalletsValePallet(unidadeTrabalho, codigo);


                return new JsonpResult(new
                {
                    devolucao.Codigo,
                    NotaFiscal = devolucao.XMLNotaFiscal?.Numero,
                    devolucao.NumeroDevolucao,
                    Carga = devolucao.CargaPedido?.Carga.CodigoCargaEmbarcador ?? "Sem carga",
                    Veiculo = devolucao.CargaPedido?.Carga.RetornarPlacas ?? string.Empty,
                    Motorista = devolucao.CargaPedido?.Carga.RetornarMotoristas ?? string.Empty,
                    Transportador = devolucao.Transportador.RazaoSocial,
                    NumeroPallets = devolucao.QuantidadePallets,
                    NumeroPalletsValePallet = quantidadePalletsValePallet,
                    NumeroTotalPallets = devolucao.QuantidadePallets + quantidadePalletsValePallet,
                    Anexos = (from anexo in anexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo
                              }).ToList()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados para devolução de pallets.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarSituacoes()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet repSituacao = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> situacoes = repSituacao.BuscarAtivos();

                return new JsonpResult((from obj in situacoes
                                        select new
                                        {
                                            obj.Codigo,
                                            obj.Descricao,
                                            obj.ValorUnitario
                                        }).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados das situações de devolução de pallets.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pallets/Devolucao");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, false, "Não é possível realizar esta operação para este tipo de empresa.");

                int codigo, codigoFilial;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                double cpfCnpjCliente = Request.GetDoubleParam("Cliente");
                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeTrabalho);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repDevolucao.BuscarPorCodigo(codigo, true);

                if (devolucao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega)
                    return new JsonpResult(false, true, "A situação da devolução de pallets não permite a baixa.");

                var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unidadeTrabalho);
                var valePallet = repositorioValePallet.BuscarPorDevolucaoComValePalletNaoDevolvido(devolucao.Codigo);

                DateTime? dataDevolucao = DateTime.Now;

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pallets_PermiteDataRetroativa_DevolucaoPallet))
                    dataDevolucao = Request.GetNullableDateTimeParam("DataBaixa");

                if (valePallet != null)
                    return new JsonpResult(false, true, $"O vale pallet n° {valePallet.Numero} ainda não foi baixado.");

                if (!dataDevolucao.HasValue)
                    return new JsonpResult(false, true, "A data da baixa é obrigatória.");

                devolucao.NumeroDevolucao = repDevolucao.BuscarProximoCodigo();
                devolucao.Cliente = cpfCnpjCliente > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente) : null;
                devolucao.DataDevolucao = dataDevolucao.Value;
                devolucao.Filial = codigoFilial > 0 ? repFilial.BuscarPorCodigo(codigoFilial) : null;
                devolucao.Observacao = observacao;
                devolucao.Usuario = Usuario;
                devolucao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue;

                unidadeTrabalho.Start();

                repDevolucao.Atualizar(devolucao, Auditado);

                SalvarSituacoes(devolucao, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados das situações de devolução de pallets.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> CancelarBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, false, "Não é possível realizar esta operação para este tipo de empresa.");

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
                Repositorio.Embarcador.Pallets.CancelamentoBaixaPallets repCancelamentoBaixaPallets = new Repositorio.Embarcador.Pallets.CancelamentoBaixaPallets(unitOfWork);

                int codigo = Request.GetIntParam("Devolucao");
                string motivo = Request.GetStringParam("Motivo");

                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repDevolucao.BuscarPorCodigo(codigo, true);

                if (devolucao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue)
                    return new JsonpResult(false, true, "A situação da devolução de pallets não permite o cancelamento.");

                if (devolucao.Fechamento != null && devolucao.Fechamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets.Finalizado)
                    return new JsonpResult(false, true, "Não é possível cancelar a devolução pois o fechamento " + devolucao.Fechamento.Numero + " já foi finalizado.");

                devolucao.NumeroDevolucao = 0;
                devolucao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega;
                devolucao.DataDevolucao = null;

                Dominio.Entidades.Embarcador.Pallets.CancelamentoBaixaPallets cancelamento = new Dominio.Entidades.Embarcador.Pallets.CancelamentoBaixaPallets()
                {
                    Devolucao = devolucao,
                    DataCancelamento = DateTime.Now,
                    Usuario = this.Usuario,
                    Observacao = motivo
                };

                unitOfWork.Start();
                repDevolucao.Atualizar(devolucao);
                repCancelamentoBaixaPallets.Inserir(cancelamento, Auditado);
                ProcessaCancelamentoBaixa(cancelamento, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, devolucao, "Devolução cancelada.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar a baixa da devolução.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> EnviarPallet()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<Dominio.ObjetosDeValor.Embarcador.Pallets.ValePallet> listaValePalet = new List<Dominio.ObjetosDeValor.Embarcador.Pallets.ValePallet>();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        Dominio.ObjetosDeValor.Embarcador.Pallets.ValePallet novovalePallet = new Dominio.ObjetosDeValor.Embarcador.Pallets.ValePallet();
                        novovalePallet.NomeArquivo = file.FileName;
                        novovalePallet.Processado = true;

                        byte[] arquivo = null;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            arquivo = memoryStream.ToArray();
                            novovalePallet.PDF = arquivo;
                        }

                    }

                    unitOfWork.CommitChanges();
                    return new JsonpResult(listaValePalet);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar pallet");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadComprovanteEntrega()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, false, "Não é possível realizar esta operação para este tipo de empresa.");

                int codigoDevolucao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoDevolucao);

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucaoPallets = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repDevolucaoPallets.BuscarPorCodigo(codigoDevolucao);

                if (devolucao == null)
                    return new JsonpResult(true, false, "Devolução de pallets não encontrada, atualize a página e tente novamente.");

                if (devolucao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue)
                    return new JsonpResult(true, false, "A situação da devolução de pallets não permite a geração do comprovante de entrega.");

                string mensagemErro = string.Empty;

                byte[] pdf = new Servicos.Embarcador.Pallets.DevolucaoPallets(unidadeTrabalho).GerarComprovanteEntrega(devolucao.Codigo, out mensagemErro);

                if (pdf == null)
                    return new JsonpResult(true, false, mensagemErro);

                return Arquivo(pdf, "application/pdf", $"Comprovante de Entrega de Pallets{(devolucao.XMLNotaFiscal == null ? "" : $" - NF {devolucao.XMLNotaFiscal.Numero}")}.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do comprovante de entrega.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDetalhesDevolucao()
        {

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorioDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeTrabalho);
                var devolucao = repositorioDevolucao.BuscarPorCodigo(codigo);

                if (devolucao == null)
                    return new JsonpResult(false, true, "Devolução de pallets não encontrada.");

                Repositorio.Embarcador.Pallets.DevolucaoPalletAnexo repositorioDevolucaoPalletAnexo = new Repositorio.Embarcador.Pallets.DevolucaoPalletAnexo(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletAnexo> anexos = repositorioDevolucaoPalletAnexo.BuscarPorDevolucao(devolucao.Codigo);
                var quantidadePalletsValePallet = ObterQuantidadePalletsValePallet(unidadeTrabalho, codigo);

                var dynDevolucao = new
                {
                    Chave = devolucao.XMLNotaFiscal != null ? devolucao.XMLNotaFiscal.Chave : "",
                    Numero = devolucao.XMLNotaFiscal?.Numero + devolucao.XMLNotaFiscal?.Serie,
                    Empresa = devolucao.Transportador.RazaoSocial,
                    Destinatario = devolucao.XMLNotaFiscal?.Destinatario?.Nome,
                    DataEmissao = devolucao.XMLNotaFiscal?.DataEmissao.ToString("dd/MM/yyyy"),
                    Valor = devolucao.XMLNotaFiscal?.Valor.ToString("n2"),
                    Peso = devolucao.XMLNotaFiscal?.Peso.ToString("n2"),
                    devolucao.XMLNotaFiscal?.NaturezaOP,
                    Filial = devolucao.Filial != null ? devolucao.Filial.Descricao : "",
                    Carga = devolucao.CargaPedido != null ? devolucao.CargaPedido.Carga.CodigoCargaEmbarcador : " - ",
                    Motoristas = devolucao.CargaPedido?.Carga.RetornarMotoristas ?? string.Empty,
                    Emitente = devolucao.XMLNotaFiscal?.Emitente != null ? devolucao.XMLNotaFiscal.Emitente.Nome : "",
                    devolucao.Observacao,
                    ValorTotalPallets = devolucao.ValorTotalPallets.ToString("n2"),
                    devolucao.QuantidadePallets,
                    QuantidadePalletsValePallet = quantidadePalletsValePallet,
                    QuantidadeTotalPallets = devolucao.QuantidadePallets + quantidadePalletsValePallet,
                    NumeroDevolucao = devolucao.NumeroDevolucao > 0 ? devolucao.NumeroDevolucao.ToString() : "",
                    PesoTotalPallets = devolucao.PesoTotalPallets.ToString("n2"),
                    Situacoes = (
                        from obj in devolucao.Situacoes
                        select new
                        {
                            Situacao = obj.Situacao.Descricao,
                            ValorTotal = obj.ValorTotal.ToString("n2"),
                            ValorUnitario = obj.ValorUnitario.ToString("n2"),
                            obj.Quantidade
                        }
                    ).ToList(),
                    Anexos = (from anexo in anexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo
                              }).ToList()
                };
                return new JsonpResult(dynDevolucao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados para devolução de pallets.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Devolucao");
                int quantidadePallets = Request.GetIntParam("QuantidadePallets");

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeTrabalho);

                Servicos.Embarcador.Pallets.DevolucaoPallets servicoDevolucaoPallets = new Servicos.Embarcador.Pallets.DevolucaoPallets(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repDevolucao.BuscarPorCodigo(codigo, true);

                if (devolucao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (devolucao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado)
                    return new JsonpResult(false, true, "Não é permitido alterar a devolução de pallets na situação cancelado.");

                if (quantidadePallets == 0)
                    return new JsonpResult(false, true, "A quantidade de pallets não pode ser zerada.");

                int quantidadePalletsAnterior = devolucao.QuantidadePallets;
                if (quantidadePalletsAnterior == quantidadePallets)
                    return new JsonpResult(false, true, "A quantidade de pallets informada é a mesma que está atualmente, não sendo permitido alterar.");

                unidadeTrabalho.Start();

                devolucao.QuantidadePallets = quantidadePallets;
                devolucao.Usuario = Usuario;

                repDevolucao.Atualizar(devolucao, Auditado);

                servicoDevolucaoPallets.Atualizar(devolucao, quantidadePalletsAnterior, TipoServicoMultisoftware);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a devolução de pallets.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "CargaPedido.Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Veiculo")
                return "CargaPedido.Carga.Veiculo.Placa";

            if (propriedadeOrdenar == "NotaFiscal")
                return "XMLNotaFiscal.Numero";

            if (propriedadeOrdenar == "NumeroPedido")
                return "CargaPedido.Pedido.NumeroPedidoEmbarcador";

            if (propriedadeOrdenar == "Transportador")
                return "CargaPedido.Carga.Empresa.RazaoSocial";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";

            return propriedadeOrdenar;
        }

        private int ObterQuantidadePalletsValePallet(Repositorio.UnitOfWork unidadeTrabalho, int codigoDevolucao)
        {
            var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unidadeTrabalho);
            var valePallet = repositorioValePallet.BuscarPorDevolucaoComValePalletDevolvido(codigoDevolucao);

            return valePallet?.Quantidade ?? 0;
        }

        private void ProcessaCancelamentoBaixa(Dominio.Entidades.Embarcador.Pallets.CancelamentoBaixaPallets cancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pallets.DevolucaoPalletSituacao repositorioDevolucaoPalletSituacao = new Repositorio.Embarcador.Pallets.DevolucaoPalletSituacao(unitOfWork);
            Servicos.Embarcador.Pallets.EstoquePallet servicoEstoquePallet = new Servicos.Embarcador.Pallets.EstoquePallet(unitOfWork);
            int quantidadeTotal = 0;
            int quantidadeTotalDescartada = 0;

            foreach (Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao situacaoDevolucao in cancelamento.Devolucao.Situacoes)
            {
                if (situacaoDevolucao.AcresceSaldo)
                    quantidadeTotal += situacaoDevolucao.Quantidade;

                if (situacaoDevolucao.Situacao.SituacaoPalletDescartado)
                    quantidadeTotalDescartada += situacaoDevolucao.Quantidade;

                repositorioDevolucaoPalletSituacao.Deletar(situacaoDevolucao);
            }

            if (quantidadeTotal > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipo = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador;
                var dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CpfCnpjCliente = cancelamento.Devolucao.Cliente?.CPF_CNPJ ?? 0d,
                    CodigoFilial = cancelamento.Devolucao.Filial?.Codigo ?? 0,
                    CodigoTransportador = cancelamento.Devolucao.Transportador.Codigo,
                    CancelamentoBaixaPallets = cancelamento,
                    Quantidade = quantidadeTotal,
                    QuantidadeDescartada = quantidadeTotalDescartada,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoOperacaoMovimentacao = tipo,
                    TipoServicoMultisoftware = TipoServicoMultisoftware,
                    CodigoGrupoPessoas = cancelamento?.Devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
                };

                servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
            }
        }

        private void SalvarSituacoes(Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            var repositorioSituacaoDevolucao = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unidadeTrabalho);
            var repositorioDevolucaoPalletSituacao = new Repositorio.Embarcador.Pallets.DevolucaoPalletSituacao(unidadeTrabalho);
            var situacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("QuantidadeSituacoes"));
            int quantidadeTotal = 0;
            int quantidadeTotalDescartada = 0;

            foreach (var situacao in situacoes)
            {
                var situacaoDevolucao = repositorioSituacaoDevolucao.BuscarPorCodigo((int)situacao.Codigo);
                var quantidade = (int)situacao.Quantidade;

                if (situacaoDevolucao.AcresceSaldo)
                    quantidadeTotal += quantidade;

                if (situacaoDevolucao.SituacaoPalletDescartado)
                    quantidadeTotalDescartada += quantidade;

                var devolucaoSituacao = new Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao()
                {
                    AcresceSaldo = situacaoDevolucao.AcresceSaldo,
                    Devolucao = devolucao,
                    Quantidade = quantidade,
                    Situacao = situacaoDevolucao,
                    ValorTotal = situacaoDevolucao.ValorUnitario * quantidade,
                    ValorUnitario = situacaoDevolucao.ValorUnitario
                };

                repositorioDevolucaoPalletSituacao.Inserir(devolucaoSituacao);
            }

            if (quantidadeTotal > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipo = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial;
                var servicoEstoquePallet = new Servicos.Embarcador.Pallets.EstoquePallet(unidadeTrabalho);
                var dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CpfCnpjCliente = devolucao.Cliente?.CPF_CNPJ ?? 0d,
                    CodigoFilial = devolucao.Filial?.Codigo ?? 0,
                    CodigoTransportador = devolucao.Transportador.Codigo,
                    DevolucaoPallet = devolucao,
                    Quantidade = quantidadeTotal,
                    QuantidadeDescartada = quantidadeTotalDescartada,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoOperacaoMovimentacao = tipo,
                    TipoServicoMultisoftware = TipoServicoMultisoftware,
                    CodigoGrupoPessoas = devolucao.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
                };

                servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
            }
        }

        #endregion
    }
}
