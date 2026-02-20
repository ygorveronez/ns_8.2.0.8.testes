using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Frota
{    
    [CustomAuthorize("Frota/IndicadoresManutencao", "IndicadoresManutencao/Pesquisa")]
    public class IndicadoresManutencaoController : BaseController
    {
		#region Construtores

		public IndicadoresManutencaoController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico filtrosPesquisa = this.ObterFiltrosPesquisaIndicadoresManutencao(unitOfWork);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);


                int quantidade = repOrdemServicoFrota.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> listaOrdemServico = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

                if (quantidade > 0)
                    listaOrdemServico = repOrdemServicoFrota.Consultar(filtrosPesquisa, "Codigo", "desc", filtrosPesquisa.Inicio, filtrosPesquisa.Limite);

                var retorno = new
                {
                    Quantidade = quantidade,
                    Registros = (from obj in listaOrdemServico
                                 select new
                                 {

                                     obj.Codigo,
                                     Numero = obj.Numero.ToString(),
                                     DataProgramada = obj.DataProgramada.ToString("dd/MM/yyyy"),
                                     Veiculo = obj.Veiculo?.Placa ?? string.Empty,
                                     Equipamento = obj.Equipamento?.Descricao ?? string.Empty,
                                     Motorista = obj.Motorista?.Nome ?? string.Empty,
                                     LocalManutencao = obj.LocalManutencao?.Nome ?? string.Empty,
                                     Operador = obj.Operador.Nome,
                                     TipoManutencao = new
                                     {
                                         Codigo = obj.TipoManutencao,
                                         Descricao = obj.DescricaoTipoManutencao ?? string.Empty
                                     },
                                     Situacao = obj.DescricaoSituacao,
                                     CodigoVeiculo = obj.Veiculo?.Codigo ?? 0,
                                     CodigoMotorista = obj.Motorista?.Codigo ?? 0,
                                     obj.Descricao,
                                     NumeroFrota = obj.Veiculo?.NumeroFrota ?? string.Empty,
                                     VeiculoEquipamento = (obj.Veiculo?.Placa ?? "") + " " + (obj.Equipamento?.Descricao ?? ""),
                                     CodigoEquipamento = obj.Equipamento?.Codigo ?? 0,
                                     obj.QuilometragemVeiculo,
                                     ValorOS = obj.Orcamento?.ValorTotalOrcado.ToString("n2") ?? 0.ToString("n2"),
                                     CodigoCentroResultado = obj.Veiculo?.CentroResultado?.Codigo ?? 0,
                                     CentroResultado = obj.Veiculo?.CentroResultado?.Descricao ?? string.Empty,
                                     Prioridade = obj.Prioridade.HasValue ? obj.Prioridade.Value : 0,
                                     DescricaoPrioridade = obj.Prioridade.HasValue ? obj.Prioridade.Value.ObterDescricao() : "-",
                                     SituacaoOrdemServicoFrota = obj.Situacao,
                                     TipoOficina = obj.TipoOficina,
                                     DT_RowClass =
                                            obj.Situacao == SituacaoOrdemServicoFrota.Finalizada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Sucess(IntensidadeCor._100) :
                                            obj.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Info(IntensidadeCor._100) :
                                            obj.TipoOficina == TipoOficina.Externa ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Danger(IntensidadeCor._100) :
                                            obj.TipoOficina == TipoOficina.Interna ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(IntensidadeCor._100) :
                                            string.Empty,
                                     TipoOrdemServico = obj.TipoOrdemServico?.Descricao ?? string.Empty,
                                     Cor = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork).BuscarPorOrdemServico(obj.Codigo)?.FirstOrDefault()?.Servico?.Cores.Descricao() + "50" ?? string.Empty,
                                     DescricaoOS = string.Concat("OS ", obj.TipoOficina?.ObterDescricao(), " ", obj.Situacao.ObterDescricao()),
                                     QuantidadeDiasAberto = obj.DataCriacao != null ? obj.DataFechamento != null ? (int)obj.DataFechamento.Value.Subtract(obj.DataCriacao.Value).TotalDays : (int)DateTime.Today.Subtract(obj.DataCriacao.Value).TotalDays : 0

                                 }).ToList()
                };


                
                return new JsonpResult(retorno);
                 
                //return new JsonpResult(null);
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

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico ObterFiltrosPesquisaIndicadoresManutencao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoEquipamento = Request.GetIntParam("Equipamento"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoServico = Request.GetIntParam("Servico"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0,
                CpfCnpjLocalManutencao = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor ? Usuario.Cliente?.CPF_CNPJ ?? 0d : Request.GetDoubleParam("LocalManutencao"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                TipoManutencao = Request.GetNullableEnumParam<TipoManutencaoOrdemServicoFrota>("TipoManutencao"),
                Situacao = Request.GetNullableListParam<SituacaoOrdemServicoFrota>("Situacao"),
                TipoOrdemServico = Request.GetNullableEnumParam<TipoOficina>("TipoOrdemServico"),
                CodigoGrupoServico = Request.GetIntParam("GrupoServico"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                Prioridade = Request.GetNullableEnumParam<PrioridadeOrdemServico>("Prioridade"),
                Inicio = Request.GetIntParam("Inicio"),
                Limite = Request.GetIntParam("Limite")
            };

            string numeroInicial = Request.GetStringParam("NumeroInicial");
            if (!string.IsNullOrWhiteSpace(numeroInicial) && filtrosPesquisa.NumeroInicial == 0)
            {
                filtrosPesquisa.Placa = numeroInicial;
                filtrosPesquisa.NumeroInicial = 0;
            }

            if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas?.Count > 0)
            {
                filtrosPesquisa.CodigosEmpresa = Request.GetListParam<int>("Empresa");
                if (filtrosPesquisa.CodigosEmpresa == null || filtrosPesquisa.CodigosEmpresa.Count == 0)
                    filtrosPesquisa.CodigosEmpresa = this.Usuario.Empresas.Select(c => c.Codigo).ToList();
            }

            return filtrosPesquisa;
        }
    }
}
