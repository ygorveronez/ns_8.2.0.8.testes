using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.WebService.CRT
{
    public class CRT : ServicoBase
    {
        #region Propriedades Privadas

        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Construtores

        public CRT(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CRT(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork)
        {
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion


        #region Métodos Públicos

        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<bool>> InformarCRTPorCargaAsync(Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            try
            {
                await _unitOfWork.StartAsync();

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

                Dominio.Entidades.Empresa empresa = await repositorioEmpresa.BuscarPorCNPJAsync(integracaoCRT.CnpjTransportador);

                if (empresa == null)
                    throw new ServicoException("Transportador não localizado");

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = await repositorioCargaCTe.BuscarPrimeiroPorCargaAsync(integracaoCRT.ProtocoloIntegracaoCarga);

                if (cargaCTe == null)
                    throw new ServicoException("Pre-CRT não localizado");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await GerarCRTAnteriorAsync(empresa, integracaoCRT, cargaCTe.CTe);

                cargaCTe.CTe = cte;

                await repositorioCargaCTe.AtualizarAsync(cargaCTe);

                new Servicos.Embarcador.Documentos.GestaoDocumento(_unitOfWork).CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AprovacaoObrigatoria, _tipoServicoMultisoftware, cargaCTe);

                await Servicos.Auditoria.Auditoria.AuditarAsync(_auditado, cargaCTe, null, "CRT Importado com Sucesso", _unitOfWork);

                await _unitOfWork.CommitChangesAsync();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "CRT Importado com Sucesso");
            }
            catch (ServicoException ex)
            {
                await _unitOfWork.RollbackAsync();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha");
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados
        public async Task<Dominio.Entidades.ConhecimentoDeTransporteEletronico> GerarCRTAnteriorAsync(Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior)
        {
            Dominio.Entidades.EmpresaSerie serie = await ObterSerie(empresa, integracaoCRT.Serie);

            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.NaturezaDaOperacao repositorioNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.CFOP repositorioCFOP = new Repositorio.CFOP(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.ModalTransporte repositorioModalTransporte = new Repositorio.ModalTransporte(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();

            conhecimento.Empresa = empresa;
            conhecimento.ValorAReceber = integracaoCRT.ValorTotal;
            conhecimento.ValorPrestacaoServico = integracaoCRT.ValorTotal;
            conhecimento.ValorFrete = conhecimento.ValorPrestacaoServico;
            conhecimento.ValorTotalMercadoria = integracaoCRT.ValorMercadoria;
            conhecimento.ValorCotacaoMoeda = integracaoCRT.ValorCotacaMoeda;
            conhecimento.ValorTotalMoeda = integracaoCRT.ValorTotalMoeda;
            conhecimento.Peso = integracaoCRT.Peso;
            conhecimento.PercentualICMSIncluirNoFrete = 100;

            conhecimento.LocalidadeInicioPrestacao = repositorioLocalidade.BuscarPorCodigoIBGE(integracaoCRT.CodigoIBGEInicioPrestacao);
            conhecimento.LocalidadeEmissao = repositorioLocalidade.BuscarPorCodigoIBGE(integracaoCRT.CodigoIBGEEmissao);
            conhecimento.LocalidadeTerminoPrestacao = repositorioLocalidade.BuscarPorCodigoIBGE(integracaoCRT.CodigoIBGETerminoPrestacao);

            conhecimento.ModalTransporte = repositorioModalTransporte.BuscarPorCodigo(1, false);
            conhecimento.TipoControle = 1;
            conhecimento.CFOP = await repositorioCFOP.BuscarPrimeiroRegistroAsync();

            conhecimento.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            conhecimento.ModeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorModeloCRT();
            conhecimento.Serie = serie;
            conhecimento.TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato;
            conhecimento.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
            conhecimento.TipoCTE = Dominio.Enumeradores.TipoCTE.Normal;
            conhecimento.Retira = Dominio.Enumeradores.OpcaoSimNao.Nao;
            conhecimento.NaturezaDaOperacao = await repositorioNaturezaDaOperacao.BuscarPrimeiroRegistroAsync();
            conhecimento.TipoAmbiente = (_clienteAcesso?.URLHomologacao ?? false) ? Dominio.Enumeradores.TipoAmbiente.Homologacao : Dominio.Enumeradores.TipoAmbiente.Producao;

            await SetarClienteCTeOS(conhecimento, integracaoCRT);

            conhecimento.DataEmissao = integracaoCRT.DataEmissao;
            conhecimento.NumeroCRT = integracaoCRT.Numero;
            conhecimento.Numero = cteAnterior == null ? integracaoCRT.Numero.ObterSomenteNumeros().ToInt(): (cteAnterior.Numero + 1);
            conhecimento.Status = "A";
            conhecimento.Cancelado = "N";
            conhecimento.Chave = integracaoCRT.Chave;
            conhecimento.Protocolo = integracaoCRT.Protocolo;
            conhecimento.DataAutorizacao = integracaoCRT.DataEmissao;

            conhecimento.Versao = "4.00";
            conhecimento.MensagemRetornoSefaz = "CRT Processado (Importação).";
            conhecimento.Log = string.Concat("CRT importado com sucesso em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
            conhecimento.CTeSemCarga = true;

            await repositorioCTe.InserirAsync(conhecimento);

            ObterICMS(conhecimento, integracaoCRT);

            conhecimento.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            conhecimento.ExibeICMSNaDACTE = true;
            conhecimento.TomadorPagador = conhecimento.Remetente;

            await ObterInformacoesCTeNormalAsync(conhecimento, integracaoCRT);
            await SalvarInformacoesComponentesDaPrestacao(conhecimento, integracaoCRT);
            await GerarOutroDocumentoCRTAsync(conhecimento, integracaoCRT);

            await repositorioCTe.AtualizarAsync(conhecimento);

            Repositorio.IntegracaoCTeRecebimento repositorioIntegracaoCTeRecebimento = new Repositorio.IntegracaoCTeRecebimento(_unitOfWork);
            Dominio.Entidades.IntegracaoCTeRecebimento integracaoCTeRecebimento = new Dominio.Entidades.IntegracaoCTeRecebimento()
            {
                CTe = conhecimento,
                Data = DateTime.Now,
                DataStatus = DateTime.Now,
                Status = Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe.Pendente,
                Tipo = Dominio.Enumeradores.TipoIntegracaoEnvioCTe.Autorizacao
            };

            await repositorioIntegracaoCTeRecebimento.InserirAsync(integracaoCTeRecebimento);

            return conhecimento;
        }

        private async Task<Dominio.Entidades.EmpresaSerie> ObterSerie(Dominio.Entidades.Empresa empresa, int serie)
        {
            Repositorio.EmpresaSerie repositorioSerie = new Repositorio.EmpresaSerie(_unitOfWork);
            Dominio.Entidades.EmpresaSerie empSerie = await repositorioSerie.BuscarPorSerieAsync(empresa.Codigo, serie, Dominio.Enumeradores.TipoSerie.CTe);

            if (empSerie == null)
            {
                empSerie = new Dominio.Entidades.EmpresaSerie()
                {
                    Empresa = empresa,
                    Numero = serie,
                    Status = "A",
                    Tipo = Dominio.Enumeradores.TipoSerie.CTe
                };

                await repositorioSerie.InserirAsync(empSerie);
            }

            return empSerie;
        }

        private async Task SetarClienteCTeOS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.DadosCliente repositorioDadosCliente = new Repositorio.DadosCliente(_unitOfWork);

            double cnpjRemetente = integracaoCRT.CnpjRemetente.ToDouble();
            Dominio.Entidades.Cliente remetente = await repositorioCliente.BuscarPorCPFCNPJAsync(cnpjRemetente);
            Dominio.Entidades.DadosCliente dadosRemetente = repositorioDadosCliente.Buscar(cte.Empresa.Codigo, cnpjRemetente);

            double cnpjDestinatario = integracaoCRT.CnpjDestinatario.ToDouble();
            Dominio.Entidades.Cliente destinatario = await repositorioCliente.BuscarPorCPFCNPJAsync(cnpjDestinatario);
            Dominio.Entidades.DadosCliente dadosDestinatario = repositorioDadosCliente.Buscar(cte.Empresa.Codigo, cnpjDestinatario);

            double cnpjConsignatario = integracaoCRT.CnpjConsignatario.ToDouble();
            Dominio.Entidades.Cliente tomador = await repositorioCliente.BuscarPorCPFCNPJAsync(cnpjConsignatario);
            Dominio.Entidades.DadosCliente dadosTomador = repositorioDadosCliente.Buscar(cte.Empresa.Codigo, cnpjConsignatario);

            cte.SetarParticipante(remetente, Dominio.Enumeradores.TipoTomador.Remetente, null, dadosRemetente);
            cte.SetarParticipante(destinatario, Dominio.Enumeradores.TipoTomador.Destinatario, null, dadosDestinatario);
            cte.SetarParticipante(tomador, Dominio.Enumeradores.TipoTomador.Tomador, null, dadosTomador);
        }

        private void ObterICMS(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            conhecimento.AliquotaICMS = integracaoCRT.AliquotaICMS;
            conhecimento.PercentualReducaoBaseCalculoICMS = integracaoCRT.PercentualReducaoBaseCalculoICMS;
            conhecimento.BaseCalculoICMS = integracaoCRT.BaseCalculoICMS;
            conhecimento.ValorICMS = integracaoCRT.ValorICMS;
            conhecimento.CST = integracaoCRT.CST;
        }

        private async Task ObterInformacoesCTeNormalAsync(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            await SalvarInformacoesDeQuantidadeDaCargaAsync(conhecimento, integracaoCRT);

            if (conhecimento.ModalTransporte.modalProcCTe == 1 || conhecimento.ModalTransporte.modalProcCTe == 0)
                await SalvarInformacoesModalRodoviarioAsync(conhecimento, integracaoCRT);
        }

        private async Task SalvarInformacoesDeQuantidadeDaCargaAsync(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            Repositorio.InformacaoCargaCTE repositorioInfoCarga = new Repositorio.InformacaoCargaCTE(_unitOfWork);

            Dominio.Entidades.InformacaoCargaCTE infoQuantCarga = new Dominio.Entidades.InformacaoCargaCTE()
            {
                CTE = conhecimento,
                UnidadeMedida = "04",
                Tipo = "UN",
                Quantidade = integracaoCRT.QuantidadeItensCarga
            };

            await repositorioInfoCarga.InserirAsync(infoQuantCarga);
        }

        private async Task SalvarInformacoesModalRodoviarioAsync(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            conhecimento.DataPrevistaEntrega = DateTime.Now;
            conhecimento.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Nao;
            conhecimento.RNTRC = conhecimento.Empresa.RegistroANTT;

            await SalvarInformacoesVeiculosAsync(conhecimento, integracaoCRT);
        }

        private async Task SalvarInformacoesVeiculosAsync(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            if (string.IsNullOrEmpty(integracaoCRT.PlacaVeiculo))
                return;

            Repositorio.VeiculoCTE repositorioVeiculoCTe = new Repositorio.VeiculoCTE(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = await repositorioVeiculo.BuscarPorPlacaAsync(conhecimento.Empresa.Codigo, integracaoCRT.PlacaVeiculo);

            if (veiculo == null)
                veiculo = await repositorioVeiculo.BuscarPorPlacaESemEmpresaAsync(integracaoCRT.PlacaVeiculo);

            if (veiculo == null) return;

            Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE();

            veiculoCTe.CTE = conhecimento;
            veiculoCTe.Veiculo = veiculo;
            veiculoCTe.SetarDadosVeiculo(veiculo);

            await repositorioVeiculoCTe.InserirAsync(veiculoCTe);

        }

        private async Task SalvarInformacoesComponentesDaPrestacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            Repositorio.ComponentePrestacaoCTE repositorioCompPrestCTe = new Repositorio.ComponentePrestacaoCTE(_unitOfWork);

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesDaPrestacao = new List<Dominio.Entidades.ComponentePrestacaoCTE>();

            foreach (Dominio.ObjetosDeValor.WebService.CRT.ComponentePrestacaoCRT item in integracaoCRT.ComponentesFrete)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteDaPrestacao = new Dominio.Entidades.ComponentePrestacaoCTE();

                componenteDaPrestacao.CTE = conhecimento;
                componenteDaPrestacao.Nome = item.Nome;
                componenteDaPrestacao.Valor = Math.Round(item.Valor, 2, MidpointRounding.ToEven);
                componenteDaPrestacao.IncluiNaBaseDeCalculoDoICMS = item.IncluiNaBaseDeCalculoDoICMS;
                componenteDaPrestacao.IncluiNoTotalAReceber = item.IncluiNoTotalAReceber;

                componentesDaPrestacao.Add(componenteDaPrestacao);
            }

            await repositorioCompPrestCTe.InserirAsync(componentesDaPrestacao, "T_CTE_COMP_PREST");
        }

        private async Task GerarOutroDocumentoCRTAsync(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            Repositorio.DocumentosCTE repositorioDocumentoCTe = new Repositorio.DocumentosCTE(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);

            List<Dominio.Entidades.DocumentosCTE> documentos = new List<Dominio.Entidades.DocumentosCTE>();

            foreach (Dominio.ObjetosDeValor.WebService.CRT.Factura factura in integracaoCRT.Facturas)
            {
                Dominio.Entidades.DocumentosCTE documento = new Dominio.Entidades.DocumentosCTE
                {
                    CTE = cte,
                    DataEmissao = cte.DataEmissao ?? DateTime.Now,
                    Descricao = "Outros",
                    Numero = factura.Numero,
                    ModeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorModelo("99"),
                    Valor = factura.ValorMercadoria,
                    Peso = factura.Peso,
                };

                documentos.Add(documento);
            }

            await repositorioDocumentoCTe.InserirAsync(documentos, "T_CTE_DOCS");
        }

        #endregion Métodos Privados
    }
}

