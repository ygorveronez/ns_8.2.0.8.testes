using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Localidades
{
    [CustomAuthorize("Localidades/Regiao")]
    public class RegiaoController : BaseController
    {
        #region Construtores

        public RegiaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int localidadePolo, localidadeRegiao;
                int.TryParse(Request.Params("LocalidadePolo"), out localidadePolo);
                int.TryParse(Request.Params("LocalidadeRegiao"), out localidadeRegiao);
                int filial = Request.GetIntParam("Filial");
                int tipoCarga = Request.GetIntParam("TipoCarga");
                int tipoOperacao = Request.GetIntParam("TipoOperacao");

                Dominio.Entidades.Localidade parLocalidadePolo = localidadePolo > 0 ? new Dominio.Entidades.Localidade() { Codigo = localidadePolo } : null;
                Dominio.Entidades.Localidade parLocalidadeRegiao = localidadeRegiao > 0 ? new Dominio.Entidades.Localidade() { Codigo = localidadeRegiao } : null;
                Dominio.Entidades.Embarcador.Filiais.Filial codigoFilial = filial > 0 ? new Dominio.Entidades.Embarcador.Filiais.Filial() { Codigo = filial } : null;
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga codigoTipoCarga = tipoCarga > 0 ? new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = tipoCarga } : null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao codigoTipoOperacao = tipoOperacao > 0 ? new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = tipoOperacao } : null;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Regiao.Descricao, "Descricao", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Regiao.LocalidadePolo, "LocalidadePolo", 20, Models.Grid.Align.left, false, false, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Regiao.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Localidades.Regiao> listaRegiao = repRegiao.Consultar(descricao, codigoIntegracao, parLocalidadePolo, ativo, parLocalidadeRegiao, codigoTipoOperacao, codigoTipoCarga, codigoFilial, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegiao.ContarConsulta(descricao, codigoIntegracao, parLocalidadePolo, ativo, parLocalidadeRegiao, codigoTipoOperacao, codigoTipoCarga, codigoFilial));

                dynamic lista = (from p in listaRegiao
                                 select new
                                 {
                                     Codigo = p.Codigo,
                                     Descricao = p.Descricao,
                                     LocalidadePolo = p.LocalidadePolo == null ? string.Empty : p.LocalidadePolo.DescricaoCidadeEstado,
                                     DescricaoAtivo = p.DescricaoAtivo
                                 }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Consultas.Regiao.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoRegiao();

            return new JsonpResult(configuracoes);
        }

        public async Task<IActionResult> ConfiguracaoImportacaoPrazos()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoRegiaoPrazos();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork, cancellationToken);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);

            try
            {
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                //lista criada apenas para melhorar a performance.
                List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioes = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        await unitOfWork.StartAsync(cancellationToken);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.Entidades.Embarcador.Localidades.Regiao regiao = null;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracao = linha.Colunas.FirstOrDefault(x => x.NomeCampo == "CodigoIntegracao");
                        string codigoIntegracao = "";
                        if (colCodigoIntegracao != null)
                            codigoIntegracao = colCodigoIntegracao.Valor;

                        if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                        {
                            regiao = regioes.FirstOrDefault(obj => obj.CodigoIntegracao == codigoIntegracao);

                            if (regiao == null)
                                regiao = repRegiao.BuscarPorCodigoIntegracao(codigoIntegracao);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricao = linha.Colunas.FirstOrDefault(x => x.NomeCampo == "Descricao");
                        string descricao = "";
                        if (colDescricao != null)
                            descricao = colDescricao.Valor;

                        if (string.IsNullOrWhiteSpace(descricao))
                            throw new ControllerException("É obrigatório informar a descriação da Região");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPrazoDias = linha.Colunas.FirstOrDefault(x => x.NomeCampo == "PrazoDias");
                        string strprazoDias = "";
                        if (colPrazoDias != null)
                            strprazoDias = colPrazoDias.Valor;

                        int diasPrazo = strprazoDias.ToInt();

                        bool inserir = false;

                        if (regiao == null)
                        {
                            regiao = regioes.FirstOrDefault(obj => obj.Descricao == descricao);

                            if (regiao == null)
                                regiao = repRegiao.BuscarPorDescricao(descricao);
                        }

                        if (regiao == null)
                        {
                            regiao = new Dominio.Entidades.Embarcador.Localidades.Regiao();
                            inserir = true;
                        }

                        regiao.Ativo = true;
                        regiao.CodigoIntegracao = codigoIntegracao;
                        regiao.Descricao = descricao;
                        regiao.DiasPrazoEntrega = diasPrazo;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidade = linha.Colunas.FirstOrDefault(x => x.NomeCampo == "Localidade");
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFLocalidade = linha.Colunas.FirstOrDefault(x => x.NomeCampo == "UFLocalidade");

                        Dominio.Entidades.Localidade localidadeRegiao = null;
                        if (colLocalidade != null && colUFLocalidade != null)
                        {
                            string localidade = Utilidades.String.RemoveDiacritics(colLocalidade.Valor);
                            string uf = colUFLocalidade.Valor;
                            localidadeRegiao = repLocalidade.BuscarPorDescricaoEUF(localidade.Trim(), uf.Trim());

                            if (localidadeRegiao == null)
                                throw new ControllerException("Não foi encontrada a localidade na base Multisoftware.");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadePolo = linha.Colunas.FirstOrDefault(x => x.NomeCampo == "LocalidadePolo");
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFLocalidadePolo = linha.Colunas.FirstOrDefault(x => x.NomeCampo == "UFLocalidadePolo");

                        Dominio.Entidades.Localidade localidadePolo = null;
                        if (colLocalidadePolo != null && colUFLocalidadePolo != null)
                        {
                            string localidade = Utilidades.String.RemoveDiacritics(colLocalidadePolo.Valor);
                            string uf = colUFLocalidadePolo.Valor;
                            localidadePolo = repLocalidade.BuscarPorDescricaoEUF(localidade.Trim(), uf.Trim());

                            if (localidadePolo == null)
                                throw new ControllerException("Não foi encontrada a localidade polo na base Multisoftware.");
                        }
                        regiao.LocalidadePolo = localidadePolo;

                        if (inserir)
                            await repRegiao.InserirAsync(regiao);

                        if (!regioes.Contains(regiao))
                        {
                            await repRegiao.AtualizarAsync(regiao);
                            regioes.Add(regiao);
                        }

                        if (localidadeRegiao != null && localidadeRegiao.Regiao?.Codigo != regiao.Codigo)
                        {
                            localidadeRegiao.Regiao = regiao;
                            await repLocalidade.AtualizarAsync(localidadeRegiao);
                        }

                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i, contar: false);
                        retornoImportacao.Retornolinhas.Add(retornoLinha);

                        await unitOfWork.CommitChangesAsync(cancellationToken);
                    }
                    catch (ControllerException exception)
                    {
                        await unitOfWork.RollbackAsync(cancellationToken);
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(exception.Message, i));
                    }
                    catch (Exception ex2)
                    {
                        await unitOfWork.RollbackAsync(cancellationToken);
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha("Ocorreu uma falha ao processar a linha.", i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count;
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ImportarPrazos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega repRegiaoPrazoEntrega = new Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFiliais = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoRegiao();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                //lista criada apenas para melhorar a performance.
                List<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega> regioesPrazoEntrega = new List<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>();

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega regiaoPrazoEntrega = new Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoRegiao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoRegiao" select obj).FirstOrDefault();
                        string codigoIntegracaoRegiao = "";
                        if (colCodigoIntegracaoRegiao != null)
                            codigoIntegracaoRegiao = colCodigoIntegracaoRegiao.Valor;

                        if (string.IsNullOrWhiteSpace(codigoIntegracaoRegiao))
                            retorno = "É obrigatório informar a descriação da Região";

                        Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigoIntegracao(codigoIntegracaoRegiao);

                        if (regiao == null)
                            throw new Exception("Região não encontrada por esse Código de Integração.");

                        if (regiao != null)
                            regiaoPrazoEntrega.Regiao = regiao;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilial = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoFilial" select obj).FirstOrDefault();
                        string codigoIntegracaoFilial = "";
                        if (colFilial != null)
                            codigoIntegracaoFilial = colFilial.Valor;

                        if (string.IsNullOrWhiteSpace(codigoIntegracaoFilial))
                            retorno = "É obrigatório informar o Código de Integração da Filial.";

                        Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFiliais.BuscarPorCodigoIntegracao(codigoIntegracaoFilial);

                        if (filial == null)
                            retorno = "Filial não encontrada por esse Código de Integração.";

                        regiaoPrazoEntrega.Filial = filial;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoDeCarga = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoTipoDeCarga" select obj).FirstOrDefault();
                        string codigoIntegracaoTipoDeCarga = "";
                        if (colTipoDeCarga != null)
                            codigoIntegracaoTipoDeCarga = colTipoDeCarga.Valor;

                        if (string.IsNullOrWhiteSpace(codigoIntegracaoTipoDeCarga))
                            retorno = "É obrigatório informar o Código de Integração do Tipo de Carga.";

                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(codigoIntegracaoTipoDeCarga);

                        if (tipoDeCarga == null)
                            retorno = "Tipo de Carga não encontrada por esse Código de Integração.";

                        regiaoPrazoEntrega.TipoDeCarga = tipoDeCarga;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoTipoOperacao" select obj).FirstOrDefault();
                        string codigoIntegracaoTipoOperacao = "";
                        if (colTipoOperacao != null)
                            codigoIntegracaoTipoOperacao = colTipoOperacao.Valor;

                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigoIntegracao(codigoIntegracaoTipoOperacao);

                        if (tipoOperacao == null)
                            retorno = "Tipo de Operação não encontrada por esse Código de Integração.";

                        regiaoPrazoEntrega.TipoOperacao = tipoOperacao;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoPrazo = (from obj in linha.Colunas where obj.NomeCampo == "PadraoTempo" select obj).FirstOrDefault();

                        string padraoTempo = "";
                        if (colTipoPrazo != null)
                            padraoTempo = colTipoPrazo.Valor;

                        if (string.IsNullOrWhiteSpace(padraoTempo))
                            retorno = "É obrigatório informar o Padrão de Tempo.";

                        PadraoTempoDiasMinutos padraoTempoDiasMinutos = PadraoTempoDiasMinutosHelper.ConverterImportacaoEnumerador(padraoTempo);

                        regiaoPrazoEntrega.PadraoTempo = padraoTempoDiasMinutos;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPrazo = (from obj in linha.Colunas where obj.NomeCampo == "Prazo" select obj).FirstOrDefault();
                        string prazo = "";
                        if (colPrazo != null)
                            prazo = colPrazo.Valor;

                        int tempoDeViagemEmMinutos = prazo.ToInt();
                        if (tempoDeViagemEmMinutos <= 0)
                            retorno = "É obrigatório informar o Prazo.";

                        regiaoPrazoEntrega.TempoDeViagemEmMinutos = padraoTempoDiasMinutos == PadraoTempoDiasMinutos.Dias ? tempoDeViagemEmMinutos * 1440 : tempoDeViagemEmMinutos;

                        if (repRegiaoPrazoEntrega.ExisteRegraDuplicada(codigoIntegracaoRegiao, codigoIntegracaoFilial, codigoIntegracaoTipoDeCarga, codigoIntegracaoTipoOperacao))
                            retorno = "Já existe um registro com os mesmos dados.";
                        else
                            repRegiaoPrazoEntrega.Inserir(regiaoPrazoEntrega);

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(retorno, i));
                        }
                        else
                        {
                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i, contar: false);
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(ex2.Message, i));
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Repositorio.Localidade repLocalidades = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega repRegiaoPrazoEntrega = new Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = new Dominio.Entidades.Embarcador.Localidades.Regiao();

                PreencherRegiao(regiao, unitOfWork);

                if (string.IsNullOrWhiteSpace(regiao.CodigoIntegracao) || repRegiao.BuscarPorCodigoIntegracao(regiao.CodigoIntegracao) == null)
                {
                    repRegiao.Inserir(regiao, Auditado);

                    dynamic dynLocalidades = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Localidades"));
                    foreach (var dynLocalidade in dynLocalidades)
                    {
                        Dominio.Entidades.Localidade localidade = repLocalidades.BuscarPorCodigo((int)dynLocalidade.Codigo);
                        if (localidade.Regiao == null || localidade.Regiao.Codigo == regiao.Codigo)
                        {
                            localidade.Regiao = regiao;
                            repLocalidades.Atualizar(localidade, Auditado);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "A localidade " + localidade.DescricaoCidadeEstado + " já pertence a região " + localidade.Regiao.Descricao);
                        }
                    }

                    SalvarRegioesPrazoEntrega(regiao, unitOfWork);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma Região com este Código de Integração");
                }
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Repositorio.Localidade repLocalidades = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega repRegiaoPrazoEntrega = new Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigo(codigo, true);

                if (regiao == null)
                    return new JsonpResult(false, "Registro não foi encontrado.");

                PreencherRegiao(regiao, unitOfWork);

                for (int i = 0; i < regiao.Localidades.Count; i++)
                {
                    Dominio.Entidades.Localidade localidade = regiao.Localidades[i];
                    localidade.Regiao = null;
                    repLocalidades.Atualizar(localidade);
                }

                dynamic dynLocalidades = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Localidades"));
                foreach (var dynLocalidade in dynLocalidades)
                {
                    Dominio.Entidades.Localidade localidade = repLocalidades.BuscarPorCodigo((int)dynLocalidade.Codigo, true);
                    if (localidade.Regiao == null || localidade.Regiao.Codigo == regiao.Codigo)
                    {
                        localidade.Regiao = regiao;
                        repLocalidades.Atualizar(localidade, Auditado);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        Dominio.Entidades.Embarcador.Localidades.Regiao regiaoOriginal = repRegiao.BuscarPorCodigo(localidade.Regiao.Codigo);
                        return new JsonpResult(false, true, "A localidade " + localidade.DescricaoCidadeEstado + " já pertence a região " + regiaoOriginal.Descricao);
                    }
                }

                Dominio.Entidades.Embarcador.Localidades.Regiao regiaoExiste = null;
                if (!string.IsNullOrWhiteSpace(regiao.CodigoIntegracao))
                    regiaoExiste = repRegiao.BuscarPorCodigoIntegracaoDiferente(regiao.CodigoIntegracao, regiao.Codigo);

                SalvarRegioesPrazoEntrega(regiao, unitOfWork);

                if (regiaoExiste != null && regiaoExiste.Codigo != regiao.Codigo)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma Região com este Código de Integração");
                }
                else
                {
                    repRegiao.Atualizar(regiao, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega repositorioRegiaoPrazoEntrega = new Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega> regiaoPrazoEntrega = repositorioRegiaoPrazoEntrega.BuscarPorRegiao(codigo);

                if (regiaoPrazoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynRegiao = new
                {
                    regiao.Codigo,
                    regiao.Descricao,
                    regiao.CodigoIntegracao,
                    LocalidadePolo = regiao.LocalidadePolo == null ? null : new { Codigo = regiao.LocalidadePolo.Codigo, Descricao = regiao.LocalidadePolo.DescricaoCidadeEstado },
                    regiao.Ativo,
                    regiao.DiasPrazoEntrega,
                    Localidades = (from obj in regiao.Localidades
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Descricao
                                   }).ToList(),
                    RegioesPrazoEntrega = (from obj in regiaoPrazoEntrega
                                           select new
                                           {
                                               obj.Codigo,
                                               Filial = new { Codigo = obj.Filial?.Codigo ?? 0, Descricao = obj.Filial?.Descricao ?? string.Empty },
                                               TipoOperacao = new { Codigo = obj.TipoOperacao?.Codigo ?? 0, Descricao = obj.TipoOperacao?.Descricao ?? string.Empty },
                                               TipoDeCarga = new { Codigo = obj.TipoDeCarga?.Codigo ?? 0, Descricao = obj.TipoDeCarga?.Descricao ?? string.Empty },
                                               obj.PadraoTempo,
                                               obj.TempoDeViagemEmMinutos,
                                               TempoDeViagemEmDias = obj.PadraoTempo == PadraoTempoDiasMinutos.Dias ? (obj.TempoDeViagemEmMinutos / 1440) : 0,
                                           }).ToList(),
                    Recebedor = new { Codigo = regiao.Recebedor?.Codigo ?? 0, Descricao = regiao.Recebedor?.Descricao ?? string.Empty },
                };
                return new JsonpResult(dynRegiao);
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
                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigo(codigo);
                repRegiao.Deletar(regiao, Auditado);
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

        public async Task<IActionResult> ObterCidades()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string estado = Request.GetStringParam("Estado");
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                List<Dominio.Entidades.Localidade> cidades = repLocalidade.BuscarPorUF(estado, 0);

                dynamic retorno = (from c in cidades
                                   select new
                                   {
                                       Codigo = c.Codigo,
                                       Descricao = c.Descricao
                                   }).ToList();

                return new JsonpResult(retorno, true, string.Empty);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha na buscas das localidades.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherRegiao(Dominio.Entidades.Embarcador.Localidades.Regiao regiao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidades = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            regiao.Ativo = bool.Parse(Request.Params("Ativo"));
            regiao.Descricao = Request.Params("Descricao");
            regiao.CodigoIntegracao = Request.Params("CodigoIntegracao");
            regiao.DiasPrazoEntrega = Request.GetIntParam("DiasPrazoEntrega");

            int codigoLocalidadePolo = Request.GetIntParam("LocalidadePolo");
            regiao.LocalidadePolo = codigoLocalidadePolo > 0 ? repLocalidades.BuscarPorCodigo(codigoLocalidadePolo) : null;

            double codigoRecebedor = Request.GetDoubleParam("Recebedor");
            regiao.Recebedor = codigoRecebedor > 0 ? repCliente.BuscarPorCPFCNPJ(codigoRecebedor) : null;

        }

        private void SalvarRegioesPrazoEntrega(Dominio.Entidades.Embarcador.Localidades.Regiao regiao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega repositorioRegiaoPrazoEntrega = new Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            dynamic dynRegioesPrazoEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegioesPrazoEntrega"));

            List<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega> regioesPrazoEntrega = repositorioRegiaoPrazoEntrega.BuscarPorRegiao(regiao.Codigo);

            if (regioesPrazoEntrega.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic dynRegiaoPrazoEntrega in dynRegioesPrazoEntrega)
                {
                    int codigo = ((string)dynRegiaoPrazoEntrega.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega> deletar = (from obj in regioesPrazoEntrega where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < deletar.Count; i++)
                    repositorioRegiaoPrazoEntrega.Deletar(deletar[i]);
            }

            foreach (dynamic dynRegiaoPrazoEntrega in dynRegioesPrazoEntrega)
            {
                int codigo = ((string)dynRegiaoPrazoEntrega.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega regiaoPrazoEntrega = codigo > 0 ? repositorioRegiaoPrazoEntrega.BuscarPorCodigo(codigo, false) : null;

                if (regiaoPrazoEntrega == null)
                {
                    regiaoPrazoEntrega = new Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega();

                    int codigoFilial = ((string)dynRegiaoPrazoEntrega.Filial.Codigo).ToInt();
                    int codigoTipoDeCarga = ((string)dynRegiaoPrazoEntrega.TipoDeCarga.Codigo).ToInt();
                    int codigoTipoOperacao = ((string)dynRegiaoPrazoEntrega.TipoOperacao.Codigo).ToInt();
                    int tempoDeViagemEmMinutos = ((string)dynRegiaoPrazoEntrega.TempoDeViagemEmMinutos).ToInt();
                    int tempoDeViagemEmDias = ((string)dynRegiaoPrazoEntrega.TempoDeViagemEmDias).ToInt();

                    regiaoPrazoEntrega.Regiao = regiao;
                    regiaoPrazoEntrega.Filial = repositorioFilial.BuscarPorCodigo(codigoFilial);
                    regiaoPrazoEntrega.TipoDeCarga = codigoTipoDeCarga > 0 ? repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga) : null;
                    regiaoPrazoEntrega.TipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
                    regiaoPrazoEntrega.PadraoTempo = ((string)dynRegiaoPrazoEntrega.PadraoTempo).ToEnum<PadraoTempoDiasMinutos>();
                    regiaoPrazoEntrega.TempoDeViagemEmMinutos = regiaoPrazoEntrega.PadraoTempo == PadraoTempoDiasMinutos.Dias ? tempoDeViagemEmDias * 1440 : tempoDeViagemEmMinutos;

                    repositorioRegiaoPrazoEntrega.Inserir(regiaoPrazoEntrega);
                }
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoRegiao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes =
            [
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Codigo de Integração", Propriedade = "CodigoIntegracao", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Prazo em dias", Propriedade = "PrazoDias", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Localidade da Região", Propriedade = "Localidade", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "UF Localidade da Região", Propriedade = "UFLocalidade", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Localidade Polo da Região", Propriedade = "LocalidadePolo", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "UF Localidade polo da Região", Propriedade = "UFLocalidadePolo", Tamanho = 200 },
            ];

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoRegiaoPrazos()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Região", Propriedade = "CodigoIntegracaoRegiao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Filial", Propriedade = "CodigoIntegracaoFilial", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Tipo de Operação", Propriedade = "CodigoIntegracaoTipoOperacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Tipo de Carga", Propriedade = "CodigoIntegracaoTipoDeCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Tipo do Prazo", Propriedade = "PadraoTempo", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Prazo", Propriedade = "Prazo", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        #endregion
    }
}
