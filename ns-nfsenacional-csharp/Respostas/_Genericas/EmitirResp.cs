using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_nfsenacional_csharp.Respostas._Genericas
{
    public class EmitirResp
    {
		public string status { get; set; }
		public string motivo { get; set; }
		public string xMotivo { get; set; }
		public IList<string> erros { get; set; }
		public Erro erro { get; set; }
		public string nsNRec { get; set; }
	}
}
