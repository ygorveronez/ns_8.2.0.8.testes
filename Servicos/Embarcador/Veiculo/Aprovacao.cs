using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Veiculo
{
    public sealed class Aprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo,
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo,
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo
    >
    {
        #region Construtores

        public Aprovacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculo, List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var existeRegraSemAprovacao = false;
            var repositorio = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (var regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo aprovacao = new Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo()
                        {
                            OrigemAprovacao = cadastroVeiculo,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,
                            DataCriacao = DateTime.Now,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(cadastroVeiculo, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo()
                    {
                        OrigemAprovacao = cadastroVeiculo,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = DateTime.Now,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            cadastroVeiculo.Veiculo.SituacaoCadastro = existeRegraSemAprovacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.Pendente : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.Aprovado;
        }

        private List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculo)
        {
            var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo>(_unitOfWork);
            var listaRegras = repositorioRegra.BuscarPorAtiva();
            var listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo>();

            foreach (var regra in listaRegras)
            {
                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, cadastroVeiculo.Veiculo.Empresa?.Codigo ?? 0))
                    continue;

                if (regra.RegraPorModeloVeicular && !ValidarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaModeloVeicular, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(regra.AlcadasModeloVeicular, cadastroVeiculo.Veiculo.ModeloVeicularCarga?.Codigo ?? 0))
                    continue;

                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, cadastroVeiculo.Veiculo.FilialCarregamento?.Codigo ?? 0))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculo, Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: cadastroVeiculo.Codigo,
                URLPagina: "Veiculos/AutorizacaoCadastroVeiculo",
                titulo: Localization.Resources.Veiculos.Aprovacao.CadastroVeiculo,
                nota: string.Format(Localization.Resources.Veiculos.Aprovacao.UsuarioAcaoVeiculoRequerAprovacao, cadastroVeiculo.Usuario.Nome, cadastroVeiculo.Tipo.ObterDescricaoAcao(), cadastroVeiculo.Veiculo.Placa),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var regras = ObterRegrasAutorizacao(cadastroVeiculo);

            if (regras.Count > 0)
                CriarRegrasAprovacao(cadastroVeiculo, regras, tipoServicoMultisoftware);
            else
                cadastroVeiculo.Veiculo.SituacaoCadastro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.SemRegraAprovacao;
        }

        #endregion
    }
}
