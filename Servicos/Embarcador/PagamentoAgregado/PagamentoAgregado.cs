using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.PagamentoAgregado
{
    public class PagamentoAgregado
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public PagamentoAgregado() { }

        public PagamentoAgregado(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public static List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> VerificarRegrasAutorizacaoPagamento(Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado repRegraPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado(unitOfWork);
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> listaRegras = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado>();
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> listaFiltrada = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado>();
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> alcadasCompativeis;

            // Cliente
            alcadasCompativeis = repRegraPagamentoAgregado.AlcadasPorCliente(pagamento.Cliente.CPF_CNPJ, pagamento.Data);
            listaRegras.AddRange(alcadasCompativeis);

            // Valor
            alcadasCompativeis = repRegraPagamentoAgregado.AlcadasPorValor(pagamento.Valor, pagamento.Data);
            listaRegras.AddRange(alcadasCompativeis);

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada = listaRegras;
                foreach (Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regra in listaRegras)
                {
                    if (regra.RegraPorCliente)
                    {
                        bool valido = false;
                        if (regra.AlcadasPessoa.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Cliente.CPF_CNPJ == pagamento.Cliente.CPF_CNPJ))
                            valido = true;
                        else if (regra.AlcadasPessoa.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Cliente.CPF_CNPJ == pagamento.Cliente.CPF_CNPJ))
                            valido = true;
                        else if (regra.AlcadasPessoa.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Cliente.CPF_CNPJ != pagamento.Cliente.CPF_CNPJ))
                            valido = true;
                        else if (regra.AlcadasPessoa.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Cliente.CPF_CNPJ != pagamento.Cliente.CPF_CNPJ))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            if (listaFiltrada.Count > 0)
                                continue;
                            else
                                return listaFiltrada;
                        }
                    }

                    if (regra.RegraPorValor)
                    {
                        bool valido = false;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor == pagamento.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor == pagamento.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor != pagamento.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor != pagamento.Valor))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && pagamento.Valor >= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && pagamento.Valor >= o.Valor))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && pagamento.Valor <= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && pagamento.Valor <= o.Valor))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && pagamento.Valor > o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && pagamento.Valor > o.Valor))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && pagamento.Valor < o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && pagamento.Valor < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            if (listaFiltrada.Count > 0)
                                continue;
                            else
                                return listaFiltrada;
                        }
                    }

                }
            }

            return listaFiltrada;
        }

        public static void PagamentoAgregadoAprovado(out string erro, Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            erro = "";
            if (pagamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Finalizado)
                return;

            Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);
            Servicos.PreCTe serPreCTe = new PreCTe(unitOfWork);
            Servicos.Embarcador.CTe.CTe serCTe = new Embarcador.CTe.CTe(unitOfWork);

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> descontosAcrescimo = repPagamentoAgregadoAcrescimoDesconto.BuscarPorPagamento(pagamento.Codigo);
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> documentos = repPagamentoAgregadoDocumento.BuscarPorPagamento(pagamento.Codigo);


            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarUltimaCargaDoCTe(documentos[0].ConhecimentoDeTransporteEletronico.Codigo);
            if (cargaCTe == null || cargaCTe.Carga == null)
            {
                erro = "O Documento lançado não possui carga vinculada";
                return;
            }

            //contrato de frete
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = new Dominio.Entidades.Embarcador.Terceiros.ContratoFrete
            {
                Carga = cargaCTe.Carga,
                NumeroContrato = repContratoFreteTerceiro.BuscarProximoCodigo(),
                SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao,
                TransportadorTerceiro = pagamento.Cliente,
                Usuario = pagamento.Usuario,
                ValorFreteSubcontratacao = pagamento.Valor,
                ValorFreteSubContratacaoTabelaFrete = pagamento.Valor,
                PercentualCobradoDoTerceiro = 0m,
                PercentualAdiantamento = 0m,
                ValorAdiantamento = 0m,
                ValorAbastecimento = 0m,
                PagamentoAgregado = pagamento,
                DataEmissaoContrato = DateTime.Now.Date,
                Observacao = "Gerado por Pagamento ao Agregado. " + pagamento.Observacao
            };

            repContratoFreteTerceiro.Inserir(contratoFrete);

            //documentos
            for (int i = 0; i < documentos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe contratoFreteCTe = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe();
                Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe = new Dominio.Entidades.PreConhecimentoDeTransporteEletronico();
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterEntidadeCTeParaObjeto(documentos[i].ConhecimentoDeTransporteEletronico, enviarCTeApenasParaTomador, unitOfWork);
                serPreCTe.SalvarDadosPreCTe(ref preCTe, cteIntegracao);

                contratoFreteCTe.PreCTe = preCTe;
                cargaCTe = repCargaCTe.BuscarUltimaCargaDoCTe(documentos[i].ConhecimentoDeTransporteEletronico.Codigo);
                if (cargaCTe == null)
                {
                    erro = "O Documento lançado não possui carga vinculada";
                    return;
                }
                contratoFreteCTe.CargaCTe = cargaCTe;
                contratoFreteCTe.ContratoFrete = contratoFrete;
                repContratoFreteCTe.Inserir(contratoFreteCTe);
            }

            //valores
            for (int i = 0; i < descontosAcrescimo.Count; i++)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor()
                {
                    ContratoFrete = contratoFrete,
                    Justificativa = descontosAcrescimo[i].Justificativa,
                    Valor = descontosAcrescimo[i].Valor
                };
                if (!contratoFreteValor.Justificativa.GerarMovimentoAutomatico)
                {
                    erro = "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.";
                    return;
                }

                contratoFreteValor.TipoJustificativa = contratoFreteValor.Justificativa.TipoJustificativa;
                if (contratoFreteValor.Justificativa.AplicacaoValorContratoFrete != null)
                    contratoFreteValor.AplicacaoValor = contratoFreteValor.Justificativa.AplicacaoValorContratoFrete.HasValue ? contratoFreteValor.Justificativa.AplicacaoValorContratoFrete.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento;
                else
                    contratoFreteValor.AplicacaoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal;
                contratoFreteValor.TipoMovimentoUso = contratoFreteValor.Justificativa.TipoMovimentoUsoJustificativa;
                contratoFreteValor.TipoMovimentoReversao = contratoFreteValor.Justificativa.TipoMovimentoReversaoUsoJustificativa;

                repContratoFreteValor.Inserir(contratoFreteValor);
            }

            contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;
            string erro2 = "";
            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);
            if (!modalidadeTransportadoraPessoas.GerarPagamentoTerceiro)
            {
                if (!serTituloAPagar.AtualizarTitulos(contratoFrete, unitOfWork, TipoServicoMultisoftware, out erro2, tipoAmbiente, Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                {
                    erro = "Problemas gerando título. " + erro2;
                    return;
                }

                if (!GerarMovimentacaoFinanceiraJustificativas(contratoFrete, unitOfWork, out erro2, TipoServicoMultisoftware))
                {
                    erro = "Problemas gerando movimento das justificativas. " + erro2;
                    return;
                }
            }

            repContratoFreteTerceiro.Atualizar(contratoFrete);

            //List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaFiltrada = Servicos.Embarcador.Terceiros.ContratoFrete.VerificarRegrasAutorizacao(contratoFrete, unitOfWork);
            //if (listaFiltrada.Count() > 0)
            //{
            //    if (!Servicos.Embarcador.Terceiros.ContratoFrete.CriarRegrasAutorizacao(listaFiltrada, contratoFrete, contratoFrete.Carga.Operador, TipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork))
            //        contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;
            //    else
            //        contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao;
            //}
            //else
            //    contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra;

            //repContratoFreteTerceiro.Atualizar(contratoFrete);

            erro = string.Empty;
        }

        private static bool GerarMovimentacaoFinanceiraJustificativas(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unidadeTrabalho, out string erro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimentoFinanceiro = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeTrabalho.StringConexao);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> justificativas = repContratoFreteValor.BuscarPorContratoFrete(contratoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor justificativa in justificativas)
            {
                if (!svcMovimentoFinanceiro.GerarMovimentacao(out erro, justificativa.TipoMovimentoUso, contratoFrete.DataEmissaoContrato, justificativa.Valor, contratoFrete.NumeroContrato.ToString(), "Referente ao valor justificado no contrato de frete nº " + contratoFrete.NumeroContrato + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, TipoServicoMultisoftware))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        /// <summary>
        /// Cria o vinculo das regras com os aprovadores
        /// </summary>
        /// <returns>Retorna verdadeiro quando existe alguma regra para algum aprovador e falso para quando é aprovada automática</returns>
        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> listaFiltrada, Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiRegraPendente = false;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            foreach (Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado autorizacao = new Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado
                        {
                            PagamentoAgregado = pagamento,
                            Usuario = aprovador,
                            RegraPagamentoAgregado = regra,
                        };
                        repAprovacaoAlcadaPagamentoAgregado.Inserir(autorizacao);

                        string titulo = Localization.Resources.PagamentoAgregado.AutorizacaoPagamentoAgregado.PagamentoAgregado;
                        string nota = string.Format(Localization.Resources.PagamentoAgregado.AutorizacaoPagamentoAgregado.UsuarioSolicitouLiberacaoPagamentoValorPara, usuario.Nome, pagamento.Valor.ToString("n2"), pagamento.Cliente.Nome);
                        serNotificacao.GerarNotificacaoEmail(aprovador, usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", titulo, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado autorizacao = new Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado
                    {
                        PagamentoAgregado = pagamento,
                        Usuario = null,
                        RegraPagamentoAgregado = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao
                    };
                    repAprovacaoAlcadaPagamentoAgregado.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        #endregion
    }
}
