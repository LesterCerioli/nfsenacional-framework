using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns_nfsenacional_csharp.Requisicoes._Genericas;

namespace ns_nfsenacional_csharp.Requisicoes.NFSe
{
    public class DownloadReqNFSe : DownloadReq
    {
		public string chDPS { get; set; }
		public string chNFSe { get; set; }
	}
}
