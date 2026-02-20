using Dominio.Interfaces.Database;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class ComplementoFrete : ServicoBase
    {        
        public ComplementoFrete(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public void UtilizarCargaComplementoFrete(Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Repositorio.Embarcador.Creditos.SolicitacaoCredito repSolicitacaoCredito = new Repositorio.Embarcador.Creditos.SolicitacaoCredito(unitOfWork);


            if (serCarga.VerificarSeCargaEstaNaLogistica(cargaComplementoFrete.Carga, tipoServicoMultisoftware))
            {
                serComponetesFrete.AdicionarComponenteFreteCarga(cargaComplementoFrete.Carga, cargaComplementoFrete.ComponenteFrete, cargaComplementoFrete.ValorComplemento, 0, cargaComplementoFrete.ComponenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, cargaComplementoFrete.ComponenteFrete.TipoComponenteFrete, cargaComplementoFrete, true, false, null, tipoServicoMultisoftware, cargaComplementoFrete.Usuario, unitOfWork, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, false);
                cargaComplementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Utilizada;
                repCargaComplementoFrete.Atualizar(cargaComplementoFrete);
            }
            else
            {
                cargaComplementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgEmissaoCTeComplementar;
                repCargaComplementoFrete.Atualizar(cargaComplementoFrete);

                if (cargaComplementoFrete.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && cargaComplementoFrete.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                {
                    Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new ComplementoFrete(unitOfWork);
                    serComplementoFrete.VerificarCargaComplementoFretePendentesCTesComplementares(cargaComplementoFrete.Carga, unitOfWork, tipoServicoMultisoftware);
                }
            }

            if (cargaComplementoFrete.SolicitacaoCredito != null)
            {
                cargaComplementoFrete.SolicitacaoCredito.SituacaoSolicitacaoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Utilizado;
                repSolicitacaoCredito.Atualizar(cargaComplementoFrete.SolicitacaoCredito);
            }
        }

        public void ExtornarComplementoDeFrete(Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Credito.SolicitacaoCredito serSolicitacaoCredito = new Credito.SolicitacaoCredito(unitOfWork);
            Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Credito.CreditoMovimentacao(unitOfWork);

            if (cargaComplementoFrete.SituacaoComplementoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Rejeitada && cargaComplementoFrete.SituacaoComplementoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Estornada)
            {
                if (cargaComplementoFrete.SolicitacaoCredito != null)
                {
                    serSolicitacaoCredito.ExtornarSolicitacaoCredito(cargaComplementoFrete.SolicitacaoCredito, unitOfWork);
                }

                cargaComplementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Estornada;
                repCargaComplementoFrete.Atualizar(cargaComplementoFrete);

                List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorCreditoComplementoDeFrete(cargaComplementoFrete.Codigo);
                serCreditoMovimentacao.ExtornarCreditos(creditosUtilizadosDestino, tipoServicoMultisoftware, unitOfWork);
            }
            int numero = repCargaComplementoFrete.ContarNumeroComplementosNaoExtornadosOuRejeitados(cargaComplementoFrete.Carga.Codigo);
            if (numero == 0)
            {
                cargaComplementoFrete.Carga.AgConfirmacaoUtilizacaoCredito = false;
                repCarga.Atualizar(cargaComplementoFrete.Carga);
            }
        }


        public void VerificarCargaComplementoFretePendentesCTesComplementares(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicosMultiSoftware)
        {
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> cargaComplementoFreteAgEmissao = repCargaComplementoFrete.BuscarComplementosPendentesEmissaoCTeComplementar(carga.Codigo);
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete in cargaComplementoFreteAgEmissao)
            {

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, true);
                serCargaCTeComplementar.CriarCargaCTeComplementoInfo(cargaCTEs, cargaComplementoFrete.ValorComplemento, "", null, true, cargaComplementoFrete, cargaComplementoFrete.ComponenteFrete, unitOfWork, tipoServicosMultiSoftware, false);
                cargaComplementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.EmEmissaoCTeComplementar;
                repCargaComplementoFrete.Atualizar(cargaComplementoFrete);
            }
        }


        public string ValidarEmissaoCargaComplementoFrete(Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Repositorio.UnitOfWork unitOfWork, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string retorno = "";

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorComplementoDeFrete(cargaComplementoFrete.Codigo);

            bool emitiuTodos = true;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> ctesParaEmissao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe == null)
                {
                    emitiuTodos = false;
                    ctesParaEmissao.Add(cargaCTeComplementoInfo);
                }
                else
                {
                    if (cargaCTeComplementoInfo.CTe.Status != "A")
                        emitiuTodos = false;
                }
            }

            if (emitiuTodos)
            {
                Servicos.Embarcador.Carga.RateioProduto serRateioProduto = new Servicos.Embarcador.Carga.RateioProduto(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
                {
                    //serRateioProduto.RatearProdutoPorCargaCTe(cargaComplementoFrete.Carga, cargaCTeComplementoInfo.CargaCTe, unitOfWork, tipoServicoMultisoftware);: todo rever para mudar isso não é usado atualmente.
                }

                cargaComplementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Utilizada;
                repCargaComplementoFrete.Atualizar(cargaComplementoFrete);
            }

            if (ctesParaEmissao.Count > 0)
            {
                Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new CTeComplementar(unitOfWork);
                serCargaCTeComplementar.EmitirDocumentoComplementar(ctesParaEmissao, unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware);
            }
            //else
            //{
            //    unitOfWork.CommitChanges();
            //}

            return retorno;
        }

    }
}
