namespace MultiSoftware.EDI
{
    public class EDIBase
    {
        private static string _stringConexao;
        public static string StringConexao
        {
            get
            {
                return _stringConexao;
            }
        }

        private static Repositorio.UnitOfWork _unitOfWork;
        public static Repositorio.UnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
        }

        public EDIBase(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            _stringConexao = stringConexao;
            _unitOfWork = unitOfWork;
        }
    }
}
