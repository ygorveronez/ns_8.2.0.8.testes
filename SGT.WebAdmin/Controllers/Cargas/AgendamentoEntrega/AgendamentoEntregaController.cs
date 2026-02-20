using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SGT.WebAdmin.Controllers.Cargas.AgendamentoEntrega
{
    [CustomAuthorize("Cargas/AgendamentoEntrega")]
    public class AgendamentoEntregaController : BaseController
    {
		#region Construtores

		public AgendamentoEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega = new Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega()
                {
                    Situacao = SituacaoAgendamentoEntrega.AguardandoConfirmacao
                };

                PreencherEntidade(agendamentoEntrega, unitOfWork);

                unitOfWork.Start();

                repositorioAgendamentoEntrega.Inserir(agendamentoEntrega, Auditado);
                agendamentoEntrega.Carga = AdicionarCarga(agendamentoEntrega, unitOfWork);
                servicoJanelaDescarregamento.Adicionar(agendamentoEntrega.Carga, agendamentoEntrega.DataAgendamento, TipoServicoMultisoftware);
                repositorioAgendamentoEntrega.Atualizar(agendamentoEntrega);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega = repositorioAgendamentoEntrega.BuscarPorCodigo(codigo, false);

                if (agendamentoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    agendamentoEntrega.Codigo,
                    ModeloVeicularCarga = new { agendamentoEntrega.ModeloVeicularCarga.Codigo, agendamentoEntrega.ModeloVeicularCarga.Descricao },
                    agendamentoEntrega.Motorista,
                    agendamentoEntrega.Placa,
                    Destinatario = new { agendamentoEntrega.Destinatario.Codigo, agendamentoEntrega.Destinatario.Descricao },
                    agendamentoEntrega.Senha,
                    agendamentoEntrega.SenhaAgendamento,
                    TipoDeCarga = new { agendamentoEntrega.TipoDeCarga.Codigo, agendamentoEntrega.TipoDeCarga.Descricao },
                    agendamentoEntrega.Transportador,
                    agendamentoEntrega.Observacao,
                    Situacao = agendamentoEntrega.Situacao.ObterDescricao(),
                    DataAgendamento = agendamentoEntrega.DataAgendamento.ToString("dd/MM/yyyy HH:mm:ss"),
                    CargaAgendada = agendamentoEntrega.Carga != null
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.Carga AdicionarCarga(Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCNPJ(agendamentoEntrega.Destinatario.CPF_CNPJ_SemFormato);

            if (filial == null)
                throw new ControllerException($"Não foi possível localiar a filial.");

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido servicoProdutoPedido = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = filial.CodigoFilialEmbarcador };
            cargaIntegracao.NumeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, filial?.Codigo ?? 0).ToString();
            cargaIntegracao.NumeroPedidoEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, filial?.Codigo ?? 0).ToString();
            cargaIntegracao.DataPrevisaoChegadaDestinatario = agendamentoEntrega.DataAgendamento.ToString("dd/MM/yyyy HH:mm:ss");

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasExistentes = repositorioCarga.BuscarTodasCargasPorCodigoEmbarcador(cargaIntegracao.NumeroCarga, cargaIntegracao.Filial.CodigoIntegracao, false);

            if (cargasExistentes.Count > 0)
                throw new ControllerException($"Já existe uma carga com o número {cargaIntegracao.NumeroCarga}.");

            Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(agendamentoEntrega.Placa);

            if (veiculo != null)
            {
                cargaIntegracao.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo() { Placa = veiculo.Placa };
                cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = veiculo.Empresa.CNPJ };

                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                if (veiculoMotorista != null)
                {
                    cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
                    cargaIntegracao.Motoristas.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista() { CPF = veiculoMotorista.CPF });
                }

                if (veiculo.ModeloVeicularCarga != null)
                    cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = veiculo.ModeloVeicularCarga.CodigoIntegracao };
            }
            else
                cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = agendamentoEntrega.ModeloVeicularCarga.CodigoIntegracao };

            cargaIntegracao.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = agendamentoEntrega.Destinatario.CPF_CNPJ_SemFormato };
            cargaIntegracao.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = agendamentoEntrega.Remetente.CPF_CNPJ_SemFormato };
            cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = agendamentoEntrega.TipoDeCarga.CodigoTipoCargaEmbarcador };
            cargaIntegracao.ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto() { DescricaoProduto = "Diversos", CodigoProduto = "Diversos" };


            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            StringBuilder mensagemErro = new StringBuilder();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, ref protocoloPedidoExistente, ref codigoCargaExistente, false);

            if (mensagemErro.Length == 0)
            {
                servicoProdutoPedido.AdicionarProdutosPedido(pedido, ConfiguracaoEmbarcador, cargaIntegracao, ref mensagemErro, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref codigoCargaExistente, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, Auditado, ConfiguracaoEmbarcador, null, "", filial, tipoOperacao);

                if (cargaPedido != null)
                {
                    servicoRateioFrete.GerarComponenteICMS(cargaPedido, false, unitOfWork);

                    if (cargaPedido.CargaPedidoFilialEmissora)
                        servicoRateioFrete.GerarComponenteICMS(cargaPedido, true, unitOfWork);

                    servicoRateioFrete.GerarComponenteISS(cargaPedido, false, unitOfWork);
                    servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, unitOfWork, false);
                    carga = cargaPedido.Carga;
                }
            }

            if (mensagemErro.Length > 0)
                throw new ControllerException(mensagemErro.ToString());

            servicoCarga.FecharCarga(carga, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, this.Cliente, adicionarJanelaDescarregamento: false);

            carga.CargaFechada = true;
            carga.NumeroSequenciaCarga = cargaIntegracao.NumeroCarga.ToInt();
            pedido.NumeroSequenciaPedido = carga.NumeroSequenciaCarga;

            repositorioCarga.Atualizar(carga);
            repositorioPedido.Atualizar(pedido);

            return carga;
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.AgendamentoEntrega repAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            agendamentoEntrega.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeicularCarga"));
            agendamentoEntrega.Motorista = Request.GetStringParam("Motorista");
            agendamentoEntrega.Placa = Request.GetStringParam("Placa");
            agendamentoEntrega.Remetente = this.Usuario.Cliente;
            agendamentoEntrega.Destinatario = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Destinatario"));

            if (agendamentoEntrega.Codigo == 0)
            {
                agendamentoEntrega.Senha = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
                agendamentoEntrega.SenhaAgendamento = repAgendamentoEntrega.ObterProximaSenhaAgendamento();
            }

            agendamentoEntrega.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(Request.GetIntParam("TipoDeCarga"));
            agendamentoEntrega.Transportador = Request.GetStringParam("Transportador");
            agendamentoEntrega.Observacao = Request.GetStringParam("Observacao");
            agendamentoEntrega.DataAgendamento = Request.GetDateTimeParam("DataAgendamento");
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAgendamentoEntrega ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAgendamentoEntrega filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAgendamentoEntrega()
            {
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                Placa = Request.GetStringParam("Placa"),
                Senha = Request.GetStringParam("Senha"),
                SenhaAgendamento = Request.GetIntParam("SenhaAgendamento"),
                Situacao = Request.GetNullableEnumParam<SituacaoAgendamentoEntrega>("Situacao")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                filtrosPesquisa.CpfCnpjRemetente = this.Usuario.Cliente.CPF_CNPJ;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número Agendamento", "SenhaAgendamento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Senha Agendamento", "Senha", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Agendamento", "DataAgendamento", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Placa", "Placa", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAgendamentoEntrega filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork);
                int totalRegistros = repositorioAgendamentoEntrega.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega> listaAgendamentoEntrega = (totalRegistros > 0) ? repositorioAgendamentoEntrega.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>();

                var retorno = listaAgendamentoEntrega.Select(AgendamentoEntrega => new
                {
                    AgendamentoEntrega.Codigo,
                    AgendamentoEntrega.Senha,
                    AgendamentoEntrega.SenhaAgendamento,
                    Situacao = AgendamentoEntrega.Situacao.ObterDescricao(),
                    DataAgendamento = AgendamentoEntrega.DataAgendamento.ToString("dd/MM/yyyy HH:mm:ss"),
                    Destinatario = AgendamentoEntrega.Destinatario.Descricao,
                    ModeloVeicularCarga = AgendamentoEntrega.ModeloVeicularCarga.Descricao,
                    TipoDeCarga = AgendamentoEntrega.TipoDeCarga.Descricao,
                    AgendamentoEntrega.Placa
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
