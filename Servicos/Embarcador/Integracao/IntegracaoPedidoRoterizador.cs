using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao;

public class IntegracaoPedidoRoterizador
{
    #region Propriedades

    private readonly Repositorio.UnitOfWork _unitOfWork;
    private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

    #endregion Propriedades

    #region Construtores

    public IntegracaoPedidoRoterizador(Repositorio.UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IntegracaoPedidoRoterizador(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
    {
        _unitOfWork = unitOfWork;
        _auditado = auditado;
    }

    #endregion Construtores

    #region Métodos Publicos

    public void AdicionarParaIntegracaoAutomaticamente(int codigoPedido, TipoRoteirizadorIntegracao tipoRoterizadorIntegracao)
    {
        AdicionarParaIntegracaoAutomaticamente(new List<int> { codigoPedido }, tipoRoterizadorIntegracao);
    }

    public void AdicionarParaIntegracaoAutomaticamente(List<int> codigosPedidos, TipoRoteirizadorIntegracao tipoRoterizadorIntegracao)
    {
        Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(_unitOfWork);
        Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido repositorioRoteirizadorIntegracaoPedido = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido(_unitOfWork);

        List<int> codigosPedidosIntegrar;

        if (tipoRoterizadorIntegracao == TipoRoteirizadorIntegracao.CancelarPedido)
            codigosPedidosIntegrar = repositorioRoteirizadorIntegracaoPedido.BuscarCodigoPedidosIntegradosRoteirizador(codigosPedidos);
        else
            codigosPedidosIntegrar = codigosPedidos;

        if (codigosPedidosIntegrar.Count == 0)
            return;

        Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao = new Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao()
        {
            Tipo = tipoRoterizadorIntegracao,
            Usuario = _auditado?.Usuario,
            ProblemaIntegracao = string.Empty,
            SituacaoIntegracao = SituacaoIntegracao.AgIntegracao
        };

        repositorioRoteirizadorIntegracao.Inserir(roteirizadorIntegracao);
        repositorioRoteirizadorIntegracaoPedido.InserirPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo, codigosPedidosIntegrar);
    }

    public void AdicionarEIntegrarPedidos(List<int> codigosPedidos, TipoRoteirizadorIntegracao tipoRoterizadorIntegracao)
    {
        Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao = AdicionarRoteirizadorIntegracao(codigosPedidos, tipoRoterizadorIntegracao);

        Servicos.Embarcador.Integracao.RoutEasy.IntegracaoRoutEasy integracaoRouteasy = new Servicos.Embarcador.Integracao.RoutEasy.IntegracaoRoutEasy(_unitOfWork, _auditado);

        switch (tipoRoterizadorIntegracao)
        {
            case TipoRoteirizadorIntegracao.EnviarPedido:
                integracaoRouteasy.IntegrarPedidos(roteirizadorIntegracao);
                break;
            case TipoRoteirizadorIntegracao.AtualizarPedido:
                integracaoRouteasy.AtualizarPedidos(roteirizadorIntegracao);
                break;
            case TipoRoteirizadorIntegracao.CancelarPedido:
                integracaoRouteasy.IntegrarCancelamentoPedidos(roteirizadorIntegracao);
                break;
        }

        if (roteirizadorIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            throw new ServicoException(roteirizadorIntegracao.ProblemaIntegracao);
    }

    public void VerificarIntegracaoesRoteirizadorPendentes()
    {
        Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(_unitOfWork);

        int numeroRegistrosPorVez = 15;

        List<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao> integracoesPendentes = repositorioRoteirizadorIntegracao.BuscarIntegracoesPendentes(numeroRegistrosPorVez);

        foreach (Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao integracaoPendente in integracoesPendentes)
        {
            switch (integracaoPendente.Tipo)
            {
                case TipoRoteirizadorIntegracao.EnviarPedido:
                    new Servicos.Embarcador.Integracao.RoutEasy.IntegracaoRoutEasy(_unitOfWork).IntegrarPedidos(integracaoPendente);
                    break;
                case TipoRoteirizadorIntegracao.AtualizarPedido:
                    new Servicos.Embarcador.Integracao.RoutEasy.IntegracaoRoutEasy(_unitOfWork).AtualizarPedidos(integracaoPendente);
                    break;
                case TipoRoteirizadorIntegracao.CancelarPedido:
                    new Servicos.Embarcador.Integracao.RoutEasy.IntegracaoRoutEasy(_unitOfWork).IntegrarCancelamentoPedidos(integracaoPendente);
                    break;
            }
        }
    }

    #endregion Métodos Publicos

    #region Métodos Privados

    private Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao AdicionarRoteirizadorIntegracao(List<int> codigosPedidos, TipoRoteirizadorIntegracao tipoRoterizadorIntegracao)
    {
        Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(_unitOfWork);
        Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido repositorioRoteirizadorIntegracaoPedido = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido(_unitOfWork);

        List<int> codigosPedidosIntegrar;

        if (tipoRoterizadorIntegracao == TipoRoteirizadorIntegracao.CancelarPedido)
            codigosPedidosIntegrar = repositorioRoteirizadorIntegracaoPedido.BuscarCodigoPedidosIntegradosRoteirizador(codigosPedidos);
        else
            codigosPedidosIntegrar = codigosPedidos;

        if (codigosPedidosIntegrar.Count == 0)
            throw new ServicoException("Não existem pedidos para serem integrados");

        Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao = new Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao()
        {
            Tipo = tipoRoterizadorIntegracao,
            Usuario = _auditado?.Usuario,
            ProblemaIntegracao = string.Empty
        };

        repositorioRoteirizadorIntegracao.Inserir(roteirizadorIntegracao);
        repositorioRoteirizadorIntegracaoPedido.InserirPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo, codigosPedidosIntegrar);

        return roteirizadorIntegracao;
    }

    #endregion Métodos Privados
}
