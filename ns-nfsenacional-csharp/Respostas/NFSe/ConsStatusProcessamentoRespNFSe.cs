using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns_nfsenacional_csharp.Respostas._Genericas;

namespace ns_nfsenacional_csharp.Respostas.NFSe
{
    public class ConsStatusProcessamentoRespNFSe : ConsStatusProcessamentoResp
    {
		public string chDPS { get; set; }
		public string chNFSe { get; set; }
	}
}
