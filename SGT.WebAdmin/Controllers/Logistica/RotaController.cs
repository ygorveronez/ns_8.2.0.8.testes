using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/Rota")]
    public class RotaController : BaseController
    {
		#region Construtores

		public RotaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int? codigoOrigem = null;
                int? codigoDestino = null;

                bool somenteNaoIntegradaSemParar = false;

                double cnpjcpfRementente = 0;
                double cnpjcpfDestinatario = 0;

                double.TryParse(Request.Params("Remetente"), out cnpjcpfRementente);
                double.TryParse(Request.Params("Destinatario"), out cnpjcpfDestinatario);

                if (!String.IsNullOrEmpty(Request.Params("Origem")))
                {
                    codigoOrigem = int.Parse(Request.Params("Origem"));
                    if (codigoOrigem.Value == 0)
                        codigoOrigem = null;
                }

                if (!String.IsNullOrEmpty(Request.Params("Destino")))
                {
                    codigoDestino = int.Parse(Request.Params("Destino"));
                    if (codigoDestino.Value == 0)
                        codigoDestino = null;
                }

                bool.TryParse(Request.Params("SomenteNaoIntegradaSemParar"), out somenteNaoIntegradaSemParar);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Origem", "Origem", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Distância", "DistanciaKM", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Pedagios", "NumeroPedagios", 10, Models.Grid.Align.right, true, true, false);


                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Origem" || propOrdenar == "Destino")
                {
                    propOrdenar += ".Descricao";
                }

                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Rota> listaRota = repRota.Consultar(codigoOrigem, codigoDestino, cnpjcpfRementente, cnpjcpfDestinatario, somenteNaoIntegradaSemParar, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repRota.ContarConsulta(codigoOrigem, codigoDestino, cnpjcpfRementente, cnpjcpfDestinatario, somenteNaoIntegradaSemParar);

                grid.recordsTotal = totalRegistros;
                grid.recordsFiltered = totalRegistros;

                dynamic lista = (from p in listaRota
                                 select new
                                 {
                                     Codigo = p.Codigo,
                                     Origem = p.Remetente != null ? (p.Remetente.Descricao + " (" + p.Origem.DescricaoCidadeEstado + ")") : p.Origem.DescricaoCidadeEstado,
                                     Destino = p.Destinatario != null ? (p.Destinatario.Descricao + " (" + p.Destino.DescricaoCidadeEstado + ")") : p.Destino.DescricaoCidadeEstado,
                                     DistanciaKM = p.DistanciaKM,
                                     NumeroPedagios = p.NumeroPedagios != null ? p.NumeroPedagios.Value.ToString() : ""
                                 }
                                 ).ToList();

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
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Rota rota = new Dominio.Entidades.Embarcador.Logistica.Rota();
                rota.Ativo = bool.Parse(Request.Params("Ativo"));
                rota.PossuiPedagio = bool.Parse(Request.Params("PossuiPedagio"));
                rota.DistanciaKM = int.Parse(Request.Params("DistanciaKM"));
                rota.Origem = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Origem")) };
                rota.Destino = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Destino")) };
                rota.DescricaoRotaSemParar = Request.Params("DescricaoRotaSemParar");
                rota.CodigosPracaSemParar = Request.Params("CodigosPracaSemParar");

                double cnpjcpfRementente = 0;
                double cnpjcpfDestinatario = 0;

                double.TryParse(Request.Params("Remetente"), out cnpjcpfRementente);
                double.TryParse(Request.Params("Destinatario"), out cnpjcpfDestinatario);

                if (cnpjcpfRementente > 0)
                {
                    rota.Remetente = repCliente.BuscarPorCPFCNPJ(cnpjcpfRementente);
                    rota.Origem = rota.Remetente.Localidade;
                }

                if (cnpjcpfDestinatario > 0)
                {
                    rota.Destinatario = repCliente.BuscarPorCPFCNPJ(cnpjcpfDestinatario);
                    rota.Destino = rota.Destinatario.Localidade;
                }

                string tempoViagem = Request.Params("TempoViagem");
                if (!string.IsNullOrWhiteSpace(tempoViagem))
                {
                    string[] split = tempoViagem.Split(':');
                    TimeSpan TempoViagem = new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), 0);
                    rota.TempoViagemEmMinutos = (int)TempoViagem.TotalMinutes;
                }

                if (!string.IsNullOrEmpty(Request.Params("NumeroPedagios")))
                {
                    rota.NumeroPedagios = int.Parse(Request.Params("NumeroPedagios"));
                }

                Dominio.Entidades.Embarcador.Logistica.Rota rotaExistente = null;
                if (cnpjcpfRementente > 0 && cnpjcpfDestinatario > 0)
                    rotaExistente = repRota.BuscarRotaPorRemetenteDestino(cnpjcpfRementente, cnpjcpfDestinatario);
                else
                    rotaExistente = repRota.BuscarRotaPorOrigemDestino(rota.Origem.Codigo, rota.Destino.Codigo);

                if (rotaExistente == null)
                {                   

                    repRota.Inserir(rota, Auditado);
                    SalvarFaixaCEP(rota, unitOfWork);
                    return new JsonpResult(true);
                }
                else
                {
                    string remententeDestinatario = "";
                    if (cnpjcpfRementente > 0 && cnpjcpfDestinatario > 0)
                        remententeDestinatario = " para os clientes " + rotaExistente.Remetente.Nome + " e " + rota.Destinatario.Nome;

                    return new JsonpResult(false, true, "Já existe uma rota entre " + rotaExistente.Origem.Descricao + " e " + rotaExistente.Destino.Descricao + remententeDestinatario);
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
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                rota.Ativo = bool.Parse(Request.Params("Ativo"));
                rota.PossuiPedagio = bool.Parse(Request.Params("PossuiPedagio"));
                rota.DistanciaKM = int.Parse(Request.Params("DistanciaKM"));
                rota.DescricaoRotaSemParar = Request.Params("DescricaoRotaSemParar");
                rota.CodigosPracaSemParar = Request.Params("CodigosPracaSemParar");

                if (!string.IsNullOrEmpty(Request.Params("NumeroPedagios")))
                {
                    rota.NumeroPedagios = int.Parse(Request.Params("NumeroPedagios"));
                }
                string tempoViagem = Request.Params("TempoViagem");
                if (!string.IsNullOrWhiteSpace(tempoViagem))
                {
                    string[] split = tempoViagem.Split(':');
                    TimeSpan TempoViagem = new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), 0);
                    rota.TempoViagemEmMinutos = (int)TempoViagem.TotalMinutes;
                }
                else
                {
                    rota.TempoViagemEmMinutos = 0;
                }
                SalvarFaixaCEP(rota, unitOfWork);
                repRota.Atualizar(rota, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarPorCodigo(codigo);

                var entidade = new
                {
                    rota.Ativo,
                    rota.Codigo,
                    rota.DistanciaKM,
                    rota.NumeroPedagios,
                    rota.PossuiPedagio,
                    rota.DescricaoRotaSemParar,
                    rota.CodigosPracaSemParar,
                    Origem = new { Codigo = rota.Origem.Codigo, Descricao = rota.Origem.DescricaoCidadeEstado },
                    Destino = new { Codigo = rota.Destino.Codigo, Descricao = rota.Destino.DescricaoCidadeEstado },
                    Destinatario = new { Codigo = rota.Destinatario?.Codigo ?? 0, Descricao = rota.Destinatario?.Descricao ?? "" },
                    Remetente = new { Codigo = rota.Remetente?.Codigo ?? 0, Descricao = rota.Remetente?.Descricao ?? "" },
                    TempoViagem = rota.TempoViagemEmMinutos > 0 ? string.Format("{0:00}:{1:00}", Math.Floor(TimeSpan.FromMinutes(rota.TempoViagemEmMinutos).TotalHours), TimeSpan.FromMinutes(rota.TempoViagemEmMinutos).Minutes) : "",
                    CEPs = rota.CEPs != null && rota.CEPs.Count > 0 ? (from obj in rota.CEPs
                                                                       select new
                                                                       {
                                                                           CodigoCEP = obj.Codigo,
                                                                           CEPInicial = obj.CEPInicialFormatado,
                                                                           CEPFinal = obj.CEPFinalFormatado
                                                                       }).ToList() : null
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


        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarPorCodigo(codigo);
                repRota.Deletar(rota, Auditado);
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

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoRota(Repositorio.UnitOfWork unitOfWork)
        {
            bool semParar = false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar) != null)
                semParar = true;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CPF/CNPJ Remetente", Propriedade = "CNPJCPFRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "CPF/CNPJ Destinatário", Propriedade = "CNPJCPFDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Código Remetente", Propriedade = "CodigoRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Código Destinatário", Propriedade = "CodigoDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Distância", Propriedade = "Distancia", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            if (semParar)
            {
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Rota Sem Parar", Propriedade = "RotaSemParar", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Praças de Pedágio Sem Parar", Propriedade = "PracasPedagio", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            }

            return configuracoes;
        }


        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoRota(unitOfWork);
                return new JsonpResult(configuracoes.ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoRota(unitOfWork);
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

                        if (destinatario == null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CodigoDestinatario" select obj).FirstOrDefault();
                            if (colCodigoDestinatario != null)
                            {
                                string codigoDestinatario = (string)colCodigoDestinatario.Valor;
                                destinatario = repCliente.BuscarPorCodigoIntegracao(codigoDestinatario);
                                if (destinatario == null)
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O destinatário informado não está cadastrado na base Multisoftware", i));
                                    unitOfWork.Rollback();
                                    continue;
                                }
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

                        if (remetente == null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CodigoRemetente" select obj).FirstOrDefault();
                            if (colCodigoRemetente != null)
                            {
                                string codigoRemetente = (string)colCodigoRemetente.Valor;
                                remetente = repCliente.BuscarPorCodigoIntegracao(codigoRemetente);
                                if (remetente == null)
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O remetente informado não está cadastrado na base Multisoftware", i));
                                    unitOfWork.Rollback();
                                    continue;
                                }
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDistancia = (from obj in linha.Colunas where obj.NomeCampo == "Distancia" select obj).FirstOrDefault();

                        bool informouDistancia = false;
                        int distancia = 0;
                        if (colDistancia != null)
                        {
                            informouDistancia = true;
                            int.TryParse((string)colDistancia.Valor, out distancia);
                        }

                        if (remetente == null || destinatario == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatorio informar o remetente e o destinatário", i));
                            continue;
                        }


                        bool inserir = false;
                        Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorRemetenteDestino(remetente.CPF_CNPJ, destinatario.CPF_CNPJ);
                        if (rota == null)
                        {
                            inserir = true;
                            rota = new Dominio.Entidades.Embarcador.Logistica.Rota();
                            rota.Remetente = remetente;
                            rota.Origem = rota.Remetente.Localidade;
                            rota.Destinatario = destinatario;
                            rota.Destino = rota.Destinatario.Localidade;
                            rota.TempoViagemEmMinutos = 0;
                            rota.NumeroPedagios = 0;
                        }
                        else
                            rota.Initialize();

                        rota.Ativo = true;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRotaSemParar = (from obj in linha.Colunas where obj.NomeCampo == "RotaSemParar" select obj).FirstOrDefault();
                        if (colRotaSemParar != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPracasPedagio = (from obj in linha.Colunas where obj.NomeCampo == "PracasPedagio" select obj).FirstOrDefault();
                            if (colPracasPedagio != null)
                            {
                                rota.CodigosPracaSemParar = (string)colPracasPedagio.Valor;
                                rota.DescricaoRotaSemParar = (string)colRotaSemParar.Valor;
                                rota.PossuiPedagio = true;
                            }
                            else
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatório informar as praças do sem parar", i));
                                continue;
                            }
                        }

                        if (informouDistancia)
                            rota.DistanciaKM = distancia;

                        if (inserir)
                        {
                            repRota.Inserir(rota, Auditado);
                        }
                        else
                        {
                            repRota.Atualizar(rota, Auditado);
                        }

                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);

                        unitOfWork.CommitChanges();
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

        private bool SalvarFaixaCEP(Dominio.Entidades.Embarcador.Logistica.Rota rota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.RotaCEP repRotaCEP = new Repositorio.Embarcador.Logistica.RotaCEP(unitOfWork);

            if (rota.Codigo > 0)
                repRotaCEP.DeletarPorRota(rota.Codigo);

            dynamic dynCEPs = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CEPs"));
            foreach (var dynCEP in dynCEPs)
            {
                Dominio.Entidades.Embarcador.Logistica.RotaCEP rotaCEP = new Dominio.Entidades.Embarcador.Logistica.RotaCEP();
                rotaCEP.Rota = rota;
                rotaCEP.CEPInicial = Utilidades.String.OnlyNumbers(((string)dynCEP.CEPInicial));
                rotaCEP.CEPFinal = Utilidades.String.OnlyNumbers(((string)dynCEP.CEPFinal));

                repRotaCEP.Inserir(rotaCEP, Auditado);
            }
            return true;
        }



        #endregion
    }
}
