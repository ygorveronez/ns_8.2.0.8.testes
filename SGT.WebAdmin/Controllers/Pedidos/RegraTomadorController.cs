using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/RegraTomador")]
    public class RegraTomadorController : BaseController
    {
		#region Construtores

		public RegraTomadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                double remetente, destinatario, tomador;
                double.TryParse(Request.Params("Remetente"), out remetente);
                double.TryParse(Request.Params("Destinatario"), out destinatario);
                double.TryParse(Request.Params("Tomador"), out tomador);

                bool? origemFilial = null;
                bool? destinoFilial = null;

                if (!string.IsNullOrWhiteSpace(Request.Params("OrigemFilial")))
                    origemFilial = bool.Parse(Request.Params("OrigemFilial"));

                if (!string.IsNullOrWhiteSpace(Request.Params("DestinoFilial")))
                    destinoFilial = bool.Parse(Request.Params("DestinoFilial"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Origem Filial", "OrigemFilial", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino Filial", "DestinoFilial", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo do Tomador", "DescricaoTipoTomador", 30, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Remetente" || propOrdena == "Destinatario")
                    propOrdena = propOrdena + ".Nome";
                if (propOrdena == "DescricaoTipoTomador")
                    propOrdena = "TipoTomador";

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> listaRegraTomador = repRegraTomador.Consultar(tomador, remetente, destinatario, origemFilial, destinoFilial, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegraTomador.ContarConsulta(tomador, remetente, destinatario, origemFilial, destinoFilial, ativo));

                var retorno = (from obj in listaRegraTomador
                               select new
                               {
                                   obj.Codigo,
                                   Remetente = obj.Remetente?.Descricao ?? "",
                                   Destinatario = obj.Destinatario?.Descricao ?? "",
                                   OrigemFilial = obj.Remetente == null ? (obj.OrigemFilial ? "Sim" : "Não") : "",
                                   DestinoFilial = obj.Destinatario == null ? (obj.DestinoFilial ? "Sim" : "Não") : "",
                                   obj.DescricaoTipoTomador,
                                   obj.DescricaoAtivo
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = new Dominio.Entidades.Embarcador.Pedidos.RegraTomador();
                string retorno = preecherRegraTomador(ref regraTomador, unitOfWork);

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    repRegraTomador.Inserir(regraTomador, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
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
                Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = repRegraTomador.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                string retorno = preecherRegraTomador(ref regraTomador, unitOfWork);

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    repRegraTomador.Atualizar(regraTomador, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = repRegraTomador.BuscarPorCodigo(codigo);

                var entidade = new
                {
                    regraTomador.OrigemFilial,
                    regraTomador.DestinoFilial,
                    Destinatario = new { Codigo = regraTomador.Destinatario != null ? regraTomador.Destinatario.CPF_CNPJ : 0, Descricao = regraTomador.Destinatario != null ? regraTomador.Destinatario.Descricao : "" },
                    Remetente = new { Codigo = regraTomador.Remetente != null ? regraTomador.Remetente.CPF_CNPJ : 0, Descricao = regraTomador.Remetente != null ? regraTomador.Remetente.Descricao : "" },
                    Tomador = new { Codigo = regraTomador.Tomador != null ? regraTomador.Tomador.CPF_CNPJ : 0, Descricao = regraTomador.Tomador != null ? regraTomador.Tomador.Descricao : "" },
                    regraTomador.Ativo,
                    regraTomador.Observacao,
                    regraTomador.TipoTomador
                };

                return new JsonpResult(entidade);
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
        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTomador();

            unitOfWork.Dispose();

            return new JsonpResult(configuracoes.ToList());
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoTomador()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CPF/CNPJ Remetente", Propriedade = "CNPJCPFRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> {} },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "CPF/CNPJ Destinatário", Propriedade = "CNPJCPFDestinatario", Tamanho = 200, Obrigatorio = false, Regras =  new List<string> {} },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Origem Filial (Sim/Nao)", Propriedade = "OrigemFilial", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Destino Filial (Sim/Nao)", Propriedade = "DestinoFilial", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Tipo do Tomador", Propriedade = "Tomador", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "CPF/CNPJ Tomador", Propriedade = "CNPJCPFTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> {} },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> {} }
            };

            return configuracoes;
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTomador();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                        Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFDestinatario" select obj).FirstOrDefault();
                        Dominio.Entidades.Cliente destinatario = null;
                        if (colDestinatario != null)
                        {
                            double cpfCNPJDestinatario = double.Parse(Utilidades.String.OnlyNumbers((string)colDestinatario.Valor));
                            destinatario = repCliente.BuscarPorCPFCNPJ(cpfCNPJDestinatario);
                            if (destinatario == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O destinatário informado não está cadastrado na base Multisoftware", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFRemetente" select obj).FirstOrDefault();
                        Dominio.Entidades.Cliente remetente = null;
                        if (colRemetente != null)
                        {
                            double cpfCNPJRemetente = double.Parse(Utilidades.String.OnlyNumbers((string)colRemetente.Valor));
                            remetente = repCliente.BuscarPorCPFCNPJ(cpfCNPJRemetente);
                            if (remetente == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O remetente informado não está cadastrado na base Multisoftware", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                        }


                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTomador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFTomador" select obj).FirstOrDefault();
                        Dominio.Entidades.Cliente tomador = null;
                        if (colTomador != null)
                        {
                            double cpfCNPJTomador = double.Parse(Utilidades.String.OnlyNumbers((string)colTomador.Valor));
                            tomador = repCliente.BuscarPorCPFCNPJ(cpfCNPJTomador);
                            if (tomador == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tomador informado não está cadastrado na base Multisoftware", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinoFilial = (from obj in linha.Colunas where obj.NomeCampo == "DestinoFilial" select obj).FirstOrDefault();
                        bool destinoFilial = false;
                        if (colDestinoFilial != null && destinatario == null)
                        {
                            string strDestinoFilial = Utilidades.String.RemoveDiacritics(colDestinoFilial.Valor);
                            if (strDestinoFilial.ToLower() == "sim" || strDestinoFilial.ToLower() == "1")
                                destinoFilial = true;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colOrigemFilial = (from obj in linha.Colunas where obj.NomeCampo == "OrigemFilial" select obj).FirstOrDefault();
                        bool origemFilial = false;
                        if (colOrigemFilial != null && remetente == null)
                        {
                            string strOrigemFilial = Utilidades.String.RemoveDiacritics(colOrigemFilial.Valor);
                            if (strOrigemFilial.ToLower() == "sim" || strOrigemFilial.ToLower() == "1")
                                origemFilial = true;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoTomador = (from obj in linha.Colunas where obj.NomeCampo == "Tomador" select obj).FirstOrDefault();
                        Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                        if (colTipoTomador != null)
                        {
                            string strTipoTomador = Utilidades.String.RemoveDiacritics(colTipoTomador.Valor);
                            strTipoTomador = strTipoTomador.ToLower();
                            if (strTipoTomador == "0" || strTipoTomador == "remetente")
                                tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                            else if (strTipoTomador == "1" || strTipoTomador == "expedidor")
                                tipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                            else if (strTipoTomador == "2" || strTipoTomador == "recebedor")
                                tipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                            else if (strTipoTomador == "3" || strTipoTomador == "destinatario")
                                tipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                            else if (strTipoTomador == "5" || strTipoTomador == "intermediario")
                                tipoTomador = Dominio.Enumeradores.TipoTomador.Intermediario;
                            else if (strTipoTomador == "4" || strTipoTomador == "outros")
                            {
                                tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                                if(tomador == null)
                                {
                                    unitOfWork.Rollback();
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatório informar o Tomador", i));
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Tipo do Tomador não identificado informe os tipos (remetente, expedidor, recebedor, destinatario ou outros)", i));
                            }
                                
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatório informar o tipo do Tomador", i));
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                        string observacao = "";
                        if (colObservacao != null)
                        {
                            observacao = colObservacao.Valor;
                        }
                        Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomadorExistente = repRegraTomador.BuscarPorParametros(remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0, origemFilial, destinoFilial);

                        if (regraTomadorExistente != null)
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Já existe uma regra cadastrada para essa configuração de Tomador", i));
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = new Dominio.Entidades.Embarcador.Pedidos.RegraTomador();
                            regraTomador.Ativo = true;
                            regraTomador.Observacao = observacao;
                            regraTomador.Destinatario = destinatario;
                            regraTomador.OrigemFilial = origemFilial;
                            regraTomador.DestinoFilial = destinoFilial;
                            regraTomador.Tomador = tomador;
                            regraTomador.TipoTomador = tipoTomador;
                            repRegraTomador.Inserir(regraTomador);

                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }


        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = repRegraTomador.BuscarPorCodigo(codigo);
                repRegraTomador.Deletar(regraTomador, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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

        #region MétodosPrivados

        private string preecherRegraTomador(ref Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            string retorno = "";

            double remetente, destinatario, tomador;
            double.TryParse(Request.Params("Remetente"), out remetente);
            double.TryParse(Request.Params("Destinatario"), out destinatario);
            double.TryParse(Request.Params("Tomador"), out tomador);

            bool ativo, origemFilial, destinoFilial;
            bool.TryParse(Request.Params("Ativo"), out ativo);
            bool.TryParse(Request.Params("OrigemFilial"), out origemFilial);
            bool.TryParse(Request.Params("DestinoFilial"), out destinoFilial);

            Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            Enum.TryParse(Request.Params("TipoTomador"), out tipoTomador);

            regraTomador.Observacao = Request.Params("Observacao");
            regraTomador.Ativo = ativo;

            regraTomador.OrigemFilial = origemFilial;
            regraTomador.DestinoFilial = destinoFilial;

            if (remetente > 0)
            {
                regraTomador.Remetente = repCliente.BuscarPorCPFCNPJ(remetente);
                regraTomador.OrigemFilial = false;
            }
            else
                regraTomador.Remetente = null;

            if (destinatario > 0)
            {
                regraTomador.Destinatario = repCliente.BuscarPorCPFCNPJ(destinatario);
                regraTomador.DestinoFilial = false;
            }
            else
                regraTomador.Destinatario = null;


            if (tomador > 0)
                regraTomador.Tomador = repCliente.BuscarPorCPFCNPJ(tomador);
            else
                regraTomador.Tomador = null;


            regraTomador.TipoTomador = tipoTomador;


            Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomadorExistente = repRegraTomador.BuscarPorParametros(remetente, destinatario, origemFilial, destinoFilial);

            if (regraTomadorExistente != null && regraTomadorExistente.Codigo != regraTomador.Codigo)
                retorno = "Já existe uma regra cadastrada para essa configuração de Tomador";

            return retorno;
        }

        #endregion

    }
}
