using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos
{
    public class ServicoBase
    {
        private string _stringConexao;
        public string StringConexao
        {
            get
            {
                return _stringConexao;
            }
        }

        public ServicoBase(string stringConexao)
        {
            _stringConexao = stringConexao;
        }
    }
}
