using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Newtonsoft.Json;
using Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Frota;
using Newtonsoft.Json.Linq;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/Pneu")]
    public class PneuController : BaseController
    {
        #region Construtores

        public PneuController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frota.Pneu pneu = new Dominio.Entidades.Embarcador.Frota.Pneu();

                try
                {
                    PreencherPneu(pneu, unitOfWork);

                    pneu.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu.Disponivel;
                    pneu.ValorAquisicao = Request.GetDecimalParam("ValorAquisicao");
                    pneu.ValorCustoAtualizado = pneu.ValorAquisicao;
                    pneu.ValorCustoKmAtualizado = pneu.ValorAquisicao;
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

                if (!string.IsNullOrWhiteSpace(pneu.NumeroFogo))
                {
                    if (repositorio.ContemPneuMesmoNumeroFogo(pneu.NumeroFogo, pneu.Codigo))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Já existe uma pneu cadastrado com o mesmo número de fogo.");
                    }
                }

                repositorio.Inserir(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (pneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherPneu(pneu, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                if (!string.IsNullOrWhiteSpace(pneu.NumeroFogo))
                {
                    if (repositorio.ContemPneuMesmoNumeroFogo(pneu.NumeroFogo, pneu.Codigo))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Já existe uma pneu cadastrado com o mesmo número de fogo.");
                    }
                }

                repositorio.Atualizar(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repositorio.BuscarPorCodigo(codigo);

                if (pneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pneu.Codigo,
                    Almoxarifado = new { pneu.Almoxarifado.Codigo, pneu.Almoxarifado.Descricao },
                    BandaRodagem = new { pneu.BandaRodagem.Codigo, pneu.BandaRodagem.Descricao },
                    DataEntrada = pneu.DataEntrada.ToString("dd/MM/yyyy"),
                    pneu.DescricaoNota,
                    DocumentoEntradaItem = new { Codigo = pneu.DocumentoEntradaItem?.Codigo ?? 0, Descricao = pneu.DocumentoEntradaItem?.Descricao ?? "" },
                    pneu.DTO,
                    pneu.KmAtualRodado,
                    KmRodadoEntreSulcos = (pneu.KmRodadoEntreSulcos > 0) ? pneu.KmRodadoEntreSulcos.ToString("n0") : "",
                    Modelo = new { pneu.Modelo.Codigo, pneu.Modelo.Descricao },
                    pneu.NumeroFogo,
                    Produto = new { pneu.Produto.Codigo, pneu.Produto.Descricao },
                    Sulco = (pneu.Sulco > 0m) ? pneu.Sulco.ToString("n2") : "",
                    SulcoAnterior = (pneu.SulcoAnterior > 0m) ? pneu.SulcoAnterior.ToString("n2") : "",
                    pneu.TipoAquisicao,
                    pneu.ValorAquisicao,
                    pneu.ValorCustoAtualizado,
                    pneu.ValorCustoKmAtualizado,
                    pneu.VidaAtual,
                    pneu.Situacao,
                    pneu.Calibragem,
                    pneu.Milimitragem1,
                    pneu.Milimitragem2,
                    pneu.Milimitragem3,
                    pneu.Milimitragem4
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (pneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(pneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro, pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você envie o mesmo para sucata.");
                else
                {
                    Servicos.Log.TratarErro(excecao);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarPneuLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int qtdePneus = Request.GetIntParam("QuantidadeDePneu");
                int numeroInicial = Request.GetIntParam("NumeroInicial");
                int numeroFinal = Request.GetIntParam("NumeroFinal");
                TipoLancamentoPneu tipoLancamento = Request.GetEnumParam<TipoLancamentoPneu>("TipoLancamento");

                string retorno = string.Empty;

                int codigoEmpresa = 0;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                unitOfWork.Start();

                if (TipoLancamentoPneu.PorQuantidade == tipoLancamento)
                    for (int i = 1; i <= qtdePneus; i++)
                        GerarPneus(unitOfWork, codigoEmpresa, i, out retorno, tipoLancamento);

                if (TipoLancamentoPneu.PorFaixa == tipoLancamento)
                    for (int i = numeroInicial; i <= numeroFinal; i++)
                        GerarPneus(unitOfWork, codigoEmpresa, i, out retorno, tipoLancamento);

                if (!string.IsNullOrEmpty(retorno))
                    return new JsonpResult(false, true, retorno);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar pneus em lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RetornarDaSucata()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repositorio.BuscarPorCodigo(codigo, true);

                if (pneu == null || pneu.Almoxarifado == null)
                    return new JsonpResult(false, true, "Favor informe o almoxarifado do Pneu antes de retornar ele da sucata.");

                AdicionarRetornoSucata(unitOfWork, pneu);
                AdicionarHistoricoTrocaAlmoxarifado(unitOfWork, pneu);
                RetornoEstoqueProduto(unitOfWork, pneu, TipoServicoMultisoftware);

                pneu.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu.Disponivel;

                repositorio.Atualizar(pneu, Auditado);

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
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao retornar da sucata.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPneu(unitOfWork);

            return new JsonpResult(configuracoes.ToList());
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Frota.ModeloPneu repModeloPneu = new Repositorio.Embarcador.Frota.ModeloPneu(unitOfWork);
            Repositorio.Embarcador.Frota.BandaRodagemPneu repBandaRodagem = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);
            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

            try
            {
                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPneu(unitOfWork);
                string dados = Request.Params("Dados");
                bool atualizar = JObject.Parse(Request.Params("Parametro"))["Atualizar"].Value<bool>();
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
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
                        string retorno = "";

                        Dominio.Entidades.Embarcador.Frota.Pneu pneu = new Dominio.Entidades.Embarcador.Frota.Pneu();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumFogo = (from obj in linha.Colunas where obj.NomeCampo == "NumFogo" select obj).FirstOrDefault();
                        pneu.NumeroFogo = "";
                        if (colNumFogo != null)
                            pneu.NumeroFogo = (colNumFogo.Valor).Trim();
                        else
                            retorno += "Número de fogo é obrigatório; ";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDTO = (from obj in linha.Colunas where obj.NomeCampo == "DTO" select obj).FirstOrDefault();
                        pneu.DTO = "";
                        if (colDTO != null)
                            pneu.DTO = (colDTO.Valor).Trim();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEntrada = (from obj in linha.Colunas where obj.NomeCampo == "Data" select obj).FirstOrDefault();
                        if (colDataEntrada != null)
                        {
                            if (DateTime.TryParse(((string)colDataEntrada.Valor).Trim(), out DateTime dataEntrada) && dataEntrada != DateTime.MinValue)
                                pneu.DataEntrada = dataEntrada;
                            else
                                retorno += "Erro ao converter Data de Entrada; ";
                        }
                        else
                            retorno += "Data de Entrada é obrigatória; ";


                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoAquisicao = (from obj in linha.Colunas where obj.NomeCampo == "TipoAquisicao" select obj).FirstOrDefault();
                        if (colTipoAquisicao != null)
                        {
                            try
                            {
                                pneu.TipoAquisicao = ((((string)colTipoAquisicao.Valor).Trim()).ObterSomenteNumeros()).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu>() ?? TipoAquisicaoPneu.PneuNovoVeiculoNovo;
                            }
                            catch
                            {
                                retorno += "Erro ao localizar Tipo de Aquisição; ";
                            }
                        }
                        else
                            retorno += "Tipo de Aquisição é obrigatório; ";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVidaAtual = (from obj in linha.Colunas where obj.NomeCampo == "VidaAtual" select obj).FirstOrDefault();
                        if (colVidaAtual != null)
                        {
                            try
                            {
                                pneu.VidaAtual = ((((string)colVidaAtual.Valor).Trim()).ObterSomenteNumeros()).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu>() ?? VidaPneu.PneuNovo;
                            }
                            catch
                            {
                                retorno += "Erro ao localizar Vida Atual; ";
                            }
                        }
                        else
                        {
                            if (pneu.TipoAquisicao == TipoAquisicaoPneu.PneuNovoVeiculoNovo || pneu.TipoAquisicao == TipoAquisicaoPneu.PneuNovoReposicao)
                                pneu.VidaAtual = VidaPneu.PneuNovo;
                            else
                                retorno += "Vida Atual é obrigatória; ";
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModelo = (from obj in linha.Colunas where obj.NomeCampo == "Modelo" select obj).FirstOrDefault();
                        if (colModelo != null)
                        {
                            try
                            {
                                var modeloPneu = repModeloPneu.BuscarPorCodigo((((string)colModelo.Valor).Trim()).ToInt());
                                if (modeloPneu == null)
                                    throw new Exception($"Modelo código {((string)colModelo.Valor)} não encontrado");
                                pneu.Modelo = modeloPneu;
                            }
                            catch (Exception ex)
                            {
                                retorno += $"Erro ao localizar modelo: {ex.Message}; ";
                            }
                        }
                        else
                            retorno += "Modelo é obrigatório; ";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBandaRodagem = (from obj in linha.Colunas where obj.NomeCampo == "BandaRodagem" select obj).FirstOrDefault();
                        if (colBandaRodagem != null)
                        {
                            try
                            {
                                var bandaRodagem = repBandaRodagem.BuscarPorCodigo((((string)colBandaRodagem.Valor).Trim()).ToInt());
                                if (bandaRodagem == null)
                                    throw new Exception($"Banda de Rodagem código {((string)colBandaRodagem.Valor)} não encontrada");
                                pneu.BandaRodagem = bandaRodagem;
                            }
                            catch (Exception ex)
                            {
                                retorno += $"Erro ao localizar a Banda de Rodagem: {ex.Message}; ";
                            }
                        }
                        else
                            retorno += "Banda de Rodagem é obrigatória; ";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAlmoxarifado = (from obj in linha.Colunas where obj.NomeCampo == "Almoxarifado" select obj).FirstOrDefault();
                        if (colAlmoxarifado != null)
                        {
                            try
                            {
                                var almoxarifado = repAlmoxarifado.BuscarPorCodigo((((string)colAlmoxarifado.Valor).Trim()).ToInt());
                                if (almoxarifado == null)
                                    throw new Exception($"Almoxarifado código {((string)colAlmoxarifado.Valor)} não encontrado");
                                pneu.Almoxarifado = almoxarifado;
                            }
                            catch (Exception ex)
                            {
                                retorno += $"Erro ao localizar o Almoxarifado:  {ex.Message}; ";
                            }
                        }
                        else
                            retorno += "Almoxarifado é obrigatório; ";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricaoNota = (from obj in linha.Colunas where obj.NomeCampo == "DescricaoNota" select obj).FirstOrDefault();
                        pneu.DescricaoNota = "";
                        if (colDescricaoNota != null)
                            pneu.DescricaoNota = ((string)colDescricaoNota.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colProduto = (from obj in linha.Colunas where obj.NomeCampo == "Produto" select obj).FirstOrDefault();
                        if (colProduto != null)
                        {
                            try
                            {
                                var produto = repProduto.BuscarPorCodigo((((string)colProduto.Valor).Trim()).ToInt());
                                if (produto == null)
                                    throw new Exception($"Produto código {((string)colProduto.Valor)} não encontrado");
                                pneu.Produto = produto;
                            }
                            catch (Exception ex)
                            {
                                retorno += $"Erro ao localizar o Produto: {ex.Message}; ";
                            }
                        }
                        else
                            retorno += "Produto é obrigatório; ";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSulcoAnterior = (from obj in linha.Colunas where obj.NomeCampo == "SulcoAnterior" select obj).FirstOrDefault();
                        if (colSulcoAnterior != null)
                            pneu.SulcoAnterior = (((string)colSulcoAnterior.Valor).Trim()).ToNullableDecimal() ?? 0;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSulcoAtual = (from obj in linha.Colunas where obj.NomeCampo == "SulcoAtual" select obj).FirstOrDefault();
                        if (colSulcoAtual != null)
                            pneu.Sulco = (((string)colSulcoAtual.Valor).Trim()).ToNullableDecimal() ?? 0;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colKMEntreSulcos = (from obj in linha.Colunas where obj.NomeCampo == "KMEntreSulcos" select obj).FirstOrDefault();
                        if (colKMEntreSulcos != null)
                            pneu.KmRodadoEntreSulcos = (((string)colKMEntreSulcos.Valor).Trim()).ToNullableInt() ?? 0;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colKMAtualRodado = (from obj in linha.Colunas where obj.NomeCampo == "KMAtualRodado" select obj).FirstOrDefault();
                        if (colKMAtualRodado != null)
                            pneu.KmAtualRodado = (((string)colKMAtualRodado.Valor).Trim()).ToNullableInt() ?? 0;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorAquisicao = (from obj in linha.Colunas where obj.NomeCampo == "ValorAquisicao" select obj).FirstOrDefault();
                        if (colValorAquisicao != null)
                            pneu.ValorAquisicao = (((string)colValorAquisicao.Valor).Trim()).ToNullableDecimal() ?? 0;
                        else
                            retorno += "Valor de Aquisição é obrigatório; ";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCustoAtualizado = (from obj in linha.Colunas where obj.NomeCampo == "CustoAtualizado" select obj).FirstOrDefault();
                        if (colCustoAtualizado != null)
                            pneu.ValorCustoAtualizado = (((string)colCustoAtualizado.Valor).Trim()).ToNullableDecimal() ?? 0;
                        else
                            pneu.ValorCustoAtualizado = pneu.ValorAquisicao;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCustoKMAtualizado = (from obj in linha.Colunas where obj.NomeCampo == "CustoKMAtualizado" select obj).FirstOrDefault();
                        if (colCustoKMAtualizado != null)
                            pneu.ValorCustoKmAtualizado = (((string)colCustoKMAtualizado.Valor).Trim()).ToNullableDecimal() ?? 0;
                        else
                            pneu.ValorCustoKmAtualizado = pneu.ValorAquisicao;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCalibragem = (from obj in linha.Colunas where obj.NomeCampo == "Calibragem" select obj).FirstOrDefault();
                        if (colCalibragem != null)
                            pneu.Calibragem = (((string)colCalibragem.Valor).Trim()).ToNullableInt() ?? 0;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMilimitragem1 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem1" select obj).FirstOrDefault();
                        if (colMilimitragem1 != null)
                            pneu.Milimitragem1 = colMilimitragem1.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMilimitragem2 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem2" select obj).FirstOrDefault();
                        if (colMilimitragem2 != null)
                            pneu.Milimitragem2 = colMilimitragem2.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMilimitragem3 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem3" select obj).FirstOrDefault();
                        if (colMilimitragem3 != null)
                            pneu.Milimitragem3 = colMilimitragem3.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMilimitragem4 = (from obj in linha.Colunas where obj.NomeCampo == "Milimitragem4" select obj).FirstOrDefault();
                        if (colMilimitragem4 != null)
                            pneu.Milimitragem4 = colMilimitragem4.Valor;

                        pneu.Situacao = SituacaoPneu.Disponivel;

                        //Salvar o pneu
                        try
                        {
                            if (string.IsNullOrWhiteSpace(retorno))
                                SalvarImportacaoPneu(pneu, unitOfWork, atualizar);
                        }
                        catch (Exception ex)
                        {
                            retorno += ex.Message;
                        }

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            contador++;
                            retornoImportacao.Retornolinhas.Add(RetornarSucessoLinha(i));
                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Falha ao importar linha", i));
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
                return new JsonpResult(false, "Falha ao importar arquivo");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Frota.Almoxarifado ObterAlmoxarifado(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoAlmoxarifado = Request.GetIntParam("Almoxarifado");
            Repositorio.Embarcador.Frota.Almoxarifado repositorio = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoAlmoxarifado) ?? throw new ControllerException("Almoxarifado não encontrado");
        }

        private Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu ObterBandaRodagemPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoBandaRodagem = Request.GetIntParam("BandaRodagem");
            Repositorio.Embarcador.Frota.BandaRodagemPneu repositorio = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoBandaRodagem) ?? throw new ControllerException("Banda de rodagem não encontrada");
        }

        private Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem ObterDocumentoEntradaItem(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoDocumentoEntradaItem = Request.GetIntParam("DocumentoEntradaItem");

            if (codigoDocumentoEntradaItem <= 0)
                return null;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repositorio = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoDocumentoEntradaItem) ?? throw new ControllerException("Item do documento de entrada não encontrado");
        }

        private Dominio.Entidades.Embarcador.Frota.ModeloPneu ObterModeloPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoModelo = Request.GetIntParam("Modelo");
            Repositorio.Embarcador.Frota.ModeloPneu repositorio = new Repositorio.Embarcador.Frota.ModeloPneu(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoModelo) ?? throw new ControllerException("Modelo não encontrado");
        }

        private Dominio.Entidades.Produto ObterProduto(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoProduto = Request.GetIntParam("Produto");
            Repositorio.Produto repositorio = new Repositorio.Produto(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoProduto) ?? throw new ControllerException("Produto não encontrado");
        }

        private void PreencherPneu(Dominio.Entidades.Embarcador.Frota.Pneu pneu, Repositorio.UnitOfWork unitOfWork, int numeroFogo = 0)
        {
            pneu.NumeroFogo = numeroFogo > 0 ? numeroFogo.ToString("D").ObterSomenteNumeros() : Request.GetNullableStringParam("NumeroFogo") ?? throw new ControllerException("O número de fogo deve ser informado");
            pneu.DTO = Request.GetNullableStringParam("DTO"); //?? throw new ControllerException("O DTO deve ser informado");
            pneu.Modelo = ObterModeloPneu(unitOfWork);
            pneu.BandaRodagem = ObterBandaRodagemPneu(unitOfWork);
            pneu.Almoxarifado = ObterAlmoxarifado(unitOfWork);
            pneu.DataEntrada = Request.GetDateTimeParam("DataEntrada");
            pneu.TipoAquisicao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu>("TipoAquisicao");
            pneu.VidaAtual = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu>("VidaAtual");
            pneu.DescricaoNota = Request.GetNullableStringParam("DescricaoNota") ?? string.Empty;
            pneu.DocumentoEntradaItem = ObterDocumentoEntradaItem(unitOfWork);
            pneu.Produto = ObterProduto(unitOfWork);
            pneu.Sulco = Request.GetDecimalParam("Sulco");
            pneu.SulcoAnterior = Request.GetDecimalParam("SulcoAnterior");
            pneu.KmAtualRodado = Request.GetIntParam("KmAtualRodado");
            pneu.KmRodadoEntreSulcos = Request.GetIntParam("KmRodadoEntreSulcos");
            pneu.Calibragem = Request.GetIntParam("Calibragem");
            pneu.Milimitragem1 = Request.GetDecimalParam("Milimitragem1");
            pneu.Milimitragem2 = Request.GetDecimalParam("Milimitragem2");
            pneu.Milimitragem3 = Request.GetDecimalParam("Milimitragem3");
            pneu.Milimitragem4 = Request.GetDecimalParam("Milimitragem4");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                pneu.Empresa = this.Usuario.Empresa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu()
                {
                    CodigoAlmoxarifado = Request.GetIntParam("Almoxarifado"),
                    CodigoBandaRodagem = Request.GetIntParam("BandaRodagem"),
                    CodigoModelo = Request.GetIntParam("Modelo"),
                    DataEntradaInicio = Request.GetNullableDateTimeParam("DataEntradaInicio"),
                    DataEntradaLimite = Request.GetNullableDateTimeParam("DataEntradaLimite"),
                    DTO = Request.GetStringParam("DTO"),
                    NumeroFogo = Request.GetStringParam("NumeroFogo"),
                    TipoAquisicao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu>("TipoAquisicao"),
                    VidaAtual = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu>("VidaAtual"),
                    SituacaoCadastroPneu = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroPneu>("SituacaoPneu"),
                    NumeroNota = Request.GetIntParam("NumeroNota")
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número de Fogo", "Descricao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Frota", "Frota", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Estoque Atual", "EstoqueAtual", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("DOT", "DTO", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data de Entrada", "DataEntrada", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Marca", "MarcaPneu", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "Modelo", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Banda de Rodagem", "BandaRodagem", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo da banda", "TipoBandaRodagem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Almoxarifado", "Almoxarifado", 14, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.Pneu repositorio = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.Pneu> listaPneu = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaPneuRetornar = (
                    from pneu in listaPneu
                    select new
                    {
                        pneu.Codigo,
                        Frota = ObterNumeroFrota(pneu.VeiculoPneu.FirstOrDefault(), pneu.VeiculoEstepe.FirstOrDefault()),//pneu.VeiculoPneu.FirstOrDefault() != null ? $"{pneu.VeiculoPneu.FirstOrDefault().Veiculo.Placa} - {string.IsNullOrWhiteSpace(pneu.VeiculoPneu.FirstOrDefault().Veiculo.NumeroFrota)}" : pneu.VeiculoEstepe.FirstOrDefault() != null ? $"{pneu.VeiculoEstepe.FirstOrDefault().Veiculo.Placa} - {pneu.VeiculoEstepe.FirstOrDefault().Veiculo.NumeroFrota}" : string.Empty,
                        EstoqueAtual = pneu.Situacao == SituacaoPneu.Disponivel ? pneu.Almoxarifado.Descricao : pneu.Situacao == SituacaoPneu.Todos ? string.Empty : pneu.Situacao.ObterDescricao(),
                        MarcaPneu = pneu.Modelo?.Marca?.Descricao ?? string.Empty,
                        TipoBandaRodagem = pneu.BandaRodagem != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneuHelper.ObterDescricao(pneu.BandaRodagem.Tipo) : string.Empty,
                        Almoxarifado = pneu.Almoxarifado.Descricao,
                        BandaRodagem = pneu.BandaRodagem.Descricao,
                        DataEntrada = pneu.DataEntrada.ToString("dd/MM/yyyy"),
                        Descricao = pneu.NumeroFogo,
                        pneu.DTO,
                        Modelo = pneu.Modelo.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaPneuRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Almoxarifado")
                return "Almoxarifado.Descricao";

            if (propriedadeOrdenar == "BandaRodagem")
                return "BandaRodagem.Descricao";

            if (propriedadeOrdenar == "Descricao")
                return "NumeroFogo";

            if (propriedadeOrdenar == "Modelo")
                return "Modelo.Descricao";

            return propriedadeOrdenar;
        }

        private void AdicionarRetornoSucata(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Retorno da sucata.",
                Pneu = pneu,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPneuHistorico.RetornoSucada,
                BandaRodagem = pneu.BandaRodagem,
                KmAtualRodado = pneu.KmAtualRodado,
                DataHoraMovimentacao = DateTime.Now,
                Usuario = this.Usuario,
            };

            repositorioPneuHistorico.Inserir(pneuHistorico);

            Repositorio.Embarcador.Frota.PneuSucata repositorioPneuSucata = new Repositorio.Embarcador.Frota.PneuSucata(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuSucata pneuSucata = repositorioPneuSucata.BuscarPorPneu(pneu.Codigo);

            if (pneuSucata != null && pneuSucata.Motivo != null && pneuSucata.Motivo.TipoMovimento != null)
            {
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                servProcessoMovimento.GerarMovimentacao(out string erro, null, DateTime.Now.Date, pneuSucata.ValorCarcaca, pneu.NumeroFogo, "RETORNO DA SUCATA DO PNEU " + pneu.NumeroFogo + " " + pneuSucata.Observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, pneuSucata.Motivo.TipoMovimento.PlanoDeContaDebito, pneuSucata.Motivo.TipoMovimento.PlanoDeContaCredito);
            }

            repositorioPneuSucata.Deletar(pneuSucata);
        }

        private void AdicionarHistoricoTrocaAlmoxarifado(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            Repositorio.Embarcador.Frota.PneuHistorico repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.Entidades.Embarcador.Frota.PneuHistorico()
            {
                Data = DateTime.Now,
                Descricao = $"Retorno da sucada para o almoxarifado de {pneu.Almoxarifado?.Descricao}",
                Pneu = pneu,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPneuHistorico.RetornoSucada,
                BandaRodagem = pneu.BandaRodagem,
                KmAtualRodado = pneu.KmAtualRodado,
                DataHoraMovimentacao = DateTime.Now,
                Usuario = this.Usuario,
            };

            repositorioPneuHistorico.Inserir(pneuHistorico);
        }

        private void RetornoEstoqueProduto(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.Pneu pneu, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (pneu.Produto != null && pneu.DocumentoEntradaItem != null && pneu.DocumentoEntradaItem.NaturezaOperacao != null && pneu.DocumentoEntradaItem.NaturezaOperacao.ControlaEstoque)
            {
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

                servicoEstoque.MovimentarEstoque(out string erro, pneu.Produto, 1, Dominio.Enumeradores.TipoMovimento.Entrada, "PNEU", pneu.NumeroFogo, pneu.ValorCustoAtualizado, pneu.Empresa, DateTime.Now, TipoServicoMultisoftware);
            }
        }

        private string ObterNumeroFrota(Dominio.Entidades.VeiculoPneu veiculoPneu, Dominio.Entidades.VeiculoEstepe veiculoEstepe)
        {
            if (veiculoPneu != null)
            {
                return veiculoPneu.Veiculo.Placa + (!string.IsNullOrWhiteSpace(veiculoPneu.Veiculo.NumeroFrota) ? $" - {veiculoPneu.Veiculo.NumeroFrota}" : string.Empty);
            }
            else if (veiculoEstepe != null)
            {
                return veiculoEstepe.Veiculo.Placa + (!string.IsNullOrWhiteSpace(veiculoEstepe.Veiculo.NumeroFrota) ? $" - {veiculoEstepe.Veiculo.NumeroFrota}" : string.Empty);
            }

            return string.Empty;
        }

        private void GerarPneus(Repositorio.UnitOfWork unitOfWork, int codigoEmpresa, int indice, out string retorno, TipoLancamentoPneu tipoLancamentoPneu)
        {
            retorno = string.Empty;
            int ultimoRegistroPneu = 0;

            Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

            if (tipoLancamentoPneu == TipoLancamentoPneu.PorFaixa)
                ultimoRegistroPneu = indice;
            else
                ultimoRegistroPneu = repositorioPneu.BuscarUltimoNumeroFogoLock(codigoEmpresa);

            Dominio.Entidades.Embarcador.Frota.Pneu pneu = new Dominio.Entidades.Embarcador.Frota.Pneu();

            try
            {
                PreencherPneu(pneu, unitOfWork, tipoLancamentoPneu == TipoLancamentoPneu.PorFaixa ? ultimoRegistroPneu : ultimoRegistroPneu + 1);

                pneu.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu.Disponivel;
                pneu.ValorAquisicao = Request.GetDecimalParam("ValorAquisicao");
                pneu.ValorCustoAtualizado = pneu.ValorAquisicao;
                pneu.ValorCustoKmAtualizado = pneu.ValorAquisicao;
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                retorno = excecao.Message;
            }

            if (!string.IsNullOrWhiteSpace(pneu.NumeroFogo))
            {
                if (repositorioPneu.ContemPneuMesmoNumeroFogo(pneu.NumeroFogo, pneu.Codigo))
                {
                    unitOfWork.Rollback();
                    retorno = $"Já existe uma pneu cadastrado com o mesmo número de fogo: {pneu.NumeroFogo}";
                }
            }

            if (string.IsNullOrEmpty(retorno))
                repositorioPneu.Inserir(pneu, Auditado);
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoPneu(Repositorio.UnitOfWork unitOfWork)
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 200;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Almoxarifado", Propriedade = "Almoxarifado", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Banda de Rodagem", Propriedade = "BandaRodagem", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Calibragem", Propriedade = "Calibragem", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Custo Atualizado", Propriedade = "CustoAtualizado", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Custo KM Atualizado", Propriedade = "CustoKMAtualizado", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Data de Entrada", Propriedade = "Data", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Descrição da Nota", Propriedade = "DescricaoNota", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "DTO", Propriedade = "DTO", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = "KM Rodado Entre Sulcos", Propriedade = "KMEntreSulcos", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 10, Descricao = "KM Atual Rodado", Propriedade = "KMAtualRodado", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 11, Descricao = "Milimitragem 1", Propriedade = "Milimitragem1", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 12, Descricao = "Milimitragem 2", Propriedade = "Milimitragem2", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 13, Descricao = "Milimitragem 3", Propriedade = "Milimitragem3", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 14, Descricao = "Milimitragem 4", Propriedade = "Milimitragem4", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 15, Descricao = "Modelo", Propriedade = "Modelo", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 16, Descricao = "Num. de Fogo", Propriedade = "NumFogo", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 17, Descricao = "Produto", Propriedade = "Produto", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 18, Descricao = "Sulco Anterior", Propriedade = "SulcoAnterior", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 19, Descricao = "Sulco Atual", Propriedade = "SulcoAtual", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 20, Descricao = "Tipo Aquisição", Propriedade = "TipoAquisicao", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 21, Descricao = "Valor de Aquisição", Propriedade = "ValorAquisicao", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 22, Descricao = "Vida Atual", Propriedade = "VidaAtual", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        private void SalvarImportacaoPneu(Dominio.Entidades.Embarcador.Frota.Pneu pneuImportacao, Repositorio.UnitOfWork unitOfWork, bool atualizarRegistros = false)
        {
            Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

            Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorNumeroFogo(pneuImportacao.NumeroFogo);
            if (pneu == null)
            {
                repPneu.Inserir(pneuImportacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pneuImportacao, null, $"Pneu {pneuImportacao.NumeroFogo} inserido via planilha.", unitOfWork);

            }
            else if (atualizarRegistros)
            {
                pneu.Initialize();
                PreencherAlteracoes(pneu,pneuImportacao);
                repPneu.Atualizar(pneu);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pneu, pneu.GetChanges(), $"Pneu {pneu.NumeroFogo} alterado via planilha.", unitOfWork);
            }
            else
            {
                throw new ArgumentException($"Número de fogo {pneu.NumeroFogo} já cadastrado no sistema. Caso queira atualizar o registro, permita a atualização dos dados!");
            }
        }

        private void PreencherAlteracoes (Pneu pneu, Pneu pneuImportacao)
        {
            if (pneuImportacao == null || pneu == null)
            {
                return;
            }

            pneu.NumeroFogo = pneuImportacao.NumeroFogo;
            pneu.DTO = pneuImportacao.DTO;
            pneu.DataEntrada = pneuImportacao.DataEntrada;
            pneu.TipoAquisicao = pneuImportacao.TipoAquisicao;
            pneu.VidaAtual = pneuImportacao.VidaAtual;
            pneu.Modelo = pneuImportacao.Modelo;
            pneu.BandaRodagem = pneuImportacao.BandaRodagem;
            pneu.Almoxarifado = pneuImportacao.Almoxarifado;
            pneu.DescricaoNota = pneuImportacao.DescricaoNota;
            pneu.Produto = pneuImportacao.Produto;
            pneu.SulcoAnterior = pneuImportacao.SulcoAnterior;
            pneu.Sulco = pneuImportacao.Sulco;
            pneu.KmRodadoEntreSulcos = pneuImportacao.KmRodadoEntreSulcos;
            pneu.KmAtualRodado = pneuImportacao.KmAtualRodado;
            pneu.ValorAquisicao = pneuImportacao.ValorAquisicao;
            pneu.ValorCustoAtualizado = pneuImportacao.ValorCustoAtualizado;
            pneu.ValorCustoKmAtualizado = pneuImportacao.ValorCustoKmAtualizado;
            pneu.Calibragem = pneuImportacao.Calibragem;
            pneu.Milimitragem1 = pneuImportacao.Milimitragem1;
            pneu.Milimitragem2 = pneuImportacao.Milimitragem2;
            pneu.Milimitragem3 = pneuImportacao.Milimitragem3;
            pneu.Milimitragem4 = pneuImportacao.Milimitragem4;

        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = true };
            return retorno;
        }

        #endregion
    }
}
