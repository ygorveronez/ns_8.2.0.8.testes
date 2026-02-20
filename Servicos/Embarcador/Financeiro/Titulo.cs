using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Servicos.Embarcador.Financeiro
{
    public class Titulo
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public Titulo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Titulo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
        }

		#endregion

		#region Métodos Públicos

		public static string ObterPortalClienteCodigo(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.Embarcador.Financeiro.Titulo repTitulo)
		{
			if (!string.IsNullOrWhiteSpace(titulo.PortalClienteCodigo))
				return titulo.PortalClienteCodigo;

			return GerarPortalClienteCodigo(titulo, repTitulo);
		}

		public static string GerarPortalClienteCodigo(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.Embarcador.Financeiro.Titulo repTitulo)
		{
			bool portalClienteCodigoJaExistente;
			string portalClienteCodigo;

			do
			{
				portalClienteCodigo = Guid.NewGuid().ToString().Replace("-", "");
				portalClienteCodigoJaExistente = repTitulo.PortalClienteCodigoJaExistente(portalClienteCodigo);
			} while (portalClienteCodigoJaExistente);

			titulo.PortalClienteCodigo = portalClienteCodigo;
			repTitulo.Atualizar(titulo);

			return portalClienteCodigo;
		}

		public static string ObterURLPortalClienteCodigo(string urlAcesso, string portalClienteCodigo)
		{
			return $"https://{urlAcesso}/PortalCliente/Titulo/{portalClienteCodigo}";
		}

		public static void GerarCancelamentoAutomaticoTitulosEmAberto(List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos, string motivoCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if ((titulos == null) || (titulos.Count == 0))
                return;

            for (int i = 0; i < titulos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = titulos[i];

                GerarCancelamentoAutomaticoTituloEmAberto(titulo, motivoCancelamento, tipoServicoMultisoftware, unitOfWork);
            }
        }

        public static void GerarCancelamentoAutomaticoTituloEmAberto(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, string motivoCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
            {
                if (titulo.TipoMovimento != null)
                    servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, titulo.ValorOriginal, titulo.Codigo.ToString(), motivoCancelamento, unitOfWork, titulo.TipoTitulo == TipoTitulo.Pagar ? TipoDocumentoMovimento.Pagamento : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas);

                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                titulo.DataAlteracao = DateTime.Now;
                titulo.DataCancelamento = DateTime.Now.Date;

                repTitulo.Atualizar(titulo);

                if (titulo.TipoTitulo == TipoTitulo.Receber)
                {
                    new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Cancelamento);

                    Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento in titulo.Documentos.ToList())
                    {
                        Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;

                        if (tituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.Carga)
                            documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(tituloDocumento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                        else
                            documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(tituloDocumento.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);

                        if (documentoFaturamento != null)
                        {
                            documentoFaturamento.ValorEmFatura -= tituloDocumento.Valor;
                            documentoFaturamento.ValorDesconto -= tituloDocumento.ValorDesconto;
                            documentoFaturamento.ValorAcrescimo -= tituloDocumento.ValorAcrescimo;
                            documentoFaturamento.ValorAFaturar += tituloDocumento.Valor;

                            repDocumentoFaturamento.Atualizar(documentoFaturamento);
                        }
                    }
                }
            }
        }

        public void GerarTituloPorDocumentoRecebidoIntegracao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.WebService.CTe.CTeTitulo cteTitulo, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Servicos.WebService.Pessoas.Pessoa(_unitOfWork);
            Servicos.Cliente svsCliente = new Servicos.Cliente(_unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUso = cteTitulo.TipoMovimento != null ? repTipoMovimento.BuscarPorDescricao(cteTitulo.TipoMovimento.Descricao) : null;
            if (tipoMovimentoUso == null)
                tipoMovimentoUso = ObterTipoMovimentoConfiguracaoFinanceiraFatura(cte, null, configuracaoFinanceiraFatura);

            var pessoaDestinatario = svsCliente.ConverterObjetoValorPessoa(cteTitulo.Pessoa, string.Empty, _unitOfWork, 0, false, true, Auditado, tipoServicoMultisoftware, false, true);

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
            {
                DataEmissao = cteTitulo.DataEmissao,
                DataVencimento = cteTitulo.DataVencimento,
                Sequencia = cteTitulo.Sequencia,
                StatusTitulo = cteTitulo.StatusTitulo,
                DataAlteracao = cteTitulo.DataAlteracao,
                TipoTitulo = cteTitulo.TipoTitulo,
                ValorOriginal = cteTitulo.ValorOriginal,
                ValorPendente = cteTitulo.ValorPendente,
                ValorTituloOriginal = cteTitulo.ValorTituloOriginal,
                Valor = cteTitulo.Valor,
                ValorTotal = cteTitulo.ValorTotal,
                GrupoPessoas = cteTitulo.GrupoPessoas != null ? repGrupoPessoa.BuscarPorCodigoIntegracao(cteTitulo.GrupoPessoas.CodigoIntegracao) : null,
                Pessoa = pessoaDestinatario.Status && pessoaDestinatario.cliente != null ? pessoaDestinatario.cliente : null,
                TipoMovimento = tipoMovimentoUso,
                TipoDocumentoTituloOriginal = cteTitulo.TipoDocumentoTituloOriginal,
                NumeroDocumentoTituloOriginal = cteTitulo.NumeroDocumentoTituloOriginal,
                Empresa = cte.Empresa,
                DataLancamento = cteTitulo.DataLancamento,
                Usuario = cte.Usuario,               
                ConhecimentoDeTransporteEletronico = cte
            };

            if (cteTitulo.Codigo > 0)
                titulo.CodigoRecebidoIntegracao = cteTitulo.Codigo;

            if (titulo.Pessoa == null)
                return;

            if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

            titulo.DataProgramacaoPagamento = titulo.DataVencimento;
            titulo.FormaTitulo = cteTitulo.FormaTitulo;

            titulo.BoletoStatusTitulo = cteTitulo.BoletoStatusTitulo;
            titulo.BoletoConfiguracao = !string.IsNullOrWhiteSpace(cteTitulo.BoletoConfiguracao) ? repBoletoConfiguracao.BuscarPorDescricaoBanco(cteTitulo.BoletoConfiguracao) : null;
            titulo.BoletoEnviadoPorEmail = cteTitulo.BoletoEnviadoPorEmail;
            titulo.BoletoGeradoAutomaticamente = cteTitulo.BoletoGeradoAutomaticamente;
            titulo.NossoNumero = cteTitulo.NossoNumero;
            titulo.EnviarDocumentacaoFaturamentoCTe = cteTitulo.BoletoGeradoAutomaticamente;

            if (cte.Moeda.HasValue && cte.Moeda != MoedaCotacaoBancoCentral.Real)
            {
                titulo.MoedaCotacaoBancoCentral = cte.Moeda;
                titulo.ValorOriginalMoedaEstrangeira = cteTitulo.ValorOriginalMoedaEstrangeira;
                titulo.ValorMoedaCotacao = cte.ValorCotacaoMoeda ?? 0m;
                titulo.DataBaseCRT = cteTitulo.DataBaseCRT;
            }

            repTitulo.Inserir(titulo);

            Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento
            {
                CTe = cte,
                TipoDocumento = TipoDocumentoTitulo.CTe,
                Titulo = titulo,
                Valor = titulo.ValorOriginal,
                ValorTotal = titulo.ValorOriginal,
                ValorPendente = titulo.ValorOriginal,
                ValorMoeda = titulo.ValorOriginalMoedaEstrangeira,
                ValorTotalMoeda = titulo.ValorOriginalMoedaEstrangeira,
                ValorPendenteMoeda = titulo.ValorOriginalMoedaEstrangeira,
                ValorCotacaoMoeda = titulo.ValorMoedaCotacao
            };

            cte.CTePendenteIntegracaoFatura = true;

            repCTe.Atualizar(cte);
            repTituloDocumento.Inserir(tituloDocumento);

            servProcessoMovimento.GerarMovimentacao(tipoMovimentoUso, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Historico, _unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas);

            if (cteTitulo.GerarFaturamentoAVista)
                QuitarTitulo(out _, titulo, DateTime.Now.Date, DateTime.Now.Date, _unitOfWork, titulo.Pessoa, titulo.GrupoPessoas, cte?.Usuario, tipoServicoMultisoftware, "", 0m, false, 0m, false, Auditado);
        }

        public void GerarTituloPorDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool gerarFaturamentoAVista, bool gerarBoletoAutomaticamente, bool gerarTituloBloqueado, int codigoBoletoConfiguracao, bool enviarBoletoPorEmailAutomaticamente, bool enviarDocumentacaoFaturamentoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, int codigoRecebidoIntegracao)
        {
            if (cte.Status != "A")
                return;

            if (cte.ModeloDocumentoFiscal.NaoGerarFaturamento)
                return;

            if (cargaPedido?.Carga?.CargaSVM ?? false)
                return;

            if (cargaPedido?.Carga?.TipoOperacao?.NaoGerarFaturamento ?? false)
                return;

            if (GerarTitulosAutomaticamenteDeAdiantamentoSaldo(cte, cargaPedido, grupoPessoas, tomador, configuracaoFinanceiraFatura, tipoServicoMultisoftware, gerarFaturamentoAVista, gerarBoletoAutomaticamente, gerarTituloBloqueado, codigoBoletoConfiguracao, enviarBoletoPorEmailAutomaticamente, enviarDocumentacaoFaturamentoCTe, Auditado))
                return;

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUso = ObterTipoMovimentoConfiguracaoFinanceiraFatura(cte, null, configuracaoFinanceiraFatura);
            List<int> diasParcela = ObterDiasPrazoVencimento(tomador, null);

            int qtdParcelas = diasParcela.Count;

            decimal valorTotal = Math.Round(cte.ValorAReceber, 2, MidpointRounding.ToEven);
            decimal valorMoedaTotal = Math.Round(cte.ValorTotalMoeda ?? 0m, 2, MidpointRounding.ToEven);

            decimal valorParcelas = qtdParcelas > 1 ? Math.Round(valorTotal / qtdParcelas, 2) : valorTotal;
            decimal valorMoedaParcelas = qtdParcelas > 1 ? Math.Round(valorMoedaTotal / qtdParcelas, 2) : valorMoedaTotal;

            int posParcela = 0;

            decimal valorAcumulado = 0m;
            decimal valorMoedaAcumulado = 0m;

            foreach (int dia in diasParcela)
            {
                posParcela++;

                if (posParcela == qtdParcelas)
                {
                    valorParcelas = valorTotal - valorAcumulado;
                    valorMoedaParcelas = valorMoedaTotal - valorMoedaAcumulado;
                }
                else
                {
                    valorAcumulado += valorParcelas;
                    valorMoedaAcumulado += valorMoedaParcelas;
                }

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                {
                    DataEmissao = cte.DataEmissao,
                    DataVencimento = ObterDataVencimentoTitulo(tomador, null, cte.DataEmissao.Value, _unitOfWork, dia),
                    Sequencia = 1,
                    StatusTitulo = gerarTituloBloqueado ? StatusTitulo.Bloqueado : StatusTitulo.EmAberto,
                    DataAlteracao = DateTime.Now,
                    TipoTitulo = TipoTitulo.Receber,
                    ValorOriginal = valorParcelas,
                    ValorPendente = valorParcelas,
                    ValorTituloOriginal = valorParcelas,
                    Valor = valorParcelas,
                    ValorTotal = valorParcelas,
                    GrupoPessoas = grupoPessoas,
                    Pessoa = tomador,
                    TipoMovimento = tipoMovimentoUso,
                    TipoDocumentoTituloOriginal = "CT-e",
                    NumeroDocumentoTituloOriginal = cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(),
                    Empresa = cte.Empresa,
                    DataLancamento = DateTime.Now,
                    Usuario = cte.Usuario                    
                };

                if (codigoRecebidoIntegracao > 0)
                    titulo.CodigoRecebidoIntegracao = codigoRecebidoIntegracao;

                if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                titulo.DataProgramacaoPagamento = titulo.DataVencimento;
                titulo.FormaTitulo = ObterFormaTituloGrupoPessoa(titulo);

                if (gerarBoletoAutomaticamente)
                {
                    titulo.BoletoStatusTitulo = BoletoStatusTitulo.Emitido;
                    titulo.BoletoConfiguracao = codigoBoletoConfiguracao > 0 ? repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao) : repBoletoConfiguracao.BuscarPrimeiraConfiguracao();
                    titulo.BoletoEnviadoPorEmail = false;
                    titulo.BoletoGeradoAutomaticamente = enviarBoletoPorEmailAutomaticamente;
                }

                titulo.EnviarDocumentacaoFaturamentoCTe = enviarDocumentacaoFaturamentoCTe;

                if (cte.Moeda.HasValue && cte.Moeda != MoedaCotacaoBancoCentral.Real)
                {
                    titulo.MoedaCotacaoBancoCentral = cte.Moeda;
                    titulo.ValorOriginalMoedaEstrangeira = valorMoedaParcelas;
                    titulo.ValorMoedaCotacao = cte.ValorCotacaoMoeda ?? 0m;
                    titulo.DataBaseCRT = cargaPedido?.Pedido?.DataBaseCRT;
                }

                repTitulo.Inserir(titulo);

                if (gerarBoletoAutomaticamente && titulo.BoletoStatusTitulo == BoletoStatusTitulo.Emitido)
                {
                    Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(_unitOfWork);
                    servTitulo.IntegrarEmitido(titulo, _unitOfWork);
                }

                Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento
                {
                    CTe = cte,
                    TipoDocumento = TipoDocumentoTitulo.CTe,
                    Titulo = titulo,
                    Valor = titulo.ValorOriginal,
                    ValorTotal = titulo.ValorOriginal,
                    ValorPendente = titulo.ValorOriginal,
                    ValorMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorTotalMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorPendenteMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorCotacaoMoeda = titulo.ValorMoedaCotacao
                };

                cte.CTePendenteIntegracaoFatura = true;

                repCTe.Atualizar(cte);
                repTituloDocumento.Inserir(tituloDocumento);

                servProcessoMovimento.GerarMovimentacao(tipoMovimentoUso, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Historico, _unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas);

                new Servicos.Embarcador.Integracao.IntegracaoTitulo(_unitOfWork).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Criacao);

                if (gerarFaturamentoAVista)
                    QuitarTitulo(out _, titulo, DateTime.Now.Date, DateTime.Now.Date, _unitOfWork, tomador, grupoPessoas, cargaPedido?.Carga?.Operador, tipoServicoMultisoftware, "", 0m, false, 0m, false, Auditado);
            }
        }

        public static void GerarTituloGNRE(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> configuracaoFinanceiraGNRERegistros, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            if (cargaCTe.CTe.Status != "A" ||
                cargaCTe.CTe.ValorICMS <= 0m ||
                cargaCTe.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento ||
                (cargaPedido?.Carga?.CargaSVM ?? false) ||
                (cargaPedido?.Carga?.TipoOperacao?.NaoGerarFaturamento ?? false))
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro registro = configuracaoFinanceiraGNRERegistros.Where(o => o.CFOP != null && o.Estado != null && o.Estado.Sigla == cargaCTe.CTe.LocalidadeInicioPrestacao.Estado.Sigla && o.CFOP.Codigo == cargaCTe.CTe.CFOP.Codigo).FirstOrDefault();

            if (registro == null)
                registro = configuracaoFinanceiraGNRERegistros.Where(o => o.CFOP == null && o.Estado != null && o.Estado.Sigla == cargaCTe.CTe.LocalidadeInicioPrestacao.Estado.Sigla).FirstOrDefault();

            if (registro == null)
                return;

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unidadeTrabalho);

            decimal valor = cargaCTe.CTe.ValorICMS;

            if (registro.PorcentagemDesconto > 0)
                valor = cargaCTe.CTe.ValorICMS - ((registro.PorcentagemDesconto * cargaCTe.CTe.ValorICMS) / 100);

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
            {
                DataEmissao = cargaCTe.CTe.DataEmissao,
                DataVencimento = cargaCTe.CTe.DataEmissao,
                DataProgramacaoPagamento = cargaCTe.CTe.DataEmissao,
                Sequencia = 1,
                StatusTitulo = StatusTitulo.EmAberto,
                DataAlteracao = DateTime.Now,
                TipoTitulo = TipoTitulo.Pagar,
                ValorOriginal = valor,
                ValorPendente = valor,
                ValorTituloOriginal = valor,
                GrupoPessoas = registro.Pessoa.GrupoPessoas,
                Empresa = cargaCTe.CTe.Empresa,
                Pessoa = registro.Pessoa,
                TipoMovimento = registro.TipoMovimento,
                Valor = valor,
                ValorTotal = valor,
                TipoDocumentoTituloOriginal = "GNRE",
                NumeroDocumentoTituloOriginal = cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(),
                DataLancamento = DateTime.Now,
                Usuario = cargaCTe.Carga?.Operador,
            };

            titulo.Observacao = $"Referente à GNRE do documento {titulo.NumeroDocumentoTituloOriginal}.";

            repTitulo.Inserir(titulo);

            Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento()
            {
                CTe = cargaCTe.CTe,
                TipoDocumento = TipoDocumentoTitulo.CTe,
                Titulo = titulo,
                Valor = titulo.Valor,
                ValorTotal = titulo.Valor
            };

            repTituloDocumento.Inserir(tituloDocumento);

            if (configuracaoFinanceiro.AtivarControleDespesas)
            {
                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(registro.TipoMovimento.Codigo);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = cargaCTe.CTe.CentroResultadoFaturamento;
                if (tipoDespesa != null && centroResultado != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa tituloCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa();
                    tituloCentroResultadoTipoDespesa.Titulo = titulo;
                    tituloCentroResultadoTipoDespesa.TipoDespesaFinanceira = tipoDespesa;
                    tituloCentroResultadoTipoDespesa.CentroResultado = centroResultado;
                    tituloCentroResultadoTipoDespesa.Percentual = 100;

                    repTituloCentroResultadoTipoDespesa.Inserir(tituloCentroResultadoTipoDespesa);
                }
            }

            servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Observacao, unidadeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas);
        }

        public void GerarTituloPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool gerarFaturamentoAVista, bool gerarBoletoAutomaticamente, int codigoBoletoConfiguracao, bool enviarBoletoPorEmailAutomaticamente, bool enviarDocumentacaoFaturamentoCTe, bool pagamentoBloqueado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);
            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = tomador?.GrupoPessoas ?? cargaPedido.Pedido.GrupoPessoas;

            if (GerarTitulosAutomaticamenteDeAdiantamentoSaldo(null, cargaPedido, grupoPessoas, tomador, configuracaoFinanceiraFatura, tipoServicoMultisoftware, gerarFaturamentoAVista, gerarBoletoAutomaticamente, pagamentoBloqueado, codigoBoletoConfiguracao, enviarBoletoPorEmailAutomaticamente, enviarDocumentacaoFaturamentoCTe, Auditado))
                return;

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUso = ObterTipoMovimentoConfiguracaoFinanceiraFatura(null, carga, configuracaoFinanceiraFatura);

            List<int> diasParcela = ObterDiasPrazoVencimento(tomador, cargaPedido);

            int qtdParcelas = diasParcela.Count;

            decimal valorTotal = Math.Round(repCargaCTe.BuscarValorTotalReceberPorCarga(carga.Codigo, "A"), 2, MidpointRounding.ToEven);
            decimal valorMoedaTotal = Math.Round(repCargaCTe.BuscarValorTotalMoedaPorCarga(carga.Codigo, "A") ?? 0m, 2, MidpointRounding.ToEven);

            decimal valorParcelas = qtdParcelas > 1 ? Math.Round(valorTotal / qtdParcelas, 2, MidpointRounding.ToEven) : valorTotal;
            decimal valorMoedaParcelas = qtdParcelas > 1 ? Math.Round(valorMoedaTotal / qtdParcelas, 2, MidpointRounding.ToEven) : valorMoedaTotal;

            int posParcela = 0;

            decimal valorAcumulado = 0m;
            decimal valorAcumuladoMoeda = 0m;

            foreach (var dia in diasParcela)
            {
                posParcela++;

                if (posParcela == qtdParcelas)
                {
                    valorParcelas = valorTotal - valorAcumulado;
                    valorMoedaParcelas = valorMoedaTotal - valorAcumuladoMoeda;
                }

                valorAcumulado += valorParcelas;
                valorAcumuladoMoeda += valorMoedaParcelas;

                DateTime dataEmissao = repCargaCTe.BuscarUltimaDataEmissaoPorCarga(carga.Codigo, "A");
                DateTime dataVencimento = ObterDataVencimentoTitulo(tomador, cargaPedido, dataEmissao, _unitOfWork, dia);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                {
                    DataEmissao = dataEmissao,
                    DataVencimento = dataVencimento,
                    DataProgramacaoPagamento = dataVencimento,
                    DataAlteracao = DateTime.Now,
                    Sequencia = 1,
                    StatusTitulo = pagamentoBloqueado ? StatusTitulo.Bloqueado : StatusTitulo.EmAberto,
                    TipoTitulo = TipoTitulo.Receber,
                    ValorOriginal = valorParcelas,
                    ValorPendente = valorParcelas,
                    ValorTituloOriginal = valorParcelas,
                    Valor = valorParcelas,
                    ValorTotal = valorParcelas,
                    GrupoPessoas = grupoPessoas,
                    Pessoa = tomador,
                    TipoMovimento = tipoMovimentoUso,
                    TipoDocumentoTituloOriginal = "Carga",
                    NumeroDocumentoTituloOriginal = carga.CodigoCargaEmbarcador,
                    Empresa = carga.Empresa,
                    DataLancamento = DateTime.Now,
                    Usuario = carga.Operador,
                };

                if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                titulo.FormaTitulo = ObterFormaTituloGrupoPessoa(titulo);

                if (gerarBoletoAutomaticamente)
                {
                    titulo.BoletoStatusTitulo = BoletoStatusTitulo.Emitido;
                    titulo.BoletoConfiguracao = codigoBoletoConfiguracao > 0 ? repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao) : repBoletoConfiguracao.BuscarPrimeiraConfiguracao();
                    titulo.BoletoGeradoAutomaticamente = enviarBoletoPorEmailAutomaticamente;
                    titulo.BoletoEnviadoPorEmail = false;
                }

                titulo.EnviarDocumentacaoFaturamentoCTe = enviarDocumentacaoFaturamentoCTe;

                if (carga.Moeda.HasValue && carga.Moeda != MoedaCotacaoBancoCentral.Real)
                {
                    titulo.MoedaCotacaoBancoCentral = carga.Moeda;
                    titulo.DataBaseCRT = cargaPedido?.Pedido?.DataBaseCRT;
                    titulo.ValorMoedaCotacao = carga.ValorCotacaoMoeda ?? 0m;
                    titulo.ValorOriginalMoedaEstrangeira = valorMoedaParcelas;
                }

                repTitulo.Inserir(titulo);

                if (gerarBoletoAutomaticamente && titulo.BoletoStatusTitulo == BoletoStatusTitulo.Emitido)
                {
                    Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(_unitOfWork);
                    servTitulo.IntegrarEmitido(titulo, _unitOfWork);
                }

                Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento
                {
                    Carga = carga,
                    TipoDocumento = TipoDocumentoTitulo.Carga,
                    Titulo = titulo,
                    Valor = titulo.ValorOriginal,
                    ValorTotal = titulo.ValorOriginal,
                    ValorPendente = titulo.ValorOriginal,
                    ValorMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorTotalMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorPendenteMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorCotacaoMoeda = titulo.ValorMoedaCotacao
                };

                repTituloDocumento.Inserir(tituloDocumento);

                servProcessoMovimento.GerarMovimentacao(tipoMovimentoUso, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Historico, _unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas);

                if (gerarFaturamentoAVista)
                    QuitarTitulo(out _, titulo, DateTime.Now.Date, DateTime.Now.Date, _unitOfWork, tomador, grupoPessoas, carga.Operador, tipoServicoMultisoftware, "", 0m, false, 0m, false, Auditado);
            }
        }

        public bool GerarTituloPorDocumentoFaturamento(out string erro, int codigoDocumentoFaturamento, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo(codigoDocumentoFaturamento);

            if (documentoFaturamento == null)
            {
                erro = "Documento não encontrado.";
                return false;
            }

            if (documentoFaturamento.Situacao != SituacaoDocumentoFaturamento.Autorizado)
            {
                erro = "O(a) " + documentoFaturamento.DescricaoNumeroDocumento + " não está autorizado(a), não sendo possível gerar o título.";
                return false;
            }

            if (documentoFaturamento.ValorAFaturar <= 0)
            {
                erro = "O(a) " + documentoFaturamento.DescricaoNumeroDocumento + " já possui todo o valor faturado, não sendo possível gerar o título.";
                return false;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUso = ObterTipoMovimentoConfiguracaoFinanceiraFatura(documentoFaturamento.CTe, documentoFaturamento.Carga, configuracaoFinanceiraFatura);
            List<int> diasParcela = ObterDiasPrazoVencimento(documentoFaturamento.Tomador, null);

            int qtdParcelas = diasParcela.Count;

            decimal valorTotal = Math.Round(documentoFaturamento.ValorAFaturar, 2, MidpointRounding.ToEven);
            decimal valorMoedaTotal = Math.Round(documentoFaturamento.ValorTotalMoeda ?? 0m, 2, MidpointRounding.ToEven);

            decimal valorParcelas = qtdParcelas > 1 ? Math.Round(valorTotal / qtdParcelas, 2, MidpointRounding.ToEven) : valorTotal;
            decimal valorMoedaParcelas = qtdParcelas > 1 ? Math.Round(valorMoedaTotal / qtdParcelas, 2, MidpointRounding.ToEven) : valorMoedaTotal;

            int posParcela = 0;

            decimal valorAcumulado = 0m;
            decimal valorMoedaAcumulado = 0m;

            foreach (int dia in diasParcela)
            {
                posParcela++;

                if (posParcela == qtdParcelas)
                {
                    valorParcelas = valorTotal - valorAcumulado;
                    valorMoedaParcelas = valorMoedaTotal - valorMoedaAcumulado;
                }

                valorAcumulado += valorParcelas;
                valorMoedaAcumulado += valorMoedaParcelas;

                DateTime dataEmissao = documentoFaturamento.DataEmissao;
                DateTime dataVencimento = ObterDataVencimentoTitulo(documentoFaturamento.Tomador, null, documentoFaturamento.DataEmissao, _unitOfWork, dia);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                {
                    DataEmissao = dataEmissao,
                    DataVencimento = dataVencimento,
                    DataProgramacaoPagamento = dataVencimento,
                    DataAlteracao = DateTime.Now,
                    Sequencia = 1,
                    StatusTitulo = StatusTitulo.EmAberto,
                    TipoTitulo = TipoTitulo.Receber,
                    ValorOriginal = valorParcelas,
                    ValorPendente = valorParcelas,
                    ValorTituloOriginal = valorParcelas,
                    Valor = valorParcelas,
                    ValorTotal = valorParcelas,
                    GrupoPessoas = documentoFaturamento.GrupoPessoas ?? documentoFaturamento.Tomador?.GrupoPessoas,
                    Pessoa = documentoFaturamento.Tomador,
                    TipoMovimento = tipoMovimentoUso,
                    DataLancamento = DateTime.Now,
                    Usuario = usuario,
                };

                titulo.FormaTitulo = ObterFormaTituloGrupoPessoa(titulo);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = tipoAmbiente;

                if (documentoFaturamento.Moeda.HasValue && documentoFaturamento.Moeda != MoedaCotacaoBancoCentral.Real)
                {
                    titulo.MoedaCotacaoBancoCentral = documentoFaturamento.Moeda;
                    titulo.ValorMoedaCotacao = documentoFaturamento.ValorCotacaoMoeda ?? 0m;
                    titulo.ValorOriginalMoedaEstrangeira = valorMoedaParcelas;
                }

                repTitulo.Inserir(titulo, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento();

                if (documentoFaturamento.TipoDocumento == TipoDocumentoFaturamento.Carga)
                {
                    tituloDocumento.Carga = documentoFaturamento.Carga;
                    tituloDocumento.TipoDocumento = TipoDocumentoTitulo.Carga;
                }
                else
                {
                    tituloDocumento.CTe = documentoFaturamento.CTe;
                    tituloDocumento.TipoDocumento = TipoDocumentoTitulo.CTe;
                }

                tituloDocumento.Titulo = titulo;
                tituloDocumento.Valor = valorParcelas;
                tituloDocumento.ValorTotal = valorParcelas;
                tituloDocumento.ValorPendente = valorParcelas;
                tituloDocumento.ValorMoeda = valorMoedaParcelas;
                tituloDocumento.ValorTotalMoeda = valorMoedaParcelas;
                tituloDocumento.ValorPendenteMoeda = valorMoedaParcelas;

                repTituloDocumento.Inserir(tituloDocumento, Auditado);

                documentoFaturamento.ValorEmFatura += valorParcelas;
                documentoFaturamento.ValorAFaturar -= valorParcelas;
                documentoFaturamento.Titulo = titulo;
                documentoFaturamento.DataVencimento = titulo.DataVencimento;

                if (posParcela == qtdParcelas)
                    documentoFaturamento.ValorAFaturar = 0m;

                repDocumentoFaturamento.Atualizar(documentoFaturamento, Auditado);

                if (!servProcessoMovimento.GerarMovimentacao(out erro, tipoMovimentoUso, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Historico, _unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas))
                    return false;
            }

            erro = "";
            return true;
        }

        public static bool QuitarTituloAPagar(out string erro, Dominio.Entidades.Embarcador.Financeiro.Titulo tituloAbaixar, DateTime dataBaixa, DateTime dataBase, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string observacao, decimal valorAcrescimo, bool modeloAntigo, decimal valorDesconto, bool aplicarAcrescimoPrimeiroTitulo, string codigoIntegracaoFormaPagamento)
        {
            try
            {
                erro = string.Empty;
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeTrabalho.StringConexao);
                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unidadeTrabalho);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento formaPagamento = repTipoPagamentoRecebimento.BuscarPorCodigoIntegracao(codigoIntegracaoFormaPagamento);

                if (formaPagamento == null)
                {
                    erro = "Não foi localizado uma forma de pagamento/recebimento cadastrado.";
                    return false;
                }

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(tituloAbaixar.Codigo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();


                tituloBaixa.DataBaixa = dataBaixa;
                tituloBaixa.DataBase = dataBase;
                tituloBaixa.DataOperacao = DateTime.Now;
                tituloBaixa.Numero = 1;
                tituloBaixa.Observacao = !string.IsNullOrWhiteSpace(observacao) ? observacao : "" + (grupoPessoas != null ? grupoPessoas.Descricao : cliente.Nome) + " - TÍTULO Nº " + tituloAbaixar.Codigo.ToString("D") + " (" + dataBaixa.ToString("dd/MM/yyyy") + ")";
                tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada;
                tituloBaixa.Sequencia = 1;
                tituloBaixa.Valor = titulo.ValorOriginal;
                tituloBaixa.ValorTotalAPagar = titulo.ValorTotal;
                tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                tituloBaixa.Pessoa = cliente;
                tituloBaixa.GrupoPessoas = grupoPessoas;
                tituloBaixa.TipoPagamentoRecebimento = formaPagamento;
                tituloBaixa.Usuario = usuario;

                repTituloBaixa.Inserir(tituloBaixa);

                if (titulo.DataEmissao.Value.Date > tituloBaixa.DataBaixa.Value.Date)
                {
                    erro = "O título " + titulo.Codigo.ToString() + " possui a data de emissão maior que a data da baixa.";
                    unidadeTrabalho.Rollback();
                    return false;
                }

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento tipoPagamentoRecebimento = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento()
                {
                    DataBaseCRT = dataBaixa,
                    MoedaCotacaoBancoCentral = MoedaCotacaoBancoCentral.Real,
                    TipoPagamentoRecebimento = formaPagamento,
                    TituloBaixa = tituloBaixa,
                    Valor = titulo.ValorOriginal + valorAcrescimo - valorDesconto,
                    ValorMoedaCotacao = 0,
                    ValorOriginalMoedaEstrangeira = titulo.ValorOriginal + valorAcrescimo - valorDesconto
                };

                repTituloBaixaTipoPagamentoRecebimento.Inserir(tipoPagamentoRecebimento);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                tituloBaixaAgrupado.Titulo = titulo;
                tituloBaixaAgrupado.TituloBaixa = tituloBaixa;
                tituloBaixaAgrupado.ValorPago = titulo.ValorOriginal + valorAcrescimo - valorDesconto;
                tituloBaixaAgrupado.ValorTotalAPagar = titulo.ValorOriginal + valorAcrescimo - valorDesconto;
                tituloBaixaAgrupado.DataBase = dataBase;
                tituloBaixaAgrupado.DataBaixa = dataBaixa;

                repTituloBaixaAgrupado.Inserir(tituloBaixaAgrupado);

                tituloBaixa = repTituloBaixa.BuscarPorCodigo(tituloBaixa.Codigo);
                tituloBaixa.TipoPagamentoRecebimento = formaPagamento;
                tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
                tituloBaixa.ModeloAntigo = true;
                repTituloBaixa.Atualizar(tituloBaixa);

                int codigoTituloBaixa = tituloBaixa.Codigo;

                titulo.DataLiquidacao = dataBaixa;
                titulo.DataBaseLiquidacao = dataBase;
                titulo.Desconto = valorDesconto;
                titulo.Acrescimo = valorAcrescimo;
                titulo.ValorPago = titulo.ValorOriginal + valorAcrescimo - valorDesconto;
                titulo.ValorPendente = 0;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                titulo.DataAlteracao = DateTime.Now;
                repTitulo.Atualizar(titulo);

                if (valorAcrescimo > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;
                    if (titulo != null && titulo.BoletoConfiguracao != null && titulo.BoletoConfiguracao.TipoMovimentoJuros != null)
                        justificativa = repJustificativa.BuscarPorTipoMovimento(titulo.BoletoConfiguracao.TipoMovimentoJuros.Codigo, TipoJustificativa.Acrescimo);

                    if (justificativa == null)
                        justificativa = repJustificativa.BuscarPrimeiraJustificativa(TipoJustificativa.Acrescimo);

                    if (justificativa != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimo = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo()
                        {
                            Justificativa = justificativa,
                            DataBaseCRT = null,
                            MoedaCotacaoBancoCentral = MoedaCotacaoBancoCentral.Real,
                            TituloBaixa = tituloBaixa,
                            Valor = valorAcrescimo,
                            ValorMoedaCotacao = 0m,
                            ValorOriginalMoedaEstrangeira = 0m
                        };

                        repTituloBaixaAcrescimo.Inserir(acrescimo);
                    }
                }

                if (valorDesconto > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;
                    if (titulo != null && titulo.BoletoConfiguracao != null && titulo.BoletoConfiguracao.TipoMovimentoDesconto != null)
                        justificativa = repJustificativa.BuscarPorTipoMovimento(titulo.BoletoConfiguracao.TipoMovimentoDesconto.Codigo, TipoJustificativa.Desconto);

                    if (justificativa == null)
                        justificativa = repJustificativa.BuscarPrimeiraJustificativa(TipoJustificativa.Desconto);

                    if (justificativa != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimo = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo()
                        {
                            Justificativa = justificativa,
                            DataBaseCRT = null,
                            MoedaCotacaoBancoCentral = MoedaCotacaoBancoCentral.Real,
                            TituloBaixa = tituloBaixa,
                            Valor = valorDesconto,
                            ValorMoedaCotacao = 0m,
                            ValorOriginalMoedaEstrangeira = 0m
                        };

                        repTituloBaixaAcrescimo.Inserir(acrescimo);
                    }
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                if (configuracaoFinanceiro.AtivarControleDespesas)
                    servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraControleDespesas(codigoTituloBaixa, unidadeTrabalho, tipoServicoMultisoftware, false);
                else if (configuracaoTMS.GerarMovimentacaoNaBaixaIndividualmente)
                {
                    if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraIndividual(out erro, codigoTituloBaixa, unidadeTrabalho, unidadeTrabalho.StringConexao, tipoServicoMultisoftware, false, null))
                    {
                        unidadeTrabalho.Rollback();
                        return false;
                    }
                }
                else if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out erro, codigoTituloBaixa, unidadeTrabalho, unidadeTrabalho.StringConexao, tipoServicoMultisoftware, false, null))
                {
                    unidadeTrabalho.Rollback();
                    return false;
                }

                unidadeTrabalho.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                erro = "Problemas na quitação do título de forma automática";
                return false;
            }
        }

        public static bool QuitarTitulo(out string erro, Dominio.Entidades.Embarcador.Financeiro.Titulo tituloAbaixar, DateTime dataBaixa, DateTime dataBase, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string observacao, decimal valorAcrescimo, bool modeloAntigo, decimal valorDesconto, bool aplicarAcrescimoPrimeiroTitulo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, string codigoIntegracaoFormaPagamento = null)
        {
            try
            {
                erro = string.Empty;
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeTrabalho.StringConexao);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unidadeTrabalho);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(tituloAbaixar.Codigo);
                titulo.Initialize();
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento formaPagamento = null;

                if(!string.IsNullOrEmpty(codigoIntegracaoFormaPagamento))
                    formaPagamento = repTipoPagamentoRecebimento.BuscarPorCodigoIntegracao(codigoIntegracaoFormaPagamento);

                if (titulo.BoletoConfiguracao != null && titulo.BoletoConfiguracao.PlanoConta != null)
                    formaPagamento = repTipoPagamentoRecebimento.BuscarPorPlanoContas(titulo.BoletoConfiguracao.PlanoConta.Codigo, 0);
                if (formaPagamento == null && usuario != null && usuario.Empresa != null && usuario.Empresa.TipoPagamentoRecebimento != null)
                    formaPagamento = usuario.Empresa.TipoPagamentoRecebimento;
                if (formaPagamento == null)
                    formaPagamento = repTipoPagamentoRecebimento.BuscarFormaAVista();                

                if (formaPagamento == null)
                {
                    erro = "Não foi localizado uma forma de pagamento/recebimento cadastrado.";
                    return false;
                }

                tituloBaixa.DataBaixa = dataBaixa;
                tituloBaixa.DataBase = dataBase;
                tituloBaixa.DataOperacao = DateTime.Now;
                tituloBaixa.Numero = 1;
                tituloBaixa.Observacao = !string.IsNullOrWhiteSpace(observacao) ? observacao : "" + (grupoPessoas != null ? grupoPessoas.Descricao : cliente.Nome) + " - TÍTULO Nº " + tituloAbaixar.Codigo.ToString("D") + " (" + dataBaixa.ToString("dd/MM/yyyy") + ")";
                tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada;
                tituloBaixa.Sequencia = 1;
                tituloBaixa.Valor = titulo.ValorOriginal;// + valorAcrescimo;
                tituloBaixa.ValorTotalAPagar = titulo.ValorTotal;// + valorAcrescimo;
                tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                tituloBaixa.Pessoa = cliente;
                tituloBaixa.GrupoPessoas = grupoPessoas;

                tituloBaixa.TipoPagamentoRecebimento = formaPagamento;
                tituloBaixa.Usuario = usuario;

                repTituloBaixa.Inserir(tituloBaixa, Auditado);

                if (titulo.DataEmissao.Value.Date > tituloBaixa.DataBaixa.Value.Date)
                {
                    erro = "O título " + titulo.Codigo.ToString() + " possui a data de emissão maior que a data da baixa.";
                    return false;
                }

                bool contemDocumentos = titulo.Documentos != null && titulo.Documentos.Count > 0;
                if (!contemDocumentos)
                    contemDocumentos = repTituloDocumento.ContemDocumentosNoTitulo(titulo.Codigo);

                if (!modeloAntigo || contemDocumentos)
                {
                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(tituloBaixa.Codigo);

                    //tituloBaixa.ValorTotalAPagar += valorAcrescimo - valorDesconto;

                    Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarTituloABaixa(tituloBaixa, titulo.Codigo, unidadeTrabalho, usuario, valorAcrescimo, valorDesconto, aplicarAcrescimoPrimeiroTitulo);
                    Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unidadeTrabalho, true);

                    tituloBaixa.DataEmissao = repTituloBaixaAgrupado.BuscarMaiorDataEmissaoPorTituloBaixa(tituloBaixa.Codigo);

                    List<int> faturas = repTituloBaixaAgrupado.BuscarCodigoFaturasPorTituloBaixa(tituloBaixa.Codigo);
                    List<double> tomadores = repTituloBaixaAgrupado.BuscarTomadoresPorTituloBaixa(tituloBaixa.Codigo);
                    List<int> grupoPessoasBaixa = repTituloBaixaAgrupado.BuscarGrupoPessoasPorTituloBaixa(tituloBaixa.Codigo);

                    if (faturas.Count == 1)
                        tituloBaixa.Fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura() { Codigo = faturas[0] };

                    if (tomadores.Count == 1)
                        tituloBaixa.Pessoa = new Dominio.Entidades.Cliente() { CPF_CNPJ = tomadores[0] };

                    if (grupoPessoasBaixa.Count == 1)
                        tituloBaixa.GrupoPessoas = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = grupoPessoasBaixa[0] };

                    tituloBaixa.TipoPagamentoRecebimento = formaPagamento;
                    tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmFinalizacao;

                    repTituloBaixa.Atualizar(tituloBaixa);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                    tituloBaixaAgrupado.Titulo = titulo;
                    tituloBaixaAgrupado.TituloBaixa = tituloBaixa;
                    tituloBaixaAgrupado.ValorPago = titulo.ValorOriginal + valorAcrescimo - valorDesconto;
                    tituloBaixaAgrupado.ValorTotalAPagar = titulo.ValorOriginal + valorAcrescimo - valorDesconto;
                    tituloBaixaAgrupado.DataBase = dataBase;
                    tituloBaixaAgrupado.DataBaixa = dataBaixa;

                    repTituloBaixaAgrupado.Inserir(tituloBaixaAgrupado);

                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(tituloBaixa.Codigo);
                    tituloBaixa.TipoPagamentoRecebimento = formaPagamento;
                    tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
                    tituloBaixa.ModeloAntigo = true;
                    repTituloBaixa.Atualizar(tituloBaixa);

                    titulo.DataLiquidacao = dataBaixa;
                    titulo.DataBaseLiquidacao = dataBase;
                    titulo.Desconto = valorDesconto;
                    titulo.Acrescimo = valorAcrescimo;
                    titulo.ValorPago = titulo.ValorOriginal + valorAcrescimo - valorDesconto;
                    titulo.ValorPendente = 0;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                    titulo.DataAlteracao = DateTime.Now;

                    repTitulo.Atualizar(titulo, Auditado);

                    if (valorAcrescimo > 0)
                    {
                        Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;
                        if (titulo != null && titulo.BoletoConfiguracao != null && titulo.BoletoConfiguracao.TipoMovimentoJuros != null)
                            justificativa = repJustificativa.BuscarPorTipoMovimento(titulo.BoletoConfiguracao.TipoMovimentoJuros.Codigo, TipoJustificativa.Acrescimo);

                        if (justificativa == null)
                            justificativa = repJustificativa.BuscarPrimeiraJustificativa(TipoJustificativa.Acrescimo);

                        if (justificativa != null)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimo = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo()
                            {
                                Justificativa = justificativa,
                                DataBaseCRT = null,
                                MoedaCotacaoBancoCentral = MoedaCotacaoBancoCentral.Real,
                                TituloBaixa = tituloBaixa,
                                Valor = valorAcrescimo,
                                ValorMoedaCotacao = 0m,
                                ValorOriginalMoedaEstrangeira = 0m
                            };

                            repTituloBaixaAcrescimo.Inserir(acrescimo);
                        }
                    }

                    if (valorDesconto > 0)
                    {
                        Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;
                        if (titulo != null && titulo.BoletoConfiguracao != null && titulo.BoletoConfiguracao.TipoMovimentoDesconto != null)
                            justificativa = repJustificativa.BuscarPorTipoMovimento(titulo.BoletoConfiguracao.TipoMovimentoDesconto.Codigo, TipoJustificativa.Desconto);

                        if (justificativa == null)
                            justificativa = repJustificativa.BuscarPrimeiraJustificativa(TipoJustificativa.Desconto);

                        if (justificativa != null)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimo = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo()
                            {
                                Justificativa = justificativa,
                                DataBaseCRT = null,
                                MoedaCotacaoBancoCentral = MoedaCotacaoBancoCentral.Real,
                                TituloBaixa = tituloBaixa,
                                Valor = valorDesconto,
                                ValorMoedaCotacao = 0m,
                                ValorOriginalMoedaEstrangeira = 0m
                            };

                            repTituloBaixaAcrescimo.Inserir(acrescimo);
                        }
                    }

                    if (!servBaixaTituloReceber.GeraReverteMovimentacaoFinanceira(out erro, tituloBaixa.Codigo, unidadeTrabalho, unidadeTrabalho.StringConexao, tipoServicoMultisoftware, false, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                erro = "Problemas na quitação do título de forma automática";
                return false;
            }
        }

        public static bool LimparDadosBoleto(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            listaTitulo.Add(titulo);

            RemoverDadosBoletos(listaTitulo, usuario, unitOfWork, Auditado);
            return true;
        }

        public static bool LimparDadosBoleto(List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            RemoverDadosBoletos(listaTitulo, usuario, unitOfWork, Auditado);
            return true;
        }

        public static List<int> ObterDiasPrazoVencimento(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            List<int> diasPrazoVencimento = new List<int>();
            bool achouConfiguracao = false;

            if (cargaPedido != null && (cargaPedido.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false))
            {
                achouConfiguracao = true;
                if ((cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.GerarFaturamentoMultiplaParcela ?? false) 
                    && !string.IsNullOrWhiteSpace(cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.QuantidadeParcelasFaturamento ?? ""))
                {
                    var diasConfigurado = cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.QuantidadeParcelasFaturamento.Split('.');
                    foreach (var dia in diasConfigurado)
                    {
                        int.TryParse(dia, out int diaConvertido);
                        if (diaConvertido > 0)
                            diasPrazoVencimento.Add(diaConvertido);
                    }
                }
                else if (cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasDePrazoFatura.HasValue == true)
                    diasPrazoVencimento.Add(cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.DiasDePrazoFatura.Value);
                else
                    diasPrazoVencimento.Add(0);

            }
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoFaturaGrupo || tomador.GrupoPessoas == null)
                {
                    achouConfiguracao = true;
                    if (tomador.GerarFaturamentoMultiplaParcela && !string.IsNullOrWhiteSpace(tomador.QuantidadeParcelasFaturamento))
                    {
                        var diasConfigurado = tomador.QuantidadeParcelasFaturamento.Split('.');
                        foreach (var dia in diasConfigurado)
                        {
                            int.TryParse(dia, out int diaConvertido);
                            if (diaConvertido > 0)
                                diasPrazoVencimento.Add(diaConvertido);
                        }
                    }
                    else if (tomador.DiasDePrazoFatura.HasValue)
                        diasPrazoVencimento.Add(tomador.DiasDePrazoFatura.Value);
                    else
                        diasPrazoVencimento.Add(0);

                }
                else if (tomador.GrupoPessoas != null)
                {
                    achouConfiguracao = true;
                    if (tomador.GrupoPessoas.GerarFaturamentoMultiplaParcela && !string.IsNullOrWhiteSpace(tomador.GrupoPessoas.QuantidadeParcelasFaturamento))
                    {
                        var diasConfigurado = tomador.GrupoPessoas.QuantidadeParcelasFaturamento.Split('.');
                        foreach (var dia in diasConfigurado)
                        {
                            int.TryParse(dia, out int diaConvertido);
                            if (diaConvertido > 0)
                                diasPrazoVencimento.Add(diaConvertido);
                        }
                    }
                    else if (tomador.GrupoPessoas.DiasDePrazoFatura.HasValue)
                        diasPrazoVencimento.Add(tomador.GrupoPessoas.DiasDePrazoFatura.Value);
                    else
                        diasPrazoVencimento.Add(0);
                }
            }

            if (!achouConfiguracao)
            {
                if (cargaPedido != null && cargaPedido.Pedido.GrupoPessoas != null)
                {
                    if (cargaPedido.Pedido.GrupoPessoas.GerarFaturamentoMultiplaParcela && !string.IsNullOrWhiteSpace(cargaPedido.Pedido.GrupoPessoas.QuantidadeParcelasFaturamento))
                    {
                        var diasConfigurado = cargaPedido.Pedido.GrupoPessoas.QuantidadeParcelasFaturamento.Split('.');
                        foreach (var dia in diasConfigurado)
                        {
                            int.TryParse(dia, out int diaConvertido);
                            if (diaConvertido > 0)
                                diasPrazoVencimento.Add(diaConvertido);
                        }
                    }
                    else if (cargaPedido.Pedido.GrupoPessoas.DiasDePrazoFatura.HasValue)
                        diasPrazoVencimento.Add(cargaPedido.Pedido.GrupoPessoas.DiasDePrazoFatura.Value);
                    else
                        diasPrazoVencimento.Add(0);

                }
            }

            return diasPrazoVencimento.Distinct().ToList();
        }

        public void RemoverTituloBloqueioFinanceiroPessoa(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Cliente pessoa = titulo.Pessoa;
            if (pessoa == null || titulo.TipoTitulo == TipoTitulo.Pagar || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

            bool tituloExiste = pessoa.TitulosBloqueioFinanceiro?.Where(o => o.Codigo == titulo.Codigo)?.Any() ?? false;
            if (tituloExiste)
            {
                pessoa.TitulosBloqueioFinanceiro.Remove(titulo);

                if (pessoa.TitulosBloqueioFinanceiro.Count == 0)
                    pessoa.SituacaoFinanceira = SituacaoFinanceira.Liberada;
                pessoa.DataUltimaAtualizacao = DateTime.Now;
                pessoa.Integrado = false;
                repCliente.Atualizar(pessoa);
            }
            if (pessoa.GrupoPessoas != null)
            {
                if (!repGrupoPessoas.ContemPessoasBloqueadasFinanceiramento(pessoa.GrupoPessoas.Codigo))
                {
                    pessoa.GrupoPessoas.SituacaoFinanceira = SituacaoFinanceira.Liberada;
                    repGrupoPessoas.Atualizar(pessoa.GrupoPessoas);
                }
            }
        }

        public static void AnularTituloCancelamentoCargaAnulacao(int codigoCarga, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = codigoCarga > 0 ? repCargaCTe.BuscarPrimeirPorCarga(codigoCarga) : null;

            if (cargaCTe != null && cargaCTe.CTe != null && cargaCTe.CTe.Titulo != null)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cargaCTe.CTe.Titulo;

                if (titulo != null)
                {
                    if (titulo.StatusTitulo != StatusTitulo.Cancelado)
                    {
                        titulo.StatusTitulo = StatusTitulo.Cancelado;
                        repTitulo.Atualizar(titulo);

                        new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Cancelamento);

                        Auditoria.Auditoria.Auditar(auditado, titulo, null, "Título cancelado através do Cancelamento Carga Anulação", unitOfWork);
                    }
                }
            }
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimento ObterTipoMovimentoConfiguracaoFinanceiraFatura(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura, bool reversao = false)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento repConfiguracaoFinanceiraFaturaModeloTipoMovimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao repConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoPadrao = reversao ? configuracaoFinanceiraFatura.TipoMovimentoReversao : configuracaoFinanceiraFatura.TipoMovimentoUso;

            if (cte == null && carga != null)
                cte = repCargaCTe.BuscarPrimeiroCTePorCarga(carga.Codigo);

            if (cte == null)
                return tipoMovimentoPadrao;

            if (reversao)
            {
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = configuracaoFinanceiraFatura.GeracaoMovimentoFinanceiroPorModeloDocumentoReversao ? repConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao.BuscarTipoMovimentoPorModeloDocumento(cte.ModeloDocumentoFiscal.Codigo) : null;
                if (tipoMovimento != null)
                    return tipoMovimento;
            }
            else
            {
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = configuracaoFinanceiraFatura.GeracaoMovimentoFinanceiroPorModeloDocumento ? repConfiguracaoFinanceiraFaturaModeloTipoMovimento.BuscarTipoMovimentoPorModeloDocumento(cte.ModeloDocumentoFiscal.Codigo) : null;
                if (tipoMovimento != null)
                    return tipoMovimento;
            }

            return tipoMovimentoPadrao;
        }

        public FormaTitulo ObterFormaTituloGrupoPessoa(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            if (titulo.FaturaCargaDocumento?.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false && titulo.FaturaCargaDocumento.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.FormaTitulo.HasValue)
                return titulo.FaturaCargaDocumento.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.FormaTitulo.Value;
            else if (titulo.Pessoa != null && titulo.Pessoa.NaoUsarConfiguracaoFaturaGrupo && titulo.Pessoa.FormaTitulo.HasValue)
                return titulo.Pessoa.FormaTitulo.Value;
            else if (titulo.GrupoPessoas != null && titulo.GrupoPessoas.FormaTitulo.HasValue)
                return titulo.GrupoPessoas.FormaTitulo.Value;
            else if (titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null && titulo.Pessoa.GrupoPessoas.FormaTitulo.HasValue)
                return titulo.Pessoa.GrupoPessoas.FormaTitulo.Value;

            return FormaTitulo.Outros;
        }

        public List<(int Dia, decimal Percentual)> ObterDiasPrazoVencimentoAdiantamentoSaldo(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            List<(int Dia, decimal Percentual)> diasPercentualPrazoVencimento = new List<(int dia, decimal percentual)>();
            bool achouConfiguracao = false;

            if (cargaPedido != null && (cargaPedido.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false))
            {
                achouConfiguracao = true;
                if ((cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.PercentualAdiantamentoTituloAutomatico ?? 0) > 0)
                    diasPercentualPrazoVencimento.Add(ValueTuple.Create(cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.PrazoAdiantamentoEmDiasTituloAutomatico,
                                                                        cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.PercentualAdiantamentoTituloAutomatico));
                if ((cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.PercentualSaldoTituloAutomatico ?? 0 ) > 0)
                    diasPercentualPrazoVencimento.Add(ValueTuple.Create(cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.PrazoSaldoEmDiasTituloAutomatico,
                                                                        cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.PercentualSaldoTituloAutomatico));
            }
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoFaturaGrupo || tomador.GrupoPessoas == null)
                {
                    achouConfiguracao = true;

                    if (tomador.ConfiguracaoFatura.PercentualAdiantamentoTituloAutomatico > 0)
                        diasPercentualPrazoVencimento.Add(ValueTuple.Create(tomador.ConfiguracaoFatura.PrazoAdiantamentoEmDiasTituloAutomatico, tomador.ConfiguracaoFatura.PercentualAdiantamentoTituloAutomatico));
                    if (tomador.ConfiguracaoFatura.PercentualSaldoTituloAutomatico > 0)
                        diasPercentualPrazoVencimento.Add(ValueTuple.Create(tomador.ConfiguracaoFatura.PrazoSaldoEmDiasTituloAutomatico, tomador.ConfiguracaoFatura.PercentualSaldoTituloAutomatico));
                }
                else if (tomador.GrupoPessoas != null)
                {
                    achouConfiguracao = true;

                    if (tomador.GrupoPessoas.ConfiguracaoFatura.PercentualAdiantamentoTituloAutomatico > 0)
                        diasPercentualPrazoVencimento.Add(ValueTuple.Create(tomador.GrupoPessoas.ConfiguracaoFatura.PrazoAdiantamentoEmDiasTituloAutomatico, tomador.GrupoPessoas.ConfiguracaoFatura.PercentualAdiantamentoTituloAutomatico));
                    if (tomador.GrupoPessoas.ConfiguracaoFatura.PercentualSaldoTituloAutomatico > 0)
                        diasPercentualPrazoVencimento.Add(ValueTuple.Create(tomador.GrupoPessoas.ConfiguracaoFatura.PrazoSaldoEmDiasTituloAutomatico, tomador.GrupoPessoas.ConfiguracaoFatura.PercentualSaldoTituloAutomatico));
                }
            }

            if (!achouConfiguracao)
            {
                if (cargaPedido?.Pedido.GrupoPessoas != null)
                {
                    if (cargaPedido.Pedido.GrupoPessoas.ConfiguracaoFatura.PercentualAdiantamentoTituloAutomatico > 0)
                        diasPercentualPrazoVencimento.Add(ValueTuple.Create(cargaPedido.Pedido.GrupoPessoas.ConfiguracaoFatura.PrazoAdiantamentoEmDiasTituloAutomatico, cargaPedido.Pedido.GrupoPessoas.ConfiguracaoFatura.PercentualAdiantamentoTituloAutomatico));
                    if (cargaPedido.Pedido.GrupoPessoas.ConfiguracaoFatura.PercentualSaldoTituloAutomatico > 0)
                        diasPercentualPrazoVencimento.Add(ValueTuple.Create(cargaPedido.Pedido.GrupoPessoas.ConfiguracaoFatura.PrazoSaldoEmDiasTituloAutomatico, cargaPedido.Pedido.GrupoPessoas.ConfiguracaoFatura.PercentualSaldoTituloAutomatico));
                }
            }

            return diasPercentualPrazoVencimento;
        }

        public bool PossuiGeracaoTituloAdiantamentoSaldo(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas)
        {
            if (tomador?.NaoUsarConfiguracaoFaturaGrupo ?? false)
                return tomador.ConfiguracaoFatura?.GerarTituloAutomaticamenteComAdiantamentoSaldo ?? false;
            if (grupoPessoas != null)
                return grupoPessoas.ConfiguracaoFatura?.GerarTituloAutomaticamenteComAdiantamentoSaldo ?? false;

            return false;
        }

        public void CancelarTitulo(int codigo, string motivo = "", int codigoJustificativaCancelamento = 0)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(_unitOfWork);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro repJustificativaCancelamentoFinanceiro = new Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro(_unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigo);
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo pagamentoEletronicoTitulo = repPagamentoEletronicoTitulo.BuscarPorTitulo(titulo.Codigo);

            if (pagamentoEletronicoTitulo != null)
                throw new ServicoException("Não é possível cancelar o título pois o mesmo já possuí um arquivo de pagamento eletrônico.");

            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (titulo.ConhecimentoDeTransporteEletronico != null)
                    throw new ServicoException("Este título possui um conhecimento vinculado, impossível de cancelar o mesmo.");

                if (titulo.ContratoFrete != null)
                    throw new ServicoException("Este título possui um contrato de frete vinculado, impossível de cancelar o mesmo.");

                if (titulo.DuplicataDocumentoEntrada != null)
                    throw new ServicoException("Este título possui um documento de entrada vinculado, impossível de cancelar o mesmo.");

                if (titulo.FaturaParcela != null)
                    throw new ServicoException("Este título possui uma fatura vinculada, impossível de cancelar o mesmo.");

                if (titulo.NotaFiscal != null)
                    throw new ServicoException("Este título possui uma nota fiscal de saída vinculado, impossível de cancelar o mesmo.");

                if (titulo.TituloBaixaNegociacao != null)
                    throw new ServicoException("Este título é referente à uma renegociação, não sendo possível cancelar o mesmo.");
            }

            if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado)
                throw new ServicoException("Este título já se encontra cancelado.");

            if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                throw new ServicoException("Este título se encontra quitado, favor reverta o mesmo.");

            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (titulo.TipoMovimento == null)
                    throw new ServicoException("Este título não possui um tipo de movimento vinculado, impossível de cancelar o mesmo.");

                if (titulo.TipoMovimento.PlanoDeContaDebito == null)
                    throw new ServicoException("Este título não possui um tipo de movimento vinculado, impossível de cancelar o mesmo.");

                if (titulo.TipoMovimento.PlanoDeContaCredito == null)
                    throw new ServicoException("Este título não possui um tipo de movimento vinculado, impossível de cancelar o mesmo.");
            }

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorTitulo(codigo);
            if (tituloBaixa != null)
                throw new ServicoException("Este título está vinculado a uma baixa, favor cancelar a mesma para efetuar esse procedimento.");

            Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorTitulo(codigo);
            if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                throw new ServicoException("Este título possui um Faturamento Mensal vinculado, favor Finalizar o mesmo antes de cancelar.");

            List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> lancamentosCentroResultado = repLancamentoCentroResultado.BuscarPorTitulo(titulo.Codigo);


            if (titulo.TipoMovimento != null)
            {
                if (lancamentosCentroResultado.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado in lancamentosCentroResultado)
                    {
                        //Reverte o valor antigo
                        if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, null, titulo.DataEmissao.Value.Date, lancamentoCentroResultado.Valor, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO MANUAL ", _unitOfWork, TipoDocumentoMovimento.Manual, _tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, lancamentoCentroResultado.CentroResultado, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                            throw new ControllerException(msgErro);
                    }
                }
                else
                {
                    if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, null, titulo.DataEmissao.Value.Date, titulo.ValorOriginal, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO MANUAL", _unitOfWork, TipoDocumentoMovimento.Manual, _tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                        throw new ControllerException(msgErro);
                }
            }

            if (lancamentosCentroResultado.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado in lancamentosCentroResultado)
                {
                    lancamentoCentroResultado.Ativo = false;

                    repLancamentoCentroResultado.Atualizar(lancamentoCentroResultado);
                }
            }

            if (titulo.TipoTitulo == TipoTitulo.Receber)
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento in titulo.Documentos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;

                    if (tituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.Carga)
                        documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(tituloDocumento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                    else
                        documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(tituloDocumento.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);

                    documentoFaturamento.ValorAFaturar += tituloDocumento.ValorTotal;
                    documentoFaturamento.ValorEmFatura -= tituloDocumento.ValorTotal;

                    repDocumentoFaturamento.Atualizar(documentoFaturamento);
                }
            }

            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
            titulo.DataCancelamento = DateTime.Now.Date;
            titulo.DataAlteracao = DateTime.Now;
            titulo.MotivoCancelamento = motivo;
            titulo.JustificativaCancelamento = codigoJustificativaCancelamento > 0 ? repJustificativaCancelamentoFinanceiro.BuscarPorCodigo(codigoJustificativaCancelamento, false) : null;

            Servicos.Auditoria.Auditoria.Auditar(_auditado, titulo, null, "Atualizado título.", _unitOfWork);
            repTitulo.Atualizar(titulo);

            Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = repRateioDespesaVeiculo.BuscarPorTitulo(codigo);

            if (rateioDespesaVeiculo != null)
            {
                Servicos.Auditoria.Auditoria.Auditar(_auditado, rateioDespesaVeiculo, null, "Excluido e revertido o rateio de despesa do veículo a partir do título financeiro", _unitOfWork);

                rateioDespesaVeiculo.Veiculos = null;
                rateioDespesaVeiculo.SegmentosVeiculos = null;
                rateioDespesaVeiculo.CentroResultados = null;

                repRateioDespesaVeiculoLancamento.DeletarPorRateioDespesaVeiculo(rateioDespesaVeiculo.Codigo);
                repRateioDespesaVeiculo.Deletar(rateioDespesaVeiculo);
            }

            RemoverTituloBloqueioFinanceiroPessoa(titulo, _tipoServicoMultisoftware);

            new Servicos.Embarcador.Integracao.IntegracaoTitulo(_unitOfWork).IniciarIntegracoesDeTitulos(titulo, TipoAcaoIntegracao.Cancelamento);
            new Servicos.Embarcador.Integracao.IntegracaoTitulo(_unitOfWork).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Cancelamento);
        }

        public void IntegrarEmitido (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho); ;

            if (repTipoIntegracao.BuscarTipos().Exists(tipo => tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeTrabalho);
                var integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

                if (integracaoIntercab.AtivarIntegracaoFatura)
                {
                    Servicos.Embarcador.Integracao.IntegracaoTitulo servIntegracaoTitulo = new Servicos.Embarcador.Integracao.IntegracaoTitulo(unidadeTrabalho);
                    servIntegracaoTitulo.AdicionarTituloIntegracao(titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Criacao, unidadeTrabalho);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private bool GerarTitulosAutomaticamenteDeAdiantamentoSaldo(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool gerarFaturamentoAVista, bool gerarBoletoAutomaticamente, bool gerarTituloBloqueado, int codigoBoletoConfiguracao, bool enviarBoletoPorEmailAutomaticamente, bool enviarDocumentacaoFaturamentoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (!PossuiGeracaoTituloAdiantamentoSaldo(tomador, grupoPessoas))
                return false;

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido?.Carga;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUso = ObterTipoMovimentoConfiguracaoFinanceiraFatura(cte, carga, configuracaoFinanceiraFatura);

            List<(int Dia, decimal Percentual)> diasPercentualPrazoVencimento = ObterDiasPrazoVencimentoAdiantamentoSaldo(tomador, cargaPedido);

            int qtdParcelas = diasPercentualPrazoVencimento.Count;

            decimal valorTotal = Math.Round(cte != null ? cte.ValorAReceber : repCargaCTe.BuscarValorTotalReceberPorCarga(carga.Codigo, "A"), 2, MidpointRounding.ToEven);
            decimal valorMoedaTotal = Math.Round(cte != null ? (cte.ValorTotalMoeda ?? 0m) : (repCargaCTe.BuscarValorTotalMoedaPorCarga(carga.Codigo, "A") ?? 0m), 2, MidpointRounding.ToEven);

            int posParcela = 0;

            decimal valorAcumulado = 0m;
            decimal valorMoedaAcumulado = 0m;

            foreach ((int Dia, decimal Percentual) diaPercentual in diasPercentualPrazoVencimento)
            {
                posParcela++;

                decimal valorParcelas = Math.Round((diaPercentual.Percentual / 100) * valorTotal, 2);
                decimal valorMoedaParcelas = Math.Round((diaPercentual.Percentual / 100) * valorMoedaTotal, 2);

                if (posParcela == qtdParcelas)
                {
                    valorParcelas = valorTotal - valorAcumulado;
                    valorMoedaParcelas = valorMoedaTotal - valorMoedaAcumulado;
                }
                else
                {
                    valorAcumulado += valorParcelas;
                    valorMoedaAcumulado += valorMoedaParcelas;
                }

                DateTime dataEmissao = cte != null ? cte.DataEmissao.Value : repCargaCTe.BuscarUltimaDataEmissaoPorCarga(carga.Codigo, "A");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                {
                    DataEmissao = dataEmissao,
                    DataVencimento = ObterDataVencimentoTitulo(tomador, cargaPedido, dataEmissao, _unitOfWork, diaPercentual.Dia),
                    Sequencia = posParcela,
                    StatusTitulo = gerarTituloBloqueado ? StatusTitulo.Bloqueado : StatusTitulo.EmAberto,
                    DataAlteracao = DateTime.Now,
                    TipoTitulo = TipoTitulo.Receber,
                    ValorOriginal = valorParcelas,
                    ValorPendente = valorParcelas,
                    ValorTituloOriginal = valorParcelas,
                    Valor = valorParcelas,
                    ValorTotal = valorParcelas,
                    GrupoPessoas = grupoPessoas,
                    Pessoa = tomador,
                    TipoMovimento = tipoMovimentoUso,
                    TipoDocumentoTituloOriginal = cte != null ? "CT-e" : "Carga",
                    NumeroDocumentoTituloOriginal = cte != null ? cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString() : carga.CodigoCargaEmbarcador,
                    Empresa = cte?.Empresa ?? carga?.Empresa,
                    DataLancamento = DateTime.Now,
                    Usuario = cte?.Usuario ?? carga?.Operador,
                };

                if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                titulo.DataProgramacaoPagamento = titulo.DataVencimento;
                titulo.FormaTitulo = ObterFormaTituloGrupoPessoa(titulo);

                if (gerarBoletoAutomaticamente)
                {
                    titulo.BoletoStatusTitulo = BoletoStatusTitulo.Emitido;
                    titulo.BoletoConfiguracao = codigoBoletoConfiguracao > 0 ? repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao) : repBoletoConfiguracao.BuscarPrimeiraConfiguracao();
                    titulo.BoletoEnviadoPorEmail = false;
                    titulo.BoletoGeradoAutomaticamente = enviarBoletoPorEmailAutomaticamente;
                }

                titulo.EnviarDocumentacaoFaturamentoCTe = enviarDocumentacaoFaturamentoCTe;

                if (cte != null && cte.Moeda.HasValue && cte.Moeda != MoedaCotacaoBancoCentral.Real)
                {
                    titulo.MoedaCotacaoBancoCentral = cte.Moeda;
                    titulo.ValorMoedaCotacao = cte.ValorCotacaoMoeda ?? 0m;
                    titulo.DataBaseCRT = cargaPedido?.Pedido?.DataBaseCRT;
                    titulo.ValorOriginalMoedaEstrangeira = valorMoedaParcelas;
                }
                else if (carga != null && carga.Moeda.HasValue && carga.Moeda != MoedaCotacaoBancoCentral.Real)
                {
                    titulo.MoedaCotacaoBancoCentral = carga.Moeda;
                    titulo.DataBaseCRT = cargaPedido?.Pedido?.DataBaseCRT;
                    titulo.ValorMoedaCotacao = carga.ValorCotacaoMoeda ?? 0m;
                    titulo.ValorOriginalMoedaEstrangeira = valorMoedaParcelas;
                }

                repTitulo.Inserir(titulo);

                if (gerarBoletoAutomaticamente && titulo.BoletoStatusTitulo == BoletoStatusTitulo.Emitido)
                {
                    Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(_unitOfWork);
                    servTitulo.IntegrarEmitido(titulo, _unitOfWork);
                }

                Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento
                {
                    CTe = cte,
                    Carga = cte == null ? carga : null,
                    TipoDocumento = cte != null ? TipoDocumentoTitulo.CTe : TipoDocumentoTitulo.Carga,
                    Titulo = titulo,
                    Valor = titulo.ValorOriginal,
                    ValorTotal = titulo.ValorOriginal,
                    ValorPendente = titulo.ValorOriginal,
                    ValorMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorTotalMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorPendenteMoeda = titulo.ValorOriginalMoedaEstrangeira,
                    ValorCotacaoMoeda = titulo.ValorMoedaCotacao
                };

                if (cte != null)
                {
                    cte.CTePendenteIntegracaoFatura = true;
                    repCTe.Atualizar(cte);
                }

                repTituloDocumento.Inserir(tituloDocumento);

                servProcessoMovimento.GerarMovimentacao(tipoMovimentoUso, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Historico, _unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas);

                if (gerarFaturamentoAVista)
                    QuitarTitulo(out _, titulo, DateTime.Now.Date, DateTime.Now.Date, _unitOfWork, tomador, grupoPessoas, carga?.Operador, tipoServicoMultisoftware, "", 0m, false, 0m, false, Auditado);
            }

            return true;
        }

        private static DateTime ObterDataVencimentoTitulo(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, DateTime dataEmissaoTitulo, Repositorio.UnitOfWork unidadeTrabalho, int diaPrazoVencimentoPadrao)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento repVencimentoGrupo = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento(unidadeTrabalho);
            Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento repVencimentoCliente = new Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacaoFaturaVencimento repVencimentoTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacaoFaturaVencimento(unidadeTrabalho);

            bool? permiteFinalSemana = null;
            int? diaVencimentoFixo = null, diaPrazoVencimento = null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? diaSemana = null;

            bool achouConfiguracao = false;
            bool pularMes = false;
            
            if (cargaPedido != null && (cargaPedido.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)) 
            {
                achouConfiguracao = true;
                permiteFinalSemana = cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.PermiteFinalDeSemana ?? false;
                diaVencimentoFixo = (cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasMesFatura != null 
                                       && cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.DiasMesFatura.Count > 0
                                    ? cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.DiasMesFatura.FirstOrDefault() : 0);
                if (cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasSemanaFatura != null
                      && cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.DiasSemanaFatura.Count > 0)
                    diaSemana = cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasSemanaFatura.FirstOrDefault();
                diaPrazoVencimento = diaPrazoVencimentoPadrao > 0 ? diaPrazoVencimentoPadrao : cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasDePrazoFatura;
            } 
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoFaturaGrupo || tomador.GrupoPessoas == null)
                {
                    achouConfiguracao = true;
                    permiteFinalSemana = tomador.PermiteFinalDeSemana;
                    diaVencimentoFixo = tomador.DiasMesFatura != null && tomador.DiasMesFatura.Count > 0 ? tomador.DiasMesFatura.FirstOrDefault() : 0;
                    if (tomador.DiasSemanaFatura != null && tomador.DiasSemanaFatura.Count > 0)
                        diaSemana = tomador.DiasSemanaFatura.FirstOrDefault();
                    diaPrazoVencimento = diaPrazoVencimentoPadrao > 0 ? diaPrazoVencimentoPadrao : tomador.DiasDePrazoFatura;
                }
                else
                {
                    achouConfiguracao = true;
                    permiteFinalSemana = tomador.GrupoPessoas.PermiteFinalDeSemana;
                    diaVencimentoFixo = tomador.GrupoPessoas.DiasMesFatura != null && tomador.GrupoPessoas.DiasMesFatura.Count > 0 ? tomador.GrupoPessoas.DiasMesFatura.FirstOrDefault() : 0;//tomador.GrupoPessoas.DiaMesFatura;
                    if (tomador.GrupoPessoas.DiasSemanaFatura != null && tomador.GrupoPessoas.DiasSemanaFatura.Count > 0)
                        diaSemana = tomador.GrupoPessoas.DiasSemanaFatura.FirstOrDefault();//tomador.GrupoPessoas.DiaSemana;
                    diaPrazoVencimento = diaPrazoVencimentoPadrao > 0 ? diaPrazoVencimentoPadrao : tomador.GrupoPessoas.DiasDePrazoFatura;
                }
            }

            if (!achouConfiguracao)
            {
                if (cargaPedido != null && cargaPedido.Pedido.GrupoPessoas != null)
                {
                    permiteFinalSemana = cargaPedido.Pedido.GrupoPessoas.PermiteFinalDeSemana;
                    diaVencimentoFixo = cargaPedido.Pedido.GrupoPessoas.DiasMesFatura != null && cargaPedido.Pedido.GrupoPessoas.DiasMesFatura.Count > 0 ? cargaPedido.Pedido.GrupoPessoas.DiasMesFatura.FirstOrDefault() : 0;//cargaPedido.Pedido.GrupoPessoas.DiaMesFatura;
                    if (cargaPedido.Pedido.GrupoPessoas.DiasSemanaFatura != null && cargaPedido.Pedido.GrupoPessoas.DiasSemanaFatura.Count > 0)
                        diaSemana = cargaPedido.Pedido.GrupoPessoas.DiasSemanaFatura.FirstOrDefault();//cargaPedido.Pedido.GrupoPessoas.DiaSemana;
                    diaPrazoVencimento = diaPrazoVencimentoPadrao > 0 ? diaPrazoVencimentoPadrao : cargaPedido.Pedido.GrupoPessoas.DiasDePrazoFatura;
                }
            }

            DateTime dataVencimento = dataEmissaoTitulo.Date;

            if (diaVencimentoFixo.HasValue && diaVencimentoFixo.Value > 0)
            {
                DateTime dataVencimentoMinimo = dataEmissaoTitulo.Date;
                if (diaPrazoVencimento.HasValue && diaPrazoVencimento.Value > 0)
                    dataVencimentoMinimo = dataVencimentoMinimo.AddDays(diaPrazoVencimento.Value);

                do
                {
                    dataVencimento = dataVencimento.AddDays(1);
                }
                while (dataVencimento.Day != diaVencimentoFixo || (diaVencimentoFixo >= 30 && DateTime.DaysInMonth(dataVencimento.Year, dataVencimento.Month) != dataVencimento.Day) || dataVencimento <= dataVencimentoMinimo);
            }
            else if (diaPrazoVencimento.HasValue && diaPrazoVencimento.Value > 0)
            {
                dataVencimento = dataVencimento.AddDays(diaPrazoVencimento.Value);
            }

            if (diaSemana.HasValue)
            {
                DayOfWeek dayOfWeek = Servicos.Embarcador.Financeiro.Titulo.ObterDiaSemana(diaSemana.Value);

                int daysToAdd = ((int)dayOfWeek - (int)dataVencimento.DayOfWeek + 7) % 7;

                dataVencimento = dataVencimento.AddDays(daysToAdd);
            }

            if (!permiteFinalSemana.HasValue || permiteFinalSemana.Value)
            {
                if (dataVencimento.DayOfWeek == DayOfWeek.Saturday)
                    dataVencimento = dataVencimento.AddDays(2);
                else if (dataVencimento.DayOfWeek == DayOfWeek.Sunday)
                    dataVencimento = dataVencimento.AddDays(1);
            }

            int diaVencimento = dataVencimento > DateTime.MinValue ? dataVencimento.Day : 0;
            
            if (cargaPedido != null && (cargaPedido.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false))
            {
                if (cargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.HabilitarPeriodoVencimentoEspecifico ?? false)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento> vencimentos = repVencimentoTipoOperacao.BuscarPorTipoOperacao(cargaPedido.Carga?.TipoOperacao?.Codigo ?? 0);
                    if (vencimentos != null && vencimentos.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento faturaVencimento = vencimentos.Where(c => c.DiaInicial <= diaVencimento && c.DiaFinal >= diaVencimento)?.FirstOrDefault() ?? null;
                        int diaMultiplosVencimentos = faturaVencimento?.DiaVencimento ?? 0;

                        if (diaMultiplosVencimentos > 0)
                        {
                            pularMes = faturaVencimento.DiaInicial > diaMultiplosVencimentos && faturaVencimento.DiaFinal > diaMultiplosVencimentos;
                            dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, diaMultiplosVencimentos);
                            if (pularMes)
                                dataVencimento = dataVencimento.AddMonths(1);
                        }
                    }
                }
            }
            else if (tomador != null && diaVencimento > 0)
            {
                if (tomador.NaoUsarConfiguracaoFaturaGrupo || tomador.GrupoPessoas == null)
                {
                    if (tomador.HabilitarPeriodoVencimentoEspecifico)
                    {
                        List<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento> vencimentos = repVencimentoCliente.BuscarPorCliente(tomador.CPF_CNPJ);
                        if (vencimentos != null && vencimentos.Count > 0)
                        {
                            Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento faturaVencimento = vencimentos.Where(c => c.DiaInicial <= diaVencimento && c.DiaFinal >= diaVencimento)?.FirstOrDefault() ?? null;
                            int diaMultiplosVencimentos = faturaVencimento?.DiaVencimento ?? 0;

                            if (diaMultiplosVencimentos > 0)
                            {
                                pularMes = faturaVencimento.DiaInicial > diaMultiplosVencimentos && faturaVencimento.DiaFinal > diaMultiplosVencimentos;
                                dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, diaMultiplosVencimentos);
                                if (pularMes)
                                    dataVencimento = dataVencimento.AddMonths(1);
                            }
                        }
                    }
                }
                else if (tomador.GrupoPessoas.HabilitarPeriodoVencimentoEspecifico)
                {
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento> vencimentos = repVencimentoGrupo.BuscarPorGrupoPessoas(tomador.GrupoPessoas.Codigo);
                    if (vencimentos != null && vencimentos.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento faturaVencimento = vencimentos.Where(c => c.DiaInicial <= diaVencimento && c.DiaFinal >= diaVencimento)?.FirstOrDefault() ?? null;
                        int diaMultiplosVencimentos = faturaVencimento?.DiaVencimento ?? 0;

                        if (diaMultiplosVencimentos > 0)
                        {
                            pularMes = faturaVencimento.DiaInicial > diaMultiplosVencimentos && faturaVencimento.DiaFinal > diaMultiplosVencimentos;
                            dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, diaMultiplosVencimentos);
                            if (pularMes)
                                dataVencimento = dataVencimento.AddMonths(1);
                        }
                    }
                }
            }
            if (!achouConfiguracao && cargaPedido != null && cargaPedido.Pedido.GrupoPessoas != null && diaVencimento > 0)
            {
                if (cargaPedido.Pedido.GrupoPessoas.HabilitarPeriodoVencimentoEspecifico)
                {
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento> vencimentos = repVencimentoGrupo.BuscarPorGrupoPessoas(cargaPedido.Pedido.GrupoPessoas.Codigo);
                    if (vencimentos != null && vencimentos.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento faturaVencimento = vencimentos.Where(c => c.DiaInicial <= diaVencimento && c.DiaFinal >= diaVencimento)?.FirstOrDefault() ?? null;
                        int diaMultiplosVencimentos = faturaVencimento?.DiaVencimento ?? 0;

                        if (diaMultiplosVencimentos > 0)
                        {
                            pularMes = faturaVencimento.DiaInicial > diaMultiplosVencimentos && faturaVencimento.DiaFinal > diaMultiplosVencimentos;
                            dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, diaMultiplosVencimentos);
                            if (pularMes)
                                dataVencimento = dataVencimento.AddMonths(1);
                        }
                    }
                }
            }

            return dataVencimento;
        }

        private static DayOfWeek ObterDiaSemana(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            switch (diaSemana)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo:
                    return DayOfWeek.Sunday;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Segunda:
                    return DayOfWeek.Monday;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Terca:
                    return DayOfWeek.Tuesday;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quarta:
                    return DayOfWeek.Wednesday;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quinta:
                    return DayOfWeek.Thursday;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sexta:
                    return DayOfWeek.Friday;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sabado:
                    return DayOfWeek.Saturday;
                default:
                    return DayOfWeek.Sunday;
            }
        }

        private static void RemoverDadosBoletos(List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            for (int i = 0; i < listaTitulo.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Nenhum;
                titulo.Historico += " - BOLETO DELETADO POR " + usuario.Nome + " EM " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " NÚMERO ANTERIOR: " + titulo.NossoNumero;
                if (titulo.BoletoRemessa != null)
                    titulo.Historico += " REMESSA ANTERIOR: " + titulo.BoletoRemessa.NumeroSequencial.ToString();

                if (Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                    Utilidades.IO.FileStorageService.Storage.Delete(titulo.CaminhoBoleto);

                titulo.BoletoRemessa = null;
                titulo.BoletoConfiguracao = null;
                titulo.NossoNumero = null;
                titulo.CaminhoBoleto = null;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Removido os dados do boleto.", unitOfWork);
                repTitulo.Atualizar(titulo);
            }
        }

        #endregion
    }
}
