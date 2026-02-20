using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cotacoes
{
    [CustomAuthorize("Cotacoes/CotacaoPedido")]
    public class CotacaoPedidoController : BaseController
    {
        #region Construtores

        public CotacaoPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return await Task.FromResult(ObterGridPesquisa(unitOfWork));
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
                return await Task.FromResult(ObterGridPesquisa(unitOfWork, true));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o arquivo em excel.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridConsultarAutorizacoes(unitOfWork);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao autorizacao = repCotacaoPedidoAutorizacao.BuscarAutorizacaoPedido(codigo);

                // Valida
                if (cotacaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    cotacaoPedido.Codigo,
                    cotacaoPedido.SituacaoPedido,
                    cotacaoPedido.DescricaoSituacaoPedido,
                    DescricaoEtapaPedido = cotacaoPedido.DescricaoSituacaoPedido,
                    cotacaoPedido.Numero,
                    Data = cotacaoPedido.Data.Value.ToString("dd/MM/yyyy"),
                    Usuario = new { Codigo = cotacaoPedido.Usuario?.Codigo ?? 0, Descricao = cotacaoPedido.Usuario?.Descricao ?? string.Empty },
                    Previsao = cotacaoPedido.Previsao.Value.ToString("dd/MM/yyyy hh:MM"),
                    cotacaoPedido.TipoClienteCotacaoPedido,
                    ClienteAtivo = new { Codigo = cotacaoPedido.ClienteAtivo?.Codigo ?? 0, Descricao = cotacaoPedido.ClienteAtivo?.Descricao ?? string.Empty },
                    ClienteInativo = new { Codigo = cotacaoPedido.ClienteInativo?.Codigo ?? 0, Descricao = cotacaoPedido.ClienteInativo?.Descricao ?? string.Empty },
                    cotacaoPedido.ClienteNovo,
                    ClienteProspect = new { Codigo = cotacaoPedido.ClienteProspect?.Codigo ?? 0, Descricao = cotacaoPedido.ClienteProspect?.Descricao ?? string.Empty },
                    GrupoPessoas = new { Codigo = cotacaoPedido.GrupoPessoas?.Codigo ?? 0, Descricao = cotacaoPedido.GrupoPessoas?.Descricao ?? string.Empty },
                    Origem = new { Codigo = cotacaoPedido.Origem?.Codigo ?? 0, Descricao = cotacaoPedido.Origem?.Descricao ?? string.Empty },
                    Destino = new { Codigo = cotacaoPedido.Destino?.Codigo ?? 0, Descricao = cotacaoPedido.Destino?.Descricao ?? string.Empty },
                    Destinatario = new { Codigo = cotacaoPedido.Destinatario?.Codigo ?? 0, Descricao = cotacaoPedido.Destinatario?.Descricao ?? string.Empty },
                    TipoDeCarga = new { Codigo = cotacaoPedido.TipoDeCarga?.Codigo ?? 0, Descricao = cotacaoPedido.TipoDeCarga?.Descricao ?? string.Empty },
                    TipoOperacao = new { Codigo = cotacaoPedido.TipoOperacao?.Codigo ?? 0, Descricao = cotacaoPedido.TipoOperacao?.Descricao ?? string.Empty },
                    Prospeccao = new { Codigo = cotacaoPedido.Prospeccao?.Codigo ?? 0, Descricao = cotacaoPedido.Prospeccao?.Descricao ?? string.Empty },
                    cotacaoPedido.Solicitante,
                    Produto = new { Codigo = cotacaoPedido.Produto?.Codigo ?? 0, Descricao = cotacaoPedido.Produto?.Descricao ?? string.Empty },
                    ModeloVeicularCarga = new { Codigo = cotacaoPedido.ModeloVeicularCarga?.Codigo ?? 0, Descricao = cotacaoPedido.ModeloVeicularCarga?.Descricao ?? string.Empty },
                    cotacaoPedido.EmailContato,
                    cotacaoPedido.TelefoneContato,
                    cotacaoPedido.StatusCotacaoPedido,
                    cotacaoPedido.TipoTomador,
                    cotacaoPedido.UsarTipoTomadorCotacaoPedido,
                    Tomador = new { Codigo = cotacaoPedido.Tomador?.CPF_CNPJ ?? 0, Descricao = cotacaoPedido.Tomador?.Descricao ?? "" },
                    Recebedor = new { Codigo = cotacaoPedido.Recebedor?.Codigo ?? 0, Descricao = cotacaoPedido.Recebedor?.Descricao ?? string.Empty },
                    Expedidor = new { Codigo = cotacaoPedido.Expedidor?.Codigo ?? 0, Descricao = cotacaoPedido.Expedidor?.Descricao ?? string.Empty },
                    AbaOrigem = new
                    {
                        cotacaoPedido.Codigo,
                        MudarEndereco = cotacaoPedido.UsarOutroEnderecoOrigem,
                        ClienteOutroEndereco = cotacaoPedido.UsarOutroEnderecoOrigem,
                        Localidade = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null ? new { Codigo = cotacaoPedido.EnderecoOrigem.Localidade.Codigo, Descricao = cotacaoPedido.EnderecoOrigem.Localidade.DescricaoCidadeEstado } : null,
                        Bairro = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.Bairro : "",
                        CEP = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.CEP : "",
                        Numero = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.Numero : "",
                        Complemento = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.Complemento : "",
                        Endereco = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.Endereco : "",
                        Telefone1 = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.Telefone : "",
                        RGIE = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.IE_RG : "",
                        IERG = cotacaoPedido.EnderecoOrigem != null ? cotacaoPedido.EnderecoOrigem.IE_RG : "",
                        LocalidadePolo = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null && cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo != null ? new { Codigo = cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo.Codigo : 0, Descricao = cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                        Pais = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null ? cotacaoPedido.EnderecoOrigem.Localidade.Pais != null ? cotacaoPedido.EnderecoOrigem.Localidade.Pais.Nome : "" : "",
                        CodigoIBGE = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null ? cotacaoPedido.EnderecoOrigem.Localidade.CodigoIBGE : 0,
                        UF = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null ? cotacaoPedido.EnderecoOrigem.Localidade.Estado.Sigla : "",
                        Origem = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null && cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo != null ? new { cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo.Codigo, Descricao = cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo.DescricaoCidadeEstado } : null,
                        CidadePoloOrigem = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null && cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo != null ? new { Codigo = cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo.Codigo : 0, Descricao = cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoOrigem.Localidade.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                        PaisOrigem = cotacaoPedido.EnderecoOrigem != null && cotacaoPedido.EnderecoOrigem.Localidade != null ? cotacaoPedido.EnderecoOrigem.Localidade.Pais != null ? cotacaoPedido.EnderecoOrigem.Localidade.Pais.Nome : "" : ""
                    },
                    AbaDestino = new
                    {
                        cotacaoPedido.Codigo,
                        MudarEndereco = cotacaoPedido.UsarOutroEnderecoDestino,
                        ClienteOutroEndereco = cotacaoPedido.UsarOutroEnderecoDestino,
                        Localidade = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null ? new { Codigo = cotacaoPedido.EnderecoDestino.Localidade.Codigo, Descricao = cotacaoPedido.EnderecoDestino.Localidade.DescricaoCidadeEstado } : null,
                        Bairro = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.Bairro : "",
                        CEP = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.CEP : "",
                        Numero = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.Numero : "",
                        Complemento = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.Complemento : "",
                        Endereco = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.Endereco : "",
                        Telefone1 = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.Telefone : "",
                        RGIE = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.IE_RG : "",
                        IERG = cotacaoPedido.EnderecoDestino != null ? cotacaoPedido.EnderecoDestino.IE_RG : "",
                        LocalidadePolo = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null && cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo != null ? new { Codigo = cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo.Codigo : 0, Descricao = cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                        Pais = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null ? cotacaoPedido.EnderecoDestino.Localidade.Pais != null ? cotacaoPedido.EnderecoDestino.Localidade.Pais.Nome : "" : "",
                        CodigoIBGE = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null ? cotacaoPedido.EnderecoDestino.Localidade.CodigoIBGE : 0,
                        UF = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null ? cotacaoPedido.EnderecoDestino.Localidade.Estado.Sigla : "",
                        Destino = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null && cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo != null ? new { cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo.Codigo, Descricao = cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo.DescricaoCidadeEstado } : null,
                        CidadePoloDestino = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null && cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo != null ? new { Codigo = cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo.Codigo : 0, Descricao = cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo != null ? cotacaoPedido.EnderecoDestino.Localidade.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                        PaisDestino = cotacaoPedido.EnderecoDestino != null && cotacaoPedido.EnderecoDestino.Localidade != null ? cotacaoPedido.EnderecoDestino.Localidade.Pais != null ? cotacaoPedido.EnderecoDestino.Localidade.Pais.Nome : "" : ""
                    },
                    AbaAdicional = new
                    {
                        cotacaoPedido.Codigo,
                        DataInicialColeta = cotacaoPedido.DataInicialColeta.HasValue ? cotacaoPedido.DataInicialColeta.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        DataFinalColeta = cotacaoPedido.DataFinalColeta.HasValue ? cotacaoPedido.DataFinalColeta.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        cotacaoPedido.TipoModal,
                        cotacaoPedido.NumeroPaletes,
                        cotacaoPedido.PesoTotal,
                        cotacaoPedido.ValorTotalNotasFiscais,
                        cotacaoPedido.QuantidadeNotas,
                        cotacaoPedido.QtdEntregas,
                        cotacaoPedido.Temperatura,
                        cotacaoPedido.KMTotal,
                        cotacaoPedido.ValorPorKM,
                        cotacaoPedido.Observacao,
                        cotacaoPedido.Rastreado,
                        cotacaoPedido.GerenciamentoRisco,
                        cotacaoPedido.EscoltaArmada,
                        cotacaoPedido.QtdEscoltas,
                        cotacaoPedido.Ajudante,
                        cotacaoPedido.QtdAjudantes,
                        cotacaoPedido.TipoPagamento,
                        cotacaoPedido.ObservacaoInterna,
                        Cubagens = (from obj in cotacaoPedido.Cubagens
                                    select new
                                    {
                                        obj.Codigo,
                                        Altura = obj.Altura.ToString("n2"),
                                        Comprimento = obj.Comprimento.ToString("n2"),
                                        Largura = obj.Largura.ToString("n2"),
                                        QtdVolume = obj.QtdVolume.ToString("n0"),
                                        MetroCubico = obj.MetroCubico.ToString("n3"),
                                        FatorCubico = obj.FatorCubico.ToString("n2"),
                                        PesoCubado = obj.PesoCubado.ToString("n3")
                                    }).ToList(),
                        cotacaoPedido.CubagemTotal,
                        TotalPesoCubado = cotacaoPedido.PesoCubado,
                        cotacaoPedido.QtVolumes
                    },
                    AbaPercurso = new
                    {
                        cotacaoPedido.Codigo
                    },
                    AbaValor = new
                    {
                        cotacaoPedido.Codigo,
                        Componentes = (from obj in cotacaoPedido.Componentes
                                       select new
                                       {
                                           obj.Codigo,
                                           CodigoComponente = obj.ComponenteFrete?.Codigo ?? 0,
                                           DescricaoComponente = obj.ComponenteFrete?.Descricao ?? string.Empty,
                                           Valor = obj.Valor.ToString("n2"),
                                           Percentual = obj.Percentual.ToString("n2"),
                                           PercentualAcrescimo = obj.PercentualAcrescimo.ToString("n2"),
                                           PercentualDesconto = obj.PercentualDesconto.ToString("n2"),
                                           ValorTotal = obj.ValorTotal.ToString("n2")
                                       }).ToList(),
                        cotacaoPedido.ValorCotacao,
                        PercentualAcrescimoValorCotacao = cotacaoPedido.PercentualAcrescimo,
                        PercentualDescontoValorCotacao = cotacaoPedido.PercentualDesconto,
                        cotacaoPedido.ValorTotalCotacao,
                        cotacaoPedido.ValorFrete,
                        cotacaoPedido.ValorICMS,
                        cotacaoPedido.ValorTotalCotacaoComICMS,
                        cotacaoPedido.AliquotaICMS,
                        cotacaoPedido.IncluirValorICMSBaseCalculo,
                        cotacaoPedido.ValorFreteTerceiro,
                    },
                    AbaImportacao = new
                    {
                        cotacaoPedido.Codigo,
                        cotacaoPedido.NumeroContainer,
                        cotacaoPedido.NumeroBL,
                        cotacaoPedido.NumeroNavio,
                        Porto = new { Codigo = cotacaoPedido.Porto?.Codigo ?? 0, Descricao = cotacaoPedido.Porto?.Descricao ?? string.Empty },
                        TipoTerminalImportacao = new { Codigo = cotacaoPedido.TipoTerminalImportacao?.Codigo ?? 0, Descricao = cotacaoPedido.TipoTerminalImportacao?.Descricao ?? string.Empty },
                        cotacaoPedido.EnderecoEntregaImportacao,
                        cotacaoPedido.BairroEntregaImportacao,
                        cotacaoPedido.CEPEntregaImportacao,
                        LocalidadeEntregaImportacao = new { Codigo = cotacaoPedido.LocalidadeEntregaImportacao?.Codigo ?? 0, Descricao = cotacaoPedido.LocalidadeEntregaImportacao?.Descricao ?? string.Empty },
                        DataVencimentoArmazenamentoImportacao = cotacaoPedido.DataVencimentoArmazenamentoImportacao.HasValue ? cotacaoPedido.DataVencimentoArmazenamentoImportacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                        cotacaoPedido.ArmadorImportacao,
                        GridDI = (from obj in cotacaoPedido.CotacaoPedidoImportacao
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.CodigoImportacao,
                                      obj.CodigoReferencia,
                                      obj.NumeroDI,
                                      Peso = obj.Peso.ToString("n2"),
                                      ValorCarga = obj.ValorCarga.ToString("n2"),
                                      Volume = obj.Volume.ToString("n2")
                                  }).ToList()
                    },
                    DadosAutorizacao = new
                    {
                        cotacaoPedido.Codigo,
                        SituacaoSolicitacao = SituacaoSolicitacao(cotacaoPedido),
                        DescricaoSituacao = cotacaoPedido.DescricaoSituacaoPedido,
                        DataEmissao = cotacaoPedido.Data.HasValue ? cotacaoPedido.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                        Solicitado = cotacaoPedido.Usuario?.Nome,
                        Peso = cotacaoPedido.PesoTotal.ToString("n2"),
                        QtdEntregas = cotacaoPedido.QtdEntregas.ToString("n0"),
                        TipoCarga = cotacaoPedido.TipoDeCarga?.Descricao ?? string.Empty,
                        ModeloVeicular = cotacaoPedido.ModeloVeicularCarga?.Descricao ?? string.Empty,
                        Cliente = cotacaoPedido.ClienteCotacaoPedido,
                        ValorFrete = cotacaoPedido.ValorTotalCotacao.ToString("n2"),
                        Observacao = cotacaoPedido.Observacao,
                        DataRetorno = autorizacao?.Data != null ? autorizacao?.Data.ToString("dd/MM/yyyy") ?? string.Empty : string.Empty,
                        Creditor = autorizacao?.Usuario?.Nome ?? string.Empty,
                        ValorLiberado = autorizacao != null && autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? cotacaoPedido.ValorTotalCotacao.ToString("n2") : string.Empty,
                        RetornoSolicitacao = autorizacao?.Motivo ?? string.Empty,
                        ComRegraAutorizacao = autorizacao != null
                    },
                    Resumo = new
                    {
                        cotacaoPedido.Codigo,
                        NumeroCotacaoPedido = cotacaoPedido.Numero.ToString("n0"),
                        TipoOperacao = cotacaoPedido.TipoOperacao?.Descricao ?? string.Empty,
                        Etapa = cotacaoPedido.DescricaoTipoClienteCotacaoPedido,
                        Situacao = cotacaoPedido.DescricaoSituacaoPedido,
                        Cliente = cotacaoPedido.ClienteCotacaoPedido,
                        DataPrevisao = cotacaoPedido.Previsao.Value.ToString("dd/MM/yyyy HH:mm"),
                        Status = cotacaoPedido.DescricaoStatusCotacaoPedido
                    },
                    ConfiguracoesTipoOperacao = new
                    {
                        HabilitaInformarDadosDosPedidosNaContacao = cotacaoPedido.TipoOperacao?.ConfiguracaoCotacaoPedido?.HabilitaInformarDadosDosPedidosNaCotacao ?? null
                    }

                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido();

                // Preenche entidade com dados
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                cotacaoPedido.Numero = repCotacaoPedido.BuscarProximoNumero(codigoEmpresa);
                cotacaoPedido.Usuario = this.Usuario;

                PreencheEntidade(ref cotacaoPedido, unitOfWork, true);

                // Valida entidade
                if (!ValidaEntidade(cotacaoPedido, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCotacaoPedido.Inserir(cotacaoPedido, Auditado);
                SalvarListasEntidades(cotacaoPedido, unitOfWork, true);

                if (cotacaoPedido.StatusCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Fechada)
                {
                    if (VerificarRegrasCotacaoPedido(cotacaoPedido, TipoServicoMultisoftware, unitOfWork))
                    {
                        Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarSituacaoCotacaoPedido(repCotacaoPedido.BuscarPorCodigo(cotacaoPedido.Codigo), unitOfWork, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);
                    }
                    else
                    {
                        cotacaoPedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                    }
                }
                repCotacaoPedido.Atualizar(cotacaoPedido);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(codigo);

                // Valida
                if (cotacaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref cotacaoPedido, unitOfWork, false);
                SalvarListasEntidades(cotacaoPedido, unitOfWork, false);

                // Valida entidade
                if (!ValidaEntidade(cotacaoPedido, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCotacaoPedido.Atualizar(cotacaoPedido);

                if (cotacaoPedido.StatusCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Fechada)
                {
                    if (VerificarRegrasCotacaoPedido(cotacaoPedido, TipoServicoMultisoftware, unitOfWork))
                    {
                        Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarSituacaoCotacaoPedido(repCotacaoPedido.BuscarPorCodigo(cotacaoPedido.Codigo), unitOfWork, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);
                    }
                    else
                    {
                        cotacaoPedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                    }
                }
                repCotacaoPedido.Atualizar(cotacaoPedido);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(codigo);

                // Valida
                if (cotacaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repCotacaoPedido.Deletar(cotacaoPedido, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosCotacaoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
                Dominio.Entidades.Usuario usuarioLogado = this.Usuario;
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                var dadosUsuarioLogado = new
                {
                    usuarioLogado.Codigo,
                    usuarioLogado.Nome,
                    ProximoNumero = repCotacaoPedido.BuscarProximoNumero(codigoEmpresa)
                };

                return new JsonpResult(dadosUsuarioLogado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosMapa()
        {

            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cnpjClienteAtivo = Request.GetDoubleParam("CNPJClienteAtivo");
                double cnpjClienteInativo = Request.GetDoubleParam("CNPClienteInativo");
                double cnpjDestinatario = Request.GetDoubleParam("CNPJDestinatario");
                int codigoLocalidadeDestino = Request.GetIntParam("CodigoLocalidadeDestino");
                int codigoLocalidadeOrigem = Request.GetIntParam("CodigoLocalidadeOrigem");

                Dominio.Entidades.Cliente clienteOrigem = null;
                Dominio.Entidades.Cliente clienteDestino = null;

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                if (cnpjClienteAtivo > 0)
                    clienteOrigem = repCliente.BuscarPorCPFCNPJ(cnpjClienteAtivo);
                else if (cnpjClienteInativo > 0)
                    clienteOrigem = repCliente.BuscarPorCPFCNPJ(cnpjClienteInativo);
                if (cnpjDestinatario > 0)
                    clienteDestino = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);

                var retorno = (
                    new
                    {
                        PolilinhaPrevista = string.Empty,
                        PolilinhaRealizada = string.Empty,
                        PontosPrevistos = string.Empty,
                        Latitude = clienteOrigem != null && !string.IsNullOrWhiteSpace(clienteOrigem.Latitude) ? clienteOrigem.Latitude.Replace(".", ",") : "",
                        Longitude = clienteOrigem != null && !string.IsNullOrWhiteSpace(clienteOrigem.Longitude) ? clienteOrigem.Longitude.Replace(".", ",") : "",
                        LatitudeDestino = clienteDestino != null && !string.IsNullOrWhiteSpace(clienteDestino.Latitude) ? clienteDestino.Latitude.Replace(".", ",") : "",
                        LongitudeDestino = clienteDestino != null && !string.IsNullOrWhiteSpace(clienteDestino.Longitude) ? clienteDestino.Longitude.Replace(".", ",") : "",
                        PlacaVeiculo = string.Empty,
                        Descricao = string.Empty
                    }
                );


                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter dados do mapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosRotaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrigem = Request.GetIntParam("Origem");
                int codigoDestino = Request.GetIntParam("Destino");
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Dominio.Entidades.Localidade origem = repositorioLocalidade.BuscarPorCodigo(codigoOrigem) ?? throw new ControllerException("Não foi possível encontrar a origem.");
                Dominio.Entidades.Localidade destino = repositorioLocalidade.BuscarPorCodigo(codigoDestino) ?? throw new ControllerException("Não foi possível encontrar o destino.");
                Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorOrigemEDestino(origem, destino).FirstOrDefault();

                return new JsonpResult(new
                {
                    Quilometros = rotaFrete?.Quilometros.ToString("n0") ?? ""
                });
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados da rota de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R181_CotacaoPedido, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R181_CotacaoPedido, TipoServicoMultisoftware, "Relatório de Cotação Pedido", "Cotacoes", "CotacaoPedido.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Cotacoes.CotacaoPedido> dadosCotacaoPedido = repCotacaoPedido.RelatorioCotacaoPedido(codigo);
                if (dadosCotacaoPedido.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioCotacaoPedido(codigo, dadosCotacaoPedido, stringConexao, relatorioControleGeracao));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de cotação de pedido para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarValoresTabelafrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Parametros
                DateTime? dataColeta = Request.GetNullableDateTimeParam("DataColeta");
                DateTime? dataFinalViagem = Request.GetNullableDateTimeParam("DataFinalViagem");
                DateTime? dataInicialViagem = Request.GetNullableDateTimeParam("DataInicialViagem");
                DateTime dataVigencia = Request.GetDateTimeParam("DataVigencia");

                double cnpjClienteAtivo = Request.GetDoubleParam("CNPJClienteAtivo");
                double cnpjClienteInativo = Request.GetDoubleParam("CNPClienteInativo");
                double cnpjDestinatario = Request.GetDoubleParam("CNPJDestinatario");

                int codigoLocalidadeDestino = Request.GetIntParam("CodigoLocalidadeDestino");
                int codigoEmpresa = 0;
                int codigoLocalidadeOrigem = Request.GetIntParam("CodigoLocalidadeOrigem");
                int codigoGrupoPessoa = Request.GetIntParam("CodigoGrupoPessoa");
                int codigoModeloVeicular = Request.GetIntParam("CodigoModeloVeicular");
                int numeroEntregas = Request.GetIntParam("NumeroEntregas");
                int numeroPedidos = Request.GetIntParam("NumeroPedidos");
                int quantidadeNotasFiscais = Request.GetIntParam("QuantidadeNotasFiscais");
                int codigoTipoDeCarga = Request.GetIntParam("CodigoTipoDeCarga");
                int codigoTipoOperacao = Request.GetIntParam("CodigoTipoOperacao");

                decimal distancia = Request.GetDecimalParam("Distancia");
                decimal numeroAjudantes = Request.GetDecimalParam("NumeroAjudantes");
                decimal numeroDeslocamento = Request.GetDecimalParam("NumeroDeslocamento");
                decimal numeroDiarias = Request.GetDecimalParam("NumeroDiarias");
                decimal numeroPallets = Request.GetDecimalParam("NumeroPallets");
                decimal peso = Request.GetDecimalParam("Peso");
                decimal pesoCubado = Request.GetDecimalParam("PesoCubado");
                decimal valorNotasFiscais = Request.GetDecimalParam("ValorNotasFiscais");
                decimal volumes = Request.GetDecimalParam("Volumes");
                decimal pesoTotal = Request.GetDecimalParam("PesoTotal");

                bool escoltaArmada = Request.GetBoolParam("EscoltaArmada");
                bool gerenciamentoRisco = Request.GetBoolParam("GerenciamentoRisco");
                bool rastreado = Request.GetBoolParam("Rastreado");
                bool pagamentoTerceiro = Request.GetBoolParam("PagamentoTerceiro");

                if ((cnpjClienteAtivo == 0d) && (cnpjClienteInativo == 0d))
                    codigoLocalidadeOrigem = Request.GetIntParam("Origem");

                if (cnpjDestinatario == 0d)
                    codigoLocalidadeDestino = Request.GetIntParam("Destino");

                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = Servicos.Embarcador.Carga.Frete.CalcularFretePorCotacaoPedido(unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware,
                    dataColeta, dataFinalViagem, dataInicialViagem, dataVigencia, cnpjClienteAtivo, cnpjClienteInativo, codigoLocalidadeDestino, codigoEmpresa,
                    codigoLocalidadeOrigem, codigoGrupoPessoa, codigoModeloVeicular,
                    distancia, escoltaArmada, gerenciamentoRisco, numeroAjudantes, numeroEntregas, numeroDeslocamento, numeroDiarias, numeroPallets, numeroPedidos,
                    peso, pesoCubado, quantidadeNotasFiscais, rastreado, cnpjDestinatario, codigoTipoDeCarga, codigoTipoOperacao,
                    valorNotasFiscais, volumes, pesoTotal, ConfiguracaoEmbarcador, pagamentoTerceiro);

                dynamic retorno = null;
                if (dadosCalculo?.Componentes != null)
                {
                    retorno = new
                    {
                        Componentes = (
                                from obj in dadosCalculo.Componentes
                                select new
                                {
                                    Codigo = Guid.NewGuid().ToString(),
                                    CodigoComponente = obj.ComponenteFrete?.Codigo ?? 0,
                                    DescricaoComponente = obj.ComponenteFrete?.Descricao ?? string.Empty,
                                    Valor = obj.ValorComponente.ToString("n2"),
                                    Percentual = obj.Percentual.ToString("n2"),
                                    PercentualAcrescimo = 0.ToString("n2"),
                                    PercentualDesconto = 0.ToString("n2"),
                                    ValorTotal = obj.ValorComponente.ToString("n2")
                                }
                            ).ToList(),
                        ValorFrete = dadosCalculo.ValorFrete.ToString("n2"),
                        ValorComponentes = (from obj in dadosCalculo.Componentes where obj.SomarComponenteFreteLiquido || obj.DescontarComponenteFreteLiquido select obj.DescontarComponenteFreteLiquido ? obj.ValorComponente * -1 : obj.ValorComponente).Sum(),
                        dadosCalculo.FreteCalculado,
                        dadosCalculo.MensagemRetorno
                    };
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a tabela de frete, verifique os parâmetros informados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarAliquotaICMS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                double cnpjClienteAtivo = Request.GetDoubleParam("ClienteAtivo");
                double cnpjClienteInativo = Request.GetDoubleParam("ClienteInativo");
                double cnpjDestinatario = Request.GetDoubleParam("Destinatario");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int codigoTipoCarga = Request.GetIntParam("TipoDeCarga");
                var TipoTomador = Request.GetEnumParam<Dominio.Enumeradores.TipoTomador>("TipoTomador");
                double cnpjTomador = Request.GetDoubleParam("Tomador");
                dynamic dynAbaOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaOrigem"));
                dynamic dynAbaDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaDestino"));

                Dominio.Entidades.Cliente remetente = null;
                Dominio.Entidades.Cliente destinatario = null;
                Dominio.Entidades.Cliente tomador = null;

                Dominio.Entidades.Localidade origem = null;
                Dominio.Entidades.Localidade destino = null;

                if (cnpjClienteAtivo > 0)
                    remetente = repCliente.BuscarPorCPFCNPJ(cnpjClienteAtivo);
                else if (cnpjClienteInativo > 0)
                    remetente = repCliente.BuscarPorCPFCNPJ(cnpjClienteInativo);

                if (cnpjDestinatario > 0)
                    destinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);

                if (cnpjTomador > 0)
                    tomador = repCliente.BuscarPorCPFCNPJ(cnpjTomador);

                if (remetente == null || ( destinatario == null && TipoTomador != Dominio.Enumeradores.TipoTomador.Remetente))
                {
                    var retorno = new
                    {
                        AliquotaICMS = "0,00"
                    };
                    return new JsonpResult(retorno);
                }
                else
                {
                    int codigoLocalidadeDestino = 0;
                    int.TryParse((string)dynAbaDestino.Localidade, out codigoLocalidadeDestino);
                    if (codigoLocalidadeDestino == 0)
                        codigoLocalidadeDestino = Request.GetIntParam("Destino");
                    if (codigoLocalidadeDestino == 0)
                        codigoLocalidadeDestino = destinatario?.Localidade?.Codigo ?? 0;

                    destino = repLocalidade.BuscarPorCodigo(codigoLocalidadeDestino);

                    if (TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        tomador = remetente;
                    else if (TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        tomador = destinatario;
                    else if (TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        tomador = remetente;
                    else if (TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        tomador = destinatario;

                    if (configuracaoEmbarcador.ValidarICMSTelaCotacaoPedidosRegraICMS)
                    {
                        int codigoLocalidadeOrigem = 0;
                        int.TryParse((string)dynAbaOrigem.Localidade, out codigoLocalidadeOrigem);
                        if (codigoLocalidadeOrigem == 0)
                            codigoLocalidadeOrigem = Request.GetIntParam("Origem");
                        if (codigoLocalidadeOrigem == 0)
                            codigoLocalidadeOrigem = remetente.Localidade?.Codigo ?? 0;

                        origem = repLocalidade.BuscarPorCodigo(codigoLocalidadeOrigem);

                        Dominio.Entidades.Cliente destinatarioExportacao = null;
                        if (destinatario?.Localidade != null && destinatario?.Localidade.Estado.Sigla == "EX")
                            destinatarioExportacao = destinatario;

                        Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.ObterRegraICMSCotacaoPedido(Empresa, remetente, destinatario, tomador, origem, destino, null, unitOfWork, true, codigoTipoOperacao, destinatarioExportacao, codigoTipoCarga);

                        var retornoRegraIcms = new
                        {
                            AliquotaICMS = regraICMS != null && regraICMS.Aliquota > 0 && regraICMS.Aliquota.HasValue ? regraICMS.Aliquota.Value.ToString("n2") : "0,00"
                        };
                        return new JsonpResult(retornoRegraIcms);
                    }
                    else
                    {
                        Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
                        Dominio.Entidades.Aliquota aliquota = serICMS.ObterAliquota(remetente.Localidade.Estado, remetente.Localidade.Estado, destinatario?.Localidade?.Estado ?? destino?.Estado, remetente.Atividade, destinatario?.Atividade ?? tomador.Atividade, unitOfWork);

                        var retornoAliquotaICMS = new
                        {
                            AliquotaICMS = aliquota != null && aliquota.Percentual > 0 ? aliquota.Percentual.ToString("n2") : "0,00"
                        };

                        return new JsonpResult(retornoAliquotaICMS);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a alíquota de ICMS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CriarPedidoFracionado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                // Parametros
                var parametros = ObterParametrosPedidoFracionado();
                if (!ValidarParametrosPedidoFracionado(parametros, out var mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);
                
                // Busca informacoes
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(parametros.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCotacaoQueNaoEstejaCanceladaOuRejeitada(cotacaoPedido.Codigo);
                if (cotacaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Valida se a cotação respeita as configurações do modelo veicular
                if (!ValidarModeloVeicular(cotacaoPedido, parametros, out mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                // Valida pedidos já criados a partir da cotação e suas quantidades
                if (!ValidarPedidosExistentes(cotacaoPedido, pedidos, parametros, out mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                if (cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado)
                {
                    var retorno = Servicos.Embarcador.CotacaoPedido.CotacaoPedido.CriarPedido(
                        cotacaoPedido, unitOfWork, this.Usuario, TipoServicoMultisoftware,
                        _conexao.StringConexao, Auditado, parametros.PesoLiquido,
                        parametros.NumeroPaletes, parametros.Cubagem, parametros.Unidades,
                        parametros.DataColeta);

                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new Exception(retorno);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDetalhesPedidosFracionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid gridDetalhes = GridDetalhesPedidoFracionado();

                // Ordenacao da grid
                //var propOrdenar = gridDetalhes.header[gridDetalhes.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistrosDetalhes = 4;
                var listaDetalhes = ExecutaBuscaDetalhesPedidoFracionado(ref totalRegistrosDetalhes, ref gridDetalhes, unitOfWork);

                // Seta valores na grid
                gridDetalhes.AdicionaRows(listaDetalhes);
                gridDetalhes.setarQuantidadeTotal(totalRegistrosDetalhes);

                return new JsonpResult(gridDetalhes);
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
        public async Task<IActionResult> BuscarHistoricoPedidosFracionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid gridHistoricoPedidos = GridHistoricoPedidosFracionados();

                // Ordenacao da grid
                //var propOrdenar = gridHistoricoPedidos.header[gridHistoricoPedidos.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistrosHistoricoPedidos = 0;
                var listaHistoricoPedidos = ExecutaBuscaHistoricoPedidosFracionados(ref totalRegistrosHistoricoPedidos, ref gridHistoricoPedidos, unitOfWork);

                // Seta valores na grid
                gridHistoricoPedidos.AdicionaRows(listaHistoricoPedidos);
                gridHistoricoPedidos.setarQuantidadeTotal(totalRegistrosHistoricoPedidos);

                return new JsonpResult(gridHistoricoPedidos);
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

        #region Métodos Globais - Importações

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Pedido.Cotacao servicoCotacao = new Servicos.Embarcador.Pedido.Cotacao(unitOfWork);

                string dados = Request.GetStringParam("Dados");
                var parametros = JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Parametro"));

                (string Nome, string Guid) arquivoGerador = ValueTuple.Create(Request.GetStringParam("Nome") ?? string.Empty, Request.GetStringParam("ArquivoSalvoComo") ?? string.Empty);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoCotacao.ImportarCotacao(dados, arquivoGerador, this.Usuario, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Pedido.Cotacao servicoCotacao = new Servicos.Embarcador.Pedido.Cotacao(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoCotacao.ConfiguracaoImportacaoCotacao(unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(configuracoes.ToList());
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridConsultarAutorizacoes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido tepCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

            int codOcorrencia = int.Parse(Request.Params("Codigo"));

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Regra", false);
            grid.AdicionarCabecalho("Data", false);
            grid.AdicionarCabecalho("Motivo", false);
            grid.AdicionarCabecalho("Justificativa", false);
            grid.AdicionarCabecalho("DT_RowColor", false);
            grid.AdicionarCabecalho("DT_FontColor", false);

            string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> listaCotacaoPedidoAutorizacao = repCotacaoPedidoAutorizacao.ConsultarAutorizacoesPorPedido(codOcorrencia, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(repCotacaoPedidoAutorizacao.ContarConsultarAutorizacoesPorPedido(codOcorrencia));

            var lista = (from obj in listaCotacaoPedidoAutorizacao
                         select new
                         {
                             obj.Codigo,
                             Situacao = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? "Pendente" : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? "Aprovada" : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? "Rejeitada" : string.Empty,
                             Usuario = obj.Usuario?.Nome,
                             Regra = obj.RegrasCotacaoPedido?.Descricao ?? string.Empty,
                             Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                             Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                             Justificativa = obj.Motivo,
                             DT_RowColor = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : "",
                             DT_FontColor = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : ""
                         }).ToList();
            grid.AdicionaRows(lista);

            return grid;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("TipoClienteCotacaoPedido").Nome("Tipo Cliente").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("ClienteCotacaoPedido").Nome("Cliente").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Origem").Nome("Origem").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Destinatario").Nome("Destinatário").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Previsao").Nome("Previsão").Tamanho(12).Align(Models.Grid.Align.left);
            grid.Prop("DescricaoSituacaoPedido").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("ClienteProspect"), out int clienteProspect);
            int.TryParse(Request.Params("GrupoPessoas"), out int grupoPessoas);
            int.TryParse(Request.Params("TipoDeCarga"), out int tipoDeCarga);
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);

            double.TryParse(Request.Params("ClienteAtivo"), out double clienteAtivo);
            double.TryParse(Request.Params("ClienteInativo"), out double clienteInativo);
            double.TryParse(Request.Params("Destinatario"), out double destinatario);

            DateTime.TryParse(Request.Params("DataPrevista"), out DateTime dataPrevista);

            string clienteNovo = Request.Params("ClienteNovo");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido tipoClienteCotacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.Todos;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido statusCotacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Todos;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Todos;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos;
            Enum.TryParse(Request.Params("TipoCliente"), out tipoClienteCotacaoPedido);
            Enum.TryParse(Request.Params("Modal"), out tipoModal);
            Enum.TryParse(Request.Params("StatusCotacaoPedido"), out statusCotacaoPedido);
            Enum.TryParse(Request.Params("Situacao"), out situacaoPedido);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> listaGrid = repCotacaoPedido.Consultar(codigoEmpresa, numero, clienteProspect, grupoPessoas, tipoDeCarga, tipoOperacao, clienteAtivo, clienteInativo, destinatario, dataPrevista, tipoClienteCotacaoPedido, tipoModal, statusCotacaoPedido, situacaoPedido, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCotacaoPedido.ContarConsulta(codigoEmpresa, numero, clienteProspect, grupoPessoas, tipoDeCarga, tipoOperacao, clienteAtivo, clienteInativo, destinatario, dataPrevista, tipoClienteCotacaoPedido, tipoModal, statusCotacaoPedido, situacaoPedido);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            TipoClienteCotacaoPedido = obj.DescricaoTipoClienteCotacaoPedido,
                            obj.ClienteCotacaoPedido,
                            Origem = obj.Origem?.DescricaoCidadeEstado ?? string.Empty,
                            Destinatario = obj.Destinatario?.Descricao ?? string.Empty,
                            Previsao = obj.Previsao.Value.ToString("dd/MM/yyyy HH:mm"),
                            obj.DescricaoSituacaoPedido
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                codigoEmpresa = this.Usuario.Empresa.Codigo;
                cotacaoPedido.Empresa = this.Usuario.Empresa;
            }

            cotacaoPedido.Data = Request.GetDateTimeParam("Data");
            cotacaoPedido.Previsao = Request.GetDateTimeParam("Previsao");
            cotacaoPedido.UltimaAtualizacao = DateTime.Now;

            cotacaoPedido.TipoClienteCotacaoPedido = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido>("TipoClienteCotacaoPedido");
            cotacaoPedido.StatusCotacaoPedido = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido>("StatusCotacaoPedido");
            if (cotacaoPedido.StatusCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Fechada)
                cotacaoPedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao;
            else if (cotacaoPedido.StatusCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.EmAnalise || cotacaoPedido.StatusCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Sondagem)
                cotacaoPedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
            else
                cotacaoPedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;

            cotacaoPedido.ClienteAtivo = Request.GetLongParam("ClienteAtivo") > 0 ? repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("ClienteAtivo")) : null;
            cotacaoPedido.ClienteInativo = Request.GetLongParam("ClienteInativo") > 0 ? repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("ClienteInativo")) : null;
            cotacaoPedido.ClienteProspect = repClienteProspect.BuscarPorCodigo(Request.GetIntParam("ClienteProspect"), codigoEmpresa);
            cotacaoPedido.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(Request.GetIntParam("GrupoPessoas"));
            cotacaoPedido.Origem = repLocalidade.BuscarPorCodigo(Request.GetIntParam("Origem"));
            cotacaoPedido.Destino = repLocalidade.BuscarPorCodigo(Request.GetIntParam("Destino"));
            cotacaoPedido.Destinatario = Request.GetLongParam("Destinatario") > 0 ? repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("Destinatario")) : null;
            cotacaoPedido.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(Request.GetIntParam("TipoDeCarga"));
            cotacaoPedido.TipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"));
            cotacaoPedido.Prospeccao = repProspeccao.BuscarPorCodigo(Request.GetIntParam("Prospeccao"));
            cotacaoPedido.Produto = repProdutoEmbarcador.BuscarPorCodigo(Request.GetIntParam("Produto"));
            cotacaoPedido.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeicularCarga"));
            cotacaoPedido.Recebedor = Request.GetLongParam("Recebedor") > 0 ? repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("Recebedor")) : null;
            cotacaoPedido.Expedidor = Request.GetLongParam("Expedidor") > 0 ? repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("Expedidor")) : null;

            cotacaoPedido.ClienteNovo = Request.GetStringParam("ClienteNovo");
            cotacaoPedido.Solicitante = Request.GetStringParam("Solicitante");
            cotacaoPedido.EmailContato = Request.GetStringParam("EmailContato");
            cotacaoPedido.TelefoneContato = Request.GetStringParam("TelefoneContato");

            double.TryParse(Request.Params("Tomador"), out double tomador);
            if (tomador > 0d)
                cotacaoPedido.Tomador = repCliente.BuscarPorCPFCNPJ(tomador);
            else
                cotacaoPedido.Tomador = null;

            cotacaoPedido.UsarTipoTomadorCotacaoPedido = Request.GetBoolParam("UsarTipoTomadorCotacaoPedido");
            if (cotacaoPedido.UsarTipoTomadorCotacaoPedido)
            {
                cotacaoPedido.TipoTomador = Request.GetEnumParam<Dominio.Enumeradores.TipoTomador>("TipoTomador");
                if (cotacaoPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && cotacaoPedido.Tomador == null)
                {
                    unitOfWork.Rollback();
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.QuandoTipoTomadorForOutrosObrigatorioInformarTomador);
                }

                if (cotacaoPedido.TipoTomador != Dominio.Enumeradores.TipoTomador.Outros)
                    cotacaoPedido.Tomador = null;
            }

            SalvarValor(ref cotacaoPedido, unitOfWork, inserindo);
            SalvarOrigem(ref cotacaoPedido, unitOfWork, inserindo);
            SalvarDestino(ref cotacaoPedido, unitOfWork, inserindo);
            SalvarImportacao(ref cotacaoPedido, unitOfWork, inserindo);
            SalvarAbaAdicional(ref cotacaoPedido, unitOfWork, inserindo);
        }

        private void SalvarImportacao(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoImportacao repCotacaoPedidoImportacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoImportacao(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

            dynamic dynAbaImportacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaImportacao"));

            double.TryParse((string)dynAbaImportacao.Porto, out double codigoPorto);

            int.TryParse((string)dynAbaImportacao.TipoTerminalImportacao, out int codigoTipoTerminalImportacao);
            int.TryParse((string)dynAbaImportacao.LocalidadeEntregaImportacao, out int codigoLocalidadeEntregaImportacao);

            if (codigoPorto > 0)
                cotacaoPedido.Porto = repCliente.BuscarPorCPFCNPJ(codigoPorto);
            else
                cotacaoPedido.Porto = null;

            if (codigoTipoTerminalImportacao > 0)
                cotacaoPedido.TipoTerminalImportacao = repTipoTerminalImportacao.BuscarPorCodigo(codigoTipoTerminalImportacao);
            else
                cotacaoPedido.TipoTerminalImportacao = null;

            if (codigoLocalidadeEntregaImportacao > 0)
                cotacaoPedido.LocalidadeEntregaImportacao = repLocalidade.BuscarPorCodigo(codigoLocalidadeEntregaImportacao);
            else
                cotacaoPedido.LocalidadeEntregaImportacao = null;

            DateTime.TryParse((string)dynAbaImportacao.DataVencimentoArmazenamentoImportacao, out DateTime dataVencimentoArmazenamentoImportacao);

            if (dataVencimentoArmazenamentoImportacao > DateTime.MinValue)
                cotacaoPedido.DataVencimentoArmazenamentoImportacao = dataVencimentoArmazenamentoImportacao;
            else
                cotacaoPedido.DataVencimentoArmazenamentoImportacao = null;

            cotacaoPedido.NumeroContainer = (string)dynAbaImportacao.NumeroContainer;
            cotacaoPedido.NumeroBL = (string)dynAbaImportacao.NumeroBL;
            cotacaoPedido.NumeroNavio = (string)dynAbaImportacao.NumeroNavio;
            cotacaoPedido.EnderecoEntregaImportacao = (string)dynAbaImportacao.EnderecoEntregaImportacao;
            cotacaoPedido.BairroEntregaImportacao = (string)dynAbaImportacao.BairroEntregaImportacao;
            cotacaoPedido.CEPEntregaImportacao = (string)dynAbaImportacao.CEPEntregaImportacao;
            cotacaoPedido.ArmadorImportacao = (string)dynAbaImportacao.ArmadorImportacao;

        }

        private void SalvarValor(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

            dynamic dynAbaValor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaValor"));

            decimal.TryParse((string)dynAbaValor.ValorCotacao, out decimal valorCotacao);
            decimal.TryParse((string)dynAbaValor.PercentualAcrescimoValorCotacao, out decimal percentualAcrescimoValorCotacao);
            decimal.TryParse((string)dynAbaValor.PercentualDescontoValorCotacao, out decimal percentualDescontoValorCotacao);
            decimal.TryParse((string)dynAbaValor.ValorTotalCotacao, out decimal valorTotalCotacao);
            decimal.TryParse((string)dynAbaValor.ValorICMS, out decimal valorICMS);
            decimal.TryParse((string)dynAbaValor.AliquotaICMS, out decimal aliquotaICMS);
            decimal.TryParse((string)dynAbaValor.ValorTotalCotacaoComICMS, out decimal valorTotalCotacaoComICMS);
            decimal.TryParse((string)dynAbaValor.ValorFrete, out decimal valorFrete);
            decimal.TryParse((string)dynAbaValor.ValorFreteTerceiro, out decimal valorFreteTerceiro);

            cotacaoPedido.ValorCotacao = valorCotacao;
            cotacaoPedido.PercentualAcrescimo = percentualAcrescimoValorCotacao;
            cotacaoPedido.PercentualDesconto = percentualDescontoValorCotacao;
            cotacaoPedido.ValorTotalCotacao = valorTotalCotacao;
            cotacaoPedido.ValorICMS = valorICMS;
            cotacaoPedido.AliquotaICMS = aliquotaICMS;
            cotacaoPedido.ValorTotalCotacaoComICMS = valorTotalCotacaoComICMS;
            cotacaoPedido.ValorFrete = valorFrete;
            cotacaoPedido.ValorFreteTerceiro = valorFreteTerceiro;
            cotacaoPedido.IncluirValorICMSBaseCalculo = ((string)dynAbaValor.IncluirValorICMSBaseCalculo).ToBool();
        }

        private void SalvarOrigem(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco repCotacaoPedidoEndereco = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco endereco = null;

            dynamic dynAbaOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaOrigem"));

            bool.TryParse((string)dynAbaOrigem.MudarEndereco, out bool mudarEndereco);

            int.TryParse((string)dynAbaOrigem.ClienteOutroEndereco, out int codigoClienteOutroEndereco);
            int.TryParse((string)dynAbaOrigem.Localidade, out int codigoLocalidade);

            cotacaoPedido.UsarOutroEnderecoOrigem = mudarEndereco;
            if (mudarEndereco)
            {
                if (cotacaoPedido.EnderecoOrigem != null)
                    endereco = repCotacaoPedidoEndereco.BuscarPorCodigo(cotacaoPedido.EnderecoOrigem.Codigo);
                else
                    endereco = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco();

                if (codigoLocalidade > 0)
                    endereco.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                else
                    endereco.Localidade = null;
                if (codigoClienteOutroEndereco > 0)
                    endereco.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(codigoClienteOutroEndereco);
                else
                    endereco.ClienteOutroEndereco = null;
                endereco.Bairro = (string)dynAbaOrigem.Bairro;
                endereco.CEP = (string)dynAbaOrigem.CEP;
                endereco.Numero = (string)dynAbaOrigem.Numero;
                endereco.Complemento = (string)dynAbaOrigem.Complemento;
                endereco.IE_RG = (!string.IsNullOrWhiteSpace((string)dynAbaOrigem.IERG)) ? ((string)dynAbaOrigem.IERG) : ((string)dynAbaOrigem.RGIE);
                endereco.Endereco = (string)dynAbaOrigem.Endereco;
                endereco.Telefone = (string)dynAbaOrigem.Telefone1;

                if (cotacaoPedido.EnderecoOrigem != null)
                    repCotacaoPedidoEndereco.Atualizar(endereco);
                else
                    repCotacaoPedidoEndereco.Inserir(endereco);

                cotacaoPedido.EnderecoOrigem = endereco;
            }
            else if (cotacaoPedido.EnderecoOrigem != null)
            {
                endereco = repCotacaoPedidoEndereco.BuscarPorCodigo(cotacaoPedido.EnderecoOrigem.Codigo);
                repCotacaoPedidoEndereco.Deletar(endereco);

                cotacaoPedido.EnderecoOrigem = null;
            }
        }

        private void SalvarDestino(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco repCotacaoPedidoEndereco = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco endereco = null;

            dynamic dynAbaDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaDestino"));

            bool.TryParse((string)dynAbaDestino.MudarEndereco, out bool mudarEndereco);

            int.TryParse((string)dynAbaDestino.ClienteOutroEndereco, out int codigoClienteOutroEndereco);
            int.TryParse((string)dynAbaDestino.Localidade, out int codigoLocalidade);

            cotacaoPedido.UsarOutroEnderecoDestino = mudarEndereco;
            if (mudarEndereco)
            {
                if (cotacaoPedido.EnderecoDestino != null)
                    endereco = repCotacaoPedidoEndereco.BuscarPorCodigo(cotacaoPedido.EnderecoDestino.Codigo);
                else
                    endereco = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco();

                if (codigoLocalidade > 0)
                    endereco.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                else
                    endereco.Localidade = null;
                if (codigoClienteOutroEndereco > 0)
                    endereco.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(codigoClienteOutroEndereco);
                else
                    endereco.ClienteOutroEndereco = null;
                endereco.Bairro = (string)dynAbaDestino.Bairro;
                endereco.CEP = (string)dynAbaDestino.CEP;
                endereco.Numero = (string)dynAbaDestino.Numero;
                endereco.Complemento = (string)dynAbaDestino.Complemento;
                endereco.IE_RG = (!string.IsNullOrWhiteSpace((string)dynAbaDestino.IERG)) ? ((string)dynAbaDestino.IERG) : ((string)dynAbaDestino.RGIE);
                endereco.Endereco = (string)dynAbaDestino.Endereco;
                endereco.Telefone = (string)dynAbaDestino.Telefone1;

                if (cotacaoPedido.EnderecoDestino != null)
                    repCotacaoPedidoEndereco.Atualizar(endereco);
                else
                    repCotacaoPedidoEndereco.Inserir(endereco);

                cotacaoPedido.EnderecoDestino = endereco;
            }
            else if (cotacaoPedido.EnderecoDestino != null)
            {
                endereco = repCotacaoPedidoEndereco.BuscarPorCodigo(cotacaoPedido.EnderecoDestino.Codigo);
                repCotacaoPedidoEndereco.Deletar(endereco);

                cotacaoPedido.EnderecoDestino = null;
            }
        }

        private void SalvarAbaAdicional(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoCubagem repCotacaoPedidoCubagem = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoCubagem(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

            dynamic dynAbaAdicional = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaAdicional"));

            DateTime.TryParse((string)dynAbaAdicional.DataInicialColeta, out DateTime dataInicialColeta);
            DateTime.TryParse((string)dynAbaAdicional.DataFinalColeta, out DateTime dataFinalColeta);
            if (dataInicialColeta > DateTime.MinValue)
                cotacaoPedido.DataInicialColeta = dataInicialColeta;
            else
                cotacaoPedido.DataInicialColeta = null;
            if (dataFinalColeta > DateTime.MinValue)
                cotacaoPedido.DataFinalColeta = dataFinalColeta;
            else
                cotacaoPedido.DataFinalColeta = null;

            Enum.TryParse((string)dynAbaAdicional.TipoModal, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal);
            Enum.TryParse((string)dynAbaAdicional.TipoPagamento, out Dominio.Enumeradores.TipoPagamento tipoPagamento);
            cotacaoPedido.TipoModal = tipoModal;
            cotacaoPedido.TipoPagamento = tipoPagamento;

            int.TryParse((string)dynAbaAdicional.NumeroPaletes, out int numeroPaletes);
            int.TryParse((string)dynAbaAdicional.QuantidadeNotas, out int quantidadeNotas);
            int.TryParse((string)dynAbaAdicional.QtdEntregas, out int qtdEntregas);
            int.TryParse((string)dynAbaAdicional.KMTotal, out int kmTotal);
            int.TryParse((string)dynAbaAdicional.QtdEscoltas, out int qtdEscoltas);
            int.TryParse((string)dynAbaAdicional.QtdAjudantes, out int qtdAjudantes);
            int.TryParse((string)dynAbaAdicional.QtVolumes, out int qtVolumes);
            cotacaoPedido.NumeroPaletes = numeroPaletes;
            cotacaoPedido.QuantidadeNotas = quantidadeNotas;
            cotacaoPedido.QtdEntregas = qtdEntregas;
            cotacaoPedido.KMTotal = kmTotal;
            cotacaoPedido.QtdEscoltas = qtdEscoltas;
            cotacaoPedido.QtdAjudantes = qtdAjudantes;
            cotacaoPedido.QtVolumes = qtVolumes;

            decimal.TryParse((string)dynAbaAdicional.PesoTotal, out decimal pesoTotal);
            decimal.TryParse((string)dynAbaAdicional.ValorTotalNotasFiscais, out decimal valorTotalNotasFiscais);
            decimal.TryParse((string)dynAbaAdicional.ValorPorKM, out decimal valorPorKM);
            decimal.TryParse((string)dynAbaAdicional.CubagemTotal, out decimal cubagemTotal);
            decimal.TryParse((string)dynAbaAdicional.TotalPesoCubado, out decimal totalPesoCubado);
            cotacaoPedido.PesoTotal = pesoTotal;
            cotacaoPedido.ValorTotalNotasFiscais = valorTotalNotasFiscais;
            cotacaoPedido.ValorPorKM = valorPorKM;
            cotacaoPedido.CubagemTotal = cubagemTotal;
            cotacaoPedido.PesoCubado = totalPesoCubado;

            cotacaoPedido.Temperatura = (string)dynAbaAdicional.Temperatura;
            cotacaoPedido.Observacao = (string)dynAbaAdicional.Observacao;
            cotacaoPedido.ObservacaoInterna = (string)dynAbaAdicional.ObservacaoInterna;

            bool.TryParse((string)dynAbaAdicional.Rastreado, out bool rastreado);
            bool.TryParse((string)dynAbaAdicional.GerenciamentoRisco, out bool gerenciamentoRisco);
            bool.TryParse((string)dynAbaAdicional.EscoltaArmada, out bool escoltaArmada);
            bool.TryParse((string)dynAbaAdicional.Ajudante, out bool ajudante);
            cotacaoPedido.Rastreado = rastreado;
            cotacaoPedido.GerenciamentoRisco = gerenciamentoRisco;
            cotacaoPedido.EscoltaArmada = escoltaArmada;
            cotacaoPedido.Ajudante = ajudante;
        }

        private void SalvarListasEntidades(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoCubagem repCotacaoPedidoCubagem = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoCubagem(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente repCotacaoPedidoComponente = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoImportacao repCotacaoPedidoImportacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoImportacao(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            //Lista das cubagens
            dynamic dynAbaAdicional = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaAdicional"));
            dynamic dynCubagem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynAbaAdicional.ListaCubagem);
            List<int> codigosCubagem = new List<int>();
            if (dynCubagem.Count > 0)
            {
                foreach (var cub in dynCubagem)
                {
                    int.TryParse((string)cub.Codigo, out int codigoCubagem);
                    Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem cubagem;
                    if (codigoCubagem > 0)
                        cubagem = repCotacaoPedidoCubagem.BuscarPorCodigo(codigoCubagem);
                    else
                        cubagem = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem();

                    cubagem.CotacaoPedido = cotacaoPedido;

                    decimal.TryParse((string)cub.Altura, out decimal altura);
                    decimal.TryParse((string)cub.Comprimento, out decimal comprimento);
                    decimal.TryParse((string)cub.FatorCubico, out decimal fatorCubico);
                    decimal.TryParse((string)cub.Largura, out decimal largura);
                    decimal.TryParse((string)cub.MetroCubico, out decimal metroCubico);
                    decimal.TryParse((string)cub.PesoCubado, out decimal pesoCubado);
                    cubagem.Altura = altura;
                    cubagem.Comprimento = comprimento;
                    cubagem.FatorCubico = fatorCubico;
                    cubagem.Largura = largura;
                    cubagem.MetroCubico = metroCubico;
                    cubagem.PesoCubado = pesoCubado;

                    int.TryParse((string)cub.QtdVolume, out int qtdVolume);
                    cubagem.QtdVolume = qtdVolume;

                    if (codigoCubagem > 0)
                        repCotacaoPedidoCubagem.Atualizar(cubagem);
                    else
                    {
                        repCotacaoPedidoCubagem.Inserir(cubagem);
                    }

                    codigosCubagem.Add(cubagem.Codigo);
                }
            }
            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem> cubagens = repCotacaoPedidoCubagem.BuscarPorCotacao(cotacaoPedido.Codigo);
            foreach (var cub in cubagens)
            {
                if (!codigosCubagem.Contains(cub.Codigo))
                    repCotacaoPedidoCubagem.Deletar(cub);
            }

            //Lista dos componentes
            dynamic dynAbaValor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaValor"));
            dynamic dynComponente = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynAbaValor.ListaComponente);
            List<long> codigosComponente = new List<long>();
            if (dynComponente.Count > 0)
            {
                foreach (var comp in dynComponente)
                {
                    int.TryParse((string)comp.Codigo, out int codigoComponente);
                    Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente componente;
                    if (codigoComponente > 0)
                        componente = repCotacaoPedidoComponente.BuscarPorCodigo(codigoComponente);
                    else
                        componente = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente();

                    componente.CotacaoPedido = cotacaoPedido;

                    int.TryParse((string)comp.CodigoComponente, out int codigoComponenteFrete);
                    if (codigoComponenteFrete > 0)
                        componente.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(codigoComponenteFrete);
                    else
                        componente.ComponenteFrete = null;

                    decimal.TryParse((string)comp.Valor, out decimal Valor);
                    decimal.TryParse((string)comp.Percentual, out decimal Percentual);
                    decimal.TryParse((string)comp.PercentualAcrescimo, out decimal percentualAcrescimo);
                    decimal.TryParse((string)comp.PercentualDesconto, out decimal percentualDesconto);
                    decimal.TryParse((string)comp.ValorTotal, out decimal valorTotal);

                    componente.Valor = Valor;
                    componente.Percentual = Percentual;
                    componente.PercentualAcrescimo = percentualAcrescimo;
                    componente.PercentualDesconto = percentualDesconto;
                    componente.ValorTotal = valorTotal;

                    if (codigoComponente > 0)
                        repCotacaoPedidoComponente.Atualizar(componente);
                    else
                    {
                        repCotacaoPedidoComponente.Inserir(componente);
                    }

                    codigosComponente.Add(componente.Codigo);
                }
            }
            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente> componentes = repCotacaoPedidoComponente.BuscarPorCotacao(cotacaoPedido.Codigo);
            foreach (var comp in componentes)
            {
                if (!codigosComponente.Contains(comp.Codigo))
                    repCotacaoPedidoComponente.Deletar(comp);
            }

            //Lista das Importações
            dynamic dynAbaImportacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AbaImportacao"));
            dynamic dynDI = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynAbaImportacao.ListaDI);
            List<int> codigosDI = new List<int>();
            if (dynDI.Count > 0)
            {
                foreach (var di in dynDI)
                {
                    int.TryParse((string)di.Codigo, out int codigoDI);
                    Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao importacao;
                    if (codigoDI > 0)
                        importacao = repCotacaoPedidoImportacao.BuscarPorCodigo(codigoDI);
                    else
                        importacao = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao();

                    importacao.CotacaoPedido = cotacaoPedido;

                    decimal.TryParse((string)di.ValorCarga, out decimal valorCarga);
                    decimal.TryParse((string)di.Volume, out decimal volume);
                    decimal.TryParse((string)di.Peso, out decimal peso);

                    importacao.ValorCarga = valorCarga;
                    importacao.Volume = volume;
                    importacao.Peso = peso;

                    importacao.NumeroDI = (string)di.NumeroDI;
                    importacao.CodigoImportacao = (string)di.CodigoImportacao;
                    importacao.CodigoReferencia = (string)di.CodigoReferencia;

                    if (codigoDI > 0)
                        repCotacaoPedidoImportacao.Atualizar(importacao);
                    else
                    {
                        repCotacaoPedidoImportacao.Inserir(importacao);
                    }

                    codigosDI.Add(importacao.Codigo);
                }
            }
            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao> importacoes = repCotacaoPedidoImportacao.BuscarPorCotacao(cotacaoPedido.Codigo);
            foreach (var imp in importacoes)
            {
                if (!codigosDI.Contains(imp.Codigo))
                    repCotacaoPedidoImportacao.Deletar(imp);
            }

        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, out string msgErro)
        {
            msgErro = "";

            if (cotacaoPedido.Numero <= 0)
            {
                msgErro = "Número é obrigatório.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar.Contains("ClienteCotacaoPedido"))
                propOrdenar = "ClienteAtivo.Nome, ClienteInativo.Nome, ClienteNovo, ClienteProspect.Nome, GrupoPessoas.Descricao";
            else if (propOrdenar.Contains("DescricaoSituacaoPedido"))
                propOrdenar = "SituacaoPedido";
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito SituacaoSolicitacao(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido)
        {
            if (cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.AgLiberacao;
            else if (cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Rejeitado)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Rejeitado;
            else if (cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Utilizado;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Todos;
        }

        private bool VerificarRegrasCotacaoPedido(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaFiltrada = Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarRegrasCotacaoPedido(cotacaoPedido, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.CotacaoPedido.CotacaoPedido.CriarRegrasAutorizacao(listaFiltrada, cotacaoPedido, this.Usuario, tipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
                return true;
            }

            return false;
        }

        private void GerarRelatorioCotacaoPedido(int codigoCotacao, IList<Dominio.Relatorios.Embarcador.DataSource.Cotacoes.CotacaoPedido> dadosCotacaoPedido, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {
            ReportRequest.WithType(ReportType.CotacaoPedido)
                .WithExecutionType(ExecutionType.Async)
                .AddExtraData("CodigoCotacao", codigoCotacao)
                .AddExtraData("DadosCotacaoPedidoDs", dadosCotacaoPedido.ToJson())
                .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                .CallReport();
        }

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {

            // Manipula grids
            Models.Grid.Grid grid = GridPesquisa();

            // Ordenacao da grid
            string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
            PropOrdena(ref propOrdenar);

            // Busca Dados
            int totalRegistros = 0;
            var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

            if (exportacao && totalRegistros > 10000)
                return new JsonpResult(false, true, "Exportação não permitida para mais de 10.000 registros.");

            // Seta valores na grid
            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private (int Codigo, DateTime DataColeta, decimal PesoLiquido, decimal Cubagem, int NumeroPaletes, int Unidades) ObterParametrosPedidoFracionado()
        {
            int.TryParse(Request.Params("CodigoCotacaoPedido"), out int codigo);
            DateTime.TryParse(Request.Params("DataColetaPedidoFracionado"), out DateTime dataColeta);
            decimal.TryParse(Request.Params("PesoLiquidoPedidoFracionado"), out decimal pesoLiquido);
            decimal.TryParse(Request.Params("CubagemPedidoFracionado"), out decimal cubagem);
            int.TryParse(Request.Params("NumeroPaletesPedidoFracionado"), out int numeroPaletes);
            int.TryParse(Request.Params("UnidadesPedidoFracionado"), out int unidades);

            return (codigo, dataColeta, pesoLiquido, cubagem, numeroPaletes, unidades);
        }

        private static bool ValidarParametrosPedidoFracionado((int Codigo, DateTime DataColeta, decimal PesoLiquido, decimal Cubagem, int NumeroPaletes, int Unidades) parametros, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (parametros.DataColeta == DateTime.MinValue)
            {
                mensagemErro = "É necessário adicionar uma data de coleta para gerar um pedido fracionado da cotação!";
                return false;
            }

            if (parametros.PesoLiquido == 0 && parametros.Cubagem == 0 && parametros.NumeroPaletes == 0 && parametros.Unidades == 0)
            {
                mensagemErro = "Não é possível gerar um pedido fracionado sem referenciar quantidades!";
                return false;
            }

            return true;
        }

        private static bool ValidarModeloVeicular(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido,
            (int Codigo, DateTime DataColeta, decimal PesoLiquido, decimal Cubagem, int NumeroPaletes, int Unidades) parametros,
            out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (cotacaoPedido.ModeloVeicularCarga == null)
                return true;

            var modelo = cotacaoPedido.ModeloVeicularCarga;

            if (parametros.PesoLiquido > 0 && modelo.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Peso)
            {
                if (parametros.PesoLiquido > modelo.CapacidadePesoTransporte || parametros.PesoLiquido < modelo.ToleranciaPesoMenor)
                {
                    mensagemErro = "O pedido não pode ser criado devido as configurações de modelo veicular!";
                    return false;
                }
            }

            if (parametros.Unidades > 0 && modelo.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Unidade)
            {
                if (parametros.Unidades > modelo.CapacidadePesoTransporte || parametros.Unidades < modelo.ToleranciaPesoMenor)
                {
                    mensagemErro = "O pedido não pode ser criado devido as configurações de modelo veicular!";
                    return false;
                }
            }

            if (parametros.Cubagem > 0 && modelo.ModeloControlaCubagem)
            {
                if (parametros.Cubagem > modelo.Cubagem || parametros.Cubagem < modelo.ToleranciaMinimaCubagem)
                {
                    mensagemErro = "O pedido não pode ser criado devido as configurações de modelo veicular!";
                    return false;
                }
            }

            if (parametros.NumeroPaletes > 0 && modelo.VeiculoPaletizado)
            {
                if (parametros.NumeroPaletes > modelo.NumeroPaletes || parametros.NumeroPaletes < modelo.ToleranciaMinimaPaletes)
                {
                    mensagemErro = "O pedido não pode ser criado devido as configurações de modelo veicular!";
                    return false;
                }
            }

            return true;
        }

        private static bool ValidarPedidosExistentes(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido,
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos,
            (int Codigo, DateTime DataColeta, decimal PesoLiquido, decimal Cubagem, int NumeroPaletes, int Unidades) parametros,
            out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (pedidos.Count == 0)
                return true;

            int unidadesPedidosCriados = pedidos.Sum(x => x.QtdEntregas);
            int numeroPaletesPedidosCriados = pedidos.Sum(x => x.NumeroPaletes);
            decimal pesoLiquidoPedidosCriados = pedidos.Sum(x => x.PesoTotal);
            decimal cubagemPedidosCriados = pedidos.Sum(x => x.CubagemTotal);

            if (cotacaoPedido.QtdEntregas > 0 && (parametros.PesoLiquido > (cotacaoPedido.QtdEntregas - unidadesPedidosCriados)))
            {
                mensagemErro = "Não é possível criar um pedido com a quantidade específicada pois supera a quantidade aprovada na cotação!";
                return false;
            }

            if (cotacaoPedido.NumeroPaletes > 0 && (parametros.PesoLiquido > (cotacaoPedido.NumeroPaletes - numeroPaletesPedidosCriados)))
            {
                mensagemErro = "Não é possível criar um pedido com a quantidade específicada pois supera a quantidade aprovada na cotação!";
                return false;
            }

            if (cotacaoPedido.PesoTotal > 0M && (parametros.PesoLiquido > (cotacaoPedido.PesoTotal - pesoLiquidoPedidosCriados)))
            {
                mensagemErro = "Não é possível criar um pedido com a quantidade específicada pois supera a quantidade aprovada na cotação!";
                return false;
            }

            if (cotacaoPedido.CubagemTotal > 0M && (parametros.PesoLiquido > (cotacaoPedido.CubagemTotal - cubagemPedidosCriados)))
            {
                mensagemErro = "Não é possível criar um pedido com a quantidade específicada pois supera a quantidade aprovada na cotação!";
                return false;
            }

            return true;
        }

        private Models.Grid.Grid GridDetalhesPedidoFracionado()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Medida", "Medida", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Já solicitado", "JaSolicitado", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Já transportado", "JaTransportado", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Total da cotação", "TotalCotacao", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("% Transportado", "PorcentagemTransportado", 10, Models.Grid.Align.right, true);
            return grid;
        }

        private Models.Grid.Grid GridHistoricoPedidosFracionados()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data coleta", "DataColeta", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situacao", "SituacaoPedido", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Peso bruto", "PesoBruto", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("N.° Pallets", "NumeroPallets", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Peso cubado", "PesoCubado", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Unidades", "Unidades", 5, Models.Grid.Align.left, true);
            return grid;
        }

        private dynamic ExecutaBuscaDetalhesPedidoFracionado(ref int totalRegistros, ref Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            // Parametros
            int.TryParse(Request.Params("Codigo"), out int codigo);

            // Busca informacoes
            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCotacaoQueNaoEstejaCanceladaOuRejeitada(cotacaoPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCotacaoCargaFinalizada(cotacaoPedido.Codigo);

            // Calcula medidas dos pedidos criados
            var medidasPedidosCriados = CalcularMedidas(pedidos);

            // Calcula medidas das cargas transportadas
            var medidasCargasTransportadas = CalcularMedidas(cargasPedidos.Select(x => x.Pedido).ToList());

            // Calcula porcentagens transportadas
            var porcentagensTransportadas = CalcularPorcentagensTransportadas(medidasCargasTransportadas, cotacaoPedido);

            // Cria lista de medidas
            var medidas = new[]
            {
                CriarMedida("Peso", medidasPedidosCriados.Peso, medidasCargasTransportadas.Peso, cotacaoPedido.PesoTotal, porcentagensTransportadas.Peso),
                CriarMedida("Pallets", medidasPedidosCriados.NumeroPaletes, medidasCargasTransportadas.NumeroPaletes, cotacaoPedido.NumeroPaletes, porcentagensTransportadas.NumeroPaletes),
                CriarMedida("Cubagem", medidasPedidosCriados.Cubagem, medidasCargasTransportadas.Cubagem, cotacaoPedido.CubagemTotal, porcentagensTransportadas.Cubagem),
                CriarMedida("Unidade", medidasPedidosCriados.Unidades, medidasCargasTransportadas.Unidades, cotacaoPedido.QtdEntregas, porcentagensTransportadas.Unidades)
            };

            return medidas.ToList();
        }

        private dynamic ExecutaBuscaHistoricoPedidosFracionados(ref int totalRegistros, ref Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            // Parametros
            int.TryParse(Request.Params("Codigo"), out int codigo);

            // Busca informacoes
            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCotacao(cotacaoPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCotacaoCarga(cotacaoPedido.Codigo);

            // Otimização: cria um dicionário lookup pra agilizar o acesso
            var cargaPedidoLookup = cargasPedidos.ToLookup(c => c.Pedido.Codigo);

            totalRegistros = pedidos.Count;

            var listaHistoricoPedidos = pedidos.Select(pedido =>
            {
                var cargaPedido = cargaPedidoLookup[pedido.Codigo].FirstOrDefault();

                return new
                {
                    NumeroPedido = pedido.Numero.ToString() ?? string.Empty,
                    NumeroCarga = cargaPedido?.Carga.CodigoCargaEmbarcador ?? string.Empty,
                    Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                    Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                    DataColeta = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? string.Empty,
                    SituacaoPedido = pedido.SituacaoPedido.ObterDescricao() ?? string.Empty,
                    PesoBruto = pedido.PesoTotal.ToString("N2") ?? string.Empty,
                    NumeroPallets = pedido.NumeroPaletes.ToString() ?? string.Empty,
                    PesoCubado = pedido.PesoCubado.ToString("N2") ?? string.Empty,
                    Unidades = pedido.QtdEntregas.ToString() ?? string.Empty,
                };
            });

            return listaHistoricoPedidos.ToList();
        }

        private static (decimal Peso, int Unidades, decimal Cubagem, int NumeroPaletes) CalcularMedidas(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            if (pedidos == null || !pedidos.Any())
                return (0, 0, 0, 0);

            return (
                pedidos.Sum(x => x.PesoTotal),
                pedidos.Sum(x => x.QtdEntregas),
                pedidos.Sum(x => x.CubagemTotal),
                pedidos.Sum(x => x.NumeroPaletes)
            );
        }

        private static (decimal Peso, decimal Unidades, decimal Cubagem, decimal NumeroPaletes) CalcularPorcentagensTransportadas(
            (decimal Peso, int Unidades, decimal Cubagem, int NumeroPaletes) medidasCargasTransportadas,
            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido)
        {
            static decimal DivisaoSegura(decimal numerador, decimal denominador) => denominador == 0 ? 0 : (numerador / denominador) * 100;

            return (
                DivisaoSegura(medidasCargasTransportadas.Peso, cotacaoPedido.PesoTotal),
                DivisaoSegura(medidasCargasTransportadas.Unidades, cotacaoPedido.QtdEntregas),
                DivisaoSegura(medidasCargasTransportadas.Cubagem, cotacaoPedido.CubagemTotal),
                DivisaoSegura(medidasCargasTransportadas.NumeroPaletes, cotacaoPedido.NumeroPaletes)
            );
        }

        private static object CriarMedida(string medida, decimal jaSolicitado, decimal jaTransportado, decimal totalCotacao, decimal porcentagemTransportado)
        {
            return new
            {
                Medida = medida,
                JaSolicitado = jaSolicitado.ToString("N2"),
                JaTransportado = jaTransportado.ToString("N2"),
                TotalCotacao = totalCotacao.ToString("N2"),
                PorcentagemTransportado = porcentagemTransportado.ToString("N2")
            };
        }

        #endregion

        #region Métodos Privados - Importações

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ", Propriedade = "CNPJ", Tamanho = 150, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } }
            };

            return configuracoes;
        }

        #endregion
    }
}
