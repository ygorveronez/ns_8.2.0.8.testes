using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pallets
{
    public class DevolucaoValePallet : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet
    >
    {
        #region Atributos Privados Somente Leitura

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public DevolucaoValePallet(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork)
        {
            _auditado = auditado;
        }

        #endregion

        #region Métodos Privados

        private void AtualizarDevolucaoPallet(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet)
        {
            var repositorioDevolucaoPallet = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);

            if (!devolucaoValePallet.ValePallet.Devolucao.IsInitialized())
                devolucaoValePallet.ValePallet.Devolucao.Initialize();

            if (devolucaoValePallet.ValePallet.Devolucao.QuantidadePallets > devolucaoValePallet.QuantidadePallets)
                devolucaoValePallet.ValePallet.Devolucao.QuantidadePallets -= devolucaoValePallet.QuantidadePallets;
            else
            {
                devolucaoValePallet.ValePallet.Devolucao.NumeroDevolucao = repositorioDevolucaoPallet.BuscarProximoCodigo();
                devolucaoValePallet.ValePallet.Devolucao.DataDevolucao = System.DateTime.Now;
                devolucaoValePallet.ValePallet.Devolucao.Filial = devolucaoValePallet.Filial;
                devolucaoValePallet.ValePallet.Devolucao.Observacao = $"Devolução realizada com o vale pallet n° {devolucaoValePallet.ValePallet.Numero}";
                devolucaoValePallet.ValePallet.Devolucao.Usuario = _auditado.Usuario;
                devolucaoValePallet.ValePallet.Devolucao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue;
            }

            repositorioDevolucaoPallet.Atualizar(devolucaoValePallet.ValePallet.Devolucao, _auditado);
        }

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet, List<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var existeRegraSemAprovacao = false;
            var repositorio = new Repositorio.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (var regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet aprovacao = new Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet()
                        {
                            OrigemAprovacao = devolucaoValePallet,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,
                            DataCriacao = devolucaoValePallet.Data,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(devolucaoValePallet, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet()
                    {
                        OrigemAprovacao = devolucaoValePallet,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = devolucaoValePallet.Data,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            devolucaoValePallet.Situacao = existeRegraSemAprovacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.AguardandoAprovacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.Finalizada;
        }

        private List<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet)
        {
            var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet>(_unitOfWork);
            var listaRegras = repositorioRegra.BuscarPorAtiva();
            var listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet>();

            foreach (var regra in listaRegras)
            {
                if (regra.RegraPorDiasDevolucao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaDiasDevolucao, int>(regra.AlcadasDiasDevolucao, (devolucaoValePallet.Data.Date.Subtract(devolucaoValePallet.ValePallet.DataLancamento.Date).TotalDays)))
                    continue;

                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, devolucaoValePallet.Filial.Codigo))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet, Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: devolucaoValePallet.Codigo,
                URLPagina: "Pallets/DevolucaoValePallet",
                titulo: Localization.Resources.Avarias.DevolucaoValePallet.DevolucaoValePallets,
                nota: string.Format(Localization.Resources.Avarias.DevolucaoValePallet.CriadaDevolucaoValePallet,devolucaoValePallet.Codigo),
                icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra,
                tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var regras = ObterRegrasAutorizacao(devolucaoValePallet);

            if (regras.Count > 0)
            {
                CriarRegrasAprovacao(devolucaoValePallet, regras, tipoServicoMultisoftware);
                InserirMovimentacaoEstoque(devolucaoValePallet);
            }
            else
                devolucaoValePallet.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.SemRegraAprovacao;
        }

        public void InserirMovimentacaoEstoque(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet)
        {
            if (devolucaoValePallet.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.Finalizada)
            {
                AtualizarDevolucaoPallet(devolucaoValePallet);

                var servicoEstoque = new EstoquePallet(_unitOfWork);

                var movimentacaoTransportadorFilial = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CodigoFilial = devolucaoValePallet.Filial.Codigo,
                    CodigoSetor = devolucaoValePallet.Setor.Codigo,
                    CodigoTransportador = devolucaoValePallet.ValePallet.Devolucao.Transportador.Codigo,
                    DevolucaoPallet = devolucaoValePallet.ValePallet.Devolucao,
                    Quantidade = devolucaoValePallet.QuantidadePallets,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial,
                    CodigoGrupoPessoas = devolucaoValePallet.ValePallet?.Devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
                };

                servicoEstoque.InserirMovimentacao(movimentacaoTransportadorFilial);

                var cpfCnpjCliente = devolucaoValePallet.ValePallet.Devolucao.XMLNotaFiscal?.Destinatario?.CPF_CNPJ ?? throw new Dominio.Excecoes.Embarcador.ServicoException("A nota fiscal vinculada a devolução não possui destinatário. Impossível realizar a movimentação de estoque");
                var movimentacaoFilialCliente = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CodigoFilial = devolucaoValePallet.Filial.Codigo,
                    CodigoSetor = devolucaoValePallet.Setor.Codigo,
                    CpfCnpjCliente = cpfCnpjCliente,
                    DevolucaoPallet = devolucaoValePallet.ValePallet.Devolucao,
                    Quantidade = devolucaoValePallet.QuantidadePallets,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.FilialCliente,
                    CodigoGrupoPessoas = devolucaoValePallet.ValePallet?.Devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
                };

                servicoEstoque.InserirMovimentacao(movimentacaoFilialCliente);
            }
        }

        #endregion
    }
}
