using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.GestaoPatio
{
    sealed class FluxoGestaoPatioEtapaFactory
    {
        public static FluxoGestaoPatioEtapa CriarEtapa(EtapaFluxoGestaoPatio etapaFluxo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            switch (etapaFluxo)
            {
                case EtapaFluxoGestaoPatio.AvaliacaoDescarga:
                    return new AvaliacaoDescarga(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.CheckList:
                    return new CheckList(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.ChegadaLoja:
                    return new ChegadaLoja(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                    return new ChegadaVeiculo(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.DeslocamentoPatio:
                    return new DeslocamentoPatio(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.DocumentoFiscal:
                    return new DocumentoFiscal(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.DocumentosTransporte:
                    return new DocumentosTransporte(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.Expedicao:
                    return new Expedicao(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.Faturamento:
                    return new Faturamento(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.FimCarregamento:
                    return new FimCarregamento(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.FimDescarregamento:
                    return new FimDescarregamento(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.FimHigienizacao:
                    return new FimHigienizacao(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.FimViagem:
                    return new FimViagem(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.Guarita:
                    return new Guarita(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.InformarDoca:
                    return new InformarDoca(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.InicioCarregamento:
                    return new InicioCarregamento(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.InicioDescarregamento:
                    return new InicioDescarregamento(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.InicioHigienizacao:
                    return new InicioHigienizacao(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.InicioViagem:
                    return new InicioViagem(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.LiberacaoChave:
                    return new LiberacaoChave(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.MontagemCarga:
                    return new MontagemCargaPatio(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.Posicao:
                    return new RastreamentoCarga(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.SaidaLoja:
                    return new SaidaLoja(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.SeparacaoMercadoria:
                    return new SeparacaoMercadoria(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.SolicitacaoVeiculo:
                    return new SolicitacaoVeiculo(unitOfWork, auditado, cliente);

                case EtapaFluxoGestaoPatio.TravamentoChave:
                    return new TravamentoChave(unitOfWork, auditado, cliente);

                default:
                    return null;
            }
        }

        public static IFluxoGestaoPatioEtapaAdicionar CriarEtapaAdicionar(EtapaFluxoGestaoPatio etapaFluxo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            switch (etapaFluxo)
            {
                case EtapaFluxoGestaoPatio.AvaliacaoDescarga:
                    return new AvaliacaoDescarga(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.CheckList:
                    return new CheckList(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                    return new ChegadaVeiculo(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.DocumentoFiscal:
                    return new DocumentoFiscal(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.DocumentosTransporte:
                    return new DocumentosTransporte(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Expedicao:
                    return new Expedicao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Faturamento:
                    return new Faturamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.FimCarregamento:
                    return new FimCarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.FimDescarregamento:
                    return new FimDescarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.FimHigienizacao:
                    return new FimHigienizacao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Guarita:
                    return new Guarita(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InformarDoca:
                    return new InformarDoca(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioCarregamento:
                    return new InicioCarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioDescarregamento:
                    return new InicioDescarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioHigienizacao:
                    return new InicioHigienizacao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioViagem:
                    return new InicioViagem(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.LiberacaoChave:
                    return new LiberacaoChave(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.MontagemCarga:
                    return new MontagemCargaPatio(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Posicao:
                    return new RastreamentoCarga(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.SeparacaoMercadoria:
                    return new SeparacaoMercadoria(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.SolicitacaoVeiculo:
                    return new SolicitacaoVeiculo(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.TravamentoChave:
                    return new TravamentoChave(unitOfWork, auditado);

                default:
                    return null;
            }
        }

        public static IFluxoGestaoPatioEtapaAlterarCarga CriarEtapaAlterarCarga(EtapaFluxoGestaoPatio etapaFluxo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            switch (etapaFluxo)
            {
                case EtapaFluxoGestaoPatio.AvaliacaoDescarga:
                    return new AvaliacaoDescarga(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.CheckList:
                    return new CheckList(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                    return new ChegadaVeiculo(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.DocumentoFiscal:
                    return new DocumentoFiscal(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.DocumentosTransporte:
                    return new DocumentosTransporte(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Expedicao:
                    return new Expedicao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Faturamento:
                    return new Faturamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.FimCarregamento:
                    return new FimCarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.FimDescarregamento:
                    return new FimDescarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.FimHigienizacao:
                    return new FimHigienizacao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Guarita:
                    return new Guarita(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InformarDoca:
                    return new InformarDoca(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioCarregamento:
                    return new InicioCarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioDescarregamento:
                    return new InicioDescarregamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioHigienizacao:
                    return new InicioHigienizacao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioViagem:
                    return new InicioViagem(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.LiberacaoChave:
                    return new LiberacaoChave(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.MontagemCarga:
                    return new MontagemCargaPatio(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Posicao:
                    return new RastreamentoCarga(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.SeparacaoMercadoria:
                    return new SeparacaoMercadoria(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.SolicitacaoVeiculo:
                    return new SolicitacaoVeiculo(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.TravamentoChave:
                    return new TravamentoChave(unitOfWork, auditado);

                default:
                    return null;
            }
        }

        public static IFluxoGestaoPatioEtapaLiberarAutomaticamente CriarEtapaLiberarAutomaticamente(EtapaFluxoGestaoPatio etapaFluxo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            switch (etapaFluxo)
            {
                case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                    return new ChegadaVeiculo(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.DocumentosTransporte:
                    return new DocumentosTransporte(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.Faturamento:
                    return new Faturamento(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.FimHigienizacao:
                    return new FimHigienizacao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.InicioHigienizacao:
                    return new InicioHigienizacao(unitOfWork, auditado);

                case EtapaFluxoGestaoPatio.DocumentoFiscal:
                    return new DocumentoFiscal(unitOfWork, auditado);

                default:
                    return null;
            }
        }

        public static IFluxoGestaoPatioEtapaRetornada CriarEtapaRetornada(EtapaFluxoGestaoPatio etapaFluxo, Repositorio.UnitOfWork unitOfWork)
        {
            switch (etapaFluxo)
            {
                case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                    return new ChegadaVeiculo(unitOfWork, null);

                case EtapaFluxoGestaoPatio.Guarita:
                    return new Guarita(unitOfWork, null);

                case EtapaFluxoGestaoPatio.InicioViagem:
                    return new InicioViagem(unitOfWork, null);

                case EtapaFluxoGestaoPatio.LiberacaoChave:
                    return new LiberacaoChave(unitOfWork, null);

                case EtapaFluxoGestaoPatio.TravamentoChave:
                    return new TravamentoChave(unitOfWork, null);

                default:
                    return null;
            }
        }
    }
}