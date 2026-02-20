using System.Collections.Generic;

namespace Servicos.Embarcador.Pallets
{
    public class Fechamento
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Fechamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void CancelarFinalizacaoFechamento(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            CancelarFinalizacaoEstoquePallet(fechamento);
            CancelarFinalizacaoAvaria(fechamento);
            CancelarFinalizacaoCompraPallets(fechamento);
            CancelarFinalizacaoDevolucao(fechamento);
            CancelarFinalizacaoDevolucaoValePallet(fechamento);
            CancelarFinalizacaoReforma(fechamento);
            CancelarFinalizacaoTransferencia(fechamento);
            CancelarFinalizacaoValePallet(fechamento);
        }

        public void GerarComposicaoFechamento(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            ComposicaoEstoquePallet(fechamento);
            ComposicaoAvaria(fechamento);
            ComposicaoCompraPallets(fechamento);
            ComposicaoDevolucao(fechamento);
            ComposicaoDevolucaoValePallet(fechamento);
            ComposicaoReforma(fechamento);
            ComposicaoTransferencia(fechamento);
            ComposicaoValePallet(fechamento);
        }

        public void FinalizarFechamento(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            FinalizarEstoquePallet(fechamento);
            FinalizarAvaria(fechamento);
            FinalizarCompraPallets(fechamento);
            FinalizarDevolucao(fechamento);
            FinalizarDevolucaoValePallet(fechamento);
            FinalizarReforma(fechamento);
            FinalizarTransferencia(fechamento);
            FinalizarValePallet(fechamento);
        }

        #endregion

        #region Métodos Privados Cancelar Finalização Fechamento

        private void CancelarFinalizacaoEstoquePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.EstoquePallet repEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> estoques = repEstoquePallet.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var estoque in estoques)
            {
                estoque.Fechamento = null;
                estoque.AdicionarAoFechamento = false;
                repEstoquePallet.Atualizar(estoque);
            }
        }

        private void CancelarFinalizacaoAvaria(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.AvariaPallet repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                dado.AdicionarAoFechamento = false;
                repositorio.Atualizar(dado);
            }
        }

        private void CancelarFinalizacaoCompraPallets(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.CompraPallets repositorio = new Repositorio.Embarcador.Pallets.CompraPallets(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                dado.AdicionarAoFechamento = false;
                repositorio.Atualizar(dado);
            }
        }

        private void CancelarFinalizacaoDevolucao(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.DevolucaoPallet repositorio = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                dado.AdicionarAoFechamento = false;
                repositorio.Atualizar(dado);
            }
        }

        private void CancelarFinalizacaoDevolucaoValePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.DevolucaoValePallet repositorio = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                dado.AdicionarAoFechamento = false;
                repositorio.Atualizar(dado);
            }
        }

        private void CancelarFinalizacaoReforma(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.Reforma.ReformaPallet repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                dado.AdicionarAoFechamento = false;
                repositorio.Atualizar(dado);
            }
        }

        private void CancelarFinalizacaoTransferencia(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.TransferenciaPallet repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                dado.AdicionarAoFechamento = false;
                repositorio.Atualizar(dado);
            }
        }

        private void CancelarFinalizacaoValePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.ValePallet repositorio = new Repositorio.Embarcador.Pallets.ValePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.ValePallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, null);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                dado.AdicionarAoFechamento = false;
                repositorio.Atualizar(dado);
            }
        }

        #endregion

        #region Métodos Privados Composição Fechamento

        private void ComposicaoEstoquePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.EstoquePallet repositorio = new Repositorio.Embarcador.Pallets.EstoquePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        private void ComposicaoAvaria(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.AvariaPallet repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        private void ComposicaoCompraPallets(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.CompraPallets repositorio = new Repositorio.Embarcador.Pallets.CompraPallets(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        private void ComposicaoDevolucao(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.DevolucaoPallet repositorio = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        private void ComposicaoDevolucaoValePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.DevolucaoValePallet repositorio = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        private void ComposicaoReforma(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.Reforma.ReformaPallet repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        private void ComposicaoTransferencia(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.TransferenciaPallet repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        private void ComposicaoValePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.ValePallet repositorio = new Repositorio.Embarcador.Pallets.ValePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.ValePallet> dados = repositorio.BuscarPorData(fechamento.DataInicial, fechamento.DataFinal);

            foreach (var dado in dados)
            {
                dado.Fechamento = fechamento;
                dado.AdicionarAoFechamento = true;

                repositorio.Atualizar(dado);
            }
        }

        #endregion

        #region Métodos Privados Finalizar Fechamento

        private void FinalizarEstoquePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.EstoquePallet repEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> estoques = repEstoquePallet.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var estoque in estoques)
            {
                estoque.Fechamento = null;
                repEstoquePallet.Atualizar(estoque);
            }
        }

        private void FinalizarAvaria(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.AvariaPallet repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                repositorio.Atualizar(dado);
            }
        }

        private void FinalizarCompraPallets(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.CompraPallets repositorio = new Repositorio.Embarcador.Pallets.CompraPallets(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                repositorio.Atualizar(dado);
            }
        }

        private void FinalizarDevolucao(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.DevolucaoPallet repositorio = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                repositorio.Atualizar(dado);
            }
        }

        private void FinalizarDevolucaoValePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.DevolucaoValePallet repositorio = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                repositorio.Atualizar(dado);
            }
        }

        private void FinalizarReforma(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.Reforma.ReformaPallet repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                repositorio.Atualizar(dado);
            }
        }

        private void FinalizarTransferencia(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.TransferenciaPallet repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                repositorio.Atualizar(dado);
            }
        }

        private void FinalizarValePallet(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento)
        {
            Repositorio.Embarcador.Pallets.ValePallet repositorio = new Repositorio.Embarcador.Pallets.ValePallet(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pallets.ValePallet> dados = repositorio.BuscarPorFechamento(fechamento.Codigo, false);

            foreach (var dado in dados)
            {
                dado.Fechamento = null;
                repositorio.Atualizar(dado);
            }
        }

        #endregion
    }
}
