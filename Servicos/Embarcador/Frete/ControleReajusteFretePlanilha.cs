using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public class ControleReajusteFretePlanilha
    {
        public static List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha repRegraDescarte = new Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> listaRegras = new List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha>();
            List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> listaFiltrada = new List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha>();
            List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> alcadasCompativeis;

            alcadasCompativeis = repRegraDescarte.AlcadasPorTipoOperacao(controle.TipoOperacao.Codigo, DateTime.Now);
            listaRegras.AddRange(alcadasCompativeis);

            alcadasCompativeis = repRegraDescarte.AlcadasPorFilial(controle.Filial.Codigo, DateTime.Now);
            listaRegras.AddRange(alcadasCompativeis);

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras);
                foreach (Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regra in listaRegras)
                {
                    if (regra.RegraPorTipoOperacao)
                    {
                        bool valido = false;
                        if (regra.AlcadasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.TipoOperacao.Codigo == controle.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.AlcadasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.TipoOperacao.Codigo == controle.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.AlcadasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.TipoOperacao.Codigo != controle.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.AlcadasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.TipoOperacao.Codigo != controle.TipoOperacao.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorFilial)
                    {
                        bool valido = false;
                        if (regra.AlcadasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Filial.Codigo == controle.Filial.Codigo))
                            valido = true;
                        else if (regra.AlcadasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Filial.Codigo == controle.Filial.Codigo))
                            valido = true;
                        else if (regra.AlcadasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Filial.Codigo != controle.Filial.Codigo))
                            valido = true;
                        else if (regra.AlcadasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Filial.Codigo != controle.Filial.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return listaFiltrada;
        }

        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> listaFiltrada, Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiRegraPendente = false;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repAprovacaoAlcadaControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);

            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            foreach (Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha autorizacao = new Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha
                        {
                            ControleReajusteFretePlanilha = controle,
                            Usuario = aprovador,
                            RegraControleReajusteFretePlanilha = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,
                            DataCriacao = controle.DataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };
                        repAprovacaoAlcadaControleReajusteFretePlanilha.Inserir(autorizacao);

                        string titulo = Localization.Resources.Fretes.AutorizacaoControleReajusteFretePlanilha.ControleReajusteFretePlanilha;
                        string nota = string.Format(Localization.Resources.Fretes.AutorizacaoControleReajusteFretePlanilha.UsuarioAdicionouPlanilhaReajuste, usuario.Nome, controle.Numero.ToString());
                        serNotificacao.GerarNotificacaoEmail(aprovador, usuario, controle.Codigo, "Fretes/ControleReajusteFretePlanilha", titulo, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha autorizacao = new Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha
                    {
                        ControleReajusteFretePlanilha = controle,
                        Usuario = null,
                        RegraControleReajusteFretePlanilha = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        DataCriacao = controle.DataCriacao,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao
                    };
                    repAprovacaoAlcadaControleReajusteFretePlanilha.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        public static void ContratoAprovado(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha contrato, Repositorio.UnitOfWork unitOfWork)
        {
            if (contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Aprovado)
                return;
        }
    }
}
