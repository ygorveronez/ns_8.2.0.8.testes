using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/RegraAutorizacaoGestaoDocumentos")]
    public class RegraAutorizacaoGestaoDocumentosController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento>
    {
		#region Construtores

		public RegraAutorizacaoGestaoDocumentosController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento regra)
        {
            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorTransportador = regra.RegraPorTransportador,
                UsarRegraPorTomador = regra.RegraPorTomador,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                UsarRegraPorValorPagamento = regra.RegraPorValorPagamento,
                UsarRegraPorMotivoRejeicao = regra.RegraPorMotivoRejeicao,
                UsarRegraPorCanalEntrega = regra.RegraPorCanalEntrega,
                UsarRegraPorPeso = regra.RegraPorPeso,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                AlcadasValorPagamento = (from alcada in regra.AlcadasValorPagamento select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaValorPagamento, decimal>(alcada)).ToList(),
                AlcadasTomador = (from alcada in regra.AlcadasTomador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTomador, Dominio.Entidades.Cliente>(alcada)).ToList(),
                AlcadasMotivoRejeicao = (from alcada in regra.AlcadasMotivoRejeicao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaMotivoRejeicao, MotivoInconsistenciaGestaoDocumento>(alcada)).ToList(),
                AlcadasCanalEntrega = (from alcada in regra.AlcadasCanalEntrega select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaCanalEntrega, Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>(alcada)).ToList(),
                AlcadasPeso = (from alcada in regra.AlcadasPeso select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaPeso, decimal>(alcada)).ToList()
            });
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento>(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento regraAutorizacao = new Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorTomador = Request.GetBoolParam("UsarRegraPorTomador");
                    regra.RegraPorValorPagamento = Request.GetBoolParam("UsarRegraPorValorPagamento");
                    regra.RegraPorMotivoRejeicao = Request.GetBoolParam("UsarRegraPorMotivoRejeicao");
                    regra.RegraPorCanalEntrega = Request.GetBoolParam("UsarRegraPorCanalEntrega");
                    regra.RegraPorPeso = Request.GetBoolParam("UsarRegraPorPeso");
                }));

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTomador, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacao, "AlcadasTomador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = tomador ?? throw new ControllerException("Tomador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacao, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaValorPagamento, decimal>(unitOfWork, regraAutorizacao, "AlcadasValorPagamento", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("Valor do pagamento deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaMotivoRejeicao, MotivoInconsistenciaGestaoDocumento>(unitOfWork, regraAutorizacao, "AlcadasMotivoRejeicao", ((valorPropriedade, alcada) =>
                {
                    string valor = Convert.ToString(valorPropriedade);
                    MotivoInconsistenciaGestaoDocumento? motivoRejeicao = valor.ToNullableEnum<MotivoInconsistenciaGestaoDocumento>();

                    alcada.PropriedadeAlcada = motivoRejeicao ?? throw new ControllerException("Motivo da rejeição não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaCanalEntrega, Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>(unitOfWork, regraAutorizacao, "AlcadasCanalEntrega", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repositorioCanalEntrega.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = canalEntrega ?? throw new ControllerException("Canal de entrega não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaPeso, decimal>(unitOfWork, regraAutorizacao, "AlcadasPeso", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("Peso deve ser maior do que zero.");
                }));

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public override IActionResult Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento>(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorTomador = Request.GetBoolParam("UsarRegraPorTomador");
                    regra.RegraPorValorPagamento = Request.GetBoolParam("UsarRegraPorValorPagamento");
                    regra.RegraPorMotivoRejeicao = Request.GetBoolParam("UsarRegraPorMotivoRejeicao");
                    regra.RegraPorCanalEntrega = Request.GetBoolParam("UsarRegraPorCanalEntrega");
                    regra.RegraPorPeso = Request.GetBoolParam("UsarRegraPorPeso");
                }));

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTomador, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTomador, "AlcadasTomador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = tomador ?? throw new ControllerException("Tomador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaValorPagamento, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasValorPagamento, "AlcadasValorPagamento", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("Valor do pagamento deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaMotivoRejeicao, MotivoInconsistenciaGestaoDocumento>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasMotivoRejeicao, "AlcadasMotivoRejeicao", ((valorPropriedade, alcada) =>
                {
                    string valor = Convert.ToString(valorPropriedade);
                    MotivoInconsistenciaGestaoDocumento? motivoRejeicao = valor.ToNullableEnum<MotivoInconsistenciaGestaoDocumento>();

                    alcada.PropriedadeAlcada = motivoRejeicao ?? throw new ControllerException("Motivo da rejeição não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaCanalEntrega, Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasCanalEntrega, "AlcadasCanalEntrega", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repositorioCanalEntrega.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = canalEntrega ?? throw new ControllerException("Canal de entrega não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaPeso, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPeso, "AlcadasPeso", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("Peso deve ser maior do que zero.");
                }));

                repositorioRegra.Atualizar(regraAutorizacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos Sobrescritos
    }
}