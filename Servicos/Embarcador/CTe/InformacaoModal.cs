using Repositorio;
using System;

namespace Servicos.Embarcador.CTe
{
    public class InformacaoModal : ServicoBase
    {        
        public InformacaoModal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoModal ConverterDynamicInformacaoModal(dynamic dynInformacaoModal, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoModal informacaoModal = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoModal();

            informacaoModal.PortoOrigem = ((string)dynInformacaoModal.PortoOrigem).ToInt();
            informacaoModal.PortoPassagemUm = ((string)dynInformacaoModal.PortoPassagemUm).ToInt();
            informacaoModal.PortoPassagemDois = ((string)dynInformacaoModal.PortoPassagemDois).ToInt();
            informacaoModal.PortoPassagemTres = ((string)dynInformacaoModal.PortoPassagemTres).ToInt();
            informacaoModal.PortoPassagemQuatro = ((string)dynInformacaoModal.PortoPassagemQuatro).ToInt();
            informacaoModal.PortoPassagemCinco = ((string)dynInformacaoModal.PortoPassagemCinco).ToInt();
            informacaoModal.PortoDestino = ((string)dynInformacaoModal.PortoDestino).ToInt();
            informacaoModal.TerminalOrigem = ((string)dynInformacaoModal.TerminalOrigem).ToInt();
            informacaoModal.TerminalDestino = ((string)dynInformacaoModal.TerminalDestino).ToInt();
            informacaoModal.Viagem = ((string)dynInformacaoModal.Viagem).ToInt();
            informacaoModal.NumeroControle = (string)dynInformacaoModal.NumeroControle;
            informacaoModal.NumeroBooking = (string)dynInformacaoModal.NumeroBooking;
            informacaoModal.DescricaoCarrier = (string)dynInformacaoModal.DescricaoCarrier;
            informacaoModal.TipoPropostaFeeder = ((string)dynInformacaoModal.TipoPropostaFeeder).ToEnum<Dominio.Enumeradores.TipoPropostaFeeder>();
            informacaoModal.OcorreuSinistroAvaria = ((string)dynInformacaoModal.OcorreuSinistroAvaria).ToBool();

            return informacaoModal;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoModal ConverterInformacaoCTeParaInformacaoModal(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoModal informacaoModal = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoModal()
            {
                CodigoDocumentacaoPortoOrigem = cte.PortoOrigem?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoPortoPassagemUm = cte.PortoPassagemUm?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoPortoPassagemDois = cte.PortoPassagemDois?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoPortoPassagemTres = cte.PortoPassagemTres?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoPortoPassagemQuatro = cte.PortoPassagemQuatro?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoPortoPassagemCinco = cte.PortoPassagemCinco?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoPortoDestino = cte.PortoDestino?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoTerminalOrigem = cte.TerminalOrigem?.CodigoDocumento ?? string.Empty,
                CodigoDocumentacaoTerminalDestino = cte.TerminalDestino?.CodigoDocumento ?? string.Empty,
                DescricaoViagem = cte.Viagem?.Descricao ?? string.Empty,
                NumeroBooking = cte.NumeroBooking,
                DescricaoCarrier = cte.DescricaoCarrier,
                TipoPropostaFeeder = cte.TipoPropostaFeeder,
                OcorreuSinistroAvaria = cte.OcorreuSinistroAvaria
            };
            return informacaoModal;
        }

        public void SalvarInformacaoModal(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoModal informacaoModal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);

            cte.PortoOrigem = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoPortoOrigem) ? repPorto.BuscarPorCodigoDocumento(informacaoModal.CodigoDocumentacaoPortoOrigem) : informacaoModal.PortoOrigem > 0 ? repPorto.BuscarPorCodigo(informacaoModal.PortoOrigem) : null;
            cte.PortoPassagemUm = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoPortoPassagemUm) ? repPorto.BuscarPorCodigoDocumento(informacaoModal.CodigoDocumentacaoPortoPassagemUm) : informacaoModal.PortoPassagemUm > 0 ? repPorto.BuscarPorCodigo(informacaoModal.PortoPassagemUm) : null;
            cte.PortoPassagemDois = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoPortoPassagemDois) ? repPorto.BuscarPorCodigoDocumento(informacaoModal.CodigoDocumentacaoPortoPassagemDois) : informacaoModal.PortoPassagemDois > 0 ? repPorto.BuscarPorCodigo(informacaoModal.PortoPassagemDois) : null;
            cte.PortoPassagemTres = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoPortoPassagemTres) ? repPorto.BuscarPorCodigoDocumento(informacaoModal.CodigoDocumentacaoPortoPassagemTres) : informacaoModal.PortoPassagemTres > 0 ? repPorto.BuscarPorCodigo(informacaoModal.PortoPassagemTres) : null;
            cte.PortoPassagemQuatro = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoPortoPassagemQuatro) ? repPorto.BuscarPorCodigoDocumento(informacaoModal.CodigoDocumentacaoPortoPassagemQuatro) : informacaoModal.PortoPassagemQuatro > 0 ? repPorto.BuscarPorCodigo(informacaoModal.PortoPassagemQuatro) : null;
            cte.PortoPassagemCinco = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoPortoPassagemCinco) ? repPorto.BuscarPorCodigoDocumento(informacaoModal.CodigoDocumentacaoPortoPassagemCinco) : informacaoModal.PortoPassagemCinco > 0 ? repPorto.BuscarPorCodigo(informacaoModal.PortoPassagemCinco) : null;
            cte.PortoDestino = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoPortoDestino) ? repPorto.BuscarPorCodigoDocumento(informacaoModal.CodigoDocumentacaoPortoDestino) : informacaoModal.PortoDestino > 0 ? repPorto.BuscarPorCodigo(informacaoModal.PortoDestino) : null;
            cte.TerminalOrigem = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoTerminalOrigem) ? repTipoTerminalImportacao.BuscarTodosPorCodigoDocumento(informacaoModal.CodigoDocumentacaoTerminalOrigem) : informacaoModal.TerminalOrigem > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(informacaoModal.TerminalOrigem) : null;
            cte.TerminalDestino = !string.IsNullOrWhiteSpace(informacaoModal.CodigoDocumentacaoTerminalDestino) ? repTipoTerminalImportacao.BuscarTodosPorCodigoDocumento(informacaoModal.CodigoDocumentacaoTerminalDestino) : informacaoModal.TerminalDestino > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(informacaoModal.TerminalDestino) : null;
            cte.Viagem = !string.IsNullOrWhiteSpace(informacaoModal.DescricaoViagem) ? repPedidoViagemNavio.BuscarPorDescricao(informacaoModal.DescricaoViagem) : informacaoModal.Viagem > 0 ? repPedidoViagemNavio.BuscarPorCodigo(informacaoModal.Viagem) : null;
            //cte.NumeroControle = informacaoModal.NumeroControle;
            cte.NumeroBooking = informacaoModal.NumeroBooking;
            cte.DescricaoCarrier = informacaoModal.DescricaoCarrier;
            cte.TipoPropostaFeeder = informacaoModal.TipoPropostaFeeder;
            cte.OcorreuSinistroAvaria = informacaoModal.OcorreuSinistroAvaria;
            if (cte.Navio == null && cte.Viagem != null && cte.Viagem.Navio != null)
                cte.Navio = cte.Viagem.Navio;

            cte.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cte.ViagemPassagemCinco?.Codigo ?? 0, cte.ViagemPassagemQuatro?.Codigo ?? 0, cte.ViagemPassagemTres?.Codigo ?? 0, cte.ViagemPassagemDois?.Codigo ?? 0, cte.ViagemPassagemUm?.Codigo ?? 0, cte.Viagem?.Codigo ?? 0, cte.PortoDestino?.Codigo ?? 0, cte.TerminalDestino?.Codigo ?? 0));
        }
    }
}
