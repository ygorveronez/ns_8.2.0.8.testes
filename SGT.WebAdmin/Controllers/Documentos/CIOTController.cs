using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/CIOT")]
    public class CIOTController : BaseController
    {
		#region Construtores

		public CIOTController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Número", "Numero", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cód. Verificador", "CodigoVerificador", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Protocolo", "ProtocoloAutorizacao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cargas", "Cargas", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo CIOT", "TipoCIOT", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Operadora", "Operadora", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Final da Viagem", "DataFinalViagem", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Para Fechamento", "DataParaFechamento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 16, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 16, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Filial", "Filial", 16, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Transportador")
                    propOrdenar = "Transportador.Nome";

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);

                int countCIOTs = repCIOT.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Documentos.CIOT> ciots = new List<Dominio.Entidades.Embarcador.Documentos.CIOT>();

                if (countCIOTs > 0)
                    ciots = repCIOT.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countCIOTs);

                var retorno = (from obj in ciots
                               select new
                               {
                                   obj.Codigo,
                                   Descricao = obj.Numero,
                                   obj.Numero,
                                   Cargas = string.Join(", ", obj.CargaCIOT.Select(o => o.Carga.CodigoCargaEmbarcador)),
                                   obj.CodigoVerificador,
                                   obj.ProtocoloAutorizacao,
                                   DataFinalViagem = obj.DataFinalViagem.ToString("dd/MM/yyyy"),
                                   DataParaFechamento = obj.DataParaFechamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   Transportador = obj.Transportador?.Nome,
                                   Situacao = obj.DescricaoSituacao,
                                   obj.Mensagem,
                                   TipoCIOT = obj.CIOTPorPeriodo ? "Por Período" : "Por Viagem",
                                   Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOTHelper.ObterDescricao(obj.Operadora),
                                   Filial = string.Join(", ", obj.CargaCIOT.Select(o => o.Carga?.Filial?.Descricao ?? string.Empty)),
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        public async Task<IActionResult> Abrir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataFinalViagem = Request.GetDateTimeParam("DataFinalViagem");

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Transportador")), out double cpfCnpjTransportador);

                int codEmpresa = Request.GetIntParam("Empresa");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoOperadora = Request.GetIntParam("Operadora");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unidadeTrabalho);

                Dominio.Entidades.Empresa empresa = Empresa;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                    TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    empresa = repEmpresa.BuscarPorCodigo(codEmpresa);

                Dominio.Entidades.Cliente terceiro = repCliente.BuscarPorCPFCNPJ(cpfCnpjTransportador);
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCodigo(codigoMotorista);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPorCodigo(codigoOperadora);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(terceiro, unidadeTrabalho);

                if (veiculo == null)
                    return new JsonpResult(false, true, "É necessário selecionar um veículo para abrir o CIOT.");

                if (motorista == null)
                    return new JsonpResult(false, true, "É necessário selecionar um motorista para abrir o CIOT.");

                if (modalidadeTerceiro == null)
                    return new JsonpResult(false, true, "O transportador informado não está configurado como terceiro, por favor, ajuste o cadastro do transportador e tente novamente.");

                if (configuracaoCIOT == null)
                    return new JsonpResult(false, true, "É necessário selecionar a operadora para abrir o CIOT.");

                bool gerarCIOTPorViagem = configuracaoCIOT.GerarUmCIOTPorViagem;

                if (modalidadeTerceiro.TipoGeracaoCIOT.HasValue)
                    gerarCIOTPorViagem = modalidadeTerceiro.TipoGeracaoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT.PorViagem;

                if (gerarCIOTPorViagem)
                    return new JsonpResult(false, true, "Para configuração de CIOT informada, o mesmo só pode ser gerado por viagem, ou seja, ao gerar uma viagem para esse terceiro o CIOT será gerado automaticamente.");
                else if (modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado)
                    return new JsonpResult(false, true, "Para TAC Independente ou Outros, não é possível gerar CIOT por período.");

                string mensagemErro = "";

                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = svcCIOT.AbrirCIOT(configuracaoCIOT, TipoServicoMultisoftware, terceiro, motorista, veiculo, dataFinalViagem, empresa, unidadeTrabalho, out mensagemErro);

                if (ciot == null)
                    return new JsonpResult(false, true, mensagemErro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, null, "Abriu o CIOT.", unidadeTrabalho);

                return new JsonpResult(new { ciot.Codigo });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao abrir o CIOT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Encerrar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                    return new JsonpResult(false, true, "CIOT não encontrado.");

                if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                    return new JsonpResult(false, true, "O CIOT não pode ser encerrado na situação atual.");

                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                string mensagemErro = "";

                //unidadeTrabalho.Start();

                Servicos.Log.TratarErro("CIOTController Encerrar codigoCiot" + ciot.Codigo.ToString(), "QuitacaoCIOTCarga");
                if (svcCIOT.EncerrarCIOT(ciot, unidadeTrabalho, TipoServicoMultisoftware, out mensagemErro))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, null, "Encerrou o CIOT.", unidadeTrabalho);
                    //unidadeTrabalho.CommitChanges();
                    return new JsonpResult(true);
                }

                //unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, mensagemErro);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao encerrar o CIOT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EncerrarGerencialmente()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/CIOT");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CIOT_EncerrarGerencialmente))
                    return new JsonpResult(false, true, "Você não possui permissão para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                    return new JsonpResult(false, true, "CIOT não encontrado.");

                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                string mensagemErro = "";

                unidadeTrabalho.Start();

                if (!svcCIOT.EncerrarCIOTGerencialmente(out mensagemErro, ciot, Auditado, unidadeTrabalho, TipoServicoMultisoftware))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao encerrar o CIOT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                    return new JsonpResult(false, true, "CIOT não encontrado.");

                if ((ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia || ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao) && ciot.CargaCIOT.Count() <= 0)
                {
                    unidadeTrabalho.Start();

                    ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;

                    repCIOT.Atualizar(ciot);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, null, "Cancelou o CIOT.", unidadeTrabalho);

                    unidadeTrabalho.CommitChanges();

                    return new JsonpResult(true);
                }

                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                if (svcCIOT.CancelarCIOT(ciot, Usuario, TipoServicoMultisoftware, unidadeTrabalho, out string mensagemErro))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, null, "Cancelou o CIOT.", unidadeTrabalho);

                    return new JsonpResult(true);
                }

                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, mensagemErro);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar o CIOT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Interromper()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                    return new JsonpResult(false, true, "CIOT não encontrado.");

                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                string mensagemErro = "";

                if (svcCIOT.InterromperCIOT(ciot, Usuario, TipoServicoMultisoftware, unidadeTrabalho, out mensagemErro))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, null, "Interrompeu o CIOT.", unidadeTrabalho);

                    return new JsonpResult(true);
                }

                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, mensagemErro);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar o CIOT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarIntegracaoQuitacaoAX()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                    return new JsonpResult(false, true, "CIOT não encontrado.");

                string mensagemErro = "";

                List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
                {
                    Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = cargaCIOT.ContratoFrete;
                    if (contrato != null)
                    {
                        mensagemErro = "";
                        Servicos.Embarcador.Terceiros.ContratoFrete.RealizarCompensacaoAX(contrato, unidadeTrabalho, out mensagemErro);
                        if (!string.IsNullOrWhiteSpace(mensagemErro))
                            return new JsonpResult(false, false, mensagemErro);
                    }
                }

                if (string.IsNullOrWhiteSpace(mensagemErro))
                    return new JsonpResult(true, true, "Sucesso");
                else
                    return new JsonpResult(false, false, mensagemErro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar integração com AX do CIOT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarConciliacaoCIOT()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.CIOT.CIOT.ConciliarCIOTs(TipoServicoMultisoftware, unidadeTrabalho, true);

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao conciliar o CIOT.");
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

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                    return new JsonpResult(false, true, "CIOT não encontrado.");

                return new JsonpResult(new
                {
                    ciot.Codigo,
                    ciot.Numero,
                    ciot.CodigoVerificador,
                    DataFinalViagem = ciot.DataFinalViagem.ToString("dd/MM/yyyy"),
                    DataAbertura = ciot.DataAbertura?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataFechamento = ciot.DataEncerramento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataCancelamento = ciot.DataCancelamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    ciot.Situacao,
                    ciot.DescricaoSituacao,
                    Operadora = ciot.ConfiguracaoCIOT?.Codigo.ToString() ?? "",
                    Empresa = new
                    {
                        Codigo = ciot.Contratante?.Codigo ?? 0,
                        Descricao = ciot.Contratante?.Descricao ?? string.Empty
                    },
                    Transportador = new
                    {
                        Descricao = ciot.Transportador.Nome,
                        Codigo = ciot.Transportador.CPF_CNPJ_SemFormato
                    },
                    Veiculo = new
                    {
                        Codigo = ciot.Veiculo?.Codigo ?? 0,
                        Descricao = ciot.Veiculo?.Descricao ?? string.Empty
                    },
                    Motorista = new
                    {
                        Codigo = ciot.Motorista?.Codigo ?? 0,
                        Descricao = ciot.Motorista?.Descricao ?? string.Empty
                    },
                    integracao.PossuiIntegracaoAX
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados do CIOT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadContrato()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCIOT = 0;
                int.TryParse(Request.Params("Codigo"), out codigoCIOT);

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return new JsonpResult(true, false, "CIOT não encontrado, atualize a página e tente novamente.");

                if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado &&
                    ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto &&
                    ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado)
                    return new JsonpResult(true, false, "O status do CIOT não permite a geração do contrato.");

                string mensagemErro = string.Empty;

                byte[] pdf = new Servicos.Embarcador.CIOT.CIOT().GerarContratoFrete(ciot.Codigo, unidadeTrabalho, out mensagemErro);

                if (pdf == null)
                    return new JsonpResult(true, false, mensagemErro);

                return Arquivo(pdf, "application/pdf", "CIOT-" + ciot.Numero + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do contrato de frete.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigoArquivo(codigo);

                if (ciot == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo arquivoIntegracao = ciot.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Integração CIOT " + ciot.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);

                grid.setarQuantidadeTotal(ciot.ArquivosTransacao.Count());

                var retorno = (from obj in ciot.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT ObterFiltrosPesquisa()
        {
            var situacoes = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>("Situacoes");
            var tiposTransportador = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>("TiposTransportador");

            return new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CpfCnpjTransportador = Request.GetDoubleParam("Transportador"),
                TiposTransportador = tiposTransportador,
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>("Situacao"),
                Situacoes = situacoes,
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                Operadora = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT>("Operadora"),
                CodigoVerificador = Request.GetStringParam("CodigoVerificador"),
                Numero = Request.GetStringParam("Numero"),
                CodigosFiliais = Request.GetListParam<int>("Filial")
            };
        }

        #endregion
    }
}
