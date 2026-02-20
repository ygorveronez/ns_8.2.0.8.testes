using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize("Acertos/AcertoViagem")]
    public class AcertoCargaController : BaseController
    {
		#region Construtores

		public AcertoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly decimal _tamanhoColunaPequena = 1.75m;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AtualizarCargas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de carga antes.");

                acertoViagem.Etapa = etapa;
                acertoViagem.Situacao = situacao;
                acertoViagem.DataAlteracao = DateTime.Now;
                acertoViagem.CargaSalvo = true;

                repAcertoViagem.Atualizar(acertoViagem, Auditado);

                servAcertoViagem.AtualizarCargasAcerto(acertoViagem, unitOfWork, Request.Params("ListaCargas"), Auditado);
                servAcertoViagem.AtualizarVeiculoAcerto(acertoViagem, unitOfWork, Auditado);
                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Cargas, this.Usuario);

                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarCargas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarDetalhesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller SalvarDetalhesCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigoCarga;
                int.TryParse(Request.Params("Codigo"), out codigoCarga);
                decimal valorFrete = 0, valorICMS = 0, valorBonificacao = 0;
                decimal.TryParse(Request.Params("ValorBrutoCarga"), out valorFrete);
                decimal.TryParse(Request.Params("ValorICMSCarga"), out valorICMS);
                decimal.TryParse(Request.Params("ValorBonificacaoCliente"), out valorBonificacao);
                bool cargaFracionada = false;
                bool.TryParse(Request.Params("CargaFracionada"), out cargaFracionada);

                Dominio.Entidades.Embarcador.Acerto.AcertoCarga acertoCarga = repAcertoCarga.BuscarPorCodigoCarga(codigoCarga);

                if (acertoCarga != null)
                {
                    acertoCarga.Initialize();
                    acertoCarga.CargaFracionada = cargaFracionada;
                    acertoCarga.ValorICMSCarga = valorICMS;
                    acertoCarga.ValorBrutoCarga = valorFrete;
                    acertoCarga.ValorBonificacaoCliente = valorBonificacao;

                    if (cargaFracionada)
                        servAcertoViagem.InserirLogAcerto(acertoCarga.AcertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.AlteradoCargaFracionada, this.Usuario);
                    if (valorBonificacao > 0)
                        servAcertoViagem.InserirLogAcerto(acertoCarga.AcertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.AlteradoBonificacaoCliente, this.Usuario);

                    repAcertoCarga.Atualizar(acertoCarga, Auditado);


                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoCarga.AcertoViagem, null, "Atualizou a carga " + acertoCarga.Descricao + ".", unitOfWork);

                    var dynRetorno = new
                    {
                        Codigo = codigoCarga
                    };

                    unitOfWork.CommitChanges();
                    return new JsonpResult(dynRetorno, true, "Sucesso");
                }
                else
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(false, "Favor salve as novas cargas antes de editar os seus detalhes.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os detalhes da carga.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller SalvarDetalhesCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaBonificacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisaBonificacoes " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codCarga, codAcerto;
                int.TryParse(Request.Params("Codigo"), out codCarga);
                int.TryParse(Request.Params("CodigoAcerto"), out codAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoBonificaca", false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoBonificaca", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoTipoBonificaca")
                    propOrdenar = "TipoBonificaca";

                Repositorio.Embarcador.Acerto.AcertoCargaBonificacao repAcertoCargaBonificacao = new Repositorio.Embarcador.Acerto.AcertoCargaBonificacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao> listaAcertoCargaBonificacao = repAcertoCargaBonificacao.ConsultarAcertoCargaBonificacao(codCarga, codAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoCargaBonificacao.ContarConsultarAcertoCargaBonificacao(codCarga, codAcerto));

                var dynRetorno = (from obj in listaAcertoCargaBonificacao
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      obj.TipoBonificaca,
                                      obj.DescricaoTipoBonificaca,
                                      Justificativa = obj.Justificativa != null ? obj.Justificativa.Descricao : string.Empty,
                                      Valor = obj.Valor.ToString("n2")
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as bonificações da carga no acerto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisaBonificacoes " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverBonificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverBonificacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoCargaBonificacao repAcertoCargaBonificacao = new Repositorio.Embarcador.Acerto.AcertoCargaBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao valor = repAcertoCargaBonificacao.BuscarPorCodigo(codigo);
                unitOfWork.Start();
                repAcertoCargaBonificacao.Deletar(valor, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga, null, "Removida bonificação " + valor.Descricao + " da Carga.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga.AcertoViagem, null, "Removida bonificação " + valor.Descricao + " da Carga " + valor.AcertoCarga.Descricao + ".", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o valor da bonificação.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverBonificacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarBonificacaoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AdicionarBonificacaoCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoCarga, codigoAcerto, codigoJustificativaBonificacao;
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);
                int.TryParse(Request.Params("JustificativaBonificacao"), out codigoJustificativaBonificacao);

                decimal valorBonificacaoDesconto;
                decimal.TryParse(Request.Params("ValorBonificacaoDesconto"), out valorBonificacaoDesconto);

                Repositorio.Embarcador.Acerto.AcertoCargaBonificacao repAcertoCargaBonificacao = new Repositorio.Embarcador.Acerto.AcertoCargaBonificacao(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao valor = new Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao();
                valor.AcertoCarga = repAcertoCarga.BuscarPorCodigoAcertoCodigoCarga(codigoAcerto, codigoCarga);
                valor.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativaBonificacao); ;
                valor.TipoBonificaca = valor.Justificativa.TipoJustificativa;
                valor.Valor = valorBonificacaoDesconto;

                unitOfWork.Start();
                repAcertoCargaBonificacao.Inserir(valor, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga, null, "Adicionada bonificação " + valor.Descricao + " na Carga.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga.AcertoViagem, null, "Adicionada bonificação " + valor.Descricao + " na Carga " + valor.AcertoCarga.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    TipoBonificaca = valor.TipoBonificaca
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo valor da bonificação.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AdicionarBonificacaoCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPedagios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisaPedagios " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codCarga, codAcerto;
                int.TryParse(Request.Params("Codigo"), out codCarga);
                int.TryParse(Request.Params("CodigoAcerto"), out codAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoCargaPedagio repAcertoCargaPedagio = new Repositorio.Embarcador.Acerto.AcertoCargaPedagio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio> listaAcertoCargaPedagio = repAcertoCargaPedagio.ConsultarAcertoCargaPedagio(codCarga, codAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoCargaPedagio.ContarConsultarAcertoCargaPedagio(codCarga, codAcerto));

                var dynRetorno = (from obj in listaAcertoCargaPedagio
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      Justificativa = obj.Justificativa != null ? obj.Justificativa.Descricao : string.Empty,
                                      Valor = obj.Valor.ToString("n2")
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as bonificações da carga no acerto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisaPedagios " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverPedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverPedagio " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoCargaPedagio repAcertoCargaPedagio = new Repositorio.Embarcador.Acerto.AcertoCargaPedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio valor = repAcertoCargaPedagio.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga, null, "Removido pedágio " + valor.Descricao + " da Carga.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga.AcertoViagem, null, "Removido pedágio " + valor.Descricao + " na Carga " + valor.AcertoCarga.Descricao + ".", unitOfWork);
                repAcertoCargaPedagio.Deletar(valor, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o valor do pedágio.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverPedagio " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarPedagioCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AdicionarPedagioCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoCarga, codigoAcerto, codigoJustificativaPedagio;
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);
                int.TryParse(Request.Params("JustificativaPedagio"), out codigoJustificativaPedagio);

                decimal valorPedagio;
                decimal.TryParse(Request.Params("ValorPedagio"), out valorPedagio);

                Repositorio.Embarcador.Acerto.AcertoCargaPedagio repAcertoCargaPedagio = new Repositorio.Embarcador.Acerto.AcertoCargaPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio valor = new Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio();
                valor.AcertoCarga = repAcertoCarga.BuscarPorCodigoAcertoCodigoCarga(codigoAcerto, codigoCarga);
                valor.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativaPedagio);
                valor.Valor = valorPedagio;

                unitOfWork.Start();
                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga, null, "Adicionado pedágio " + valor.Descricao + " na Carga.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, valor.AcertoCarga.AcertoViagem, null, "Adicionado pedágio " + valor.Descricao + " na Carga " + valor.AcertoCarga.Descricao + ".", unitOfWork);

                repAcertoCargaPedagio.Inserir(valor, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(null, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo valor do pedágio.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AdicionarPedagioCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaCanhoto();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaCanhoto(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaCanhoto(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarPallets()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaPallet();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaPallet(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaPallet(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSituacaoCanhotoPalletCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller BuscarSituacaoCanhotoPalletCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                dynamic listaCodigos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Codigos"));

                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                var dynListaRetorno = new List<dynamic>();
                foreach (var codigo in listaCodigos)
                {
                    var dynRetorno = new
                    {
                        Codigo = ((string)codigo.Codigo).ToInt(),
                        SituacaoCanhotos = (repCanhoto.ConsultarSituacaoCanhotoCarga(((string)codigo.Codigo).ToInt())?.FirstOrDefault()?.Situacao ?? ""),
                        SituacaoPallets = (repDevolucao.ConsultarSituacaoPalletCarga(((string)codigo.Codigo).ToInt())?.FirstOrDefault()?.Situacao ?? "")
                    };
                    dynListaRetorno.Add(dynRetorno);
                }

                return new JsonpResult(dynListaRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a situação do canhoto e pallet da carga.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller BuscarSituacaoCanhotoPalletCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisaCanhoto()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoLocalArmazenamento", false);
            grid.AdicionarCabecalho("DescricaoLocalArmazenamento", false);
            grid.AdicionarCabecalho("GuidNomeArquivo", false);
            grid.AdicionarCabecalho("Observacao", false);
            grid.AdicionarCabecalho("SituacaoCanhoto", false);
            grid.AdicionarCabecalho("SituacaoDigitalizacaoCanhoto", false);
            grid.AdicionarCabecalho("CargaEncerrada", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.center, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("CT-e", "NumeroCTe", 10, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Doc. Orig.", "NumeroDocumentoOriginario", 10, Models.Grid.Align.left, false, true);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoCanhoto", 8, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Chave da NF-e", "Chave", 20, Models.Grid.Align.left, false, false);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 10, Models.Grid.Align.center, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Emitente", "Emitente", 20, Models.Grid.Align.left, true);
                else
                {
                    grid.AdicionarCabecalho("Valor NF-e", "Valor", 10, Models.Grid.Align.left, false, true);
                    grid.AdicionarCabecalho("Destinatário", "Destinatario", 25, Models.Grid.Align.left, true);
                }
                grid.AdicionarCabecalho("Motorista", "Motorista", 18, Models.Grid.Align.left, false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Nota", "DataNotaFiscal", 10, Models.Grid.Align.left, false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho("CNPJ Destinatário", "CNPJDestinatarioFormatado", 20, Models.Grid.Align.right, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 13, Models.Grid.Align.center, true);

            if (Request.GetEnumParam<SituacaoDigitalizacaoCanhoto>("SituacaoDigitalizacaoCanhoto") == SituacaoDigitalizacaoCanhoto.Todas)
                grid.AdicionarCabecalho("Digitalização", "DescricaoDigitalizacao", 10, Models.Grid.Align.center, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Baixa", "DataEnvioCanhoto", 10, Models.Grid.Align.center, false);
            }
            else
                grid.AdicionarCabecalho("Data Digitalização", "DataDigitalizacao", 10, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("NomeArquivo", false);

            if (ConfiguracaoEmbarcador.UtilizaPgtoCanhoto && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho("Situação Pgto.", "DescricaoSituacaoPgtoCanhoto", 10, Models.Grid.Align.left, false);

            return grid;
        }

        private void PropOrdenaCanhoto(ref string propOrdena)
        {
            if (propOrdena == "DescricaoTipoCanhoto")
                propOrdena = "TipoCanhoto";

            if (propOrdena == "DescricaoDigitalizacao")
                propOrdena = "SituacaoDigitalizacaoCanhoto";

            if (propOrdena == "DescricaoSituacao")
                propOrdena = "SituacaoCanhoto";

            else if (propOrdena == "Emitente")
                propOrdena = "XMLNotaFiscal.Emitente.Nome";

            else if (propOrdena == "Empresa")
                propOrdena += ".RazaoSocial";

            else if (propOrdena == "DataNotaFiscal")
                propOrdena = "XMLNotaFiscal.DataEmissao";

            else if (propOrdena == "DataDigitalizacao")
                propOrdena = "DataEnvioCanhoto";
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> ExecutaPesquisaCanhoto(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa = ObterFiltrosPesquisaCanhoto(unitOfWork);

            totalRegistros = repCanhoto.ContarConsulta(filtrosPesquisa);
            List<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> listaGrid = (totalRegistros > 0) ? repCanhoto.ConsultarDynamic(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite) : new List<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos>();

            return listaGrid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto ObterFiltrosPesquisaCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto
            {
                Carga = Request.GetIntParam("Codigo") > 0 ? Request.GetIntParam("Codigo") : -1
            };
        }

        private Models.Grid.Grid GridPesquisaPallet()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Qtd. Pallets", "NumeroPallets", _tamanhoColunaPequena, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Qtd. Entregue", "NumeroPalletsEntregues", _tamanhoColunaPequena, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Transporte", "DescricaoDataTransporte", _tamanhoColunaMedia, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Devolução", "DescricaoDataDevolucao", _tamanhoColunaMedia, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Empresa/Filial", "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", _tamanhoColunaGrande, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Veiculo", "Veiculo", _tamanhoColunaGrande, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", _tamanhoColunaMedia, Models.Grid.Align.left, false);

            return grid;
        }

        private void PropOrdenaPallet(ref string propOrdena)
        {
            //if (propOrdena == "DescricaoTipoCanhoto")
            //    propOrdena = "TipoCanhoto";

            //if (propOrdena == "DescricaoDigitalizacao")
            //    propOrdena = "SituacaoDigitalizacaoCanhoto";

            //if (propOrdena == "DescricaoSituacao")
            //    propOrdena = "SituacaoCanhoto";

            //else if (propOrdena == "Emitente")
            //    propOrdena = "XMLNotaFiscal.Emitente.Nome";

            //else if (propOrdena == "Empresa")
            //    propOrdena += ".RazaoSocial";

            //else if (propOrdena == "DataNotaFiscal")
            //    propOrdena = "XMLNotaFiscal.DataEmissao";

            //else if (propOrdena == "DataDigitalizacao")
            //    propOrdena = "DataEnvioCanhoto";
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao> ExecutaPesquisaPallet(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucaoPallets = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
            int codigoCarga = Request.GetIntParam("Codigo") > 0 ? Request.GetIntParam("Codigo") : -1;

            totalRegistros = repDevolucaoPallets.ContarConsultaRelatorio(codigoCarga, null, 0, 0, 0, "", configuracaoPaletes.NaoExibirDevolucaoPaletesSemNotaFiscal, null, DateTime.MinValue, DateTime.MinValue);
            List<Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao> listaGrid = totalRegistros > 0 ? repDevolucaoPallets.ConsultarRelatorio(codigoCarga, null, 0, 0, 0, "", configuracaoPaletes.NaoExibirDevolucaoPaletesSemNotaFiscal, null, DateTime.MinValue, DateTime.MinValue, null, "", "", propOrdenar, dirOrdena, inicio, limite) : new List<Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao>();

            return listaGrid;
        }

        #endregion
    }
}


