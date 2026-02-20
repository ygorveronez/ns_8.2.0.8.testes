using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Localization.Resources.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/LocalArmazenamentoProduto")]
    public class LocalArmazenamentoProdutoController : BaseController
    {
		#region Construtores

		public LocalArmazenamentoProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaLocalArmazenamentoProduto filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Empresa", "Empresa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Tipo de Óleo", "TipoOleo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Capacidade Total(L)", "CapacidadeTotalLitros", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade Sinalização(L)", "QuantidadeSinalizacaoLitros", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Régua", "Regua", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Densidade", "Densidade", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Controle de Abastecimento", "ControleAbastecimentoDisponivel", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoTipoOleo", false);

                if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 20, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto> localArmazenamentoProdutos = repLocalArmazenamentoProduto.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repLocalArmazenamentoProduto.ContarConsulta(filtrosPesquisa));

                var lista = (from p in localArmazenamentoProdutos
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoStatus,
                                 TipoOleo = p.TipoOleo != null ? p.TipoOleo.Descricao : string.Empty,
                                 CodigoTipoOleo = p.TipoOleo != null ? p.TipoOleo.Codigo : 0,
                                 Empresa = p.Empresa != null ? "(" + p.Empresa.LocalidadeUF + ") " + p.Empresa.RazaoSocial : string.Empty,
                                 CodigoEmpresa = p.Empresa != null ? p.Empresa.Codigo.ToString() : string.Empty,
                                 CapacidadeTotalLitros = p?.CapacidadeTotalLitros > 0 ? p?.CapacidadeTotalLitros?.ToString("n3") : string.Empty,
                                 QuantidadeSinalizacaoLitros = p?.QuantidadeSinalizacaoLitros > 0 ? p?.QuantidadeSinalizacaoLitros?.ToString("n3") : string.Empty,
                                 Regua = p?.QuantidadeSinalizacaoLitros > 0 ? p?.Regua?.ToString("n2") : string.Empty,
                                 Densidade = p?.Densidade > 0 ? p?.Densidade?.ToString("n2") : string.Empty,
                                 ControleAbastecimentoDisponivel = p.ControleAbastecimentoDisponivel == true ? "Sim" : "Não",
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProduto = new Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto();

                PreencherLocalArmazenamentoProduto(localArmazenamentoProduto, unitOfWork);

                repLocalArmazenamentoProduto.Inserir(localArmazenamentoProduto, Auditado);

                SalvarTransferencias(localArmazenamentoProduto, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProduto = repLocalArmazenamentoProduto.BuscarPorCodigo(codigo, true);

                if (localArmazenamentoProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherLocalArmazenamentoProduto(localArmazenamentoProduto, unitOfWork);

                SalvarTransferencias(localArmazenamentoProduto, unitOfWork);

                repLocalArmazenamentoProduto.Atualizar(localArmazenamentoProduto, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message ?? "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProduto = repLocalArmazenamentoProduto.BuscarPorCodigo(codigo, false);

                if (localArmazenamentoProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                var dynLocalArmazenamentoProduto = new
                {
                    localArmazenamentoProduto.Codigo,
                    localArmazenamentoProduto.Descricao,
                    localArmazenamentoProduto.Status,
                    Empresa = new { Codigo = localArmazenamentoProduto.Empresa?.Codigo ?? 0, Descricao = localArmazenamentoProduto.Empresa?.RazaoSocial ?? string.Empty },
                    TipoOleo = new { Codigo = localArmazenamentoProduto.TipoOleo?.Codigo ?? 0, Descricao = localArmazenamentoProduto.TipoOleo?.Descricao ?? string.Empty },
                    localArmazenamentoProduto.CodigoIntegracao,
                    CapacidadeTotalLitros = localArmazenamentoProduto?.CapacidadeTotalLitros?.ToString("n3"),
                    QuantidadeSinalizacaoLitros = localArmazenamentoProduto?.QuantidadeSinalizacaoLitros?.ToString("n3"),
                    Regua = localArmazenamentoProduto?.Regua?.ToString("n2"),
                    Densidade = localArmazenamentoProduto?.Densidade?.ToString("n2"),
                    localArmazenamentoProduto.ControleAbastecimentoDisponivel,
                    Transferencias = ObterLocalArmazenamentoProdutoTransferencia(localArmazenamentoProduto),
                    Saldo = localArmazenamentoProduto.SaldoDoTanque > 0 ? localArmazenamentoProduto.SaldoDoTanque?.ToString("n0") : "0",
                    Posto = localArmazenamentoProduto.Posto != null ? new { Codigo = localArmazenamentoProduto.Posto.CPF_CNPJ, Descricao = localArmazenamentoProduto.Posto.Nome } : null,

                };

                return new JsonpResult(dynLocalArmazenamentoProduto);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProduto = repLocalArmazenamentoProduto.BuscarPorCodigo(codigo, true);

                if (localArmazenamentoProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repLocalArmazenamentoProduto.Deletar(localArmazenamentoProduto, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private void SalvarTransferencias(Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProduto, Repositorio.UnitOfWork unitOfWork)
        {

            var jsonTransferencias = Request.Params("Transferencias");

            if (string.IsNullOrEmpty(jsonTransferencias))
                return;

            Repositorio.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia repLocalArmazenamentoProdutoTransferencia = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            var transferenciasNovas = JsonConvert.DeserializeObject<dynamic>(jsonTransferencias);
            var transferenciaExistentes = repLocalArmazenamentoProdutoTransferencia.BuscarPorLocalArmazenamentoProduto(localArmazenamentoProduto.Codigo);
            var transferenciaSalvar = new List<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia>();

            foreach (var nova in transferenciasNovas)
            {
                int codigo;
                if (!int.TryParse(nova.Codigo.ToString(), out codigo))
                {
                    codigo = 0;
                }

                decimal quantidadeTransferida = 0;
                if (nova.QuantidadeTransferida != null)
                    decimal.TryParse(nova.QuantidadeTransferida.ToString(), out quantidadeTransferida);

                // Agora, você pode usar 'codigo' para a comparação
                var transferenciaExistente = transferenciaExistentes.FirstOrDefault(l => l.Codigo == codigo);

                if (transferenciaExistente == null)
                {
                    int codigoLocalDestino;
                    if (!int.TryParse(nova.CodigoLocalArmazenamentoDestino.Descricao.ToString(), out codigoLocalDestino))
                    {
                        codigoLocalDestino = 0;
                    }

                    ValidarDisponibilidadeDoTanque(localArmazenamentoProduto.Codigo, codigoLocalDestino, quantidadeTransferida, unitOfWork);

                    // Cria uma nova licença
                    var novaTransferencia = new Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia
                    {
                        Descricao = nova.Descricao.ToString().Replace("\"", ""),
                        DataTransferencia = DateTime.Parse(nova.DataTransferencia.ToString().Replace("\"", "")),
                        DescricaoTransferencia = nova.DescricaoTransferencia.ToString().Replace("\"", ""),
                        LocalArmazenamentoProdutoDestino = codigoLocalDestino > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalDestino) : null,
                        Situacao = (SituacaoLocalArmazanamentoProdutoTransferencia)nova.CodigoSituacao,
                        LocalArmazenamentoProduto = localArmazenamentoProduto,
                        QuantidadeTransferida = quantidadeTransferida
                    };

                    repLocalArmazenamentoProdutoTransferencia.Inserir(novaTransferencia);
                    transferenciaSalvar.Add(novaTransferencia);
                }
                else
                {
                    // Atualiza os dados da licença existente
                    transferenciaExistente.Descricao = nova.Descricao.ToString().Replace("\"", "");
                    transferenciaExistente.DataTransferencia = DateTime.Parse(nova.DataTransferencia.ToString().Replace("\"", ""));
                    transferenciaExistente.DescricaoTransferencia = nova.DescricaoTransferencia.ToString().Replace("\"", "");
                    transferenciaExistente.Situacao = (SituacaoLocalArmazanamentoProdutoTransferencia)nova.CodigoSituacao;
                    transferenciaExistente.LocalArmazenamentoProduto = localArmazenamentoProduto;
                    int codigoLocalDestino;
                    if (!int.TryParse(nova.CodigoLocalArmazenamentoDestino.ToString(), out codigoLocalDestino))
                    {
                        codigoLocalDestino = 0;
                    }
                    transferenciaExistente.LocalArmazenamentoProdutoDestino = codigoLocalDestino > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalDestino) : null;
                    transferenciaExistente.QuantidadeTransferida = quantidadeTransferida > 0 ? quantidadeTransferida : 0;

                    ValidarDisponibilidadeDoTanque(localArmazenamentoProduto.Codigo, codigoLocalDestino, quantidadeTransferida, unitOfWork);

                    repLocalArmazenamentoProdutoTransferencia.Atualizar(transferenciaExistente);
                    transferenciaSalvar.Add(transferenciaExistente);
                }

            }

            // Remove as licenças que não estão presentes no JSON enviado
            foreach (var existente in transferenciaExistentes)
            {
                var codigoExistente = existente.Codigo;
                if (!transferenciaSalvar.Any(n => n.Codigo == codigoExistente))
                {
                    repLocalArmazenamentoProdutoTransferencia.Deletar(existente);
                }
            }
        }
        private void PreencherLocalArmazenamentoProduto(Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProduto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);

            int codigoEmpresa = Request.GetIntParam("Empresa");
            int codigoTipoOleo = Request.GetIntParam("TipoOleo");

            decimal? capacidadeTotalLitros = Request.GetDecimalParam("CapacidadeTotalLitros");
            decimal? quantidadeSinalizacaoLitros = Request.GetDecimalParam("QuantidadeSinalizacaoLitros");
            decimal? regua = Request.GetDecimalParam("Regua");
            decimal? densidade = Request.GetDecimalParam("Densidade");

            double codigoPosto = Request.GetDoubleParam("Posto");

            localArmazenamentoProduto.Descricao = Request.GetStringParam("Descricao");
            localArmazenamentoProduto.Status = Request.GetBoolParam("Status");
            localArmazenamentoProduto.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            localArmazenamentoProduto.ControleAbastecimentoDisponivel = Request.GetBoolParam("ControleAbastecimentoDisponivel");
            localArmazenamentoProduto.TipoOleo = codigoTipoOleo > 0 ? repTipoOleo.BuscarPorCodigo(codigoTipoOleo) : null;
            localArmazenamentoProduto.CapacidadeTotalLitros = capacidadeTotalLitros > 0 ? capacidadeTotalLitros : null;
            localArmazenamentoProduto.QuantidadeSinalizacaoLitros = quantidadeSinalizacaoLitros > 0 ? quantidadeSinalizacaoLitros : null;
            localArmazenamentoProduto.Regua = regua > 0 ? regua : null;
            localArmazenamentoProduto.Densidade = densidade > 0 ? densidade : null;
            localArmazenamentoProduto.Posto = codigoPosto > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPosto) : null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                localArmazenamentoProduto.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;

            if (localArmazenamentoProduto.Codigo == 0 && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                localArmazenamentoProduto.Empresa = Usuario.Empresa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaLocalArmazenamentoProduto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaLocalArmazenamentoProduto()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                TipoOleo = Request.GetIntParam("TipoOleo")
            };
        }


        private dynamic ObterLocalArmazenamentoProdutoTransferencia(Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProduto)
        {
            return (from obj in localArmazenamentoProduto.Transferencias
                    select new
                    {
                        Codigo = obj.Codigo,
                        DataTransferencia = obj.DataTransferencia.ToString("dd/MM/yyyy"),
                        CodigoLocalArmazenamentoDestino = obj.LocalArmazenamentoProdutoDestino?.Codigo,
                        DescricaoLocalArmazenamentoDestino = obj.LocalArmazenamentoProdutoDestino?.Descricao,
                        QuantidadeTransferida = obj.QuantidadeTransferida,

                        Descricao = obj.Descricao,
                        DescricaoSituacao = obj.Situacao.ObterDescricao(),
                        CodigoSituacao = obj.Situacao,
                        DescricaoTransferencia = obj.DescricaoTransferencia
                    }).ToList();

 
        }

        private bool ValidarDisponibilidadeDoTanque(int codigoLocalArmazenamentoOrigem, int codigoArmazenamentoDestino, decimal quantidadeTransferida, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
            Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProdutoOrigem = repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamentoOrigem);
            Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoProdutoDestino = repLocalArmazenamentoProduto.BuscarPorCodigo(codigoArmazenamentoDestino);

            if (localArmazenamentoProdutoOrigem.SaldoDoTanque == null)
                throw new Exception("Não foi realizada nenhuma movimentação de entrada no tanque ainda!");

            if (quantidadeTransferida + localArmazenamentoProdutoDestino.SaldoDoTanque > localArmazenamentoProdutoDestino.CapacidadeTotalLitros)
                throw new Exception("A quantidade adicionada para transferência somada com o saldo do tanque de destino é maior que a capacidade total!");

            if (quantidadeTransferida > localArmazenamentoProdutoOrigem.SaldoDoTanque)
                throw new Exception("A quantidade adicionada para transferência é maior que o saldo do tanque de origem!");

            return true;
        }
        #endregion
    }
}
