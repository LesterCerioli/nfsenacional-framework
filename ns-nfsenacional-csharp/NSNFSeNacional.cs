using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ns_nfsenacional_csharp.Compartilhados;
using ns_nfsenacional_csharp.Requisicoes._Genericas;
using ns_nfsenacional_csharp.Respostas._Genericas;
using ns_nfsenacional_csharp.Respostas.NFSe;
using ns_nfsenacional_csharp.Requisicoes.NFSe;
using ns_nfsenacional_csharp.Retornos.NFSe;

namespace ns_nfsenacional_csharp
{
	public class NSNFSeNacional
	{
		private static string token = "SEU_TOKEN_AQUI";
		private static Endpoints Endpoints = new Endpoints();
		private static Parametros Parametros = new Parametros();

		public static string enviaConteudoParaAPI(string conteudo, string url, string tpConteudo)
		{
			string retorno = "";
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			httpWebRequest.Headers["X-AUTH-TOKEN"] = token;

			if (tpConteudo == "txt")
			{
				httpWebRequest.ContentType = "text/plain;charset=utf-8";
			}
			else if (tpConteudo == "xml")
			{
				httpWebRequest.ContentType = "application/xml;charset=utf-8";
			}
			else
			{
				httpWebRequest.ContentType = "application/json;charset=utf-8";
			}

			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(conteudo);
				streamWriter.Flush();
				streamWriter.Close();
			}

			try
			{
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

				using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					retorno = streamReader.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse response = (HttpWebResponse)ex.Response;

					using (var streamReader = new StreamReader(response.GetResponseStream()))
					{
						retorno = streamReader.ReadToEnd();
					}

					switch (System.Convert.ToInt32(response.StatusCode))
					{
						case 401:
							{
								MessageBox.Show("Token não enviado ou inválido");
								break;
							}

						case 403:
							{
								MessageBox.Show("Token sem permissão");
								break;
							}

						case 404:
							{
								MessageBox.Show("Não encontrado, verifique o retorno para mais informações");
								break;
							}

						default:
							{
								break;
							}
					}
				}
			}

			return retorno;
		}

		public static string emitirDocumento(string conteudo, string tpConteudo, string cnpjEmitente, bool a3)
		{
			string urlEnvio;

			urlEnvio = Endpoints.NFSeEnvio;

			Genericos.gravarLinhaLog("NFSE: ", "[ENVIO_DADOS]");
			Genericos.gravarLinhaLog("NFSE: ", conteudo);

			string resposta = enviaConteudoParaAPI(conteudo, urlEnvio, tpConteudo);

			Genericos.gravarLinhaLog("NFSE: ", "[ENVIO_RESPOSTA]");
			Genericos.gravarLinhaLog("NFSE: ", resposta);

			return resposta;
		}

		public static string consultarStatusProcessamento(ConsStatusProcessamentoReq ConsStatusProcessamentoReq)
		{
			string urlConsulta;

			urlConsulta = Endpoints.NFSeConsStatusProcessamento;

			string json = JsonConvert.SerializeObject(ConsStatusProcessamentoReq);

			Genericos.gravarLinhaLog("NFSE: ", "[CONSULTA_DADOS]");
			Genericos.gravarLinhaLog("NFSE: ", json);

			string resposta = enviaConteudoParaAPI(json, urlConsulta, "json");

			Genericos.gravarLinhaLog("NFSE: ", "[CONSULTA_RESPOSTA]");
			Genericos.gravarLinhaLog("NFSE: ", resposta);
			return resposta;
		}

		public static string downloadDocumento(DownloadReq DownloadReq)
		{
			string urlDownload;

			urlDownload = Endpoints.NFSeDownload;

			string json = JsonConvert.SerializeObject(DownloadReq);

			Genericos.gravarLinhaLog("NFSE: ", "[DOWNLOAD_DADOS]");
			Genericos.gravarLinhaLog("NFSE: ", json);

			string resposta = enviaConteudoParaAPI(json, urlDownload, "json");

			string status;

			DownloadResp DownloadResp = new DownloadResp();
			DownloadResp = JsonConvert.DeserializeObject<DownloadResp>(resposta);
			status = DownloadResp.status;


			if (!status.Equals("200") & !status.Equals("100"))
			{
				Genericos.gravarLinhaLog("NFSE: ", "[DOWNLOAD_RESPOSTA]");
				Genericos.gravarLinhaLog("NFSE: ", resposta);
			}
			else
			{
				Genericos.gravarLinhaLog("NFSE: ", "[DOWNLOAD_STATUS]");
				Genericos.gravarLinhaLog("NFSE: ", status);
			}

			return resposta;
		}

		public static string downloadDocumentoESalvar(DownloadReq DownloadReq, string caminho, string nome, bool exibeNaTela = false)
		{
			string resposta = downloadDocumento(DownloadReq);
			DownloadResp DownloadResp = new DownloadResp();

			string status;

			DownloadResp = JsonConvert.DeserializeObject<DownloadResp>(resposta);
			status = DownloadResp.status;

			if (status.Equals("200") || status.Equals("100"))
			{
				try
				{
					if (!Directory.Exists(caminho)) Directory.CreateDirectory(caminho);
					if (!caminho.EndsWith(@"\")) caminho += @"\";
				}
				catch (IOException ex)
				{
					Genericos.gravarLinhaLog("NFSE: ", "[CRIAR_DIRETORIO]" + caminho);
					Genericos.gravarLinhaLog("NFSE: ", ex.Message);
					throw new Exception("Erro: " + ex.Message);
				}

				if (DownloadReq.tpDown.ToUpper().Contains("X"))
				{
					string xml = DownloadResp.xml;
					Genericos.salvarXML(xml, caminho, nome);
				}

				if (DownloadReq.tpDown.ToUpper().Contains("P"))
				{
					string pdf = DownloadResp.pdfDocumento;
					Genericos.salvarPDF(pdf, caminho, nome);

					if (exibeNaTela) Process.Start(new ProcessStartInfo(caminho + nome + ".pdf") { UseShellExecute = true });
				}
				else if (!DownloadReq.tpDown.ToUpper().Contains("X"))
				{
					MessageBox.Show("Ocorreu um erro, veja o retorno da API para mais informações");
				}
			}

			return resposta;
		}

		public static string emitirNFSeSincrono(string conteudo, string tpConteudo, string CNPJ, string tpDown, string tpAmb, string caminho, bool exibeNaTela = false, bool a3 = false)
		{
			string statusEnvio, statusConsulta, statusDownload, motivo, nsNRec, chNFSe, chDPS, cStat, nProt;
			string retorno, resposta;
			IList<string> erros = null;

			statusEnvio = "";
			statusConsulta = "";
			statusDownload = "";
			motivo = "";
			nsNRec = "";
			chDPS = "";
			chNFSe = "";
			cStat = "";
			nProt = "";

			Genericos.gravarLinhaLog("NFSE: ", "[EMISSAO_SINCRONA_INICIO]");

			resposta = emitirDocumento(conteudo, tpConteudo, CNPJ, a3);

			var EmitirRespNFSe = JsonConvert.DeserializeObject<EmitirRespNFSe>(resposta);
			statusEnvio = EmitirRespNFSe.status;

			if (statusEnvio.Equals("200") || statusEnvio.Equals("-6"))
			{
				nsNRec = EmitirRespNFSe.nsNRec;

				Thread.Sleep(Parametros.TEMPO_ESPERA);

				ConsStatusProcessamentoReqNFSe ConsStatusProcessamentoReqNFSe = new ConsStatusProcessamentoReqNFSe()
				{
					CNPJ = CNPJ,
					nsNRec = nsNRec,
					tpAmb = tpAmb
				};

				for (int i = 0; i < 3; i++)
				{
					resposta = consultarStatusProcessamento(ConsStatusProcessamentoReqNFSe);

					try
					{
						dynamic respDin = JsonConvert.DeserializeObject(resposta);

						if (respDin.status == -2 && respDin.erro != null && respDin.erro.cStat == 997)
						{
							Thread.Sleep(Parametros.TEMPO_ESPERA);
							continue;
						}
					}
					catch { }

					break;
				}

				var ConsStatusProcessamentoRespNFSe = JsonConvert.DeserializeObject<ConsStatusProcessamentoRespNFSe>(resposta);
				statusConsulta = ConsStatusProcessamentoRespNFSe.status;

				if (statusConsulta.Equals("200"))
				{
					cStat = ConsStatusProcessamentoRespNFSe.cStat;

					if (cStat.Equals("100") || cStat.Equals("150"))
					{
						chDPS = ConsStatusProcessamentoRespNFSe.chDPS;

						chNFSe = ConsStatusProcessamentoRespNFSe.chNFSe;

						motivo = ConsStatusProcessamentoRespNFSe.xMotivo;

						DownloadReqNFSe DownloadReqNFSe = new DownloadReqNFSe()
						{
							chDPS = chDPS,
							chNFSe = chNFSe,
							CNPJ = CNPJ,
							tpAmb = tpAmb,
							tpDown = tpDown
						};

						resposta = downloadDocumentoESalvar(DownloadReqNFSe, caminho, chNFSe + "-procNFSe", exibeNaTela);

						var DownloadRespNFSe = JsonConvert.DeserializeObject<DownloadRespNFSe>(resposta);

						statusDownload = DownloadRespNFSe.status;

						if (!statusDownload.Equals("200")) motivo = DownloadRespNFSe.motivo;
					}
					else
					{
						motivo = ConsStatusProcessamentoRespNFSe.xMotivo;
					}
				}
				else
				{
					motivo = ConsStatusProcessamentoRespNFSe.motivo;
					erros = ConsStatusProcessamentoRespNFSe.erros;
				}
			}
			else if (statusEnvio.Equals("-7"))
			{
				motivo = EmitirRespNFSe.motivo;
				nsNRec = EmitirRespNFSe.nsNRec;
			}
			else if (statusEnvio.Equals("-4"))
			{
				motivo = EmitirRespNFSe.motivo;

				try
				{
					erros = EmitirRespNFSe.erros;
				}
				catch { }

			}
			else if (statusEnvio.Equals("-9"))
			{
				motivo = EmitirRespNFSe.erro.xMotivo;
				cStat = EmitirRespNFSe.erro.cStat;
			}
			else
			{
				try
				{
					motivo = EmitirRespNFSe.motivo;
				}
				catch { motivo = EmitirRespNFSe.ToString(); }
			}

			EmitirSincronoRetNFSe EmitirSincronoRetNFSe = new EmitirSincronoRetNFSe()
			{
				statusEnvio = statusEnvio,
				statusConsulta = statusConsulta,
				statusDownload = statusDownload,
				cStat = cStat,
				chDPS = chDPS,
				chNFSe = chNFSe,
				nProt = nProt,
				motivo = motivo,
				nsNRec = nsNRec,
				erros = erros
			};

			retorno = JsonConvert.SerializeObject(EmitirSincronoRetNFSe);

			Genericos.gravarLinhaLog("NFSE: ", "[JSON_RETORNO]");
			Genericos.gravarLinhaLog("NFSE: ", retorno);
			Genericos.gravarLinhaLog("NFSE: ", "[EMISSAO_SINCRONA_FIM]");

			return retorno;
		}
	}
}
