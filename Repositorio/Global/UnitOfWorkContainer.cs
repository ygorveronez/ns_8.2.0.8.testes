using System;
using System.Data;

namespace Repositorio
{
    public sealed class UnitOfWorkContainer : IDisposable
    {
        #region Atributos

        public bool TransacaoPorContainerAtiva { get; private set; }

        public UnitOfWork UnitOfWork { get; private set; }

        #endregion

        #region Construtores

        public UnitOfWorkContainer(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;

            if (unitOfWork.IsActiveTransaction())
                TransacaoPorContainerAtiva = true;
        }

        #endregion

        #region Métodos Públicos

        public void CommitChanges()
        {
            if (!TransacaoPorContainerAtiva)
                UnitOfWork.CommitChanges();
        }

        public void CommitChangesContainer()
        {
            TransacaoPorContainerAtiva = false;

            UnitOfWork.CommitChanges();
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }

        public void Rollback()
        {
            if (!TransacaoPorContainerAtiva)
                UnitOfWork.Rollback();
        }

        public void RollbackContainer()
        {
            TransacaoPorContainerAtiva = false;

            UnitOfWork.Rollback();
        }

        public void Start()
        {
            if (!TransacaoPorContainerAtiva)
                UnitOfWork.Start();
        }

        public void Start(IsolationLevel isolation)
        {
            if (!TransacaoPorContainerAtiva)
                UnitOfWork.Start(isolation);
        }

        public void StartContainer()
        {
            TransacaoPorContainerAtiva = true;

            UnitOfWork.Start();
        }

        public void StartContainer(IsolationLevel isolation)
        {
            TransacaoPorContainerAtiva = true;

            UnitOfWork.Start(isolation);
        }

        #endregion
    }
}
