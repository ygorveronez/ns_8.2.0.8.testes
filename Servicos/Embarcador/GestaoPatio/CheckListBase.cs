using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPatio
{
    public abstract class CheckListBase : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Atributos Protegidos

        private readonly EtapaCheckList _etapaCheckList;

        #endregion

        #region Construtores

        public CheckListBase(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, EtapaFluxoGestaoPatio etapaFluxoGestaoPatio) : base(unitOfWork, auditado, etapaFluxoGestaoPatio, cliente)
        {
            if (etapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.AvaliacaoDescarga)
                _etapaCheckList = EtapaCheckList.AvaliacaoDescarga;
            else
                _etapaCheckList = EtapaCheckList.Checklist;
        }

        #endregion

        #region Métodos Protegidos

        protected Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga ObterCheckList(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = repositorioChecklist.BuscarPorFluxoGestaoPatioEEtapaCheckList(fluxoGestaoPatio.Codigo, _etapaCheckList);

            return checklist;
        }

        #endregion

        #region Métodos Públicos

        private List<SubCategoriaOpcaoCheckList> ObterSubcategorias(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, CategoriaOpcaoCheckList categoria)
        {
            if ((categoria != CategoriaOpcaoCheckList.Reboque) || !ObterConfiguracaoGestaoPatio().UtilizarCategoriaDeReboqueConformeModeloVeicularCarga || (cargaBase.ModeloVeicularCarga == null))
                return new List<SubCategoriaOpcaoCheckList>() { SubCategoriaOpcaoCheckList.NaoDefinido };

            List<SubCategoriaOpcaoCheckList> subcategorias = new List<SubCategoriaOpcaoCheckList>() { SubCategoriaOpcaoCheckList.Reboque };

            if (cargaBase.ModeloVeicularCarga.NumeroReboques > 1)
                subcategorias.Add(SubCategoriaOpcaoCheckList.SegundoReboque);

            return subcategorias;
        }

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = ObterCheckList(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio);

            if (checklist != null)
                return;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio;
            int codigoFilial = fluxoGestaoPatio.Filial?.Codigo ?? 0;

            Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioChecklistOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioChecklistPerguntaAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> perguntas = repositorioChecklistOpcoes.BuscarPerguntasPorAplicacao(fluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Destino ? AplicacaoOpcaoCheckList.Descarregamento : AplicacaoOpcaoCheckList.Carregamento, codigoFilial, _etapaCheckList);

            checklist = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga()
            {
                FluxoGestaoPatio = fluxoGestaoPatio,
                Carga = fluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatio.PreCarga,
                Aplicacao = AplicacaoOpcaoCheckList.Carregamento,
                DataAbertura = DateTime.Now,
                EtapaCheckListLiberado = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                EtapaCheckList = _etapaCheckList,
                Observacoes = "",
                Situacao = SituacaoCheckList.Aberto
            };

            repositorioChecklist.Inserir(checklist);

            perguntas = ObterPerguntasFiltradas(checklist.Carga, perguntas);
            List<CategoriaOpcaoCheckList> categorias = perguntas.Select(o => o.Categoria).Distinct().ToList();

            foreach (CategoriaOpcaoCheckList categoria in categorias)
            {
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> perguntasPorCategoria = perguntas.Where(o => o.Categoria == categoria).ToList();
                List<SubCategoriaOpcaoCheckList> subcategorias = ObterSubcategorias(checklist.CargaBase, categoria);

                foreach (SubCategoriaOpcaoCheckList subcategoria in subcategorias)
                {
                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes pergunta in perguntasPorCategoria)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta checklistPergunta = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta()
                        {
                            CheckListCarga = checklist,
                            Categoria = pergunta.Categoria,
                            Subcategoria = subcategoria,
                            Descricao = ObterDescricaoPergunta(pergunta, checklist.Carga),
                            RelacaoCampo = pergunta.RelacaoCampo,
                            RelacaoPergunta = pergunta.RelacaoPergunta,
                            Tipo = pergunta.Tipo,
                            Resposta = null,
                            Obrigatorio = pergunta.Obrigatorio,
                            Observacao = "",
                            RespostaImpeditiva = pergunta.RespostaImpeditiva,
                            TagIntegracao = pergunta.TagIntegracao
                        };

                        if (pergunta.TipoData && pergunta.TipoHora)
                            checklistPergunta.TipoInformativo = TipoInformativo.TipoDataHora;
                        else if (pergunta.TipoData)
                            checklistPergunta.TipoInformativo = TipoInformativo.TipoData;
                        else if (pergunta.TipoHora)
                            checklistPergunta.TipoInformativo = TipoInformativo.TipoHora;
                        else if (pergunta.TipoDecimal)
                            checklistPergunta.TipoInformativo = TipoInformativo.TipoDecimal;

                        repositorioChecklistPergunta.Inserir(checklistPergunta);

                        if (pergunta.Tipo.IsPossuiAlternativas())
                        {
                            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa> alternativas = pergunta.Alternativas.ToList();

                            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa alternativa in alternativas)
                            {
                                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa checklistPerguntaAlternativa = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa()
                                {
                                    CheckListCargaPergunta = checklistPergunta,
                                    Descricao = alternativa.Descricao,
                                    Ordem = alternativa.Ordem,
                                    OpcaoImpeditiva = alternativa.OpcaoImpeditiva,
                                    Marcado = false,
                                };

                                repositorioChecklistPerguntaAlternativa.Inserir(checklistPerguntaAlternativa);
                            }
                        }
                    }
                }
            }
        }

        [Obsolete("(NÂO UTILIZAR) Método adicionado para tratar apenas o problema da tarefa #58940")]
        public void AtualizarPerguntas(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist)
        {
            int codigoFilial = checklist.FluxoGestaoPatio.Filial?.Codigo ?? 0;
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioChecklistPerguntaAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> checklistCargaPerguntas = repositorioChecklistPergunta.BuscarPorCheckList(checklist.Codigo);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> checklistCargaPerguntaAlternativas = repositorioChecklistPerguntaAlternativa.BuscarPorCheckList(checklist.Codigo);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa checklistCargaPerguntaAlternativa in checklistCargaPerguntaAlternativas)
                repositorioChecklistPerguntaAlternativa.Deletar(checklistCargaPerguntaAlternativa);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta checklistCargaPergunta in checklistCargaPerguntas)
                repositorioChecklistPergunta.Deletar(checklistCargaPergunta);

            Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioChecklistOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> perguntas = repositorioChecklistOpcoes.BuscarPerguntasPorAplicacao(checklist.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Destino ? AplicacaoOpcaoCheckList.Descarregamento : AplicacaoOpcaoCheckList.Carregamento, codigoFilial, _etapaCheckList);

            perguntas = ObterPerguntasFiltradas(checklist.Carga, perguntas);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes pergunta in perguntas)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta checklistPergunta = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta()
                {
                    CheckListCarga = checklist,
                    Categoria = pergunta.Categoria,
                    Descricao = ObterDescricaoPergunta(pergunta, checklist.Carga),
                    RelacaoCampo = pergunta.RelacaoCampo,
                    RelacaoPergunta = pergunta.RelacaoPergunta,
                    Tipo = pergunta.Tipo,
                    Resposta = null,
                    Obrigatorio = pergunta.Obrigatorio,
                    Observacao = "",
                    TagIntegracao = pergunta.TagIntegracao,
                    RespostaImpeditiva = pergunta.RespostaImpeditiva
                };

                if (pergunta.TipoData && pergunta.TipoHora)
                    checklistPergunta.TipoInformativo = TipoInformativo.TipoDataHora;
                else if (pergunta.TipoData)
                    checklistPergunta.TipoInformativo = TipoInformativo.TipoData;
                else if (pergunta.TipoHora)
                    checklistPergunta.TipoInformativo = TipoInformativo.TipoHora;
                else if (pergunta.TipoDecimal)
                    checklistPergunta.TipoInformativo = TipoInformativo.TipoDecimal;

                repositorioChecklistPergunta.Inserir(checklistPergunta);

                if (pergunta.Tipo.IsPossuiAlternativas())
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa> alternativas = pergunta.Alternativas.ToList();

                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa alternativa in alternativas)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa checklistPerguntaAlternativa = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa()
                        {
                            CheckListCargaPergunta = checklistPergunta,
                            Descricao = alternativa.Descricao,
                            Ordem = alternativa.Ordem,
                            OpcaoImpeditiva = alternativa.OpcaoImpeditiva,
                            Marcado = false,
                        };

                        repositorioChecklistPerguntaAlternativa.Inserir(checklistPerguntaAlternativa);
                    }
                }
            }
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = ObterCheckList(fluxoGestaoPatio);

            if (checklist != null)
            {
                checklist.Carga = carga;
                repositorioChecklist.Atualizar(checklist);
            }
        }

        public string ObterDescricaoPergunta(Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes pergunta, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!pergunta.ExibirSomenteParaFretesOndeRemetenteForTomador)
                return pergunta.Descricao;

            return pergunta.Descricao
                .Replace("#ModeloVeicularCarga", carga?.ModeloVeicularCarga?.Descricao ?? "")
                .Replace("#NumeroEixosModeloVeicularCarga", carga?.ModeloVeicularCarga?.NumeroEixos.ToString() ?? "");
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> ObterPerguntasFiltradas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> perguntas)
        {
            if (carga == null)
                return perguntas;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            bool possuiPedidoRemetenteIgualTomador = cargasPedido.Any(cp => cp.Pedido.Remetente?.CPF_CNPJ != null && cp.Pedido.Remetente?.CPF_CNPJ == cp.Pedido.ObterTomador()?.CPF_CNPJ);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> perguntasFiltradas = new List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes>();

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes pergunta in perguntas)
            {
                if (pergunta.ExibirSomenteParaFretesOndeRemetenteForTomador)
                {
                    if (!possuiPedidoRemetenteIgualTomador)
                        continue;

                    if (
                        (carga.ModeloVeicularCarga?.NaoSolicitarNoChecklist ?? false) &&
                        (pergunta.Descricao.Contains("#ModeloVeicularCarga") || pergunta.Descricao.Contains("#NumeroEixosModeloVeicularCarga"))
                    )
                        continue;
                }

                perguntasFiltradas.Add(pergunta);
            }

            return perguntasFiltradas;
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = ObterCheckList(fluxoGestaoPatio);

            if (checklist != null)
            {
                checklist.Carga = cargaNova;
                repositorioChecklist.Atualizar(checklist);
            }
        }

        public void ResponderPerguntasComCheckListAnterior(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklistAtual = repositorioChecklist.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (checklistAtual == null || !checklistAtual.Situacao.IsPermiteEdicao())
                return;

            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklistBase = repositorioChecklist.BuscarAnteriorPorFilialVeiculoETIpoOperacao(fluxoGestaoPatio.Filial.Codigo, fluxoGestaoPatio.Veiculo.Codigo, fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo ?? 0);

            if (checklistBase == null || checklistBase.CheckListCargaVigencia.PreenchimentoManualObrigatorio || checklistBase.CheckListCargaVigencia.DataFimVigencia < DateTime.Now)
                return;

            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioCheckListCargaPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntasAtuais = repositorioCheckListCargaPergunta.BuscarPorCheckList(checklistAtual.Codigo);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntasAnteriores = repositorioCheckListCargaPergunta.BuscarPorCheckList(checklistBase.Codigo);

            if (perguntasAnteriores.Count <= 0 || perguntasAtuais.Count <= 0)
                return;

            List<int> codigosPerguntasAnteriores = perguntasAnteriores.Select(pergunta => pergunta.Codigo).ToList();
            List<int> codigosperguntasAtuais = perguntasAtuais.Select(pergunta => pergunta.Codigo).ToList();

            Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativasAnteriores = repositorioCheckListCargaPerguntaAlternativa.BuscarPorPerguntas(codigosPerguntasAnteriores);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativasAtuais = repositorioCheckListCargaPerguntaAlternativa.BuscarPorPerguntas(codigosperguntasAtuais);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta perguntaAtual in perguntasAtuais)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta perguntaAnterior = perguntasAnteriores.Find(x => x.Descricao == perguntaAtual.Descricao);

                if (perguntaAnterior == null)
                    continue;

                perguntaAtual.Observacao = perguntaAnterior.Observacao;
                perguntaAtual.Resposta = perguntaAnterior.Resposta;

                if (perguntaAtual.Tipo.IsPossuiAlternativas())
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativasPorPerguntaAnteriores = alternativasAnteriores.FindAll(x => x.CheckListCargaPergunta.Codigo == perguntaAnterior.Codigo);
                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativasPorPerguntaAtuais = alternativasAtuais.FindAll(x => x.CheckListCargaPergunta.Codigo == perguntaAtual.Codigo);

                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa alternativa in alternativasPorPerguntaAtuais)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa alternativaAnterior = alternativasPorPerguntaAnteriores.Find(x => x.Descricao == alternativa.Descricao);

                        alternativa.Marcado = alternativaAnterior.Marcado;

                        repositorioCheckListCargaPerguntaAlternativa.Atualizar(alternativa);
                    }
                }

                repositorioCheckListCargaPergunta.Atualizar(perguntaAtual);
            }

            Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura repositorioCheckListCargaAssinatura = new Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura(_unitOfWork);
            Servicos.Embarcador.GestaoPatio.CheckListAssinatura servicoCheckListAssinatura = new Servicos.Embarcador.GestaoPatio.CheckListAssinatura(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura> assinaturasAntigas = repositorioCheckListCargaAssinatura.BuscarPorCheckList(checklistBase.Codigo);

            servicoCheckListAssinatura.DeletarAssinaturas(checklistAtual);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinaturaAntiga in assinaturasAntigas)
                servicoCheckListAssinatura.CopiarAssinatura(checklistAtual, assinaturaAntiga);
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = ObterCheckList(fluxoGestaoPatio);

            if (checklist == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!checklist.Situacao.IsPermiteEdicao())
                throw new ServicoException("Não é possível alterar o checklist na situação atual.");

            if (!checklist.EtapaCheckListLiberado)
                throw new ServicoException("A liberação do checklist ainda não foi autorizada.");

            checklist.DataLiberacao = DateTime.Now;
            checklist.Situacao = SituacaoCheckList.Finalizado;

            LiberarProximaEtapa(fluxoGestaoPatio);
            repositorioChecklist.Atualizar(checklist);
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = ObterCheckList(fluxoGestaoPatio);

            if (checklist != null)
            {
                checklist.EtapaCheckListLiberado = true;
                repositorioChecklist.Atualizar(checklist);
            }

            return true;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = ObterCheckList(fluxoGestaoPatio);

            if (checklist != null)
            {
                checklist.EtapaCheckListLiberado = false;
                repositorioChecklist.Atualizar(checklist);
            }
        }

        #endregion
    }
}