using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Terceiros/AutorizacaoContratoFreteTerceiro")]
    public class AutorizacaoContratoFreteTerceiroController : BaseController
    {
		#region Construtores

		public AutorizacaoContratoFreteTerceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        //public async Task<IActionResult> REEPROCESSARTODOSCONTRATOSQUEESTAOPENDENTES()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
        //    try
        //    {
        //        // Repositorios
        //        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

        //        // Codigo requisicao
        //        int.TryParse(Request.Params("Codigo"), out int codigo);

        //        // Entidades
        //        List<int> contratos = repContratoFrete.CONTRATOSPARAREPROCESSAR();


        //        for (var i = 0; i < contratos.Count(); i++)
        //        {
        //            unitOfWork.Start();
        //            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(contratos[i]);
        //            List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaFiltrada = Servicos.Embarcador.Terceiros.ContratoFrete.VerificarRegrasAutorizacao(contrato, unitOfWork);

        //            if (listaFiltrada.Count() > 0)
        //            {
        //                if (!Servicos.Embarcador.Terceiros.ContratoFrete.CriarRegrasAutorizacao(listaFiltrada, contrato, contrato.Carga.Operador, TipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork))
        //                    contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;
        //                else
        //                    contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao;
        //            }
        //            else
        //                contrato.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra;

        //            // Persiste dados
        //            repContratoFrete.Atualizar(contrato);

        //            if (!Servicos.Embarcador.Terceiros.ContratoFrete.ProcessarContratoAprovado(contrato, TipoServicoMultisoftware, this.Usuario.Empresa.TipoAmbiente, Auditado, unitOfWork, _conexao.StringConexao, out string erro))
        //            {
        //                unitOfWork.Rollback();
        //                return new JsonpResult(false, true, erro);
        //            }
        //            unitOfWork.CommitChanges();
        //            unitOfWork.FlushAndClear();
        //        }

        //        return new JsonpResult(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}



        #region Métodos Globais
        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroContrato").Nome("Nº Contrato").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("Carga").Nome("Nº Carga").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("TransportadorTerceiro").Nome("Terceiro").Tamanho(25);
            grid.Prop("ValorFreteSubcontratacao").Nome("Val. Contrato").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("ValorOutrosAdiantamento").Nome("Val. Outros Ad.").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("ValorAdiantamento").Nome("Val. Adiantamento").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("DescricaoSituacaoContratoFrete").Nome("Situação").Tamanho(11).Align(Models.Grid.Align.center);

            return grid;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDyn(lista, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

                // Retorna Dados
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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDyn(lista, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(listaProcessada);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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
                // Repositorios
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, "Contrato de frete não encontrado.");

                var dynDados = new
                {
                    contrato.Codigo,
                    Numero = contrato.NumeroContrato,
                    DataEmissaoContrato = contrato.DataEmissaoContrato.ToString("dd/MM/yyyy"),
                    Transbordo = contrato.Transbordo != null ? contrato.Transbordo.Codigo : 0,
                    Carga = contrato.Carga.CodigoCargaEmbarcador,
                    ValorAdiantamento = contrato.ValorAdiantamento.ToString("n2"),
                    contrato.Descontos,
                    SituacaoContratoFrete = contrato.DescricaoSituacaoContratoFrete,
                    PercentualAdiantamento = contrato.PercentualAdiantamento.ToString("n2"),
                    PercentualAbastecimento = contrato.PercentualAbastecimento.ToString("n2"),
                    ValorAbastecimento = contrato.ValorAbastecimento.ToString("n2"),
                    Terceiro = contrato.TransportadorTerceiro.Nome + " (" + contrato.TransportadorTerceiro.Localidade.DescricaoCidadeEstado + ")",
                    ValorFreteSubcontratacao = contrato.ValorFreteSubcontratacao.ToString("n2"),
                    TipoFreteEscolhido = contrato.DescricaoTipoFreteEscolhido,
                    ValorOutrosAdiantamento = contrato.ValorOutrosAdiantamento.ToString("n2"),
                    ValorPedagio = contrato.ValorPedagio.ToString("n2"),
                };

                return new JsonpResult(dynDados);
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

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Usuario"), out int usuario);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);

                // Instancia repositorio 
                Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

                List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> regras = repAprovacaoAlcadaContratoFrete.BuscarPorContratoEUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from contratoAutorizacao in regras
                             select new
                             {
                                 contratoAutorizacao.Codigo,
                                 Regra = TituloRegra(contratoAutorizacao),
                                 Situacao = contratoAutorizacao.DescricaoSituacao,
                                 Usuario = contratoAutorizacao.Usuario?.Nome ?? string.Empty,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAprovacaoAlcadaContratoFrete.VerificarSePodeAprovar(codigo, contratoAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(contratoAutorizacao)
                             }).ToList();

                // Retorna Grid
                grid.setarQuantidadeTotal(lista.Count());
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

        public async Task<IActionResult> AprovarMultiplasLinhas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> contratos = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

                try
                {
                    contratos = ObterContratosSelecionadasSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> contratosAutorizacoes = BuscarRegrasPorContratos(contratos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosContratosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < contratosAutorizacoes.Count(); i++)
                {
                    int codigo = contratosAutorizacoes[i].ContratoFrete.Codigo;

                    if (!codigosContratosVerificados.Contains(codigo))
                        codigosContratosVerificados.Add(codigo);

                    EfetuarAprovacao(contratosAutorizacoes[i], unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratosAutorizacoes[i].ContratoFrete, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosContratosVerificados)
                {
                    if(!this.VerificarSituacaoContrato(repContratoFrete.BuscarPorCodigo(cod), unitOfWork, out string erro))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = contratosAutorizacoes.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplasLinhas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

                // Codigo da regra
                string motivo = Request.Params("Motivo") ?? string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> contratos = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

                try
                {
                    contratos = ObterContratosSelecionadasSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> contratosAutorizacao = BuscarRegrasPorContratos(contratos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosContratosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < contratosAutorizacao.Count(); i++)
                {
                    int codigo = contratosAutorizacao[i].ContratoFrete.Codigo;

                    if (!codigosContratosVerificados.Contains(codigo))
                        codigosContratosVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    contratosAutorizacao[i].Data = DateTime.Now;
                    contratosAutorizacao[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                    contratosAutorizacao[i].Motivo = motivo;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratosAutorizacao[i], null, "Reprovou a regra. Motivo: " + contratosAutorizacao[i].Motivo, unitOfWork);

                    // Atualiza banco
                    repAprovacaoAlcadaContratoFrete.Atualizar(contratosAutorizacao[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratosAutorizacao[i].ContratoFrete, null, "Reprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosContratosVerificados)
                {
                    if (!this.VerificarSituacaoContrato(repContratoFrete.BuscarPorCodigo(cod), unitOfWork, out string erro))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = contratosAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Instancia
                Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContratoFrete.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> contratoAutorizacao = repAprovacaoAlcadaContratoFrete.BuscarPorContratoUsuarioSituacao(codigo, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < contratoAutorizacao.Count(); i++)
                    EfetuarAprovacao(contratoAutorizacao[i], unitOfWork);

                if (!this.VerificarSituacaoContrato(contrato, unitOfWork, out string erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = contratoAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete alcada = repAprovacaoAlcadaContratoFrete.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (alcada == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (alcada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                EfetuarAprovacao(alcada, unitOfWork);

                if (!this.VerificarSituacaoContrato(alcada.ContratoFrete, unitOfWork, out string erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Rejeitar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete contratoAutorizacao = repAprovacaoAlcadaContratoFrete.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (contratoAutorizacao == null || contratoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (contratoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                contratoAutorizacao.Data = DateTime.Now;
                contratoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                contratoAutorizacao.Motivo = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repAprovacaoAlcadaContratoFrete.Atualizar(contratoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, contratoAutorizacao.ContratoFrete, unitOfWork);
                if (!this.VerificarSituacaoContrato(contratoAutorizacao.ContratoFrete, unitOfWork, out string erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "DescricaoSituacaoContratoFrete")
                propOrdena = "SituacaoContratoFrete";
        }


        /* EfetuarAprovacao
         * Aprova a autorizacao da carga
         */
        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete alcada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (alcada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente && alcada.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                alcada.Data = DateTime.Now;
                alcada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada;

                // Atualiza os dados
                repAprovacaoAlcadaContratoFrete.Atualizar(alcada);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, alcada, null, "Aprovou a regra", unitOfWork);

                //if (verificarSeEstaAprovado)
                //{
                //    if (!this.VerificarSituacaoContrato(alcada.ContratoFrete, unitOfWork, out erro))
                //        return false;
                //}

                this.NotificarAlteracao(true, alcada.ContratoFrete, unitOfWork);
            }
        }



        private List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> BuscarRegrasPorContratos(List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> contratos, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> contratoAutorizacao = new List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();

            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato in contratos)
            {
                List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> regras = repAprovacaoAlcadaContratoFrete.BuscarPorContratoUsuarioSituacao(contrato.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);
                contratoAutorizacao.AddRange(regras);
            }

            return contratoAutorizacao;
        }



        private List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> ObterContratosSelecionadasSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> listaContrato = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaContrato, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaObjetosNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ObjetosNaoSelecionados"));
                foreach (var dybObjetoNaoSelecionada in listaObjetosNaoSelecionados)
                    listaContrato.Remove(new Dominio.Entidades.Embarcador.Terceiros.ContratoFrete() { Codigo = (int)dybObjetoNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaObjetosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ObjetosSelecionados"));
                foreach (var dynObjetoSelecionada in listaObjetosSelecionados)
                    listaContrato.Add(repContratoFrete.BuscarPorCodigo((int)dynObjetoSelecionada.Codigo));
            }

            // Retorna lista
            return listaContrato;
        }



        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Define icone
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                if (aprovada)
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                else
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;

                // Emite notificação
                //string titulo = "Contrato Transportador";
                //string mensagem = "Um usuário " + (aprovada ? "aprovou" : "rejeitou") + " o contrato de frete " + contrato.Numero.ToString();
                //serNotificacao.GerarNotificacaoEmail(contrato.Usuario, this.Usuario, contrato.Codigo, string.Empty, titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }



        private bool VerificarSituacaoContrato(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = "";
            try
            {
                // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
                if (contrato.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao)
                {
                    // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                    Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);
                    Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                    Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                    List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> regras = repAprovacaoAlcadaContratoFrete.BuscarRegrasContrato(contrato.Codigo);

                    // Flag de rejeicao
                    bool rejeitada = false;
                    bool aprovada = true;

                    foreach (Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regra in regras)
                    {
                        int pendentes = repAprovacaoAlcadaContratoFrete.ContarPendentes(contrato.Codigo, regra.Codigo);

                        int aprovacoes = repAprovacaoAlcadaContratoFrete.ContarAprovacoesSolicitacao(contrato.Codigo, regra.Codigo);

                        int rejeitadas = repAprovacaoAlcadaContratoFrete.ContarRejeitadas(contrato.Codigo, regra.Codigo);

                        int necessariosParaAprovar = regra.NumeroAprovadores;

                        // Situacao
                        if (rejeitadas > 0)
                            rejeitada = true;
                        if (aprovacoes < necessariosParaAprovar)
                            aprovada = false;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;

                    if (rejeitada)
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Rejeitado;

                    if (aprovada || rejeitada)
                    {
                        contrato.SituacaoContratoFrete = situacao;
                        //contrato.DataAprovacao = DateTime.Now;
                        //contrato.UsuarioAprovador = this.Usuario;

                        repContratoFrete.Atualizar(contrato);

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                        if (rejeitada)
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                        else
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                        // Emite notificação
                        string mensagem = string.Format(Localization.Resources.Terceiros.AutorizacaoContratoFreteTerceiros.ContratoFreteFoi, contrato.NumeroContrato.ToString(), (aprovada ? Localization.Resources.Gerais.Geral.Aprovado : Localization.Resources.Gerais.Geral.Rejeitado));
                        serNotificacao.GerarNotificacao(contrato.Usuario, this.Usuario, contrato.Codigo, "Terceiros/ContratoFrete", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);

                        // Valida proxima etapa
                        if (!Servicos.Embarcador.Terceiros.ContratoFrete.ProcessarContratoAprovado(contrato, TipoServicoMultisoftware, this.Usuario.Empresa.TipoAmbiente, Auditado, unitOfWork, _conexao.StringConexao, out erro))
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                erro = "Ocorreu uma falha ao ajustar os dados do contrato de frete.";
                return false;
            }

            return true;
        }



        /* CoresRegras
         * Retorna a cor da linha de acordo com a situacoa
         */
        private string CoresRegras(Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;
            else
                return "";
        }


        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
            int.TryParse(Request.Params("Carga"), out int carga);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("Empresa"), out int empresa);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete situacaoAux))
                situacao = situacaoAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            lista = repAprovacaoAlcadaContratoFrete.Consultar(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repAprovacaoAlcadaContratoFrete.ContarConsulta(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
            Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete repAprovacaoAlcadaContratoFrete = new Repositorio.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete(unitOfWork);

            var listaProcessada = from item in lista
                                  select new
                                  {
                                      item.Codigo,
                                      item.NumeroContrato,
                                      Carga = item.Carga.CodigoCargaEmbarcador,
                                      TransportadorTerceiro = item.TransportadorTerceiro.Nome,
                                      ValorFreteSubcontratacao = item.ValorFreteSubcontratacao.ToString("n2"),
                                      ValorOutrosAdiantamento = item.ValorOutrosAdiantamento.ToString("n2"),
                                      ValorAdiantamento = item.ValorAdiantamento.ToString("n2"),
                                      item.DescricaoSituacaoContratoFrete,
                                  };

            return listaProcessada.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete regra)
        {
            return regra.RegraContratoFreteTerceiro?.Descricao;
        }
        #endregion
    }
}
