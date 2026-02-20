using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.GestaoCarga
{
    [CustomAuthorize("Cargas/CargaGestao")]
    public class CargaGestaoController : BaseController
    {
		#region Construtores

		public CargaGestaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> InformarMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoMotorista = Request.GetIntParam("Motorista");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Carga não encontrada");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    throw new ControllerException($"Não é possível alterar o motorista na atual situação da carga ({carga.DescricaoSituacaoCarga})");

                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarPorCodigo(codigoMotorista);

                if (motorista == null)
                    throw new ControllerException("Motorista não encontrado");

                Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);

                carga.Initialize();
                servicoCargaMotorista.AtualizarMotorista(carga, motorista);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Adicionou motorista na Gestão de Carga", unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                var dynCarga = BuscarDynCargas(cargaPedidos, unitOfWork);

                carga.VeiculoIntegradoEmbarcador = false;

                repositorioCarga.Atualizar(carga);
                unitOfWork.CommitChanges();

                return new JsonpResult(dynCarga);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Carga não encontrada");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    throw new ControllerException($"Não é possível alterar o veículo na atual situação da carga ({carga.DescricaoSituacaoCarga})");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    throw new ControllerException("Veiculo não encontrado");

                carga.Initialize();
                carga.Veiculo = veiculo;
                carga.VeiculosVinculados.Clear();

                foreach (Dominio.Entidades.Veiculo veiculoVinculado in veiculo.VeiculosVinculados)
                    carga.VeiculosVinculados.Add(veiculoVinculado);

                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                if (veiculoMotorista != null)
                {
                    Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);
                    servicoCargaMotorista.AtualizarMotorista(carga, veiculoMotorista);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), $"Adicionou veículo {(veiculoMotorista != null ? "e motorista " : "")}na Gestão de Carga", unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                var dynCarga = BuscarDynCargas(cargaPedidos, unitOfWork);

                carga.VeiculoIntegradoEmbarcador = false;

                repositorioCarga.Atualizar(carga);
                unitOfWork.CommitChanges();

                return new JsonpResult(dynCarga);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarSemanasAnos()
        {
            try
            {
                int anoCorrente = DateTime.Now.Year;
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                DateTime UltimoDiaAno = new DateTime(anoCorrente, 12, 31);
                Calendar calendario = dfi.Calendar;

                int semanaCorrente = calendario.GetWeekOfYear(DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                int ultimaSemanaAno = calendario.GetWeekOfYear(UltimoDiaAno, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

                var retorno = new
                {
                    ultimaSemanaAno = ultimaSemanaAno,
                    semanaCorrente = semanaCorrente,
                    anoCorrente = anoCorrente,
                    anoAnterior = anoCorrente - 1,
                    proximoAno = anoCorrente + 1
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisaCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int cidadePolo = int.Parse(Request.Params("CidadePolo"));

                int intPais = int.Parse(Request.Params("Pais"));

                int semana = int.Parse(Request.Params("Semana"));
                int ano = int.Parse(Request.Params("Ano"));
                double remetente = double.Parse(Request.Params("Remetente"));
                double destinatario = double.Parse(Request.Params("Destinatario"));
                int filial = int.Parse(Request.Params("Filial"));
                string CodigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");

                DateTime primeiroDiaSemana = RetornarPrimeiraDataSemana(ano, semana);
                DateTime ultimoDiaSemana = RetornarUltimoDataDaSemana(primeiroDiaSemana);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCarga.ConsultarPorSemana(CodigoCargaEmbarcador, filial, cidadePolo, intPais, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica, remetente, destinatario, primeiroDiaSemana, ultimoDiaSemana);


                List<Dominio.Entidades.Pais> paises = (from obj in listaCargaPedido select obj.Origem.Pais).Distinct().ToList();

                List<dynamic> dynRetorno = new List<dynamic>();

                foreach (Dominio.Entidades.Pais pais in paises)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoPaises = (from obj in listaCargaPedido where obj.Origem.Pais == pais select obj).ToList();

                    var dynCargaPais = new
                    {
                        Pais = pais != null ? pais.Nome : "",
                        Semana = semana,
                        Abreviacao = pais != null ? !string.IsNullOrWhiteSpace(pais.Abreviacao) ? pais.Abreviacao.ToLower() : "" : "",
                        DataInicioSemana = primeiroDiaSemana.ToString("dd/MM/yyyy"),
                        DataFimSemana = ultimoDiaSemana.ToString("dd/MM/yyyy"),
                        CodigoCargaEmbarcador = CodigoCargaEmbarcador,
                        Cargas = BuscarDynCargas(cargaPedidoPaises, unitOfWork)
                    };
                    dynRetorno.Add(dynCargaPais);
                }
                return new JsonpResult(dynRetorno);
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

        private List<dynamic> BuscarDynCargas(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            List<dynamic> dynCargas = new List<dynamic>();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = (from obj in cargaPedidos select obj.Carga).Distinct().ToList();
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
            var fronteiras = serCargaFronteira.ObterFronteirasPorCargas(cargas);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga = (from obj in cargaPedidos where obj.Carga.Codigo == carga.Codigo select obj).ToList();

                var fronteirasDaCarga = serCargaFronteira.ObterFronteirasPorCarga(carga, fronteiras);

                var dynCarga = new
                {
                    Situacao = carga.SituacaoCarga,
                    OrigemDestinos = serCargaDadosSumarizados.ObterOrigemDestinos(carga, false, TipoServicoMultisoftware),
                    Codigo = carga.Codigo,
                    CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                    DescricaoSituacaoCarga = carga.DescricaoSituacaoCarga,
                    ValorFrete = carga.ValorFreteAPagar.ToString("n2"),
                    DataCarregamento = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyy HH:mm") : "Não informada",
                    PrevisaoEntrega = retonarMaiorDataEntrega(cargaPedidosCarga),
                    Motoristas = (from obj in cargaMotoristas
                                  select new
                                  {
                                      obj.Motorista.Codigo,
                                      Descricao = obj.Motorista.Nome
                                  }).ToList(),
                    Veiculo = carga.Veiculo != null ? new
                    {
                        carga.Veiculo.Codigo,
                        Descricao = carga.Veiculo.Placa
                    } : new { Codigo = 0, Descricao = "" },
                    VeiculosVinculados = (from obj in carga.VeiculosVinculados
                                          select new
                                          {
                                              Codigo = obj.Codigo,
                                              Descricao = obj.Placa
                                          }).ToList(),
                    Filial = new
                    {
                        carga.Filial.Codigo,
                        carga.Filial.Descricao
                    },
                    Remetentes = (from obj in cargaPedidosCarga
                                  select new
                                  {
                                      Codigo = obj.Pedido.Remetente.CPF_CNPJ,
                                      Descricao = obj.Pedido.Remetente.Nome
                                  }).Distinct().ToList(),
                    Destinatarios = (from obj in cargaPedidosCarga
                                     select new
                                     {
                                         Codigo = obj.Pedido.Destinatario.CPF_CNPJ,
                                         Descricao = obj.Pedido.Destinatario.Nome
                                     }).Distinct().ToList(),
                    Fronteiras = (from o in fronteirasDaCarga
                                  select new
                                  {
                                      o.Fronteira.Codigo,
                                      o.Fronteira.Descricao,
                                  }).ToList(),
                };

                dynCargas.Add(dynCarga);

            }

            return dynCargas;
        }

        private string retonarMaiorDataEntrega(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            DateTime? data = (from obj in cargaPedidos where obj.Pedido.PrevisaoEntrega.HasValue select obj.Pedido.PrevisaoEntrega).Max();
            if (data.HasValue)
                return data.Value.ToString("dd/MM/yyyy HH:mm");
            else
                return "Não informada";
        }

        private DateTime RetornarPrimeiraDataSemana(int ano, int semana)
        {
            DateTime dt = new DateTime(ano, 1, 1);
            int weekNumber = semana;
            int days = (weekNumber - 1) * 7;
            DateTime dt1 = dt.AddDays(days);
            DayOfWeek dow = dt1.DayOfWeek;
            DateTime startDateOfWeek = dt1.AddDays(-(int)dow);
            if (semana == 1)
            {
                while (startDateOfWeek.Year != ano)
                    startDateOfWeek = startDateOfWeek.AddDays(1);
            }
            return startDateOfWeek;
        }

        private DateTime RetornarUltimoDataDaSemana(DateTime primeiraData)
        {
            DateTime ultimaDataSemana = primeiraData;

            while (ultimaDataSemana.DayOfWeek != DayOfWeek.Saturday)
                ultimaDataSemana = ultimaDataSemana.AddDays(1);

            while (primeiraData.Year != ultimaDataSemana.Year)
                ultimaDataSemana = ultimaDataSemana.AddDays(-1);

            ultimaDataSemana = ultimaDataSemana.AddDays(1).AddMilliseconds(-1);

            return ultimaDataSemana;
        }

        #endregion
    }
}

