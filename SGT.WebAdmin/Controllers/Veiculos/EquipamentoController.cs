using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/Equipamento")]
    public class EquipamentoController : BaseController
    {
        #region Construtores

        public EquipamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaEquipamento filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 7, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Equipamento.Numero, "Numero", 20, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Equipamento.Marca, "DescricaoMarca", 20, Models.Grid.Align.left, true, true, true);
                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false, true, true);
                grid.AdicionarCabecalho("Horimetro", false);
                grid.AdicionarCabecalho("CodigoCentroResultado", false);
                grid.AdicionarCabecalho("CentroResultado", false);
                grid.AdicionarCabecalho("DescricaoComMarcaModelo", false);

                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> listaEquipamento = repEquipamento.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repEquipamento.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaEquipamento
                             select new
                             {
                                 p.Codigo,
                                 Descricao = (p.Descricao + "   -    " + p.Numero),
                                 p.Numero,
                                 DescricaoMarca = p.MarcaEquipamento?.Descricao ?? string.Empty,
                                 p.DescricaoAtivo,
                                 p.Horimetro,
                                 CodigoCentroResultado = p.CentroResultado?.Codigo ?? 0,
                                 CentroResultado = p.CentroResultado?.Descricao ?? string.Empty,
                                 p.DescricaoComMarcaModelo
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = new Dominio.Entidades.Embarcador.Veiculos.Equipamento();

                PreencherEquipamento(equipamento, unitOfWork);

                repEquipamento.Inserir(equipamento, Auditado);

                int codigoGrupoServico = Request.GetIntParam("GrupoServico");
                if (ConfiguracaoEmbarcador.GerarOSAutomaticamenteCadastroVeiculoEquipamento && codigoGrupoServico > 0)
                {
                    Repositorio.Embarcador.Frota.GrupoServico repGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico()
                    {
                        CadastrandoVeiculoEquipamento = true,
                        Observacao = "GERADO AUTOMATICAMENTE AO CADASTRAR O EQUIPAMENTO",
                        Operador = Usuario,
                        Equipamento = equipamento,
                        Horimetro = equipamento.Horimetro,
                        GrupoServico = repGrupoServico.BuscarPorCodigo(codigoGrupoServico),
                        Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : null
                    };

                    Servicos.Embarcador.Frota.OrdemServico.GerarFinalizarOrdemServicoCompleta(objetoOrdemServico, Usuario, Auditado, unitOfWork, TipoServicoMultisoftware);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigo, true);

                if (equipamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                int horimetroViradaAnterior = equipamento.HorimetroVirada;

                PreencherEquipamento(equipamento, unitOfWork);

                if (equipamento.HorimetroVirada != horimetroViradaAnterior)
                {
                    int ultimoHorimetro = repAbastecimento.BuscarUltimoHorimetroAbastecimento(equipamento.Codigo, DateTime.Now, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                    if (ultimoHorimetro > equipamento.HorimetroVirada)
                        return new JsonpResult(false, true, "Favor verifique o Horímetro da Virada informado, pois existe um abastecimento com um horímetro maior: " + ultimoHorimetro.ToString("n0") + ".");
                }

                repEquipamento.Atualizar(equipamento, Auditado);
                
                SalvarHistoricoHorimetros(equipamento, unitOfWork);

                equipamento.HorimetroAtual = equipamento.HorimetroAtualHistoricoHorimetro;
                
                repEquipamento.Atualizar(equipamento, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigo);

                if (equipamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynEquipamento = new
                {
                    equipamento.Ativo,
                    equipamento.Codigo,
                    equipamento.Descricao,
                    equipamento.Numero,
                    MarcaEquipamento = equipamento.MarcaEquipamento != null ? new { Codigo = equipamento.MarcaEquipamento.Codigo, Descricao = equipamento.MarcaEquipamento.Descricao } : null,
                    ModeloEquipamento = equipamento.ModeloEquipamento != null ? new { Codigo = equipamento.ModeloEquipamento.Codigo, Descricao = equipamento.ModeloEquipamento.Descricao } : null,
                    SegmentoVeiculo = equipamento.SegmentoVeiculo != null ? new { Codigo = equipamento.SegmentoVeiculo.Codigo, Descricao = equipamento.SegmentoVeiculo.Descricao } : null,
                    equipamento.Chassi,
                    equipamento.Hodometro,
                    equipamento.Horimetro,
                    DataAquisicao = equipamento.DataAquisicao.HasValue ? equipamento.DataAquisicao.Value.ToString("dd/MM/yyyy") : "",
                    equipamento.AnoFabricacao,
                    equipamento.AnoModelo,
                    equipamento.Observacao,
                    equipamento.EquipamentoAceitaAbastecimento,
                    equipamento.ViradaHodometro,
                    KilometragemVirada = equipamento.HorimetroVirada > 0 ? equipamento.HorimetroVirada.ToString("n0") : string.Empty,
                    equipamento.UtilizaTanqueCompartilhado,
                    MediaPadrao = equipamento.MediaPadrao > 0 ? equipamento.MediaPadrao.ToString("n2") : string.Empty,
                    CapacidadeTanque = equipamento.CapacidadeTanque > 0 ? equipamento.CapacidadeTanque.ToString("n2") : string.Empty,
                    CentroResultado = equipamento.CentroResultado != null ? new { equipamento.CentroResultado.Codigo, equipamento.CentroResultado.Descricao } : null,
                    CapacidadeMaximaTanque = equipamento.CapacidadeMaximaTanque > 0 ? equipamento.CapacidadeMaximaTanque.ToString("n2") : string.Empty,
                    Neokohm = equipamento.Neokohm == SimNao.Sim ? 1 : 2,
                    Cor = equipamento?.Cor ?? string.Empty,
                    Renavam = equipamento?.Renavam ?? string.Empty,
                    equipamento.Integrado,
                    HorimetroAtual = equipamento.TrocaHorimetro ? equipamento.HorimetroAtual == 0 ? equipamento.HorimetroAtualHistoricoHorimetro : equipamento.HorimetroAtual : 0,                    
                    equipamento.TrocaHorimetro,                    
                    HistoricoHorimetros = ObterEquipamentoHitoricoHorimetro(equipamento),
                    
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigo, true);

                if (equipamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Veiculos.HistoricoHorimetro repHistoricoHorimetro = new Repositorio.Embarcador.Veiculos.HistoricoHorimetro(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro> historicoHorimetro = repHistoricoHorimetro.BuscarPorEquipamento(codigo);

                foreach (var item in historicoHorimetro)
                {
                    repHistoricoHorimetro.Deletar(item);
                }

                repEquipamento.Deletar(equipamento, Auditado);

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

        private void PreencherEquipamento(Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            equipamento.Ativo = bool.Parse(Request.Params("Ativo"));
            equipamento.Descricao = Request.Params("Descricao");
            equipamento.Numero = Request.Params("Numero");

            if (!string.IsNullOrWhiteSpace(Request.Params("MarcaEquipamento")) && int.Parse(Request.Params("MarcaEquipamento")) > 0)
                equipamento.MarcaEquipamento = new Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento() { Codigo = int.Parse(Request.Params("MarcaEquipamento")) };
            else
                equipamento.MarcaEquipamento = null;

            equipamento.ModeloEquipamento = Request.GetIntParam("ModeloEquipamento") > 0 ? new Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento() { Codigo = Request.GetIntParam("ModeloEquipamento") } : null;
            equipamento.SegmentoVeiculo = Request.GetIntParam("SegmentoVeiculo") > 0 ? new Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo() { Codigo = Request.GetIntParam("SegmentoVeiculo") } : null;
            equipamento.Hodometro = Request.GetIntParam("Hodometro");
            equipamento.Horimetro = Request.GetIntParam("Horimetro");
            equipamento.AnoFabricacao = Request.GetIntParam("AnoFabricacao");
            equipamento.AnoModelo = Request.GetIntParam("AnoModelo");
            equipamento.EquipamentoAceitaAbastecimento = Request.GetBoolParam("EquipamentoAceitaAbastecimento");

            equipamento.DataAquisicao = Request.GetNullableDateTimeParam("DataAquisicao");

            equipamento.Chassi = Request.GetStringParam("Chassi");
            equipamento.Observacao = Request.GetStringParam("Observacao");
            equipamento.ViradaHodometro = Request.GetBoolParam("ViradaHodometro");
            equipamento.HorimetroVirada = Request.GetIntParam("KilometragemVirada");
            equipamento.UtilizaTanqueCompartilhado = Request.GetBoolParam("UtilizaTanqueCompartilhado");
            equipamento.MediaPadrao = Request.GetDecimalParam("MediaPadrao");
            equipamento.CapacidadeTanque = Request.GetDecimalParam("CapacidadeTanque");
            equipamento.CapacidadeMaximaTanque = Request.GetDecimalParam("CapacidadeMaximaTanque");
            equipamento.TrocaHorimetro = Request.GetBoolParam("TrocaHorimetro");
            if (equipamento.TrocaHorimetro)
                equipamento.HorimetroAtual = Request.GetIntParam("HorimetroAtual");

            int codigoCentroResultado = Request.GetIntParam("CentroResultado");
            equipamento.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;

            int neokohm = Request.GetIntParam("Neokohm");
            if (neokohm == 1)
                equipamento.Neokohm = SimNao.Sim;
            else
                equipamento.Neokohm = SimNao.Nao;

            equipamento.Cor = Request.GetStringParam("Cor");
            equipamento.Renavam = Request.GetStringParam("Renavam");

            bool integrado = Request.GetBoolParam("Integrado");
            equipamento.Integrado = false;
        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaEquipamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaEquipamento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Numero = Request.GetStringParam("Numero"),
                Chassi = Request.GetStringParam("Chassi"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoMarcaEquipamento = Request.GetIntParam("MarcaEquipamento"),
                Codigo = Request.GetIntParam("Codigo")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoMarca")
                return "MarcaEquipamento.Descricao";

            return propriedadeOrdenar;
        }

        private dynamic ObterEquipamentoHitoricoHorimetro(Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento)
        {
            return (from obj in equipamento.HistoricoHorimetros
                    select new
                    {
                        Codigo = obj.Codigo,                        
                        DataAlteracao = obj.DataAlteracao?.ToString("dd/MM/yyyy"),                        
                        HorimetroAtual = obj.HorimetroAtual,
                        Observacao = obj.Observacao
                    }).ToList();


        }

        private void SalvarHistoricoHorimetros(Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento, Repositorio.UnitOfWork unitOfWork)        
        {
            var jsonHorimetros = Request.Params("HistoricoHorimetros");

            if (string.IsNullOrEmpty(jsonHorimetros))
                return;

            Repositorio.Embarcador.Veiculos.HistoricoHorimetro repositorioHistoricoHorimetro = new Repositorio.Embarcador.Veiculos.HistoricoHorimetro(unitOfWork);

            foreach (var horimetro in JsonConvert.DeserializeObject<dynamic>(jsonHorimetros))
            {
                int codigo;
                if (!int.TryParse(horimetro.Codigo.ToString(), out codigo))
                {
                    codigo = 0;
                }                                

                var horimetroExistente = repositorioHistoricoHorimetro.BuscarPorCodigo(codigo);

                Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro historicoHorimetro = null;
                if (horimetroExistente == null)
                {
                    historicoHorimetro = new Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro()
                    {                        
                        DataAlteracao = DateTime.Parse(horimetro.DataAlteracao.ToString().Replace("\"", "")),
                        HorimetroAtual = int.Parse(Utilidades.String.OnlyNumbers(horimetro.HorimetroAtual.ToString())),
                        Equipamento = equipamento,
                        Observacao = horimetro.Observacao.ToString().Replace("\"", "")
                    };
                    repositorioHistoricoHorimetro.Inserir(historicoHorimetro);
                }
                else
                {
                    horimetroExistente.DataAlteracao = DateTime.Parse(horimetro.DataAlteracao.ToString().Replace("\"", ""));
                    horimetroExistente.HorimetroAtual = int.Parse(Utilidades.String.OnlyNumbers(horimetro.HorimetroAtual.ToString()));
                    horimetroExistente.Observacao = horimetro.Observacao.ToString().Replace("\"", "");
                    repositorioHistoricoHorimetro.Atualizar(horimetroExistente);
                }                    
            }
        }

        #endregion
    }
}
