using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.NotaFiscal
{
    public sealed class NaoConformidadeAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade,
        Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade,
        Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade
    >
    {
        #region Atributos 

        private List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> _listaRegrasAtivas;

        #endregion Atributos

        #region Construtores

        public NaoConformidadeAprovacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade naoConformidade, List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade repositorio = new Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            DateTime dataCriacao = DateTime.Now;
            bool existeRegraSemAprovacao = false;

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;
                    List<Dominio.Entidades.Usuario> aprovadores;

                    if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Setor)
                        aprovadores = repositorioUsuario.BuscarUsuariosPorSetores(regra.Setores.Select(o => o.Codigo).ToList());
                    else
                        aprovadores = regra.Aprovadores.ToList();

                    foreach (Dominio.Entidades.Usuario aprovador in aprovadores)
                    {
                        Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidadeAprovar = new Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade()
                        {
                            Codigo = naoConformidade.Codigo,
                            XMLNotaFiscal = naoConformidade.NumeroNotaFiscal == 0 ? null : new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                            {
                                Numero = naoConformidade.NumeroNotaFiscal
                            }
                        };

                        Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade aprovacao = new Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade()
                        {
                            OrigemAprovacao = naoConformidadeAprovar,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = dataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            TipoAprovadorRegra = regra.TipoAprovadorRegra
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(naoConformidadeAprovar, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade aprovacao = new Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade()
                    {
                        OrigemAprovacao = new Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade() { Codigo = naoConformidade.Codigo },
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = dataCriacao,
                        TipoAprovadorRegra = regra.TipoAprovadorRegra
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            naoConformidade.Situacao = existeRegraSemAprovacao ? SituacaoNaoConformidade.AguardandoTratativa : SituacaoNaoConformidade.Concluida;
        }

        private List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> ObterRegrasAtivas()
        {
            if (_listaRegrasAtivas == null)
            {
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade>(_unitOfWork);
                _listaRegrasAtivas = repositorioRegra.BuscarPorAtiva();
            }

            return _listaRegrasAtivas;
        }

        private List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> ObterRegrasAutorizacao(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade naoConformidade)
        {
            List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> listaRegras = ObterRegrasAtivas();
            List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade>();

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, naoConformidade.CodigoFilial))
                    continue;

                if (regra.RegraPorCFOP && !ValidarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaCFOP, Dominio.Entidades.CFOP>(regra.AlcadasCFOP, naoConformidade.CodigoCFOP))
                    continue;

                if (regra.RegraPorNaoConformidade && !ValidarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaNaoConformidade, Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>(regra.AlcadasNaoConformidade, naoConformidade.CodigoItemNaoConformidade))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, naoConformidade.CodigoTipoOperacao))
                    continue;

                if (regra.Grupo != GrupoNC.NaoSelecionado && regra.Grupo != naoConformidade.Grupo)
                    continue;

                if (regra.SubGrupo != SubGrupoNC.NaoSelecionado && regra.SubGrupo != naoConformidade.SubGrupo)
                    continue;

                if (regra.Area != AreaNC.NaoSelecionado && regra.Area != naoConformidade.Area)
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade, Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: naoConformidade.Codigo,
                URLPagina: "Cargas/Carga",
                titulo: Localization.Resources.NotaFiscal.NaoConformidadeAprovacao.NaoConformidade,
                nota: string.Format(Localization.Resources.NotaFiscal.NaoConformidadeAprovacao.NaoConformidadeAguardandoAprovacao, (naoConformidade.XMLNotaFiscal != null ? string.Format(Localization.Resources.NotaFiscal.NaoConformidadeAprovacao.DaNota, naoConformidade.XMLNotaFiscal.Numero) : "")),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade CriarAprovacao(int codigoNaoConformidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repositorioNaoConformidade.BuscarDadosNaoConformidade(codigoNaoConformidade);

            Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNC = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(_unitOfWork).BuscarPorCodigo(naoConformidade.CodigoItemNaoConformidade);
            if (itemNC.PermiteContingencia)
            {
                RemoverAprovacaoPorCodigo(codigoNaoConformidade);

                List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> regras = ObterRegrasAutorizacao(naoConformidade);

                if (regras.Count > 0)
                    CriarRegrasAprovacao(naoConformidade, regras, tipoServicoMultisoftware);
                else
                    naoConformidade.Situacao = SituacaoNaoConformidade.SemRegraAprovacao;

                repositorioNaoConformidade.AtualizarSituacao(naoConformidade.Codigo, naoConformidade.Situacao);
            }
            return naoConformidade;
        }

        #endregion Métodos Públicos
    }
}
