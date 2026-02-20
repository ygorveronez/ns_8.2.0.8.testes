using System.Collections.Generic;

namespace Servicos.WebService.Carga
{
    public class TipoOperacao
    {
        #region Propriedades Privadas

        private Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public TipoOperacao(string stringConexao) { }
        public TipoOperacao(Repositorio.UnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscarTiposOperacaoPorCNPJ(string cnpj)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cnpj)));

            if (cliente != null)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>.CriarRetornoSucesso(RetornarTiposDeOperacao(repTipoOperacao.BuscarPorCliente(cliente)));
            else
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>.CriarRetornoDadosInvalidos("O CNPJ " + cnpj + " não existe na base da Multisoftware.", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao> RetornarTiposDeOperacao(List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao> tiposOperacaoDyn = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao in TiposOperacao)
            {
                tiposOperacaoDyn.Add(ConverterObjetoTipoOperacao(TipoOperacao));
            }

            return tiposOperacaoDyn;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao ConverterObjetoTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao tipoOperacaoDyn = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao();
                tipoOperacaoDyn.CNPJsDaOperacao = new List<string>();
                tipoOperacaoDyn.CodigoIntegracao = tipoOperacao.CodigoIntegracao;
                tipoOperacaoDyn.Descricao = tipoOperacao.Descricao;

                tipoOperacaoDyn.TipoCobrancaMultimodal = tipoOperacao.TipoCobrancaMultimodal;
                tipoOperacaoDyn.ModalPropostaMultimodal = tipoOperacao.ModalPropostaMultimodal;
                tipoOperacaoDyn.TipoServicoMultimodal = tipoOperacao.TipoServicoMultimodal;
                tipoOperacaoDyn.TipoPropostaMultimodal = tipoOperacao.TipoPropostaMultimodal;

                if (tipoOperacao.GrupoPessoas != null)
                {
                    foreach (Dominio.Entidades.Cliente cliente in tipoOperacao.GrupoPessoas.Clientes)
                    {
                        tipoOperacaoDyn.CNPJsDaOperacao.Add(cliente.CPF_CNPJ_SemFormato);
                    }
                }
                if (tipoOperacao.Pessoa != null)
                {
                    if (!tipoOperacaoDyn.CNPJsDaOperacao.Contains(tipoOperacao.Pessoa.CPF_CNPJ_SemFormato))
                        tipoOperacaoDyn.CNPJsDaOperacao.Add(tipoOperacao.Pessoa.CPF_CNPJ_SemFormato);
                }

                return tipoOperacaoDyn;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
