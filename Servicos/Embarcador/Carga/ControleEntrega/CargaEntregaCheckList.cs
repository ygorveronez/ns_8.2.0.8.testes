using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public sealed class CargaEntregaCheckList
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaEntregaCheckList(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public CargaEntregaCheckList(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPerguntaAlternativa ObterObjetoMobileAlternativaPerguntaCheckList(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa alternativa)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPerguntaAlternativa()
            {
                Codigo = alternativa.Codigo,
                Descricao = alternativa.Descricao,
                Ordem = alternativa.Ordem,
                Valor = alternativa.Valor,
                Marcado = alternativa.Marcado,
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta ObterObjetoMobilePerguntaCheckList(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta pergunta)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa> alternativas = pergunta.Alternativas.ToList();

            Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta objPergunta = new Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckListPergunta()
            {
                Codigo = pergunta.Codigo,
                Descricao = pergunta.Descricao,
                Ordem = pergunta.Ordem,
                Obrigatorio = pergunta.Obrigatorio,
                TipoData = pergunta.TipoData,
                TipoHora = pergunta.TipoHora,
                PermiteNaoAplica = pergunta.PermiteNaoAplica,
                Tipo = pergunta.Tipo,
                Resposta = ObterRespostaPergunta(pergunta, alternativas),
                RespostaSimNao = pergunta.RespostaSimNao,
                RespostaNaoSeAplica = pergunta.RespostaNaoSeAplica,
                Alternativas = (
                    from alternativa in alternativas
                    orderby alternativa.Ordem
                    select ObterObjetoMobileAlternativaPerguntaCheckList(alternativa)
                ).ToList()
            };

            return objPergunta;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> ObterObjetoMobileCheckList(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> objetoCheckList = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList>();

            List<string> assuntos = (from o in perguntas select o.Assunto).Distinct().ToList();
            int codigoCheckList = perguntas.FirstOrDefault().CargaEntregaCheckList.Codigo;

            foreach (string assunto in assuntos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntasAssunto = (from o in perguntas where o.Assunto == assunto select o).ToList();

                objetoCheckList.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList()
                {
                    Codigo = codigoCheckList,
                    Assunto = assunto,
                    Perguntas = (from pergunta in perguntasAssunto select ObterObjetoMobilePerguntaCheckList(pergunta)).ToList()
                });
            }

            return objetoCheckList;
        }

        public dynamic ObterRespostaPergunta(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta pergunta, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa> alternativas)
        {
            if (pergunta.Tipo == TipoOpcaoCheckList.Selecoes)
                return alternativas.Where(a => a.Marcado).Select(a => a.Codigo).FirstOrDefault().ToString() ?? string.Empty;

            if (pergunta.Tipo == TipoOpcaoCheckList.SimNao)
                return (pergunta.RespostaSimNao?.ToString() ?? string.Empty).ToLower();

            return pergunta.Resposta;
        }

        public void SalvarRespostasCheckList(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList checkList, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList> checkListRespostas)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repositorioCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repositorioCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa repositorioCargaEntregaCheckListAlternativa = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa(_unitOfWork);

            checkList.Initialize();

            Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorioCargaEntregaCheckList.Atualizar(checkList, _auditado);

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList resposta in checkListRespostas)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta pergunta = repositorioCargaEntregaCheckListPergunta.BuscarPorCodigoECheckList(resposta.Codigo, checkList.Codigo);

                if (pergunta == null)
                    continue;

                bool perguntaNaoRespondida = false;

                pergunta.Initialize();
                pergunta.RespostaNaoSeAplica = resposta.NaoAplica;

                if (pergunta.TipoComAlternativas())
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa> alernativas = repositorioCargaEntregaCheckListAlternativa.BuscarPorPergunta(pergunta.Codigo);
                    List<int> alternativasMarcadas = (from a in resposta.Alternativas where a.Codigo != 0 select a.Codigo).ToList();
                    bool permiteMultiplos = pergunta.Tipo == TipoOpcaoCheckList.Opcoes;
                    bool algumaOpcaoMarcada = false;

                    if (pergunta.Tipo == TipoOpcaoCheckList.Selecoes)
                    {
                        if (int.TryParse(resposta.Resposta, out int codigoOpcaoMarcada))
                            alternativasMarcadas = new List<int>() { codigoOpcaoMarcada };
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa alternativa in alernativas)
                    {
                        alternativa.Initialize();
                        alternativa.Marcado = !resposta.NaoAplica && (permiteMultiplos || (!permiteMultiplos && !algumaOpcaoMarcada)) && alternativasMarcadas.Contains(alternativa.Codigo);
                        repositorioCargaEntregaCheckListAlternativa.Atualizar(alternativa, _auditado, historico);

                        if (alternativa.Marcado)
                            algumaOpcaoMarcada = true;
                    }

                    if (!algumaOpcaoMarcada)
                        perguntaNaoRespondida = true;
                }
                else if (pergunta.Tipo == TipoOpcaoCheckList.SimNao)
                {
                    if (bool.TryParse(resposta.Resposta, out bool simNao))
                        pergunta.RespostaSimNao = simNao;
                    else
                    {
                        pergunta.RespostaSimNao = null;
                        perguntaNaoRespondida = true;
                    }
                }
                else if (pergunta.Tipo == TipoOpcaoCheckList.Informativo)
                {
                    if (resposta.Resposta == "N/A")
                    {
                        pergunta.Resposta = "";
                        pergunta.RespostaNaoSeAplica = true;
                        perguntaNaoRespondida = false;
                    }
                    else
                    {
                        pergunta.Resposta = resposta.Resposta;
                        perguntaNaoRespondida = ((pergunta.Resposta ?? string.Empty).Length == 0);
                    }
                }

                if (pergunta.Obrigatorio && perguntaNaoRespondida && pergunta.PermiteNaoAplica && !(pergunta.RespostaNaoSeAplica ?? false))
                    throw new ServicoException($"A pergunta \"{pergunta.Descricao}\" é obrigatório");

                repositorioCargaEntregaCheckListPergunta.Atualizar(pergunta, _auditado, historico);

                if (_auditado != null)
                    Auditoria.Auditoria.Auditar(_auditado, checkList, $"Atualizou o CheckList do Controle Coleta/Entrega - {checkList.CargaEntrega.Descricao}", _unitOfWork);
            }

            checkList.Respondido = true;
            repositorioCargaEntregaCheckList.Atualizar(checkList);
        }

        public string ObterRespostaDescricaoPergunta(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta pergunta)
        {
            switch (pergunta.Tipo)
            {
                case TipoOpcaoCheckList.SimNao:
                    return pergunta?.RespostaSimNao != null ? (pergunta?.RespostaSimNao == true ? "Sim" : "Não") : null;
                case TipoOpcaoCheckList.Aprovacao:
                    return null;
                case TipoOpcaoCheckList.Opcoes:
                    var selecionadas = pergunta.Alternativas.Where(o => o.Marcado).ToList();
                    return String.Join("; ", selecionadas?.Select(o => o.Descricao));
                case TipoOpcaoCheckList.Selecoes:
                case TipoOpcaoCheckList.Escala:
                    var selecao = pergunta.Alternativas.Where(o => o.Marcado).FirstOrDefault();
                    return selecao?.Descricao;
                case TipoOpcaoCheckList.Informativo:
                    return pergunta.Resposta;
            }

            return null;
        }

        public List<int> ObterRespostasCodigosPergunta(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta pergunta)
        {
            switch (pergunta.Tipo)
            {
                case TipoOpcaoCheckList.SimNao:
                    return null;
                case TipoOpcaoCheckList.Aprovacao:
                    return null;
                case TipoOpcaoCheckList.Opcoes:
                    var selecionadas = pergunta.Alternativas.Where(o => o.Marcado).ToList();
                    return (from o in selecionadas select o.CodigoIntegracao).ToList();
                case TipoOpcaoCheckList.Selecoes:
                case TipoOpcaoCheckList.Escala:
                    var selecao = pergunta.Alternativas.Where(o => o.Marcado).FirstOrDefault();
                    return selecao != null ? new List<int> { selecao.CodigoIntegracao } : null;
                case TipoOpcaoCheckList.Informativo:
                    return null;
            }

            return null;
        }

        #endregion
    }
}
