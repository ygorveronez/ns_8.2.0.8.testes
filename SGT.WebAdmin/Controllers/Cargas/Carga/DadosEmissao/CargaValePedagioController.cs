using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio")]
    public class CargaValePedagioController : BaseController
    {
		#region Construtores

		public CargaValePedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Fornecedor, "Fornecedor", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Responsavel, "Responsavel", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Comprovante, "NumeroComprovante", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Valor, "Valor", 18, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> listaCargaValePedagios = repCargaValePedagio.Consultar(codigoCarga, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaValePedagio.ContarConsulta(codigoCarga));

                var lista = (from p in listaCargaValePedagios
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroComprovante,
                                 Fornecedor = p.Fornecedor != null ? p.Fornecedor.Nome + " (" + p.Fornecedor.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Responsavel = p.Responsavel != null ? p.Responsavel.Nome + " (" + p.Responsavel.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Valor = p.Valor.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarValePedagio)) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                string numeroComprovante = Request.Params("NumeroComprovante");
                string codigoAgendamentoPorto = Request.Params("CodigoAgendamentoPorto");

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Fornecedor")), out double cpfCnpjFornecedor);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Responsavel")), out double cpfCnpjResponsavel);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);

                Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.CargaValePedagio();

                cargaValePedagio.Carga = carga;
                cargaValePedagio.Fornecedor = cpfCnpjFornecedor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor) : null;
                cargaValePedagio.Responsavel = cpfCnpjResponsavel > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjResponsavel) : null;
                cargaValePedagio.CodigoAgendamentoPorto = codigoAgendamentoPorto;
                cargaValePedagio.NumeroComprovante = numeroComprovante;
                cargaValePedagio.Valor = valor;

                repCargaValePedagio.Inserir(cargaValePedagio, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaValePedagio.Carga, null, "Adicionou um Vale Pedágio", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorCodigo(codigo);

                if (cargaValePedagio.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + cargaValePedagio.Carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaValePedagio.Carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão na atual situação da carga (" + cargaValePedagio.Carga.DescricaoSituacaoCarga + ")");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(cargaValePedagio.Carga, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaValePedagio.Carga, null, "Removeu um Vale Pedágio", unidadeTrabalho);
                repCargaValePedagio.Deletar(cargaValePedagio, Auditado);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        
        public async Task<IActionResult> CarregarValoresValePedagio()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = 0;
                int.TryParse(Request.Params("Codigo"), out codigoCarga);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga();

                carga = repCarga.BuscarPorCodigo(codigoCarga);

                Repositorio.Embarcador.Logistica.PracaPedagioTarifa repPracaPedagioTarifa = new Repositorio.Embarcador.Logistica.PracaPedagioTarifa(unidadeTrabalho);
                List<Dominio.ObjetosDeValor.Embarcador.Frete.TarifaModeloVeicular> retorno = repPracaPedagioTarifa.BuscarSumarizadasPorRotaFrete(carga.Rota?.Codigo ?? 0);

                decimal total = (retorno?.Where(x => x.ModeloVeicularCarga.Codigo == (carga?.Veiculo.ModeloVeicularCarga.Codigo ?? 0)).Sum(x => x.Tarifa)) ?? 0;
                int totaleixos = carga?.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                foreach (var veiculoVinculado in carga.VeiculosVinculados)
                {
                    total += (retorno?.Where(x => x.ModeloVeicularCarga.Codigo == (veiculoVinculado?.ModeloVeicularCarga.Codigo ?? 0)).Sum(x => x.Tarifa)) ?? 0;
                    totaleixos += veiculoVinculado.ModeloVeicularCarga?.NumeroEixos ?? 0;
                }

                decimal valorporeixo = (totaleixos > 0 ? (total > 0 ? (total / totaleixos) : 0) : 0);

                var lista = new
                {
                    EixosPorCargaValePedagio = totaleixos,
                    ValorPorEixoValePedagio = valorporeixo.ToString("n2"),
                    TotalValePedagio = total.ToString("n2"),
                };

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

    }
}
